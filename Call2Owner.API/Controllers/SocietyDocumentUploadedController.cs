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
    public class SocietyDocumentUploadedController : ControllerBase
    {
        private readonly DataContext _context;
        private readonly IMapper _mapper;

        public SocietyDocumentUploadedController(DataContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<SocietyDocumentUploadedDTO>>> GetAll()
        {
            var list = await _context.SocietyDocumentUploaded
                .Where(d => d.IsDeleted != true)
                .ToListAsync();

            return Ok(_mapper.Map<List<SocietyDocumentUploadedDTO>>(list));
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<SocietyDocumentUploadedDTO>> GetById(int id)
        {
            var entity = await _context.SocietyDocumentUploaded.FindAsync(id);
            if (entity == null || entity.IsDeleted == true)
                return NotFound();

            return Ok(_mapper.Map<SocietyDocumentUploadedDTO>(entity));
        }

        [HttpPost]
        public async Task<ActionResult<SocietyDocumentUploadedDTO>> Create(SocietyDocumentUploadedDTO cto)
        {
            var entity = _mapper.Map<SocietyDocumentUploaded>(cto);
            entity.CreatedOn = DateTime.UtcNow;
            entity.IsActive = true;

            _context.SocietyDocumentUploaded.Add(entity);
            await _context.SaveChangesAsync();

            var resultDto = _mapper.Map<SocietyDocumentUploadedDTO>(entity);
            return CreatedAtAction(nameof(GetById), new { id = entity.Id }, resultDto);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, SocietyDocumentUploadedDTO cto)
        {
            var entity = await _context.SocietyDocumentUploaded.FindAsync(id);
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
            var entity = await _context.SocietyDocumentUploaded.FindAsync(id);
            if (entity == null || entity.IsDeleted == true)
                return NotFound();

            entity.IsDeleted = true;
            entity.DeletedOn = DateTime.UtcNow;
            entity.DeletedBy = "System"; // Optional: Replace with user info

            await _context.SaveChangesAsync();
            return NoContent();
        }
    }
}