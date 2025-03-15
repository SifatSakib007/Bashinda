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
    public class RenterProfilesController : BaseApiController
    {
        private readonly ApplicationDbContext _context;
        private readonly IWebHostEnvironment _env;
        private readonly ILogger<RenterProfilesController> _logger;

        public RenterProfilesController(
            ApplicationDbContext context,
            IWebHostEnvironment env,
            ILogger<RenterProfilesController> logger)
        {
            _context = context;
            _env = env;
            _logger = logger;
        }

        // GET: api/RenterProfiles
        [HttpGet]
        [Authorize(Roles = "Admin,ApartmentOwner")]
        public async Task<ActionResult<ApiResponse<IEnumerable<RenterProfileListDto>>>> GetRenterProfiles()
        {
            var profiles = await _context.RenterProfiles
                .Include(p => p.Division)
                .Include(p => p.District)
                .Include(p => p.Upazila)
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
                    IsApproved = p.IsApproved,
                    DivisionName = p.Division != null ? p.Division.Name : string.Empty,
                    DistrictName = p.District != null ? p.District.Name : string.Empty,
                    UpazilaName = p.Upazila != null ? p.Upazila.Name : string.Empty
                })
                .ToListAsync();

            return Ok(ApiResponse<IEnumerable<RenterProfileListDto>>.SuccessResponse(profiles));
        }

        // GET: api/RenterProfiles/5
        [HttpGet("{id}")]
        public async Task<ActionResult<RenterProfileDto>> GetRenterProfile(int id)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var userRole = User.FindFirstValue(ClaimTypes.Role);

            var renterProfile = await _context.RenterProfiles
                .Include(r => r.User)
                .Include(r => r.Division)
                .Include(r => r.District)
                .Include(r => r.Upazila)
                .Include(r => r.Ward)
                .Include(r => r.Village)
                .FirstOrDefaultAsync(r => r.Id == id);

            if (renterProfile == null)
            {
                return NotFound();
            }

            // Security check: Users can only access their own profile unless they are Admin or ApartmentOwner
            if (userRole != "Admin" && userRole != "ApartmentOwner" && renterProfile.UserId.ToString() != userId)
            {
                return Forbid();
            }

            var dto = new RenterProfileDto
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
                Address = new AddressDto
                {
                    DivisionId = renterProfile.DivisionId,
                    DivisionName = _context.Divisions.FirstOrDefault(d => d.Id == renterProfile.DivisionId)?.Name ?? string.Empty,
                    DistrictId = renterProfile.DistrictId,
                    DistrictName = _context.Districts.FirstOrDefault(d => d.Id == renterProfile.DistrictId)?.Name ?? string.Empty,
                    UpazilaId = renterProfile.UpazilaId,
                    UpazilaName = _context.Upazilas.FirstOrDefault(u => u.Id == renterProfile.UpazilaId)?.Name ?? string.Empty,
                    AreaType = renterProfile.AreaType,
                    WardId = renterProfile.WardId,
                    WardName = _context.Wards.FirstOrDefault(w => w.Id == renterProfile.WardId)?.Name ?? string.Empty,
                    VillageId = renterProfile.VillageId,
                    VillageName = _context.Villages.FirstOrDefault(v => v.Id == renterProfile.VillageId)?.Name ?? string.Empty,
                    PostCode = renterProfile.PostCode,
                    HoldingNo = renterProfile.HoldingNo
                },
                IsApproved = renterProfile.IsApproved,
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

        // GET: api/RenterProfiles/current
        [HttpGet("current")]
        [Authorize(Roles = "ApartmentRenter")]
        public async Task<ActionResult<RenterProfileDto>> GetCurrentRenterProfile()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userId))
            {
                return Unauthorized();
            }

            var renterProfile = await _context.RenterProfiles
                .Include(r => r.User)
                .Include(r => r.Division)
                .Include(r => r.District)
                .Include(r => r.Upazila)
                .Include(r => r.Ward)
                .Include(r => r.Village)
                .FirstOrDefaultAsync(r => r.UserId.ToString() == userId);

            if (renterProfile == null)
            {
                return NotFound();
            }

            var dto = new RenterProfileDto
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
                Address = new AddressDto
                {
                    DivisionId = renterProfile.DivisionId,
                    DivisionName = _context.Divisions.FirstOrDefault(d => d.Id == renterProfile.DivisionId)?.Name ?? string.Empty,
                    DistrictId = renterProfile.DistrictId,
                    DistrictName = _context.Districts.FirstOrDefault(d => d.Id == renterProfile.DistrictId)?.Name ?? string.Empty,
                    UpazilaId = renterProfile.UpazilaId,
                    UpazilaName = _context.Upazilas.FirstOrDefault(u => u.Id == renterProfile.UpazilaId)?.Name ?? string.Empty,
                    AreaType = renterProfile.AreaType,
                    WardId = renterProfile.WardId,
                    WardName = _context.Wards.FirstOrDefault(w => w.Id == renterProfile.WardId)?.Name ?? string.Empty,
                    VillageId = renterProfile.VillageId,
                    VillageName = _context.Villages.FirstOrDefault(v => v.Id == renterProfile.VillageId)?.Name ?? string.Empty,
                    PostCode = renterProfile.PostCode,
                    HoldingNo = renterProfile.HoldingNo
                },
                IsApproved = renterProfile.IsApproved,
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
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<ApiResponse<IEnumerable<RenterProfileListDto>>>> GetPendingRenterProfiles()
        {
            var profiles = await _context.RenterProfiles
                .Where(p => !p.IsApproved)
                .Include(p => p.Division)
                .Include(p => p.District)
                .Include(p => p.Upazila)
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
                    IsApproved = p.IsApproved,
                    DivisionName = p.Division != null ? p.Division.Name : string.Empty,
                    DistrictName = p.District != null ? p.District.Name : string.Empty,
                    UpazilaName = p.Upazila != null ? p.Upazila.Name : string.Empty
                })
                .ToListAsync();

            return Ok(ApiResponse<IEnumerable<RenterProfileListDto>>.SuccessResponse(profiles));
        }

        // POST: api/RenterProfiles
        [HttpPost]
        [Authorize(Roles = "ApartmentRenter")]
        public async Task<ActionResult<RenterProfileDto>> CreateRenterProfile(CreateRenterProfileDto dto)
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
                DivisionId = dto.DivisionId,
                DistrictId = dto.DistrictId,
                UpazilaId = dto.UpazilaId,
                AreaType = dto.AreaType,
                WardId = dto.WardId,
                VillageId = dto.VillageId,
                PostCode = dto.PostCode,
                HoldingNo = dto.HoldingNo,
                IsApproved = false // Requires admin approval
            };

            _context.RenterProfiles.Add(renterProfile);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetRenterProfile), new { id = renterProfile.Id }, new RenterProfileDto
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
                Address = new AddressDto
                {
                    DivisionId = renterProfile.DivisionId,
                    DivisionName = _context.Divisions.FirstOrDefault(d => d.Id == renterProfile.DivisionId)?.Name ?? string.Empty,
                    DistrictId = renterProfile.DistrictId,
                    DistrictName = _context.Districts.FirstOrDefault(d => d.Id == renterProfile.DistrictId)?.Name ?? string.Empty,
                    UpazilaId = renterProfile.UpazilaId,
                    UpazilaName = _context.Upazilas.FirstOrDefault(u => u.Id == renterProfile.UpazilaId)?.Name ?? string.Empty,
                    AreaType = renterProfile.AreaType,
                    WardId = renterProfile.WardId,
                    WardName = _context.Wards.FirstOrDefault(w => w.Id == renterProfile.WardId)?.Name ?? string.Empty,
                    VillageId = renterProfile.VillageId,
                    VillageName = _context.Villages.FirstOrDefault(v => v.Id == renterProfile.VillageId)?.Name ?? string.Empty,
                    PostCode = renterProfile.PostCode,
                    HoldingNo = renterProfile.HoldingNo
                },
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
            renterProfile.DivisionId = dto.DivisionId;
            renterProfile.DistrictId = dto.DistrictId;
            renterProfile.UpazilaId = dto.UpazilaId;
            renterProfile.AreaType = dto.AreaType;
            renterProfile.WardId = dto.WardId;
            renterProfile.VillageId = dto.VillageId;
            renterProfile.PostCode = dto.PostCode;
            renterProfile.HoldingNo = dto.HoldingNo;

            _context.Entry(renterProfile).State = EntityState.Modified;

            // Don't allow non-admins to change approval status
            if (userRole != "Admin")
            {
                _context.Entry(renterProfile).Property(x => x.IsApproved).IsModified = false;
            }

            try
            {
                await _context.SaveChangesAsync();
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

            return NoContent();
        }

        // PATCH: api/RenterProfiles/5/approve
        [HttpPatch("{id}/approve")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> ApproveRenterProfile(int id, ApproveRenterProfileDto dto)
        {
            var renterProfile = await _context.RenterProfiles.FindAsync(id);
            if (renterProfile == null)
            {
                return NotFound();
            }

            renterProfile.IsApproved = dto.IsApproved;
            await _context.SaveChangesAsync();

            return NoContent();
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
