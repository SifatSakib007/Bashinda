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

        public HouseController(IHouseService houseService)
        {
            _houseService = houseService;
        }

        [HttpPost]
        public async Task<IActionResult> CreateHouse([FromBody] CreateHouseDto model)
        {
            if(!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "0");
            var (success, house, errors) = await _houseService.CreateHouseAsync(model, userId);
            if (success && house != null)
            {
                return CreatedAtAction(nameof(GetHouseById), new { id = house.Id }, house);
            }
            return BadRequest(new { Errors = errors });
        }

        private object GetHouseById()
        {
            throw new NotImplementedException();
        }
    }
}
