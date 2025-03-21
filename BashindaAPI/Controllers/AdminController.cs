using BashindaAPI.Data;
using BashindaAPI.DTOs;
using BashindaAPI.Models;
using BashindaAPI.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.Json;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query;
using System.Security.Claims;

namespace BashindaAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "Admin")]
    public class AdminController : BaseApiController
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<AdminController> _logger;
        private readonly IAdminService _adminService;

        public AdminController(ApplicationDbContext context, ILogger<AdminController> logger, IAdminService adminService)
        {
            _context = context;
            _logger = logger;
            _adminService = adminService;
        }
        


        // GET: api/Admin/dashboard
        [HttpGet("dashboard")]
        public async Task<ActionResult<ApiResponse<AdminDashboardDto>>> GetDashboard(
                [FromQuery] string? division,  
                [FromQuery] string? district,
                [FromQuery] string? upazila,
                [FromQuery] string? ward,
                [FromQuery] string? village)
        {
            try
            {
                // Renter profiles
                var renterBaseQuery = _context.RenterProfiles.Where(p => !p.IsApproved);
                var filteredRenters = ApplyRenterLocationFilters(renterBaseQuery, division ?? string.Empty, district ?? string.Empty, upazila ?? string.Empty, ward ?? string.Empty, village ?? string.Empty);
                var pendingRenters = await filteredRenters.CountAsync();

                // Owner profiles
                var ownerBaseQuery = _context.ApartmentOwnerProfiles.Where(p => !p.IsApproved);
                var filteredOwners = ApplyOwnerLocationFilters(ownerBaseQuery, division ?? string.Empty, district ?? string.Empty, upazila ?? string.Empty, ward ?? string.Empty, village ?? string.Empty);
                var pendingOwners = await filteredOwners.CountAsync();

                // Total counts
                var totalUsers = await _context.Users.CountAsync();
                var approvedRenters = await _context.RenterProfiles.CountAsync(p => p.IsApproved);
                var approvedOwners = await _context.ApartmentOwnerProfiles.CountAsync(p => p.IsApproved);
                var totalApartments = await _context.Apartments.CountAsync();

                var dashboard = new AdminDashboardDto
                {
                    PendingRenters = pendingRenters,
                    PendingOwners = pendingOwners,
                    TotalUsers = totalUsers,
                    ApprovedRenters = approvedRenters,
                    TotalRenters = approvedRenters + pendingRenters,
                    ApprovedOwners = approvedOwners,
                    TotalOwners = approvedOwners + pendingOwners,
                    TotalApartments = totalApartments,
                    RenterApprovalPercentage = approvedRenters + pendingRenters > 0 ?
                        (int)((double)approvedRenters / (approvedRenters + pendingRenters) * 100) : 0,
                    OwnerApprovalPercentage = approvedOwners + pendingOwners > 0 ?
                        (int)((double)approvedOwners / (approvedOwners + pendingOwners) * 100) : 0
                };

                return OkWithResponse(dashboard);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting admin dashboard");
                return ServerErrorWithResponse<AdminDashboardDto>("An error occurred while retrieving dashboard data");
            }
        }


        // GET: api/Admin/users
        [HttpGet("users")]
        public async Task<ActionResult<ApiResponse<List<UserDto>>>> GetUsers()
        {
            try
            {
                var users = await _context.Users
                    .Select(u => new UserDto
                    {
                        Id = u.Id, // Change this line to use the correct type
                        UserName = u.UserName,
                        Email = u.Email,
                        PhoneNumber = u.PhoneNumber,
                        Role = u.Role.ToString(),
                        IsVerified = u.IsVerified
                    })
                    .ToListAsync();

                return OkWithResponse(users);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving users");
                return ServerErrorWithResponse<List<UserDto>>("An error occurred while retrieving users");
            }
        }

        // GET: api/Admin/renter-profiles/pending
        [HttpGet("renter-profiles/pending")]
        public async Task<ActionResult<ApiResponse<List<RenterProfileListDto>>>> GetPendingRenterProfiles(
    [FromQuery] string? division,
    [FromQuery] string? district,
    [FromQuery] string? upazila,
    [FromQuery] string? ward,
    [FromQuery] string? village)
        {
            try
            {
                var baseQuery = _context.RenterProfiles
                    .Where(p => !p.IsApproved)
                    .Include(p => p.User);

                var filteredQuery = ApplyRenterLocationFilters(baseQuery,
                    division ?? string.Empty,
                    district ?? string.Empty,
                    upazila ?? string.Empty,
                    ward ?? string.Empty,
                    village ?? string.Empty);

                var profiles = await filteredQuery
                    .OrderByDescending(p => p.Id)
                    .Select(p => new RenterProfileListDto
                    {
                        Id = p.Id,
                        FullName = p.FullName,
                        MobileNo = p.MobileNo,
                        Email = p.Email,
                        NationalId = p.NationalId,
                        BirthRegistrationNo = p.BirthRegistrationNo,
                        DateOfBirth = p.DateOfBirth,
                        SelfImagePath = p.SelfImagePath,
                        IsApproved = p.IsApproved
                    })
                    .ToListAsync();

                return OkWithResponse(profiles);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving pending renter profiles");
                return ServerErrorWithResponse<List<RenterProfileListDto>>("Error retrieving profiles");
            }
        }

        // GET: api/Admin/owner-profiles/pending
        //[HttpGet("owner-profiles/pending")]
        //public async Task<ActionResult<ApiResponse<List<ApartmentOwnerProfileListDto>>>> GetPendingOwnerProfiles()
        //{
        //    try
        //    {

        //        var profiles = await _context.ApartmentOwnerProfiles
        //            .Where(p => !p.IsApproved)
        //            .Include(p => p.User)
        //            .OrderByDescending(p => p.Id)
        //            .Select(p => new ApartmentOwnerProfileListDto
        //            {
        //                Id = p.Id,
        //                FullName = p.FullName,
        //                MobileNo = p.MobileNo,
        //                Email = p.Email,
        //                NationalId = p.NationalId,
        //                SelfImagePath = p.SelfImagePath,
        //                IsApproved = p.IsApproved
        //            })
        //            .ToListAsync();

        //        return OkWithResponse(profiles);
        //    }
        //    catch (Exception ex)
        //    {
        //        _logger.LogError(ex, "Error retrieving pending owner profiles");
        //        return ServerErrorWithResponse<List<ApartmentOwnerProfileListDto>>("An error occurred while retrieving pending owner profiles");
        //    }
        //}
        // GET: api/Admin/owner-profiles/pending
        [HttpGet("owner-profiles/pending")]
        public async Task<ActionResult<ApiResponse<List<ApartmentOwnerProfileListDto>>>> GetPendingOwnerProfiles(
    [FromQuery] string division,
    [FromQuery] string district,
    [FromQuery] string upazila,
    [FromQuery] string ward,
    [FromQuery] string village)
        {
            try
            {
                // Start with base query including User
                var baseQuery = _context.ApartmentOwnerProfiles
                    .Where(p => !p.IsApproved)
                    .Include(p => p.User);

                // Apply filters while preserving Include relationship
                var filteredQuery = ApplyOwnerLocationFilters(baseQuery, division, district, upazila, ward, village);

                var profiles = await filteredQuery
                    .OrderByDescending(p => p.Id)
                    .Select(p => new ApartmentOwnerProfileListDto
                    {
                        Id = p.Id,
                        FullName = p.FullName,
                        MobileNo = p.MobileNo,
                        Email = p.Email,
                        NationalId = p.NationalId,
                        SelfImagePath = p.SelfImagePath,
                        IsApproved = p.IsApproved
                    })
                    .ToListAsync();

                return OkWithResponse(profiles);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving pending owner profiles");
                return ServerErrorWithResponse<List<ApartmentOwnerProfileListDto>>(
                    "An error occurred while retrieving pending owner profiles");
            }
        }

        // PUT: api/Admin/renter-profiles/{id}/approve
        //[HttpPut("renter-profiles/{id}/approve")]
        //public async Task<ActionResult<ApiResponse<bool>>> ApproveRenterProfile(int id, [FromBody] ApproveProfileDto dto)
        //{
        //    try
        //    {
        //        var profile = await _context.RenterProfiles.FindAsync(id);

        //        if (profile == null)
        //        {
        //            return NotFoundWithResponse<bool>($"Renter profile with ID {id} not found");
        //        }

        //        profile.IsApproved = dto.IsApproved;

        //        if (!dto.IsApproved && !string.IsNullOrEmpty(dto.Reason))
        //        {
        //            // Store rejection reason if provided
        //            profile.RejectionReason = dto.Reason;
        //        }

        //        await _context.SaveChangesAsync();

        //        return OkWithResponse(true);
        //    }
        //    catch (Exception ex)
        //    {
        //        _logger.LogError(ex, "Error approving/rejecting renter profile");
        //        return ServerErrorWithResponse<bool>("An error occurred while processing the request");
        //    }
        //}
        [HttpPut("renter-profiles/{id}/approve")]
        public async Task<ActionResult<ApiResponse<bool>>> ApproveRenterProfile(int id, [FromBody] ApproveProfileDto dto)
        {
            try
            {
                var adminId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));

                if (!await _adminService.CanAdminAccessUserAsync(adminId, id))
                {
                    return StatusCode(StatusCodes.Status403Forbidden,
                        new ApiResponse<bool>
                        {
                            Success = false,
                            Errors = new[] { "Not authorized to approve this profile" }
                        });
                }

                var profile = await _context.RenterProfiles
                    .Include(p => p.User)
                    .FirstOrDefaultAsync(p => p.Id == id);

                if (profile == null)
                {
                    return NotFoundWithResponse<bool>("Renter profile not found");
                }

                profile.IsApproved = dto.IsApproved;
                profile.RejectionReason = dto.IsApproved ? null : dto.Reason;

                await _context.SaveChangesAsync();

                return OkWithResponse(true);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error approving renter profile ID: {ProfileId}", id);
                return ServerErrorWithResponse<bool>("Failed to process approval");
            }
        }

        // PUT: api/Admin/owner-profiles/{id}/approve
        //[HttpPut("owner-profiles/{id}/approve")]
        //public async Task<ActionResult<ApiResponse<bool>>> ApproveOwnerProfile(int id, [FromBody] ApproveProfileDto dto)
        //{
        //    try
        //    {
        //        var profile = await _context.ApartmentOwnerProfiles.FindAsync(id);

        //        if (profile == null)
        //        {
        //            return NotFoundWithResponse<bool>($"Owner profile with ID {id} not found");
        //        }

        //        profile.IsApproved = dto.IsApproved;

        //        if (!dto.IsApproved && !string.IsNullOrEmpty(dto.Reason))
        //        {
        //            // Store rejection reason if provided
        //            profile.RejectionReason = dto.Reason;
        //        }

        //        await _context.SaveChangesAsync();

        //        return OkWithResponse(true);
        //    }
        //    catch (Exception ex)
        //    {
        //        _logger.LogError(ex, "Error approving/rejecting owner profile");
        //        return ServerErrorWithResponse<bool>("An error occurred while processing the request");
        //    }
        //}
        [HttpPut("owner-profiles/{id}/approve")]
        public async Task<ActionResult<ApiResponse<bool>>> ApproveOwnerProfile(int id, [FromBody] ApproveProfileDto dto)
        {
            try
            {
                var adminId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));

                if (!await _adminService.CanAdminAccessUserAsync(adminId, id))
                {
                    return StatusCode(StatusCodes.Status403Forbidden,
                        new ApiResponse<bool>
                        {
                            Success = false,
                            Errors = new[] { "Not authorized for this profile" }
                        });
                }

                var profile = await _context.ApartmentOwnerProfiles
                    .Include(p => p.User)
                    .FirstOrDefaultAsync(p => p.Id == id);

                if (profile == null)
                {
                    return NotFoundWithResponse<bool>("Owner profile not found");
                }

                profile.IsApproved = dto.IsApproved;
                profile.RejectionReason = dto.IsApproved ? null : dto.Reason;

                await _context.SaveChangesAsync();

                return OkWithResponse(true);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error approving owner profile ID: {ProfileId}", id);
                return ServerErrorWithResponse<bool>("Failed to process approval");
            }
        }

        // DELETE: api/Admin/users/{id}
        [HttpDelete("users/{id}")]
        public async Task<ActionResult<ApiResponse<bool>>> DeleteUser(int id)
        {
            try
            {
                var user = await _context.Users.FindAsync(id);
                
                if (user == null)
                {
                    return NotFoundWithResponse<bool>($"User with ID {id} not found");
                }
                
                // Check if this user has any related data
                var hasRenterProfile = await _context.RenterProfiles.AnyAsync(p => p.UserId == id);
                var hasOwnerProfile = await _context.ApartmentOwnerProfiles.AnyAsync(p => p.UserId == id);
                
                if (hasRenterProfile || hasOwnerProfile)
                {
                    return BadRequestWithResponse<bool>("Cannot delete user with existing profiles. Delete the profiles first.");
                }
                
                _context.Users.Remove(user);
                await _context.SaveChangesAsync();
                
                return OkWithResponse(true);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting user");
                return ServerErrorWithResponse<bool>("An error occurred while deleting the user");
            }
        }

        // In AdminController.cs
        private IQueryable<RenterProfile> ApplyRenterLocationFilters(
            IQueryable<RenterProfile> query,
            string? division,
            string? district,
            string? upazila,
            string? ward,
            string? village)
        {
            if (!string.IsNullOrEmpty(division))
            {
                query = query.Where(p => p.Division == division.Trim());

                if (!string.IsNullOrEmpty(district))
                {
                    query = query.Where(p => p.District == district.Trim());

                    if (!string.IsNullOrEmpty(upazila))
                    {
                        query = query.Where(p => p.Upazila == upazila.Trim());

                        if (!string.IsNullOrEmpty(ward))
                        {
                            query = query.Where(p => p.Ward == ward.Trim());

                            if (!string.IsNullOrEmpty(village))
                            {
                                query = query.Where(p => p.Village == village.Trim());
                            }
                        }
                    }
                }
            }
            return query;
        }
        private IQueryable<ApartmentOwnerProfile> ApplyOwnerLocationFilters(
            IQueryable<ApartmentOwnerProfile> query,
            string division, string district,
            string upazila, string ward, string village)
        {
            if (!string.IsNullOrEmpty(division))
            {
                query = query.Where(p => p.Division == division);

                if (!string.IsNullOrEmpty(district))
                {
                    query = query.Where(p => p.District == district);

                    if (!string.IsNullOrEmpty(upazila))
                    {
                        query = query.Where(p => p.Upazila == upazila);

                        if (!string.IsNullOrEmpty(ward))
                        {
                            query = query.Where(p => p.Ward == ward);

                            if (!string.IsNullOrEmpty(village))
                            {
                                query = query.Where(p => p.Village == village);
                            }
                        }
                    }
                }
            }
            return query;
        }

        // Add hierarchical filtering method for RenterProfiles
        private IQueryable<ApartmentOwnerProfile> ApplyLocationFilters(
    IQueryable<ApartmentOwnerProfile> query,
    string division, string district,
    string upazila, string ward, string village)
        {
            if (!string.IsNullOrEmpty(division))
            {
                query = query.Where(p => p.Division == division);

                if (!string.IsNullOrEmpty(district))
                {
                    query = query.Where(p => p.District == district);

                    if (!string.IsNullOrEmpty(upazila))
                    {
                        query = query.Where(p => p.Upazila == upazila);

                        if (!string.IsNullOrEmpty(ward))
                        {
                            query = query.Where(p => p.Ward == ward);

                            if (!string.IsNullOrEmpty(village))
                            {
                                query = query.Where(p => p.Village == village);
                            }
                        }
                    }
                }
            }
            return query;
        }
        [HttpGet("permissions/{adminId}")]
        public async Task<ActionResult<ApiResponse<AdminPermissionDto>>> GetAdminPermissions(int adminId)
        {
            try
            {
                var permissions = await _context.AdminPermissions
                    .FirstOrDefaultAsync(a => a.UserId == adminId);

                if (permissions == null)
                {
                    return NotFoundWithResponse<AdminPermissionDto>("Admin permissions not found");
                }

                var dto = new AdminPermissionDto
                {
                    Division = permissions.Division,
                    District = permissions.District,
                    Upazila = permissions.Upazila,
                    Ward = permissions.Ward,
                    Village = permissions.Village,
                    CanApproveRenters = permissions.CanApproveRenters,
                    CanApproveOwners = permissions.CanApproveOwners,
                    CanManageApartments = permissions.CanManageApartments
                };

                return OkWithResponse(dto);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving permissions for admin {AdminId}", adminId);
                return ServerErrorWithResponse<AdminPermissionDto>("Error retrieving permissions");
            }
        }
        // Helper method to get authentication token
        private string GetAuthToken()
        {
            // Implement your token retrieval logic here
            return HttpContext.Session.GetString("AuthToken")
                ?? User.FindFirst("Token")?.Value;
        }
    }
} 