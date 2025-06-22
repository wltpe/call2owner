using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using OfficeOpenXml.FormulaParsing.LexicalAnalysis;
using Call2Owner.DTO;
using Call2Owner.Models;
using Call2Owner.Services;
using RestSharp;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using Utilities;

namespace Call2Owner.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserAuthController : ControllerBase
    {
        private readonly IMapper _mapper;
        private readonly DataContext _context;
        private readonly IConfiguration _configuration;
        private readonly EmailService _emailService;
        private readonly string EncryptionKey = "ABCabc123!@#hdgRHF1245KDnjkjfdsfdkv";
        private readonly ILogger<AuthController> _logger;
        private readonly RestClient _client;

        public static string SanitizeBase64(string base64)
        {
            return base64.Replace(" ", "").Replace("-", "+").Replace("_", "/");
        }

        public UserAuthController(DataContext context, IConfiguration configuration,
            IMapper mapper, EmailService emailService, ILogger<AuthController> logger, RestClient client)
        {
            _mapper = mapper;
            _context = context ?? throw new ArgumentNullException(nameof(context));
            _configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
            _emailService = emailService ?? throw new ArgumentNullException(nameof(emailService));
            _logger = logger;
            _client = client;
        }

        private byte[] GetAesKey()
        {
            using (SHA256 sha256 = SHA256.Create())
            {
                return sha256.ComputeHash(Encoding.UTF8.GetBytes(EncryptionKey));
            }
        }

        [Authorize(Policy = Utilities.Module.UserManagement)]
        [Authorize(Policy = Utilities.Permission.Add)]
        [Authorize]
        [AllowAnonymous]
        [HttpPost("resident-self-register")]
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

            Guid Username = Guid.NewGuid();

            if (existingUser != null)
            {
                return Ok(new { message = "Resident already registered."});
            }
            else
            {

                // Create new user
                var newUser = new User
                {
                    Id = Username,
                    MobileNumber = dto.MobileNumber,
                    FirstName=dto.FirstName,
                    LastName = dto.LastName,
                    Email = dto.Email,
                    Otp = otp,
                    OtpExpireTime = DateTime.UtcNow.AddMinutes(5),
                    ResendOtpTime = DateTime.UtcNow.AddMinutes(2),
                    RoleId = Convert.ToInt32(UserRoles.Resident),
                    OtpValidatedOn = null,
                    IsActive = true,
                    IsVerified = false,
                    CreatedBy=Username.ToString(),
                    CreatedOn=DateTime.UtcNow
                };

                // Add User as Resident
                var AddResident = new Resident
                {
                    Id = Guid.NewGuid(),
                    UserId = newUser.Id,
                    IsDocumentUploaded = false,
                    IsApproved = false,
                    IsActive = true,
                    CreatedBy = Username.ToString(),
                    CreatedOn = DateTime.UtcNow
                };

                await _context.User.AddAsync(newUser);
                await _context.Resident.AddAsync(AddResident);
            }

            await _context.SaveChangesAsync();

            var message = $"Your one time password to {otp} into WLTPE is sign-in. Valid for 10 mins.Do not share your OTP with anyone-Yoke payment";
           
            await SendOtpAsync(dto.MobileNumber, message);

            return Ok(new { message = "OTP sent successfully!" });
        }

        [Authorize(Policy = Utilities.Module.UserManagement)]
        [Authorize(Policy = Utilities.Permission.Add)]
        [Authorize]
        [AllowAnonymous]
        [HttpPost("resident-login")]
        public async Task<IActionResult> LoginResident([FromBody] LoginResidentDto dto)
        {
            if (dto.Email == null && dto.MobileNumber == null)
            {
                return BadRequest("Invalid request: Email or Mobile Number is required.");
            }

            OTPGenerator otpGenerator = new OTPGenerator();
            string otp = otpGenerator.GenerateOTP();



            var existingUser = await _context.User
                                             .FirstOrDefaultAsync(u => u.MobileNumber == dto.MobileNumber || u.Email == dto.Email);

            if (existingUser == null)
            {
                return BadRequest("User not exist.");
            }

            // Update OTP for existing user
            existingUser.Otp = otp;
            existingUser.OtpExpireTime = DateTime.UtcNow.AddMinutes(5);
            existingUser.ResendOtpTime = DateTime.UtcNow.AddMinutes(2);
            existingUser.IsActive = true;
            existingUser.IsVerified = true;
            existingUser.UpdatedBy = existingUser.Id.ToString();
            existingUser.UpdatedOn = DateTime.UtcNow;


            _context.User.Update(existingUser);
            await _context.SaveChangesAsync();

            var message = $"Your one time password to {otp} into WLTPE is sign-in. Valid for 10 mins.Do not share your OTP with anyone-Yoke payment";

            await SendOtpAsync(existingUser.MobileNumber, message);

            return Ok(new { message = "OTP sent to your registered mobile number!" });
        }

        [AllowAnonymous]
        [HttpPost("resident-verify-otp")]
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
        
        [HttpPost("resident/approve")]
        public async Task<IActionResult> ResidentApprove([FromBody] ResidentApprovalDto model)
        {
            var currentUserId = Convert.ToString(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "0");

            var resident = await _context.Resident.FindAsync(model.ResidentId);
            if (resident == null || resident.IsDeleted == true)
                return NotFound();

            if (resident.IsApproved == true)
                return NotFound(new { message = "Already Approved" });

            resident.IsApproved = model.IsApproved;
            resident.ApprovedBy = currentUserId;
            resident.ApprovedComment = model.ApprovedComment;
            resident.ApprovedOn = DateTime.UtcNow;

            await _context.SaveChangesAsync();

            var obj = _mapper.Map<UpdateResidentDto>(resident);

            if (obj != null)
            {
                obj.ResidentType = JsonConvert.DeserializeObject<UploadSelectedRecord>(obj.DetailJson);
                return Ok(obj);
            }
            else
            {
                return NotFound(new { message = "Not found" });
            }
             
        }

        [HttpPost("society/approve")]
        public async Task<IActionResult> SocietyApprove([FromBody] SocietyApprovalDto model)
        {
            var currentUserId = Convert.ToString(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "0");

            var society = await _context.Society.FindAsync(model.SocietyId);
            if (society == null || society.IsDeleted == true)
                return NotFound(new { message = "Not found" });

            if (society.IsApproved == true)
                return NotFound(new { message = "Already Approved" });

            society.IsApproved = model.IsApproved;
            society.ApprovedBy = currentUserId;
            society.ApprovedComment = model.ApprovedComment;
            society.ApprovedOn = DateTime.UtcNow;

            await _context.SaveChangesAsync();

            var obj = _mapper.Map<UpdateSocietyDto>(society);

            if (obj != null)
            {
                obj.SocietyDocument = JsonConvert.DeserializeObject<SocietyUploadSelectedRecord>(obj.DetailJson);
                return Ok(obj);
            }
            else
            {
                return NotFound(new { message = "Not found" });
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
    }

    public class OTPGenerator
    {
        private Random random = new Random();
        private List<string> last10OTPs = new List<string>();

        public string GenerateOTP()
        {
            string otp;

            do
            {
                // Generate a random 6-digit number
                int otpNumber = random.Next(100000, 999999);

                // Convert the number to a string
                otp = otpNumber.ToString();
            } while (last10OTPs.Contains(otp)); // Ensure the OTP is not in the last 10 generated OTPs

            // Add the new OTP to the list and remove the oldest OTP if the list exceeds 10 items
            last10OTPs.Add(otp);
            if (last10OTPs.Count > 10)
            {
                last10OTPs.RemoveAt(0);
            }

            return otp;
        }
    }

    }