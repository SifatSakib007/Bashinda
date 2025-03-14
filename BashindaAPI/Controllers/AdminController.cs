using System.Security.Claims;
using BashindaAPI.Data;
using BashindaAPI.DTOs;
using BashindaAPI.Models;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace BashindaAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "Admin")]
    public class AdminController : BaseApiController
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<AdminController> _logger;

        public AdminController(ApplicationDbContext context, ILogger<AdminController> logger)
        {
            _context = context;
            _logger = logger;
        }

        // GET: api/Admin/dashboard
        [HttpGet("dashboard")]
        public async Task<ActionResult<ApiResponse<AdminDashboardDto>>> GetDashboard()
        {
            try
            {
                // Count pending renter profiles
                var pendingRenters = await _context.RenterProfiles
                    .Where(p => !p.IsApproved)
                    .CountAsync();
                
                // Count pending owner profiles
                var pendingOwners = await _context.ApartmentOwnerProfiles
                    .Where(p => !p.IsApproved)
                    .CountAsync();
                
                // Count total users
                var totalUsers = await _context.Users.CountAsync();
                
                // Count approved and total profiles
                var approvedRenters = await _context.RenterProfiles
                    .Where(p => p.IsApproved)
                    .CountAsync();
                var totalRenters = approvedRenters + pendingRenters;
                
                var approvedOwners = await _context.ApartmentOwnerProfiles
                    .Where(p => p.IsApproved)
                    .CountAsync();
                var totalOwners = approvedOwners + pendingOwners;
                
                // Count apartments
                var totalApartments = await _context.Apartments.CountAsync();
                
                var dashboard = new AdminDashboardDto
                {
                    PendingRenters = pendingRenters,
                    PendingOwners = pendingOwners,
                    TotalUsers = totalUsers,
                    ApprovedRenters = approvedRenters,
                    TotalRenters = totalRenters,
                    ApprovedOwners = approvedOwners,
                    TotalOwners = totalOwners,
                    TotalApartments = totalApartments,
                    RenterApprovalPercentage = totalRenters > 0 ? (int)((double)approvedRenters / totalRenters * 100) : 0,
                    OwnerApprovalPercentage = totalOwners > 0 ? (int)((double)approvedOwners / totalOwners * 100) : 0
                };
                
                return OkWithResponse(dashboard);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting admin dashboard");
                return ServerErrorWithResponse<AdminDashboardDto>("An error occurred while retrieving the dashboard information");
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
                        Id = u.Id,
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
        public async Task<ActionResult<ApiResponse<List<RenterProfileListDto>>>> GetPendingRenterProfiles()
        {
            try
            {
                var profiles = await _context.RenterProfiles
                    .Where(p => !p.IsApproved)
                    .Include(p => p.User)
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
                return ServerErrorWithResponse<List<RenterProfileListDto>>("An error occurred while retrieving pending renter profiles");
            }
        }

        // GET: api/Admin/owner-profiles/pending
        [HttpGet("owner-profiles/pending")]
        public async Task<ActionResult<ApiResponse<List<ApartmentOwnerProfileListDto>>>> GetPendingOwnerProfiles()
        {
            try
            {
                var profiles = await _context.ApartmentOwnerProfiles
                    .Where(p => !p.IsApproved)
                    .Include(p => p.User)
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
                return ServerErrorWithResponse<List<ApartmentOwnerProfileListDto>>("An error occurred while retrieving pending owner profiles");
            }
        }

        // PUT: api/Admin/renter-profiles/{id}/approve
        [HttpPut("renter-profiles/{id}/approve")]
        public async Task<ActionResult<ApiResponse<bool>>> ApproveRenterProfile(int id, [FromBody] ApproveProfileDto dto)
        {
            try
            {
                var profile = await _context.RenterProfiles.FindAsync(id);
                
                if (profile == null)
                {
                    return NotFoundWithResponse<bool>($"Renter profile with ID {id} not found");
                }
                
                profile.IsApproved = dto.IsApproved;
                
                if (!dto.IsApproved && !string.IsNullOrEmpty(dto.Reason))
                {
                    // Store rejection reason if provided
                    profile.RejectionReason = dto.Reason;
                }
                
                await _context.SaveChangesAsync();
                
                return OkWithResponse(true);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error approving/rejecting renter profile");
                return ServerErrorWithResponse<bool>("An error occurred while processing the request");
            }
        }
        
        // PUT: api/Admin/owner-profiles/{id}/approve
        [HttpPut("owner-profiles/{id}/approve")]
        public async Task<ActionResult<ApiResponse<bool>>> ApproveOwnerProfile(int id, [FromBody] ApproveProfileDto dto)
        {
            try
            {
                var profile = await _context.ApartmentOwnerProfiles.FindAsync(id);
                
                if (profile == null)
                {
                    return NotFoundWithResponse<bool>($"Owner profile with ID {id} not found");
                }
                
                profile.IsApproved = dto.IsApproved;
                
                if (!dto.IsApproved && !string.IsNullOrEmpty(dto.Reason))
                {
                    // Store rejection reason if provided
                    profile.RejectionReason = dto.Reason;
                }
                
                await _context.SaveChangesAsync();
                
                return OkWithResponse(true);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error approving/rejecting owner profile");
                return ServerErrorWithResponse<bool>("An error occurred while processing the request");
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
    }
} 