using System.Security.Claims;
using BashindaAPI.Data;
using BashindaAPI.DTOs;
using BashindaAPI.Models;
using BashindaAPI.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace BashindaAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public class RenterProfilesController : BaseApiController
    {
        private readonly ApplicationDbContext _context;
        private readonly IWebHostEnvironment _env;
        private readonly ILogger<RenterProfilesController> _logger;
        private readonly IAdminService _adminService;

        public RenterProfilesController(
            ApplicationDbContext context,
            IWebHostEnvironment env,
            ILogger<RenterProfilesController> logger,
            IAdminService adminService)
        {
            _context = context;
            _env = env;
            _logger = logger;
            _adminService = adminService;
        }

        // GET: api/RenterProfiles
        [HttpGet]
        [Authorize(Roles = "Admin,ApartmentOwner,SuperAdmin")]
        public async Task<ActionResult<ApiResponse<IEnumerable<RenterProfileListDto>>>> GetRenterProfiles()
        {
            // Get current user's ID from the token
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
            if (userIdClaim == null || !int.TryParse(userIdClaim.Value, out int userId))
            {
                return Unauthorized(new ApiResponse<string>
                {
                    Success = false,
                    Errors = new[] { "Invalid token" }
                });
            }

            // Get user's role
            var roleClaim = User.FindFirst(ClaimTypes.Role);
            var role = roleClaim?.Value;
            bool isSuperAdmin = role == UserRole.SuperAdmin.ToString();
            bool isAdmin = role == UserRole.Admin.ToString();

            // If regular admin, get their permissions
            User? adminUser = null;
            AdminPermission? adminPermission = null;

            if (isAdmin)
            {
                adminUser = await _context.Users
                    .Include(u => u.AdminPermission)
                    .FirstOrDefaultAsync(u => u.Id == userId);

                adminPermission = adminUser?.AdminPermission;
                if (adminPermission == null)
                {
                    _logger.LogWarning("Admin user {AdminId} has no permissions configured", userId);
                    return BadRequest(new ApiResponse<string>
                    {
                        Success = false,
                        Errors = new[] { "Admin permissions not configured" }
                    });
                }
            }

            // Get profiles based on permissions
            IQueryable<RenterProfile> profilesQuery = _context.RenterProfiles;

            // Filter by location for admin users (if not SuperAdmin)
            if (isAdmin && !isSuperAdmin && adminPermission != null)
            {
                if (!string.IsNullOrEmpty(adminPermission.Division))
                {
                    profilesQuery = profilesQuery.Where(p => p.Division == adminPermission.Division);
                }

                if (!string.IsNullOrEmpty(adminPermission.District))
                {
                    profilesQuery = profilesQuery.Where(p => p.District == adminPermission.District);
                }

                if (!string.IsNullOrEmpty(adminPermission.Upazila))
                {
                    profilesQuery = profilesQuery.Where(p => p.Upazila == adminPermission.Upazila);
                }

                if (!string.IsNullOrEmpty(adminPermission.Ward))
                {
                    profilesQuery = profilesQuery.Where(p => p.Ward == adminPermission.Ward);
                }

                if (!string.IsNullOrEmpty(adminPermission.Village))
                {
                    profilesQuery = profilesQuery.Where(p => p.Village == adminPermission.Village);
                }
            }

            // Project to DTOs with field visibility based on permissions
            var profiles = await profilesQuery.Select(p => new RenterProfileListDto
            {
                Id = p.Id,
                FullName = p.FullName,
                MobileNo = isAdmin && adminPermission != null && !adminPermission.CanViewPhone ? "***Hidden***" : p.MobileNo,
                Email = isAdmin && adminPermission != null && !adminPermission.CanViewEmail ? "***Hidden***" : p.Email,
                NationalId = isAdmin && adminPermission != null && !adminPermission.CanViewNationalId ? null : p.NationalId,
                BirthRegistrationNo = isAdmin && adminPermission != null && !adminPermission.CanViewBirthRegistration ? null : p.BirthRegistrationNo,
                DateOfBirth = p.DateOfBirth,
                SelfImagePath = isAdmin && adminPermission != null && !adminPermission.CanViewProfileImage ? null : p.SelfImagePath,
                IsApproved = p.IsApproved,
                Division = p.Division,
                District = p.District,
                Upazila = p.Upazila
            }).ToListAsync();

            return Ok(ApiResponse<IEnumerable<RenterProfileListDto>>.SuccessResponse(profiles));
        }

        // GET: api/RenterProfiles/5
        [HttpGet("{id}")]
        [Authorize(Roles = "Admin,ApartmentOwner,ApartmentRenter,SuperAdmin")]
        public async Task<ActionResult<ApiResponse<RenterProfileDTO>>> GetRenterProfile(int id)
        {
            // Get current user's ID from the token
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
            if (userIdClaim == null || !int.TryParse(userIdClaim.Value, out int userId))
            {
                return Unauthorized(new ApiResponse<string>
                {
                    Success = false,
                    Errors = new[] { "Invalid token" }
                });
            }

            // Get user's role
            var roleClaim = User.FindFirst(ClaimTypes.Role);
            var role = roleClaim?.Value;
            bool isSuperAdmin = role == UserRole.SuperAdmin.ToString();
            bool isAdmin = role == UserRole.Admin.ToString();
            bool isCurrentUser = await _context.RenterProfiles
                .AnyAsync(p => p.Id == id && p.UserId == userId);

            // Find the profile
            var renterProfile = await _context.RenterProfiles
                .FirstOrDefaultAsync(p => p.Id == id);

            if (renterProfile == null)
            {
                return NotFound(new ApiResponse<string>
                {
                    Success = false,
                    Errors = new[] { "Renter profile not found" }
                });
            }

            // Check permissions for admin users
            if (isAdmin && !isSuperAdmin)
            {
                var adminUser = await _context.Users
                    .Include(u => u.AdminPermission)
                    .FirstOrDefaultAsync(u => u.Id == userId);

                var adminPermission = adminUser?.AdminPermission;
                if (adminPermission == null)
                {
                    return BadRequest(new ApiResponse<string>
                    {
                        Success = false,
                        Errors = new[] { "Admin permissions not configured" }
                    });
                }

                // Check location-based access
                bool hasLocationAccess = await _adminService.CanAdminAccessLocationAsync(
                    userId,
                    renterProfile.Division,
                    renterProfile.District,
                    renterProfile.Upazila,
                    renterProfile.Ward,
                    renterProfile.Village);

                if (!hasLocationAccess)
                {
                    return Forbid();
                }

                // Create DTO with field filtering based on permissions
                var profileDto = new RenterProfileDTO
                {
                    Id = renterProfile.Id,
                    UserId = renterProfile.UserId,
                    IsAdult = renterProfile.IsAdult,
                    NationalId = adminPermission.CanViewNationalId ? renterProfile.NationalId : null,
                    NationalIdImagePath = adminPermission.CanViewNationalId ? renterProfile.NationalIdImagePath : null,
                    BirthRegistrationNo = adminPermission.CanViewBirthRegistration ? renterProfile.BirthRegistrationNo : null,
                    BirthRegistrationImagePath = adminPermission.CanViewBirthRegistration ? renterProfile.BirthRegistrationImagePath : null,
                    DateOfBirth = adminPermission.CanViewDateOfBirth ? renterProfile.DateOfBirth : default,
                    FullName = adminPermission.CanViewUserName ? renterProfile.FullName : "***Hidden***",
                    FatherName = adminPermission.CanViewFamilyInfo ? renterProfile.FatherName : "***Hidden***",
                    MotherName = adminPermission.CanViewFamilyInfo ? renterProfile.MotherName : "***Hidden***",
                    Nationality = renterProfile.Nationality,
                    BloodGroup = renterProfile.BloodGroup,
                    Profession = adminPermission.CanViewProfession ? renterProfile.Profession : "***Hidden***",
                    Gender = renterProfile.Gender,
                    MobileNo = adminPermission.CanViewPhone ? renterProfile.MobileNo : "***Hidden***",
                    Email = adminPermission.CanViewEmail ? renterProfile.Email : "***Hidden***",
                    SelfImagePath = adminPermission.CanViewProfileImage ? renterProfile.SelfImagePath : null,
                    Division = adminPermission.CanViewAddress ? renterProfile.Division : "***Hidden***",
                    District = adminPermission.CanViewAddress ? renterProfile.District : "***Hidden***",
                    Upazila = adminPermission.CanViewAddress ? renterProfile.Upazila : "***Hidden***",
                    Ward = adminPermission.CanViewAddress ? renterProfile.Ward : "***Hidden***",
                    Village = adminPermission.CanViewAddress ? renterProfile.Village : "***Hidden***",
                    PostCode = adminPermission.CanViewAddress ? renterProfile.PostCode : "***Hidden***",
                    HoldingNo = adminPermission.CanViewAddress ? renterProfile.HoldingNo : "***Hidden***",
                    IsApproved = renterProfile.IsApproved
                };

                return Ok(ApiResponse<RenterProfileDTO>.SuccessResponse(profileDto));
            }

            // For SuperAdmin, own profile, or apartment owner - show full details
            var dto = new RenterProfileDTO
            {
                Id = renterProfile.Id,
                UserId = renterProfile.UserId,
                IsAdult = renterProfile.IsAdult,
                NationalId = renterProfile.NationalId,
                NationalIdImagePath = renterProfile.NationalIdImagePath,
                BirthRegistrationNo = renterProfile.BirthRegistrationNo,
                BirthRegistrationImagePath = renterProfile.BirthRegistrationImagePath,
                DateOfBirth = renterProfile.DateOfBirth,
                FullName = renterProfile.FullName,
                FatherName = renterProfile.FatherName,
                MotherName = renterProfile.MotherName,
                Nationality = renterProfile.Nationality,
                BloodGroup = renterProfile.BloodGroup,
                Profession = renterProfile.Profession,
                Gender = renterProfile.Gender,
                MobileNo = renterProfile.MobileNo,
                Email = renterProfile.Email,
                SelfImagePath = renterProfile.SelfImagePath,
                Division = renterProfile.Division,
                District = renterProfile.District,
                Upazila = renterProfile.Upazila,
                Ward = renterProfile.Ward,
                Village = renterProfile.Village,
                PostCode = renterProfile.PostCode,
                HoldingNo = renterProfile.HoldingNo,
                IsApproved = renterProfile.IsApproved
            };

            return Ok(ApiResponse<RenterProfileDTO>.SuccessResponse(dto));
        }

        // GET: api/RenterProfiles/current
        [HttpGet("current")]
        [Authorize(Roles = "ApartmentRenter")]
        public async Task<ActionResult<RenterProfileDTO>> GetCurrentRenterProfile()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userId))
            {
                return Unauthorized();
            }
            var user = await _context.Users.FirstOrDefaultAsync(u => u.Id.ToString() == userId);
            if (user == null)
            {
                return NotFound();
            }

            string uniqueId = user.UniqueID ?? string.Empty;

            var renterProfile = await _context.RenterProfiles
                .Include(r => r.User)
                .FirstOrDefaultAsync(r => r.UserId.ToString() == userId);

            if (renterProfile == null)
            {
                return NotFound();
            }

            var dto = new RenterProfileDTO
            {
                Id = renterProfile.Id,
                UserId = renterProfile.UserId,
                IsAdult = renterProfile.IsAdult,
                NationalId = renterProfile.NationalId,
                NationalIdImagePath = renterProfile.NationalIdImagePath,
                BirthRegistrationNo = renterProfile.BirthRegistrationNo,
                BirthRegistrationImagePath = renterProfile.BirthRegistrationImagePath,
                DateOfBirth = renterProfile.DateOfBirth,
                FullName = renterProfile.FullName,
                FatherName = renterProfile.FatherName,
                MotherName = renterProfile.MotherName,
                Nationality = renterProfile.Nationality.ToString(),
                BloodGroup = renterProfile.BloodGroup.ToString(),
                Profession = renterProfile.Profession.ToString(),
                Gender = renterProfile.Gender.ToString(),
                MobileNo = renterProfile.MobileNo,
                Email = renterProfile.Email,
                SelfImagePath = renterProfile.SelfImagePath,
                // Map location fields directly
                Division = renterProfile.Division,
                District = renterProfile.District,
                Upazila = renterProfile.Upazila,
                AreaType = renterProfile.AreaType,
                Ward = renterProfile.Ward,
                Village = renterProfile.Village,
                PostCode = renterProfile.PostCode,
                HoldingNo = renterProfile.HoldingNo,
                IsApproved = renterProfile.IsApproved,
                RejectionReason = renterProfile.RejectionReason,
                UniqueId = uniqueId,
                User = renterProfile.User != null ? new UserDto
                {
                    Id = renterProfile.User.Id,
                    UserName = renterProfile.User.UserName,
                    Email = renterProfile.User.Email,
                    PhoneNumber = renterProfile.User.PhoneNumber,
                    Role = renterProfile.User.Role.ToString(),
                    IsVerified = renterProfile.User.IsVerified
                } : null
            };

            return Ok(dto);
        }

        // GET: api/RenterProfiles/pending
        [HttpGet("pending")]
        [Authorize(Roles = "Admin,SuperAdmin")]
        public async Task<ActionResult<ApiResponse<IEnumerable<RenterProfileListDto>>>> GetPendingRenterProfiles()
        {
            // Get current user's ID from the token
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
            if (userIdClaim == null || !int.TryParse(userIdClaim.Value, out int userId))
            {
                return Unauthorized(new ApiResponse<string>
                {
                    Success = false,
                    Errors = new[] { "Invalid token" }
                });
            }

            // Get user's role
            var roleClaim = User.FindFirst(ClaimTypes.Role);
            var role = roleClaim?.Value;
            bool isSuperAdmin = role == UserRole.SuperAdmin.ToString();
            bool isAdmin = role == UserRole.Admin.ToString();

            // If regular admin, get their permissions
            User? adminUser = null;
            AdminPermission? adminPermission = null;

            if (isAdmin && !isSuperAdmin)
            {
                adminUser = await _context.Users
                    .Include(u => u.AdminPermission)
                    .FirstOrDefaultAsync(u => u.Id == userId);

                adminPermission = adminUser?.AdminPermission;
                if (adminPermission == null)
                {
                    _logger.LogWarning("Admin user {AdminId} has no permissions configured", userId);
                    return BadRequest(new ApiResponse<string>
                    {
                        Success = false,
                        Errors = new[] { "Admin permissions not configured" }
                    });
                }

                // Check if admin has permission to approve renters
                if (!adminPermission.CanApproveRenters)
                {
                    return Forbid();
                }
            }

            // Get profiles based on permissions
            IQueryable<RenterProfile> profilesQuery = _context.RenterProfiles
                .Where(p => !p.IsApproved);

            // Filter by location for admin users (if not SuperAdmin)
            if (isAdmin && !isSuperAdmin && adminPermission != null)
            {
                if (!string.IsNullOrEmpty(adminPermission.Division))
                {
                    profilesQuery = profilesQuery.Where(p => p.Division == adminPermission.Division);
                }

                if (!string.IsNullOrEmpty(adminPermission.District))
                {
                    profilesQuery = profilesQuery.Where(p => p.District == adminPermission.District);
                }

                if (!string.IsNullOrEmpty(adminPermission.Upazila))
                {
                    profilesQuery = profilesQuery.Where(p => p.Upazila == adminPermission.Upazila);
                }

                if (!string.IsNullOrEmpty(adminPermission.Ward))
                {
                    profilesQuery = profilesQuery.Where(p => p.Ward == adminPermission.Ward);
                }

                if (!string.IsNullOrEmpty(adminPermission.Village))
                {
                    profilesQuery = profilesQuery.Where(p => p.Village == adminPermission.Village);
                }
            }

            var profiles = await profilesQuery
                .Select(p => new RenterProfileListDto
                {
                    Id = p.Id,
                    FullName = p.FullName,
                    MobileNo = isAdmin && adminPermission != null && !adminPermission.CanViewPhone ? "***Hidden***" : p.MobileNo,
                    Email = isAdmin && adminPermission != null && !adminPermission.CanViewEmail ? "***Hidden***" : p.Email,
                    NationalId = isAdmin && adminPermission != null && !adminPermission.CanViewNationalId ? null : p.NationalId,
                    BirthRegistrationNo = isAdmin && adminPermission != null && !adminPermission.CanViewBirthRegistration ? null : p.BirthRegistrationNo,
                    DateOfBirth = p.DateOfBirth,
                    SelfImagePath = isAdmin && adminPermission != null && !adminPermission.CanViewProfileImage ? null : p.SelfImagePath,
                    IsApproved = p.IsApproved,
                    Division = p.Division,
                    District = p.District,
                    Upazila = p.Upazila
                })
                .ToListAsync();

            return Ok(ApiResponse<IEnumerable<RenterProfileListDto>>.SuccessResponse(profiles));
        }

        // POST: api/RenterProfiles
        [HttpPost]
        [Authorize(Roles = "ApartmentRenter")]
        public async Task<ActionResult<RenterProfileDTO>> CreateRenterProfile(CreateRenterProfileDto dto)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userId))
            {
                return Unauthorized();
            }

            if (await _context.RenterProfiles.AnyAsync(r => r.UserId.ToString() == userId))
            {
                return Conflict("You already have a profile");
            }

            // No need to validate locations anymore as we're using strings

            var renterProfile = new RenterProfile
            {
                UserId = int.Parse(userId),
                IsAdult = dto.IsAdult,
                NationalId = dto.IsAdult ? dto.NationalId : null,
                BirthRegistrationNo = !dto.IsAdult ? dto.BirthRegistrationNo : null,
                DateOfBirth = dto.DateOfBirth,
                FullName = dto.FullName,
                FatherName = dto.FatherName,
                MotherName = dto.MotherName,
                Nationality = dto.Nationality,
                BloodGroup = dto.BloodGroup,
                Profession = dto.Profession,
                Gender = dto.Gender,
                MobileNo = dto.MobileNo,
                Email = dto.Email,
                Division = dto.Division,
                District = dto.District,
                Upazila = dto.Upazila,
                AreaType = dto.AreaType,
                Ward = dto.Ward,
                Village = dto.Village,
                PostCode = dto.PostCode,
                HoldingNo = dto.HoldingNo,
                IsApproved = false // Requires admin approval
            };

            _context.RenterProfiles.Add(renterProfile);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetRenterProfile), new { id = renterProfile.Id }, new RenterProfileDTO
            {
                Id = renterProfile.Id,
                UserId = renterProfile.UserId,
                IsAdult = renterProfile.IsAdult,
                NationalId = renterProfile.NationalId,
                DateOfBirth = renterProfile.DateOfBirth,
                FullName = renterProfile.FullName,
                FatherName = renterProfile.FatherName,
                MotherName = renterProfile.MotherName,
                Nationality = renterProfile.Nationality.ToString(),
                BloodGroup = renterProfile.BloodGroup.ToString(),
                Profession = renterProfile.Profession.ToString(),
                Gender = renterProfile.Gender.ToString(),
                MobileNo = renterProfile.MobileNo,
                Email = renterProfile.Email,
                // Use the location fields directly instead of an Address object
                Division = renterProfile.Division,
                District = renterProfile.District,
                Upazila = renterProfile.Upazila,
                AreaType = renterProfile.AreaType,
                Ward = renterProfile.Ward,
                Village = renterProfile.Village,
                PostCode = renterProfile.PostCode,
                HoldingNo = renterProfile.HoldingNo,
                IsApproved = renterProfile.IsApproved
            });
        }

        // PUT: api/RenterProfiles/5
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateRenterProfile(int id, UpdateRenterProfileDto dto)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var userRole = User.FindFirstValue(ClaimTypes.Role);

            var renterProfile = await _context.RenterProfiles.FindAsync(id);
            if (renterProfile == null)
            {
                return NotFound();
            }

            // Security check: Users can only update their own profile unless they are Admin
            if (userRole != "Admin" && renterProfile.UserId.ToString() != userId)
            {
                return Forbid();
            }

            try
            {
                // Update fields
                renterProfile.IsAdult = dto.IsAdult;
                renterProfile.NationalId = dto.IsAdult ? dto.NationalId : null;
                renterProfile.BirthRegistrationNo = !dto.IsAdult ? dto.BirthRegistrationNo : null;
                renterProfile.DateOfBirth = dto.DateOfBirth;
                renterProfile.FullName = dto.FullName;
                renterProfile.FatherName = dto.FatherName;
                renterProfile.MotherName = dto.MotherName;
                renterProfile.Nationality = dto.Nationality;
                renterProfile.BloodGroup = dto.BloodGroup;
                renterProfile.Profession = dto.Profession;
                renterProfile.Gender = dto.Gender;
                renterProfile.MobileNo = dto.MobileNo;
                renterProfile.Email = dto.Email;
                renterProfile.Division = dto.Division;
                renterProfile.District = dto.District;
                renterProfile.Upazila = dto.Upazila;
                renterProfile.AreaType = dto.AreaType;
                renterProfile.Ward = dto.Ward;
                renterProfile.Village = dto.Village;
                renterProfile.PostCode = dto.PostCode;
                renterProfile.HoldingNo = dto.HoldingNo;

                _context.Entry(renterProfile).State = EntityState.Modified;

                // Don't allow non-admins to change approval status
                if (userRole != "Admin")
                {
                    _context.Entry(renterProfile).Property(x => x.IsApproved).IsModified = false;
                }

                await _context.SaveChangesAsync();

                return Ok(ApiResponse<object>.SuccessResponse(null, "Profile updated successfully."));
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!RenterProfileExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating renter profile {ProfileId}", id);
                return StatusCode(500, ApiResponse<object>.ErrorResponse("An error occurred while updating the profile."));
            }
        }

        // PATCH: api/RenterProfiles/5/approve
        [HttpPatch("{id}/approve")]
        [Authorize(Roles = "Admin,SuperAdmin")]
        public async Task<IActionResult> ApproveRenterProfile(int id, ApproveProfileDto dto)
        {
            // Get current user's ID from the token
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
            if (userIdClaim == null || !int.TryParse(userIdClaim.Value, out int userId))
            {
                return Unauthorized(new ApiResponse<string>
                {
                    Success = false,
                    Errors = new[] { "Invalid token" }
                });
            }

            // Get user's role
            var roleClaim = User.FindFirst(ClaimTypes.Role);
            var role = roleClaim?.Value;
            bool isSuperAdmin = role == UserRole.SuperAdmin.ToString();
            bool isAdmin = role == UserRole.Admin.ToString();

            var renterProfile = await _context.RenterProfiles.FindAsync(id);
            if (renterProfile == null)
            {
                return NotFound(new ApiResponse<string>
                {
                    Success = false,
                    Errors = new[] { "Renter profile not found" }
                });
            }

            // Check admin's permission to approve renter profiles
            if (isAdmin && !isSuperAdmin)
            {
                var adminUser = await _context.Users
                    .Include(u => u.AdminPermission)
                    .FirstOrDefaultAsync(u => u.Id == userId);

                var adminPermission = adminUser?.AdminPermission;
                if (adminPermission == null)
                {
                    return BadRequest(new ApiResponse<string>
                    {
                        Success = false,
                        Errors = new[] { "Admin permissions not configured" }
                    });
                }

                // Check if admin has permission to approve renters
                if (!adminPermission.CanApproveRenters)
                {
                    return Forbid();
                }

                // Check location-based access
                bool hasLocationAccess = await _adminService.CanAdminAccessLocationAsync(
                    userId,
                    renterProfile.Division,
                    renterProfile.District,
                    renterProfile.Upazila,
                    renterProfile.Ward,
                    renterProfile.Village);

                if (!hasLocationAccess)
                {
                    return Forbid();
                }
            }

            renterProfile.IsApproved = dto.IsApproved;
            renterProfile.RejectionReason = !dto.IsApproved ? dto.Reason : null;

            try
            {
                await _context.SaveChangesAsync();
                return NoContent();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!RenterProfileExists(id))
                {
                    return NotFound();
                }
                throw;
            }
        }

        // POST: api/RenterProfiles/5/upload-image
        [HttpPost("{id}/upload-image")]
        public async Task<ActionResult<string>> UploadImage(int id, [FromForm] UploadRenterImageDto dto)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var userRole = User.FindFirstValue(ClaimTypes.Role);

            var renterProfile = await _context.RenterProfiles.FindAsync(id);
            if (renterProfile == null)
            {
                return NotFound();
            }

            // Security check: Users can only upload to their own profile unless they are Admin
            if (userRole != "Admin" && renterProfile.UserId.ToString() != userId)
            {
                return Forbid();
            }

            if (dto.Image == null || dto.Image.Length == 0)
            {
                return BadRequest("No file uploaded");
            }

            // Validate file type
            var allowedExtensions = new[] { ".jpg", ".jpeg", ".png" };
            var fileExtension = Path.GetExtension(dto.Image.FileName).ToLowerInvariant();
            if (!allowedExtensions.Contains(fileExtension))
            {
                return BadRequest("Invalid file type. Only JPG, JPEG, and PNG are allowed.");
            }

            // Validate file size (5MB)
            if (dto.Image.Length > 5 * 1024 * 1024)
            {
                return BadRequest("File size exceeds 5MB");
            }

            try
            {
                var uploadsFolder = Path.Combine(_env.WebRootPath, "uploads");
                if (!Directory.Exists(uploadsFolder))
                {
                    Directory.CreateDirectory(uploadsFolder);
                }

                // Generate a unique filename
                var uniqueFileName = Guid.NewGuid().ToString() + "_" + Path.GetFileName(dto.Image.FileName);
                var filePath = Path.Combine(uploadsFolder, uniqueFileName);

                // Save the file
                using (var fileStream = new FileStream(filePath, FileMode.Create))
                {
                    await dto.Image.CopyToAsync(fileStream);
                }

                // Update the appropriate path based on image type
                var relativePath = Path.Combine("uploads", uniqueFileName).Replace("\\", "/");
                switch (dto.ImageType.ToLower())
                {
                    case "nationalid":
                        renterProfile.NationalIdImagePath = relativePath;
                        break;
                    case "birthregistration":
                        renterProfile.BirthRegistrationImagePath = relativePath;
                        break;
                    default:
                        renterProfile.SelfImagePath = relativePath;
                        break;
                }

                await _context.SaveChangesAsync();

                return Ok(new { path = relativePath });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error uploading image: {ErrorMessage}", ex.Message);
                return StatusCode(500, "An error occurred while uploading the image");
            }
        }

        private bool RenterProfileExists(int id)
        {
            return _context.RenterProfiles.Any(e => e.Id == id);
        }
    }
}
