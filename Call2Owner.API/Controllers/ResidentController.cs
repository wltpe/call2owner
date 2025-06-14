using AutoMapper;
using Call2Owner.DTO;
using Call2Owner.Models;
using Call2Owner.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using OfficeOpenXml.FormulaParsing.LexicalAnalysis;
using RestSharp;
using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using Utilities;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;

namespace Call2Owner.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ResidentController : ControllerBase
    {
        private readonly IMapper _mapper;
        private readonly DataContext _context;
        private readonly IConfiguration _configuration;
        private readonly EmailService _emailService;
        private readonly string EncryptionKey = "ABCabc123!@#hdgRHF1245KDnjkjfdsfdkv";
        private readonly ILogger<ResidentController> _logger;
        private readonly RestClient _client;

        public static string SanitizeBase64(string base64)
        {
            return base64.Replace(" ", "").Replace("-", "+").Replace("_", "/");
        }

        public ResidentController(DataContext context, IConfiguration configuration,
            IMapper mapper, EmailService emailService, ILogger<ResidentController> logger, RestClient client)
        {
            _mapper = mapper;
            _context = context ?? throw new ArgumentNullException(nameof(context));
            _configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
            _emailService = emailService ?? throw new ArgumentNullException(nameof(emailService));
            _logger = logger;
            _client = client;
        }

        [AllowAnonymous]
        [HttpPost("register-self-resident")]
        public async Task<IActionResult> SelfRegisterResident([FromBody] UserResidentDto dto)
        {
            if (dto == null || (dto.Email == null && dto.MobileNumber == null))
            {
                return BadRequest("Invalid request: Email or Mobile Number is required.");
            }

            OTPGenerator otpGenerator = new OTPGenerator();
            string otp = otpGenerator.GenerateOTP();

            var existingUser = await _context.User
                                             .FirstOrDefaultAsync(u => u.MobileNumber == dto.MobileNumber);

            if (existingUser != null)
            {
                // Update OTP for existing user
                existingUser.Otp = otp;
                existingUser.OtpExpireTime = DateTime.UtcNow.AddMinutes(5);
                existingUser.ResendOtpTime = DateTime.UtcNow.AddMinutes(2);
                existingUser.IsActive = true;
                existingUser.IsVerified = true;

                _context.User.Update(existingUser);
            }
            else
            {

                // Create new user
                var newUser = new User
                {
                    MobileNumber = dto.MobileNumber,
                    Otp = otp,
                    OtpExpireTime = DateTime.UtcNow.AddMinutes(5),
                    ResendOtpTime = DateTime.UtcNow.AddMinutes(2),
                    RoleId = Convert.ToInt32(UserRoles.Resident),
                    OtpValidatedOn = null,
                    IsActive = true,
                    IsVerified = true
                };

                await _context.User.AddAsync(newUser);
            }

            await _context.SaveChangesAsync();

            var message = $"Your one time password {otp} for WLTPE sign-in. Valid for 10 mins. Do not share your OTP with anyone - Yoke Payment.";
            await SendOtpAsync(dto.MobileNumber, message);

            return Ok(new { message = "OTP sent successfully!" });
        }

        [Authorize(Policy = Utilities.Module.Resident)]
        [Authorize(Policy = Utilities.Permission.Add)]
        [HttpPost("register-admin-resident")]
        public async Task<IActionResult> AdminRegisterResident([FromBody] UserResidentDto dto)
        {
            if (dto == null || (dto.Email == null && dto.MobileNumber == null))
            {
                return BadRequest("Invalid request: Email or Mobile Number is required.");
            }

            OTPGenerator otpGenerator = new OTPGenerator();
            string otp = otpGenerator.GenerateOTP();

            var existingUser = await _context.User
                                             .FirstOrDefaultAsync(u => u.MobileNumber == dto.MobileNumber);

            if (existingUser != null)
            {
                // Update OTP for existing user
                existingUser.Otp = otp;
                existingUser.OtpExpireTime = DateTime.UtcNow.AddMinutes(5);
                existingUser.ResendOtpTime = DateTime.UtcNow.AddMinutes(2);
                existingUser.IsActive = true;
                existingUser.IsVerified = true;

                _context.User.Update(existingUser);
            }
            else
            {

                // Create new user
                var newUser = new User
                {
                    MobileNumber = dto.MobileNumber,
                    Otp = otp,
                    OtpExpireTime = DateTime.UtcNow.AddMinutes(5),
                    ResendOtpTime = DateTime.UtcNow.AddMinutes(2),
                    RoleId = Convert.ToInt32(UserRoles.Resident),
                    OtpValidatedOn = null,
                    IsActive = true,
                    IsVerified = true
                };

                await _context.User.AddAsync(newUser);
            }

            await _context.SaveChangesAsync();

            var message = $"Your one time password {otp} for WLTPE sign-in. Valid for 10 mins. Do not share your OTP with anyone - Yoke Payment.";
            await SendOtpAsync(dto.MobileNumber, message);

            return Ok(new { message = "OTP sent successfully!" });
        }

        [Authorize(Policy = Utilities.Module.UserManagement)]
        [Authorize(Policy = Utilities.Permission.Add)]
        [Authorize]
        [AllowAnonymous]
        [HttpPost("register-super-admin-resident")]
        public async Task<IActionResult> SuperAdminRegisterResident([FromBody] UserResidentDto dto)
        {
            if (dto == null || (dto.Email == null && dto.MobileNumber == null))
            {
                return BadRequest("Invalid request: Email or Mobile Number is required.");
            }

            OTPGenerator otpGenerator = new OTPGenerator();
            string otp = otpGenerator.GenerateOTP();

            var existingUser = await _context.User
                                             .FirstOrDefaultAsync(u => u.MobileNumber == dto.MobileNumber);

            if (existingUser != null)
            {
                // Update OTP for existing user
                existingUser.Otp = otp;
                existingUser.OtpExpireTime = DateTime.UtcNow.AddMinutes(5);
                existingUser.ResendOtpTime = DateTime.UtcNow.AddMinutes(2);
                existingUser.IsActive = true;
                existingUser.IsVerified = true;

                _context.User.Update(existingUser);
            }
            else
            {

                // Create new user
                var newUser = new User
                {
                    MobileNumber = dto.MobileNumber,
                    Otp = otp,
                    OtpExpireTime = DateTime.UtcNow.AddMinutes(5),
                    ResendOtpTime = DateTime.UtcNow.AddMinutes(2),
                    RoleId = Convert.ToInt32(UserRoles.Resident),
                    OtpValidatedOn = null,
                    IsActive = true,
                    IsVerified = true
                };

                await _context.User.AddAsync(newUser);
            }

            await _context.SaveChangesAsync();

            var message = $"Your one time password {otp} for WLTPE sign-in. Valid for 10 mins. Do not share your OTP with anyone - Yoke Payment.";
            await SendOtpAsync(dto.MobileNumber, message);

            return Ok(new { message = "OTP sent successfully!" });
        }

        [AllowAnonymous]
        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginSelfDto model)
        {
            _logger.LogInformation("Login attempt with OTP for: {UserName}", model.UserName);

            var user = await _context.User
                .Include(u => u.Role)
                    .ThenInclude(r => r.RoleClaim)
                .FirstOrDefaultAsync(u => u.Email == model.UserName || u.MobileNumber == model.UserName);

            if (user == null)
                return Unauthorized(new { message = "Invalid user credentials." });

            if (!user.IsActive.GetValueOrDefault() || !user.IsVerified.GetValueOrDefault())
                return Unauthorized(new { message = "Account is not active or verified. Please contact support." });

            // ✅ OTP Validation
            if (user.Otp != model.OTP || user.OtpExpireTime < DateTime.UtcNow)
                return Unauthorized(new { message = "Invalid or expired OTP." });

            // ✅ Clear OTP after successful login (optional but recommended)
            user.Otp = null;
            user.OtpValidatedOn = DateTime.UtcNow;
            await _context.SaveChangesAsync();

            // ✅ Get RoleClaims
            var roleClaimValues = user.Role?.RoleClaim
                                    .OrderBy(rc => rc.Id)
                                    .Select(rc => rc.ModulePermissionsJson.ToString())
                                    .FirstOrDefault();

            // ✅ Generate JWT Token
            var token = GenerateJwtToken(user, roleClaimValues);

            var userDto = _mapper.Map<UserDto>(user);

            object insurerData = null; // Placeholder for future logic

            return Ok(new { token, role = user.Role?.RoleName, User = userDto, InsurerId = insurerData });
        }

        [AllowAnonymous]
        [HttpGet("get-all-country")]
        public async Task<ActionResult<IEnumerable<CountryDto>>> GetAllCountry()
        {
            var countries = await _context.Country
         .Where(s => s.IsDeleted != true && s.IsActive == true)
         .OrderBy(s => s.DisplayOrder)
         .ToListAsync();

            return Ok(_mapper.Map<List<CountryDto>>(countries));
        }

        [AllowAnonymous]
        [HttpGet("get-all-state-by-country-id")]
        public async Task<ActionResult<IEnumerable<StateDto>>> GetAllStateByCountryId(int CountryId)
        {
            var states = await _context.State
                .Where(s => s.IsDeleted != true && s.IsActive == true && s.CountryId == CountryId)
                .OrderBy(s => s.DisplayOrder)
                .ToListAsync();

            return Ok(_mapper.Map<List<StateDto>>(states));
        }

        [AllowAnonymous]
        [HttpGet("get-all-city-by-state-id")]
        public async Task<ActionResult<IEnumerable<CityDto>>> GetAllCityByStateId(int StateId)
        {
            var cities = await _context.City
                .Where(s => s.IsDeleted != true && s.IsActive == true && s.StateId == StateId)
                .OrderBy(s => s.DisplayOrder)
                .ToListAsync();

            return Ok(_mapper.Map<List<CityDto>>(cities));
        }

        [AllowAnonymous]
        [HttpGet("get-all-society-by-city-id")]
        public async Task<ActionResult<IEnumerable<SocietyDto>>> GetAllSocietyByCityId(int CityId)
        {
            var societies = await _context.Society
                .Where(s => s.IsDeleted != true && s.IsActive == true && s.CityId == CityId && s.IsApproved == true)
                .OrderBy(s => s.Name)
                .ToListAsync();

            return Ok(_mapper.Map<List<SocietyDto>>(societies));
        }

        [AllowAnonymous]
        [HttpGet("get-all-building-by-society-id")]
        public async Task<ActionResult<IEnumerable<SocietyBuildingDTO>>> GetAllBuildingBySocietyId(Guid SocietyId)
        {
            var societyBuildings = await _context.SocietyBuilding
                .Where(s => s.IsDeleted != true && s.IsActive == true && s.SocietyId == SocietyId)
                .OrderBy(s => s.Name)
                .ToListAsync();

            return Ok(_mapper.Map<List<SocietyBuildingDTO>>(societyBuildings));
        }

        [AllowAnonymous]
        [HttpGet("get-all-flats-by-society-building-id")]
        public async Task<ActionResult<IEnumerable<SocietyFlatDTO>>> GetAllFlatsBySocietyBuildingId(Guid SocietyBuildingId)
        {
            var societyBuildingFlats = await _context.SocietyFlat
                .Where(s => s.IsDeleted != true && s.IsActive == true && s.SocietyBuildingId == SocietyBuildingId)
                .OrderBy(s => s.Name)
                .ToListAsync();

            return Ok(_mapper.Map<List<SocietyFlatDTO>>(societyBuildingFlats));
        }

        [HttpGet("resident-types")]
        public async Task<IActionResult> GetResidentTypes(int EntityTypeId)
        {
            var result = await _context.EntityTypeDetail
       .Where(d => d.IsDeleted != true && d.IsActive == true && d.EntityTypeId == EntityTypeId)
       .OrderBy(d => d.Id)
       .GroupBy(d => new
       {
           d.EntityType.Id,
           d.EntityType.Name,
           d.EntityType.Label
       })
       .Select(group => new
       {
           id = group.Key.Id,
           name = group.Key.Name,
           label = group.Key.Label,
           details = group.Select(d => new
           {
               entityTypeDetailId = d.Id,
               entityTypeId = d.EntityTypeId,
               value = d.Value,
               label = d.Label
           }).ToList()
       })
       .ToListAsync();

            return Ok(result);
        }

        [HttpGet("resident-type-form-fields")]
        public async Task<IActionResult> GetResidentTypeFormFields(int EntityTypeDetailId)
        {
            var jsonString = await _context.EntityTypeDetail
       .Where(d => d.IsDeleted != true && d.IsActive == true && d.Id == EntityTypeDetailId)
       .Select(d => d.DetailJson)
       .FirstOrDefaultAsync();

            if (string.IsNullOrEmpty(jsonString))
                return NotFound();

            try
            {
                // Deserialize JSON to List<FormField>
                var options = new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true,
                };

                //   var formFields = System.Text.Json.JsonSerializer.Deserialize<List<FormField>>(jsonString, options);
                List<ResidentDocument> documents = System.Text.Json.JsonSerializer.Deserialize<List<ResidentDocument>>(jsonString);


                return Ok(documents);
            }
            catch (System.Text.Json.JsonException)
            {
                return BadRequest("Invalid JSON format.");
            }
        }


    //    [AllowAnonymous]
        [HttpPost("resident-update-form-fields")]
        public async Task<IActionResult> GetResidentTypeFormFields(ResidentUpdateFormFields obj)
        {
            var currentUserId = Convert.ToString(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "0");

            string extensionWithoutDot = null;
            int ResidentType_EntityTypeId = 3;
            if (obj.File != null)
            {

                 extensionWithoutDot = Path.GetExtension(obj.File.FileName)?.TrimStart('.');
            }

            var targetType = extensionWithoutDot;

            var getAllEntityTypeDetailByEntityTypeId = await _context.EntityTypeDetail
       .Where(d => d.IsDeleted != true && d.IsActive == true && d.EntityTypeId == ResidentType_EntityTypeId && d.Id == obj.EntityTypeDetailId)
       .FirstOrDefaultAsync();

            if (getAllEntityTypeDetailByEntityTypeId !=null)
            {
                    // nothing to do
            }
            else
            {
                return NotFound();
            }

                var jsonString = await _context.EntityTypeDetail
           .Where(d => d.IsDeleted != true && d.IsActive == true && d.Id == obj.EntityTypeDetailId)
           .FirstOrDefaultAsync();

            if (string.IsNullOrEmpty(jsonString.DetailJson))
                return NotFound();

            try
            {
                var options = new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true,
                };

                List<ResidentDocument>? documents = System.Text.Json.JsonSerializer.Deserialize<List<ResidentDocument>>(jsonString.DetailJson, options);
                bool IsFileUploadRequired = false;

                if (documents != null)
                {
      


                    var IsType = documents
                        .Where(doc => doc.Details != null && doc.Details.Any(detail =>
                            detail.Type != null))
                        .ToList();

                    if (IsType.Count > 0)
                    {
                        
                        //var matchingDocuments = documents
                        //    .Where(doc => doc.Details != null && doc.Details.Any(detail =>
                        //        detail.Type.Any(t => string.Equals(t, targetType, StringComparison.OrdinalIgnoreCase))
                        //    ))
                        //    .ToList();

                        var matchingDocuments = documents
    .Where(doc => doc.Details != null && doc.Details.Any(detail =>
        detail.Type != null && detail.Type.Any(t =>
            string.Equals(t, targetType, StringComparison.OrdinalIgnoreCase)
        )
    ))
    .ToList();

                        if (IsType.Count > 0 && matchingDocuments.Count == 0)
                        {
                            return NotFound();
                        }

                        if (matchingDocuments.Count > 0 )
                        {
                            //noting to do
                            IsFileUploadRequired = true;
                        }
                        else
                        {
                            return NotFound();
                        }

                    }
                    else
                    {
                        // nothing to do

                    }


                }
                else
                {
                    return BadRequest();
                }

                var blobName = "";
                if (IsFileUploadRequired == true)
                {

                    // Upload file
                    if (obj.File is not null)
                    {
                        blobName = await UploadFileOrImageFolderAsync(obj.File.FileName, obj.File.OpenReadStream());
                    }
                    else
                    {
                        blobName = null;
                    }
                }
                else
                {
                    blobName = null;
                }
                    Guid Id = Guid.NewGuid();

                Guid residentDocumentId = documents
                                            .Where(doc => doc.ResidentDocumentRequiredToRegisterId != null)
                                            .Select(doc => doc.ResidentDocumentRequiredToRegisterId)
                                            .FirstOrDefault();

                Guid parsedResidentId;
                if (!Guid.TryParse(currentUserId, out parsedResidentId))
                {
                    parsedResidentId = Guid.NewGuid(); // fallback
                }

                var residentDocumentUploaded = new ResidentDocumentUploaded()
                {
                    Id = Id,
                    ResidentId = parsedResidentId,
                  //  ResidentDocumentRequiredToRegisterId = residentDocumentId,
                    Name = obj.Name,
                  //  Url = blobName,
                    IsActive = true,
                    CreatedBy = currentUserId,
                    CreatedOn = DateTime.UtcNow,
                    IsDeleted = false
                };

                var userDetal = await AddResidentDocumentUploadedAsync(obj.EntityTypeDetailId, obj.SocietyFlatId, residentDocumentUploaded);

                if (userDetal == null)
                {
                    return Ok("Invalid or expired token");
                }
                else
                {

                    return Ok(userDetal);
                }

                return Ok(documents);
            }
            catch (System.Text.Json.JsonException)
            {
                return BadRequest("Invalid JSON format.");
            }
        }


        private string GenerateJwtToken(User user, string modulePermissions)
        {
            var jwtKey = _configuration["JwtSettings:Key"];
            var jwtIssuer = _configuration["JwtSettings:Issuer"];
            var jwtAudience = _configuration["JwtSettings:Audience"];
            var expiryMinutes = _configuration["JwtSettings:ExpiryMinutes"];

            if (string.IsNullOrEmpty(jwtKey) || string.IsNullOrEmpty(jwtIssuer) || string.IsNullOrEmpty(jwtAudience) || string.IsNullOrEmpty(expiryMinutes))
                throw new InvalidOperationException("JWT settings are not configured properly.");

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey));
            var tokenHandler = new JwtSecurityTokenHandler();

            var claims = new List<Claim>
                {
                    new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                    new Claim(ClaimTypes.Email, user.Email),
                    new Claim(ClaimTypes.Role, user.Role?.Id.ToString() ?? "0"),
                    new Claim("FirstName", user.FirstName ?? ""),
                    new Claim("LastName", user.LastName ?? ""),
                    new Claim("MobileNumber", user.MobileNumber ?? ""),
                    new Claim("UserId", user.Id.ToString()),
                    new Claim("RoleName", user.Role?.RoleName ?? "User")
                };

            claims.Add(new Claim("Permissions", string.IsNullOrWhiteSpace(modulePermissions) ? "" : modulePermissions));

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(claims),
                Expires = DateTime.UtcNow.AddMinutes(int.Parse(expiryMinutes)),
                Issuer = jwtIssuer,
                Audience = jwtAudience,
                SigningCredentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256)
            };

            var token = tokenHandler.CreateToken(tokenDescriptor);
            return tokenHandler.WriteToken(token);
        }

        private async Task<string> SendOtpAsync(string number, string message)
        {
            try
            {
                HttpClient client = new HttpClient();

                HttpResponseMessage response = await client.GetAsync(
                    $"https://sms.shreetripada.com/api/sendapi.php?auth_key=3515HOtE6VZwXu51ewmgrO&mobiles={number}&message={message}&sender=YKPYMT&templateid=1007843886982450229");
                response.EnsureSuccessStatusCode();
                string responseBody = await response.Content.ReadAsStringAsync();
                return responseBody;
            }
            catch (HttpRequestException e)
            {
                Console.WriteLine("\nException Caught!");
                Console.WriteLine("Message :{0} ", e.Message);
                return null;
            }
        }

        private async Task<string> UploadFileOrImageFolderAsync(string blobName, Stream imageStream)
        {
            try
            {
                // Ensure blobName is sanitized to prevent path traversal attacks
                if (string.IsNullOrWhiteSpace(blobName))
                {
                    throw new ArgumentException("Invalid blob name", nameof(blobName));
                }

                // Define the IIS folder path
                string folderPath = Path.Combine(Directory.GetCurrentDirectory(), "Images", "Resident", "Documents");

                // Ensure the folder exists
                if (!Directory.Exists(folderPath))
                {
                    Directory.CreateDirectory(folderPath);
                }

                // Combine the folder path and file name to create the full file path
                string filePath = Path.Combine(folderPath, blobName);


                // Save the file stream to the IIS folder
                using (var fileStream = new FileStream(filePath, FileMode.Create, FileAccess.Write))
                {
                    await imageStream.CopyToAsync(fileStream);
                }

                // Generate the URL to access the uploaded file
                //  string fileUrl = $"https://api.wltpe.com/wltpe_images/images/profile-images/{blobName}";

                //Local Path
                string fileUrl = $"C://Users//WLTPE//source//repos//c2o//Call2Owner.API//Images//Resident//Documents/{blobName}";


                return fileUrl; // Return the accessible file URL
            }
            catch (Exception ex)
            {
                // Log and rethrow the exception
                throw new InvalidOperationException("An error occurred while uploading the file/image.", ex);
            }
        }
        private async Task<ResidentDocumentUploaded?> AddResidentDocumentUploadedAsync(int EntityTypeDetailId, Guid SocietyFlatId, ResidentDocumentUploaded obj)
        {
            // Check if a record already exists
            var userExists = await _context.ResidentDocumentUploaded.FirstOrDefaultAsync(x =>
                                    x.ResidentId == obj.ResidentId
                                  //  && x.ResidentDocumentRequiredToRegisterId == obj.ResidentDocumentRequiredToRegisterId
                                    && x.IsDeleted != true
                                    && x.IsActive == true);

            if (userExists == null)
            {
               

                var updateResident = await _context.Resident.FirstOrDefaultAsync(x => 
                x.UserId == obj.ResidentId && x.IsApproved == false && x.IsDocumentUploaded == false);

               

                if (updateResident !=null)
                {
                    obj.ResidentId = updateResident.Id;

                    // Directly add the obj to the context
                    await _context.ResidentDocumentUploaded.AddAsync(obj);
                    
                    updateResident.IsDocumentUploaded = true;
                    updateResident.EntityTypeDetailId = EntityTypeDetailId;
                    updateResident.SocietyFlatId = SocietyFlatId;
                    updateResident.UpdatedBy = obj.CreatedBy;
                    updateResident.UpdatedOn = DateTime.UtcNow;

                     _context.Resident.Update(updateResident);

                }

                // Save changes to the database
                await _context.SaveChangesAsync();

                return obj;
            }
            else
            {
                // Return null if the record exists
                return userExists;
            }
        }

    }

    public class FormField
    {

        [JsonPropertyName("fieldText")]
        public string? FieldText { get; set; }

        [JsonPropertyName("fieldType")]
        public string? FieldType { get; set; }

        [JsonPropertyName("required")]
        public bool? Required { get; set; }

        [JsonPropertyName("isActive")]
        public bool? IsActive { get; set; }

        [JsonPropertyName("fieldId")]
        public string? FieldId { get; set; }

        [JsonPropertyName("fieldName")]
        public string? FieldName { get; set; }

        [JsonPropertyName("min")]
        public DateTime? Min { get; set; }

        [JsonPropertyName("type")]
        public List<string>? Type { get; set; }

    }
    public class ResidentDocument
    {
        [JsonPropertyName("ResidentDocumentRequiredToRegisterId")]
        public Guid ResidentDocumentRequiredToRegisterId { get; set; }

        [JsonPropertyName("Details")]
        public List<FormField>? Details { get; set; }
    }

    public class ResidentUpdateFormFields {
        public int EntityTypeId { get; set; }
        public int EntityTypeDetailId { get; set; }
        public Guid SocietyFlatId { get; set; }
        public string Name { get; set; }
        public string? FileOrImage { get; set; }
        // public string? Address { get; set; }
        public IFormFile? File { get; set; }
    }

}