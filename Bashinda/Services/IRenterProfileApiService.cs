using Bashinda.ViewModels;
using System.Text.Json;
using Microsoft.Extensions.Logging;

namespace Bashinda.Services
{
    public interface IRenterProfileApiService
    {
        Task<(bool Success, RenterProfileDto? Data, string[] Errors)> GetByIdAsync(int id, string token);
        Task<(bool Success, RenterProfileDto? Data, string[] Errors)> GetCurrentAsync(string token);
        Task<(bool Success, List<RenterProfileListDto> Data, string[] Errors)> GetAllAsync(string token);
        Task<(bool Success, List<RenterProfileListDto> Data, string[] Errors)> GetPendingAsync(string token);
        Task<(bool Success, RenterProfileDto? Data, string[] Errors)> CreateAsync(CreateRenterProfileDto model, string token);
        Task<(bool Success, string[] Errors)> UpdateAsync(int id, CreateRenterProfileDto model, string token);
        Task<(bool Success, string[] Errors)> ApproveAsync(int id, ApproveRenterProfileDto model, string token);
        Task<(bool Success, string? ImagePath, string[] Errors)> UploadImageAsync(int id, string imageType, IFormFile file, string token);
    }

    public class RenterProfileApiService : IRenterProfileApiService
    {
        private readonly IApiService _apiService;
        private readonly HttpClient _httpClient;
        private readonly JsonSerializerOptions _jsonOptions;
        private readonly ILogger<RenterProfileApiService> _logger;

        public RenterProfileApiService(IApiService apiService, IHttpClientFactory httpClientFactory, ILogger<RenterProfileApiService> logger)
        {
            _apiService = apiService;
            _httpClient = httpClientFactory.CreateClient("BashindaAPI");
            _jsonOptions = new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            };
            _logger = logger;
        }

        public async Task<(bool Success, RenterProfileDto? Data, string[] Errors)> GetByIdAsync(int id, string token)
        {
            try
            {
                var response = await _apiService.GetAsync($"api/renterprofiles/{id}", token);

                if (response.IsSuccessStatusCode)
                {
                    var content = await response.Content.ReadAsStringAsync();
                    var result = JsonSerializer.Deserialize<ApiResponse<RenterProfileDto>>(content, _jsonOptions);

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

        public async Task<(bool Success, RenterProfileDto? Data, string[] Errors)> GetCurrentAsync(string token)
        {
            try
            {
                var response = await _apiService.GetAsync("api/renterprofiles/current", token);

                if (response.IsSuccessStatusCode)
                {
                    var content = await response.Content.ReadAsStringAsync();
                    _logger.LogInformation("API response content: {Content}", content);

                    try
                    {
                        // Try to deserialize the content directly as RenterProfileDto first
                        var directResult = JsonSerializer.Deserialize<RenterProfileDto>(content, _jsonOptions);
                        if (directResult != null)
                        {
                            return (true, directResult, Array.Empty<string>());
                        }
                    }
                    catch
                    {
                        // If direct deserialization fails, try with ApiResponse wrapper
                        try
                        {
                            var result = JsonSerializer.Deserialize<ApiResponse<RenterProfileDto>>(content, _jsonOptions);
                            if (result != null && result.Success)
                            {
                                return (true, result.Data, Array.Empty<string>());
                            }
                            else if (result != null)
                            {
                                return (false, null, result.Errors ?? new[] { "API returned failure response" });
                            }
                        }
                        catch (Exception ex)
                        {
                            _logger.LogError(ex, "Failed to deserialize response as ApiResponse<RenterProfileDto>");
                        }
                    }

                    return (false, null, new[] { "Unable to parse API response" });
                }

                return (false, null, new[] { $"API request failed with status code {response.StatusCode}" });
            }
            catch (Exception ex)
            {
                return (false, null, new[] { $"Exception occurred: {ex.Message}" });
            }
        }

        public async Task<(bool Success, List<RenterProfileListDto> Data, string[] Errors)> GetAllAsync(string token)
        {
            try
            {
                var response = await _apiService.GetAsync("api/renterprofiles", token);

                if (response.IsSuccessStatusCode)
                {
                    var content = await response.Content.ReadAsStringAsync();
                    var result = JsonSerializer.Deserialize<ApiResponse<List<RenterProfileListDto>>>(content, _jsonOptions);

                    if (result != null && result.Success)
                    {
                        return (true, result.Data ?? new List<RenterProfileListDto>(), Array.Empty<string>());
                    }

                    return (false, new List<RenterProfileListDto>(), result?.Errors ?? new[] { "Unknown error occurred" });
                }

                return (false, new List<RenterProfileListDto>(), new[] { $"API request failed with status code {response.StatusCode}" });
            }
            catch (Exception ex)
            {
                return (false, new List<RenterProfileListDto>(), new[] { $"Exception occurred: {ex.Message}" });
            }
        }

        public async Task<(bool Success, List<RenterProfileListDto> Data, string[] Errors)> GetPendingAsync(string token)
        {
            try
            {
                var response = await _apiService.GetAsync("api/renterprofiles/pending", token);

                if (response.IsSuccessStatusCode)
                {
                    var content = await response.Content.ReadAsStringAsync();
                    var result = JsonSerializer.Deserialize<ApiResponse<List<RenterProfileListDto>>>(content, _jsonOptions);

                    if (result != null && result.Success)
                    {
                        return (true, result.Data ?? new List<RenterProfileListDto>(), Array.Empty<string>());
                    }

                    return (false, new List<RenterProfileListDto>(), result?.Errors ?? new[] { "Unknown error occurred" });
                }

                return (false, new List<RenterProfileListDto>(), new[] { $"API request failed with status code {response.StatusCode}" });
            }
            catch (Exception ex)
            {
                return (false, new List<RenterProfileListDto>(), new[] { $"Exception occurred: {ex.Message}" });
            }
        }

        public async Task<(bool Success, RenterProfileDto? Data, string[] Errors)> CreateAsync(CreateRenterProfileDto model, string token)
        {
            try
            {
                var apiModel = new
                {
                    IsAdult = model.IsAdult,
                    NationalId = model.IsAdult ? model.NationalId : null,
                    BirthRegistrationNo = !model.IsAdult ? model.BirthRegistrationNo : null,
                    DateOfBirth = model.DateOfBirth,
                    FullName = model.FullName,
                    FatherName = model.FatherName,
                    MotherName = model.MotherName,
                    Nationality = model.Nationality.ToString(),
                    BloodGroup = model.BloodGroup.ToString(),
                    Profession = model.Profession.ToString(),
                    Gender = model.Gender.ToString(),
                    MobileNo = model.MobileNo,
                    Email = model.Email,
                    Division = model.Division,
                    District = model.District,
                    Upazila = model.Upazila,
                    AreaType = model.AreaType,
                    Ward = model.Ward,
                    Village = model.Village,
                    PostCode = model.PostCode,
                    HoldingNo = model.HoldingNo
                };

                var response = await _apiService.PostAsync("api/renterprofiles", apiModel, token);

                if (response.IsSuccessStatusCode)
                {
                    var content = await response.Content.ReadAsStringAsync();
                    var result = JsonSerializer.Deserialize<ApiResponse<RenterProfileDto>>(content, _jsonOptions);

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

        public async Task<(bool Success, string[] Errors)> UpdateAsync(int id, CreateRenterProfileDto model, string token)
        {
            try
            {
                var apiModel = new
                {
                    IsAdult = model.IsAdult,
                    NationalId = model.IsAdult ? model.NationalId : null,
                    BirthRegistrationNo = !model.IsAdult ? model.BirthRegistrationNo : null,
                    DateOfBirth = model.DateOfBirth,
                    FullName = model.FullName,
                    FatherName = model.FatherName,
                    MotherName = model.MotherName,
                    Nationality = model.Nationality.ToString(),
                    BloodGroup = model.BloodGroup.ToString(),
                    Profession = model.Profession.ToString(),
                    Gender = model.Gender.ToString(),
                    MobileNo = model.MobileNo,
                    Email = model.Email,
                    Division = model.Division,
                    District = model.District,
                    Upazila = model.Upazila,
                    AreaType = model.AreaType,
                    Ward = model.Ward,
                    Village = model.Village,
                    PostCode = model.PostCode,
                    HoldingNo = model.HoldingNo
                };

                var response = await _apiService.PutAsync($"api/renterprofiles/{id}", apiModel, token);

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

        public async Task<(bool Success, string[] Errors)> ApproveAsync(int id, ApproveRenterProfileDto model, string token)
        {
            try
            {
                var response = await _apiService.PatchAsync($"api/renterprofiles/{id}/approve", model, token);

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

                var response = await _httpClient.PostAsync($"api/renterprofiles/{id}/upload-image", formContent);

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