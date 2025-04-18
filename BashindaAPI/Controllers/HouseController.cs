using BashindaAPI.DTOs;
using BashindaAPI.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace BashindaAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class HouseController : ControllerBase
    {
        public readonly IHouseService _houseService;
        public readonly ILogger<HouseController> _logger;

        public HouseController(IHouseService houseService, ILogger<HouseController> logger)
        {
            _houseService = houseService;
            _logger = logger;
        }

        [HttpPost("create")]
        public async Task<IActionResult> CreateHouse([FromBody] CreateHouseDto model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "0");
            _logger.LogInformation($"User ID: {userId}");   
            var (success, house, errors) = await _houseService.CreateHouseAsync(model, userId);
            _logger.LogInformation($"House creation success: {success}, Errors: {string.Join(", ", errors)}");
            if (success && house != null)
            {
                _logger.LogInformation($"House created successfully: {house.Id}");
                return CreatedAtAction(nameof(GetHouseById), new { id = house.Id }, house);
            }
            _logger.LogError($"House creation failed: {string.Join(", ", errors)}");
            return BadRequest(new { Errors = errors });
        }

        [HttpGet("getHouse/{id}")]
        public async Task<IActionResult> GetHouseById(int id)
        {
            var (success, house, errors) = await _houseService.GetHouseByIdAsync(id);
            if (success && house != null)
            {
                return Ok(house);
            }
            return NotFound(new { Errors = errors });

        }
    }
}
