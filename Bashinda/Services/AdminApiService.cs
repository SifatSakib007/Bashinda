using Bashinda.ViewModels;
using BashindaAPI.DTOs;
using Newtonsoft.Json.Linq;
using System.Text.Json;
using System.Text.Json.Serialization;
//using RenterProfileDto = Bashinda.ViewModels.RenterProfileDto;

namespace Bashinda.Services
{
    public class AdminApiService : IAdminApiService
    {
        private readonly IApiService _apiService;
        private readonly JsonSerializerOptions _jsonOptions;
        private readonly ILogger<AdminApiService> _logger;
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public AdminApiService(IApiService apiService, ILogger<AdminApiService> logger, IHttpClientFactory httpClientFactory, IHttpContextAccessor httpContextAccessor)
        {
            _apiService = apiService;
            _logger = logger;
            _httpClientFactory = httpClientFactory;
            _httpContextAccessor = httpContextAccessor;
            _jsonOptions = new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true,
                // Add if using System.Text.Json attributes:
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase
            };
        }

        public async Task<(bool Success, List<AdminViewModel> Data, string[] Errors)> GetAllAdminsAsync(string token)
        {
            try
            {
                var response = await _apiService.GetAsync("api/admins", token);
                
                if (response.IsSuccessStatusCode)
                {
                    var content = await response.Content.ReadAsStringAsync();
                    var apiResult = JsonSerializer.Deserialize<List<AdminViewModel>>(content, _jsonOptions);
                    
                    if (apiResult != null)
                    {
                        return (true, apiResult, Array.Empty<string>());
                    }
                    
                    return (false, new List<AdminViewModel>(), new[] { "Failed to deserialize response" });
                }
                
                return (false, new List<AdminViewModel>(), new[] { $"API request failed with status code {response.StatusCode}" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting all admins");
                return (false, new List<AdminViewModel>(), new[] { $"Exception occurred: {ex.Message}" });
            }
        }

        public async Task<(bool Success, AdminViewModel? Data, string[] Errors)> GetAdminByIdAsync(string id, string token)
        {
            try
            {
                var response = await _apiService.GetAsync($"api/admins/{id}", token);
                
                if (response.IsSuccessStatusCode)
                {
                    var content = await response.Content.ReadAsStringAsync();
                    var apiResult = JsonSerializer.Deserialize<AdminViewModel>(content, _jsonOptions);
                    
                    if (apiResult != null)
                    {
                        return (true, apiResult, Array.Empty<string>());
                    }
                    
                    return (false, null, new[] { "Failed to deserialize response" });
                }
                
                return (false, null, new[] { $"API request failed with status code {response.StatusCode}" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting admin by id {id}", id);
                return (false, null, new[] { $"Exception occurred: {ex.Message}" });
            }
        }

        public async Task<(bool Success, AdminViewModel? Data, string[] Errors)> CreateAdminAsync(CreateAdminDto model, string token)
        {
            try
            {
                var response = await _apiService.PostAsync("api/admins", model, token);
                
                if (response.IsSuccessStatusCode)
                {
                    var content = await response.Content.ReadAsStringAsync();
                    var apiResult = JsonSerializer.Deserialize<AdminViewModel>(content, _jsonOptions);
                    
                    if (apiResult != null)
                    {
                        return (true, apiResult, Array.Empty<string>());
                    }
                    
                    return (false, null, new[] { "Failed to deserialize response" });
                }

                var errorContent = await response.Content.ReadAsStringAsync();
                return (false, null, new[] { errorContent });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating admin");
                return (false, null, new[] { $"Exception occurred: {ex.Message}" });
            }
        }

        public async Task<(bool Success, string[] Errors)> UpdateAdminPermissionsAsync(string id, AdminPermissionViewModel permissions, string token)
        {
            try
            {
                var response = await _apiService.PutAsync($"api/admins/{id}/permissions", permissions, token);
                
                if (response.IsSuccessStatusCode)
                {
                    return (true, Array.Empty<string>());
                }
                
                var content = await response.Content.ReadAsStringAsync();
                return (false, new[] { content });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating admin permissions for {id}", id);
                return (false, new[] { $"Exception occurred: {ex.Message}" });
            }
        }

        public async Task<(bool Success, string[] Errors)> DeleteAdminAsync(string id, string token)
        {
            try
            {
                var response = await _apiService.DeleteAsync($"api/admins/{id}", token);
                
                if (response.IsSuccessStatusCode)
                {
                    return (true, Array.Empty<string>());
                }
                
                var content = await response.Content.ReadAsStringAsync();
                return (false, new[] { content });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting admin {id}", id);
                return (false, new[] { $"Exception occurred: {ex.Message}" });
            }
        }
        //public async Task<(bool Success, AdminDashboardViewModel Dashboard, string[] Errors)>
        //    GetDashboardAsync(string token, AdminLocationFilters filters)
        //{
        //    var endpoint = BuildEndpoint("api/Admin/Dashboard", filters);
        //    return await HandleApiCall<AdminDashboardViewModel>(endpoint, token);
        //}
        public async Task<(bool Success, AdminDashboardViewModel Dashboard, string[] Errors)> GetDashboardAsync(string token, AdminLocationFilters filters)
        {
            try
            {
                var response = await _apiService.GetAsync("api/Admin/dashboard", token);

                if (response.IsSuccessStatusCode)
                {
                    var content = await response.Content.ReadAsStringAsync();
                    var dashboard = JsonSerializer.Deserialize<AdminDashboardViewModel>(content, _jsonOptions);

                    if (dashboard != null)
                    {
                        return (true, dashboard, Array.Empty<string>());
                    }

                    return (false, new AdminDashboardViewModel(), new[] { "Failed to deserialize dashboard data" });
                }

                return (false, new AdminDashboardViewModel(), new[] { $"API request failed with status code {response.StatusCode}" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting admin dashboard");
                return (false, new AdminDashboardViewModel(), new[] { $"Exception occurred: {ex.Message}" });
            }

        }



        public async Task<(bool Success, List<ViewModels.RenterProfileListDto> Profiles, string[] Errors)>
    GetPendingProfilesAsync(string token, AdminLocationFilters filters)
        {
            var endpoint = BuildEndpoint("api/Admin/renter-profiles/pending", filters);
            return await HandleApiCall<List<ViewModels.RenterProfileListDto>>(endpoint, token);
        }

        public async Task<(bool Success, string[] Errors)> ProcessApproval(
    int profileId, bool isApproved, string reason, string token, AdminLocationFilters filters)
        {
            try
            {
                // Corrected endpoint path
                var endpoint = BuildEndpoint($"api/Admin/renter-profiles/{profileId}/approve", filters);
                var payload = new { IsApproved = isApproved, Reason = reason };

                var response = await _apiService.PutAsync(endpoint, payload, token);

                return response.IsSuccessStatusCode
                    ? (true, Array.Empty<string>())
                    : (false, new[] { await response.Content.ReadAsStringAsync() });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Approval error for profile {ProfileId}", profileId);
                return (false, new[] { ex.Message });
            }
        }


        public async Task<bool> VerifyProfileAccess(int profileId, string token, AdminLocationFilters filters)
        {
            try
            {
                // First get profile to find associated user
                var (success, profile, errors) = await GetRenterProfileAsync(profileId, token);

                if (!success || profile?.UserId == null)
                {
                    _logger.LogError("Can't verify access for profile {ProfileId}: {Errors}",
                        profileId, string.Join(", ", errors));
                    return false;
                }

                // Now verify access using the USER ID
                var endpoint = BuildEndpoint($"api/Admin/verify-access/{profile.UserId}", filters);
                _logger.LogInformation("Verifying access for user {UserId} (profile {ProfileId})",
                    profile.UserId, profileId);

                var response = await _apiService.GetAsync(endpoint, token);

                if (!response.IsSuccessStatusCode)
                {
                    _logger.LogWarning("Access denied for user {UserId}: {StatusCode}",
                        profile.UserId, response.StatusCode);
                }

                return response.IsSuccessStatusCode;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Verification error for profile {ProfileId}", profileId);
                return false;
            }
        }
        private async Task<(bool Success, ViewModels.RenterProfileDto Profile, string[] Errors)> GetRenterProfileAsync(int profileId, string token)
        {
            try
            {
                var response = await _apiService.GetAsync($"api/RenterProfiles/{profileId}", token);

                if (!response.IsSuccessStatusCode)
                {
                    var errorContent = await response.Content.ReadAsStringAsync();
                    _logger.LogError("Failed to get renter profile {ProfileId}: {StatusCode} - {Error}",
                        profileId, response.StatusCode, errorContent);
                    return (false, null, new[] { $"Failed to retrieve profile: {errorContent}" });
                }

                var content = await response.Content.ReadAsStringAsync();
                _logger.LogDebug("Raw profile response: {Content}", content);

                var options = new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true,
                    Converters = { new JsonStringEnumConverter() }
                };

                var apiResponse = JsonSerializer.Deserialize<ViewModels.ApiResponse<ViewModels.RenterProfileDto>>(content, options);

                if (apiResponse?.Success != true || apiResponse.Data == null)
                {
                    _logger.LogError("Invalid profile response format for {ProfileId}", profileId);
                    return (false, null, apiResponse?.Errors ?? new[] { "Invalid profile response format" });
                }

                // Ensure UserId is present
                if (apiResponse.Data.UserId <= 0)
                {
                    _logger.LogError("Profile {ProfileId} has invalid UserId: {UserId}",
                        profileId, apiResponse.Data.UserId);
                    return (false, null, new[] { "Profile has invalid user association" });
                }

                return (true, apiResponse.Data, Array.Empty<string>());
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving profile {ProfileId}", profileId);
                return (false, null, new[] { $"Exception: {ex.Message}" });
            }
        }
        private string BuildEndpoint(string basePath, AdminLocationFilters filters)
        {
            var queryParams = new List<string>();

            // Add parameter logging
            _logger.LogDebug("Building endpoint with filters: {@Filters}", filters);

            if (!string.IsNullOrEmpty(filters.Division))
                queryParams.Add($"division={Uri.EscapeDataString(filters.Division)}");

            if (!string.IsNullOrEmpty(filters.District))
                queryParams.Add($"district={Uri.EscapeDataString(filters.District)}");

            if (!string.IsNullOrEmpty(filters.Upazila))
                queryParams.Add($"upazila={Uri.EscapeDataString(filters.Upazila)}");

            if (!string.IsNullOrEmpty(filters.Ward))
                queryParams.Add($"ward={Uri.EscapeDataString(filters.Ward)}");

            if (!string.IsNullOrEmpty(filters.Village))
                queryParams.Add($"village={Uri.EscapeDataString(filters.Village)}");

            var finalUrl = queryParams.Count > 0
                            ? $"{basePath}?{string.Join("&", queryParams)}"
                            : basePath;

            _logger.LogInformation("Constructed API endpoint: {FinalUrl}", finalUrl);
            return finalUrl;
        }

        // In AdminApiService.cs
        public async Task<(bool Success, List<Bashinda.ViewModels.RenterProfileListDto> PendingProfiles, string[] Errors)> GetPendingRenterProfilesAsync(string token)
        {
            try
            {
                var response = await _apiService.GetAsync("api/Admin/pending-renters", token);

                if (response.IsSuccessStatusCode)
                {
                    var content = await response.Content.ReadAsStringAsync();
                    var profiles = JsonSerializer.Deserialize<List<Bashinda.ViewModels.RenterProfileListDto>>(content, _jsonOptions);

                    return (true, profiles ?? new List<Bashinda.ViewModels.RenterProfileListDto>(), Array.Empty<string>());
                }

                return (false, new List<Bashinda.ViewModels.RenterProfileListDto>(),
                    new[] { $"API request failed: {response.StatusCode}" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting pending renter profiles");
                return (false, new List<Bashinda.ViewModels.RenterProfileListDto>(),
                    new[] { ex.Message });
            }
        }

        public async Task<(bool Success, List<OwnerProfileListDto> PendingProfiles, string[] Errors)> GetPendingOwnerProfilesAsync(string token)
        {
            try
            {
                // Corrected endpoint path
                var response = await _apiService.GetAsync("api/Admin/owner-profiles/pending", token);

                if (response.IsSuccessStatusCode)
                {
                    var content = await response.Content.ReadAsStringAsync();
                    var profiles = JsonSerializer.Deserialize<List<OwnerProfileListDto>>(content, _jsonOptions);

                    if (profiles != null)
                    {
                        return (true, profiles, Array.Empty<string>());
                    }

                    return (false, new List<OwnerProfileListDto>(), new[] { "Failed to deserialize profile data" });
                }

                return (false, new List<OwnerProfileListDto>(), new[] { $"API request failed with status code {response.StatusCode}" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting pending owner profiles");
                return (false, new List<OwnerProfileListDto>(), new[] { $"Exception occurred: {ex.Message}" });
            }
        }

        public async Task<(bool Success, string[] Errors)> ApproveRenterProfileAsync(int id, bool isApproved, string? rejectionReason, string token)
        {
            try
            {
                var approvalData = new { IsApproved = isApproved, Reason = rejectionReason };
                var response = await _apiService.PutAsync($"api/Admin/renter-profiles/{id}/approve", approvalData, token);
                
                if (response.IsSuccessStatusCode)
                {
                    return (true, Array.Empty<string>());
                }
                
                var content = await response.Content.ReadAsStringAsync();
                return (false, new[] { content });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error {action} renter profile {id}", isApproved ? "approving" : "rejecting", id);
                return (false, new[] { $"Exception occurred: {ex.Message}" });
            }
        }

        public async Task<(bool Success, string[] Errors)> ApproveOwnerProfileAsync(int id, bool isApproved, string? rejectionReason, string token)
        {
            try
            {
                var approvalData = new { IsApproved = isApproved, Reason = rejectionReason };
                var response = await _apiService.PutAsync($"api/Admin/owner-profiles/{id}/approve", approvalData, token);
                
                if (response.IsSuccessStatusCode)
                {
                    return (true, Array.Empty<string>());
                }
                
                var content = await response.Content.ReadAsStringAsync();
                return (false, new[] { content });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error {action} owner profile {id}", isApproved ? "approving" : "rejecting", id);
                return (false, new[] { $"Exception occurred: {ex.Message}" });
            }
        }

        public async Task<(bool Success, List<UserViewModel> Users, string[] Errors)> GetUsersAsync(string token)
        {
            try
            {
                var response = await _apiService.GetAsync("api/admin/users", token);
                
                if (response.IsSuccessStatusCode)
                {
                    var content = await response.Content.ReadAsStringAsync();
                    var users = JsonSerializer.Deserialize<List<UserViewModel>>(content, _jsonOptions);
                    
                    if (users != null)
                    {
                        return (true, users, Array.Empty<string>());
                    }
                    
                    return (false, new List<UserViewModel>(), new[] { "Failed to deserialize user data" });
                }
                
                return (false, new List<UserViewModel>(), new[] { $"API request failed with status code {response.StatusCode}" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting user accounts");
                return (false, new List<UserViewModel>(), new[] { $"Exception occurred: {ex.Message}" });
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
                return (false, new[] { content });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting user {id}", id);
                return (false, new[] { $"Exception occurred: {ex.Message}" });
            }
        }

        private async Task<(bool Success, T Data, string[] Errors)> HandleApiCall<T>(string endpoint, string token)
        {
            try
            {
                var response = await _apiService.GetAsync(endpoint, token);

                if (!response.IsSuccessStatusCode)
                    return (false, default, new[] { $"API Error: {response.StatusCode}" });

                var content = await response.Content.ReadAsStringAsync();
                var apiResponse = JsonSerializer.Deserialize<Models.ApiResponse<T>>(content, _jsonOptions);

                return apiResponse != null
                    ? (apiResponse.Success, apiResponse.Data, apiResponse.Errors.ToArray())
                    : (false, default, new[] { "Invalid API response format" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "API call failed for {Endpoint}", endpoint);
                return (false, default, new[] { ex.Message });
            }
        }

        public async Task<(bool Success, ViewModels.AdminPermissionDto Permissions, string[] Errors)> GetAdminPermissionsAsync(int adminId)
        {
            try
            {
                var token = GetAuthToken();
                if (string.IsNullOrEmpty(token))
                {
                    return (false, new ViewModels.AdminPermissionDto(), new[] { "Authentication token not found" });
                }
                var response = await _apiService.GetAsync($"api/Admin/permissions/{adminId}", token);

                if (response.IsSuccessStatusCode)
                {
                    var content = await response.Content.ReadAsStringAsync();
                    _logger.LogInformation("Raw API Response: {Content}", content); // Debug raw JSON
                                                                                    // Deserialize into the full API response structure
                    var apiResponse = JsonSerializer.Deserialize<ViewModels.ApiResponse<ViewModels.AdminPermissionDto>>(
                        content,
                        new JsonSerializerOptions { PropertyNameCaseInsensitive = true }
                    );

                    if (apiResponse?.Success == true && apiResponse.Data != null)
                    {
                        return (true, apiResponse.Data, Array.Empty<string>());
                    }

                    return (false, new ViewModels.AdminPermissionDto(), apiResponse?.Errors ?? new[] { "Invalid response" });
                }

                var errorContent = await response.Content.ReadAsStringAsync();
                return (false, new ViewModels.AdminPermissionDto(), new[] { errorContent });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving permissions for admin {AdminId}", adminId);
                return (false, new ViewModels.AdminPermissionDto(), new[] { ex.Message });
            }
        }

        private string GetAuthToken()
        {
            // Get access to HttpContext
            var httpContext = _httpContextAccessor.HttpContext;
            if (httpContext == null) return null;

            // Try to get token from session
            var token = httpContext.Session.GetString("AuthToken");

            // Fallback to authentication cookie
            if (string.IsNullOrEmpty(token))
            {
                token = httpContext.User.FindFirst("Token")?.Value;
            }

            return token;
        }
    }
} 