using Bashinda.Models;
using Bashinda.ViewModels;
using System.Text.Json;

namespace Bashinda.Services
{
    public interface ILocationApiService
    {
        Task<List<LocationItemDto>> GetDivisionsAsync(string token);
        Task<List<LocationItemDto>> GetDistrictsAsync(int divisionId, string token);
        Task<List<LocationItemDto>> GetUpazilasAsync(int districtId, string token);
        Task<List<LocationItemDto>> GetWardsAsync(int upazilaId, Bashinda.Models.AreaType areaType, string token);
        Task<List<LocationItemDto>> GetVillagesAsync(int wardId, string token);
        Task<Bashinda.Models.AreaType> GetAreaTypeAsync(string localityTypeName);
    }

    public class LocationApiService : ILocationApiService
    {
        private readonly IApiService _apiService;
        private readonly JsonSerializerOptions _jsonOptions;
        private readonly ILogger<LocationApiService> _logger;

        public LocationApiService(IApiService apiService, ILogger<LocationApiService> logger)
        {
            _apiService = apiService;
            _logger = logger;
            _jsonOptions = new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            };
        }

        public async Task<List<LocationItemDto>> GetDivisionsAsync(string token)
        {
            try
            {
                var response = await _apiService.GetAsync("api/location/divisions", token);
                
                if (response.IsSuccessStatusCode)
                {
                    var content = await response.Content.ReadAsStringAsync();
                    _logger.LogInformation("Divisions API response content: {Content}", content);
                    
                    var result = JsonSerializer.Deserialize<ApiResponse<List<LocationItemDto>>>(content, _jsonOptions);
                    
                    if (result != null && result.Success)
                    {
                        return result.Data ?? new List<LocationItemDto>();
                    }
                    
                    _logger.LogWarning("Failed to get divisions: {Errors}", string.Join(", ", result?.Errors ?? Array.Empty<string>()));
                }
                else
                {
                    var errorContent = await response.Content.ReadAsStringAsync();
                    _logger.LogWarning("Failed to fetch divisions: {StatusCode}, Response: {Response}", 
                        response.StatusCode, errorContent);
                }
                
                return new List<LocationItemDto>();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching divisions");
                return new List<LocationItemDto>();
            }
        }

        public async Task<List<LocationItemDto>> GetDistrictsAsync(int divisionId, string token)
        {
            try
            {
                var response = await _apiService.GetAsync($"api/location/divisions/{divisionId}/districts", token);
                
                if (response.IsSuccessStatusCode)
                {
                    var content = await response.Content.ReadAsStringAsync();
                    _logger.LogInformation("Districts API response content: {Content}", content);
                    
                    var result = JsonSerializer.Deserialize<ApiResponse<List<LocationItemDto>>>(content, _jsonOptions);
                    
                    if (result != null && result.Success)
                    {
                        return result.Data ?? new List<LocationItemDto>();
                    }
                    
                    _logger.LogWarning("Failed to get districts: {Errors}", string.Join(", ", result?.Errors ?? Array.Empty<string>()));
                }
                else
                {
                    var errorContent = await response.Content.ReadAsStringAsync();
                    _logger.LogWarning("Failed to fetch districts: {StatusCode}, Response: {Response}", 
                        response.StatusCode, errorContent);
                }
                
                return new List<LocationItemDto>();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching districts for division {DivisionId}", divisionId);
                return new List<LocationItemDto>();
            }
        }

        public async Task<List<LocationItemDto>> GetUpazilasAsync(int districtId, string token)
        {
            try
            {
                var response = await _apiService.GetAsync($"api/location/districts/{districtId}/upazilas", token);
                
                if (response.IsSuccessStatusCode)
                {
                    var content = await response.Content.ReadAsStringAsync();
                    _logger.LogInformation("Upazilas API response content: {Content}", content);
                    
                    var result = JsonSerializer.Deserialize<ApiResponse<List<LocationItemDto>>>(content, _jsonOptions);
                    
                    if (result != null && result.Success)
                    {
                        return result.Data ?? new List<LocationItemDto>();
                    }
                    
                    _logger.LogWarning("Failed to get upazilas: {Errors}", string.Join(", ", result?.Errors ?? Array.Empty<string>()));
                }
                else
                {
                    var errorContent = await response.Content.ReadAsStringAsync();
                    _logger.LogWarning("Failed to fetch upazilas: {StatusCode}, Response: {Response}", 
                        response.StatusCode, errorContent);
                }
                
                return new List<LocationItemDto>();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching upazilas for district {DistrictId}", districtId);
                return new List<LocationItemDto>();
            }
        }

        public async Task<List<LocationItemDto>> GetWardsAsync(int upazilaId, Bashinda.Models.AreaType areaType, string token)
        {
            try
            {
                var response = await _apiService.GetAsync($"api/location/upazilas/{upazilaId}/wards?areaType={areaType}", token);
                
                if (response.IsSuccessStatusCode)
                {
                    var content = await response.Content.ReadAsStringAsync();
                    _logger.LogInformation("Wards API response content: {Content}", content);
                    
                    var result = JsonSerializer.Deserialize<ApiResponse<List<LocationItemDto>>>(content, _jsonOptions);
                    
                    if (result != null && result.Success)
                    {
                        return result.Data ?? new List<LocationItemDto>();
                    }
                    
                    _logger.LogWarning("Failed to get wards: {Errors}", string.Join(", ", result?.Errors ?? Array.Empty<string>()));
                }
                else
                {
                    var errorContent = await response.Content.ReadAsStringAsync();
                    _logger.LogWarning("Failed to fetch wards: {StatusCode}, Response: {Response}", 
                        response.StatusCode, errorContent);
                }
                
                return new List<LocationItemDto>();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching wards for upazila {UpazilaId} and area type {AreaType}", upazilaId, areaType);
                return new List<LocationItemDto>();
            }
        }

        public async Task<List<LocationItemDto>> GetVillagesAsync(int wardId, string token)
        {
            try
            {
                var response = await _apiService.GetAsync($"api/location/wards/{wardId}/villages", token);
                
                if (response.IsSuccessStatusCode)
                {
                    var content = await response.Content.ReadAsStringAsync();
                    _logger.LogInformation("Villages API response content: {Content}", content);
                    
                    var result = JsonSerializer.Deserialize<ApiResponse<List<LocationItemDto>>>(content, _jsonOptions);
                    
                    if (result != null && result.Success)
                    {
                        return result.Data ?? new List<LocationItemDto>();
                    }
                    
                    _logger.LogWarning("Failed to get villages: {Errors}", string.Join(", ", result?.Errors ?? Array.Empty<string>()));
                }
                else
                {
                    var errorContent = await response.Content.ReadAsStringAsync();
                    _logger.LogWarning("Failed to fetch villages: {StatusCode}, Response: {Response}", 
                        response.StatusCode, errorContent);
                }
                
                return new List<LocationItemDto>();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching villages for ward {WardId}", wardId);
                return new List<LocationItemDto>();
            }
        }

        public Task<Bashinda.Models.AreaType> GetAreaTypeAsync(string localityTypeName)
        {
            return Task.FromResult(localityTypeName.ToLower() switch
            {
                "citycorporation" => Bashinda.Models.AreaType.CityCorporation,
                "pourashava" => Bashinda.Models.AreaType.Pourashava,
                "union" => Bashinda.Models.AreaType.Union,
                _ => Bashinda.Models.AreaType.CityCorporation // Default
            });
        }
    }

    public class LocationItemDto
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
    }
} 