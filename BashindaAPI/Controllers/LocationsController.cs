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
    public class LocationsController : BaseApiController
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<LocationsController> _logger;

        public LocationsController(
            ApplicationDbContext context,
            ILogger<LocationsController> logger)
        {
            _context = context;
            _logger = logger;
        }

        // GET: api/Locations/divisions
        [HttpGet("divisions")]
        public async Task<ActionResult<IEnumerable<LocationDto>>> GetDivisions()
        {
            var divisions = await _context.Divisions
                .Select(d => new LocationDto
                {
                    Id = d.Id,
                    Name = d.Name
                })
                .OrderBy(d => d.Name)
                .ToListAsync();

            return Ok(divisions);
        }

        // GET: api/Locations/districts/{divisionId}
        [HttpGet("districts/{divisionId}")]
        public async Task<ActionResult<IEnumerable<LocationDto>>> GetDistrictsByDivision(int divisionId)
        {
            var districts = await _context.Districts
                .Where(d => d.DivisionId == divisionId)
                .Select(d => new LocationDto
                {
                    Id = d.Id,
                    Name = d.Name
                })
                .OrderBy(d => d.Name)
                .ToListAsync();

            return Ok(districts);
        }

        // GET: api/Locations/upazilas/{districtId}
        [HttpGet("upazilas/{districtId}")]
        public async Task<ActionResult<IEnumerable<LocationDto>>> GetUpazilasByDistrict(int districtId)
        {
            var upazilas = await _context.Upazilas
                .Where(u => u.DistrictId == districtId)
                .Select(u => new LocationDto
                {
                    Id = u.Id,
                    Name = u.Name
                })
                .OrderBy(u => u.Name)
                .ToListAsync();

            return Ok(upazilas);
        }

        // GET: api/Locations/wards/{upazilaId}/{areaType}
        [HttpGet("wards/{upazilaId}/{areaType}")]
        public async Task<ActionResult<IEnumerable<LocationDto>>> GetWardsByUpazilaAndAreaType(int upazilaId, AreaType areaType)
        {
            var wards = await _context.Wards
                .Where(w => w.UpazilaId == upazilaId && w.AreaType == areaType)
                .Select(w => new LocationDto
                {
                    Id = w.Id,
                    Name = w.Name
                })
                .OrderBy(w => w.Name)
                .ToListAsync();

            return Ok(wards);
        }

        // GET: api/Locations/villages/{wardId}
        [HttpGet("villages/{wardId}")]
        public async Task<ActionResult<IEnumerable<LocationDto>>> GetVillagesByWard(int wardId)
        {
            var villages = await _context.Villages
                .Where(v => v.WardId == wardId)
                .Select(v => new LocationDto
                {
                    Id = v.Id,
                    Name = v.Name
                })
                .OrderBy(v => v.Name)
                .ToListAsync();

            return Ok(villages);
        }
        
        // GET: api/Locations/area-types
        [HttpGet("area-types")]
        public ActionResult<IEnumerable<AreaTypeDto>> GetAreaTypes()
        {
            var areaTypes = Enum.GetValues(typeof(AreaType))
                .Cast<AreaType>()
                .Select(t => new AreaTypeDto
                {
                    Id = (int)t,
                    Name = t.ToString()
                })
                .ToList();

            return Ok(areaTypes);
        }
        
        // POST: api/Locations/seed
        [HttpPost("seed")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> SeedLocationData()
        {
            // Only seed if the database is empty
            if (await _context.Divisions.AnyAsync())
            {
                return Ok("Data already exists. Skipping seed operation.");
            }

            try
            {
                // Sample divisions for Bangladesh
                var dhaka = new Division { Name = "Dhaka" };
                var chittagong = new Division { Name = "Chittagong" };
                var khulna = new Division { Name = "Khulna" };
                var rajshahi = new Division { Name = "Rajshahi" };
                var sylhet = new Division { Name = "Sylhet" };

                _context.Divisions.AddRange(dhaka, chittagong, khulna, rajshahi, sylhet);
                await _context.SaveChangesAsync();

                // Sample districts for Dhaka division
                var dhakaDistrict = new District { Name = "Dhaka", Division = dhaka };
                var gazipur = new District { Name = "Gazipur", Division = dhaka };
                var narayanganj = new District { Name = "Narayanganj", Division = dhaka };
                var narsingdi = new District { Name = "Narsingdi", Division = dhaka };
                var tangail = new District { Name = "Tangail", Division = dhaka };

                _context.Districts.AddRange(dhakaDistrict, gazipur, narayanganj, narsingdi, tangail);

                // Sample districts for Chittagong division
                var chittagongDistrict = new District { Name = "Chittagong", Division = chittagong };
                var coxsBazar = new District { Name = "Cox's Bazar", Division = chittagong };
                var feni = new District { Name = "Feni", Division = chittagong };
                var rangamati = new District { Name = "Rangamati", Division = chittagong };
                var bandarban = new District { Name = "Bandarban", Division = chittagong };

                _context.Districts.AddRange(chittagongDistrict, coxsBazar, feni, rangamati, bandarban);
                await _context.SaveChangesAsync();

                // Sample upazilas for Dhaka district
                var dhanmondi = new Upazila { Name = "Dhanmondi", District = dhakaDistrict };
                var mirpur = new Upazila { Name = "Mirpur", District = dhakaDistrict };
                var uttara = new Upazila { Name = "Uttara", District = dhakaDistrict };
                var gulshan = new Upazila { Name = "Gulshan", District = dhakaDistrict };
                var mohammadpur = new Upazila { Name = "Mohammadpur", District = dhakaDistrict };

                _context.Upazilas.AddRange(dhanmondi, mirpur, uttara, gulshan, mohammadpur);
                await _context.SaveChangesAsync();

                // Sample wards for Dhanmondi (both City Corporation and Union)
                var dhanmondiWard1 = new Ward { Name = "Ward 1", Upazila = dhanmondi, AreaType = AreaType.CityCorporation };
                var dhanmondiWard2 = new Ward { Name = "Ward 2", Upazila = dhanmondi, AreaType = AreaType.CityCorporation };
                var dhanmondiUnion1 = new Ward { Name = "Union 1", Upazila = dhanmondi, AreaType = AreaType.Union };
                var dhanmondiUnion2 = new Ward { Name = "Union 2", Upazila = dhanmondi, AreaType = AreaType.Union };
                var dhanmondiPourasava = new Ward { Name = "Pourasava 1", Upazila = dhanmondi, AreaType = AreaType.Pourasava };

                _context.Wards.AddRange(dhanmondiWard1, dhanmondiWard2, dhanmondiUnion1, dhanmondiUnion2, dhanmondiPourasava);
                await _context.SaveChangesAsync();

                // Sample villages/areas for Ward 1
                var area1Ward1 = new Village { Name = "Shankar", Ward = dhanmondiWard1 };
                var area2Ward1 = new Village { Name = "Kalabagan", Ward = dhanmondiWard1 };
                var area3Ward1 = new Village { Name = "Jigatola", Ward = dhanmondiWard1 };
                var area4Ward1 = new Village { Name = "Sobhanbag", Ward = dhanmondiWard1 };
                var area5Ward1 = new Village { Name = "Science Lab", Ward = dhanmondiWard1 };

                _context.Villages.AddRange(area1Ward1, area2Ward1, area3Ward1, area4Ward1, area5Ward1);
                await _context.SaveChangesAsync();

                return Ok("Location data seeded successfully.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error seeding location data");
                return StatusCode(500, $"Error seeding data: {ex.Message}");
            }
        }
    }
} 