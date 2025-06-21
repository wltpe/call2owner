using AutoMapper;
using Azure;
using Call2Owner.DTO;
using Call2Owner.Models;
using Call2Owner.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using OfficeOpenXml.FormulaParsing.LexicalAnalysis;
using RestSharp;
using System;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Net.Mime;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using Utilities;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;
using Microsoft.Net.Http.Headers;

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

        [HttpGet("get-all-resident-types")]
        public async Task<IActionResult> GetAllResidentTypes(int EntityTypeId)
        {
            var result = await _context.EntityTypeDetail
       .Where(d => d.IsDeleted != true && d.IsActive == true && d.EntityTypeId == EntityTypeId)
       .OrderBy(d => d.Id)
       .ToListAsync();

            // result is assumed to be a collection of dynamic or database rows
            List<EntityTypeDetail> entityTypeDetails = new List<EntityTypeDetail>();

            if (result.Count > 0)
            {

                foreach (var row in result)
                {
                    var entity = new EntityTypeDetail
                    {
                        Id = row.Id,
                        EntityTypeId = row.EntityTypeId,
                        Value = row.Value,
                        Label = row.Label,
                        DetailJson = JsonConvert.DeserializeObject<List<DetailItem>>(row.DetailJson.ToString()),
                        IsDafault = row.IsDafault,
                        IsActive = row.IsActive,
                        CreatedBy = row.CreatedBy,
                        CreatedOn = row.CreatedOn,
                        UpdatedBy = row.UpdatedBy,
                        UpdatedOn = row.UpdatedOn,
                        IsDeleted = row.IsDeleted,
                        DeletedBy = row.DeletedBy,
                        DeletedOn = row.DeletedOn
                    };

                    entityTypeDetails.Add(entity);
                }
            }
            else
            {
                entityTypeDetails = null;
            }

            if (entityTypeDetails == null)
            {
                return NotFound("No Resident Type found, contact administation.");
            }
                return Ok(entityTypeDetails);
        }

        [HttpPost("update-resident-selected-type")]
        public async Task<IActionResult> UpdateResidentSelectedType([FromForm] SelectedRecord obj)
        {
            try
            {
                var currentUserId = Convert.ToString(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "0");

                var mismatchMessages = new List<MismatchOutput>();

                bool IsFileRequired = false;

                var result = await _context.EntityTypeDetail
      .Where(d => d.IsDeleted != true && d.IsActive == true && d.EntityTypeId == obj.entityTypeId)
      .OrderBy(d => d.Id)
      .ToListAsync();

            // result is assumed to be a collection of dynamic or database rows
            List<EntityTypeDetail> entityTypeDetails = new List<EntityTypeDetail>();

            if (result.Count > 0)
            {

                foreach (var row in result)
                {
                    var entity = new EntityTypeDetail
                    {
                        Id = row.Id,
                        EntityTypeId = row.EntityTypeId,
                        Value = row.Value,
                        Label = row.Label,
                        DetailJson = JsonConvert.DeserializeObject<List<DetailItem>>(row.DetailJson.ToString()),
                        IsDafault = row.IsDafault,
                        IsActive = row.IsActive,
                        CreatedBy = row.CreatedBy,
                        CreatedOn = row.CreatedOn,
                        UpdatedBy = row.UpdatedBy,
                        UpdatedOn = row.UpdatedOn,
                        IsDeleted = row.IsDeleted,
                        DeletedBy = row.DeletedBy,
                        DeletedOn = row.DeletedOn
                    };

                    entityTypeDetails.Add(entity);
                }
            }
            else
            {
                entityTypeDetails = null;
            }

            if (entityTypeDetails == null)
            {
                return NotFound("No Resident Type found, contact administation.");
            }

           var fullResponse = entityTypeDetails;
           var selectedRecords = obj;

            //Compare 
            List<MismatchReport> mismatchedResults = new();

                string extensionFromFile = null;
                string extensionFromFileName = null;

                if (selectedRecords.file is not null)
                {
                    extensionFromFile = Path.GetExtension(ContentDispositionHeaderValue.Parse(selectedRecords.file.ContentDisposition)
                                        .FileName.ToString().Trim('"'))?.TrimStart('.');
                }

                if (!string.IsNullOrWhiteSpace(selectedRecords.fileName))
                {
                extensionFromFileName = Path.GetExtension(selectedRecords.fileName)?.TrimStart('.');
                }
                


                bool matchingDetail = fullResponse.Any(entity =>
                                 entity.DetailJson.Any(detailItem =>
                                     detailItem.Details.Any(detail =>
                                         detail.fieldText == selectedRecords.fieldText &&
                                         detail.fieldId == selectedRecords.fieldId &&
                                         detail.fieldName == selectedRecords.fieldName &&
                                         detail.recordType == selectedRecords.recordType &&
                                         (
                                             (detail.type == null && selectedRecords.type == null) ||
                                             (detail.type != null && selectedRecords.type != null &&
                                              detail.type.Contains(selectedRecords.type) &&
                                              detail.type.Contains(extensionFromFile) &&
                                              detail.type.Contains(extensionFromFileName)) // ✅ works now
                                         )
                                     )
                                 )
                                );

                // Try to find a "close" match by same fieldId or fieldText for better reporting
                var possible = fullResponse
                    .SelectMany(e => e.DetailJson)
                    .SelectMany(d => d.Details)
                    .FirstOrDefault(d => d.fieldId == selectedRecords.fieldId); // or .fieldText

                if (matchingDetail == false)
                    {
                     

                        var report = new MismatchReport
                        {
                            Input = selectedRecords,
                            Mismatches = new Dictionary<string, (object?, object?)>()
                        };

                        if (possible != null)
                        {
                            if (possible.fieldText != selectedRecords.fieldText)
                                report.Mismatches["fieldText"] = (selectedRecords.fieldText, possible.fieldText);

                            if (possible.fieldId != selectedRecords.fieldId)
                                report.Mismatches["fieldId"] = (selectedRecords.fieldId, possible.fieldId);

                            if (possible.fieldName != selectedRecords.fieldName)
                                report.Mismatches["fieldName"] = (selectedRecords.fieldName, possible.fieldName);

                            if (possible.recordType != selectedRecords.recordType)
                                report.Mismatches["recordType"] = (selectedRecords.recordType, possible.recordType);

                            // Assuming selected.type is string and possible.type is List<string>
                            if (possible.type !=null)
                            {
                            IsFileRequired = true;

                                if (possible.type == null || !possible.type.Contains(selectedRecords.type))
                                    report.Mismatches["type"] = (selectedRecords.type, possible.type);
                                 
                                if (string.IsNullOrWhiteSpace(selectedRecords.fileName))
                                    report.Mismatches["fileName"] = (selectedRecords.fileName, "File Name required");
                                else if (!possible.type.Contains(extensionFromFileName))
                                    report.Mismatches["fileName"] = (extensionFromFileName, possible.type);

                            if (selectedRecords.file is null)
                                    report.Mismatches["file"] = (extensionFromFile, "Upload doc/image");
                                        
                                if (selectedRecords.file != null)
                                {
                                        if (!possible.type.Contains(extensionFromFile))
                                        {
                                             report.Mismatches["file"] = (extensionFromFile, possible.type);
                                        }

                                }

                                     
                                 
                            }
                         
                        }
                        else
                        {
                            // No match found at all
                            report.Mismatches["match"] = ("Expected match", "Not found");
                        }

                        mismatchedResults.Add(report);



                   //     var response = new List<MismatchOutput>();

                        foreach (var mismatch in mismatchedResults)
                        {
                            var output = new MismatchOutput
                            {
                                Message = "Mismatch for selected input",
                                Input = mismatch.Input,
                                Mismatches = mismatch.Mismatches.Select(kvp => new MismatchDetail
                                {
                                    key = kvp.Key,
                                    value = kvp.Value.InputValue,
                                    requiredValue = kvp.Value.FoundValue
                                }).ToList()
                            };

                            mismatchMessages.Add(output);
                        }

                        return NotFound(mismatchMessages);

                        // Return all mismatch messages as response


                    }

                    // Valid Values 
                if (mismatchMessages.Count == 0)
                {
                    var blobName = "";

                bool IsFileReqiured  =  possible.type.Contains(selectedRecords.type);
                    if (IsFileReqiured == true)
                    {
                        // Upload file
                        if (obj.file is not null)
                        {
                            blobName = await UploadFileOrImageFolderAsync(selectedRecords.file.FileName, selectedRecords.file.OpenReadStream());
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

                    var selected = new UploadSelectedRecord
                    {
                        entityTypeId = selectedRecords.entityTypeId,
                        fieldText = selectedRecords.fieldText,
                        fieldId = selectedRecords.fieldId,
                        fieldName = selectedRecords.fieldName,
                        recordType = selectedRecords.recordType,
                        type = selectedRecords.type,
                        fileName = selectedRecords.fieldName,
                        file = blobName
                    };

                    string json = JsonConvert.SerializeObject(selected, Formatting.Indented);

                    UpdateResident updateResident = new UpdateResident();

                        updateResident.Id = Guid.Parse(currentUserId);
                        updateResident.UpdatedOn = DateTime.UtcNow;
                        updateResident.UpdatedBy = currentUserId;
                    updateResident.DetailJson = json;


                    var returnResult = await UpdateResidentDocumentUploadedAsync(updateResident);


                    if (returnResult != null)
                    {
                        returnResult.ResidentType = JsonConvert.DeserializeObject<UploadSelectedRecord>(returnResult.DetailJson);
                        return Ok(returnResult);
                    }
                    else
                    {
                        return NotFound(new { Message = "Unable to update Resident Type" });
                    }
                  
                }
                else
                {

                    return NotFound(mismatchMessages);
                }

            }
            catch (System.Text.Json.JsonException)
            {
                return BadRequest();
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

        private async Task<UpdateResidentDto?> UpdateResidentDocumentUploadedAsync(UpdateResident updateResident)
        {
            // Check if a record already exists
            var userExists = await _context.Resident.FirstOrDefaultAsync(x =>
                                    x.UserId == updateResident.Id && x.IsDeleted != true
                                    && x.IsActive == true && x.IsDocumentUploaded == false && x.IsApproved == false);

            if (userExists != null)
            {
                userExists.UpdatedBy = updateResident.UpdatedBy;
                userExists.UpdatedOn = updateResident.UpdatedOn;
                userExists.DetailJson = updateResident.DetailJson;
                userExists.IsDocumentUploaded = true;

                 _context.Resident.Update(userExists);

                // Save changes to the database
                await _context.SaveChangesAsync();

                var residentResponseDTO = _mapper.Map<UpdateResidentDto>(userExists);

                return residentResponseDTO;
            }
            else
            {
                // Return null if the record exists
                return null;
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
        public string? FileName { get; set; }
        // public string? Address { get; set; }
        public IFormFile? File { get; set; }
    }
    public class JsonDetail
    {
        public string fieldText { get; set; }
        public string fieldType { get; set; }
        public string fieldId { get; set; }
        public string fieldName { get; set; }
        public bool required { get; set; }
        public bool isActive { get; set; }
        public bool isInputControl { get; set; }
        public string recordType { get; set; }
        public List<string> type { get; set; } // Optional, for "file" type fields
    }
    public class DetailItem
    {
        public bool IsDocumentRequired { get; set; }
        public List<JsonDetail> Details { get; set; }
    }
    public class EntityTypeDetail
    {
        public int Id { get; set; }
        public int EntityTypeId { get; set; }
        public string Value { get; set; }
        public string Label { get; set; }
        public List<DetailItem> DetailJson { get; set; }
        public bool? IsDafault { get; set; }
        public bool IsActive { get; set; }
        public string? CreatedBy { get; set; }
        public DateTime? CreatedOn { get; set; }
        public string? UpdatedBy { get; set; }
        public DateTime? UpdatedOn { get; set; }
        public bool? IsDeleted { get; set; }
        public string? DeletedBy { get; set; }
        public DateTime? DeletedOn { get; set; }
    }

    public class SelectedRecord
    {
        public int entityTypeId { get; set; }
        public string fieldText { get; set; }
        public string fieldId { get; set; }
        public string fieldName { get; set; }
        public string recordType { get; set; }
        public string? type { get; set; }
        public string? fileName { get; set; }
        public IFormFile? file { get; set; }
    }

  
    public class UploadSelectedRecord
    {
        public int entityTypeId { get; set; }
        public string fieldText { get; set; }
        public string fieldId { get; set; }
        public string fieldName { get; set; }
        public string recordType { get; set; }
        public string? type { get; set; }
        public string? fileName { get; set; }
        public string? file { get; set; }
    }

    public class SocietySelectedRecord
    {
        public Guid societyId { get; set; }
        public int entityTypeId { get; set; }
        public string fieldText { get; set; }
        public string fieldId { get; set; }
        public string fieldName { get; set; }
        public string recordType { get; set; }
        public string? type { get; set; }
        public string? fileName { get; set; }
        public IFormFile? file { get; set; }
    }

    public class SocietyUploadSelectedRecord
    {
      //  public Guid societyId { get; set; }
        public int entityTypeId { get; set; }
        public string fieldText { get; set; }
        public string fieldId { get; set; }
        public string fieldName { get; set; }
        public string recordType { get; set; }
        public string? type { get; set; }
        public string? fileName { get; set; }
        public string? file { get; set; }
    }


    public class MismatchReport
    {
        public SelectedRecord Input { get; set; }
        public Dictionary<string, (object? InputValue, object? FoundValue)> Mismatches { get; set; }
    }

    public class SocietyMismatchReport
    {
        public SocietySelectedRecord Input { get; set; }
        public Dictionary<string, (object? InputValue, object? FoundValue)> Mismatches { get; set; }
    }

    public class MismatchOutput
    {
        public string Message { get; set; }
        public object Input { get; set; }
        public List<MismatchDetail> Mismatches { get; set; }
    }

    public class MismatchDetail
    {
        public string key { get; set; }
        public object value { get; set; }
        public object requiredValue { get; set; }
    }

    public class UpdateResident 
    {
                public Guid Id { get; set; }
                public string UpdatedBy {get; set;}
                public DateTime UpdatedOn {get; set;}
                public string DetailJson { get; set; }
    }
}