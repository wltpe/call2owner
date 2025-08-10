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
using System.Net;
using Microsoft.AspNetCore.Http;

namespace Call2Owner.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TestController : ControllerBase
    {
        private readonly IMapper _mapper;
        private readonly DataContext _context;
        private readonly IConfiguration _configuration;
        private readonly EmailService _emailService;
        private readonly string EncryptionKey = "ABCabc123!@#hdgRHF1245KDnjkjfdsfdkv";
        private readonly ILogger<SocietyController> _logger;
        private readonly RestClient _client;

        public TestController(DataContext context, IConfiguration configuration,
            IMapper mapper, EmailService emailService, ILogger<SocietyController> logger, RestClient client)
        {
            _mapper = mapper;
            _context = context ?? throw new ArgumentNullException(nameof(context));
            _configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
            _logger = logger;
            _client = client;
        }

        [Route("getGetHOADocTest")]
        [HttpGet]
        public async Task<IActionResult> getGetHOADocTest(int docId, string folder)
        {
            var fileUrl = "https://access.nordicsec.com/documents/A100A/Resident/Legal/dismiss.gif";

            // Validate domain
            Uri uri = new Uri(fileUrl);
            if (!uri.Host.EndsWith("nordicsec.com"))
                return StatusCode(StatusCodes.Status403Forbidden, "Invalid file host");

            try
            {
                using (var client = new HttpClient())
                {
                    ServicePointManager.SecurityProtocol =
                        SecurityProtocolType.Tls12 | SecurityProtocolType.Tls11;

                    var fileResponse = await client.GetAsync(fileUrl);

                    if (!fileResponse.IsSuccessStatusCode)
                        return NotFound("File not found on server");

                    var fileName = Path.GetFileName(uri.LocalPath);
                    var fileStream = await fileResponse.Content.ReadAsStreamAsync();

                    return File(fileStream, "application/octet-stream", fileName);
                }
            }
            catch (Exception ex)
            {
                // Optionally log the exception: _logger.LogError(ex, "Failed to retrieve file.");
                return NotFound("File not found on server");
            }
        }

    }
}