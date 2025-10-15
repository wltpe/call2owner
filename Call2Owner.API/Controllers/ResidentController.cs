using AutoMapper;
using Azure;
using Call2Owner.DTO;
using Call2Owner.Models;
using Call2Owner.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Microsoft.IdentityModel.Tokens;
using Microsoft.Net.Http.Headers;
using Microsoft.VisualBasic.FileIO;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using OfficeOpenXml.Drawing.Chart;
using OfficeOpenXml.FormulaParsing.LexicalAnalysis;
using RestSharp;
using Swashbuckle.AspNetCore.Annotations;
using System;
using System.Buffers;
using System.ComponentModel.DataAnnotations;
using System.Data;
using System.Drawing;
using System.Drawing.Printing;
using System.Globalization;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Net.Mime;
using System.Reflection;
using System.Runtime.InteropServices.JavaScript;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using Utilities;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;
using static System.Net.WebRequestMethods;
using Dapper;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using System.Data;

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

        #region CommonAPIs
        //[Authorize(Policy = Utilities.Module.Resident)]
        //[Authorize(Policy = Utilities.Permission.Add)]
        [Authorize]
        [HttpPost("resident-self-register")]
        public async Task<IActionResult> SelfRegisterResident()
        {
            try
            {
                var currentUserId = User.FindFirst(ClaimTypes.Name)?.Value;

                if (string.IsNullOrEmpty(currentUserId))
                {
                    return NotFound(new
                    {
                        statusCode = StatusCodes.Status404NotFound,
                        message = "Invalid or Expired Token."
                    });
                }

                // Fetch user by primary key and active status
                var existingUser = await _context.User
                    .Include(u => u.Roles)
                    .FirstOrDefaultAsync(u => u.UserName.ToString() == currentUserId && u.IsActive);

                if (existingUser == null)
                {
                    return Conflict(new { message = "Unable to register for society." });
                }

                // Check if user already has Resident role
                var role = await _context.Role.AsNoTracking()
                                .FirstOrDefaultAsync(u => u.RoleName == "Resident");

                if (role==null)
                {
                    return NotFound(new
                    {
                        statusCode = StatusCodes.Status404NotFound,
                        message = "Unable to access society."
                    });
                }

                var roleDto = _mapper.Map<RoleDto>(role);

                if (roleDto == null)
                {
                    return NotFound(new
                    {
                        statusCode = StatusCodes.Status404NotFound,
                        message = "Unable to access society."
                    });
                }

                if (existingUser.Roles==null)
                {
                    existingUser.Roles = role;
                    existingUser.UpdatedOn = DateTime.UtcNow;

                    _context.User.Update(existingUser);
                    await _context.SaveChangesAsync();
                }


                // Check if resident already registsered
                var existingResident = await _context.Resident
                    .FirstOrDefaultAsync(r => r.UserId == existingUser.UserName);

                if (existingResident != null)
                {
                    return Ok(new
                    {
                        statusCode = StatusCodes.Status200OK,
                        message = "Resident already registered."
                    });
                }

                // Generate resident code
                var otpGenerator = new OTPGenerator();
                string residentCode = otpGenerator.GenerateOTP();

                // Create new Resident record
                var newResident = new Resident
                {
                    Id = Guid.NewGuid(),
                    UserId = existingUser.UserName,
                    ResidentCode = residentCode,
                    IsDocumentUploaded = false,
                    IsApproved = false,
                    IsActive = true,
                    CreatedBy = existingUser.UserName.ToString(),
                    CreatedOn = DateTime.UtcNow
                };

                await _context.Resident.AddAsync(newResident);
                await _context.SaveChangesAsync();

                return Ok(new
                {
                    statusCode = StatusCodes.Status200OK,
                    message = "Resident created successfully!"
                });
            }
            catch (Exception ex)
            {
                return BadRequest(new
                {
                    statusCode = StatusCodes.Status400BadRequest,
                    message = "An error occurred while registering the resident."
                });
            }
        }


        [Authorize(Policy = Utilities.Module.Resident)]
        [Authorize(Policy = Utilities.Permission.Get)]
        [HttpGet("resident-profile")]
        public async Task<IActionResult> ResidentProfile()
        {
            try
            {
                  var currentUserId = User.FindFirst(ClaimTypes.Name)?.Value;

                if (string.IsNullOrEmpty(currentUserId))
                {
                    return NotFound(new
                    {
                        statusCode = StatusCodes.Status404NotFound,
                        message = "Invalid or Expired Token."
                    });
                }

                var residentprofile = await _context.Resident
                                                 .FirstOrDefaultAsync(u => u.UserId.ToString() == currentUserId);
                if (residentprofile == null)
                {
                    return NotFound(new { statusCode = StatusCodes.Status404NotFound, message = "Resident not found." });
                }
                else
                {
                    string fullAddress = "";

                    // Fetch flat record
                    var societyFlat = residentprofile.SocietyFlatId != null
                        ? await _context.SocietyFlat.FirstOrDefaultAsync(f => f.Id == residentprofile.SocietyFlatId)
                        : null;

                    // Fetch building record
                    var societyBuilding = societyFlat != null
                        ? await _context.SocietyBuilding.FirstOrDefaultAsync(b => b.Id == societyFlat.SocietyBuildingId)
                        : null;

                    // Fetch society record
                    var society = societyBuilding != null
                        ? await _context.Society.FirstOrDefaultAsync(s => s.Id == societyBuilding.SocietyId)
                        : null;

                    // Fetch city record
                    var city = society != null
                        ? await _context.City.FirstOrDefaultAsync(c => c.Id == society.CityId)
                        : null;

                    // Fetch state record
                    var state = city != null
                        ? await _context.State.FirstOrDefaultAsync(s => s.Id == city.StateId)
                        : null;

                    // Fetch country record
                    var country = state != null
                        ? await _context.Country.FirstOrDefaultAsync(c => c.Id == state.CountryId)
                        : null;


                    var addressParts = new[]
                                               {
                                                societyFlat?.Name,
                                                societyBuilding?.Name,
                                                society?.Name,
                                                city?.Name,
                                                state?.Name,
                                                country?.Name
                                            };


                    ResidentDto returnobj = new ResidentDto();

                        returnobj.Id = residentprofile.Id;
                        returnobj.UserId = residentprofile.UserId;
                    returnobj.Address = string.Join(",", addressParts.Where(part => !string.IsNullOrWhiteSpace(part)));
                    returnobj.EntityTypeDetailId = residentprofile.EntityTypeDetailId;
                        returnobj.IsDocumentUploaded = residentprofile.IsDocumentUploaded;
                        returnobj.ResidentCode = residentprofile.ResidentCode;
                        returnobj.IsApproved = residentprofile.IsApproved;
                        returnobj.ApprovedBy = residentprofile.ApprovedBy;
                        returnobj.ApprovedOn = residentprofile.ApprovedOn;
                        returnobj.IsActive = residentprofile.IsActive;
                        returnobj.CreatedBy = residentprofile.CreatedBy;
                        returnobj.CreatedOn = residentprofile.CreatedOn;
                        returnobj.UpdatedBy = residentprofile.UpdatedBy;
                        returnobj.UpdatedOn = residentprofile.UpdatedOn;
                        returnobj.IsDeleted = residentprofile.IsDeleted;
                        returnobj.DeletedBy = residentprofile.DeletedBy;
                        returnobj.DeletedOn = residentprofile.DeletedOn;
                        returnobj.DetailJson = residentprofile.DetailJson;

                        return Ok(new { statusCode = StatusCodes.Status200OK, message = "Resident profile.", data = returnobj });
                    }

            }
            catch (Exception ex)
            {
                return BadRequest("Invalid resident");
            }

        }

        [Authorize(Policy = Utilities.Module.Resident)]
        [Authorize(Policy = Utilities.Permission.Get)]
        [HttpGet("get-all-country")]
        public async Task<ActionResult<IEnumerable<CountryDto>>> GetAllCountry()
        {
            var currentUserId = User.FindFirst(ClaimTypes.Name)?.Value;

            if (string.IsNullOrEmpty(currentUserId))
            {
                return NotFound(new
                {
                    statusCode = StatusCodes.Status404NotFound,
                    message = "Invalid or Expired Token."
                });
            }

            var countries = await _context.Country
         .Where(s => s.IsDeleted != true && s.IsActive == true)
         .OrderBy(s => s.DisplayOrder)
         .ToListAsync();

            return Ok(_mapper.Map<List<CountryDto>>(countries));
        }

        [Authorize(Policy = Utilities.Module.Resident)]
        [Authorize(Policy = Utilities.Permission.Get)]
        [HttpGet("get-all-state-by-country-id")]
        public async Task<ActionResult<IEnumerable<StateDto>>> GetAllStateByCountryId(int CountryId)
        {
            var currentUserId = User.FindFirst(ClaimTypes.Name)?.Value;

            if (string.IsNullOrEmpty(currentUserId))
            {
                return NotFound(new
                {
                    statusCode = StatusCodes.Status404NotFound,
                    message = "Invalid or Expired Token."
                });
            }

            var states = await _context.State
                .Where(s => s.IsDeleted != true && s.IsActive == true && s.CountryId == CountryId)
                .OrderBy(s => s.DisplayOrder)
                .ToListAsync();

            return Ok(_mapper.Map<List<StateDto>>(states));
        }

        [Authorize(Policy = Utilities.Module.Resident)]
        [Authorize(Policy = Utilities.Permission.Get)]
        [HttpGet("get-all-city-by-state-id")]
        public async Task<ActionResult<IEnumerable<CityDto>>> GetAllCityByStateId(int StateId)
        {
            var currentUserId = User.FindFirst(ClaimTypes.Name)?.Value;

            if (string.IsNullOrEmpty(currentUserId))
            {
                return NotFound(new
                {
                    statusCode = StatusCodes.Status404NotFound,
                    message = "Invalid or Expired Token."
                });
            }

            var cities = await _context.City
                .Where(s => s.IsDeleted != true && s.IsActive == true && s.StateId == StateId)
                .OrderBy(s => s.DisplayOrder)
                .ToListAsync();

            return Ok(_mapper.Map<List<CityDto>>(cities));
        }

        [Authorize(Policy = Utilities.Module.Resident)]
        [Authorize(Policy = Utilities.Permission.Get)]
        [HttpGet("get-all-society-by-city-id")]
        public async Task<ActionResult<IEnumerable<SocietyDto>>> GetAllSocietyByCityId(int CityId)
        {
            var currentUserId = User.FindFirst(ClaimTypes.Name)?.Value;

            if (string.IsNullOrEmpty(currentUserId))
            {
                return NotFound(new
                {
                    statusCode = StatusCodes.Status404NotFound,
                    message = "Invalid or Expired Token."
                });
            }

            var societies = await _context.Society
                .Where(s => s.IsDeleted != true && s.IsActive == true && s.CityId == CityId && s.IsApproved == true)
                .OrderBy(s => s.Name)
                .ToListAsync();

            return Ok(_mapper.Map<List<SocietyDto>>(societies));
        }

        [Authorize(Policy = Utilities.Module.Resident)]
        [Authorize(Policy = Utilities.Permission.Get)]
        [HttpGet("get-all-building-by-society-id")]
        public async Task<ActionResult<IEnumerable<SocietyBuildingDTO>>> GetAllBuildingBySocietyId(Guid SocietyId)
        {
            var currentUserId = User.FindFirst(ClaimTypes.Name)?.Value;

            if (string.IsNullOrEmpty(currentUserId))
            {
                return NotFound(new
                {
                    statusCode = StatusCodes.Status404NotFound,
                    message = "Invalid or Expired Token."
                });
            }

            var societyBuildings = await _context.SocietyBuilding
                .Where(s => s.IsDeleted != true && s.IsActive == true && s.SocietyId == SocietyId)
                .OrderBy(s => s.Name)
                .ToListAsync();

            return Ok(_mapper.Map<List<SocietyBuildingDTO>>(societyBuildings));
        }

        [Authorize(Policy = Utilities.Module.Resident)]
        [Authorize(Policy = Utilities.Permission.Get)]
        [HttpGet("get-all-flats-by-society-building-id")]
        public async Task<ActionResult<IEnumerable<SocietyFlatDTO>>> GetAllFlatsBySocietyBuildingId(Guid SocietyBuildingId)
        {
            var currentUserId = User.FindFirst(ClaimTypes.Name)?.Value;

            if (string.IsNullOrEmpty(currentUserId))
            {
                return NotFound(new
                {
                    statusCode = StatusCodes.Status404NotFound,
                    message = "Invalid or Expired Token."
                });
            }

            var societyBuildingFlats = await _context.SocietyFlat
                .Where(s => s.IsDeleted != true && s.IsActive == true && s.SocietyBuildingId == SocietyBuildingId)
                .OrderBy(s => s.Name)
                .ToListAsync();

            return Ok(_mapper.Map<List<SocietyFlatDTO>>(societyBuildingFlats));
        }

        [Authorize(Policy = Utilities.Module.Resident)]
        [Authorize(Policy = Utilities.Permission.Get)]
        [HttpGet("get-all-resident-types")]
        public async Task<IActionResult> GetAllResidentTypes(int EntityTypeId)
        {
            var currentUserId = User.FindFirst(ClaimTypes.Name)?.Value;

            if (string.IsNullOrEmpty(currentUserId))
            {
                return NotFound(new
                {
                    statusCode = StatusCodes.Status404NotFound,
                    message = "Invalid or Expired Token."
                });
            }

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

        [Authorize(Policy = Utilities.Module.Resident)]
        [Authorize(Policy = Utilities.Permission.Update)]
        [HttpPost("update-resident-selected-type")]
        public async Task<IActionResult> UpdateResidentSelectedType([FromForm] SelectedRecord obj)
        {
            try
            {
                var currentUserId = User.FindFirst(ClaimTypes.Name)?.Value;

                if (string.IsNullOrEmpty(currentUserId))
                {
                    return NotFound(new
                    {
                        statusCode = StatusCodes.Status404NotFound,
                        message = "Invalid or Expired Token."
                    });
                }

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
                extensionFromFileName = Path.GetExtension(selectedRecords.file.FileName)?.TrimStart('.');
                }



                bool matchingDetail = fullResponse.Any(entity =>
                {
                    return entity.DetailJson.Any(detailItem =>
                    {
                        return detailItem.Details.Any(detail =>
                        {
                            bool isBasicMatch =
                                detail.fieldText == selectedRecords.fieldText &&
                                detail.fieldId == selectedRecords.fieldId &&
                                detail.fieldName == selectedRecords.fieldName &&
                                detail.recordType == selectedRecords.recordType;

                            bool isTypeMatch =
                                (detail.type == null && selectedRecords.type == null) ||
                                (detail.type != null && selectedRecords.type != null &&
                                 detail.type.Contains(selectedRecords.type) &&
                                 detail.type.Contains(extensionFromFile.ToLower()) &&
                                 detail.type.Contains(extensionFromFileName.ToLower()));

                            return isBasicMatch && isTypeMatch;
                        });
                    });
                });


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
                    updateResident.SocietyFlatId = obj.societyFlatId;
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
        #endregion

        #region FamilyAPIs

        [Authorize(Policy = Utilities.Module.Resident)]
        [Authorize(Policy = Utilities.Permission.Get)]
        [HttpGet("get-resident-household-family-form")]
        public async Task<IActionResult> GetResidentHouseholdFamilyForm()
        {
            try
            {
                var currentUserId = User.FindFirst(ClaimTypes.Name)?.Value;

                if (string.IsNullOrEmpty(currentUserId))
                {
                    return NotFound(new
                    {
                        statusCode = StatusCodes.Status404NotFound,
                        message = "Invalid or Expired Token."
                    });
                }

                Type type = typeof(Utilities.EntityTypeDetail);
                FieldInfo field = type.GetField("Family", BindingFlags.Public | BindingFlags.Static);

                int EntityTypeDetailValue = 0;

                if (field != null && field.IsLiteral && field.FieldType == typeof(string))
                {
                    string value = (string)field.GetRawConstantValue();

                    // Convert to int if needed
                    if (int.TryParse(value, out int parsedValue))
                    {
                        EntityTypeDetailValue = parsedValue;
                    }
                }

                var result = await _context.EntityTypeDetail
                .Where(d => d.IsDeleted != true && d.IsActive == true && d.Id == EntityTypeDetailValue)
                .OrderBy(d => d.Id)
                .FirstOrDefaultAsync();

            List<FamilyEntityTypeDetailModel> entityTypeDetails = new List<FamilyEntityTypeDetailModel>();

            if (result !=null)
            {
                    var entity = new FamilyEntityTypeDetailModel
                    {
                        Id = result.Id,
                        EntityTypeId = result.EntityTypeId,
                        Value = result.Value,
                        Label = result.Label,
                        FamilyType = JsonConvert.DeserializeObject<FamilyRoot>(result.DetailJson),
                        IsDafault = result.IsDafault,
                        IsActive = result.IsActive,
                        CreatedBy = result.CreatedBy,
                        CreatedOn = result.CreatedOn,
                        UpdatedBy = result.UpdatedBy,
                        UpdatedOn = result.UpdatedOn,
                        IsDeleted = result.IsDeleted,
                        DeletedBy = result.DeletedBy,
                        DeletedOn = result.DeletedOn
                    };

                    entityTypeDetails.Add(entity);
                
            }

            if (entityTypeDetails.Count == 0)
            {
                return NotFound("No Household found, contact administration.");
            }

            return Ok(entityTypeDetails);

            }
            catch (Exception ex)
            {
                return NotFound(new { message = "No Household found, contact administration." });
            }
        }

        [Authorize(Policy = Utilities.Module.Resident)]
        [Authorize(Policy = Utilities.Permission.Add)]
        [HttpPost("add-resident-household-family")]
        public async Task<IActionResult> AddResidentHouseholdFamily([FromForm] AddFamilySelectedRecord obj)
        {
            try
            {
                var currentUserId = User.FindFirst(ClaimTypes.Name)?.Value;

                if (string.IsNullOrEmpty(currentUserId))
                {
                    return NotFound(new
                    {
                        statusCode = StatusCodes.Status404NotFound,
                        message = "Invalid or Expired Token."
                    });
                }

                var mismatchMessages = new List<MismatchOutput>();

                bool IsFileRequired = false;

                Type type = typeof(Utilities.EntityTypeDetail);

                FieldInfo field = type.GetField("Family", BindingFlags.Public | BindingFlags.Static);

                int EntityTypeDetailValue = 0;

                if (field != null && field.IsLiteral && field.FieldType == typeof(string))
                {
                    string value = (string)field.GetRawConstantValue();

                    // Convert to int if needed
                    if (int.TryParse(value, out int parsedValue))
                    {
                        EntityTypeDetailValue = parsedValue;
                    }
                }


                var result = await _context.EntityTypeDetail
                                     .Where(d => d.IsDeleted != true && d.IsActive == true && d.Id == EntityTypeDetailValue)
                                     .OrderBy(d => d.Id)
                                     .FirstOrDefaultAsync();

             

                string extensionFromFile = null;
                string extensionFromFileName = null;

                FamilyEntityTypeDetailModel entityTypeDetails = null;

                if (result != null)
                {
                    var entity = new FamilyEntityTypeDetailModel
                    {
                        Id = result.Id,
                        EntityTypeId = result.EntityTypeId,
                        Value = result.Value,
                        Label = result.Label,
                        FamilyType = JsonConvert.DeserializeObject<FamilyRoot>(result.DetailJson),
                        IsDafault = result.IsDafault,
                        IsActive = result.IsActive,
                        CreatedBy = result.CreatedBy,
                        CreatedOn = result.CreatedOn,
                        UpdatedBy = result.UpdatedBy,
                        UpdatedOn = result.UpdatedOn,
                        IsDeleted = result.IsDeleted,
                        DeletedBy = result.DeletedBy,
                        DeletedOn = result.DeletedOn
                    };

                    entityTypeDetails = entity;

                }

                if (entityTypeDetails == null)
                {
                    return NotFound("No Household found, contact administration.");
                }

                // ✅ Normalize the input to lower case for matching
                string familyType = obj.familyType?.Trim().ToLower();

                bool familyTypeExists =
    (familyType == "adult" && entityTypeDetails.FamilyType?.Adult != null && entityTypeDetails.FamilyType.Adult.Any()) ||
    (familyType == "kid" && entityTypeDetails.FamilyType?.Kid != null && entityTypeDetails.FamilyType.Kid.Any());

                if (!familyTypeExists)
                {
                    return NotFound(new { message = $"No configuration found for family type '{obj.familyType}'." });
                }

                AddResidentFamily addResidentFamily = new AddResidentFamily();

                var getResidentIdByUserId = await _context.Resident
                      .Where(d => d.IsDeleted != true && d.IsActive == true && d.UserId == Guid.Parse(currentUserId))
                      .Select(d => d.Id)
                      .FirstOrDefaultAsync();


                if (getResidentIdByUserId == null)
                {
                    return NotFound(new { message = "Resident not found." });
                }

                if (familyType == "adult")
                {
                    if (obj.file is not null)
                    {
                        extensionFromFile = Path.GetExtension(ContentDispositionHeaderValue.Parse(obj.file.ContentDisposition)
                                            .FileName.ToString().Trim('"'))?.TrimStart('.').ToLower();
                    }

                    if (!string.IsNullOrWhiteSpace(obj.fileName))
                    {
                        extensionFromFileName = Path.GetExtension(obj.fileName)?.TrimStart('.').ToLower();
                    }

                    bool isValidExtension = entityTypeDetails.FamilyType.Adult
                                          .Where(detail => detail.type != null && detail.type.Any())
                                          .Any(detail =>
                                              (extensionFromFile != null && detail.type.Contains(extensionFromFile)) &&
                                              (extensionFromFileName != null && detail.type.Contains(extensionFromFileName))
                                          );

                    if (!isValidExtension)
                    {
                        return BadRequest(new { message = $"File not in correct format." });
                    }

                    var blobName = "";

                    // Upload file
                    if (obj.file is not null)
                    {
                        blobName = await ResidentFamilyUploadProfilePictureAsync(obj.file.FileName, obj.file.OpenReadStream());
                    }
                    else
                    {
                        blobName = null;
                    }

                    addResidentFamily.ProfilePicture = blobName;
                    addResidentFamily.ExitType = null;


                }
                else // Kid
                {
                    bool isValidExtension = entityTypeDetails.FamilyType.Kid
                                         .Where(detail => detail.fieldType != null && detail.fieldType.ToLower() == "radio")
                                         .Any(detail =>
                                             !string.IsNullOrWhiteSpace(obj.exittype) &&
                                             detail.fieldId.Equals(obj.exittype, StringComparison.OrdinalIgnoreCase)
                                         );


                    if (!isValidExtension)
                    {
                        return BadRequest(new { message = $"Exit Type not matched." });
                    }


                    addResidentFamily.ProfilePicture = null;
                    addResidentFamily.ExitType = obj.exittype;
                }

                OTPGenerator otpGenerator = new OTPGenerator();
                string otp = otpGenerator.GenerateOTP();

                addResidentFamily.Id = Guid.NewGuid();
                addResidentFamily.ResidentId = getResidentIdByUserId;
                addResidentFamily.FamilyType = obj.familyType;
                addResidentFamily.Name = obj.name;
                addResidentFamily.MobileNumber = obj.mobileNumber;
                addResidentFamily.ResidentFamilyCode = otp;
                addResidentFamily.IsActive = true;
                addResidentFamily.CreatedOn = DateTime.UtcNow;
                addResidentFamily.CreatedBy = currentUserId;
                addResidentFamily.IsDeleted = false;

                var returnResult = await AddResidentFamilyAsync(addResidentFamily);


                if (returnResult != null)
                {

                    return Ok(returnResult);
                }
                else
                {
                    return NotFound(new { Message = "Unable to Add Family" });
                }

            }
            catch (System.Text.Json.JsonException)
            {
                return BadRequest();
            }
        }

        [Authorize(Policy = Utilities.Module.Resident)]
        [Authorize(Policy = Utilities.Permission.Get)]
        [HttpGet("get-all-resident-household-family")]
        public async Task<ActionResult<IEnumerable<ResidentFamilyDto>>> GetAllResidentHouseholdFamily()
        {
            try
            {
                var currentUserId = User.FindFirst(ClaimTypes.Name)?.Value;

                if (string.IsNullOrEmpty(currentUserId))
                {
                    return NotFound(new
                    {
                        statusCode = StatusCodes.Status404NotFound,
                        message = "Invalid or Expired Token."
                    });
                }

                var getResidentByUserId = await _context.Resident
                .Where(s => s.IsDeleted != true && s.IsActive == true && s.UserId == Guid.Parse(currentUserId))
                .Select(s => s.Id)
                .FirstOrDefaultAsync();

            if (getResidentByUserId == null)
            {
                return NotFound(new { message = "Resident not found." });
            }

            var resudentFamilies = await _context.ResidentFamily
                .Where(s => s.IsDeleted != true && s.IsActive == true && s.ResidentId == getResidentByUserId)
                .OrderBy(s => s.Name)
                .ToListAsync();

            if (resudentFamilies.Count == 0)
            {
                return Ok(new { message = "No family added." });
            }

            return Ok(_mapper.Map<List<ResidentFamilyDto>>(resudentFamilies));

            }
            catch (Exception ex)
            {
                return NotFound(new { message = "No family added." });
            }
        }

        [Authorize(Policy = Utilities.Module.Resident)]
        [Authorize(Policy = Utilities.Permission.Update)]
        [HttpPost("update-resident-household-family-by-id")]
        public async Task<IActionResult> UpdateResidentHouseholdFamilyById([FromForm] UpdateFamilySelectedRecord obj)
        {
            try
            {
                var currentUserId = User.FindFirst(ClaimTypes.Name)?.Value;

                if (string.IsNullOrEmpty(currentUserId))
                {
                    return NotFound(new
                    {
                        statusCode = StatusCodes.Status404NotFound,
                        message = "Invalid or Expired Token."
                    });
                }

                var getFamilyById = await _context.ResidentFamily
                                    .Where(d => d.IsDeleted != true && d.IsActive == true && d.Id == obj.ResidentFamilyId)
                                    .OrderBy(d => d.Id)
                                    .FirstOrDefaultAsync();

                if (getFamilyById == null)
                {
                    return NotFound(new {message = "Family not found."});
                }

                var mismatchMessages = new List<MismatchOutput>();

                bool IsFileRequired = false;

                Type type = typeof(Utilities.EntityTypeDetail);

                FieldInfo field = type.GetField("Family", BindingFlags.Public | BindingFlags.Static);

                int EntityTypeDetailValue = 0;

                if (field != null && field.IsLiteral && field.FieldType == typeof(string))
                {
                    string value = (string)field.GetRawConstantValue();

                    // Convert to int if needed
                    if (int.TryParse(value, out int parsedValue))
                    {
                        EntityTypeDetailValue = parsedValue;
                    }
                }


                var result = await _context.EntityTypeDetail
                                     .Where(d => d.IsDeleted != true && d.IsActive == true && d.Id == EntityTypeDetailValue)
                                     .OrderBy(d => d.Id)
                                     .FirstOrDefaultAsync();



                string extensionFromFile = null;
                string extensionFromFileName = null;

                FamilyEntityTypeDetailModel entityTypeDetails = null;

                if (result != null)
                {
                    var entity = new FamilyEntityTypeDetailModel
                    {
                        Id = result.Id,
                        EntityTypeId = result.EntityTypeId,
                        Value = result.Value,
                        Label = result.Label,
                        FamilyType = JsonConvert.DeserializeObject<FamilyRoot>(result.DetailJson),
                        IsDafault = result.IsDafault,
                        IsActive = result.IsActive,
                        CreatedBy = result.CreatedBy,
                        CreatedOn = result.CreatedOn,
                        UpdatedBy = result.UpdatedBy,
                        UpdatedOn = result.UpdatedOn,
                        IsDeleted = result.IsDeleted,
                        DeletedBy = result.DeletedBy,
                        DeletedOn = result.DeletedOn
                    };

                    entityTypeDetails = entity;

                }

                if (entityTypeDetails == null)
                {
                    return NotFound("No Household found, contact administration.");
                }

                // ✅ Normalize the input to lower case for matching
                string familyType = obj.familyType?.Trim().ToLower();

                bool familyTypeExists =
    (familyType == "adult" && entityTypeDetails.FamilyType?.Adult != null && entityTypeDetails.FamilyType.Adult.Any()) ||
    (familyType == "kid" && entityTypeDetails.FamilyType?.Kid != null && entityTypeDetails.FamilyType.Kid.Any());

                if (!familyTypeExists)
                {
                    return NotFound(new { message = $"No configuration found for family type '{obj.familyType}'." });
                }

                UpdateResidentFamily updateResidentFamily = new UpdateResidentFamily();

                var getResidentIdByUserId = await _context.Resident
                      .Where(d => d.IsDeleted != true && d.IsActive == true && d.UserId == Guid.Parse(currentUserId))
                      .Select(d => d.Id)
                      .FirstOrDefaultAsync();


                if (getResidentIdByUserId == null)
                {
                    return NotFound(new { message = "Family not found." });
                }

                if (familyType == "adult")
                {
                    if (obj.file is not null)
                    {
                        extensionFromFile = Path.GetExtension(ContentDispositionHeaderValue.Parse(obj.file.ContentDisposition)
                                            .FileName.ToString().Trim('"'))?.TrimStart('.').ToLower();


                        if (!string.IsNullOrWhiteSpace(obj.fileName))
                        {
                            extensionFromFileName = Path.GetExtension(obj.fileName)?.TrimStart('.').ToLower();
                        }

                        bool isValidExtension = entityTypeDetails.FamilyType.Adult
                                              .Where(detail => detail.type != null && detail.type.Any())
                                              .Any(detail =>
                                                  (extensionFromFile != null && detail.type.Contains(extensionFromFile)) &&
                                                  (extensionFromFileName != null && detail.type.Contains(extensionFromFileName))
                                              );

                        if (!isValidExtension)
                        {
                            return BadRequest(new { message = $"File not in correct format." });
                        }
                    }

                    var blobName = "";

                    // Upload file
                    if (obj.file is not null)
                    {
                        blobName = await ResidentFamilyUploadProfilePictureAsync(obj.file.FileName, obj.file.OpenReadStream());
                    }
                    else
                    {
                        blobName = getFamilyById.ProfilePicture;
                    }

                    updateResidentFamily.ProfilePicture = blobName;
                    updateResidentFamily.ExitType = null;


                }
                else // Kid
                {
                    bool isValidExtension = entityTypeDetails.FamilyType.Kid
                                         .Where(detail => detail.fieldType != null && detail.fieldType.ToLower() == "radio")
                                         .Any(detail =>
                                             !string.IsNullOrWhiteSpace(obj.exittype) &&
                                             detail.fieldId.Equals(obj.exittype, StringComparison.OrdinalIgnoreCase)
                                         );


                    if (!isValidExtension)
                    {
                        return BadRequest(new { message = $"Exit Type not matched." });
                    }


                    updateResidentFamily.ProfilePicture = null;
                    updateResidentFamily.ExitType = obj.exittype;
                }

                updateResidentFamily.ResidentFamilyId = getFamilyById.Id;
                updateResidentFamily.FamilyType = obj.familyType;
                updateResidentFamily.Name = obj.name;
                updateResidentFamily.MobileNumber = obj.mobileNumber;
                updateResidentFamily.IsActive = true;
                updateResidentFamily.UpdatedBy = currentUserId;
                updateResidentFamily.UpdatedOn = DateTime.UtcNow;

                if (obj.IsDeleted != null && obj.IsDeleted == true)
                {
                    updateResidentFamily.IsDeleted = true;
                    updateResidentFamily.DeletedBy = currentUserId;
                    updateResidentFamily.DeletedOn = DateTime.UtcNow;
                    updateResidentFamily.IsActive = false;
                }
                    

                var returnResult = await UpdateResidentFamilyAsync(updateResidentFamily);


                if (returnResult != null)
                {

                    return Ok(returnResult);
                }
                else
                {
                    return NotFound(new { Message = "Unable to update family" });
                }

            }
            catch (System.Text.Json.JsonException)
            {
                return BadRequest();
            }
        }

        #endregion

        #region PetAPIs

        [Authorize(Policy = Utilities.Module.Resident)]
        [Authorize(Policy = Utilities.Permission.Get)]
        [HttpGet("get-resident-household-pet-form")]
        public async Task<IActionResult> GetResidentHouseholdPetForm()
        {
            try
            {
                var currentUserId = User.FindFirst(ClaimTypes.Name)?.Value;

                if (string.IsNullOrEmpty(currentUserId))
                {
                    return NotFound(new
                    {
                        statusCode = StatusCodes.Status404NotFound,
                        message = "Invalid or Expired Token."
                    });
                }

                Type type = typeof(Utilities.EntityTypeDetail);
                FieldInfo field = type.GetField("Pets", BindingFlags.Public | BindingFlags.Static);

                int EntityTypeDetailValue = 0;

                if (field != null && field.IsLiteral && field.FieldType == typeof(string))
                {
                    string value = (string)field.GetRawConstantValue();

                    // Convert to int if needed
                    if (int.TryParse(value, out int parsedValue))
                    {
                        EntityTypeDetailValue = parsedValue;
                    }
                }



                var result = await _context.EntityTypeDetail
                .Where(d => d.IsDeleted != true && d.IsActive == true && d.Id == EntityTypeDetailValue)
                .OrderBy(d => d.Id)
                .FirstOrDefaultAsync();

                var entityTypeInput = JsonConvert.DeserializeObject<EntityTypeInput>(result.DetailJson);

                int entityTypeId = int.Parse(entityTypeInput.EntityTypeId); // convert "9" to int

                var getAllPetTypesByEntityTypeId = await _context.EntityTypeDetail
               .Where(d => d.IsActive == true && d.EntityTypeId == entityTypeId)
               .OrderBy(d => d.Id)
               .ToListAsync();

                if (getAllPetTypesByEntityTypeId.Count == 0)
                {
                    return Ok(new { message = "No Pet found" });
                }

                var returnResult = getAllPetTypesByEntityTypeId.Select(r =>
                {
                    var detailObj = JsonConvert.DeserializeObject<PetDetails>(r.DetailJson!);

                    return new
                    {
                        Id = r.Id,
                        EntityTypeId = r.EntityTypeId,
                        Value = r.Value,
                        Label = r.Label,
                        WebImage = detailObj.WebImage,
                        MobileImage = detailObj.MobileImage,
                        Breeds = detailObj.Breed,
                        detailObj.Questions,
                    };
                }).ToList();

                if (returnResult.Count > 0)
                {
                    return Ok(returnResult);
                }
                else
                {
                    return NotFound(new { message = "No Pet found, contact administration." });
                }

            }
            catch (Exception ex)
            {
                return NotFound(new { message = "No Pet found, contact administration." });
            }
        }

        [Authorize(Policy = Utilities.Module.Resident)]
        [Authorize(Policy = Utilities.Permission.Add)]
        [HttpPost("add-resident-household-pet")]
        public async Task<IActionResult> AddResidentHouseholdPet([FromForm] PetRequest petDataJson)
        {
            try
            {
                var currentUserId = User.FindFirst(ClaimTypes.Name)?.Value;

                if (string.IsNullOrEmpty(currentUserId))
                {
                    return NotFound(new
                    {
                        statusCode = StatusCodes.Status404NotFound,
                        message = "Invalid or Expired Token."
                    });
                }

                Type type = typeof(Utilities.EntityTypeDetail);
                FieldInfo field = type.GetField("Pets", BindingFlags.Public | BindingFlags.Static);

                int EntityTypeDetailValue = 0;

                if (field != null && field.IsLiteral && field.FieldType == typeof(string))
                {
                    string value = (string)field.GetRawConstantValue();

                    // Convert to int if needed
                    if (int.TryParse(value, out int parsedValue))
                    {
                        EntityTypeDetailValue = parsedValue;
                    }
                }



                var result = await _context.EntityTypeDetail
                .Where(d => d.IsDeleted != true && d.IsActive == true && d.Id == EntityTypeDetailValue)
                .OrderBy(d => d.Id)
                .FirstOrDefaultAsync();

                var entityTypeInput = JsonConvert.DeserializeObject<EntityTypeInput>(result.DetailJson);

                int entityTypeId = int.Parse(entityTypeInput.EntityTypeId); // convert "9" to int

                var getAllPetTypesByEntityTypeId = await _context.EntityTypeDetail
               .Where(d => d.IsActive == true && d.EntityTypeId == entityTypeId)
               .OrderBy(d => d.Id)
               .ToListAsync();

                if (getAllPetTypesByEntityTypeId.Count == 0)
                {
                    return Ok(new { message = "No Pet found" });
                }

                var returnResult = getAllPetTypesByEntityTypeId.Select(r =>
                {
                    var detailObj = JsonConvert.DeserializeObject<PetDetails>(r.DetailJson!);

                    return new
                    {
                        Id = r.Id,
                        EntityTypeId = r.EntityTypeId,
                        Value = r.Value,
                        Label = r.Label,
                        WebImage = detailObj.WebImage,
                        MobileImage = detailObj.MobileImage,
                        Breeds = detailObj.Breed,
                        detailObj.Questions,
                    };
                }).ToList();

                if (returnResult.Count > 0)
                {
                    var match = returnResult.FirstOrDefault(x =>
                                  x.Id == petDataJson.Id &&
                                  string.Equals(x.Label, petDataJson.Label, StringComparison.OrdinalIgnoreCase) &&
                                  x.Breeds.Any(b => string.Equals(b.Name, petDataJson.BreedName, StringComparison.OrdinalIgnoreCase))
                              );

                    if (match == null)
                    {
                        return BadRequest(new { message = "Pet detail mismatch. Please check the id, label, or breed name." });
                    }

                    if (petDataJson.petFile == null || petDataJson.petFile.Length == 0)
                    {
                        return BadRequest(new { message = "Pet profile picture is required." });
                    }

                    var allowedExtensions = new[] { ".jpg", ".jpeg", ".png" };
                    var fileExtension = Path.GetExtension(petDataJson.petFile.FileName).ToLowerInvariant();
                    const long maxFileSize = 2 * 1024 * 1024; // 2 MB

                    if (!allowedExtensions.Contains(fileExtension))
                    {
                        return BadRequest(new { message = "Only .jpg, .jpeg, or .png formats are allowed." });
                    }

                    if (petDataJson.petFile.Length > maxFileSize)
                    {
                        return BadRequest(new { message = "Pet profile picture must not exceed 2 MB." });
                    }

                    // Step 1: Deserialize
                    var getQAnswersList = JsonConvert.DeserializeObject<List<QuestionDto>>(petDataJson.Questions);

                    // Step 2: Ensure all 3 required questions are present
                    var requiredQuestions = new[] { "Pet Name", "Pet Image", "Vaccination Done" };

                    string PetName = "";
                    string VaccinationDone = "";

                    foreach (var required in requiredQuestions)
                    {
                        var question = getQAnswersList.FirstOrDefault(q =>
                            string.Equals(q.QuestionText.Trim(), required, StringComparison.OrdinalIgnoreCase));

                        if (question == null)
                        {
                            return BadRequest(new { message = $"Required question '{required}' is missing." });
                        }

                        if (string.IsNullOrWhiteSpace(question.QAnswerText))
                        {
                            return BadRequest(new { message = $"Answer for question '{required}' is required." });
                        }

                        if (required == "Pet Name")
                        {
                            PetName = question.QAnswerText;
                        }
                        // Step 3a: Pet Image - validate file extension
                        if (required.Equals("Pet Image", StringComparison.OrdinalIgnoreCase))
                        {
                            var extension = Path.GetExtension(question.QAnswerText)?.ToLowerInvariant();
                            if (!allowedExtensions.Contains(extension))
                            {
                                return BadRequest(new { message = "Pet Image must be in .jpg, .jpeg, or .png format." });
                            }
                        }

                        // Step 3b: Vaccination done - validate boolean
                        if (required.Equals("Vaccination Done", StringComparison.OrdinalIgnoreCase))
                        {
                            var value = question.QAnswerText.Trim().ToLowerInvariant();
                            if (value != "true" && value != "false")
                            {
                                return BadRequest(new { message = "Vaccination answer must be 'true' or 'false'." });
                            }

                            VaccinationDone = value;
                        }
                    }

                        var blobName = "";

                    blobName = await ResidentPetUploadProfilePictureAsync(petDataJson.petFile.FileName, petDataJson.petFile.OpenReadStream());

                    if (blobName == null)
                    {
                        return NotFound(new { message = "Pet profile picture failed to upload, contact administration." });
                    }

                    var getResidentIdByUserId = await _context.Resident
                   .Where(d => d.IsDeleted != true && d.IsActive == true && d.UserId == Guid.Parse(currentUserId))
                   .Select(d => d.Id)
                   .FirstOrDefaultAsync();


                    if (getResidentIdByUserId == null)
                    {
                        return NotFound(new { message = "Resident not found." });
                    }

                    AddResidentPet addResidentPet = new AddResidentPet();

                    addResidentPet.Id = Guid.NewGuid();
                    addResidentPet.ResidentId = getResidentIdByUserId;
                    addResidentPet.PetType = petDataJson.Label;
                    addResidentPet.PetBreed = petDataJson.BreedName;
                    addResidentPet.PetName = PetName;
                    addResidentPet.VaccinationType = VaccinationDone;
                    addResidentPet.PetPicture = blobName;
                    addResidentPet.IsActive = true;
                    addResidentPet.CreatedOn = DateTime.UtcNow;
                    addResidentPet.CreatedBy = currentUserId;
                    addResidentPet.IsDeleted = false;

                    var returnPet = await AddResidentPetAsync(addResidentPet);

                    if (returnPet != null)
                    {

                        return Ok(returnPet);
                    }
                    else
                    {
                        return NotFound(new { Message = "Unable to Add Pet" });
                    }
                }
                else
                {
                    return NotFound(new { message = "No Pet found, contact administration." });
                }

            }
            catch (System.Text.Json.JsonException)
            {
                return BadRequest();
            }
        }

        [Authorize(Policy = Utilities.Module.Resident)]
        [Authorize(Policy = Utilities.Permission.Get)]
        [HttpGet("get-all-resident-household-pet")]
        public async Task<ActionResult<IEnumerable<ResidentPetDto>>> GetAllResidentHouseholdPet()
        {
            try
            {

                var currentUserId = User.FindFirst(ClaimTypes.Name)?.Value;

                if (string.IsNullOrEmpty(currentUserId))
                {
                    return NotFound(new
                    {
                        statusCode = StatusCodes.Status404NotFound,
                        message = "Invalid or Expired Token."
                    });
                }

                var getResidentByUserId = await _context.Resident
                    .Where(s => s.IsDeleted != true && s.IsActive == true && s.UserId == Guid.Parse(currentUserId))
                    .Select(s => s.Id)
                    .FirstOrDefaultAsync();

                if (getResidentByUserId == null)
                {
                    return NotFound(new { message = "Resident not found." });
                }

                var residentPets = await _context.ResidentPet
                    .Where(s => s.IsDeleted != true && s.IsActive == true && s.ResidentId == getResidentByUserId)
                    .OrderBy(s => s.PetName)
                    .ToListAsync();

                if (residentPets.Count == 0)
                {
                    return Ok(new { message = "No pet added." });
                }

                return Ok(_mapper.Map<List<ResidentPetDto>>(residentPets));

            }
            catch (Exception ex)
            {
                return NotFound(new { message = "No pet added." });
            }
        }

        [Authorize(Policy = Utilities.Module.Resident)]
        [Authorize(Policy = Utilities.Permission.Update)]
        [HttpPost("update-resident-household-pet-by-id")]
        public async Task<IActionResult> UpdateResidentHouseholdPetById([FromForm] UpdatePetSelectedRecord obj)
        {
            try
            {
                var currentUserId = User.FindFirst(ClaimTypes.Name)?.Value;

                if (string.IsNullOrEmpty(currentUserId))
                {
                    return NotFound(new
                    {
                        statusCode = StatusCodes.Status404NotFound,
                        message = "Invalid or Expired Token."
                    });
                }

                var getPetById = await _context.ResidentPet
                                    .Where(d => d.IsDeleted != true && d.IsActive == true && d.Id == obj.ResidentPetId)
                                    .OrderBy(d => d.Id)
                                    .FirstOrDefaultAsync();

                if (getPetById == null)
                {
                    return NotFound(new { message = "Pet not found." });
                }

                var mismatchMessages = new List<MismatchOutput>();

                bool IsFileRequired = false;


                string extensionFromFile = null;
                string extensionFromFileName = null;


                UpdateResidentPet updateResidentPet = new UpdateResidentPet();

                var getResidentIdByUserId = await _context.Resident
                      .Where(d => d.IsDeleted != true && d.IsActive == true && d.UserId == Guid.Parse(currentUserId))
                      .Select(d => d.Id)
                      .FirstOrDefaultAsync();


                if (getResidentIdByUserId == null)
                {
                    return NotFound(new { message = "Resident not found." });
                }

                if (obj.vaccinationType.ToLower() == "true" || obj.vaccinationType.ToLower() == "false")
                {

                }
                else
                {
                    return NotFound(new { message = "Vaccination Type should be 'true' or 'false'." });
                }

                if (obj.file is not null)
                {
                    extensionFromFile = Path.GetExtension(ContentDispositionHeaderValue.Parse(obj.file.ContentDisposition)
                                        .FileName.ToString().Trim('"'))?.TrimStart('.');


                    var allowedExtensions = new[] { ".jpg", ".jpeg", ".png" };
                    var fileExtension = Path.GetExtension(obj.file.FileName).ToLowerInvariant();
                    const long maxFileSize = 2 * 1024 * 1024; // 2 MB

                    if (!allowedExtensions.Contains(fileExtension))
                    {
                        return BadRequest(new { message = "Only .jpg, .jpeg, or .png formats are allowed." });
                    }

                    if (obj.file.Length > maxFileSize)
                    {
                        return BadRequest(new { message = "Pet profile picture must not exceed 2 MB." });
                    }
                }

                    var blobName = "";

                    // Upload file
                    if (obj.file is not null)
                    {
                        blobName = await ResidentPetUploadProfilePictureAsync(obj.file.FileName, obj.file.OpenReadStream());
                    }
                    else
                    {
                        blobName = getPetById.PetPicture;
                    }



              
                

                updateResidentPet.ResidentPetId = getPetById.Id;
                updateResidentPet.PetName = obj.PetName;
                updateResidentPet.VaccinationType = obj.vaccinationType;
                updateResidentPet.PetPicture = blobName;
                updateResidentPet.IsActive = true;
                updateResidentPet.UpdatedBy = currentUserId;
                updateResidentPet.UpdatedOn = DateTime.UtcNow;

                if (obj.IsDeleted != null && obj.IsDeleted == true)
                {
                    updateResidentPet.IsDeleted = true;
                    updateResidentPet.DeletedBy = currentUserId;
                    updateResidentPet.DeletedOn = DateTime.UtcNow;
                    updateResidentPet.IsActive = false;
                }


                var returnResult = await UpdateResidentPetAsync(updateResidentPet);


                if (returnResult != null)
                {

                    return Ok(returnResult);
                }
                else
                {
                    return NotFound(new { Message = "Unable to update Pet" });
                }

            }
            catch (System.Text.Json.JsonException)
            {
                return BadRequest(new { Message = "Unable to update Pet" });
            }
        }


        #endregion

        #region VehicleAPIs

        [Authorize(Policy = Utilities.Module.Resident)]
        [Authorize(Policy = Utilities.Permission.Get)]
        [HttpGet("get-resident-household-vehicle-form")]
        public async Task<IActionResult> GetResidentHouseholdVehicleForm()
        {
            try
            {
                var currentUserId = User.FindFirst(ClaimTypes.Name)?.Value;

                if (string.IsNullOrEmpty(currentUserId))
                {
                    return NotFound(new
                    {
                        statusCode = StatusCodes.Status404NotFound,
                        message = "Invalid or Expired Token."
                    });
                }

                Type type = typeof(Utilities.EntityTypeDetail);
                FieldInfo field = type.GetField("Vehicles", BindingFlags.Public | BindingFlags.Static);

                int EntityTypeDetailValue = 0;

                if (field != null && field.IsLiteral && field.FieldType == typeof(string))
                {
                    string value = (string)field.GetRawConstantValue();

                    // Convert to int if needed
                    if (int.TryParse(value, out int parsedValue))
                    {
                        EntityTypeDetailValue = parsedValue;
                    }
                }

                var result = await _context.EntityTypeDetail
                .Where(d => d.IsDeleted != true && d.IsActive == true && d.Id == EntityTypeDetailValue)
                .OrderBy(d => d.Id)
                .FirstOrDefaultAsync();


                if (result != null)
                {

                   var  FamilyType = JsonConvert.DeserializeObject<List<VehicleFormField>>(result.DetailJson);

                    if (FamilyType != null)
                    {
                        return Ok(FamilyType);
                    }
                    else
                    {
                        return NotFound(new { message = "No Vehicle found." });
                    }

                }
                else
                {
                    return NotFound(new { message = "No Household found, contact administration." });
                }

               

            }
            catch (Exception ex)
            {
                return NotFound(new { message = "No Household found, contact administration." });
            }
        }

        [Authorize(Policy = Utilities.Module.Resident)]
        [Authorize(Policy = Utilities.Permission.Add)]
        [HttpPost("add-resident-household-vehicle")]
        public async Task<IActionResult> AddResidentHouseholdVehicle([FromForm] VehicleRequest vehicleData)
        {
            try
            {
                var currentUserId = User.FindFirst(ClaimTypes.Name)?.Value;

                if (string.IsNullOrEmpty(currentUserId))
                {
                    return NotFound(new
                    {
                        statusCode = StatusCodes.Status404NotFound,
                        message = "Invalid or Expired Token."
                    });
                }

                Type type = typeof(Utilities.EntityType);
                FieldInfo field = type.GetField("MaximumResidentVehicleAdd", BindingFlags.Public | BindingFlags.Static);

                int EntityTypeDetailValue = 0;

                if (field != null && field.IsLiteral && field.FieldType == typeof(string))
                {
                    string value = (string)field.GetRawConstantValue();

                    // Convert to int if needed
                    if (int.TryParse(value, out int parsedValue))
                    {
                        EntityTypeDetailValue = parsedValue;
                    }
                }

                var maxResidentVehicleAddLimit = await _context.EntityTypeDetail
                .Where(d => d.IsDeleted != true && d.IsActive == true && d.EntityTypeId == EntityTypeDetailValue)
                .ToListAsync();

                if (maxResidentVehicleAddLimit == null)
                {
                    return NotFound(new { message = "Maximum Vehicle Add limit not found." });
                }

                int countAllowed2Wheeler = maxResidentVehicleAddLimit.Count(v => v.Label != null && v.Label.Contains("2 Wheeler"));
                int countAllowed4Wheeler = maxResidentVehicleAddLimit.Count(v => v.Label != null && v.Label.Contains("4 Wheeler"));

              

                var getResidentIdByUserId = await _context.Resident
                  .Where(d => d.IsDeleted != true && d.IsActive == true && d.UserId == Guid.Parse(currentUserId))
                  .Select(d => d.Id)
                  .FirstOrDefaultAsync();

                if (getResidentIdByUserId == null)
                {
                    return NotFound(new { message = "Resident not found." });
                }

                var ResidentActiveVehicle = await _context.ResidentVehicle
                .Where(d => d.IsDeleted != true && d.IsActive == true && d.ResidentId == getResidentIdByUserId)
                .ToListAsync();


                int countActive2Wheeler = ResidentActiveVehicle.Count(v => v.VehicleType != null && v.VehicleType.Contains("2 Wheeler"));
                int countActive4Wheeler = ResidentActiveVehicle.Count(v => v.VehicleType != null && v.VehicleType.Contains("4 Wheeler"));

                var vehicleType = new[] { "2 Wheeler", "4 Wheeler" };
                var fuelType = new[] { "Electric", "Petrol", "Diesel", "CNG" };


                if (!vehicleType.Contains(vehicleData.VehicleType))
                {
                    return NotFound(new { message = "Vehicle Type should be '2 Wheeler' or '4 Wheeler'." });
                }


                if (vehicleData.VehicleType.Contains("2 Wheeler"))
                {
                    if (countAllowed2Wheeler <= countActive2Wheeler)
                    {
                        return NotFound(new
                        {
                            message = $"You can add only '{countAllowed2Wheeler}' 2 Wheelers against your flat as per society rule."
                        });

                    }
                }
                else // 4 Wheeler
                {
                    if (countAllowed4Wheeler <= countActive4Wheeler)
                    {
                        return NotFound(new { message = $"You can add only '{countAllowed4Wheeler}' 4 Wheelers against your flat as per society rule." });
                    }
                }

                if (vehicleData.vehicleFile == null || vehicleData.vehicleFile.Length == 0)
                {
                    return BadRequest(new { message = "Vehicle picture is required." });
                }

                    var allowedExtensions = new[] { ".jpg", ".jpeg", ".png" };
                    var fileExtension = Path.GetExtension(vehicleData.vehicleFile.FileName).ToLowerInvariant();
                    const long maxFileSize = 2 * 1024 * 1024; // 2 MB

                    if (!allowedExtensions.Contains(fileExtension))
                    {
                        return BadRequest(new { message = "Only .jpg, .jpeg, or .png formats are allowed." });
                    }

                    if (vehicleData.vehicleFile.Length > maxFileSize)
                    {
                        return BadRequest(new { message = "Vehicle picture must not exceed 2 MB." });
                    }


                   
                if (!fuelType.Contains(vehicleData.FuelType))
                {
                    return NotFound(new { message = "Fuel Type should be 'Electric','Petrol','Diesel' or 'CNG'." });
                }

                var blobName = "";

                    blobName = await ResidentVehicleUploadProfilePictureAsync(vehicleData.vehicleFile.FileName, vehicleData.vehicleFile.OpenReadStream());

                    if (blobName == null)
                    {
                        return NotFound(new { message = "Vehicle picture failed to upload, contact administration." });
                    }


                    AddResidentVehicle addResidentVehicle = new AddResidentVehicle();

                    addResidentVehicle.Id = Guid.NewGuid();
                    addResidentVehicle.ResidentId = getResidentIdByUserId;
                    addResidentVehicle.VehicleName = vehicleData.VehicleName;
                    addResidentVehicle.VehicleNumber = vehicleData.VehicleNumber;
                    addResidentVehicle.VehicleType = vehicleData.VehicleType;
                    addResidentVehicle.VehiclePicture = blobName;
                    addResidentVehicle.FuelType = vehicleData.FuelType;
                    addResidentVehicle.RfidTagNumber = vehicleData.RfidTagNumber;
                    addResidentVehicle.Code = vehicleData.Code;
                    addResidentVehicle.NotifyMeOnEntryExit = vehicleData.NotifyMeOnEntryExit;
                    addResidentVehicle.CreatedOn = DateTime.UtcNow;
                    addResidentVehicle.CreatedBy = currentUserId;
                    addResidentVehicle.IsActive = true;
                    addResidentVehicle.IsDeleted = false;


                    var returnVehicle = await AddResidentVehicleAsync(addResidentVehicle);

                    if (returnVehicle != null)
                    {

                        return Ok(returnVehicle);
                    }
                    else
                    {
                        return NotFound(new { Message = "Unable to Add Vehicle" });
                    }
             

            }
            catch (System.Text.Json.JsonException)
            {
                return BadRequest();
            }
        }

        [Authorize(Policy = Utilities.Module.Resident)]
        [Authorize(Policy = Utilities.Permission.Get)]
        [HttpGet("get-all-resident-household-vehicle")]
        public async Task<ActionResult<IEnumerable<ResidentVehicleDto>>> GetAllResidentHouseholdVehicle()
        {
            try
            {
                var currentUserId = User.FindFirst(ClaimTypes.Name)?.Value;

                if (string.IsNullOrEmpty(currentUserId))
                {
                    return NotFound(new
                    {
                        statusCode = StatusCodes.Status404NotFound,
                        message = "Invalid or Expired Token."
                    });
                }

                var getResidentByUserId = await _context.Resident
                    .Where(s => s.IsDeleted != true && s.IsActive == true && s.UserId == Guid.Parse(currentUserId))
                    .Select(s => s.Id)
                    .FirstOrDefaultAsync();

                if (getResidentByUserId == null)
                {
                    return NotFound(new { message = "Resident not found." });
                }

                var residentVehicles = await _context.ResidentVehicle
                    .Where(s => s.IsDeleted != true && s.IsActive == true && s.ResidentId == getResidentByUserId)
                    .OrderByDescending(s => s.CreatedOn)
                    .ToListAsync();

                if (residentVehicles.Count == 0)
                {
                    return Ok(new { message = "No vehicle added." });
                }

                return Ok(_mapper.Map<List<ResidentVehicleDto>>(residentVehicles));

            }
            catch (Exception ex)
            {
                return NotFound(new { message = "No vehicle added." });
            }
        }

        [Authorize(Policy = Utilities.Module.Resident)]
        [Authorize(Policy = Utilities.Permission.Update)]
        [HttpPost("update-resident-household-vehicle-by-id")]
        public async Task<IActionResult> UpdateResidentHouseholdVehicleById([FromForm] UpdateVehicleSelectedRecord obj)
        {
            try
            {
                var currentUserId = User.FindFirst(ClaimTypes.Name)?.Value;

                if (string.IsNullOrEmpty(currentUserId))
                {
                    return NotFound(new
                    {
                        statusCode = StatusCodes.Status404NotFound,
                        message = "Invalid or Expired Token."
                    });
                }

                var getVehicleById = await _context.ResidentVehicle
                                    .Where(d => d.IsDeleted != true && d.IsActive == true && d.Id == obj.ResidentVehicleId)
                                    .OrderBy(d => d.Id)
                                    .FirstOrDefaultAsync();

                if (getVehicleById == null)
                {
                    return NotFound(new { message = "Vehicle not found." });
                }

                string extensionFromFile = null;
                string extensionFromFileName = null;


                UpdateResidentVehicle updateResidentVehicle = new UpdateResidentVehicle();

                var getResidentIdByUserId = await _context.Resident
                      .Where(d => d.IsDeleted != true && d.IsActive == true && d.UserId == Guid.Parse(currentUserId))
                      .Select(d => d.Id)
                      .FirstOrDefaultAsync();


                if (getResidentIdByUserId == null)
                {
                    return NotFound(new { message = "Resident not found." });
                }


                if (obj.file is not null)
                {
                    extensionFromFile = Path.GetExtension(ContentDispositionHeaderValue.Parse(obj.file.ContentDisposition)
                                        .FileName.ToString().Trim('"'))?.TrimStart('.');


                    var allowedExtensions = new[] { ".jpg", ".jpeg", ".png" };
                    var fileExtension = Path.GetExtension(obj.file.FileName).ToLowerInvariant();
                    const long maxFileSize = 2 * 1024 * 1024; // 2 MB

                    if (!allowedExtensions.Contains(fileExtension))
                    {
                        return BadRequest(new { message = "Only .jpg, .jpeg, or .png formats are allowed." });
                    }

                    if (obj.file.Length > maxFileSize)
                    {
                        return BadRequest(new { message = "Vehicle picture must not exceed 2 MB." });
                    }
                }

                var blobName = "";

                // Upload file
                if (obj.file is not null)
                {
                    blobName = await ResidentVehicleUploadProfilePictureAsync(obj.file.FileName, obj.file.OpenReadStream());
                }
                else
                {
                    blobName = getVehicleById.VehiclePicture;
                }

                updateResidentVehicle.ResidentVehicleId = getVehicleById.Id;
                updateResidentVehicle.VehicleName = obj.VehicleName;
                updateResidentVehicle.VehicleNumber = obj.VehicleNumber;
                updateResidentVehicle.VehiclePicture = blobName;
                updateResidentVehicle.RfidTagNumber = obj.RfidTagNumber;
                updateResidentVehicle.Code = obj.Code;
                updateResidentVehicle.NotifyMeOnEntryExit = obj.NotifyMeOnEntryExit;
                updateResidentVehicle.IsActive = true;
                updateResidentVehicle.UpdatedBy = currentUserId;
                updateResidentVehicle.UpdatedOn = DateTime.UtcNow;

                if (obj.IsDeleted != null && obj.IsDeleted == true)
                {
                    updateResidentVehicle.IsDeleted = true;
                    updateResidentVehicle.DeletedBy = currentUserId;
                    updateResidentVehicle.DeletedOn = DateTime.UtcNow;
                    updateResidentVehicle.IsActive = false;
                }


                var returnResult = await UpdateResidentVehicleAsync(updateResidentVehicle);


                if (returnResult != null)
                {

                    return Ok(returnResult);
                }
                else
                {
                    return NotFound(new { Message = "Unable to update Vehicle" });
                }

            }
            catch (System.Text.Json.JsonException)
            {
                return BadRequest(new { Message = "Unable to update Vehicle" });
            }
        }

        #endregion

        #region Frequent GuestsAPIs


        [Authorize(Policy = Utilities.Module.Resident)]
        [Authorize(Policy = Utilities.Permission.Get)]
        [HttpGet("get-resident-household-frequent-guests-form")]
        public async Task<IActionResult> GetResidentHouseholdFrequentGuestsForm()
        {
            try
            {
                var currentUserId = User.FindFirst(ClaimTypes.Name)?.Value;

                if (string.IsNullOrEmpty(currentUserId))
                {
                    return NotFound(new
                    {
                        statusCode = StatusCodes.Status404NotFound,
                        message = "Invalid or Expired Token."
                    });
                }

                Type type = typeof(Utilities.EntityTypeDetail);
                FieldInfo field = type.GetField("FrequentGuests", BindingFlags.Public | BindingFlags.Static);

                int entityTypeDetailValue = 0;

                if (field != null && field.IsLiteral && field.FieldType == typeof(string))
                {
                    string value = (string)field.GetRawConstantValue();
                    if (int.TryParse(value, out int parsedValue))
                    {
                        entityTypeDetailValue = parsedValue;
                    }
                }

                var result = await _context.EntityTypeDetail
                    .Where(d => d.IsDeleted != true && d.IsActive == true && d.Id == entityTypeDetailValue)
                    .OrderBy(d => d.Id)
                    .FirstOrDefaultAsync();

                if (result != null)
                {
                    var frequentGuestsResponse = JsonConvert.DeserializeObject<GuestVisitForm>(result.DetailJson);

                    if (frequentGuestsResponse != null)
                    {
                        return Ok(frequentGuestsResponse);
                    }
                    else
                    {
                        return NotFound(new { message = "No Frequent Guests data found." });
                    }
                }
                else
                {
                    return NotFound(new { message = "No Frequent Guests found, contact administration." });
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred. Please contact administration.", details = ex.Message });
            }
        }

        [Authorize(Policy = Utilities.Module.Resident)]
        [Authorize(Policy = Utilities.Permission.Add)]
        [HttpPost("add-resident-household-frequent-guests")]
        public async Task<IActionResult> AddResidentHouseholdFrequentGuests([FromForm] AddResidentFrequentGuestsSelectedRecord obj)
        {
            try
            {
                var currentUserId = User.FindFirst(ClaimTypes.Name)?.Value;

                if (string.IsNullOrEmpty(currentUserId))
                {
                    return NotFound(new
                    {
                        statusCode = StatusCodes.Status404NotFound,
                        message = "Invalid or Expired Token."
                    });
                }

                AddResidentFrequentGuests addFrequentGuests = new AddResidentFrequentGuests();

                var getResidentIdByUserId = await _context.Resident
                      .Where(d => d.IsDeleted != true && d.IsActive == true && d.UserId == Guid.Parse(currentUserId))
                      .Select(d => d.Id)
                      .FirstOrDefaultAsync();


                if (getResidentIdByUserId == null)
                {
                    return NotFound(new { message = "Resident not found." });
                }

                string [] GuestType = ["Once", "Frequently"];

                bool isMatchGuestType = GuestType.Any(g => g.StartsWith(obj.GuestType, StringComparison.OrdinalIgnoreCase));

                // common check
                if (!isMatchGuestType)
                {
                    return NotFound(new { message = $"Guest Type not found, please choose any of: {string.Join(", ",GuestType)}'." });
                }


                if (obj.GuestType == "Once")
                {

                    if ((!obj.IsPrivateEntry== true) && (!obj.IsPrivateEntry == false))
                        return NotFound(new { message = "Is Private Entry required?" });


                    if (string.IsNullOrWhiteSpace(obj.StartDate))
                        return NotFound(new { message = "Start date required." });

                    if (!CheckValidDate(obj.StartDate))
                        return NotFound(new { message = "Start date must be in 'dd-MM-yyyy' format." });
                  

                    if (string.IsNullOrWhiteSpace(obj.StartingFrom))
                        return NotFound(new { message = "Starting time required." });

                    if (!CheckValidTime(obj.StartingFrom))
                        return NotFound(new { message = "Starting time must be in 'HH:mm:ss' format." });
               

                    string[] ValidFor = ["1 Hour","2 Hours","4 Hours","8 Hours","12 Hours","24 Hours"];

                    bool isMatchValidFor = ValidFor.Any(g => g.StartsWith(obj.ValidFor, StringComparison.OrdinalIgnoreCase));

                    if (!isMatchValidFor)
                        return NotFound(new { message = $"Valid for: {string.Join(", ",ValidFor)}" });

                 
                    addFrequentGuests.IsPrivateEntry = obj.IsPrivateEntry;
                    addFrequentGuests.SelectDate = DateOnly.FromDateTime(DateTime.ParseExact(obj.SelectDate, "dd-MM-yyyy", CultureInfo.InvariantCulture));
                    addFrequentGuests.StartingFrom = TimeOnly.FromDateTime(DateTime.ParseExact(obj.StartingFrom, "HH:mm:ss", CultureInfo.InvariantCulture));

                    addFrequentGuests.ValidFor = obj.ValidFor;

                }
                else if (obj.GuestType == "Frequently")
                {
                    string [] AllowEntryForNext = ["1 week","1 month","> 1 month"];

                    bool isMatchAllowEntryForNext = AllowEntryForNext.Any(g => g.StartsWith(obj.AllowEntryForNext, StringComparison.OrdinalIgnoreCase));

                    if (!isMatchAllowEntryForNext)
                    return NotFound(new { message = $"Match allowed entry for next: {string.Join(", ", AllowEntryForNext)}" });

                    if (string.IsNullOrWhiteSpace(obj.StartDate))
                        return NotFound(new { message = "Start date required." });

                    if (!CheckValidDate(obj.StartDate))
                        return NotFound(new { message = "Start date must be in 'dd-MM-yyyy' format." });

                    if (string.IsNullOrWhiteSpace(obj.EndDate))
                            return NotFound(new { message = "End date required." });

                    if (!CheckValidDate(obj.EndDate))
                        return NotFound(new { message = "End date must be in 'dd-MM-yyyy' format." });

                    addFrequentGuests.AllowEntryForNext = obj.AllowEntryForNext;
                    addFrequentGuests.StartDate = DateOnly.FromDateTime(DateTime.ParseExact(obj.StartDate, "dd-MM-yyyy", CultureInfo.InvariantCulture));
                    addFrequentGuests.EndDate = DateOnly.FromDateTime(DateTime.ParseExact(obj.EndDate, "dd-MM-yyyy", CultureInfo.InvariantCulture));

                }


                OTPGenerator otpGenerator = new OTPGenerator();
                string otp = otpGenerator.GenerateOTP();
                addFrequentGuests.UniqueEntryNumber = otp;

                addFrequentGuests.Id = Guid.NewGuid();
                addFrequentGuests.ResidentId = getResidentIdByUserId;
                addFrequentGuests.Type = obj.GuestType;
                addFrequentGuests.GuestName = obj.GuestName;
                addFrequentGuests.GuestNumber = obj.GuestNumber;
                addFrequentGuests.Note = obj.Note;
                addFrequentGuests.IsActive = true;
                addFrequentGuests.CreatedOn = DateTime.UtcNow;
                addFrequentGuests.CreatedBy = currentUserId;
                addFrequentGuests.IsDeleted = false;

                var returnResult = await AddResidentFrequentGuestsAsync(addFrequentGuests);


                if (returnResult != null)
                {

                    return Ok(returnResult);
                }
                else
                {
                    return NotFound(new { Message = "Unable to Add Frequent Guests" });
                }

            }
            catch (Exception ex)
            {
                return BadRequest();
            }
        }


        [Authorize(Policy = Utilities.Module.Resident)]
        [Authorize(Policy = Utilities.Permission.Get)]
        [HttpGet("get-all-resident-household-frequent-guests")]
        public async Task<ActionResult<IEnumerable<ResidentFrequentGuestsDto>>> GetAllResidentHouseholdFrequentGuests()
        {
            try
            {

                var currentUserId = User.FindFirst(ClaimTypes.Name)?.Value;

                if (string.IsNullOrEmpty(currentUserId))
                {
                    return NotFound(new
                    {
                        statusCode = StatusCodes.Status404NotFound,
                        message = "Invalid or Expired Token."
                    });
                }

                var getResidentByUserId = await _context.Resident
                    .Where(s => s.IsDeleted != true && s.IsActive == true && s.UserId == Guid.Parse(currentUserId))
                    .Select(s => s.Id)
                    .FirstOrDefaultAsync();

                if (getResidentByUserId == null)
                {
                    return NotFound(new { message = "Resident not found." });
                }

                var residentFrequentlyGuests = await _context.ResidentFrequentlyGuest
                    .Where(s => s.IsDeleted != true && s.IsActive == true && s.ResidentId == getResidentByUserId)
                    .OrderByDescending(s => s.CreatedOn)
                    .ToListAsync();

                if (residentFrequentlyGuests.Count == 0)
                {
                    return Ok(new { message = "No frequent guests added." });
                }

                return Ok(_mapper.Map<List<ResidentFrequentGuestsDto>>(residentFrequentlyGuests));

            }
            catch (Exception ex)
            {
                return NotFound(new { message = "No frequent guest added." });
            }
        }

        [Authorize(Policy = Utilities.Module.Resident)]
        [Authorize(Policy = Utilities.Permission.Update)]
        [HttpPost("update-resident-household-frequent-guests-by-id")]
        public async Task<IActionResult> UpdateResidentHouseholdFrequentGuestsById([FromForm] UpdateFrequentlyGuestsSelectedRecord obj)
        {
            try
            {
                var currentUserId = User.FindFirst(ClaimTypes.Name)?.Value;

                if (string.IsNullOrEmpty(currentUserId))
                {
                    return NotFound(new
                    {
                        statusCode = StatusCodes.Status404NotFound,
                        message = "Invalid or Expired Token."
                    });
                }

                var getFrequentlyGuestById = await _context.ResidentFrequentlyGuest
                                    .Where(d => d.IsDeleted != true && d.IsActive == true && d.Id == obj.ResidentFrequentGuestsId)
                                    .OrderBy(d => d.Id)
                                    .FirstOrDefaultAsync();

                if (getFrequentlyGuestById == null)
                {
                    return NotFound(new { message = "Frequently Guests not found." });
                }

                UpdateResidentFrequentlyGuests updateResidentFrequentlyGuests = new UpdateResidentFrequentlyGuests();

                var getResidentIdByUserId = await _context.Resident
                      .Where(d => d.IsDeleted != true && d.IsActive == true && d.UserId == Guid.Parse(currentUserId))
                      .Select(d => d.Id)
                      .FirstOrDefaultAsync();


                if (getResidentIdByUserId == null)
                {
                    return NotFound(new { message = "Resident not found." });
                }

                string[] GuestType = ["Once", "Frequently"];

                bool isMatchGuestType = GuestType.Any(g => g.StartsWith(obj.GuestType, StringComparison.OrdinalIgnoreCase));

                // common check
                if (!isMatchGuestType)
                {
                    return NotFound(new { message = $"Guest Type not found, please choose any of: {string.Join(", ", GuestType)}'." });
                }


                if (obj.GuestType == "Once")
                {

                    if (!obj.IsPrivateEntry == true || !obj.IsPrivateEntry == false)
                        return NotFound(new { message = "Is Private Entry?" });


                    if (!obj.StartDate.HasValue)
                        return NotFound(new { message = "Start date required." });


                    if (!obj.StartingFrom.HasValue)
                        return NotFound(new { message = "Starting date required." });

                    string[] ValidFor = ["1 Hour", "2 Hours", "4 Hours", "8 Hours", "12 Hours", "24 Hours"];

                    bool isMatchValidFor = ValidFor.Any(g => g.StartsWith(obj.ValidFor, StringComparison.OrdinalIgnoreCase));

                    if (!isMatchValidFor)
                        return NotFound(new { message = $"Valid for: {string.Join(", ", ValidFor)}" });


                    updateResidentFrequentlyGuests.IsPrivateEntry = obj.IsPrivateEntry;
                    updateResidentFrequentlyGuests.SelectDate = obj.SelectDate;
                    updateResidentFrequentlyGuests.StartingFrom = obj.StartingFrom;
                    updateResidentFrequentlyGuests.ValidFor = obj.ValidFor;

                }
                else if (obj.GuestType == "Frequently")
                {
                    string[] AllowEntryForNext = ["1 week", "1 month", "> 1 month"];

                    bool isMatchAllowEntryForNext = AllowEntryForNext.Any(g => g.StartsWith(obj.AllowEntryForNext, StringComparison.OrdinalIgnoreCase));

                    if (!isMatchAllowEntryForNext)
                        return NotFound(new { message = $"Match allowed entry for next: {string.Join(", ", AllowEntryForNext)}" });

                    if (!obj.StartDate.HasValue)
                        return NotFound(new { message = "Start date required." });

                    if (!obj.EndDate.HasValue)
                        return NotFound(new { message = "End date required." });

                    updateResidentFrequentlyGuests.AllowEntryForNext = obj.AllowEntryForNext;
                    updateResidentFrequentlyGuests.StartDate = obj.StartDate;
                    updateResidentFrequentlyGuests.EndDate = obj.EndDate;

                }


                updateResidentFrequentlyGuests.ResidentFrequentlyGuestsId = getFrequentlyGuestById.Id;
                updateResidentFrequentlyGuests.Type = obj.GuestType;
                updateResidentFrequentlyGuests.GuestName = obj.GuestName;
                updateResidentFrequentlyGuests.GuestNumber = obj.GuestNumber;
                updateResidentFrequentlyGuests.Note = obj.Note;
                updateResidentFrequentlyGuests.UniqueEntryNumber = getFrequentlyGuestById.UniqueEntryNumber;
                updateResidentFrequentlyGuests.IsActive = true;
                updateResidentFrequentlyGuests.UpdatedBy = currentUserId;
                updateResidentFrequentlyGuests.UpdatedOn = DateTime.UtcNow;



                if (obj.IsDeleted != null && obj.IsDeleted == true)
                {
                    updateResidentFrequentlyGuests.IsDeleted = true;
                    updateResidentFrequentlyGuests.DeletedBy = currentUserId;
                    updateResidentFrequentlyGuests.DeletedOn = DateTime.UtcNow;
                    updateResidentFrequentlyGuests.IsActive = false;
                }


                var returnResult = await UpdateResidentFrequentGuestsAsync(updateResidentFrequentlyGuests);


                if (returnResult != null)
                {

                    return Ok(returnResult);
                }
                else
                {
                    return NotFound(new { Message = "Unable to update Frequent Guest" });
                }

            }
            catch (System.Text.Json.JsonException)
            {
                return BadRequest(new { Message = "Unable to update Vehicle" });
            }
        }

        #endregion

        #region Frequent Entries APIs

        [Authorize(Policy = Utilities.Module.Resident)]
        [Authorize(Policy = Utilities.Permission.Get)]
        [HttpGet("get-resident-household-frequent-entries-form")]
        public async Task<IActionResult> GetResidentHouseholdFrequentEntriesForm()
        {
            try
            {
                var currentUserId = User.FindFirst(ClaimTypes.Name)?.Value;

                if (string.IsNullOrEmpty(currentUserId))
                {
                    return NotFound(new
                    {
                        statusCode = StatusCodes.Status404NotFound,
                        message = "Invalid or Expired Token."
                    });
                }

                // Step 1: Get the main FrequentEntries ID from static field
                int entityTypeDetailValue = 0;
                var field = typeof(Utilities.EntityTypeDetail).GetField("FrequentEntries", BindingFlags.Public | BindingFlags.Static);
                if (field?.GetRawConstantValue() is string value && int.TryParse(value, out int parsedValue))
                    entityTypeDetailValue = parsedValue;

                var result = await _context.EntityTypeDetail
                    .Where(d => d.IsDeleted != true && d.IsActive && d.Id == entityTypeDetailValue)
                    .FirstOrDefaultAsync();

                if (result == null)
                    return NotFound(new { message = "EntityTypeDetail not found." });

                // Step 2: Deserialize parent fields and sort in required order
                var parentFields = JsonConvert.DeserializeObject<List<ParentFieldModel>>(result.DetailJson);

                var orderedFieldIds = new List<string> { "100", "101", "97", "98", "99" };
                parentFields = parentFields
                    .Where(p => orderedFieldIds.Contains(p.fieldId))
                    .OrderBy(p => orderedFieldIds.IndexOf(p.fieldId))
                    .ToList();

                // Step 3: Load child blocks for each container field
                var childFieldIds = new List<int> { 97, 98, 99 };
                var childEntities = await _context.EntityTypeDetail
                    .Where(d => d.IsDeleted!=true && d.IsActive && childFieldIds.Contains(d.Id))
                    .ToListAsync();

                var childMap = new Dictionary<string, FieldSet>();
                foreach (var child in childEntities)
                {
                    var fieldSet = JsonConvert.DeserializeObject<FieldSet>(child.DetailJson);
                    childMap[child.Id.ToString()] = fieldSet;
                }

                // Step 4: Attach matching child form to each container
                foreach (var parent in parentFields)
                {
                    if (parent.fieldType == "container" && childMap.TryGetValue(parent.fieldId, out var children))
                    {
                        parent.childForm = children;
                    }
                }

                // Step 5: Return response in required format
                var response = new EntityTypeDetailResponse
                {
                    FrequentEntriesForm = parentFields
                };

                return Ok(response);
            }
            catch (Exception ex)
            {
                return NotFound(new { message = "No Household found, contact administration." });
            }
        }

        [Authorize(Policy = Utilities.Module.Resident)]
        [Authorize(Policy = Utilities.Permission.Get)]
        [HttpGet("get-all-resident-household-frequent-entries-companies-by-entrytype")]
        public async Task<IActionResult> GetAllByEntryType(string EntryType)
        {
            try
            {
                var currentUserId = User.FindFirst(ClaimTypes.Name)?.Value;

                if (string.IsNullOrEmpty(currentUserId))
                {
                    return NotFound(new
                    {
                        statusCode = StatusCodes.Status404NotFound,
                        message = "Invalid or Expired Token."
                    });
                }
           
            var result = new List<object>();

            if (!new[] { "cab", "delivery", "visiting help" }.Contains(EntryType.Trim().ToLower()))
            {
                return BadRequest(new {StatusCode=StatusCodes.Status400BadRequest, message = "Invalid entry type. Allowed: Cab, Delivery, or Visiting Help." });
            }

            EntryType = EntryType.Trim().ToLower();

            if (EntryType == "cab")
            {
                result = _context.CabCompany
                    .Where(c => c.IsActive && (c.IsDeleted == null || c.IsDeleted == false))
                    .OrderBy(c => c.Name)
                    .Select(c => new
                    {
                        c.Id,
                        c.Name,
                        c.Logo
                    })
                    .Cast<object>() // 👈 Needed to match List<object>
                    .ToList();
            }
            else if (EntryType == "delivery") 
            {
                result = _context.DeliveryCompany
                                         .Where(c => c.IsActive && (c.IsDeleted == null || c.IsDeleted == false))
                                         .OrderBy(c => c.Name)
                                         .Select(c => new
                                         {
                                             c.Id,
                                             c.Name,
                                             c.Logo
                                         })
                                         .Cast<object>()
                                         .ToList();

            }
            else if (EntryType == "visiting help")
            {
                result = _context.VisitingHelpCategory
                                       .Where(c => c.IsActive && (c.IsDeleted == null || c.IsDeleted == false))
                                       .OrderBy(c => c.Name)
                                       .Select(c => new
                                       {
                                           c.Id,
                                           c.Name,
                                           Logo = (string?)null
                                       })
                                        .Cast<object>()
                                       .ToList();

            }
            else
            {
                return NotFound(new { StatusCode=StatusCodes.Status404NotFound, message="No data found."});
            }

                return Ok(result);
            }
            catch (Exception)
            {

                return NotFound(new { StatusCode = StatusCodes.Status404NotFound, message = "No data found." });
            }
        }


        [Authorize(Policy = Utilities.Module.Resident)]
        [Authorize(Policy = Utilities.Permission.Add)]
        [HttpPost("add-resident-frequent-entry")]
        public async Task<IActionResult> AddResidentFrequentEntry([FromForm] AddResidentFrequentEntriesSelectedRecord request)
        {
            try
            {
                var currentUserId = User.FindFirst(ClaimTypes.Name)?.Value;

                if (string.IsNullOrEmpty(currentUserId))
                {
                    return Unauthorized(new { message = "Invalid or expired token." });
                }

                var getResidentIdByUserId = await _context.Resident
                   .Where(d => d.IsDeleted != true && d.IsActive == true && d.UserId == Guid.Parse(currentUserId))
                   .Select(d => d.Id)
                   .FirstOrDefaultAsync();

                if (getResidentIdByUserId == null)
                {
                    return NotFound(new { message = "Resident not found." });
                }

                // Validate EntryType
                var allowedEntryTypes = new[] { "cab", "delivery", "visiting help" };
                if (!allowedEntryTypes.Contains(request.EntryType.Trim().ToLower()))
                {
                    return BadRequest( new { StatusCode = StatusCodes.Status404NotFound, message = $"Invalid Entry Type. Allowed: {string.Join(", ", allowedEntryTypes)}" });
                }

                // Validate Frequently Type
                var allowedFreqTypes = new[] { "once", "frequently" };

                if (!allowedFreqTypes.Contains(request.FrequentlyType.Trim().ToLower()))
                {
                    return BadRequest(new { StatusCode = StatusCodes.Status404NotFound, message = $"Invalid Frequently Type. Allowed: {string.Join(", ", allowedFreqTypes)}" });
                }

                var entry = new AddResidentFrequentEntries
                {
                    Id = Guid.NewGuid(),
                    ResidentId = getResidentIdByUserId,
                    EntryType = request.EntryType,
                    FrequentlyType = request.FrequentlyType,
                    VehicleNo = request.VehicleNo,
                    CabCompanyId = request.EntryType.ToLower() == "cab" ? request.CompanyId : null,
                    DeliveryCompanyId = request.EntryType.ToLower() == "delivery" ? request.CompanyId : null,
                    VisitingHelpCategoryId = request.EntryType.ToLower() == "visiting help" ? request.CompanyId : null,
                    VisitingHelpName = request.EntryType.ToLower() == "visiting help" ? request.VisitingHelpName?.ToString() : null,
                    IsActive = true,
                    CreatedBy = currentUserId,
                    CreatedOn = DateTime.UtcNow,
                    IsDeleted = false,
                    UniqueEntryCode = new OTPGenerator().GenerateOTP()
                };

                if (request.FrequentlyType.ToLower() == "once")
                {
                    if (string.IsNullOrWhiteSpace(request.SelectDate) || string.IsNullOrWhiteSpace(request.StartingFrom))
                        return BadRequest(new { message = "SelectDate and StartingFrom are required for 'Once' entries." });

                    if (!CheckValidDate(request.SelectDate))
                        return NotFound(new { message = "Select Date must be in 'dd-MM-yyyy' format." });

                    if (!CheckValidTime(request.StartingFrom))
                        return NotFound(new { message = "Starting Time must be in 'HH:mm:ss' format." });

                    entry.EntryDate = DateOnly.FromDateTime(DateTime.ParseExact(request.SelectDate, "dd-MM-yyyy", CultureInfo.InvariantCulture));
                    entry.EntryTimeStart = TimeOnly.FromDateTime(DateTime.ParseExact(request.StartingFrom, "HH:mm:ss", CultureInfo.InvariantCulture));
                    entry.Validity = request.ValidFor;
                    entry.IsSurpriseDelivery = request.EntryType.ToLower() == "delivery" ? request.IsPrivateEntry : null;
                }
                else if (request.FrequentlyType.ToLower() == "frequently")
                {
                    if (string.IsNullOrWhiteSpace(request.DaysOfWeek) || string.IsNullOrWhiteSpace(request.Validity)
                        || string.IsNullOrWhiteSpace(request.StartTime) || string.IsNullOrWhiteSpace(request.EndTime)
                        || string.IsNullOrWhiteSpace(request.EntriesPerDay))
                    {
                        return BadRequest(new { message = "All fields required for 'Frequently' type are missing." });
                    }

                    entry.DaysOfWeek = request.DaysOfWeek;
                    entry.Validity = request.Validity;
                    entry.EntriesPerDay = request.EntriesPerDay;

                    if (!CheckValidTime(request.StartTime))
                        return NotFound(new { message = "Start Time must be in 'HH:mm:ss' format." });

                    if (!CheckValidTime(request.EndTime))
                        return NotFound(new { message = "End Time must be in 'HH:mm:ss' format." });


                    entry.EntryTimeStart = TimeOnly.FromDateTime(DateTime.ParseExact(request.StartTime, "HH:mm:ss", CultureInfo.InvariantCulture));
                    entry.EntryTimeEnd = TimeOnly.FromDateTime(DateTime.ParseExact(request.EndTime, "HH:mm:ss", CultureInfo.InvariantCulture));
                }

                var saveEntity = _mapper.Map<ResidentFrequentlyEntry>(entry);

                _context.ResidentFrequentlyEntry.Add(saveEntity);
                await _context.SaveChangesAsync();

                var resultDto = _mapper.Map<AddResidentFrequentEntriesDto>(saveEntity);
                return Ok(resultDto);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Server error", error = ex.Message });
            }
        }


        [Authorize(Policy = Utilities.Module.Resident)]
        [Authorize(Policy = Utilities.Permission.Get)]
        [HttpGet("get-all-resident-household-frequent-entry")]
        public async Task<ActionResult<IEnumerable<ResidentFrequentEntryDto>>> GetAllResidentHouseholdFrequentEntry()
        {
            try
            {

                var currentUserId = User.FindFirst(ClaimTypes.Name)?.Value;

                if (string.IsNullOrEmpty(currentUserId))
                {
                    return NotFound(new
                    {
                        statusCode = StatusCodes.Status404NotFound,
                        message = "Invalid or Expired Token."
                    });
                }

                var getResidentByUserId = await _context.Resident
                    .Where(s => s.IsDeleted != true && s.IsActive == true && s.UserId == Guid.Parse(currentUserId))
                    .Select(s => s.Id)
                    .FirstOrDefaultAsync();

                if (getResidentByUserId == null)
                {
                    return NotFound(new { message = "Resident not found." });
                }
                var residentFrequentlyEntries = await (
                    from entry in _context.ResidentFrequentlyEntry
                    where entry.IsDeleted != true
                          && entry.IsActive == true
                          && entry.ResidentId == getResidentByUserId
                    orderby entry.CreatedOn descending
                    join cab in _context.CabCompany
                        on entry.CabCompanyId equals cab.Id into cabJoin
                    from cabCompany in cabJoin.DefaultIfEmpty()
                    join delivery in _context.DeliveryCompany
                        on entry.DeliveryCompanyId equals delivery.Id into deliveryJoin
                    from deliveryCompany in deliveryJoin.DefaultIfEmpty()
                    join visiting in _context.VisitingHelpCategory
                        on entry.VisitingHelpCategoryId equals visiting.Id into visitingJoin
                    from visitingCategory in visitingJoin.DefaultIfEmpty()
                    select new ResidentFrequentEntryDto
                    {
                        Id = entry.Id,
                        ResidentId = entry.ResidentId,
                        EntryType = entry.EntryType,
                        FrequentlyType = entry.FrequentlyType,
                        AllowEntryInNext = entry.AllowEntryInNext,
                        EntryDate = entry.EntryDate,
                        IsSurpriseDelivery = entry.IsSurpriseDelivery,
                        IsLeaveAtGate = entry.IsLeaveAtGate,
                        EntryTimeStart = entry.EntryTimeStart,
                        EntryTimeEnd = entry.EntryTimeEnd,
                        VehicleNo = entry.VehicleNo,
                        CabCompanyId = entry.CabCompanyId,
                        CabCompanyName = cabCompany != null ? cabCompany.Name : null,
                        DeliveryCompanyId = entry.DeliveryCompanyId,
                        DeliveryCompanyName = deliveryCompany != null ? deliveryCompany.Name : null,
                        VisitingHelpCategoryId = entry.VisitingHelpCategoryId,
                        VisitingHelpCategoryName = visitingCategory != null ? visitingCategory.Name : null,
                        DaysOfWeek = entry.DaysOfWeek,
                        Validity = entry.Validity,
                        EntriesPerDay = entry.EntriesPerDay,
                        UniqueEntryCode = entry.UniqueEntryCode,
                        IsActive = entry.IsActive,
                        CreatedBy = entry.CreatedBy,
                        CreatedOn = entry.CreatedOn,
                        IsDeleted = entry.IsDeleted,
                        VisitingHelpName = entry.VisitingHelpName
                    }).ToListAsync();

                if (!residentFrequentlyEntries.Any())
                {
                    return Ok(new { message = "No frequent entry added." });
                }

              //  return Ok(residentFrequentlyEntries);

                return Ok(_mapper.Map<List<ResidentFrequentEntryDto>>(residentFrequentlyEntries));

            }
            catch (Exception ex)
            {
                return NotFound(new { message = "No frequent entry added." });
            }
        }


        [Authorize(Policy = Utilities.Module.Resident)]
        [Authorize(Policy = Utilities.Permission.Update)]
        [HttpPost("update-resident-household-frequent-entry-by-id")]
        public async Task<IActionResult> UpdateResidentFrequentEntry([FromForm] UpdateResidentFrequentEntriesSelectedRecord request)
        {
            try
            {
                var currentUserId = User.FindFirst(ClaimTypes.Name)?.Value;

                if (string.IsNullOrEmpty(currentUserId))
                {
                    return Unauthorized(new { message = "Invalid or expired token." });
                }

                var getResidentIdByUserId = await _context.Resident
                    .Where(d => d.IsDeleted != true && d.IsActive == true && d.UserId == Guid.Parse(currentUserId))
                    .Select(d => d.Id)
                    .FirstOrDefaultAsync();

                if (getResidentIdByUserId == Guid.Empty)
                {
                    return NotFound(new { message = "Resident not found." });
                }

                // Find existing entry
                var entry = await _context.ResidentFrequentlyEntry
                    .FirstOrDefaultAsync(e => e.Id == request.ResidentFrequentEntryId && e.ResidentId == getResidentIdByUserId && e.IsDeleted != true);

                if (entry == null)
                {
                    return NotFound(new { message = "Frequent entry not found." });
                }

                // Validate EntryType
                var allowedEntryTypes = entry.EntryType;

                // Validate Frequently Type
                var allowedFreqTypes = entry.FrequentlyType;
                

                // Update common fields
                entry.VehicleNo = request.VehicleNo;
                entry.CabCompanyId = allowedEntryTypes.ToLower() == "cab" ? request.CompanyId : null;
                entry.DeliveryCompanyId = allowedEntryTypes.ToLower() == "delivery" ? request.CompanyId : null;
                entry.VisitingHelpCategoryId = allowedEntryTypes.ToLower() == "visiting help" ? request.CompanyId : null;
                entry.VisitingHelpName = allowedEntryTypes.ToLower() == "visiting help" ? request.VisitingHelpName?.ToString() : null;
                entry.UpdatedBy = currentUserId;
                entry.UpdatedOn = DateTime.UtcNow;

                if (request.IsDeleted != null && request.IsDeleted == true)
                {
                    entry.IsActive = false;
                    entry.IsDeleted = true;
                    entry.DeletedBy = currentUserId;
                    entry.DeletedOn = DateTime.UtcNow;
                }

                // Handle "once"
                if (allowedFreqTypes.ToLower() == "once")
                {
                    if (string.IsNullOrWhiteSpace(request.SelectDate) || string.IsNullOrWhiteSpace(request.StartingFrom))
                        return BadRequest(new { message = "SelectDate and StartingFrom are required for 'Once' entries." });

                    if (!CheckValidDate(request.SelectDate))
                        return NotFound(new { message = "Select Date must be in 'dd-MM-yyyy' format." });

                    if (!CheckValidTime(request.StartingFrom))
                        return NotFound(new { message = "Starting Time must be in 'HH:mm:ss' format." });

                    entry.EntryDate = DateOnly.FromDateTime(DateTime.ParseExact(request.SelectDate, "dd-MM-yyyy", CultureInfo.InvariantCulture));
                    entry.EntryTimeStart = TimeOnly.FromDateTime(DateTime.ParseExact(request.StartingFrom, "HH:mm:ss", CultureInfo.InvariantCulture));
                    entry.Validity = request.ValidFor;
                    entry.IsSurpriseDelivery = allowedEntryTypes.ToLower() == "delivery" ? request.IsPrivateEntry : null;

                    // Clear fields not relevant to 'once'
                 //   entry.EntryTimeEnd = null;
                    entry.DaysOfWeek = null;
                    entry.EntriesPerDay = null;
                }

                // Handle "frequently"
                else if (allowedFreqTypes.ToLower() == "frequently")
                {
                    if (string.IsNullOrWhiteSpace(request.DaysOfWeek) || string.IsNullOrWhiteSpace(request.Validity)
                        || string.IsNullOrWhiteSpace(request.StartTime) || string.IsNullOrWhiteSpace(request.EndTime)
                        || string.IsNullOrWhiteSpace(request.EntriesPerDay))
                    {
                        return BadRequest(new { message = "All fields required for 'Frequently' type are missing." });
                    }

                    if (!CheckValidTime(request.StartTime))
                        return NotFound(new { message = "Start Time must be in 'HH:mm:ss' format." });

                    if (!CheckValidTime(request.EndTime))
                        return NotFound(new { message = "End Time must be in 'HH:mm:ss' format." });

                    entry.DaysOfWeek = request.DaysOfWeek;
                    entry.Validity = request.Validity;
                    entry.EntriesPerDay = request.EntriesPerDay;
                    entry.EntryTimeStart = TimeOnly.FromDateTime(DateTime.ParseExact(request.StartTime, "HH:mm:ss", CultureInfo.InvariantCulture));
                    entry.EntryTimeEnd = TimeOnly.FromDateTime(DateTime.ParseExact(request.EndTime, "HH:mm:ss", CultureInfo.InvariantCulture));

                    // Clear fields not relevant to 'frequently'
                    entry.EntryDate = null;
                    entry.IsSurpriseDelivery = null;
                }

                _context.ResidentFrequentlyEntry.Update(entry);
                await _context.SaveChangesAsync();

                var resultDto = _mapper.Map<UpdateResidentFrequentEntryDto>(entry);

                return Ok(resultDto);
            }
            catch (Exception ex)
            {
                return BadRequest( new {StatusCode= StatusCodes.Status400BadRequest, message = "Unable to update frequent entry."});
            }
        }


        #endregion

        #region Daily Help APIs

        [Authorize(Policy = Utilities.Module.Resident)]
        [Authorize(Policy = Utilities.Permission.Get)]
        [HttpGet("get-all-daily-help-categories")]
        public async Task<ActionResult<IEnumerable<DailyHelpCategoriesDto>>> GetAllDailyHelpCategories()
        {
            try
            {

            var currentUserId = User.FindFirst(ClaimTypes.Name)?.Value;

            if (string.IsNullOrEmpty(currentUserId))
            {
                return NotFound(new
                {
                    statusCode = StatusCodes.Status404NotFound,
                    message = "Invalid or Expired Token."
                });
            }

            var dailyHelpCategories = await _context.EntityTypeDetail
         .Where(s => s.IsDeleted != true && s.IsActive == true && s.EntityTypeId==10)
         .OrderBy(s => s.Id)
           .Select(s => new DailyHelpCategoriesDto
            {
                Id = Convert.ToInt32(s.Value),
                Name = s.Label 
            })
         .ToListAsync();

            if (dailyHelpCategories !=null)
            {
            return Ok(_mapper.Map<List<DailyHelpCategoriesDto>>(dailyHelpCategories));
            }
            else
            {
                return NotFound(new { StatusCode=StatusCodes.Status404NotFound, message = "No daily help found."});
            }

            }
            catch (Exception ex)
            {
                return NotFound(new { StatusCode = StatusCodes.Status404NotFound, message = "No daily help found." });
            }

        }

        [Authorize(Policy = Utilities.Module.Resident)]
        [Authorize(Policy = Utilities.Permission.Get)]
        [HttpGet("get-all-daily-helps-by-categoryid")]
        public async Task<ActionResult<IEnumerable<SocietyUserProfileDto>>> GetAllDailyHelpsByCategoryid(Guid societyId, int categoryId)
        {
            try
            {
                var currentUserId = User.FindFirst(ClaimTypes.Name)?.Value;

                if (string.IsNullOrEmpty(currentUserId))
                {
                    return NotFound(new
                    {
                        statusCode = StatusCodes.Status404NotFound,
                        message = "Invalid or Expired Token."
                    });
                }

                var connectionString = _configuration.GetConnectionString("DefaultConnection");

                using var connection = new SqlConnection(connectionString);

                var parameters = new DynamicParameters();
                parameters.Add("@SocietyId", societyId, DbType.Guid);
                parameters.Add("@CategoryId", categoryId, DbType.Int32);

                var result = await connection.QueryAsync<SocietyUserProfileDto>(
                    "[dbo].[GetSocietyUsersProfile]",
                    parameters,
                    commandType: CommandType.StoredProcedure
                );

                var dailyHelpList = result.Select(user =>
                {
                    List<TimeSlot> timeSlots = new();

                        timeSlots = System.Text.Json.JsonSerializer.Deserialize<List<TimeSlot>>(user.AllowedTimeSlotJson ?? "[]")
                                     ?? new List<TimeSlot>();
                   

                    return new SocietyUserProfileDto
                    {
                        UserProfileId = user.UserProfileId,
                        SocietyId = user.SocietyId,
                        Name = user.Name,
                        PhoneNumber = user.PhoneNumber,
                        RolesId = user.RolesId,
                        ProfilePic = user.ProfilePic,
                        IsInside = user.IsInside,
                        HousesWorking = user.HousesWorking,
                        RatingOutOf5 = user.RatingOutOf5,
                        AllowedTimeSlots = timeSlots
                    };
                }).ToList();


                return Ok(dailyHelpList);
            }
            catch (Exception ex)
            {
                return NotFound(new
                {
                    StatusCode = StatusCodes.Status404NotFound,
                    message = "No daily help found."
                });
            }
        }

        [Authorize(Policy = Utilities.Module.Resident)]
        [Authorize(Policy = Utilities.Permission.Get)]
        [HttpGet("get-daily-helps-user-detail")]
        public async Task<ActionResult<IEnumerable<SocietyUserProfileDetailDto>>> GetDailyHelpsUserDetail(Guid UserProfileId)
        {
            try
            {
                var currentUserId = User.FindFirst(ClaimTypes.Name)?.Value;

                if (string.IsNullOrEmpty(currentUserId))
                {
                    return NotFound(new
                    {
                        statusCode = StatusCodes.Status404NotFound,
                        message = "Invalid or Expired Token."
                    });
                }

                var connectionString = _configuration.GetConnectionString("DefaultConnection");

                using var connection = new SqlConnection(connectionString);

                var parameters = new DynamicParameters();
                parameters.Add("@UserProfileId", UserProfileId, DbType.Guid);

                var result = await connection.QueryAsync<SocietyUserProfileDetailDto>(
                    "[dbo].[GetSocietyUserProfileDetail]",
                    parameters,
                    commandType: CommandType.StoredProcedure
                );

                var dailyHelpList = result.Select(user =>
                {
                    List<TimeSlot> timeSlots = new();


                    timeSlots = System.Text.Json.JsonSerializer.Deserialize<List<TimeSlot>>(user.AllowedTimeSlotJson ?? "[]")
                                 ?? new List<TimeSlot>();

                    List<WorkingHistory> workingHistories = new();

                    workingHistories = System.Text.Json.JsonSerializer.Deserialize<List<WorkingHistory>>(user.WorkingHistoryJson ?? "[]")
                                 ?? new List<WorkingHistory>();


                    return new SocietyUserProfileDetailDto
                    {
                        UserProfileId = user.UserProfileId,
                        Name = user.Name,
                        PhoneNumber = user.PhoneNumber,
                        RolesId = user.RolesId,
                        ProfilePic = user.ProfilePic,
                        IsInside = user.IsInside,
                        HousesWorking = user.HousesWorking,
                        RatingOutOf5 = user.RatingOutOf5,
                        AllowedTimeSlots = timeSlots,
                        WorkingHistory = workingHistories
                    };
                }).ToList();


                return Ok(dailyHelpList);
            }
            catch (Exception ex)
            {
                return NotFound(new
                {
                    StatusCode = StatusCodes.Status404NotFound,
                    message = "No detail found."
                });
            }
        }


        [Authorize(Policy = Utilities.Module.Resident)]
        [Authorize(Policy = Utilities.Permission.Add)]
        [HttpPost("add-resident-daily-help")]
        public async Task<IActionResult> AddResidentDailyHelp([FromForm] AddResidentDailyHelpSelectedRecord request)
        {
            try
            {
                var currentUserId = User.FindFirst(ClaimTypes.Name)?.Value;

                if (string.IsNullOrEmpty(currentUserId))
                {
                    return Unauthorized(new { message = "Invalid or expired token." });
                }

                var getResidentIdByUserId = await _context.Resident
                   .Where(d => d.IsDeleted != true && d.IsActive == true && d.UserId == Guid.Parse(currentUserId))
                   .Select(d => new { d.Id, d.SocietyFlatId })
                   .FirstOrDefaultAsync();

                if (getResidentIdByUserId == null)
                {
                    return NotFound(new { message = "Resident not found." });
                }


                // Step 1: Get the Id of the "SocietyUser" role
                var societyUserRoleId = await _context.Role
                    .Where(r => r.RoleName.Contains("SocietyUser"))
                    .Select(r => r.Id)
                    .FirstOrDefaultAsync();

                if (societyUserRoleId == null)
                {
                    return NotFound(new { message = "Resident not found." });
                }


                // Step 2: Verify the user profile
                var verifyUserProfileId = await (
                    from d in _context.SocietyUserProfile
                    join e in _context.SocietyUser on d.SocietyUserId equals e.Id
                    join f in _context.User on e.SocietyUserId equals f.UserName
                    join g in _context.Role on f.RolesId equals g.Id
                    where (d.IsDeleted == false || d.IsDeleted == null)
                          && d.IsActive == true
                          && (e.IsDeleted == false || e.IsDeleted == null)
                          && e.IsActive == true
                          && (f.IsDeleted == false || f.IsDeleted == null)
                          && f.IsActive == true
                          && f.Role == "admin"
                          && g.ParentRoleId == societyUserRoleId
                          && d.Id == request.UserProfileId
                    select new
                    {
                        d.Id
                    }
                ).FirstOrDefaultAsync();

                if (verifyUserProfileId == null)
                {
                    return NotFound(new { message = "User profile not found." });
                }




                var addSocietyUserFlatWorkingHistory = new AddSocietyUserFlatWorkingHistory
                {
                    Id = Guid.NewGuid(),
                    SocietyUserProfileId = request.UserProfileId,
                    SocietyFlatId = getResidentIdByUserId.SocietyFlatId ?? Guid.Empty,
                    IsAddedByResident = true,
                    IsNotify = false,
                    Ratings = 0,
                    CreatedBy = currentUserId,
                    CreatedOn = DateTime.UtcNow
                };

             

                var savedSocietyUserFlatWorkingHistory = _mapper.Map<SocietyUserFlatWorkingHistory>(addSocietyUserFlatWorkingHistory);

                _context.SocietyUserFlatWorkingHistory.Add(savedSocietyUserFlatWorkingHistory);
                await _context.SaveChangesAsync();

                //var addRe = new AddSocietyUserFlatWorkingHistory
                //{
                //    Id = Guid.NewGuid(),
                //    SocietyUserProfileId = request.UserProfileId,
                //    SocietyFlatId = getResidentIdByUserId.SocietyFlatId ?? Guid.Empty,
                //    IsAddedByResident = true,
                //    IsNotify = false,
                //    Ratings = 0,
                //    CreatedBy = currentUserId,
                //    CreatedOn = DateTime.UtcNow
                //};



                //var savedSocietyUserFlatWorkingHistory = _mapper.Map<SocietyUserFlatWorkingHistory>(addSocietyUserFlatWorkingHistory);

                //_context.SocietyUserFlatWorkingHistory.Add(savedSocietyUserFlatWorkingHistory);
                //await _context.SaveChangesAsync();

                var resultDto = _mapper.Map<AddResidentFrequentEntriesDto>(savedSocietyUserFlatWorkingHistory);
                return Ok(resultDto);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Server error", error = ex.Message });
            }
        }

        #endregion

        #region CommonMethod
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
                 string fileUrl = $"https://apisociety.call2owner.com/Images/Resident/Documents/{blobName}";

                //Local Path
              //  string fileUrl = $"C://Users//WLTPE//source//repos//c2o//Call2Owner.API//Images//Resident//Documents/{blobName}";


                return fileUrl; // Return the accessible file URL
            }
            catch (Exception ex)
            {
                // Log and rethrow the exception
                throw new InvalidOperationException("An error occurred while uploading the file/image.", ex);
            }
        }

        #endregion

        #region FamilyMethod
        private async Task<string> ResidentFamilyUploadProfilePictureAsync(string blobName, Stream imageStream)
        {
            try
            {
                // Ensure blobName is sanitized to prevent path traversal attacks
                if (string.IsNullOrWhiteSpace(blobName))
                {
                    throw new ArgumentException("Invalid blob name", nameof(blobName));
                }

                // Define the IIS folder path
                string folderPath = Path.Combine(Directory.GetCurrentDirectory(), "Images", "Resident", "Family");

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
                  string fileUrl = $"https://apisociety.call2owner.com/Images/Resident/Family/{blobName}";

                //Local Path
                //string fileUrl = $"C://Users//WLTPE//source//repos//c2o//Call2Owner.API//Images//Resident//Family/{blobName}";


                return fileUrl; // Return the accessible file URL
            }
            catch (Exception ex)
            {
                // Log and rethrow the exception
                throw new InvalidOperationException("An error occurred while uploading the image.", ex);
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
                userExists.SocietyFlatId = updateResident.SocietyFlatId;
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

        private async Task<AddResidentFamilyDto?> AddResidentFamilyAsync(AddResidentFamily addResidentFamily)
        {
            // Convert input DTO to EF Core entity
            var residentFamilyEntity = _mapper.Map<ResidentFamily>(addResidentFamily);

            // Add to DbContext
            _context.ResidentFamily.Add(residentFamilyEntity);

            // Save to DB
            await _context.SaveChangesAsync();

            // Convert the saved entity to output DTO
            var residentFamilyResponseDTO = _mapper.Map<AddResidentFamilyDto>(residentFamilyEntity);

            return residentFamilyResponseDTO;
        }

        private async Task<UpdateResidentFamilyDto?> UpdateResidentFamilyAsync(UpdateResidentFamily updateResidentFamily)
        {
            // Step 1: Get existing record
            var existingEntity = await _context.ResidentFamily
                .FirstOrDefaultAsync(r => r.Id == updateResidentFamily.ResidentFamilyId);

            if (existingEntity == null)
            {
                return null; // Not found
            }

            // Step 2: Update fields using AutoMapper (or manually)
            _mapper.Map(updateResidentFamily, existingEntity);

            // Step 3: Save changes
            await _context.SaveChangesAsync();

            // Step 4: Map to DTO and return
            var responseDto = _mapper.Map<UpdateResidentFamilyDto>(existingEntity);
            return responseDto;
        }

        #endregion

        #region PetMethod

        private async Task<string> ResidentPetUploadProfilePictureAsync(string blobName, Stream imageStream)
        {
            try
            {
                // Ensure blobName is sanitized to prevent path traversal attacks
                if (string.IsNullOrWhiteSpace(blobName))
                {
                    throw new ArgumentException("Invalid blob name", nameof(blobName));
                }

                // Define the IIS folder path
                string folderPath = Path.Combine(Directory.GetCurrentDirectory(), "Images", "Resident", "Pet");

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
                  string fileUrl = $"https://apisociety.call2owner.com/Images/Resident/Pet/{blobName}";

                //Local Path
               // string fileUrl = $"C://Users//WLTPE//source//repos//c2o//Call2Owner.API//Images//Resident//Pet/{blobName}";


                return fileUrl; // Return the accessible file URL
            }
            catch (Exception ex)
            {
                // Log and rethrow the exception
               return null;
            }
        }

        private async Task<AddResidentPetDto?> AddResidentPetAsync(AddResidentPet addResidentPet)
        {
            // Convert input DTO to EF Core entity
            var residentPetEntity = _mapper.Map<ResidentPet>(addResidentPet);

            // Add to DbContext
            _context.ResidentPet.Add(residentPetEntity);

            // Save to DB
            await _context.SaveChangesAsync();

            // Convert the saved entity to output DTO
            var residentPetResponseDTO = _mapper.Map<AddResidentPetDto>(residentPetEntity);

            return residentPetResponseDTO;
        }

        private async Task<UpdateResidentPetDto?> UpdateResidentPetAsync(UpdateResidentPet updateResidentPet)
        {
            // Step 1: Get existing record
            var existingEntity = await _context.ResidentPet
                .FirstOrDefaultAsync(r => r.Id == updateResidentPet.ResidentPetId);

            if (existingEntity == null)
            {
                return null; // Not found
            }

            // Step 2: Update fields using AutoMapper (or manually)
            _mapper.Map(updateResidentPet, existingEntity);

            // Step 3: Save changes
            await _context.SaveChangesAsync();

            // Step 4: Map to DTO and return
            var responseDto = _mapper.Map<UpdateResidentPetDto>(existingEntity);
            return responseDto;
        }

        #endregion

        #region VehicleMethod

        private async Task<string> ResidentVehicleUploadProfilePictureAsync(string blobName, Stream imageStream)
        {
            try
            {
                // Ensure blobName is sanitized to prevent path traversal attacks
                if (string.IsNullOrWhiteSpace(blobName))
                {
                    throw new ArgumentException("Invalid blob name", nameof(blobName));
                }

                // Define the IIS folder path
                string folderPath = Path.Combine(Directory.GetCurrentDirectory(), "Images", "Resident", "Vehicle");

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
                  string fileUrl = $"https://apisociety.call2owner.com/Images/Resident/Vehicle/{blobName}";

                //Local Path
               // string fileUrl = $"C://Users//WLTPE//source//repos//c2o//Call2Owner.API//Images//Resident//Vehicle/{blobName}";


                return fileUrl; // Return the accessible file URL
            }
            catch (Exception ex)
            {
                // Log and rethrow the exception
                return null;
            }
        }

        private async Task<AddResidentVehicleDto?> AddResidentVehicleAsync(AddResidentVehicle addResidentVehicle)
        {
            // Convert input DTO to EF Core entity
            var residentVehicleEntity = _mapper.Map<ResidentVehicle>(addResidentVehicle);

            // Add to DbContext
            _context.ResidentVehicle.Add(residentVehicleEntity);

            // Save to DB
            await _context.SaveChangesAsync();

            // Convert the saved entity to output DTO
            var residentVehicleResponseDTO = _mapper.Map<AddResidentVehicleDto>(residentVehicleEntity);

            return residentVehicleResponseDTO;
        }

        private async Task<UpdateResidentVehicleDto?> UpdateResidentVehicleAsync(UpdateResidentVehicle updateResidentVehicle)
        {
            // Step 1: Get existing record
            var existingEntity = await _context.ResidentVehicle
                .FirstOrDefaultAsync(r => r.Id == updateResidentVehicle.ResidentVehicleId);

            if (existingEntity == null)
            {
                return null; // Not found
            }

            // Step 2: Update fields using AutoMapper (or manually)
            _mapper.Map(updateResidentVehicle, existingEntity);

            // Step 3: Save changes
            await _context.SaveChangesAsync();

            // Step 4: Map to DTO and return
            var responseDto = _mapper.Map<UpdateResidentVehicleDto>(existingEntity);
            return responseDto;
        }


        #endregion

        #region FrequentGuestsMethod

        private async Task<AddResidentFrequentGuestsDto?> AddResidentFrequentGuestsAsync(AddResidentFrequentGuests addResidentFrequentGuests)
        {
            // Convert input DTO to EF Core entity
            var residentFrequentGuests = _mapper.Map<ResidentFrequentlyGuest>(addResidentFrequentGuests);

            // Add to DbContext
            _context.ResidentFrequentlyGuest.Add(residentFrequentGuests);

            // Save to DB
            await _context.SaveChangesAsync();

            // Convert the saved entity to output DTO
            var residentFrequentGuestsResponseDTO = _mapper.Map<AddResidentFrequentGuestsDto>(residentFrequentGuests);

            return residentFrequentGuestsResponseDTO;
        }

        private async Task<UpdateResidentFrequentGuestsDto?> UpdateResidentFrequentGuestsAsync(UpdateResidentFrequentlyGuests updateResidentFrequentlyGuests)
        {
            // Step 1: Get existing record
            var existingEntity = await _context.ResidentFrequentlyGuest
                .FirstOrDefaultAsync(r => r.Id == updateResidentFrequentlyGuests.ResidentFrequentlyGuestsId);

            if (existingEntity == null)
            {
                return null; // Not found
            }

            // Step 2: Update fields using AutoMapper (or manually)
            _mapper.Map(updateResidentFrequentlyGuests, existingEntity);

            // Step 3: Save changes
            await _context.SaveChangesAsync();

            // Step 4: Map to DTO and return
            var responseDto = _mapper.Map<UpdateResidentFrequentGuestsDto>(existingEntity);
            return responseDto;
        }

        private bool CheckValidDate(string Date)
        {
            string input = Date;
            DateTime parsedDate;

            bool isValid = DateTime.TryParseExact(
                input,
                "dd-MM-yyyy",
                CultureInfo.InvariantCulture,
                DateTimeStyles.None,
                out parsedDate
            );

            return isValid;
        }

        private bool CheckValidTime(string Time)
        {
            string input = Time;
            DateTime parsedDate;

            DateTime parsedTime;

            bool isValid = DateTime.TryParseExact(
                Time,
                "HH:mm:ss",
                CultureInfo.InvariantCulture,
                DateTimeStyles.None,
                out parsedTime
            );

            return isValid;
        }
        #endregion

        #region FrequentEntriesMethod

        private async Task<AddResidentFrequentEntriesDto?> AddResidentFrequentEntriesAsync(AddResidentFrequentEntries addResidentFrequentEntries)
        {
            // Convert input DTO to EF Core entity
            var residentFrequentEntry = _mapper.Map<ResidentFrequentlyEntry>(addResidentFrequentEntries);

            // Add to DbContext
            _context.ResidentFrequentlyEntry.Add(residentFrequentEntry);

            // Save to DB
            await _context.SaveChangesAsync();

            // Convert the saved entity to output DTO
            var residentFrequentEntryResponseDto = _mapper.Map<AddResidentFrequentEntriesDto>(residentFrequentEntry);

            return residentFrequentEntryResponseDto;
        }


        #endregion

        #region DailyHelpMethod
        #endregion
    }

    #region CommonModel
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
        public Guid societyFlatId { get; set; }
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
        public Guid SocietyFlatId { get; set; }
        public string UpdatedBy {get; set;}
                public DateTime UpdatedOn {get; set;}
                public string DetailJson { get; set; }
    }

    #endregion

    #region FamilyModel
    public class FamilyRoot
    {
        public List<FamilyFieldDetail> Adult { get; set; }
        public List<FamilyFieldDetail> Kid { get; set; }
    }

    public class FamilyFieldDetail
    {
        public string fieldText { get; set; }
        public string fieldType { get; set; }
        public string fieldId { get; set; }
        public string fieldName { get; set; }
        public List<string> type { get; set; } // Only applicable for some fields
        public int? minLength { get; set; }    // Nullable because not all fields have this
        public int? maxlength { get; set; }    // Nullable because not all fields have this
        public string regex { get; set; }      // Nullable
        public bool required { get; set; }
        public bool isActive { get; set; }
        public bool isInputControl { get; set; }
        public string recordType { get; set; }
    }

    public class FamilyEntityTypeDetailModel
    {
        public int Id { get; set; }
        public int EntityTypeId { get; set; }
        public string Value { get; set; }
        public string Label { get; set; }
        public FamilyRoot FamilyType { get; set; }
        public bool IsDafault { get; set; }
        public bool IsActive { get; set; }
        public string? CreatedBy { get; set; }
        public DateTime? CreatedOn { get; set; }
        public string? UpdatedBy { get; set; }
        public DateTime? UpdatedOn { get; set; }
        public bool? IsDeleted { get; set; }
        public string? DeletedBy { get; set; }
        public DateTime? DeletedOn { get; set; }
    }

    public class AddFamilySelectedRecord
    {
        public string familyType { get; set; }
        public string name { get; set; }
        public string? mobileNumber { get; set; }
        public string? fileName { get; set; }
        public string? exittype { get; set; }
        public IFormFile? file { get; set; }
    }

    public class AddResidentFamily
    {
        public Guid Id { get; set; }

        public Guid ResidentId { get; set; }

        public string FamilyType { get; set; } = null!;

        public string? ProfilePicture { get; set; }

        public string Name { get; set; } = null!;

        public string? MobileNumber { get; set; }

        public string? ExitType { get; set; }
        public string ResidentFamilyCode { get; set; }

        public bool IsActive { get; set; }

        public string? CreatedBy { get; set; }

        public DateTime? CreatedOn { get; set; }

        public bool? IsDeleted { get; set; }

    }

    public class UpdateFamilySelectedRecord
    {
        public Guid ResidentFamilyId { get; set; }
        public string familyType { get; set; }
        public string name { get; set; }
        public string? mobileNumber { get; set; }
        public string? fileName { get; set; }
        public string? exittype { get; set; }
        public IFormFile? file { get; set; }
        public bool? IsDeleted { get; set; }
    }

    public class UpdateResidentFamily
    {
        public Guid ResidentFamilyId { get; set; }
        public string FamilyType { get; set; } = null!;

        public string? ProfilePicture { get; set; }

        public string Name { get; set; } = null!;

        public string? MobileNumber { get; set; }

        public string? ExitType { get; set; }

        public bool IsActive { get; set; }

        public string? UpdatedBy { get; set; }

        public DateTime? UpdatedOn { get; set; }

        public bool? IsDeleted { get; set; }
        public string? DeletedBy { get; set; }

        public DateTime? DeletedOn { get; set; }

    }

    #endregion

    #region PetModel

    public class EntityTypeInput
    {
        public string EntityTypeId { get; set; }
    }

    public class EntityTypeDetailDto
    {
        public int Id { get; set; }
        public int EntityTypeId { get; set; }
        public string Value { get; set; } = "";
        public string Label { get; set; } = "";
        public string? WebImage { get; set; }
        public string? MobileImage { get; set; }
        public PetDetails Detail { get; set; } = null!; // Or appropriate type
    }

    public class DailyHelpCategoriesDto
    {
        public int Id { get; set; }
        public string Name { get; set; }
    }

    public class TimeSlot
    {
        public int id { get; set; }
        public string start { get; set; }
        public string end { get; set; }
        public bool isActive { get; set; }
    }

    public class WorkingHistory
    {
        public string FlatName { get; set; }
        public string Working_From { get; set; }
    }

    public class SocietyUserProfileDto
    {
        public Guid UserProfileId { get; set; }
        public Guid SocietyId { get; set; }
        public string Name { get; set; }
        public string PhoneNumber { get; set; }
        public int RolesId { get; set; }
        public string ProfilePic { get; set; }
        public bool IsInside { get; set; }
        public string AllowedTimeSlotJson { get; set; }
        public int HousesWorking { get; set; }
        public double RatingOutOf5 { get; set; }

        // New property to hold deserialized JSON
        public List<TimeSlot> AllowedTimeSlots { get; set; } = new();
    }

    public class SocietyUserProfileDetailDto
    {
        public Guid UserProfileId { get; set; }
        public string Name { get; set; }
        public string PhoneNumber { get; set; }
        public int RolesId { get; set; }
        public string ProfilePic { get; set; }
        public bool IsInside { get; set; }
        public string AllowedTimeSlotJson { get; set; }
        public int HousesWorking { get; set; }
        public double RatingOutOf5 { get; set; }
        public string WorkingHistoryJson { get; set; }

        // New property to hold deserialized JSON
        public List<TimeSlot> AllowedTimeSlots { get; set; } = new();

        public List<WorkingHistory> WorkingHistory { get; set; } = new();
    }
    public class Breed
    {
        public string Name { get; set; } = null!;
        public bool IsActive { get; set; }
    }

    public class Question
    {
        public string QuestionText { get; set; } = null!;
        public string QuestionType { get; set; } = null!;
        public int stepNo { get; set; } = 0!;
        public int stepOutOf { get; set; } = 0!;
    }

    public class PetDetails
    {
        public string? WebImage { get; set; }
        public string? MobileImage { get; set; }
        public List<Breed> Breed { get; set; } = new();
        public List<Question> Questions { get; set; } = new();
    }

    public class PetDetailFull
    {
        public string? WebImage { get; set; }
        public string? MobileImage { get; set; }
        public List<Breed> Breed { get; set; } = new();
        public List<Question> Questions { get; set; } = new();
    }

    public class PetDetailFiltered
    {
        public List<Breed> Breed { get; set; } = new();
        public List<Question> Questions { get; set; } = new();
    }

    public class PetRequest
    {
        public int Id { get; set; }
        public string Label { get; set; }
        public string BreedName { get; set; }
        public string Questions { get; set; }
        public IFormFile petFile { get; set; }
    }

    public class QuestionDto
    {
        public string QuestionText { get; set; }
        public string QAnswerText { get; set; } // Can also use object if type varies
    }

    public class AddResidentPet
    {
        public Guid Id { get; set; }
        public Guid ResidentId { get; set; }
        public string PetType { get; set; } = null!;
        public string PetBreed { get; set; } = null!;
        public string PetName { get; set; } = null!;
        public string VaccinationType { get; set; } = null!; 
        public string PetPicture { get; set; } = null!;
        public bool IsActive { get; set; }
        public string? CreatedBy { get; set; }
        public DateTime? CreatedOn { get; set; }
        public bool? IsDeleted { get; set; }

    }
    public class UpdatePetSelectedRecord
    {
        public Guid ResidentPetId { get; set; }
        public string PetName { get; set; } = null!;
        public string vaccinationType { get; set; } = null!;
        public string fileName { get; set; } = null!;
        public IFormFile? file { get; set; }
        public bool? IsDeleted { get; set; }
    }

    public class UpdateResidentPet
    {
        public Guid ResidentPetId { get; set; }
        public string PetName { get; set; } = null!;
        public string VaccinationType { get; set; } = null!;
        public string PetPicture { get; set; } = null!;

        public bool IsActive { get; set; }

        public string? UpdatedBy { get; set; }

        public DateTime? UpdatedOn { get; set; }

        public bool? IsDeleted { get; set; }
        public string? DeletedBy { get; set; }

        public DateTime? DeletedOn { get; set; }

    }

    #endregion

    #region VehicleModel

    public class VehicleFormField
    {
        public string FieldText { get; set; }
        public string FieldType { get; set; }
        public string FieldId { get; set; }
        public string FieldName { get; set; }

        // Use object here to support both string and array (for "type")
        public List<string> Type { get; set; }

        public bool Required { get; set; }
        public bool IsActive { get; set; }
        public bool IsInputControl { get; set; }
        public string RecordType { get; set; }

        // Nullable int? properties for optional fields
        public int? MinLength { get; set; }
        public int? MaxLength { get; set; }
        public string Pattern { get; set; }
    }
    public class VehicleRequest
    {
        public string VehicleName { get; set; } = null!;

        public string VehicleNumber { get; set; } = null!;

        public string VehicleType { get; set; } = null!;

        public string FuelType { get; set; } = null!;

        public string? RfidTagNumber { get; set; }

        public string? Code { get; set; }

        public bool NotifyMeOnEntryExit { get; set; }
        public IFormFile vehicleFile { get; set; }

    }
    public class AddResidentVehicle
    {
        public Guid Id { get; set; }
        public Guid ResidentId { get; set; }
        public string VehicleName { get; set; } = null!;

        public string VehicleNumber { get; set; } = null!;

        public string VehicleType { get; set; } = null!;

        public string FuelType { get; set; } = null!;

        public string? RfidTagNumber { get; set; }

        public string? Code { get; set; }

        public bool NotifyMeOnEntryExit { get; set; }
        public string? VehiclePicture { get; set; }
        public bool IsActive { get; set; }
        public string? CreatedBy { get; set; }
        public DateTime? CreatedOn { get; set; }
        public bool? IsDeleted { get; set; }



    }

    public class UpdateVehicleSelectedRecord
    {
        public Guid ResidentVehicleId { get; set; }
        public string? VehicleName { get; set; } = null!;
        public string? VehicleNumber { get; set; } = null!;
        public string? RfidTagNumber { get; set; }
        public string? Code { get; set; }
        public bool? NotifyMeOnEntryExit { get; set; }
        public IFormFile? file { get; set; }
        public bool? IsDeleted { get; set; }
    }

    public class UpdateResidentVehicle
    {
        public Guid ResidentVehicleId { get; set; }
        public string? VehicleName { get; set; } = null!;
        public string? VehicleNumber { get; set; } = null!;
        public string? RfidTagNumber { get; set; }
        public string? Code { get; set; }
        public bool? NotifyMeOnEntryExit { get; set; }
        public string? VehiclePicture { get; set; } = null!;
        public bool IsActive { get; set; }

        public string? UpdatedBy { get; set; }

        public DateTime? UpdatedOn { get; set; }

        public bool? IsDeleted { get; set; }
        public string? DeletedBy { get; set; }

        public DateTime? DeletedOn { get; set; }

    }
    #endregion

    #region FrequentGuestsModel

    public class GuestVisitForm
    {
        public List<VisitField> Once { get; set; }
        public List<VisitField> Frequently { get; set; }
    }

    public class VisitField
    {
        public string FieldText { get; set; }
        public string FieldType { get; set; }
        public string FieldId { get; set; }
        public string FieldName { get; set; }
        public bool Required { get; set; }
        public bool IsActive { get; set; }
        public bool IsInputControl { get; set; }
        public string RecordType { get; set; }

        public List<string> SelectedItems { get; set; }  // Optional; only for dropdowns
    }

    public class AddResidentFrequentGuestsSelectedRecord
    {
        public string GuestType { get; set; }
        public bool? IsPrivateEntry { get; set; }
        public string? SelectDate { get; set; }
        public string? StartingFrom { get; set; }
        public string? ValidFor { get; set; }
        public string? AllowEntryForNext { get; set; }
        public string? StartDate { get; set; }
        public string? EndDate { get; set; }
        public string GuestName { get; set; }
        public string GuestNumber { get; set; }
        public string? Note { get; set; }
    }

    public class AddResidentFrequentGuests
    {
        public Guid Id { get; set; }

        public Guid ResidentId { get; set; }

        public string Type { get; set; } = null!;

        public bool? IsPrivateEntry { get; set; }

        public DateOnly? SelectDate { get; set; }

        public TimeOnly? StartingFrom { get; set; }

        public string? ValidFor { get; set; }

        public string? AllowEntryForNext { get; set; }

        public DateOnly? StartDate { get; set; }

        public DateOnly? EndDate { get; set; }

        public string? GuestNumber { get; set; }

        public string? GuestName { get; set; }

        public string UniqueEntryNumber { get; set; } = null!;

        public string? Note { get; set; }

        public bool IsActive { get; set; }

        public string? CreatedBy { get; set; }

        public DateTime? CreatedOn { get; set; }

        public bool? IsDeleted { get; set; }



    }

    public class UpdateFrequentlyGuestsSelectedRecord
    {
        public Guid ResidentFrequentGuestsId { get; set; }
        public string GuestType { get; set; }
        public bool? IsPrivateEntry { get; set; }
        public DateOnly? SelectDate { get; set; }
        public TimeOnly? StartingFrom { get; set; }
        public string? ValidFor { get; set; }
        public string? AllowEntryForNext { get; set; }
        public DateOnly? StartDate { get; set; }
        public DateOnly? EndDate { get; set; }
        public string GuestName { get; set; }
        public string GuestNumber { get; set; }
        public string? Note { get; set; }
        public bool? IsDeleted { get; set; }
    }

    public class UpdateResidentFrequentlyGuests
    {
        public Guid ResidentFrequentlyGuestsId { get; set; }
        public string Type { get; set; } = null!;

        public bool? IsPrivateEntry { get; set; }

        public DateOnly? SelectDate { get; set; }

        public TimeOnly? StartingFrom { get; set; }

        public string? ValidFor { get; set; }

        public string? AllowEntryForNext { get; set; }

        public DateOnly? StartDate { get; set; }

        public DateOnly? EndDate { get; set; }

        public string? GuestNumber { get; set; }

        public string? GuestName { get; set; }

        public string UniqueEntryNumber { get; set; } = null!;

        public string? Note { get; set; }

        public bool IsActive { get; set; }

        public string? UpdatedBy { get; set; }

        public DateTime? UpdatedOn { get; set; }

        public bool? IsDeleted { get; set; }
        public string? DeletedBy { get; set; }

        public DateTime? DeletedOn { get; set; }

    }

    #endregion

    #region Frequent Entries Model

    public class EntityTypeDetailResponse
    {
        [JsonProperty("FrequentEntriesForm")]
        public List<ParentFieldModel> FrequentEntriesForm { get; set; }
    }

    public class ParentFieldModel
    {
        public string fieldText { get; set; }
        public string fieldType { get; set; }
        public string fieldId { get; set; }
        public string fieldName { get; set; }
        public bool required { get; set; }
        public bool isActive { get; set; }
        public bool isInputControl { get; set; }
        public string recordType { get; set; }
        public string iconUrl { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public FieldSet childForm { get; set; }
    }

    public class FieldSet
    {
        public List<ChildFieldModel> once { get; set; }
        public List<ChildFieldModel> frequently { get; set; }
    }

    public class ChildFieldModel
    {
        public string fieldText { get; set; }
        public string fieldType { get; set; }
        public string fieldId { get; set; }
        public string fieldName { get; set; }
        public bool required { get; set; }
        public bool isActive { get; set; }
        public bool isInputControl { get; set; }
        public string recordType { get; set; }
        public string iconUrl { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Include)]
        public List<string> selectedItems { get; set; }
    }

   

    public class AddResidentFrequentEntriesSelectedRecord
    {
        public string EntryType { get; set; } // Cab, Delivery or Visiting Help
        public string FrequentlyType { get; set; } // Once or Frequently
        public bool? IsPrivateEntry { get; set; }// Required : Delivery : Once
        public string? SelectDate { get; set; } // Required : Any EntryType : Once
        public string? StartingFrom { get; set; } // Required: Any EntryType :  Once
        public string? ValidFor { get; set; } // Required: Any EntryType : Once
        public string? VehicleNo { get; set; } // Required:  Cab : Once
        public int? CompanyId { get; set; } // Required:  Any EntryType : Any FrequentlyType :  value: CabCompanyId, DeliveryCompanyId or VisitingCategoryId
        public string? VisitingHelpName { get; set; } // Required: Visiting Help: Any FrequentlyType: value: Company Name Enter
        public string? DaysOfWeek { get; set; } //  Required : Any EntryType : Frequently 
        public string? Validity { get; set; } //  Required : Any EntryType : Frequently 
        public string? StartTime { get; set; } //  Required : Any EntryType : Frequently 
        public string? EndTime { get; set; } //  Required : Any EntryType : Frequently 
        public string? EntriesPerDay { get; set; } //  Required : Any EntryType : Frequently 
    }

    public class AddResidentFrequentEntries
    {
        public Guid Id { get; set; }

        public Guid ResidentId { get; set; }

        public string EntryType { get; set; }

        public string FrequentlyType { get; set; }

        public string? AllowEntryInNext { get; set; }

        public DateOnly? EntryDate { get; set; }

        public bool? IsSurpriseDelivery { get; set; }

        public bool? IsLeaveAtGate { get; set; }

        public TimeOnly? EntryTimeStart { get; set; }

        public TimeOnly? EntryTimeEnd { get; set; }

        public string? VehicleNo { get; set; }

        public int? CabCompanyId { get; set; }

        public int? DeliveryCompanyId { get; set; }

        public int? VisitingHelpCategoryId { get; set; }

        public string? DaysOfWeek { get; set; }

        public string? Validity { get; set; }

        public string? EntriesPerDay { get; set; }

        public string? UniqueEntryCode { get; set; } = null!;

        public bool? IsActive { get; set; }

        public string? CreatedBy { get; set; }

        public DateTime? CreatedOn { get; set; }

        public bool? IsDeleted { get; set; }

        public string? VisitingHelpName { get; set; }

    }



    public class UpdateResidentFrequentEntriesSelectedRecord
    {
        public Guid ResidentFrequentEntryId { get; set; }
        public bool? IsPrivateEntry { get; set; }// Required : Delivery : Once
        public string? SelectDate { get; set; } // Required : Any EntryType : Once
        public string? StartingFrom { get; set; } // Required: Any EntryType :  Once
        public string? ValidFor { get; set; } // Required: Any EntryType : Once
        public string? VehicleNo { get; set; } // Required:  Cab : Once
        public int? CompanyId { get; set; } // Required:  Any EntryType : Any FrequentlyType :  value: CabCompanyId, DeliveryCompanyId or VisitingCategoryId
        public string? VisitingHelpName { get; set; } // Required: Visiting Help: Any FrequentlyType: value: Company Name Enter
        public string? DaysOfWeek { get; set; } //  Required : Any EntryType : Frequently 
        public string? Validity { get; set; } //  Required : Any EntryType : Frequently 
        public string? StartTime { get; set; } //  Required : Any EntryType : Frequently 
        public string? EndTime { get; set; } //  Required : Any EntryType : Frequently 
        public string? EntriesPerDay { get; set; } //  Required : Any EntryType : Frequently 
        public bool? IsDeleted { get; set; } //  Required : Any EntryType : Frequently 
    }
    public class UpdateResidentFrequentlyEntry
    {
        public Guid ResidentFrequentlyEntryId { get; set; }

        public string EntryType { get; set; } = null!; // Cab, Delivery, Visiting Help

        public string FrequentlyType { get; set; } = null!; // Once or Frequently

        public string? AllowEntryInNext { get; set; }

        public DateOnly? EntryDate { get; set; }

        public bool? IsSurpriseDelivery { get; set; }

        public bool? IsLeaveAtGate { get; set; }

        public TimeOnly? EntryTimeStart { get; set; }

        public TimeOnly? EntryTimeEnd { get; set; }

        public string? VehicleNo { get; set; }

        public int? CabCompanyId { get; set; }

        public int? DeliveryCompanyId { get; set; }

        public int? VisitingHelpCategoryId { get; set; }

        public string? VisitingHelpName { get; set; }

        public string? DaysOfWeek { get; set; }

        public string? Validity { get; set; }

        public string? EntriesPerDay { get; set; }

        public string? UniqueEntryCode { get; set; }

        public bool? IsActive { get; set; }

        public string? UpdatedBy { get; set; }

        public DateTime? UpdatedOn { get; set; }

        public bool? IsDeleted { get; set; }

        public string? DeletedBy { get; set; }

        public DateTime? DeletedOn { get; set; }
    }


    #endregion

    #region Daily Help Model

    public class AddResidentDailyHelpSelectedRecord
    {
        public Guid UserProfileId { get; set; }
    }
    public class AddResidentDailyHelp
    {
        public Guid Id { get; set; }

        public Guid ResidentId { get; set; }

        public Guid SocietyUserFlatWorkingHistoryId { get; set; }

        public bool? IsActive { get; set; }

        public string? CreatedBy { get; set; }

        public DateTime? CreatedOn { get; set; }


    }
    public class AddSocietyUserFlatWorkingHistory
    {
        public Guid Id { get; set; }

        public Guid SocietyUserProfileId { get; set; }

        public Guid SocietyFlatId { get; set; }

        public bool IsAddedByResident { get; set; }

        public string? CreatedBy { get; set; }

        public DateTime? CreatedOn { get; set; }

        public bool IsNotify { get; set; }

        public int? Ratings { get; set; }

    }

    #endregion

}