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
    [ApiController]
    [Route("api/[controller]")]
    public class SocietyFlatsController : ControllerBase
    {
        private readonly DataContext _context;
        private readonly IMapper _mapper;

        public SocietyFlatsController(DataContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<SocietyFlatDTO>>> GetAll()
        {
            var flats = await _context.SocietyFlat
                .Where(f => f.IsDeleted != true)
                .ToListAsync();

            return Ok(_mapper.Map<List<SocietyFlatDTO>>(flats));
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<SocietyFlatDTO>> GetById(int id)
        {
            var flat = await _context.SocietyFlat.FindAsync(id);
            if (flat == null || flat.IsDeleted == true)
                return NotFound();

            return Ok(_mapper.Map<SocietyFlatDTO>(flat));
        }

        [HttpPost]
        public async Task<ActionResult<SocietyFlatDTO>> Create(SocietyFlatDTO cto)
        {
            var entity = _mapper.Map<SocietyFlat>(cto);
            entity.CreatedOn = DateTime.UtcNow;

            _context.SocietyFlat.Add(entity);
            await _context.SaveChangesAsync();

            var dto = _mapper.Map<SocietyFlatDTO>(entity);
            return CreatedAtAction(nameof(GetById), new { id = dto.Id }, dto);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, SocietyFlatDTO cto)
        {
            var entity = await _context.SocietyFlat.FindAsync(id);
            if (entity == null || entity.IsDeleted == true)
                return NotFound();

            _mapper.Map(cto, entity);
            entity.UpdatedOn = DateTime.UtcNow;

            await _context.SaveChangesAsync();
            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var entity = await _context.SocietyFlat.FindAsync(id);
            if (entity == null || entity.IsDeleted == true)
                return NotFound();

            entity.IsDeleted = true;
            entity.DeletedOn = DateTime.UtcNow;
            entity.DeletedBy = "System"; // Replace with real user info if needed

            await _context.SaveChangesAsync();
            return NoContent();
        }
    }
}