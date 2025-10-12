using AutoMapper;
using Call2Owner.DTO;
//using Call2Owner.Model;
using Call2Owner.Models;
using Call2Owner.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.Metadata;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using OfficeOpenXml.FormulaParsing.LexicalAnalysis;
using RestSharp;
using System;
using System.IdentityModel.Tokens.Jwt;
using System.Reflection;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using Utilities;

namespace Call2Owner.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
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

        public AuthController(DataContext context, IConfiguration configuration,
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

        #region Public Methods

        [Authorize(Policy = Utilities.Module.Society)]
        //[Authorize(Policy = Utilities.Permission.AddUser)]
        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] UserDto model)
        {
            var currentUserId = Convert.ToString(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "0");

            if (currentUserId == "0")
                return Unauthorized(new { message = "Invalid user." });

            var currentUser = await _context.User.Include(u => u.Role).FirstOrDefaultAsync(u => u.UserName.ToString() == currentUserId);
            if (currentUser == null)
                return Unauthorized(new { message = "User not found or unauthorized." });

            var token = HttpContext.Request.Headers["Authorization"].ToString();
            if (string.IsNullOrEmpty(token))
            {
                Console.WriteLine("Missing Authorization Token");
                return Unauthorized("Missing Authorization Token");
            }

            token = token.Replace("Bearer ", "");
            var handler = new JwtSecurityTokenHandler();
            var jwtToken = handler.ReadJwtToken(token);
            var roleId = jwtToken.Claims.FirstOrDefault(c => c.Type == "role")?.Value;

            if (currentUser.Roles.Id == Convert.ToInt32(UserRoles.Admin))
            {
                // Forcefully assign InsurerCustomer role, no need for client to send it
                model.RoleId = Convert.ToInt32(UserRoles.SocietyAdmin);
            }

            if (await _context.User.AnyAsync(u => u.Email == model.Email || u.PhoneNumber == model.MobileNumber))
                return BadRequest(new { message = "Email / Phone number already exists!" });

            // Ensure that the new user cannot have the same role as the current user
            if (currentUser.Roles.Id == model.RoleId)
                return BadRequest(new { message = "You cannot assign the same role as yours." });
            // Validate whether the current user can assign the requested role
            string role = GetUserRoleFromToken();

            var validChildRole = await _context.Role.AnyAsync(r => r.Id == model.RoleId && r.ParentRoleId == currentUser.Roles.Id);
            if (!validChildRole && role != UserRoles.SuperAdmin)
                return Forbid("You do not have permission to assign this role.");

            OTPGenerator otpGenerator = new OTPGenerator();
            string verificationCode = otpGenerator.GenerateOTP();

            Guid Username = Guid.NewGuid();
            var password = "";
            var message = "";

            var user = new User
            {
                UserName = Username,
                FirstName = model.FirstName,
                LastName = model.LastName,
                Email = model.Email,
                PhoneNumber = model.MobileNumber,
                VerificationCode = verificationCode,
                VerificationCodeGenerationTime = DateTime.UtcNow,
                IsActive = true,
                IsDeleted = false,
                IsVerified = false,
                CreatedBy = currentUserId,
                CreatedOn = DateTime.UtcNow
            };

            if (model.UsePassword.HasValue && model.UsePassword.Value == true)
            {
                password = PasswordGenerator.GeneratePassword();
                user.PasswordHash = BCrypt.Net.BCrypt.HashPassword(password);
                message = $"Your one time password to {password} into C2O is sign-in. Valid for 10 mins.Do not share your OTP with anyone";
            }
            else
            {
                message = $"Your one time password to {verificationCode} into C2O is sign-in. Valid for 10 mins.Do not share your OTP with anyone";
            }

            await _context.User.AddAsync(user);
            await _context.SaveChangesAsync();

            await SendOtpAsync(model.MobileNumber, message);

            return Ok(new { message = "User registered successfully! Check your email to set a password." });
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


        [Authorize(Policy = Utilities.Module.UserManagement)]
        [Authorize(Policy = Utilities.Permission.Add)]
        [HttpPost("society/user/register")]

        public async Task<IActionResult> SocietyUserRegister([FromBody] SocietyUserDto model)
        {
            var currentUserId = Convert.ToString(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "0");

            if (currentUserId == "0")
                return Unauthorized(new { message = "Invalid user." });

            var currentUser = await _context.User.Include(u => u.Role).FirstOrDefaultAsync(u => u.UserName.ToString() == currentUserId);
            if (currentUser == null)
                return Unauthorized(new { message = "User not found or unauthorized." });

            var token = HttpContext.Request.Headers["Authorization"].ToString();
            if (string.IsNullOrEmpty(token))
            {
                return Unauthorized(new { message = "Missing Authorization Token" });
            }

            token = token.Replace("Bearer ", "");
            var handler = new JwtSecurityTokenHandler();
            var jwtToken = handler.ReadJwtToken(token);
            var roleId = jwtToken.Claims.FirstOrDefault(c => c.Type == "role")?.Value;

            var email = model.Email.Trim().ToLower();
            var mobile = model.MobileNumber.Trim();

            if (string.IsNullOrWhiteSpace(email))
            {
                email = null;
            }

            if (email == null)
            {
                if (await _context.User.AnyAsync(u =>  u.PhoneNumber == mobile))
                {
                    return BadRequest(new { message = "Mobile Number or Email already exists!" });
                }
            }
            else if (await _context.User.AnyAsync(u => u.Email.ToLower() == email || u.PhoneNumber == mobile))
            {
                return BadRequest(new { message = "Mobile Number or Email already exists!" });
            }


            // Ensure that the new user cannot have the same role as the current user
            if (currentUser.Roles.Id == model.RoleId)
                return BadRequest(new { message = "You cannot assign the same role as yours." });
            // Validate whether the current user can assign the requested role
            string role = GetUserRoleFromToken();

            var validChildRole = await _context.Role.AnyAsync(r => r.Id == model.RoleId && r.ParentRoleId == currentUser.Roles.Id);
            if (!validChildRole && (role != UserRoles.SuperAdmin || role != UserRoles.Admin))
                return Forbid("You do not have permission to assign this role.");

            var verificationCode = Guid.NewGuid().ToString();

            string encryptedEmail = Encrypt(model.Email);
            string encryptedToken = Encrypt(verificationCode);


            string resetLink = $"https://www.call2owner.com/set-password?{encryptedToken}&&{encryptedEmail}";

            Guid Username = Guid.NewGuid();

            bool IsVerified = true;
            bool IsApproved = true;
            bool IsDocumentRequired = false;
            string? ApprovedBy = currentUserId;
            DateTime? ApprovedOn = DateTime.UtcNow;

            if (model.IsDocumentRequired == true)
            {
                IsVerified = false;
                IsApproved = false;
                ApprovedBy = null;
                ApprovedOn = null;
                IsDocumentRequired = true;
            }

            var user = new User
            {
                UserName = Username,
                FirstName = model.FirstName,
                LastName = model.LastName,
                Email = email,
                PhoneNumber = mobile,
                PasswordHash = BCrypt.Net.BCrypt.HashPassword(model.Password),
                VerificationCode = verificationCode,
                VerificationCodeGenerationTime = DateTime.UtcNow,
                IsActive = true,
                IsDeleted = false,
                IsVerified = IsVerified,
                CreatedBy = currentUserId,
                CreatedOn = DateTime.UtcNow
                //     ResetLink = resetLink
            };

            await _context.User.AddAsync(user);

            // Add User as Resident
            var AddSocietyUser = new SocietyUser
            {
                Id = Guid.NewGuid(),
                Username = user.UserName,
                SocietyId = model.SocietyId,
                IsApproved = IsApproved,
                ApprovedOn = ApprovedOn,
                ApprovedBy = ApprovedBy,
                IsDocumentRequired = IsDocumentRequired,
                IsActive = true,
                CreatedBy = Username.ToString(),
                CreatedOn = DateTime.UtcNow
            };

            await _context.SocietyUser.AddAsync(AddSocietyUser);

            await _context.SaveChangesAsync();

            if (user.Roles.Id == Convert.ToInt32(UserRoles.Admin))
            {
                var insurerToSendDTO = new InsurerUserDTO
                {
                    UserId = user.UserName,
                    InsurerId = Guid.Parse(currentUserId),
                    IsActive = user.IsActive,
                    IsDeleted = user.IsDeleted.Value
                };
            }

            return Ok(new { message = "User registered successfully!" });
        }

        [Authorize(Policy = Utilities.Module.UserManagement)]
        //[Authorize(Policy = Utilities.Permission.GetById)]
        [Authorize] // Ensure only authenticated users can access this
        [HttpGet("currentUserDetail")]
        public async Task<IActionResult> GetCurrentUser()
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier); // Extract user ID from token
            if (userIdClaim == null)
                return Unauthorized(new { message = "Invalid token or user not found." });

            Guid userId = new Guid(userIdClaim.Value);

            var user = await _context.User
                .Include(u => u.Role)
                .FirstOrDefaultAsync(u => u.UserName == userId);

            if (user == null)
                return NotFound(new { message = "User not found." });

            var userDto = new UserDto
            {
                FirstName = user.FirstName,
                LastName = user.LastName,
                Email = user.Email,
                MobileNumber = user.PhoneNumber,
                RoleId = user.Roles.Id
            };

            return Ok(new
            {
                UserId = user.UserName,
                Token = Request.Headers["Authorization"].ToString().Replace("Bearer ", "") // Send token
            });
        }

        //[EmailTrigger(Utilities.Module.UsersManagement, Utilities.Permission.ResendVerificationEmail)]
        [HttpPost("resend-verification-email")]
        public async Task<IActionResult> ResendVerificationEmail([FromBody] ResendEmailRequest request)
        {
            if (string.IsNullOrWhiteSpace(request.MobileNumber))
                return BadRequest("Email is required.");

            var user = await _context.User
                .FirstOrDefaultAsync(u => u.PhoneNumber == request.MobileNumber);

            if (user == null)
                return NotFound("User not found.");

            if (user.IsVerified == true)
                return BadRequest("Email / Mobile Number is already verified.");

            OTPGenerator otpGenerator = new OTPGenerator();
            string verificationCode = otpGenerator.GenerateOTP();

            user.VerificationCode = verificationCode;
            user.VerificationCodeGenerationTime = DateTime.UtcNow;
            await _context.SaveChangesAsync();

            var password = PasswordGenerator.GeneratePassword();
            user.PasswordHash = BCrypt.Net.BCrypt.HashPassword(password);
            var message = $"Your one time password to {password} into C2O is sign-in. Valid for 10 mins.Do not share your OTP with anyone";

            await _context.User.AddAsync(user);
            await _context.SaveChangesAsync();

            await SendOtpAsync(request.MobileNumber, message);

            return Ok("Verification email has been resent.");
        }

        [AllowAnonymous]
        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginDto model)
        {
            var user = await _context.User
                .Include(u => u.Roles)
                    .ThenInclude(r => r.RoleClaim) // Include RoleClaims under Role
                .FirstOrDefaultAsync(u => u.Email == model.UserName || u.PhoneNumber == model.UserName);

            if (user == null || !user.IsActive || !user.IsVerified.Value)
                return Unauthorized(new { message = "Account is not active or verified. Please reset your password." });

            if (!BCrypt.Net.BCrypt.Verify(model.Password, user.PasswordHash))
                return Unauthorized(new { message = "Invalid Password!" });

            var roleClaimValues = user.Roles.RoleClaim
                                .OrderBy(rc => rc.Id)
                                .Select(rc => rc.ModulePermissionsJson.ToString())
                                .FirstOrDefault();

            var token = GenerateJwtToken(user, roleClaimValues);

            UserDto userDto = _mapper.Map<UserDto>(user); // Convert to DTOs

            return Ok(new { token, role = user.Roles?.RoleName, User = userDto });
        }

        [Authorize(Policy = Utilities.Module.UserManagement)]
        //[Authorize(Policy = Utilities.Permission.GetById)]
        [HttpGet("profile")]
        [Authorize]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public IActionResult GetUserProfile()
        {
            var userId = User.FindFirst("UserId")?.Value;
            var firstName = User.FindFirst("FirstName")?.Value;
            var lastName = User.FindFirst("LastName")?.Value;
            var role = User.FindFirst(ClaimTypes.Role)?.Value;

            if (string.IsNullOrEmpty(userId) || string.IsNullOrEmpty(firstName) || string.IsNullOrEmpty(lastName) || string.IsNullOrEmpty(role))
                return Unauthorized(new { message = "User claims not found!" });

            return Ok(new
            {
                UserId = userId,
                FirstName = firstName,
                LastName = lastName,
                Role = role
            });
        }

        [HttpPost("send-reset-link")]
        public async Task<IActionResult> SendResetLink([FromQuery] string email)
        {
            var user = await _context.User.FirstOrDefaultAsync(u => u.Email == email);
            if (user == null)
                return BadRequest(new { message = "User not found." });

            var token = Guid.NewGuid().ToString();
            user.VerificationCode = token;
            user.VerificationCodeGenerationTime = DateTime.UtcNow;
            await _context.SaveChangesAsync();

            string encryptedEmail = Encrypt(email);
            string encryptedToken = Encrypt(token);
            var resetLink = $"https://geneinsure.kindlebit.com/api/auth/set-password?{encryptedToken}&&{encryptedEmail}";
            //var resetLink = $"http://outriskappback.kindlebit.net/set-password?{encryptedToken}&&{encryptedEmail}";

            string emailBody = $@"
                <h2>Set Your Password</h2>
                <p>Click the link below to set your password:</p>
                <a href='{resetLink}' style='padding:10px 20px; background:#28a745; color:white; text-decoration:none; border-radius:5px;'>Set Password</a>
                <p>If you didn't request this, ignore this email.</p>";

            await _emailService.SendEmailAsync(email, "Set Your Password - Oversight", emailBody);

            return Ok(new { message = "Reset link sent successfully.", email, emailBody });
        }

        [HttpPost("forget-password/{email}")]
        public async Task<IActionResult> ForgetPassword(string email)
        {
            var user = await _context.User.FirstOrDefaultAsync(u => u.Email == email);
            if (user == null)
                return BadRequest(new { message = "User not found." });

            var token = Guid.NewGuid().ToString();
            user.VerificationCode = token;
            user.VerificationCodeGenerationTime = DateTime.UtcNow;
            await _context.SaveChangesAsync();

            string encryptedEmail = Encrypt(email);
            string encryptedToken = Encrypt(token);
            var resetLink = $"http://geneinsure.kindlebit.com/set-password?{encryptedToken}&&{encryptedEmail}";

            //user.ResetLink = resetLink;

            //var resetLink = $"http://outriskappback.kindlebit.net/set-password?{encryptedToken}&&{encryptedEmail}";

            string emailBody = $@"
        <h2>Set Your Password</h2>
        <p>Click the link below to set new password:</p>
        <a href='{resetLink}' style='padding:10px 20px; background:#28a745; color:white; text-decoration:none; border-radius:5px;'>Set Password</a>
        <p>If you didn't request this, ignore this email.</p>";

            //await _emailService.SendEmailAsync(email, "Set Your Password - Oversight", emailBody);

            string jsonVariables = JsonConvert.SerializeObject(user);
            var recipientEmail = Helper.ExtractMatchingValues(jsonVariables);
            HttpContext.Items["RecipientEmail"] = recipientEmail;
            HttpContext.Items["VariablesRaw"] = jsonVariables;


            return Ok(new { message = "Reset link sent successfully.", email, emailBody });
        }


        //[HttpPost("reset-password")]
        //public async Task<IActionResult> ResetPassword([FromBody] SetPasswordModel model)
        //{
        //    try
        //    {
        //        // Validate input
        //        if (model == null || string.IsNullOrEmpty(model.EncryptedToken) || string.IsNullOrEmpty(model.EncryptedPassword))
        //            return BadRequest(new { message = "Invalid input data." });

        //        // Split token before and after "&&"
        //        string[] tokenParts = model.EncryptedToken.Split(new string[] { "&&" }, StringSplitOptions.None);
        //        if (tokenParts.Length != 2)
        //            return BadRequest(new { message = "Invalid token format. Ensure correct encoding and structure." });

        //        string encryptedTokenPart1 = tokenParts[0];
        //        string encryptedTokenPart2 = tokenParts[1];

        //        // Ensure valid Base64 format
        //        if (!IsBase64String(encryptedTokenPart1) || !IsBase64String(encryptedTokenPart2))
        //            return BadRequest(new { message = "Invalid token format. Ensure Base64 encoding." });

        //        // Attempt decryption
        //        string token, email, newPassword;
        //        try
        //        {
        //            token = Decrypt(encryptedTokenPart1);
        //            email = Decrypt(encryptedTokenPart2);
        //            newPassword = model.EncryptedPassword;
        //        }
        //        catch (FormatException ex)
        //        {
        //            return BadRequest(new { message = "Decryption failed. Invalid Base64 format.", error = ex.Message });
        //        }

        //        // Find user by token
        //        var user = await _context.Userss.FirstOrDefaultAsync(u => u.Email == email);
        //        if (user == null)
        //            return NotFound(new { message = "Invalid email.", ResendToken = false });

        //        // Validate stored token
        //        string storedToken = user.VerificationCode;
        //        if (string.IsNullOrEmpty(storedToken) || storedToken != token)
        //            return NotFound(new { message = "Invalid token.", ResendToken = false, email });

        //        // Check token expiration (valid for 1 hour)
        //        bool isTokenValid = user.VerificationCodeGenerationTime > DateTime.UtcNow.AddMinutes(-60);
        //        if (!isTokenValid)
        //            return NotFound(new { message = "Token has expired.", ResendToken = true, Email = email });

        //        // Validate password strength
        //        if (!IsValidPassword(newPassword))
        //            return Unauthorized(new { message = "Password must contain at least one uppercase letter, one lowercase letter, one number, and one special character.", ResendToken = false });

        //        // Activate user
        //        if (user.IsVerified == false || user.IsActive == true || user.IsVerified != null || user.IsActive != null)
        //        {
        //            user.IsVerified = true;
        //        }

        //        // Hash and store new password
        //        user.PasswordHash = BCrypt.Net.BCrypt.HashPassword(newPassword);
        //        user.VerificationCode = null; // Remove token after use
        //        user.VerificationCodeValidationTime = DateTime.UtcNow;

        //        await _context.SaveChangesAsync();

        //        return Ok(new { message = "Password successfully set! You can now log in.", status = true, Email = email });
        //    }
        //    catch (Exception ex)
        //    {
        //        return StatusCode(500, new { message = "An error occurred while processing your request.", error = ex.Message, status = false });
        //    }
        //}


        [HttpPost("set-password")]
        public async Task<IActionResult> SetPassword([FromBody] SetPasswordModel model)
        {
            try
            {
                // Validate input
                if (model == null || string.IsNullOrEmpty(model.EncryptedToken) || string.IsNullOrEmpty(model.EncryptedPassword))
                    return BadRequest(new { message = "Invalid input data." });

                // Split token before and after "&&"
                string[] tokenParts = model.EncryptedToken.Split(new string[] { "&&" }, StringSplitOptions.None);
                if (tokenParts.Length != 2)
                    return BadRequest(new { message = "Invalid token format. Ensure correct encoding and structure." });

                string encryptedTokenPart1 = tokenParts[0];
                string encryptedTokenPart2 = tokenParts[1];

                // Ensure valid Base64 format
                if (!IsBase64String(encryptedTokenPart1) || !IsBase64String(encryptedTokenPart2))
                    return BadRequest(new { message = "Invalid token format. Ensure Base64 encoding." });

                // Attempt decryption
                string token, email, newPassword;
                try
                {
                    token = Decrypt(encryptedTokenPart1);
                    email = Decrypt(encryptedTokenPart2);
                    newPassword = model.EncryptedPassword;
                }
                catch (FormatException ex)
                {
                    return BadRequest(new { message = "Decryption failed. Invalid Base64 format.", error = ex.Message });
                }

                // Find user by token
                var user = await _context.User.FirstOrDefaultAsync(u => u.Email == email);
                if (user == null)
                    return NotFound(new { message = "Invalid email.", ResendToken = false });

                // Validate stored token
                string storedToken = user.VerificationCode;
                if (string.IsNullOrEmpty(storedToken) || storedToken != token)
                    return NotFound(new { message = "Invalid token.", ResendToken = false, email });

                // Check token expiration (valid for 24 hours)
                bool isTokenValid = user.VerificationCodeGenerationTime > DateTime.UtcNow.AddMinutes(-60);
                if (!isTokenValid)
                    return NotFound(new { message = "Token has expired.", ResendToken = true, Email = email });

                // Validate password strength
                if (!IsValidPassword(newPassword))
                    return Unauthorized(new { message = "Password must contain at least one uppercase letter, one lowercase letter, one number, and one special character.", ResendToken = false });

                // Activate user
                if (user.IsVerified == false || user.IsActive == true || user.IsVerified != null || user.IsActive != null)
                {
                    user.IsVerified = true;
                }

                // Hash and store new password
                user.PasswordHash = BCrypt.Net.BCrypt.HashPassword(newPassword);
                user.VerificationCode = null; // Remove token after use
                user.VerificationCodeValidationTime = DateTime.UtcNow;

                // Create UserParent relationship (only if not exists)
                var userParent = new UserParent
                {
                    UserId = user.UserName,
                    ParentId = int.Parse(user.CreatedBy),
                    IsActive = true,
                    IsDeleted = false,
                    IsVerified = true
                };
                _context.UserParent.Add(userParent);
                await _context.SaveChangesAsync();

                await _context.SaveChangesAsync();

                return Ok(new { message = "Password successfully set! You can now log in.", ResendToken = false, Email = email });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred while processing your request.", error = ex.Message, ResendToken = false });
            }
        }

        [Authorize(Policy = Utilities.Module.UserManagement)]
        [Authorize(Policy = Utilities.Permission.GetAll)]
        [HttpGet("GetUsersByChildRoles")]
        public async Task<IActionResult> GetUsersByChildRoles()
        {
            string role = GetUserRoleFromToken();
            if (string.IsNullOrEmpty(role))
                return Unauthorized("Role not found in token");

            // Find the Role ID of the Logged-in User
            var userRole = await _context.Role.FirstOrDefaultAsync(r => r.Id.ToString() == role);
            if (userRole == null)
                return NotFound("Role not found in database");

            // Get all descendant Role IDs recursively
            var childRoleIds = await GetAllChildRoleIds(userRole.Id);

            // Include the current role in the list
            childRoleIds.Add(userRole.Id);

            if (!childRoleIds.Any())
                return NotFound("No child roles found");

            // Fetch Users belonging to those roles
            var users = await _context.User
                .Where(u => childRoleIds.Contains(u.Roles.Id))
                .ToListAsync();

            var userList = _mapper.Map<List<UserDto>>(users); // Convert to DTOs

            return Ok(userList);
        }

        //[Authorize(Policy = Utilities.Module.UsersManagement)]
        //[Authorize(Policy = Utilities.Permission.GetAll)]
        [HttpGet("getAllRoles")]
        public async Task<IActionResult> GetAllRoles()
        {
            var roles = await _context.Role
                .Select(r => new RoleDtoOutput
                {
                    RoleId = r.Id,
                    RoleName = r.RoleName
                })
                .ToListAsync();

            return Ok(roles);
        }

        [Authorize(Policy = Utilities.Module.UserManagement)]
        [Authorize(Policy = Utilities.Permission.GetAll)]
        [HttpGet("getAllUsers")]
        public async Task<IActionResult> GetAllUsers([FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 10)
        {
            var usersQuery = _context.User
                .Include(c => c.Roles)
                .Select(u => new UsersDtoOutput
                {
                    userId = u.UserName,
                    FirstName = u.FirstName,
                    LastName = u.LastName,
                    RoleId = u.Roles.Id,
                    RoleName = u.Roles.RoleName,
                    Email = u.Email,
                    MobileNumber = u.PhoneNumber,
                    IsActive = u.IsActive,
                    IsVerified = u.IsVerified
                });

            var totalRecords = await usersQuery.CountAsync();

            var users = await usersQuery
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return Ok(new
            {
                PageNumber = pageNumber,
                PageSize = pageSize,
                TotalRecords = totalRecords,
                Data = users
            });
        }

        [HttpGet("getAllUsersByRole/{roleId}")]
        public async Task<IActionResult> GetAllUsersByRole(int roleId)
        {
            var users = await _context.User
                .Select(u => new UsersDtoOutput
                {
                    FirstName = u.FirstName,
                    LastName = u.LastName,
                    RoleId = u.Roles.Id,
                    Email = u.Email,
                    MobileNumber = u.PhoneNumber,
                    IsActive = u.IsActive,
                    IsVerified = u.IsVerified,
                    Id = u.UserName
                }).Where(x => x.RoleId == roleId)
                .ToListAsync();

            return Ok(users);
        }

        [HttpGet("parent-role-users/{roleId}")]
        public async Task<IActionResult> GetAllParentRoleUsers(int roleId)
        {
            // Step 1: Get the ParentRoleId of the given role
            var parentRoleId = await _context.Role
                .Where(r => r.Id == roleId)
                .Select(r => r.ParentRoleId)
                .FirstOrDefaultAsync();

            if (parentRoleId == null)
            {
                return Ok(new List<UsersDtoOutput>());
            }

            // Step 2: Get users with the parent role ID
            var users = await _context.User
                .Where(u => u.Roles.Id == parentRoleId)
                .Select(u => new UsersDtoOutput
                {
                    FirstName = u.FirstName,
                    LastName = u.LastName,
                    RoleId = u.Roles.Id,
                    Email = u.Email,
                    MobileNumber = u.PhoneNumber,
                    IsActive = u.IsActive,
                    IsVerified = u.IsVerified,
                    Id = u.UserName
                })
                .ToListAsync();

            return Ok(users);
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
                    new Claim(ClaimTypes.NameIdentifier, user.UserName.ToString()),
                    new Claim(ClaimTypes.Email, user.Email),
                    new Claim(ClaimTypes.Role, user.Roles?.Id.ToString() ?? "0"),
                    new Claim("FirstName", user.FirstName ?? ""),
                    new Claim("LastName", user.LastName ?? ""),
                    new Claim("MobileNumber", user.PhoneNumber ?? ""),
                    new Claim("UserId", user.UserName.ToString()),
                    new Claim("RoleName", user.Roles?.RoleName ?? "User")
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

        public static async Task SeedSuperAdminAsync(DataContext context)
        {
            //var roleHierarchy = new List<(int Id, string RoleName, int? ParentRoleId)>
            //                    {
            //                        (301, "SuperAdmin", null),
            //                        (302, "Admin", 301),
            //                        (303, "SocietyAdmin", 302),
            //                        (304, "Resident", 303),
            //                        (305, "Guest", 304)
            //                    };

            //var existingRoles = await context.Role.ToDictionaryAsync(r => r.RoleName, r => r);

            //// Begin transaction to ensure consistency
            //using var transaction = await context.Database.BeginTransactionAsync();

            //try
            //{
            //    //    await context.Database.ExecuteSqlRawAsync("SET IDENTITY_INSERT Roles ON");

            //    //    foreach (var (id, roleName, parentRoleId) in roleHierarchy)
            //    //    {
            //    //        if (!existingRoles.ContainsKey(roleName))
            //    //        {
            //    //            var trackedEntity = context.ChangeTracker.Entries<Role>()
            //    //                                       .FirstOrDefault(e => e.Entity.Id == id);

            //    //            if (trackedEntity != null)
            //    //            {
            //    //                trackedEntity.State = EntityState.Detached;
            //    //            }

            //    //            var role = new Role
            //    //            {
            //    //                Id = id,
            //    //                RoleName = roleName,
            //    //                ParentRoleId = parentRoleId
            //    //            };

            //    //            await context.Role.AddAsync(role);

            //    //            existingRoles[roleName] = role;
            //    //        }
            //    //    }

            //    //    await context.SaveChangesAsync();

            //    //    await context.Database.ExecuteSqlRawAsync("SET IDENTITY_INSERT Roles OFF");


            //    //    await context.SaveChangesAsync();

            //    //    await context.Database.ExecuteSqlRawAsync("SET IDENTITY_INSERT Roles OFF");

            //    var existingRoles = await context.Role.ToListAsync();

            //    // Seed SuperAdmin user
            //    var superAdminRole = existingRoles.FirstOrDefault(x => x.RoleName == "SuperAdmin");

            //    if (superAdminRole != null && !await context.User.AnyAsync(u => u.Email == "superadmin@gmail.com"))
            //    {
            //        Guid username = Guid.NewGuid();

            //        var superAdmin = new User
            //        {
            //            UserName = username,
            //            FirstName = "Super",
            //            LastName = "Admin",
            //            Email = "superadmin@gmail.com",
            //            PhoneNumber = "1122334455",
            //            PasswordHash = BCrypt.Net.BCrypt.HashPassword("Super@123"),
            //            IsActive = true,
            //            IsVerified = true,
            //            CreatedBy= username.ToString(),
            //            CreatedOn = DateTime.UtcNow
            //        };

            //        await context.User.AddAsync(superAdmin);
            //        await context.SaveChangesAsync();
            //    }

            //    await transaction.CommitAsync();
            //}
            //catch
            //{
            //    await transaction.RollbackAsync();
            //}
        }

        public static bool IsBase64String(string base64)
        {
            if (string.IsNullOrEmpty(base64) || base64.Length % 4 != 0)
                return false;

            try
            {
                Convert.FromBase64String(base64);
                return true;
            }
            catch
            {
                return false;
            }
        }

        public static string DecryptToken(string encryptedText)
        {
            if (string.IsNullOrEmpty(encryptedText))
                throw new ArgumentException("Encrypted text cannot be null or empty.");

            try
            {
                encryptedText = SanitizeBase64(encryptedText); // Fix invalid characters
                byte[] encryptedBytes = Convert.FromBase64String(encryptedText);
                return Encoding.UTF8.GetString(encryptedBytes);
            }
            catch (FormatException)
            {
                throw new FormatException("Invalid Base64 format: The input string is not a valid Base64 encoded string.");
            }
        }


        [Authorize(Policy = Utilities.Module.UserManagement)]
        [Authorize(Policy = Utilities.Permission.GetAll)]
        [HttpGet("child-roles")]
        public async Task<IActionResult> GetChildRoles()
        {
            string roleName = GetUserRolesFromToken();
            if (string.IsNullOrEmpty(roleName))
                return Unauthorized("Role not found in token");

            var roles = await GetChildRolesAsync(roleName);
            if (!roles.Any())
                return NotFound("No child roles found");

            var rolesList = _mapper.Map<List<RoleDto>>(roles); // Convert to DTOs

            return Ok(rolesList);
        }

        [AllowAnonymous]
        [HttpGet("GetAllClaimUsersByInsurerId")]
        public async Task<IActionResult> GetAllClaimUsersByInsurerId([FromQuery] int insurerid)
        {
            var usersUnderInsurer = await _context.UserParent
                .Where(x => x.ParentId == insurerid)
                .Select(x => x.UserId)
                .ToListAsync();

            var users = await _context.User
                .Where(x => usersUnderInsurer.Contains(x.UserName))
               .Select(u => new UsersDtoOutput
               {
                   
                   FirstName = u.FirstName,
                   LastName = u.LastName,
                   RoleId = u.Roles.Id,
                   Email = u.Email,
                   MobileNumber = u.PhoneNumber,
                   IsActive = u.IsActive,
                   IsVerified = u.IsVerified,
                   userId = u.UserName
               })
               .ToListAsync();

            return Ok(users);
        }

        #endregion

        #region Private Method

        //private List<ChildUsersDTO> BuildUserTree(List<User> users, int? parentId)
        //{
        //    return users
        //        .Where(u => u.CreatedBy == parentId)
        //        .Select(u => new ChildUsersDTO
        //        {
        //            UserId = u.Id,
        //            FullName = $"{u.FirstName} {u.LastName}",
        //            Email = u.Email,
        //            Children = BuildUserTree(users, u.Id) // Recursive
        //        })
        //        .ToList();
        //}


        //private ChildUsersDTO? FindUserSubTree(List<ChildUsersDTO> tree, int userId)
        //{
        //    foreach (var node in tree)
        //    {
        //        if (node.UserId == userId)
        //            return node;

        //        var found = FindUserSubTree(node.Children, userId);
        //        if (found != null)
        //            return found;
        //    }
        //    return null;
        //}



        //private List<ChildUsersDTO> BuildUserTree(List<User> users, int? parentId = null)
        //{
        //    return users
        //        .Where(u => u.CreatedBy == parentId)
        //        .Select(u => new ChildUsersDTO
        //        {
        //            UserId = u.Id,
        //            FullName = $"{u.FirstName} {u.LastName}",
        //            Email = u.Email,
        //            Children = BuildUserTree(users, u.Id)
        //        })
        //        .ToList();
        //}


        private async Task SendVerificationEmail(string email, string token)
        {
            string encryptedEmail = Encrypt(email);
            string encryptedToken = Encrypt(token);

            //var resetLink = $"http://localhost:7260/set-password?{encryptedToken}&&{encryptedEmail}";
            var resetLink = $"http://geneinsure.kindlebit.com/set-password?{encryptedToken}&&{encryptedEmail}";

            string emailBody = $@"
                <h2>Set Your Password</h2>
                <p>Click the link below to set your password:</p>
                <a href='{resetLink}' style='padding:10px 20px; background:#28a745; color:white; text-decoration:none; border-radius:5px;'>Set Password</a>
                <p>If you didn't request this, ignore this email.</p>";

            await _emailService.SendEmailAsync(email, "Set Your Password - Oversight", emailBody);
        }

        private string GetUserRolesFromToken()
        {
            var identity = HttpContext.User.Identity as ClaimsIdentity;

            if (identity == null) return null;

            var roleClaim = identity.FindFirst(ClaimTypes.Role);
            return roleClaim?.Value;
        }

        private async Task<List<int>> GetAllChildRoleIdsAsync(int parentId)
        {
            var childRoles = await _context.Role
                .Where(r => r.ParentRoleId == parentId)
                .ToListAsync();

            var childRoleIds = childRoles.Select(r => r.Id).ToList();

            foreach (var child in childRoles)
            {
                var subChildIds = await GetAllChildRoleIdsAsync(child.Id);
                childRoleIds.AddRange(subChildIds);
            }

            return childRoleIds;
        }

        private async Task<List<Role>> GetChildRolesAsync(string roleName)
        {
            var userRole = await _context.Role.FirstOrDefaultAsync(r => r.Id.ToString() == roleName);
            if (userRole == null) return new List<Role>();

            var childRoleIds = await GetAllChildRoleIdsAsync(userRole.Id);
            return await _context.Role.Where(r => childRoleIds.Contains(r.Id)).ToListAsync();
        }


        // Recursive function to get all child role IDs
        private async Task<List<int>> GetAllChildRoleIds(int parentId)
        {
            var childRoles = await _context.Role
                .Where(r => r.ParentRoleId == parentId)
                .ToListAsync();

            var childRoleIds = childRoles.Select(r => r.Id).ToList();

            foreach (var child in childRoles)
            {
                var subChildIds = await GetAllChildRoleIds(child.Id);
                childRoleIds.AddRange(subChildIds);
            }

            return childRoleIds;
        }

        private string GetUserRoleFromToken()
        {
            var identity = HttpContext.User.Identity as ClaimsIdentity;

            if (identity == null) return null;

            var roleClaim = identity.FindFirst(ClaimTypes.Role);  // Extract role name from token
            return roleClaim?.Value;
        }


        // Function to check password strength
        private bool IsValidPassword(string password)
        {
            return password.Length >= 8 &&
                   password.Any(char.IsUpper) &&
                   password.Any(char.IsLower) &&
                   password.Any(char.IsDigit) &&
                   password.Any(ch => "!@#$%^&*()-_=+[]{};:'\",.<>?/\\|".Contains(ch));
        }

        private string Encrypt(string plainText)
        {
            byte[] keyBytes = GetAesKey();
            using (Aes aes = Aes.Create())
            {
                aes.Key = keyBytes;
                aes.Mode = CipherMode.ECB;
                aes.Padding = PaddingMode.PKCS7;
                using (ICryptoTransform encryptor = aes.CreateEncryptor())
                {
                    byte[] inputBytes = Encoding.UTF8.GetBytes(plainText);
                    byte[] encryptedBytes = encryptor.TransformFinalBlock(inputBytes, 0, inputBytes.Length);
                    return Convert.ToBase64String(encryptedBytes);
                }
            }
        }

        private string Decrypt(string encryptedText)
        {
            byte[] keyBytes = GetAesKey();
            using (Aes aes = Aes.Create())
            {
                aes.Key = keyBytes;
                aes.Mode = CipherMode.ECB;
                aes.Padding = PaddingMode.PKCS7;
                using (ICryptoTransform decryptor = aes.CreateDecryptor())
                {
                    byte[] inputBytes = Convert.FromBase64String(encryptedText);
                    byte[] decryptedBytes = decryptor.TransformFinalBlock(inputBytes, 0, inputBytes.Length);
                    return Encoding.UTF8.GetString(decryptedBytes);
                }
            }
        }

        #endregion
    }

public static class PasswordGenerator
    {
        public static string GeneratePassword(int length = 8)
        {
            if (length < 3)
                throw new ArgumentException("Password length must be at least 3 to include digit, special character, and letter.");

            const string digits = "0123456789";
            const string specialChars = "!@#$%^&*";
            const string letters = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ";
            const string allChars = digits + specialChars + letters;

            var password = new char[length];
            var rng = new RNGCryptoServiceProvider();
            byte[] buffer = new byte[1];

            // Ensure at least one digit, one special character, and one letter
            password[0] = digits[GetRandomIndex(rng, digits.Length)];
            password[1] = specialChars[GetRandomIndex(rng, specialChars.Length)];
            password[2] = letters[GetRandomIndex(rng, letters.Length)];

            // Fill the remaining characters randomly
            for (int i = 3; i < length; i++)
            {
                password[i] = allChars[GetRandomIndex(rng, allChars.Length)];
            }

            // Shuffle the password so it's not predictable
            return new string(password.OrderBy(_ => Guid.NewGuid()).ToArray());
        }

        private static int GetRandomIndex(RandomNumberGenerator rng, int max)
        {
            byte[] randomNumber = new byte[4];
            int value;

            do
            {
                rng.GetBytes(randomNumber);
                value = BitConverter.ToInt32(randomNumber, 0) & int.MaxValue; // Make positive
            } while (value >= max * (int.MaxValue / max)); // Avoid modulo bias

            return value % max;
        }
    }
}