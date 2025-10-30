using Call2Owner;
using Call2Owner.API;
using Call2Owner.Controllers;
using Call2Owner.Models;
using Call2Owner.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.FileProviders;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using RestSharp;
using System.Reflection;
using System.Security.Claims;
using System.Text;
using Utilities;

var publicEndpoints = new HashSet<string>
{
    "/swagger/index.html",
    "/swagger/docs/v1/apigateway",
    "/swagger/v1/swagger.json",
    "/api/auth/login",
    "/api/auth/resend-verification-email",
    "/api/auth/send-reset-link",
    "/api/auth/set-password",
    "/api/RoleClaims/roles-permissions",
    "/api/auth/getAllUsers"
};

var builder = WebApplication.CreateBuilder(args);

// ✅ Application Insights
builder.Services.AddApplicationInsightsTelemetry(options =>
{
    builder.Configuration.Bind("ApplicationInsights", options);
});

// ✅ CORS Configuration
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowOrigin", policy =>
    {
        policy.WithOrigins("https://localhost:44351", "http://112.196.3.222:8081")
              .AllowAnyHeader()
              .AllowAnyMethod();
    });
});

// ✅ Database
builder.Services.AddDbContext<DataContext>(options =>
{
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"));
});

// ✅ Mail Config
builder.Services.Configure<MailSettings>(builder.Configuration.GetSection("MailSettings"));

// ✅ JWT Configuration
var key = Encoding.UTF8.GetBytes(builder.Configuration["JwtSettings:Key"]
    ?? throw new InvalidOperationException("JWT Secret Key not found!"));

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = false,
            ValidateAudience = false,
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(key),
            ValidIssuer = builder.Configuration["JwtSettings:Issuer"],
            ValidAudience = builder.Configuration["JwtSettings:Audience"],
            ValidateLifetime = true,
            ClockSkew = TimeSpan.Zero
        };

        options.MapInboundClaims = false;

        options.Events = new JwtBearerEvents
        {
            OnTokenValidated = ctx =>
            {
                var permissions = ctx.Principal?.FindFirst("Permissions")?.Value;

                if (permissions == null &&
                    ctx.SecurityToken is System.IdentityModel.Tokens.Jwt.JwtSecurityToken jwtToken &&
                    jwtToken.Payload.TryGetValue("Permissions", out var permsObj))
                {
                    var identity = ctx.Principal!.Identity as ClaimsIdentity;
                    identity?.AddClaim(new Claim("Permissions", permsObj.ToString()!));
                }

                return Task.CompletedTask;
            }
        };
        options.SaveToken = true;
    });

// ✅ Custom Services
builder.Services.AddScoped<EmailService>();
builder.Services.AddScoped<NotificationService>(); // ✅ Add NotificationService

builder.Services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();

// ✅ Authorization setup
builder.Services.AddAuthorization(options =>
{
    var modules = typeof(Utilities.Module)
        .GetFields(BindingFlags.Public | BindingFlags.Static | BindingFlags.FlattenHierarchy)
        .Where(fi => fi.IsLiteral && !fi.IsInitOnly)
        .Select(fi => fi.GetValue(null)?.ToString())
        .Where(value => !string.IsNullOrEmpty(value))
        .ToList();

    foreach (var item in modules)
        options.AddPolicy(item, policy => policy.Requirements.Add(new PermissionRequirement(item)));

    var permissions = typeof(Utilities.Permission)
        .GetFields(BindingFlags.Public | BindingFlags.Static | BindingFlags.FlattenHierarchy)
        .Where(fi => fi.IsLiteral && !fi.IsInitOnly)
        .Select(fi => fi.GetValue(null)?.ToString())
        .Where(value => !string.IsNullOrEmpty(value))
        .ToList();

    foreach (var permission in permissions)
        options.AddPolicy(permission, policy => policy.Requirements.Add(new PermissionRequirement(permission)));
});

// ✅ Swagger Config
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "Call2Owner.API", Version = "v1" });
    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Name = "Authorization",
        Type = SecuritySchemeType.Http,
        Scheme = "Bearer",
        BearerFormat = "JWT",
        In = ParameterLocation.Header,
        Description = "Enter 'Bearer {token}' (without quotes)"
    });
    c.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                { Type = ReferenceType.SecurityScheme, Id = "Bearer" }
            },
            Array.Empty<string>()
        }
    });
});

builder.Services.AddScoped<IAuthorizationHandler, PermissionAuthorizationHandler>();
builder.Services.AddSingleton<IEmailServiceClient, EmailServiceClient>();
builder.Services.AddScoped<IEmailTriggerService, EmailTriggerService>();
builder.Services.AddSingleton(sp => new RestClient());

// ✅ Build app
var app = builder.Build();

// ✅ Initialize Firebase safely
try
{
    FirebaseInitializer.InitializeFirebase();
    Console.WriteLine("✅ Firebase initialized successfully.");
}
catch (Exception ex)
{
    Console.WriteLine("❌ Firebase initialization failed: " + ex.Message);
}

// ✅ Seed initial data
using (var scope = app.Services.CreateScope())
{
    var context = scope.ServiceProvider.GetRequiredService<DataContext>();
    DbSeeder.SeedIfNotExists(context);
}

// ✅ CORS middleware
app.UseCors(options => options.AllowAnyOrigin().AllowAnyHeader().AllowAnyMethod());

// ✅ Swagger setup
if (app.Environment.IsDevelopment() || app.Environment.IsProduction())
{
    app.UseCors("AllowOrigin");
    app.UseSwagger();
    app.UseSwaggerUI();
}

// ✅ Seed Super Admin
using (var scope = app.Services.CreateScope())
{
    var dbContext = scope.ServiceProvider.GetRequiredService<DataContext>();
    await AuthController.SeedSuperAdminAsync(dbContext);
}

// ✅ Middleware
app.UseHttpsRedirection();
app.UseStaticFiles(new StaticFileOptions
{
    FileProvider = new PhysicalFileProvider(Path.Combine(Directory.GetCurrentDirectory(), "Images")),
    RequestPath = "/Images"
});

app.UseAuthentication();
app.UseAuthorization();

// ✅ Public endpoint bypass
app.Use(async (context, next) =>
{
    var path = context.Request.Path.Value?.ToLower();
    if (publicEndpoints.Contains(path))
    {
        context.User = new ClaimsPrincipal(new ClaimsIdentity());
        await next();
        return;
    }
    await next();
});

app.MapControllers();
app.Run();
