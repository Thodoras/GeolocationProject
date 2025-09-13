using GeolocationAPI.API.Http.DTOs;
using GeolocationAPI.Domain;
using GeolocationAPI.Application.BackgroundGeolocationIP;
using GeolocationAPI.Utils.Exceptions;
using Microsoft.AspNetCore.Mvc;
using GeolocationAPI.Application.GeolocationIP;

namespace GeolocationAPI.API.Http.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class GeolocationIPController : ControllerBase
    {

        private readonly IGeolocationIPService _geolocationIPService;
        private readonly IBackgroundGeolocationIPService _backgroundGeolocationIPService;

        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly ILogger<GeolocationIPController> _logger;

        public GeolocationIPController(
            IGeolocationIPService geolocationIPService,
            IBackgroundGeolocationIPService backgroundGeolocationIPService,
            IHttpContextAccessor httpContextAccessor,
            ILogger<GeolocationIPController> logger
            )
        {
            _geolocationIPService = geolocationIPService;
            _backgroundGeolocationIPService = backgroundGeolocationIPService;

            _httpContextAccessor = httpContextAccessor;
            _logger = logger;
        }

        [HttpGet("{ipAddress}")]
        public async Task<IActionResult> GetGeolocationIP(string ipAddress)
        {
            try
            {
                var structuredIP = new IP(ipAddress);
                var domainData = await _geolocationIPService.GetGeolocationIPAsync(structuredIP);
                var response = new GetGeolocationIPResponse(domainData);
                return Ok(response);
            }
            catch (InvalidIPFormat ex)
            {
                _logger.LogError("Invalid IP format: {Message}", ex.Message);
                return BadRequest(new { error = ex.Message });
            }
            catch (BadGateway ex)
            {
                _logger.LogError("External service error: {Message}", ex.Message);
                return StatusCode(502, "Failed to retrieve data from external service. Please try again later.");
            }
            catch (Exception ex)
            {
                _logger.LogError("Internal error: {Message}", ex.Message);
                return StatusCode(500, "An unexpected error occurred. Please try again later.");
            }

        }

        [HttpPost("batch")]
        public async Task<IActionResult> CreateBatchGeolocationIP([FromBody] CreateBatchGeolocationIPRequest request)
        {
            try
            {
                var domainIps = request.IpAddresses.Select(ip => new IP(ip)).ToList();
                var processId = await _backgroundGeolocationIPService.StartProcessBatch(domainIps);
                var baseUrl = $"{_httpContextAccessor.HttpContext?.Request.Scheme}://{_httpContextAccessor.HttpContext?.Request.Host}";
                var response = new CreateBatchGeolocationIPResponse
                {
                    ProcessId = processId,
                    StatusUrl = $"{baseUrl}/api/geolocationip/batch/{processId}/status"
                };
                return Ok(response);
            }
            catch (Exception ex)
            {
                _logger.LogError("Internal error: {Message}", ex.Message);
                return StatusCode(500, "An unexpected error occurred. Please try again later.");
            }
        }

        [HttpGet("batch/{processId}/status")]
        public async Task<IActionResult> GetBatchStatus(Guid processId)
        {
            try
            {
                if (!Guid.TryParse(processId.ToString(), out var validProcessId))
                {
                    _logger.LogWarning("Invalid processId format: {ProcessId}", processId);
                    return BadRequest(new { error = "Invalid processId format." });
                }
                var batchStatus = await _backgroundGeolocationIPService.GetBatchStatusAsync(validProcessId);
                var response = GetBatchStatusResponse.FromDomain(batchStatus);
                return Ok(response);
            }
            catch (Exception ex)
            {
                _logger.LogError("Error retrieving batch status for process: {Message}", ex.Message);
                return StatusCode(500, "Error retrieving batch status.");
            }
        }
    }
}