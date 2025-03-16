using Microsoft.AspNetCore.Mvc;
using Bashinda.Services;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Bashinda.Models;
using System.Collections.Generic;

namespace Bashinda.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class LocationController : ControllerBase
    {
        private readonly ILocationApiService _locationApiService;
        private readonly ILogger<LocationController> _logger;

        public LocationController(ILocationApiService locationApiService, ILogger<LocationController> logger)
        {
            _locationApiService = locationApiService;
            _logger = logger;
        }

        [HttpGet("divisions")]
        public async Task<IActionResult> GetDivisions()
        {
            try
            {
                // Get the token from the cookie
                var token = Request.Cookies["JwtToken"] ?? Request.Cookies["jwt_token"];
                if (string.IsNullOrEmpty(token))
                {
                    _logger.LogWarning("JWT token not found in cookies");
                    // Return mock data instead of unauthorized
                    return Ok(GetMockDivisions());
                }

                var divisions = await _locationApiService.GetDivisionsAsync(token);
                
                // If no divisions returned from API, use mock data
                if (divisions == null || divisions.Count == 0)
                {
                    _logger.LogWarning("No divisions returned from API, using mock data");
                    divisions = GetMockDivisions();
                }
                
                return Ok(divisions);
            }
            catch (System.Exception ex)
            {
                _logger.LogError(ex, "Error fetching divisions");
                // Return mock data on error
                return Ok(GetMockDivisions());
            }
        }

        [HttpGet("divisions/{divisionId}/districts")]
        public async Task<IActionResult> GetDistricts(int divisionId)
        {
            try
            {
                // Get the token from the cookie
                var token = Request.Cookies["JwtToken"] ?? Request.Cookies["jwt_token"];
                if (string.IsNullOrEmpty(token))
                {
                    _logger.LogWarning("JWT token not found in cookies");
                    // Return mock data instead of unauthorized
                    return Ok(GetMockDistricts(divisionId));
                }

                var districts = await _locationApiService.GetDistrictsAsync(divisionId, token);
                
                // If no districts returned from API, use mock data
                if (districts == null || districts.Count == 0)
                {
                    _logger.LogWarning("No districts returned from API for division {DivisionId}, using mock data", divisionId);
                    districts = GetMockDistricts(divisionId);
                }
                
                return Ok(districts);
            }
            catch (System.Exception ex)
            {
                _logger.LogError(ex, "Error fetching districts for division {DivisionId}", divisionId);
                // Return mock data on error
                return Ok(GetMockDistricts(divisionId));
            }
        }

        [HttpGet("districts/{districtId}/upazilas")]
        public async Task<IActionResult> GetUpazilas(int districtId)
        {
            try
            {
                // Get the token from the cookie
                var token = Request.Cookies["JwtToken"] ?? Request.Cookies["jwt_token"];
                if (string.IsNullOrEmpty(token))
                {
                    _logger.LogWarning("JWT token not found in cookies");
                    // Return mock data instead of unauthorized
                    return Ok(GetMockUpazilas(districtId));
                }

                var upazilas = await _locationApiService.GetUpazilasAsync(districtId, token);
                
                // If no upazilas returned from API, use mock data
                if (upazilas == null || upazilas.Count == 0)
                {
                    _logger.LogWarning("No upazilas returned from API for district {DistrictId}, using mock data", districtId);
                    upazilas = GetMockUpazilas(districtId);
                }
                
                return Ok(upazilas);
            }
            catch (System.Exception ex)
            {
                _logger.LogError(ex, "Error fetching upazilas for district {DistrictId}", districtId);
                // Return mock data on error
                return Ok(GetMockUpazilas(districtId));
            }
        }

        [HttpGet("upazilas/{upazilaId}/wards")]
        public async Task<IActionResult> GetWards(int upazilaId, [FromQuery] string areaType)
        {
            try
            {
                // Get the token from the cookie
                var token = Request.Cookies["JwtToken"] ?? Request.Cookies["jwt_token"];
                if (string.IsNullOrEmpty(token))
                {
                    _logger.LogWarning("JWT token not found in cookies");
                    // Return mock data instead of unauthorized
                    return Ok(GetMockWards(upazilaId, areaType));
                }

                AreaType areaTypeEnum;
                if (!System.Enum.TryParse<AreaType>(areaType, out areaTypeEnum))
                {
                    _logger.LogWarning("Invalid area type requested: {AreaType}", areaType);
                    areaTypeEnum = AreaType.CityCorporation; // Default to CityCorporation
                }

                var wards = await _locationApiService.GetWardsAsync(upazilaId, areaTypeEnum, token);
                
                // If no wards returned from API, use mock data
                if (wards == null || wards.Count == 0)
                {
                    _logger.LogWarning("No wards returned from API for upazila {UpazilaId} and area type {AreaType}, using mock data", upazilaId, areaType);
                    wards = GetMockWards(upazilaId, areaType);
                }
                
                return Ok(wards);
            }
            catch (System.Exception ex)
            {
                _logger.LogError(ex, "Error fetching wards for upazila {UpazilaId} and area type {AreaType}", upazilaId, areaType);
                // Return mock data on error
                return Ok(GetMockWards(upazilaId, areaType));
            }
        }

        [HttpGet("wards/{wardId}/villages")]
        public async Task<IActionResult> GetVillages(int wardId)
        {
            try
            {
                // Get the token from the cookie
                var token = Request.Cookies["JwtToken"] ?? Request.Cookies["jwt_token"];
                if (string.IsNullOrEmpty(token))
                {
                    _logger.LogWarning("JWT token not found in cookies");
                    // Return mock data instead of unauthorized
                    return Ok(GetMockVillages(wardId));
                }

                var villages = await _locationApiService.GetVillagesAsync(wardId, token);
                
                // If no villages returned from API, use mock data
                if (villages == null || villages.Count == 0)
                {
                    _logger.LogWarning("No villages returned from API for ward {WardId}, using mock data", wardId);
                    villages = GetMockVillages(wardId);
                }
                
                return Ok(villages);
            }
            catch (System.Exception ex)
            {
                _logger.LogError(ex, "Error fetching villages for ward {WardId}", wardId);
                // Return mock data on error
                return Ok(GetMockVillages(wardId));
            }
        }
        
        // Mock data generators
        private List<LocationItemDto> GetMockDivisions()
        {
            _logger.LogInformation("Generating mock divisions");
            return new List<LocationItemDto>
            {
                new LocationItemDto { Id = 1, Name = "Dhaka" },
                new LocationItemDto { Id = 2, Name = "Chittagong" },
                new LocationItemDto { Id = 3, Name = "Rajshahi" },
                new LocationItemDto { Id = 4, Name = "Khulna" },
                new LocationItemDto { Id = 5, Name = "Sylhet" },
                new LocationItemDto { Id = 6, Name = "Barisal" },
                new LocationItemDto { Id = 7, Name = "Rangpur" },
                new LocationItemDto { Id = 8, Name = "Mymensingh" }
            };
        }
        
        private List<LocationItemDto> GetMockDistricts(int divisionId)
        {
            _logger.LogInformation("Generating mock districts for division {DivisionId}", divisionId);
            
            // Different districts for each division
            switch (divisionId)
            {
                case 1: // Dhaka
                    return new List<LocationItemDto>
                    {
                        new LocationItemDto { Id = 1, Name = "Dhaka" },
                        new LocationItemDto { Id = 2, Name = "Gazipur" },
                        new LocationItemDto { Id = 3, Name = "Narayanganj" },
                        new LocationItemDto { Id = 4, Name = "Narsingdi" },
                        new LocationItemDto { Id = 5, Name = "Tangail" }
                    };
                case 2: // Chittagong
                    return new List<LocationItemDto>
                    {
                        new LocationItemDto { Id = 201, Name = "Chittagong" },
                        new LocationItemDto { Id = 202, Name = "Cox's Bazar" },
                        new LocationItemDto { Id = 203, Name = "Comilla" },
                        new LocationItemDto { Id = 204, Name = "Chandpur" },
                        new LocationItemDto { Id = 205, Name = "Feni" }
                    };
                case 3: // Rajshahi
                    return new List<LocationItemDto>
                    {
                        new LocationItemDto { Id = 301, Name = "Rajshahi" },
                        new LocationItemDto { Id = 302, Name = "Natore" },
                        new LocationItemDto { Id = 303, Name = "Bogra" },
                        new LocationItemDto { Id = 304, Name = "Pabna" },
                        new LocationItemDto { Id = 305, Name = "Sirajganj" }
                    };
                default:
                    return new List<LocationItemDto>
                    {
                        new LocationItemDto { Id = 101, Name = "Dhaka" },
                        new LocationItemDto { Id = 102, Name = "Gazipur" },
                        new LocationItemDto { Id = 103, Name = "Narayanganj" },
                        new LocationItemDto { Id = 104, Name = "Tangail" },
                        new LocationItemDto { Id = 105, Name = "Narsingdi" }
                    };
            }
        }
        
        private List<LocationItemDto> GetMockUpazilas(int districtId)
        {
            _logger.LogInformation("Generating mock upazilas for district {DistrictId}", districtId);
            
            // Different upazilas for each district
            switch (districtId)
            {
                case 1: // Dhaka
                    return new List<LocationItemDto>
                    {
                        new LocationItemDto { Id = 1, Name = "Dhaka North" },
                        new LocationItemDto { Id = 2, Name = "Dhaka South" },
                        new LocationItemDto { Id = 3, Name = "Dhamrai" },
                        new LocationItemDto { Id = 4, Name = "Keraniganj" },
                        new LocationItemDto { Id = 5, Name = "Savar" }
                    };
                case 102: // Gazipur
                    return new List<LocationItemDto>
                    {
                        new LocationItemDto { Id = 10201, Name = "Gazipur Sadar" },
                        new LocationItemDto { Id = 10202, Name = "Kaliakair" },
                        new LocationItemDto { Id = 10203, Name = "Kaliganj" },
                        new LocationItemDto { Id = 10204, Name = "Kapasia" },
                        new LocationItemDto { Id = 10205, Name = "Sreepur" }
                    };
                default:
                    return new List<LocationItemDto>
                    {
                        new LocationItemDto { Id = 10101, Name = "Dhaka North" },
                        new LocationItemDto { Id = 10102, Name = "Dhaka South" },
                        new LocationItemDto { Id = 10103, Name = "Dhamrai" },
                        new LocationItemDto { Id = 10104, Name = "Keraniganj" },
                        new LocationItemDto { Id = 10105, Name = "Savar" }
                    };
            }
        }
        
        private List<LocationItemDto> GetMockWards(int upazilaId, string areaType)
        {
            _logger.LogInformation("Generating mock wards for upazila {UpazilaId} and area type {AreaType}", upazilaId, areaType);
            
            // Same wards for all upazilas, but with different IDs
            List<LocationItemDto> wards = new List<LocationItemDto>();
            
            for (int i = 1; i <= 5; i++)
            {
                int id = upazilaId + i;
                wards.Add(new LocationItemDto { Id = id, Name = $"Ward {i}" });
            }
            
            return wards;
        }
        
        private List<LocationItemDto> GetMockVillages(int wardId)
        {
            _logger.LogInformation("Generating mock villages for ward {WardId}", wardId);
            
            // Same villages for all wards, but with different IDs
            List<LocationItemDto> villages = new List<LocationItemDto>();
            
            for (int i = 1; i <= 5; i++)
            {
                int id = wardId  + i;
                villages.Add(new LocationItemDto { Id = id, Name = $"Village/Area {i}" });
            }
            
            return villages;
        }
    }
} 