using Bashinda.ViewModels;
using System.Text.Json;

namespace Bashinda.Services
{
    public interface IApartmentApiService
    {
        Task<(bool Success, ApartmentDto? Data, string[] Errors)> GetByIdAsync(int id, string? token = null);
        Task<(bool Success, List<ApartmentListDto> Data, string[] Errors)> GetAllAsync(string? token = null);
        Task<(bool Success, List<ApartmentListDto> Data, string[] Errors)> GetAvailableAsync(string? token = null);
        Task<(bool Success, List<ApartmentListDto> Data, string[] Errors)> GetOwnerApartmentsAsync(string token);
        Task<(bool Success, ApartmentDto? Data, string[] Errors)> CreateAsync(CreateApartmentDto model, string token);
        Task<(bool Success, string[] Errors)> UpdateAsync(int id, CreateApartmentDto model, string token);
        Task<(bool Success, string[] Errors)> DeleteAsync(int id, string token);
        Task<(bool Success, string? ImagePath, string[] Errors)> UploadImageAsync(int id, IFormFile file, string token);
    }

    public class ApartmentApiService : IApartmentApiService
    {
        private readonly IApiService _apiService;
        private readonly HttpClient _httpClient;
        private readonly JsonSerializerOptions _jsonOptions;

        public ApartmentApiService(IApiService apiService, IHttpClientFactory httpClientFactory)
        {
            _apiService = apiService;
            _httpClient = httpClientFactory.CreateClient("BashindaAPI");
            _jsonOptions = new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            };
        }

        public async Task<(bool Success, ApartmentDto? Data, string[] Errors)> GetByIdAsync(int id, string? token = null)
        {
            try
            {
                var response = await _apiService.GetAsync($"api/apartments/{id}", token);
                
                if (response.IsSuccessStatusCode)
                {
                    var content = await response.Content.ReadAsStringAsync();
                    var result = JsonSerializer.Deserialize<ApiResponse<ApartmentDto>>(content, _jsonOptions);
                    
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

        public async Task<(bool Success, List<ApartmentListDto> Data, string[] Errors)> GetAllAsync(string? token = null)
        {
            try
            {
                var response = await _apiService.GetAsync("api/apartments", token);
                
                if (response.IsSuccessStatusCode)
                {
                    var content = await response.Content.ReadAsStringAsync();
                    var result = JsonSerializer.Deserialize<ApiResponse<List<ApartmentListDto>>>(content, _jsonOptions);
                    
                    if (result != null && result.Success)
                    {
                        return (true, result.Data ?? new List<ApartmentListDto>(), Array.Empty<string>());
                    }
                    
                    return (false, new List<ApartmentListDto>(), result?.Errors ?? new[] { "Unknown error occurred" });
                }
                
                return (false, new List<ApartmentListDto>(), new[] { $"API request failed with status code {response.StatusCode}" });
            }
            catch (Exception ex)
            {
                return (false, new List<ApartmentListDto>(), new[] { $"Exception occurred: {ex.Message}" });
            }
        }

        public async Task<(bool Success, List<ApartmentListDto> Data, string[] Errors)> GetAvailableAsync(string? token = null)
        {
            try
            {
                var response = await _apiService.GetAsync("api/apartments/available", token);
                
                if (response.IsSuccessStatusCode)
                {
                    var content = await response.Content.ReadAsStringAsync();
                    var result = JsonSerializer.Deserialize<ApiResponse<List<ApartmentListDto>>>(content, _jsonOptions);
                    
                    if (result != null && result.Success)
                    {
                        return (true, result.Data ?? new List<ApartmentListDto>(), Array.Empty<string>());
                    }
                    
                    return (false, new List<ApartmentListDto>(), result?.Errors ?? new[] { "Unknown error occurred" });
                }
                
                return (false, new List<ApartmentListDto>(), new[] { $"API request failed with status code {response.StatusCode}" });
            }
            catch (Exception ex)
            {
                return (false, new List<ApartmentListDto>(), new[] { $"Exception occurred: {ex.Message}" });
            }
        }

        public async Task<(bool Success, List<ApartmentListDto> Data, string[] Errors)> GetOwnerApartmentsAsync(string token)
        {
            try
            {
                var response = await _apiService.GetAsync("api/apartments/owner", token);
                
                if (response.IsSuccessStatusCode)
                {
                    var content = await response.Content.ReadAsStringAsync();
                    var result = JsonSerializer.Deserialize<ApiResponse<List<ApartmentListDto>>>(content, _jsonOptions);
                    
                    if (result != null && result.Success)
                    {
                        return (true, result.Data ?? new List<ApartmentListDto>(), Array.Empty<string>());
                    }
                    
                    return (false, new List<ApartmentListDto>(), result?.Errors ?? new[] { "Unknown error occurred" });
                }
                
                return (false, new List<ApartmentListDto>(), new[] { $"API request failed with status code {response.StatusCode}" });
            }
            catch (Exception ex)
            {
                return (false, new List<ApartmentListDto>(), new[] { $"Exception occurred: {ex.Message}" });
            }
        }

        public async Task<(bool Success, ApartmentDto? Data, string[] Errors)> CreateAsync(CreateApartmentDto model, string token)
        {
            try
            {
                var response = await _apiService.PostAsync("api/apartments", model, token);
                
                if (response.IsSuccessStatusCode)
                {
                    var content = await response.Content.ReadAsStringAsync();
                    var result = JsonSerializer.Deserialize<ApiResponse<ApartmentDto>>(content, _jsonOptions);
                    
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

        public async Task<(bool Success, string[] Errors)> UpdateAsync(int id, CreateApartmentDto model, string token)
        {
            try
            {
                var response = await _apiService.PutAsync($"api/apartments/{id}", model, token);
                
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

        public async Task<(bool Success, string[] Errors)> DeleteAsync(int id, string token)
        {
            try
            {
                var response = await _apiService.DeleteAsync($"api/apartments/{id}", token);
                
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

        public async Task<(bool Success, string? ImagePath, string[] Errors)> UploadImageAsync(int id, IFormFile file, string token)
        {
            try
            {
                using var formContent = new MultipartFormDataContent();
                using var fileContent = new StreamContent(file.OpenReadStream());
                formContent.Add(fileContent, "file", file.FileName);

                // Set authorization header
                _httpClient.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);
                
                var response = await _httpClient.PostAsync($"api/apartments/{id}/upload-image", formContent);
                
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