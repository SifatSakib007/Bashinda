using BashindaAPI.DTOs;
using BashindaAPI.Models;
using BashindaAPI.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace BashindaAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class AdminsController : ControllerBase
    {
        private readonly IAdminService _adminService;
        private readonly ILogger<AdminsController> _logger;

        public AdminsController(IAdminService adminService, ILogger<AdminsController> logger)
        {
            _adminService = adminService;
            _logger = logger;
        }

        [HttpPost]
        [Authorize(Roles = "SuperAdmin")]
        public async Task<ActionResult<AdminDto>> CreateAdmin([FromBody] CreateAdminDto model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var (success, admin, errors) = await _adminService.CreateAdminAsync(model);
            if (success && admin != null)
            {
                return CreatedAtAction(nameof(GetAdmin), new { id = admin.Id }, admin);
            }

            return BadRequest(new { Errors = errors });
        }

        [HttpGet]
        [Authorize(Roles = "SuperAdmin")]
        public async Task<ActionResult<IEnumerable<AdminListDto>>> GetAllAdmins()
        {
            var (success, admins, errors) = await _adminService.GetAllAdminsAsync();
            if (success)
            {
                return Ok(admins);
            }

            return BadRequest(new { Errors = errors });
        }

        [HttpGet]
        [Authorize(Roles = "SuperAdmin")]
        public async Task<ActionResult<IEnumerable<RenterProfileDto>>> GetAllRenters()
        {
            var (success, users, errors) = await _adminService.GetAllRentersAsync();
            if (success)
            {
                return Ok(users);
            }
            return BadRequest(new { Errors = errors });

        }

        [HttpGet]
        [Authorize(Roles = "SuperAdmin")]
        public async Task<ActionResult<IEnumerable<ApartmentOwnerProfileDto>>> GetAllOwners()
        {
            var (success, users, errors) = await _adminService.GetAllOwnersAsync();
            if (success)
            {
                return Ok(users);
            }
            return BadRequest(new { Errors = errors });

        }

        [HttpGet("{id}")]
        [Authorize(Roles = "SuperAdmin")]
        public async Task<ActionResult<AdminDto>> GetAdmin(int id)
        {
            var (success, admin, errors) = await _adminService.GetAdminByIdAsync(id);
            if (success && admin != null)
            {
                return Ok(admin);
            }

            return NotFound(new { Errors = errors });
        }

        [HttpPut("{id}/permissions")]
        [Authorize(Roles = "SuperAdmin")]
        public async Task<IActionResult> UpdateAdminPermissions(int id, [FromBody] AdminPermissionDto permissions)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var updateDto = new UpdateAdminPermissionsDto
            {
                AdminId = id,
                Permissions = permissions
            };

            var (success, errors) = await _adminService.UpdateAdminPermissionsAsync(updateDto);
            if (success)
            {
                return NoContent();
            }

            return BadRequest(new { Errors = errors });
        }

        [HttpDelete("{id}")]
        [Authorize(Roles = "SuperAdmin")]
        public async Task<IActionResult> DeleteAdmin(int id)
        {
            var (success, errors) = await _adminService.DeleteAdminAsync(id);
            if (success)
            {
                return NoContent();
            }

            return NotFound(new { Errors = errors });
        }

        // Helper endpoint to check access to a specific location
        [HttpGet("check-location-access")]
        [Authorize(Roles = "Admin,SuperAdmin")]
        public async Task<ActionResult<bool>> CheckLocationAccess([FromQuery] string division, [FromQuery] string district, [FromQuery] string upazila, [FromQuery] string? ward = null, [FromQuery] string? village = null)
        {
            // Get current admin's ID from the token
            if (!User.Identity.IsAuthenticated)
            {
                return Unauthorized();
            }

            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
            if (userIdClaim == null || !int.TryParse(userIdClaim.Value, out int adminId))
            {
                return BadRequest(new { Error = "Invalid token" });
            }

            // SuperAdmin always has access
            if (User.IsInRole("SuperAdmin"))
            {
                return Ok(true);
            }

            var hasAccess = await _adminService.CanAdminAccessLocationAsync(adminId, division, district, upazila, ward, village);
            return Ok(hasAccess);
        }
    }
} 