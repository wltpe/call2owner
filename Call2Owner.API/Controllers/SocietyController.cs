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
using System.Net.Sockets;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using Utilities;
using System.Net.Http.Headers;

namespace Call2Owner.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SocietyController : ControllerBase
    {
        private readonly IMapper _mapper;
        private readonly DataContext _context;
        private readonly IConfiguration _configuration;
        private readonly EmailService _emailService;
        private readonly string EncryptionKey = "ABCabc123!@#hdgRHF1245KDnjkjfdsfdkv";
        private readonly ILogger<SocietyController> _logger;
        private readonly RestClient _client;

        public SocietyController(DataContext context, IConfiguration configuration,
            IMapper mapper, EmailService emailService, ILogger<SocietyController> logger, RestClient client)
        {
            _mapper = mapper;
            _context = context ?? throw new ArgumentNullException(nameof(context));
            _configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
            _logger = logger;
            _client = client;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<SocietyDto>>> GetAll()
        {
            var societies = await _context.Society
                .Where(s => s.IsDeleted != true)
                .ToListAsync();

            return Ok(_mapper.Map<List<SocietyDto>>(societies));
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<SocietyDto>> GetById(int id)
        {
            var society = await _context.Society.FindAsync(id);
            if (society == null || society.IsDeleted == true)
                return NotFound();

            return Ok(_mapper.Map<SocietyDto>(society));
        }

        //[Authorize(Policy = Utilities.Module.UserManagement)]
        //[Authorize(Policy = Utilities.Permission.Add)]
        //[Authorize]
        [HttpPost("create")]
        public async Task<ActionResult<SocietyDto>> Create(CreateSocietyDto dto)
        {
            var currentUserId = Convert.ToString(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "0");

            var existingSociety = await _context.Society
        .FirstOrDefaultAsync(u =>
        u.Country.Id == dto.CountryId &&
        u.State.Id == dto.StateId &&
        u.City.Id == dto.CityId &&
        u.Name.ToLower() == dto.Name.ToLower());

            if (existingSociety != null)
            {
                return BadRequest("Socity already exist.");
            }

            Guid Username = Guid.NewGuid();

            var newSociety = new Society 
            {
            Id = Username,
            CountryId = dto.CountryId,
            StateId = dto.StateId,
            CityId = dto.CityId,
            Name = dto.Name,
            Description = dto.Description,
            SocietyImage=   dto.SocietyImage,
            EntityTypeDetailId= dto.EntityTypeDetailId,
            IsActive=true,
            IsApproved=false,
            CreatedBy=currentUserId,
            CreatedOn=DateTime.UtcNow,
            IsDeleted=false,
            Longitude=dto.Longitude,
            Latitude=dto.Latitude,
            PinCode=dto.PinCode,
            Address=dto.Address,
            IsDocumentRequired = dto.IsDocumentRequired
            };

            await _context.Society.AddAsync(newSociety);

            await _context.SaveChangesAsync();

            return _mapper.Map<SocietyDto>(newSociety);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(Guid id, SocietyDto dto)
        {
            if (id != dto.Id)
                return BadRequest();

            var existing = await _context.Society.FindAsync(id);
            if (existing == null || existing.IsDeleted == true)
                return NotFound();

            // Preserve approval fields
            var preserved = new
            {
                existing.IsApproved,
                existing.ApprovedBy,
                existing.ApprovedOn,
                existing.ApprovedComment
            };

            _mapper.Map(dto, existing);

            // Restore approval fields
            existing.IsApproved = preserved.IsApproved;
            existing.ApprovedBy = preserved.ApprovedBy;
            existing.ApprovedOn = preserved.ApprovedOn;
            existing.ApprovedComment = preserved.ApprovedComment;
            existing.UpdatedOn = DateTime.UtcNow;

            await _context.SaveChangesAsync();
            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var society = await _context.Society.FindAsync(id);
            if (society == null || society.IsDeleted == true)
                return NotFound();

            society.IsDeleted = true;
            society.DeletedOn = DateTime.UtcNow;
            society.DeletedBy = "System"; // Replace with user from context if needed

            await _context.SaveChangesAsync();
            return NoContent();
        }

        [HttpPost("{id}/approve")]
        public async Task<IActionResult> Approve(int id, [FromBody] SocietyApprovalDto model)
        {
            var society = await _context.Society.FindAsync(id);
            if (society == null || society.IsDeleted == true)
                return NotFound();

            society.IsApproved = model.IsApproved;
         //   society.ApprovedBy = model.ApprovedBy;
            society.ApprovedComment = model.ApprovedComment;
            society.ApprovedOn = DateTime.UtcNow;

            await _context.SaveChangesAsync();
            return Ok(_mapper.Map<SocietyDto>(society));
        }

        [HttpGet("get-society-document-required-fields")]
        public async Task<IActionResult> GetSocietyDocumentRequiredField(int EntityTypeId)
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
                return NotFound(new { message = "No Society field found, contact administation." });
            }
            return Ok(entityTypeDetails);
        }

        [HttpGet("get-all-society")]
        public async Task<IActionResult> GetAllSociety()
        {
            try
            {
                var currentUserId = Convert.ToString(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "0");

                var result = await _context.Society
                    .OrderBy(d => d.Name)
                    .ToListAsync();

                if (result.Count == 0)
                {
                    return NotFound(new { message = "No Society found." });
                }

                var societyDTOs = _mapper.Map<List<SocietyDto>>(result);

                // Deserialize DetailJson for each society
                for (int i = 0; i < result.Count; i++)
                {
                    var detailJson = result[i].DetailJson;
                    if (!string.IsNullOrEmpty(detailJson))
                    {
                        societyDTOs[i].SocietyDocument = JsonConvert.DeserializeObject<SocietyUploadSelectedRecord>(detailJson);
                    }
                }

                return Ok(societyDTOs);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred", error = ex.Message });
            }
        }


        [HttpPost("update-society-documents")]
        public async Task<IActionResult> UpdateSocietyDocuments([FromForm] SocietySelectedRecord obj)
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
                    return NotFound("No Society field found, contact administation.");
                }

                var fullResponse = entityTypeDetails;
                var selectedRecords = obj;

                //Compare 
                List<SocietyMismatchReport> mismatchedResults = new();

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


                    var report = new SocietyMismatchReport
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
                        if (possible.type != null)
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

                    bool IsFileReqiured = possible.type.Contains(selectedRecords.type);
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

                    var selected = new SocietyUploadSelectedRecord
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

                    UpdateSociety updateSociety = new UpdateSociety();

                    updateSociety.Id = selectedRecords.societyId;
                    updateSociety.UpdatedOn = DateTime.UtcNow;
                    updateSociety.UpdatedBy = currentUserId;
                    updateSociety.DetailJson = json;


                    var returnResult = await UpdateSocietyDocumentUploadedAsync(updateSociety);


                    if (returnResult != null)
                    {
                        returnResult.SocietyDocument = JsonConvert.DeserializeObject<SocietyUploadSelectedRecord>(returnResult.DetailJson);
                        return Ok(returnResult);
                    }
                    else
                    {
                        return NotFound(new { Message = "Unable to update society" });
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
                string fileUrl = $"C://Users//WLTPE//source//repos//c2o//Call2Owner.API//Images//Society//Documents/{blobName}";


                return fileUrl; // Return the accessible file URL
            }
            catch (Exception ex)
            {
                // Log and rethrow the exception
                throw new InvalidOperationException("An error occurred while uploading the file/image.", ex);
            }
        }

        private async Task<UpdateSocietyDto?> UpdateSocietyDocumentUploadedAsync(UpdateSociety updateSociety)
        {
            // Check if a record already exists
            var userExists = await _context.Society.FirstOrDefaultAsync(x =>
                                    x.Id == updateSociety.Id && x.IsDeleted != true
                                    && x.IsActive == true && x.IsDocumentRequired == true && x.IsDocumentUploaded == false && x.IsApproved == false);

            if (userExists != null)
            {
                userExists.UpdatedBy = updateSociety.UpdatedBy;
                userExists.UpdatedOn = updateSociety.UpdatedOn;
                userExists.DetailJson = updateSociety.DetailJson;
                userExists.IsDocumentUploaded = true;

                _context.Society.Update(userExists);

                // Save changes to the database
                await _context.SaveChangesAsync();

                var residentResponseDTO = _mapper.Map<UpdateSocietyDto>(userExists);

                return residentResponseDTO;
            }
            else
            {
                // Return null if the record exists
                return null;
            }
        }

    }

    public class ApprovalModel
    {
        public bool IsApproved { get; set; }
        public string ApprovedBy { get; set; }
        public string ApprovedComment { get; set; }
    }

    public class UpdateSociety
    {
        public Guid Id { get; set; }
        public string UpdatedBy { get; set; }
        public DateTime UpdatedOn { get; set; }
        public string DetailJson { get; set; }
    }
}