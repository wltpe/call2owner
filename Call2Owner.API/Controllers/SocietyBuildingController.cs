using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Oversight.DTO;
using Oversight.Model;

namespace Oversight.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class SocietyBuildingController : ControllerBase
    {
        private readonly DataContext _context;
        private readonly IMapper _mapper;

        public SocietyBuildingController(DataContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<SocietyBuildingCTO>>> GetAll()
        {
            var entities = await _context.SocietyBuilding
                .Where(x => x.IsDeleted != true)
                .ToListAsync();

            return Ok(_mapper.Map<List<SocietyBuildingCTO>>(entities));
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<SocietyBuildingCTO>> GetById(int id)
        {
            var entity = await _context.SocietyBuilding.FindAsync(id);
            if (entity == null || entity.IsDeleted == true)
                return NotFound();

            return Ok(_mapper.Map<SocietyBuildingCTO>(entity));
        }

        [HttpPost]
        public async Task<ActionResult<SocietyBuildingCTO>> Create(SocietyBuildingCTO cto)
        {
            var entity = _mapper.Map<SocietyBuilding>(cto);
            entity.CreatedOn = DateTime.UtcNow;
            entity.IsActive = true;

            _context.SocietyBuilding.Add(entity);
            await _context.SaveChangesAsync();

            cto.Id = entity.Id;
            return CreatedAtAction(nameof(GetById), new { id = entity.Id }, cto);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, SocietyBuildingCTO cto)
        {
            if (id != cto.Id)
                return BadRequest();

            var entity = await _context.SocietyBuilding.FindAsync(id);
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
            var entity = await _context.SocietyBuilding.FindAsync(id);
            if (entity == null || entity.IsDeleted == true)
                return NotFound();

            entity.IsDeleted = true;
            entity.DeletedOn = DateTime.UtcNow;
            entity.DeletedBy = "System"; // Replace if needed

            await _context.SaveChangesAsync();
            return NoContent();
        }
    }
}