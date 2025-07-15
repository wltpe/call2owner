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
using Module = Call2Owner.Models.Module;
using Permission = Call2Owner.Models.Permission;

namespace Call2Owner.Controllers;

    [Route("api/[controller]")]
    [ApiController]
    public class AdminManagerController : ControllerBase
    {
        private readonly IMapper _mapper;
        private readonly DataContext _context;
        private readonly IConfiguration _configuration;
        private readonly ILogger<AuthController> _logger;
        private readonly RestClient _client;

        public AdminManagerController(DataContext context, IConfiguration configuration,
            IMapper mapper, EmailService emailService, ILogger<AuthController> logger, RestClient client)
        {
            _mapper = mapper;
            _context = context ?? throw new ArgumentNullException(nameof(context));
            _configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
            _logger = logger;
            _client = client;
        }

    #region Module
    [HttpPost("module")]
    public async Task<IActionResult> Create(ModuleDto dto)
    {
        var module = new Module
        {
            ModuleName = dto.ModuleName
        };

        _context.Module.Add(module);
        await _context.SaveChangesAsync();

        dto.ModuleId = module.ModuleId;
        return CreatedAtAction(nameof(GetById), new { id = dto.ModuleId }, dto);
    }

    // READ ALL
    [HttpGet("module")]
    public async Task<ActionResult<IEnumerable<ModuleDto>>> GetAll()
    {
        var modules = await _context.Module
            .Select(m => new ModuleDto
            {
                ModuleId = m.ModuleId,
                ModuleName = m.ModuleName
            }).ToListAsync();

        return Ok(modules);
    }

    // READ BY ID
    [HttpGet("module/{id}")]
    public async Task<ActionResult<ModuleDto>> GetById(int id)
    {
        var module = await _context.Module.FindAsync(id);

        if (module == null)
            return NotFound();

        return Ok(new ModuleDto
        {
            ModuleId = module.ModuleId,
            ModuleName = module.ModuleName
        });
    }

    // UPDATE
    [HttpPost("module/edit/{id}")]
    public async Task<IActionResult> Update(int id, ModuleDto dto)
    {
        var module = await _context.Module.FindAsync(id);
        if (module == null)
            return NotFound();

        module.ModuleName = dto.ModuleName;
        await _context.SaveChangesAsync();

        return NoContent();
    }

    // DELETE
    [HttpPost("module/delete/{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        var module = await _context.Module.FindAsync(id);
        if (module == null)
            return NotFound();

        _context.Module.Remove(module);
        await _context.SaveChangesAsync();

        return NoContent();
    }
    #endregion


    #region Permission

    [HttpPost("permission")]
    public async Task<IActionResult> CreatePermission(PermissionDto dto)
    {
        var permission = new Permission
        {
            PermissionName = dto.PermissionName
        };

        _context.Permission.Add(permission);
        await _context.SaveChangesAsync();

        dto.Id = permission.Id;
        return CreatedAtAction(nameof(GetPermissionById), new { id = dto.Id }, dto);
    }

    [HttpGet("permission")]
    public async Task<ActionResult<IEnumerable<PermissionDto>>> GetAllPermissions()
    {
        var permissions = await _context.Permission
            .Select(p => new PermissionDto
            {
                Id = p.Id,
                PermissionName = p.PermissionName
            }).ToListAsync();

        return Ok(permissions);
    }

    [HttpGet("permission/{id}")]
    public async Task<ActionResult<PermissionDto>> GetPermissionById(int id)
    {
        var permission = await _context.Permission.FindAsync(id);

        if (permission == null)
            return NotFound();

        return Ok(new PermissionDto
        {
            Id = permission.Id,
            PermissionName = permission.PermissionName
        });
    }

    [HttpPost("permission/edit/{id}")]
    public async Task<IActionResult> UpdatePermission(int id, PermissionDto dto)
    {
        var permission = await _context.Permission.FindAsync(id);
        if (permission == null)
            return NotFound();

        permission.PermissionName = dto.PermissionName;
        await _context.SaveChangesAsync();

        return NoContent();
    }

    [HttpPost("permission/delete/{id}")]
    public async Task<IActionResult> DeletePermission(int id)
    {
        var permission = await _context.Permission.FindAsync(id);
        if (permission == null)
            return NotFound();

        _context.Permission.Remove(permission);
        await _context.SaveChangesAsync();

        return NoContent();
    }

    #endregion

    #region Permission
    [HttpPost("role")]
    public async Task<IActionResult> CreateRole(RoleDto dto)
    {
        var role = new Role
        {
            RoleName = dto.RoleName,
            DisplayName = dto.DisplayName,
            ParentRoleId = dto.ParentRoleId
        };

        _context.Role.Add(role);
        await _context.SaveChangesAsync();

        dto.Id = role.Id;
        return CreatedAtAction(nameof(GetRoleById), new { id = dto.Id }, dto);
    }

    [HttpGet("role")]
    public async Task<ActionResult<IEnumerable<RoleDto>>> GetAllRoles()
    {
        var roles = await _context.Role
            .Select(r => new RoleDto
            {
                Id = r.Id,
                RoleName = r.RoleName,
                DisplayName = r.DisplayName,
                ParentRoleId = r.ParentRoleId,
                ParentRoleName = r.ParentRole != null ? r.ParentRole.RoleName : null
            }).ToListAsync();

        return Ok(roles);
    }

    [HttpGet("role/{id}")]
    public async Task<ActionResult<RoleDto>> GetRoleById(int id)
    {
        var role = await _context.Role
            .Include(r => r.ParentRole)
            .FirstOrDefaultAsync(r => r.Id == id);

        if (role == null)
            return NotFound();

        return Ok(new RoleDto
        {
            Id = role.Id,
            RoleName = role.RoleName,
            DisplayName = role.DisplayName,
            ParentRoleId = role.ParentRoleId,
            ParentRoleName = role.ParentRole?.RoleName
        });
    }

    [HttpPost("role/edit/{id}")]
    public async Task<IActionResult> UpdateRole(int id, RoleDto dto)
    {
        var role = await _context.Role.FindAsync(id);
        if (role == null)
            return NotFound();

        role.RoleName = dto.RoleName;
        role.DisplayName = dto.DisplayName;
        role.ParentRoleId = dto.ParentRoleId;

        await _context.SaveChangesAsync();

        return NoContent();
    }

    [HttpPost("role/delete/{id}")]
    public async Task<IActionResult> DeleteRole(int id)
    {
        var role = await _context.Role.FindAsync(id);
        if (role == null)
            return NotFound();

        _context.Role.Remove(role);
        await _context.SaveChangesAsync();

        return NoContent();
    }

    #endregion

    #region Society
    [HttpGet("society-by-city")]
    public async Task<ActionResult<IEnumerable<SocietyDto>>> GetAllSocietyByCityId(int CityId)
    {
        var societies = await _context.Society
            .Where(s => s.IsDeleted != true && s.IsActive == true && s.CityId == CityId)
            .OrderBy(s => s.Name)
            .ToListAsync();

        return Ok(_mapper.Map<List<SocietyDto>>(societies));
    }

    [HttpGet("society-document")]
    public async Task<IActionResult> GetSocietyDocumentRequiredField(Guid societyId, int EntityTypeId)
    {
        var result = await _context.EntityTypeDetail
                       .Where(d => d.IsDeleted != true && d.IsActive == true && d.EntityTypeId == EntityTypeId)
                       .OrderBy(d => d.Id)
                       .ToListAsync();

        // result is assumed to be a collection of dynamic or database rows
        List<EntityTypeDetail> entityTypeDetails = new List<EntityTypeDetail>();
        List<DetailItem> detailJson = new List<DetailItem>();

        if (result.Count > 0)
        {
            foreach (var row in result)
            {
                if (row.IsActive)
                {
                    detailJson = JsonConvert.DeserializeObject<List<DetailItem>>(row.DetailJson.ToString());
                }
            }
        }

        if (entityTypeDetails == null)
        {
            return NotFound(new { message = "No Society field found, contact administation." });
        }

        var selectedSociety = await _context.Society
                        .Where(x => x.Id == societyId)
                        .OrderBy(d => d.Name)
                        .FirstOrDefaultAsync();

        var societyDocument = await _context.SocietyDocumentUploaded
                .Where(x => x.Id == societyId)
                .OrderBy(d => d.Name)
                .FirstOrDefaultAsync();

        return Ok(new {
                            selectedSociety = selectedSociety,
                            requiredDocument = detailJson,
                            societyDocument = societyDocument
                      }
                 );
    }

    [HttpPost("approve-society")]
    public async Task<IActionResult> ApproveSocietyIfDocumentsExist(Guid societyId)
    {
        // Check if documents exist
        var documentCount = await _context.SocietyDocumentUploaded
                                .Where(doc => doc.Id == societyId)
                                .CountAsync();

        if (documentCount == 0)
        {
            return BadRequest(new { message = "Cannot approve society. No documents uploaded." });
        }

        // Fetch the society
        var society = await _context.Society.FirstOrDefaultAsync(x => x.Id == societyId);
        if (society == null)
        {
            return NotFound(new { message = "Society not found." });
        }

        // Approve logic (assuming a field like IsApproved or Status)
        society.IsApproved = true; // or society.Status = "Approved";
        society.UpdatedOn = DateTime.UtcNow;

        _context.Society.Update(society);
        await _context.SaveChangesAsync();

        return Ok(new { message = "Society approved successfully." });
    }

    #endregion
}