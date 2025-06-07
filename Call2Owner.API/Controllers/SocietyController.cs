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
            IsApproved=true,
            ApprovedBy=currentUserId,
            ApprovedOn=DateTime.UtcNow,
            CreatedBy=currentUserId,
            CreatedOn=DateTime.UtcNow,
            IsDeleted=false,
            Longitude=dto.Longitude,
            Latitude=dto.Latitude,
            PinCode=dto.PinCode,
            Address=dto.Address
            };

            //var entity = _mapper.Map<Society>(dto);
            //entity.Id = Username;
            //entity.CreatedOn = DateTime.UtcNow;
            //entity.CreatedBy = currentUserId;
            //entity.IsActive = true;
            //entity.IsDeleted = false;


            //_context.Society.Add(entity);
                await _context.Society.AddAsync(newSociety);

            await _context.SaveChangesAsync();

            // return CreatedAtAction(nameof(GetById), new { id = entity.Id }, _mapper.Map<SocietyDto>(entity));
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
            society.ApprovedBy = model.ApprovedBy;
            society.ApprovedComment = model.ApprovedComment;
            society.ApprovedOn = DateTime.UtcNow;

            await _context.SaveChangesAsync();
            return Ok(_mapper.Map<SocietyDto>(society));
        }
    }

    public class ApprovalModel
    {
        public bool IsApproved { get; set; }
        public string ApprovedBy { get; set; }
        public string ApprovedComment { get; set; }
    }
}