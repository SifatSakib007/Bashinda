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
    }

    public class ApartmentOwnerProfileApiService : IApartmentOwnerProfileApiService
    {
        private readonly IApiService _apiService;
        private readonly HttpClient _httpClient;
        private readonly JsonSerializerOptions _jsonOptions;

        public ApartmentOwnerProfileApiService(IApiService apiService, IHttpClientFactory httpClientFactory)
        {
            _apiService = apiService;
            _httpClient = httpClientFactory.CreateClient("BashindaAPI");
            _jsonOptions = new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            };
        }

        public async Task<(bool Success, ApartmentOwnerProfileDto? Data, string[] Errors)> GetByIdAsync(int id, string token)
        {
            try
            {
                var response = await _apiService.GetAsync($"api/apartmentownerprofiles/{id}", token);
                
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
                var response = await _apiService.GetAsync("api/apartmentownerprofiles/current", token);
                
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

        public async Task<(bool Success, List<ApartmentOwnerProfileListDto> Data, string[] Errors)> GetAllAsync(string token)
        {
            try
            {
                var response = await _apiService.GetAsync("api/apartmentownerprofiles", token);
                
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
                var response = await _apiService.GetAsync("api/apartmentownerprofiles/pending", token);
                
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
                var response = await _apiService.PostAsync("api/apartmentownerprofiles", model, token);
                
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

        public async Task<(bool Success, string[] Errors)> UpdateAsync(int id, CreateApartmentOwnerProfileDto model, string token)
        {
            try
            {
                var response = await _apiService.PutAsync($"api/apartmentownerprofiles/{id}", model, token);
                
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
                var response = await _apiService.PatchAsync($"api/apartmentownerprofiles/{id}/approve", model, token);
                
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
                
                var response = await _httpClient.PostAsync($"api/apartmentownerprofiles/{id}/upload-image", formContent);
                
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
    }
} 