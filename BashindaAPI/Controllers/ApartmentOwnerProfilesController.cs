using BashindaAPI.Data;
using BashindaAPI.DTOs;
using BashindaAPI.Models;
using BashindaAPI.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace BashindaAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public class ApartmentOwnerProfilesController : BaseApiController
    {
        private readonly IAdminService _adminService;
        private readonly ApplicationDbContext _context;
        private readonly IWebHostEnvironment _env;
        private readonly ILogger<ApartmentOwnerProfilesController> _logger;

        public ApartmentOwnerProfilesController(
            IAdminService adminService,
            ApplicationDbContext context,
            IWebHostEnvironment env,
            ILogger<ApartmentOwnerProfilesController> logger)
        {
            _adminService = adminService;
            _context = context;
            _env = env;
            _logger = logger;
        }

        [HttpGet("owners/{userId}")]
        public async Task<IActionResult> GetOwnerByUserId(string userId)
        {
            var (success, profile, errors) = await _adminService.GetOwnerByUserIdAsync(userId);
            if (success)
            {
                return Ok(profile);
            }
            return NotFound(new { Errors = errors });
        }

        // GET: api/ApartmentOwnerProfiles
        [HttpGet]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<IEnumerable<ApartmentOwnerProfileListDto>>> GetApartmentOwnerProfiles()
        {
            var profiles = await _context.ApartmentOwnerProfiles
                .Select(p => new ApartmentOwnerProfileListDto
                {
                    Id = p.Id,
                    FullName = p.FullName,
                    MobileNo = p.MobileNo,
                    Email = p.Email,
                    NationalId = p.NationalId,
                    SelfImagePath = p.SelfImagePath,
                    IsApproved = p.IsApproved,
                    DivisionName = p.Division,
                    DistrictName = p.District,
                    UpazilaName = p.Upazila
                })
                .ToListAsync();

            return Ok(profiles);
        }

        // GET: api/ApartmentOwnerProfiles/5
        [HttpGet("{id}")]
        public async Task<ActionResult<ApartmentOwnerProfileDto>> GetApartmentOwnerProfile(int id)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var userRole = User.FindFirstValue(ClaimTypes.Role);

            var ownerProfile = await _context.ApartmentOwnerProfiles
                .Include(r => r.User)
                .FirstOrDefaultAsync(r => r.Id == id);

            if (ownerProfile == null)
            {
                return NotFound();
            }

            // Security check: Users can only access their own profile unless they are Admin
            if (userRole != "Admin" && ownerProfile.UserId.ToString() != userId)
            {
                return Forbid();
            }

            var dto = new ApartmentOwnerProfileDto
            {
                Id = ownerProfile.Id,
                UserId = ownerProfile.UserId,
                FullName = ownerProfile.FullName,
                DateOfBirth = ownerProfile.DateOfBirth,
                NationalId = ownerProfile.NationalId,
                NationalIdImagePath = ownerProfile.NationalIdImagePath,
                SelfImagePath = ownerProfile.SelfImagePath,
                MobileNo = ownerProfile.MobileNo,
                Email = ownerProfile.Email,
                Division = ownerProfile.Division,
                District = ownerProfile.District,
                Upazila = ownerProfile.Upazila,
                AreaType = ownerProfile.AreaType.ToString(),
                Ward = ownerProfile.Ward,
                Village = ownerProfile.Village,
                PostCode = ownerProfile.PostCode,
                HoldingNo = ownerProfile.HoldingNo,
                Profession = ownerProfile.Profession,
                IsApproved = ownerProfile.IsApproved,
                User = ownerProfile.User != null ? new UserDto
                {
                    Id = ownerProfile.User.Id,
                    UserName = ownerProfile.User.UserName,
                    Email = ownerProfile.User.Email,
                    PhoneNumber = ownerProfile.User.PhoneNumber,
                    Role = ownerProfile.User.Role.ToString(),
                    IsVerified = ownerProfile.User.IsVerified
                } : null
            };

            return Ok(dto);
        }

        // GET: api/ApartmentOwnerProfiles/current
        [HttpGet("current")]
        [Authorize(Roles = "ApartmentOwner")]
        public async Task<ActionResult<ApartmentOwnerProfileDto>> GetCurrentApartmentOwnerProfile()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (!int.TryParse(userId, out int parsedUserId))
            {
                return Unauthorized();
            }

            var ownerProfile = await _context.ApartmentOwnerProfiles
                .Include(r => r.User)
                .FirstOrDefaultAsync(r => r.UserId == parsedUserId);

            if (ownerProfile == null)
            {
                return NotFound();
            }

            var dto = new ApartmentOwnerProfileDto
            {
                Id = ownerProfile.Id,
                UserId = ownerProfile.UserId,
                FullName = ownerProfile.FullName,
                DateOfBirth = ownerProfile.DateOfBirth,
                NationalId = ownerProfile.NationalId,
                NationalIdImagePath = ownerProfile.NationalIdImagePath,
                SelfImagePath = ownerProfile.SelfImagePath,
                MobileNo = ownerProfile.MobileNo,
                Email = ownerProfile.Email,
                Division = ownerProfile.Division,
                District = ownerProfile.District,
                Upazila = ownerProfile.Upazila,
                AreaType = ownerProfile.AreaType.ToString(),
                Ward = ownerProfile.Ward,
                Village = ownerProfile.Village,
                PostCode = ownerProfile.PostCode,
                HoldingNo = ownerProfile.HoldingNo,
                Profession = ownerProfile.Profession,
                IsApproved = ownerProfile.IsApproved
            };

            return Ok(dto);
        }

        // GET: api/ApartmentOwnerProfiles/pending
        [HttpGet("pending")]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<IEnumerable<ApartmentOwnerProfileListDto>>> GetPendingApartmentOwnerProfiles()
        {
            var profiles = await _context.ApartmentOwnerProfiles
                .Where(p => !p.IsApproved)
                .Select(p => new ApartmentOwnerProfileListDto
                {
                    Id = p.Id,
                    FullName = p.FullName,
                    MobileNo = p.MobileNo,
                    Email = p.Email,
                    NationalId = p.NationalId,
                    SelfImagePath = p.SelfImagePath,
                    IsApproved = p.IsApproved,
                    DivisionName = p.Division,
                    DistrictName = p.District,
                    UpazilaName = p.Upazila
                })
                .ToListAsync();

            return Ok(profiles);
        }

        // Fix for the Enum.TryParse issue in the CreateApartmentOwnerProfile method
        [HttpPost]
        [Authorize(Roles = "ApartmentOwner")]
        public async Task<ActionResult<ApartmentOwnerProfileDto>> CreateApartmentOwnerProfile(CreateApartmentOwnerProfileDto dto)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            // In API's CreateApartmentOwnerProfile action
            if (!int.TryParse(userId, out int parsedUserId))
            {
                return BadRequest("Invalid user ID");
            }

            if (await _context.ApartmentOwnerProfiles.AnyAsync(r => r.UserId == parsedUserId))
            {
                return Conflict("Profile exists");
            }

            // Parse AreaType enum - with error handling
            if (!Enum.TryParse(typeof(AreaType), dto.AreaType.ToString(), true, out var parsedAreaType))
            {
                return BadRequest($"Invalid AreaType value: {dto.AreaType}");
            }

            var areaType = (AreaType)parsedAreaType;

            var ownerProfile = new ApartmentOwnerProfile
            {
                UserId = int.Parse(userId),
                FullName = dto.FullName,
                DateOfBirth = dto.DateOfBirth,
                NationalId = dto.NationalId,
                MobileNo = dto.MobileNo,
                Email = dto.Email,
                Division = dto.Division,
                District = dto.District,
                Upazila = dto.Upazila,
                AreaType = areaType,
                Ward = dto.Ward,
                Village = dto.Village,
                PostCode = dto.PostCode,
                HoldingNo = dto.HoldingNo,
                Profession = dto.Profession,
                IsApproved = false // Requires admin approval
            };

            _context.ApartmentOwnerProfiles.Add(ownerProfile);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetApartmentOwnerProfile), new { id = ownerProfile.Id }, new ApartmentOwnerProfileDto
            {
                Id = ownerProfile.Id,
                UserId = ownerProfile.UserId,
                FullName = ownerProfile.FullName,
                DateOfBirth = ownerProfile.DateOfBirth,
                NationalId = ownerProfile.NationalId,
                MobileNo = ownerProfile.MobileNo,
                Email = ownerProfile.Email,
                Division = ownerProfile.Division,
                District = ownerProfile.District,
                Upazila = ownerProfile.Upazila,
                AreaType = ownerProfile.AreaType.ToString(),
                Ward = ownerProfile.Ward,
                Village = ownerProfile.Village,
                PostCode = ownerProfile.PostCode,
                HoldingNo = ownerProfile.HoldingNo,
                Profession = ownerProfile.Profession,
                IsApproved = ownerProfile.IsApproved
            });
        }

        // PUT: api/ApartmentOwnerProfiles/5
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateApartmentOwnerProfile(int id, UpdateApartmentOwnerProfileDto dto)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var userRole = User.FindFirstValue(ClaimTypes.Role);

            var ownerProfile = await _context.ApartmentOwnerProfiles.FindAsync(id);
            if (ownerProfile == null)
            {
                return NotFound();
            }

            // Security check: Users can only update their own profile unless they are Admin
            if (userRole != "Admin" && ownerProfile.UserId.ToString() != userId)
            {
                return Forbid();
            }

            try
            {
                // Update fields with string-based location data
                ownerProfile.FullName = dto.FullName;
                ownerProfile.DateOfBirth = dto.DateOfBirth;
                ownerProfile.NationalId = dto.NationalId;
                ownerProfile.MobileNo = dto.MobileNo;
                ownerProfile.Email = dto.Email;
                ownerProfile.Division = dto.Division;
                ownerProfile.District = dto.District;
                ownerProfile.Upazila = dto.Upazila;
                ownerProfile.AreaType = dto.AreaType;
                ownerProfile.Ward = dto.Ward;
                ownerProfile.Village = dto.Village;
                ownerProfile.PostCode = dto.PostCode;
                ownerProfile.HoldingNo = dto.HoldingNo;
                ownerProfile.Profession = dto.Profession;

                _context.Entry(ownerProfile).State = EntityState.Modified;

                // Don't allow non-admins to change approval status
                if (userRole != "Admin")
                {
                    _context.Entry(ownerProfile).Property(x => x.IsApproved).IsModified = false;
                }

                await _context.SaveChangesAsync();
                
                return Ok(ApiResponse<object>.SuccessResponse(null, "Profile updated successfully."));
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!ApartmentOwnerProfileExists(id))
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
                _logger.LogError(ex, "Error updating apartment owner profile {ProfileId}", id);
                return StatusCode(500, ApiResponse<object>.ErrorResponse("An error occurred while updating the profile."));
            }
        }

        // PATCH: api/ApartmentOwnerProfiles/5/approve
        [HttpPatch("{id}/approve")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> ApproveApartmentOwnerProfile(int id, ApproveApartmentOwnerProfileDto dto)
        {
            var ownerProfile = await _context.ApartmentOwnerProfiles.FindAsync(id);
            if (ownerProfile == null)
            {
                return NotFound();
            }

            ownerProfile.IsApproved = dto.IsApproved;
            await _context.SaveChangesAsync();

            return NoContent();
        }

        // POST: api/ApartmentOwnerProfiles/5/upload-image
        [HttpPost("{id}/upload-image")]
        public async Task<ActionResult<string>> UploadImage(int id, [FromForm] UploadOwnerImageDto dto)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var userRole = User.FindFirstValue(ClaimTypes.Role);

            var ownerProfile = await _context.ApartmentOwnerProfiles.FindAsync(id);
            if (ownerProfile == null)
            {
                return NotFound();
            }

            // Security check: Users can only upload to their own profile unless they are Admin
            if (userRole != "Admin" && ownerProfile.UserId.ToString() != userId)
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
                if (dto.ImageType.ToLower() == "nationalid")
                {
                    ownerProfile.NationalIdImagePath = relativePath;
                }
                else
                {
                    ownerProfile.SelfImagePath = relativePath;
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

        private bool ApartmentOwnerProfileExists(int id)
        {
            return _context.ApartmentOwnerProfiles.Any(e => e.Id == id);
        }
    }
}
