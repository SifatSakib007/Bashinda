using Bashinda.ViewModels;
using System.Text.Json;

namespace Bashinda.Services
{
    public interface IApartmentOwnerProfileApiService
    {
        Task<(bool Success, ApartmentOwnerProfileDto? Data, string[] Errors)> GetByIdAsync(int id, string token);
        Task<(bool Success, ApartmentOwnerProfileDto? Data, string[] Errors)> GetCurrentAsync(string token);
        Task<(bool Success, List<ApartmentOwnerProfileListDto> Data, string[] Errors)> GetAllAsync(string token);
        Task<(bool Success, List<ApartmentOwnerProfileListDto> Data, string[] Errors)> GetPendingAsync(string token);
        Task<(bool Success, ApartmentOwnerProfileDto? Data, string[] Errors)> CreateAsync(CreateApartmentOwnerProfileDto model, string token);
        Task<(bool Success, string[] Errors)> UpdateAsync(int id, CreateApartmentOwnerProfileDto model, string token);
        Task<(bool Success, string[] Errors)> ApproveAsync(int id, ApproveApartmentOwnerProfileDto model, string token);
        Task<(bool Success, string? ImagePath, string[] Errors)> UploadImageAsync(int id, string imageType, IFormFile file, string token);
        Task<(bool Success, ApartmentOwnerProfileDto? Profile, string[] Errors)> OwnerProfilesInfo(string userId, string token);
    }

    public class ApartmentOwnerProfileApiService : IApartmentOwnerProfileApiService
    {
        private readonly IApiService _apiService;
        private readonly HttpClient _httpClient;
        private readonly JsonSerializerOptions _jsonOptions;
        private readonly ILogger<ApartmentOwnerProfileApiService> _logger;

        public ApartmentOwnerProfileApiService(IApiService apiService, IHttpClientFactory httpClientFactory, ILogger<ApartmentOwnerProfileApiService> logger)
        {
            _apiService = apiService;
            _httpClient = httpClientFactory.CreateClient("BashindaAPI");
            _jsonOptions = new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            };
            _logger = logger;
        }

        public async Task<(bool Success, ApartmentOwnerProfileDto? Data, string[] Errors)> GetByIdAsync(int id, string token)
        {
            try
            {
                var response = await _apiService.GetAsync($"api/ApartmentOwnerProfiles/{id}", token);
                
                if (response.IsSuccessStatusCode)
                {
                    var content = await response.Content.ReadAsStringAsync();
                    var result = JsonSerializer.Deserialize<ApiResponse<ApartmentOwnerProfileDto>>(content, _jsonOptions);
                    
                    if (result != null && result.Success)
                    {
                        return (true, result.Data, Array.Empty<string>());
                    }
                    
                    return (false, null, result?.Errors ?? new[] { "Unknown error occurred" });
                }
                
                return (false, null, new[] { $"API request failed with status code {response.StatusCode}" });
            }
            catch (Exception ex)
            {
                return (false, null, new[] { $"Exception occurred: {ex.Message}" });
            }
        }

        public async Task<(bool Success, ApartmentOwnerProfileDto? Data, string[] Errors)> GetCurrentAsync(string token)
        {
            try
            {
                var response = await _apiService.GetAsync("api/ApartmentOwnerProfiles/current", token);
                _logger.LogInformation("API response status code: {StatusCode}", response.StatusCode);

                // Handle 404 specifically
                if (response.StatusCode == System.Net.HttpStatusCode.NotFound)
                {
                    _logger.LogWarning("Profile not found for the current user.");
                    return (false, null, new[] { "Profile not found" });
                }

                if (!response.IsSuccessStatusCode)
                {
                    _logger.LogError("API request failed with status code: {StatusCode}", response.StatusCode);
                    return (false, null, new[] { $"API request failed: {response.StatusCode}" });
                }

                if (response.IsSuccessStatusCode)
                {
                    _logger.LogInformation("API request succeeded.");
                    var content = await response.Content.ReadAsStringAsync();
                    _logger.LogInformation("API response content: {Content}", content);

                    try
                    {
                        // Try to deserialize the content directly as RenterProfileDto first
                        var directResult = JsonSerializer.Deserialize<ApartmentOwnerProfileDto>(content, _jsonOptions);
                        _logger.LogInformation("Direct deserialization result: {Result}", directResult);
                        if (directResult != null)
                        {
                            _logger.LogInformation("Direct deserialization succeeded.");
                            return (true, directResult, Array.Empty<string>());
                        }
                    }
                    catch
                    {
                        // If direct deserialization fails, try with ApiResponse wrapper
                        try
                        {
                            var result = JsonSerializer.Deserialize<ApiResponse<ApartmentOwnerProfileDto>>(content, _jsonOptions);
                            _logger.LogInformation("ApiResponse deserialization result: {Result}", result);
                            if (result != null && result.Success)
                            {
                                _logger.LogInformation("ApiResponse deserialization succeeded.");
                                return (true, result.Data, Array.Empty<string>());
                            }
                            else if (result != null)
                            {
                                _logger.LogWarning("ApiResponse deserialization failed with errors: {Errors}", result.Errors);
                                return (false, null, result.Errors ?? new[] { "API returned failure response" });
                            }
                        }
                        catch (Exception ex)
                        {
                            _logger.LogError(ex, "Failed to deserialize response as ApiResponse<ApartmentOwnerProfileDto>");
                        }
                    }
                    _logger.LogWarning("Failed to deserialize API response.");
                    return (false, null, new[] { "Unable to parse API response" });
                }
                _logger.LogError("API request failed with status code: {StatusCode}", response.StatusCode);
                return (false, null, new[] { $"API request failed with status code {response.StatusCode}" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Exception occurred while retrieving current profile");
                return (false, null, new[] { $"Exception occurred: {ex.Message}" });
            }
        }

        public async Task<(bool Success, List<ApartmentOwnerProfileListDto> Data, string[] Errors)> GetAllAsync(string token)
        {
            try
            {
                var response = await _apiService.GetAsync("api/ApartmentOwnerProfiles", token);
                
                if (response.IsSuccessStatusCode)
                {
                    var content = await response.Content.ReadAsStringAsync();
                    var result = JsonSerializer.Deserialize<ApiResponse<List<ApartmentOwnerProfileListDto>>>(content, _jsonOptions);
                    
                    if (result != null && result.Success)
                    {
                        return (true, result.Data ?? new List<ApartmentOwnerProfileListDto>(), Array.Empty<string>());
                    }
                    
                    return (false, new List<ApartmentOwnerProfileListDto>(), result?.Errors ?? new[] { "Unknown error occurred" });
                }
                
                return (false, new List<ApartmentOwnerProfileListDto>(), new[] { $"API request failed with status code {response.StatusCode}" });
            }
            catch (Exception ex)
            {
                return (false, new List<ApartmentOwnerProfileListDto>(), new[] { $"Exception occurred: {ex.Message}" });
            }
        }

        public async Task<(bool Success, List<ApartmentOwnerProfileListDto> Data, string[] Errors)> GetPendingAsync(string token)
        {
            try
            {
                var response = await _apiService.GetAsync("api/ApartmentOwnerProfiles/pending", token);
                
                if (response.IsSuccessStatusCode)
                {
                    var content = await response.Content.ReadAsStringAsync();
                    var result = JsonSerializer.Deserialize<ApiResponse<List<ApartmentOwnerProfileListDto>>>(content, _jsonOptions);
                    
                    if (result != null && result.Success)
                    {
                        return (true, result.Data ?? new List<ApartmentOwnerProfileListDto>(), Array.Empty<string>());
                    }
                    
                    return (false, new List<ApartmentOwnerProfileListDto>(), result?.Errors ?? new[] { "Unknown error occurred" });
                }
                
                return (false, new List<ApartmentOwnerProfileListDto>(), new[] { $"API request failed with status code {response.StatusCode}" });
            }
            catch (Exception ex)
            {
                return (false, new List<ApartmentOwnerProfileListDto>(), new[] { $"Exception occurred: {ex.Message}" });
            }
        }

        public async Task<(bool Success, ApartmentOwnerProfileDto? Data, string[] Errors)> CreateAsync(CreateApartmentOwnerProfileDto model, string token)
        {
            try
            {
                var response = await _apiService.PostAsync("api/ApartmentOwnerProfiles/create", model, token);
                _logger.LogInformation($"response: {response}");
                if (response.IsSuccessStatusCode)
                {
                    _logger.LogInformation($"{response.StatusCode}");
                    var content = await response.Content.ReadAsStringAsync();
                    
                    _logger.LogInformation(content.ToString());
                    var result = JsonSerializer.Deserialize<ApiResponse<ApartmentOwnerProfileDto>>(content, _jsonOptions);
                    _logger.LogInformation($"result: {result}");
                    if (result != null && result.Success)
                    {
                        return (true, result.Data, Array.Empty<string>());
                    }
                    
                    return (false, null, result?.Errors ?? new[] { "Unknown error occurred" });
                }
                
                return (false, null, new[] { $"API request failed with status code {response.StatusCode}" });
            }
            catch (Exception ex)
            {
                return (false, null, new[] { $"Exception occurred: {ex.Message}" });
            }
        }

        public async Task<(bool Success, string[] Errors)> UpdateAsync(int id, CreateApartmentOwnerProfileDto model, string token)
        {
            try
            {
                var response = await _apiService.PutAsync($"api/ApartmentOwnerProfiles/{id}", model, token);
                
                if (response.IsSuccessStatusCode)
                {
                    var content = await response.Content.ReadAsStringAsync();
                    var result = JsonSerializer.Deserialize<ApiResponse<object>>(content, _jsonOptions);
                    
                    if (result != null && result.Success)
                    {
                        return (true, Array.Empty<string>());
                    }
                    
                    return (false, result?.Errors ?? new[] { "Unknown error occurred" });
                }
                
                return (false, new[] { $"API request failed with status code {response.StatusCode}" });
            }
            catch (Exception ex)
            {
                return (false, new[] { $"Exception occurred: {ex.Message}" });
            }
        }

        public async Task<(bool Success, string[] Errors)> ApproveAsync(int id, ApproveApartmentOwnerProfileDto model, string token)
        {
            try
            {
                var response = await _apiService.PatchAsync($"api/ApartmentOwnerProfiles/{id}/approve", model, token);
                
                if (response.IsSuccessStatusCode)
                {
                    var content = await response.Content.ReadAsStringAsync();
                    var result = JsonSerializer.Deserialize<ApiResponse<object>>(content, _jsonOptions);
                    
                    if (result != null && result.Success)
                    {
                        return (true, Array.Empty<string>());
                    }
                    
                    return (false, result?.Errors ?? new[] { "Unknown error occurred" });
                }
                
                return (false, new[] { $"API request failed with status code {response.StatusCode}" });
            }
            catch (Exception ex)
            {
                return (false, new[] { $"Exception occurred: {ex.Message}" });
            }
        }

        public async Task<(bool Success, string? ImagePath, string[] Errors)> UploadImageAsync(int id, string imageType, IFormFile file, string token)
        {
            try
            {
                using var formContent = new MultipartFormDataContent();
                using var fileContent = new StreamContent(file.OpenReadStream());
                formContent.Add(fileContent, "file", file.FileName);
                formContent.Add(new StringContent(imageType), "ImageType");

                // Set authorization header
                _httpClient.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);
                
                var response = await _httpClient.PostAsync($"api/ApartmentOwnerProfiles/{id}/upload-image", formContent);
                
                if (response.IsSuccessStatusCode)
                {
                    var content = await response.Content.ReadAsStringAsync();
                    var result = JsonSerializer.Deserialize<ApiResponse<string>>(content, _jsonOptions);
                    
                    if (result != null && result.Success)
                    {
                        return (true, result.Data, Array.Empty<string>());
                    }
                    
                    return (false, null, result?.Errors ?? new[] { "Unknown error occurred" });
                }
                
                return (false, null, new[] { $"API request failed with status code {response.StatusCode}" });
            }
            catch (Exception ex)
            {
                return (false, null, new[] { $"Exception occurred: {ex.Message}" });
            }
        }

        public async Task<(bool Success, ApartmentOwnerProfileDto? Profile, string[] Errors)> OwnerProfilesInfo(string userId, string token)
        {
            try
            {
                var response = await _apiService.GetAsync($"api/ApartmentOwnerProfiles/owners/{userId}", token);

                if (response.IsSuccessStatusCode)
                {
                    var content = await response.Content.ReadAsStringAsync();
                    var profile = JsonSerializer.Deserialize<ApartmentOwnerProfileDto>(content, _jsonOptions);

                    if (profile != null)
                    {
                        return (true, profile, Array.Empty<string>());
                    }

                    return (false, null, new[] { "Failed to deserialize response." });
                }

                return (false, null, new[] { $"API request failed with status code {response.StatusCode}" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving renter profile for user ID: {UserId}", userId);
                return (false, null, new[] { $"Exception occurred: {ex.Message}" });
            }

        }
    }
} 