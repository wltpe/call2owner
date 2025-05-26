using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using OfficeOpenXml.FormulaParsing.LexicalAnalysis;
using Oversight.DTO;
using Oversight.Model;
using Oversight.Models;
using Oversight.Services;
using RestSharp;
using System.IdentityModel.Tokens.Jwt;
using System.Net.Sockets;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using Utilities;

namespace Oversight.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class SocietyDocumentRequiredToRegisterController : ControllerBase
    {
        private readonly DataContext _context;
        private readonly IMapper _mapper;

        public SocietyDocumentRequiredToRegisterController(DataContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<SocietyDocumentRequiredToRegisterDTO>>> GetAll()
        {
            var entities = await _context.SocietyDocumentRequiredToRegister
                .Where(d => d.IsDeleted != true)
                .ToListAsync();

            return Ok(_mapper.Map<List<SocietyDocumentRequiredToRegisterDTO>>(entities));
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<SocietyDocumentRequiredToRegisterDTO>> GetById(int id)
        {
            var entity = await _context.SocietyDocumentRequiredToRegister.FindAsync(id);
            if (entity == null || entity.IsDeleted == true)
                return NotFound();

            return Ok(_mapper.Map<SocietyDocumentRequiredToRegisterDTO>(entity));
        }

        [HttpPost]
        public async Task<ActionResult<SocietyDocumentRequiredToRegisterDTO>> Create(SocietyDocumentRequiredToRegisterDTO dto)
        {
            var entity = _mapper.Map<SocietyDocumentRequiredToRegister>(dto);
            entity.CreatedOn = DateTime.UtcNow;
            entity.IsActive = true;

            _context.SocietyDocumentRequiredToRegister.Add(entity);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetById), new { id = entity.Id }, _mapper.Map<SocietyDocumentRequiredToRegisterDTO>(entity));
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, SocietyDocumentRequiredToRegisterDTO dto)
        {
            if (id != dto.Id)
                return BadRequest();

            var entity = await _context.SocietyDocumentRequiredToRegister.FindAsync(id);
            if (entity == null || entity.IsDeleted == true)
                return NotFound();

            _mapper.Map(dto, entity);
            entity.UpdatedOn = DateTime.UtcNow;

            await _context.SaveChangesAsync();
            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var entity = await _context.SocietyDocumentRequiredToRegister.FindAsync(id);
            if (entity == null || entity.IsDeleted == true)
                return NotFound();

            entity.IsDeleted = true;
            entity.DeletedOn = DateTime.UtcNow;
            entity.DeletedBy = "System"; // Replace with current user if available

            await _context.SaveChangesAsync();
            return NoContent();
        }
    }
}