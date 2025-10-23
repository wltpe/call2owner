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
    public class SocietyUserController : ControllerBase
    {
        private readonly IMapper _mapper;
        private readonly DataContext _context;
        private readonly IConfiguration _configuration;
        private readonly ILogger<AuthController> _logger;
        private readonly RestClient _client;

        public SocietyUserController(DataContext context, IConfiguration configuration,
            IMapper mapper, EmailService emailService, ILogger<AuthController> logger, RestClient client)
        {
            _mapper = mapper;
            _context = context ?? throw new ArgumentNullException(nameof(context));
            _configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
            _logger = logger;
            _client = client;
        }



    #region Society
    [Authorize(Policy = Utilities.Module.UserManagement)]
    [Authorize(Policy = Utilities.Permission.Get)]
    [HttpGet("get-all-building")]
    public async Task<ActionResult<IEnumerable<SocietyBuildingDTO>>> GetAllBuilding()
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

        var SocietyBuildings = _context.User
                                 .Where(u => u.UserName == Guid.Parse(currentUserId))
                                 .Join(_context.SocietyUser, u => u.UserName, su => su.SocietyUserId, (u, su) => su)
                                 .Join(_context.SocietyBuilding, su => su.SocietyId, sb => sb.SocietyId, (su, sb) => sb)
                                 .Where(sb => sb.IsActive)
                                 .OrderBy(sb => sb.Name)
                                 .Select(sb => new
                                 {
                                     sb.Id,
                                     sb.Name,
                                     sb.Description,
                                     sb.IsFavourite,
                                     sb.BuildingImage
                                 })
                                 .ToList();

        return Ok(_mapper.Map<List<SocietyBuildingDTO>>(SocietyBuildings));

        }
        catch (Exception ex)
        {

            return BadRequest(new { Message = "No record found." });
        }
    }

    #endregion
}