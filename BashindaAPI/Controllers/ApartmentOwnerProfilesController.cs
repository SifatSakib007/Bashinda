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
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public class ApartmentOwnerProfilesController : BaseApiController
    {
        private readonly ApplicationDbContext _context;
        private readonly IWebHostEnvironment _env;
        private readonly ILogger<ApartmentOwnerProfilesController> _logger;

        public ApartmentOwnerProfilesController(
            ApplicationDbContext context,
            IWebHostEnvironment env,
            ILogger<ApartmentOwnerProfilesController> logger)
        {
            _context = context;
            _env = env;
            _logger = logger;
        }

        // GET: api/ApartmentOwnerProfiles
        [HttpGet]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<IEnumerable<ApartmentOwnerProfileListDto>>> GetApartmentOwnerProfiles()
        {
            var profiles = await _context.ApartmentOwnerProfiles
                .Include(p => p.Division)
                .Include(p => p.District)
                .Include(p => p.Upazila)
                .Select(p => new ApartmentOwnerProfileListDto
                {
                    Id = p.Id,
                    FullName = p.FullName,
                    MobileNo = p.MobileNo,
                    Email = p.Email,
                    NationalId = p.NationalId,
                    SelfImagePath = p.SelfImagePath,
                    IsApproved = p.IsApproved,
                    DivisionName = p.Division != null ? p.Division.Name : string.Empty,
                    DistrictName = p.District != null ? p.District.Name : string.Empty,
                    UpazilaName = p.Upazila != null ? p.Upazila.Name : string.Empty
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
                .Include(r => r.Division)
                .Include(r => r.District)
                .Include(r => r.Upazila)
                .Include(r => r.Ward)
                .Include(r => r.Village)
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
                Address = new AddressDto
                {
                    DivisionId = ownerProfile.DivisionId,
                    DivisionName = ownerProfile.Division?.Name ?? string.Empty,
                    DistrictId = ownerProfile.DistrictId,
                    DistrictName = ownerProfile.District?.Name ?? string.Empty,
                    UpazilaId = ownerProfile.UpazilaId,
                    UpazilaName = ownerProfile.Upazila?.Name ?? string.Empty,
                    AreaType = ownerProfile.AreaType,
                    WardId = ownerProfile.WardId,
                    WardName = ownerProfile.Ward?.Name ?? string.Empty,
                    VillageId = ownerProfile.VillageId,
                    VillageName = ownerProfile.Village?.Name ?? string.Empty,
                    PostCode = ownerProfile.PostCode,
                    HoldingNo = ownerProfile.HoldingNo
                },
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
            if (string.IsNullOrEmpty(userId))
            {
                return Unauthorized();
            }

            var ownerProfile = await _context.ApartmentOwnerProfiles
                .Include(r => r.User)
                .Include(r => r.Division)
                .Include(r => r.District)
                .Include(r => r.Upazila)
                .Include(r => r.Ward)
                .Include(r => r.Village)
                .FirstOrDefaultAsync(r => r.UserId.ToString() == userId);

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
                Address = new AddressDto
                {
                    DivisionId = ownerProfile.DivisionId,
                    DivisionName = ownerProfile.Division?.Name ?? string.Empty,
                    DistrictId = ownerProfile.DistrictId,
                    DistrictName = ownerProfile.District?.Name ?? string.Empty,
                    UpazilaId = ownerProfile.UpazilaId,
                    UpazilaName = ownerProfile.Upazila?.Name ?? string.Empty,
                    AreaType = ownerProfile.AreaType,
                    WardId = ownerProfile.WardId,
                    WardName = ownerProfile.Ward?.Name ?? string.Empty,
                    VillageId = ownerProfile.VillageId,
                    VillageName = ownerProfile.Village?.Name ?? string.Empty,
                    PostCode = ownerProfile.PostCode,
                    HoldingNo = ownerProfile.HoldingNo
                },
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
                .Include(p => p.Division)
                .Include(p => p.District)
                .Include(p => p.Upazila)
                .Select(p => new ApartmentOwnerProfileListDto
                {
                    Id = p.Id,
                    FullName = p.FullName,
                    MobileNo = p.MobileNo,
                    Email = p.Email,
                    NationalId = p.NationalId,
                    SelfImagePath = p.SelfImagePath,
                    IsApproved = p.IsApproved,
                    DivisionName = p.Division != null ? p.Division.Name : string.Empty,
                    DistrictName = p.District != null ? p.District.Name : string.Empty,
                    UpazilaName = p.Upazila != null ? p.Upazila.Name : string.Empty
                })
                .ToListAsync();

            return Ok(profiles);
        }

        // POST: api/ApartmentOwnerProfiles
        [HttpPost]
        [Authorize(Roles = "ApartmentOwner")]
        public async Task<ActionResult<ApartmentOwnerProfileDto>> CreateApartmentOwnerProfile(CreateApartmentOwnerProfileDto dto)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userId))
            {
                return Unauthorized();
            }

            if (await _context.ApartmentOwnerProfiles.AnyAsync(r => r.UserId.ToString() == userId))
            {
                return Conflict("You already have a profile");
            }
            
            // Validate location data
            if (!await _context.Divisions.AnyAsync(d => d.Id == dto.DivisionId))
            {
                ModelState.AddModelError("DivisionId", "Invalid Division");
                return BadRequest(ModelState);
            }

            if (!await _context.Districts.AnyAsync(d => d.Id == dto.DistrictId && d.DivisionId == dto.DivisionId))
            {
                ModelState.AddModelError("DistrictId", "Invalid District for the selected Division");
                return BadRequest(ModelState);
            }

            if (!await _context.Upazilas.AnyAsync(u => u.Id == dto.UpazilaId && u.DistrictId == dto.DistrictId))
            {
                ModelState.AddModelError("UpazilaId", "Invalid Upazila for the selected District");
                return BadRequest(ModelState);
            }

            if (!await _context.Wards.AnyAsync(w => w.Id == dto.WardId && w.UpazilaId == dto.UpazilaId && w.AreaType == dto.AreaType))
            {
                ModelState.AddModelError("WardId", "Invalid Ward for the selected Upazila and Area Type");
                return BadRequest(ModelState);
            }

            if (!await _context.Villages.AnyAsync(v => v.Id == dto.VillageId && v.WardId == dto.WardId))
            {
                ModelState.AddModelError("VillageId", "Invalid Village/Area for the selected Ward");
                return BadRequest(ModelState);
            }

            var ownerProfile = new ApartmentOwnerProfile
            {
                UserId = int.Parse(userId),
                FullName = dto.FullName,
                DateOfBirth = dto.DateOfBirth,
                NationalId = dto.NationalId,
                MobileNo = dto.MobileNo,
                Email = dto.Email,
                DivisionId = dto.DivisionId,
                DistrictId = dto.DistrictId,
                UpazilaId = dto.UpazilaId,
                AreaType = dto.AreaType,
                WardId = dto.WardId,
                VillageId = dto.VillageId,
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
                Address = new AddressDto
                {
                    DivisionId = ownerProfile.DivisionId,
                    DivisionName = _context.Divisions.FirstOrDefault(d => d.Id == ownerProfile.DivisionId)?.Name ?? string.Empty,
                    DistrictId = ownerProfile.DistrictId,
                    DistrictName = _context.Districts.FirstOrDefault(d => d.Id == ownerProfile.DistrictId)?.Name ?? string.Empty,
                    UpazilaId = ownerProfile.UpazilaId,
                    UpazilaName = _context.Upazilas.FirstOrDefault(u => u.Id == ownerProfile.UpazilaId)?.Name ?? string.Empty,
                    AreaType = ownerProfile.AreaType,
                    WardId = ownerProfile.WardId,
                    WardName = _context.Wards.FirstOrDefault(w => w.Id == ownerProfile.WardId)?.Name ?? string.Empty,
                    VillageId = ownerProfile.VillageId,
                    VillageName = _context.Villages.FirstOrDefault(v => v.Id == ownerProfile.VillageId)?.Name ?? string.Empty,
                    PostCode = ownerProfile.PostCode,
                    HoldingNo = ownerProfile.HoldingNo
                },
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
                // Validate location hierarchy
                if (!await ValidateLocationHierarchy(dto.DivisionId, dto.DistrictId, dto.UpazilaId, dto.AreaType, dto.WardId, dto.VillageId))
                {
                    return BadRequest(ApiResponse<object>.ErrorResponse(
                        "Invalid location hierarchy. Please ensure Division, District, Upazila, Ward, and Village/Area are related correctly."));
                }

                // Update fields
                ownerProfile.FullName = dto.FullName;
                ownerProfile.DateOfBirth = dto.DateOfBirth;
                ownerProfile.NationalId = dto.NationalId;
                ownerProfile.MobileNo = dto.MobileNo;
                ownerProfile.Email = dto.Email;
                ownerProfile.DivisionId = dto.DivisionId;
                ownerProfile.DistrictId = dto.DistrictId;
                ownerProfile.UpazilaId = dto.UpazilaId;
                ownerProfile.AreaType = dto.AreaType;
                ownerProfile.WardId = dto.WardId;
                ownerProfile.VillageId = dto.VillageId;
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

        // Helper method to validate location hierarchy
        private async Task<bool> ValidateLocationHierarchy(int divisionId, int districtId, int upazilaId, AreaType areaType, int wardId, int villageId)
        {
            // Check if division exists
            var division = await _context.Divisions.FindAsync(divisionId);
            if (division == null) return false;
            
            // Check if district exists and belongs to the specified division
            var district = await _context.Districts.FirstOrDefaultAsync(d => d.Id == districtId && d.DivisionId == divisionId);
            if (district == null) return false;
            
            // Check if upazila exists and belongs to the specified district
            var upazila = await _context.Upazilas.FirstOrDefaultAsync(u => u.Id == upazilaId && u.DistrictId == districtId);
            if (upazila == null) return false;
            
            // Check if ward exists, belongs to the specified upazila, and has the correct area type
            var ward = await _context.Wards.FirstOrDefaultAsync(w => w.Id == wardId && w.UpazilaId == upazilaId && w.AreaType == areaType);
            if (ward == null) return false;
            
            // Check if village exists and belongs to the specified ward
            var village = await _context.Villages.FirstOrDefaultAsync(v => v.Id == villageId && v.WardId == wardId);
            if (village == null) return false;
            
            return true;
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
