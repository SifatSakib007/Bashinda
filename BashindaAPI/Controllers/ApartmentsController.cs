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
    public class ApartmentsController : BaseApiController
    {
        private readonly ApplicationDbContext _context;
        private readonly IWebHostEnvironment _env;
        private readonly ILogger<ApartmentsController> _logger;

        public ApartmentsController(
            ApplicationDbContext context,
            IWebHostEnvironment env,
            ILogger<ApartmentsController> logger)
        {
            _context = context;
            _env = env;
            _logger = logger;
        }

        // GET: api/Apartments
        [HttpGet]
        [AllowAnonymous]
        public async Task<ActionResult<IEnumerable<ApartmentListDto>>> GetApartments()
        {
            var apartments = await _context.Apartments
                .Include(a => a.Owner)
                .Select(a => new ApartmentListDto
                {
                    Id = a.Id,
                    BuildingName = a.BuildingName,
                    Address = a.Address,
                    ApartmentNumber = a.ApartmentNumber,
                    NumberOfRooms = a.NumberOfRooms,
                    MonthlyRent = a.MonthlyRent,
                    IsAvailable = a.IsAvailable,
                    ImagePath = a.ImagePath,
                    OwnerName = a.Owner != null ? a.Owner.FullName : ""
                })
                .ToListAsync();

            return Ok(apartments);
        }

        // GET: api/Apartments/available
        [HttpGet("available")]
        [AllowAnonymous]
        public async Task<ActionResult<IEnumerable<ApartmentListDto>>> GetAvailableApartments()
        {
            var apartments = await _context.Apartments
                .Where(a => a.IsAvailable)
                .Include(a => a.Owner)
                .Select(a => new ApartmentListDto
                {
                    Id = a.Id,
                    BuildingName = a.BuildingName,
                    Address = a.Address,
                    ApartmentNumber = a.ApartmentNumber,
                    NumberOfRooms = a.NumberOfRooms,
                    MonthlyRent = a.MonthlyRent,
                    IsAvailable = a.IsAvailable,
                    ImagePath = a.ImagePath,
                    OwnerName = a.Owner != null ? a.Owner.FullName : ""
                })
                .ToListAsync();

            return Ok(apartments);
        }

        // GET: api/Apartments/5
        [HttpGet("{id}")]
        [AllowAnonymous]
        public async Task<ActionResult<ApartmentDto>> GetApartment(int id)
        {
            var apartment = await _context.Apartments
                .Include(a => a.Owner)
                .FirstOrDefaultAsync(a => a.Id == id);

            if (apartment == null)
            {
                return NotFound();
            }

            var dto = new ApartmentDto
            {
                Id = apartment.Id,
                OwnerId = apartment.OwnerId,
                BuildingName = apartment.BuildingName,
                Address = apartment.Address,
                ApartmentNumber = apartment.ApartmentNumber,
                NumberOfRooms = apartment.NumberOfRooms,
                NumberOfBathrooms = apartment.NumberOfBathrooms,
                SquareFeet = apartment.SquareFeet,
                MonthlyRent = apartment.MonthlyRent,
                IsAvailable = apartment.IsAvailable,
                ImagePath = apartment.ImagePath,
                Description = apartment.Description,
                CreatedAt = apartment.CreatedAt,
                Owner = apartment.Owner != null ? new ApartmentOwnerProfileListDto
                {
                    Id = apartment.Owner.Id,
                    FullName = apartment.Owner.FullName,
                    MobileNo = apartment.Owner.MobileNo,
                    Email = apartment.Owner.Email,
                    SelfImagePath = apartment.Owner.SelfImagePath,
                    IsApproved = apartment.Owner.IsApproved
                } : null
            };

            return Ok(dto);
        }

        // GET: api/Apartments/owner
        [HttpGet("owner")]
        [Authorize(Roles = "ApartmentOwner")]
        public async Task<ActionResult<IEnumerable<ApartmentListDto>>> GetOwnerApartments()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userId))
            {
                return Unauthorized();
            }

            var ownerProfile = await _context.ApartmentOwnerProfiles
                .FirstOrDefaultAsync(o => o.UserId.ToString() == userId);

            if (ownerProfile == null)
            {
                return NotFound("Owner profile not found");
            }

            var apartments = await _context.Apartments
                .Where(a => a.OwnerId == ownerProfile.Id)
                .Select(a => new ApartmentListDto
                {
                    Id = a.Id,
                    BuildingName = a.BuildingName,
                    Address = a.Address,
                    ApartmentNumber = a.ApartmentNumber,
                    NumberOfRooms = a.NumberOfRooms,
                    MonthlyRent = a.MonthlyRent,
                    IsAvailable = a.IsAvailable,
                    ImagePath = a.ImagePath,
                    OwnerName = ownerProfile.FullName
                })
                .ToListAsync();

            return Ok(apartments);
        }

        // POST: api/Apartments
        [HttpPost]
        [Authorize(Roles = "ApartmentOwner")]
        public async Task<ActionResult<ApartmentDto>> CreateApartment(CreateApartmentDto dto)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userId))
            {
                return Unauthorized();
            }

            var ownerProfile = await _context.ApartmentOwnerProfiles
                .FirstOrDefaultAsync(o => o.UserId.ToString() == userId);

            if (ownerProfile == null)
            {
                return NotFound("Owner profile not found");
            }

            if (!ownerProfile.IsApproved)
            {
                return BadRequest("Your profile is not approved yet. You cannot create apartments until your profile is approved by an admin.");
            }

            var apartment = new Apartment
            {
                OwnerId = ownerProfile.Id,
                BuildingName = dto.BuildingName,
                Address = dto.Address,
                ApartmentNumber = dto.ApartmentNumber,
                NumberOfRooms = dto.NumberOfRooms,
                NumberOfBathrooms = dto.NumberOfBathrooms,
                SquareFeet = dto.SquareFeet,
                MonthlyRent = dto.MonthlyRent,
                IsAvailable = true,
                Description = dto.Description,
                CreatedAt = DateTime.Now
            };

            _context.Apartments.Add(apartment);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetApartment), new { id = apartment.Id }, new ApartmentDto
            {
                Id = apartment.Id,
                OwnerId = apartment.OwnerId,
                BuildingName = apartment.BuildingName,
                Address = apartment.Address,
                ApartmentNumber = apartment.ApartmentNumber,
                NumberOfRooms = apartment.NumberOfRooms,
                NumberOfBathrooms = apartment.NumberOfBathrooms,
                SquareFeet = apartment.SquareFeet,
                MonthlyRent = apartment.MonthlyRent,
                IsAvailable = apartment.IsAvailable,
                Description = apartment.Description,
                CreatedAt = apartment.CreatedAt
            });
        }

        // PUT: api/Apartments/5
        [HttpPut("{id}")]
        [Authorize(Roles = "ApartmentOwner,Admin")]
        public async Task<IActionResult> UpdateApartment(int id, UpdateApartmentDto dto)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var userRole = User.FindFirstValue(ClaimTypes.Role);

            var apartment = await _context.Apartments
                .Include(a => a.Owner)
                .FirstOrDefaultAsync(a => a.Id == id);

            if (apartment == null)
            {
                return NotFound();
            }

            // Security check: Only the owner or an admin can update the apartment
            if (userRole != "Admin" && (apartment.Owner == null || apartment.Owner.UserId.ToString() != userId))
            {
                return Forbid();
            }

            // Update fields
            apartment.BuildingName = dto.BuildingName;
            apartment.Address = dto.Address;
            apartment.ApartmentNumber = dto.ApartmentNumber;
            apartment.NumberOfRooms = dto.NumberOfRooms;
            apartment.NumberOfBathrooms = dto.NumberOfBathrooms;
            apartment.SquareFeet = dto.SquareFeet;
            apartment.MonthlyRent = dto.MonthlyRent;
            apartment.IsAvailable = dto.IsAvailable;
            apartment.Description = dto.Description;

            _context.Entry(apartment).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!ApartmentExists(id))
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

        // DELETE: api/Apartments/5
        [HttpDelete("{id}")]
        [Authorize(Roles = "ApartmentOwner,Admin")]
        public async Task<IActionResult> DeleteApartment(int id)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var userRole = User.FindFirstValue(ClaimTypes.Role);

            var apartment = await _context.Apartments
                .Include(a => a.Owner)
                .FirstOrDefaultAsync(a => a.Id == id);
                
            if (apartment == null)
            {
                return NotFound();
            }

            // Security check: Only the owner or an admin can delete the apartment
            if (userRole != "Admin" && (apartment.Owner == null || apartment.Owner.UserId.ToString() != userId))
            {
                return Forbid();
            }

            _context.Apartments.Remove(apartment);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        // POST: api/Apartments/5/upload-image
        [HttpPost("{id}/upload-image")]
        [Authorize(Roles = "ApartmentOwner,Admin")]
        public async Task<ActionResult<string>> UploadImage(int id, [FromForm] UploadApartmentImageDto dto)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var userRole = User.FindFirstValue(ClaimTypes.Role);

            var apartment = await _context.Apartments
                .Include(a => a.Owner)
                .FirstOrDefaultAsync(a => a.Id == id);
                
            if (apartment == null)
            {
                return NotFound();
            }

            // Security check: Only the owner or an admin can upload images
            if (userRole != "Admin" && (apartment.Owner == null || apartment.Owner.UserId.ToString() != userId))
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

                // Update the image path
                var relativePath = Path.Combine("uploads", uniqueFileName).Replace("\\", "/");
                apartment.ImagePath = relativePath;

                await _context.SaveChangesAsync();

                return Ok(new { path = relativePath });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error uploading image: {ErrorMessage}", ex.Message);
                return StatusCode(500, "An error occurred while uploading the image");
            }
        }

        private bool ApartmentExists(int id)
        {
            return _context.Apartments.Any(e => e.Id == id);
        }
    }
}
