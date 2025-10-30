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
        private readonly NotificationService _notificationService;


    public SocietyUserController(DataContext context, IConfiguration configuration,
            IMapper mapper, EmailService emailService, ILogger<AuthController> logger, RestClient client, NotificationService notificationService)
        {
            _mapper = mapper;
            _context = context ?? throw new ArgumentNullException(nameof(context));
            _configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
            _logger = logger;
            _client = client;
            _notificationService = notificationService;
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

    [Authorize(Policy = Utilities.Module.UserManagement)]
    [Authorize(Policy = Utilities.Permission.Get)]
    [HttpPost("Send-Entry-Approval-Notification-To-Resident")]
    public async Task<IActionResult> Send([FromBody] NotificationRequest request)
    {
        var ResidentVerificationCode = _context.SocietyFlat
       .Where(u => u.Id == request.SocietyFlatId)
       .Join(_context.Resident, u => u.Id,  su => su.SocietyFlatId, (u, su) => su)
       .Join(_context.User, su => su.UserId, sb => sb.UserName, (su, sb) => sb)
       .Where(sb => sb.IsActive).OrderBy(su => su.CreatedOn)
       .Select(sb => sb.VerificationCode)
       .FirstOrDefault();

        request.ResidentVerificationCode = ResidentVerificationCode;

        await _notificationService.SendNotificationAsync(request);
        return Ok("Notification sent successfully");
    }

    #endregion

    #region Models

    public class NotificationRequest
    {
        //   public string DeviceToken { get; set; }
        public Guid SocietyFlatId { get; set; }
        public string? EntyType { get; set; } // Delivery/Cab/Guest/Daily Help
        public string? ProfilePicture { get; set; } 
        public string? Name { get; set; }
        public string? PhoneNumber { get; set; }
        public string? CompanyLogo { get; set; } // Zepto/Uber/Zomato/Swiggy logo if available etc
        public string? CompanyName { get; set; } // Zepto/Uber/Zomato/Swiggy etc
        public string? ResidentVerificationCode { get; set; }
    }



    #endregion
}