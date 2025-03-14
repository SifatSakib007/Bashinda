using Bashinda.ViewModels;
using System.Text.Json;

namespace Bashinda.Services
{
    public interface IAdminApiService
    {
        Task<(bool Success, AdminDashboardDto? Data, string[] Errors)> GetDashboardAsync(string token);
        Task<(bool Success, List<UserDto> Data, string[] Errors)> GetUsersAsync(string token);
        Task<(bool Success, List<RenterProfileListDto> Data, string[] Errors)> GetPendingRenterProfilesAsync(string token);
        Task<(bool Success, List<ApartmentOwnerProfileListDto> Data, string[] Errors)> GetPendingOwnerProfilesAsync(string token);
        Task<(bool Success, string[] Errors)> ApproveRenterProfileAsync(int id, bool isApproved, string? reason, string token);
        Task<(bool Success, string[] Errors)> ApproveOwnerProfileAsync(int id, bool isApproved, string? reason, string token);
        Task<(bool Success, string[] Errors)> DeleteUserAsync(int id, string token);
    }

    public class AdminApiService : IAdminApiService
    {
        private readonly IApiService _apiService;
        private readonly JsonSerializerOptions _jsonOptions;

        public AdminApiService(IApiService apiService)
        {
            _apiService = apiService;
            _jsonOptions = new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            };
        }

        public async Task<(bool Success, AdminDashboardDto? Data, string[] Errors)> GetDashboardAsync(string token)
        {
            try
            {
                var response = await _apiService.GetAsync("api/admin/dashboard", token);
                
                if (response.IsSuccessStatusCode)
                {
                    var content = await response.Content.ReadAsStringAsync();
                    var result = JsonSerializer.Deserialize<ApiResponse<AdminDashboardDto>>(content, _jsonOptions);
                    
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

        public async Task<(bool Success, List<UserDto> Data, string[] Errors)> GetUsersAsync(string token)
        {
            try
            {
                var response = await _apiService.GetAsync("api/admin/users", token);
                
                if (response.IsSuccessStatusCode)
                {
                    var content = await response.Content.ReadAsStringAsync();
                    var result = JsonSerializer.Deserialize<ApiResponse<List<UserDto>>>(content, _jsonOptions);
                    
                    if (result != null && result.Success)
                    {
                        return (true, result.Data ?? new List<UserDto>(), Array.Empty<string>());
                    }
                    
                    return (false, new List<UserDto>(), result?.Errors ?? new[] { "Unknown error occurred" });
                }
                
                return (false, new List<UserDto>(), new[] { $"API request failed with status code {response.StatusCode}" });
            }
            catch (Exception ex)
            {
                return (false, new List<UserDto>(), new[] { $"Exception occurred: {ex.Message}" });
            }
        }

        public async Task<(bool Success, List<RenterProfileListDto> Data, string[] Errors)> GetPendingRenterProfilesAsync(string token)
        {
            try
            {
                var response = await _apiService.GetAsync("api/admin/renter-profiles/pending", token);
                
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

        public async Task<(bool Success, List<ApartmentOwnerProfileListDto> Data, string[] Errors)> GetPendingOwnerProfilesAsync(string token)
        {
            try
            {
                var response = await _apiService.GetAsync("api/admin/owner-profiles/pending", token);
                
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

        public async Task<(bool Success, string[] Errors)> ApproveRenterProfileAsync(int id, bool isApproved, string? reason, string token)
        {
            try
            {
                var model = new ApproveProfileDto { IsApproved = isApproved, Reason = reason };
                var response = await _apiService.PutAsync($"api/admin/renter-profiles/{id}/approve", model, token);
                
                if (response.IsSuccessStatusCode)
                {
                    return (true, Array.Empty<string>());
                }
                
                var content = await response.Content.ReadAsStringAsync();
                var result = JsonSerializer.Deserialize<ApiResponse<object>>(content, _jsonOptions);
                
                return (false, result?.Errors ?? new[] { $"API request failed with status code {response.StatusCode}" });
            }
            catch (Exception ex)
            {
                return (false, new[] { $"Exception occurred: {ex.Message}" });
            }
        }

        public async Task<(bool Success, string[] Errors)> ApproveOwnerProfileAsync(int id, bool isApproved, string? reason, string token)
        {
            try
            {
                var model = new ApproveProfileDto { IsApproved = isApproved, Reason = reason };
                var response = await _apiService.PutAsync($"api/admin/owner-profiles/{id}/approve", model, token);
                
                if (response.IsSuccessStatusCode)
                {
                    return (true, Array.Empty<string>());
                }
                
                var content = await response.Content.ReadAsStringAsync();
                var result = JsonSerializer.Deserialize<ApiResponse<object>>(content, _jsonOptions);
                
                return (false, result?.Errors ?? new[] { $"API request failed with status code {response.StatusCode}" });
            }
            catch (Exception ex)
            {
                return (false, new[] { $"Exception occurred: {ex.Message}" });
            }
        }

        public async Task<(bool Success, string[] Errors)> DeleteUserAsync(int id, string token)
        {
            try
            {
                var response = await _apiService.DeleteAsync($"api/admin/users/{id}", token);
                
                if (response.IsSuccessStatusCode)
                {
                    return (true, Array.Empty<string>());
                }
                
                var content = await response.Content.ReadAsStringAsync();
                var result = JsonSerializer.Deserialize<ApiResponse<object>>(content, _jsonOptions);
                
                return (false, result?.Errors ?? new[] { $"API request failed with status code {response.StatusCode}" });
            }
            catch (Exception ex)
            {
                return (false, new[] { $"Exception occurred: {ex.Message}" });
            }
        }
    }
} 