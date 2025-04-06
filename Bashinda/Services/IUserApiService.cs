using Bashinda.ViewModels;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Bashinda.Services
{
    public interface IUserApiService
    {
        Task<(bool Success, LoginResponseDto? Data, string[] Errors)> LoginAsync(LoginRequestDto model);
        Task<(bool Success, UserDto? Data, string[] Errors)> RegisterAsync(RegisterUserDto model);
        Task<(bool Success, UserDto? Data, string[] Errors)> GetUserByIdAsync(int id, string token);
        Task<(bool Success, List<UserDto> Data, string[] Errors)> GetUsersAsync(string token);
        Task<(bool Success, string[] Errors)> LogoutAsync(string token);
        Task<(bool Success, string[] Errors)> VerifyOTPAsync(string email, string otp);
        Task<(bool Success, string[] Errors)> ResendOTPAsync(string email);
    }

    public class UserApiService : IUserApiService
    {
        private readonly IApiService _apiService;
        private readonly JsonSerializerOptions _jsonOptions;

        public UserApiService(IApiService apiService)
        {
            _apiService = apiService;
            _jsonOptions = new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true,
                AllowTrailingCommas = true,
                ReadCommentHandling = JsonCommentHandling.Skip,
                NumberHandling = JsonNumberHandling.AllowReadingFromString
            };
        }

        public async Task<(bool Success, LoginResponseDto? Data, string[] Errors)> LoginAsync(LoginRequestDto model)
        {
            try
            {
                // Create an appropriate payload based on whether email or phone number is provided
                object apiModel;
                
                if (!string.IsNullOrEmpty(model.PhoneNumber))
                {
                    apiModel = new
                    {
                        PhoneNumber = model.PhoneNumber,
                        Password = model.Password
                    };
                    System.Diagnostics.Debug.WriteLine($"Sending login request for phone number: {model.PhoneNumber}");
                }
                else if (!string.IsNullOrEmpty(model.Email))
                {
                    apiModel = new
                    {
                        Email = model.Email,
                        Password = model.Password
                    };
                    System.Diagnostics.Debug.WriteLine($"Sending login request for email: {model.Email}");
                }
                else
                {
                    // Neither email nor phone provided
                    return (false, null, new[] { "Either email or phone number must be provided" });
                }

                var response = await _apiService.PostAsync("api/auth/login", apiModel, null);
                var content = await response.Content.ReadAsStringAsync();
                
                // Log the raw response
                System.Diagnostics.Debug.WriteLine($"Raw API Response: {content}");
                System.Diagnostics.Debug.WriteLine($"Response Status Code: {response.StatusCode}");
                
                if (response.IsSuccessStatusCode)
                {
                    try
                    {
                        System.Diagnostics.Debug.WriteLine("Attempting to deserialize response...");
                        
                        // Deserialize directly into AuthResponseDto
                        var result = JsonSerializer.Deserialize<AuthResponseDto>(content, _jsonOptions);
                        System.Diagnostics.Debug.WriteLine($"Deserialization result: {(result == null ? "null" : "not null")}");
                        
                        if (result != null)
                        {
                            System.Diagnostics.Debug.WriteLine($"Deserialization successful. Success: {result.Success}");
                            System.Diagnostics.Debug.WriteLine($"User: {result.User?.UserName}");
                            
                            if (result.Success && result.User != null)
                            {
                                // Convert AuthResponseDto to LoginResponseDto
                                var loginResponse = new LoginResponseDto
                                {
                                    Token = result.Token,
                                    User = new UserDto
                                    {
                                        Id = result.User.Id,
                                        UserName = result.User.UserName,
                                        Email = result.User.Email,
                                        PhoneNumber = result.User.PhoneNumber,
                                        Role = result.User.Role,
                                        IsVerified = result.User.IsVerified,
                                        Roles = new List<string> { result.User.Role }
                                    }
                                };

                                System.Diagnostics.Debug.WriteLine("Login successful, returning data");
                                return (true, loginResponse, Array.Empty<string>());
                            }
                            
                            System.Diagnostics.Debug.WriteLine($"Login failed. Success: {result.Success}, User: {result.User != null}");
                            System.Diagnostics.Debug.WriteLine($"Errors: {string.Join(", ", result.Errors ?? new[] { "Unknown error" })}");
                            return (false, null, result.Errors ?? new[] { "Login failed: Invalid response data" });
                        }
                        
                        System.Diagnostics.Debug.WriteLine("Deserialization returned null result");
                        return (false, null, new[] { "Failed to parse API response" });
                    }
                    catch (JsonException ex)
                    {
                        System.Diagnostics.Debug.WriteLine($"JSON Deserialization Error: {ex.Message}");
                        System.Diagnostics.Debug.WriteLine($"Stack trace: {ex.StackTrace}");
                        System.Diagnostics.Debug.WriteLine($"Path: {ex.Path}");
                        System.Diagnostics.Debug.WriteLine($"LineNumber: {ex.LineNumber}");
                        System.Diagnostics.Debug.WriteLine($"BytePositionInLine: {ex.BytePositionInLine}");
                        return (false, null, new[] { $"Failed to parse API response: {ex.Message}" });
                    }
                }

                // For more detailed error messages on BadRequest or Unauthorized
                if (response.StatusCode == System.Net.HttpStatusCode.BadRequest || 
                    response.StatusCode == System.Net.HttpStatusCode.Unauthorized)
                {
                    try
                    {
                        System.Diagnostics.Debug.WriteLine($"Attempting to deserialize error response...");
                        var errorResponse = JsonSerializer.Deserialize<AuthResponseDto>(content, _jsonOptions);
                        if (errorResponse != null && errorResponse.Errors.Length > 0)
                        {
                            System.Diagnostics.Debug.WriteLine($"Login failed with status {response.StatusCode}. Errors: {string.Join(", ", errorResponse.Errors)}");
                            return (false, null, errorResponse.Errors);
                        }
                    }
                    catch (JsonException ex)
                    {
                        System.Diagnostics.Debug.WriteLine($"Error Response JSON Deserialization Error: {ex.Message}");
                        System.Diagnostics.Debug.WriteLine($"Stack trace: {ex.StackTrace}");
                    }
                }
                
                System.Diagnostics.Debug.WriteLine($"Login failed with status {response.StatusCode}. Response: {content}");
                return (false, null, new[] { $"API request failed with status code {response.StatusCode}. Response: {content}" });
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Exception in LoginAsync: {ex.Message}");
                System.Diagnostics.Debug.WriteLine($"Stack trace: {ex.StackTrace}");
                return (false, null, new[] { $"Exception occurred: {ex.Message}" });
            }
        }

        public async Task<(bool Success, UserDto? Data, string[] Errors)> RegisterAsync(RegisterUserDto model)
        {
            try
            {
                // Convert string Role to UserRole enum format the API expects
                var apiModel = new
                {
                    UserName = model.UserName,
                    Email = model.Email,
                    PhoneNumber = model.PhoneNumber,
                    Password = model.Password,
                    ConfirmPassword = model.ConfirmPassword,
                    Role = model.Role
                };

                var response = await _apiService.PostAsync("api/auth/register", apiModel, null);
                
                if (response.IsSuccessStatusCode)
                {
                    var content = await response.Content.ReadAsStringAsync();
                    var result = JsonSerializer.Deserialize<ApiResponse<UserDto>>(content, _jsonOptions);
                    
                    if (result != null && result.Success)
                    {
                        return (true, result.Data, Array.Empty<string>());
                    }
                    
                    return (false, null, result?.Errors ?? new[] { "Unknown error occurred" });
                }

                // For more detailed error messages on BadRequest
                if (response.StatusCode == System.Net.HttpStatusCode.BadRequest)
                {
                    var content = await response.Content.ReadAsStringAsync();
                    try
                    {
                        

                        var errorResponse = JsonSerializer.Deserialize<AuthResponseDto>(content, _jsonOptions);

                        if (errorResponse != null && errorResponse.Errors.Length > 0)
                        {
                            return (false, null, errorResponse.Errors);
                        }
                    }
                    catch { /* ignore and try next */  }
                    try
                    {
                        // Fallback: try deserializing as ValidationProblemDetailsDto
                        var validationError = JsonSerializer.Deserialize<ValidationProblemDetailsDto>(content, _jsonOptions);
                        if (validationError != null && validationError.Errors.Count > 0)
                        {
                            // Flatten errors into a single string array
                            var errors = validationError.Errors.SelectMany(kvp => kvp.Value).ToArray();
                            return (false, null, errors);
                        }
                    }
                    catch { /* If deserialization fails, use the generic message below */ }
                }
                
                return (false, null, new[] { $"API request failed with status code {response.StatusCode}" });
            }
            catch (Exception ex)
            {
                return (false, null, new[] { $"Exception occurred: {ex.Message}" });
            }
        }

        public async Task<(bool Success, string[] Errors)> VerifyOTPAsync(string email, string otp)
        {
            try
            {
                var apiModel = new
                {
                    Email = email,
                    OtpCode = otp
                };

                var response = await _apiService.PostAsync("api/auth/verify-otp", apiModel, null);
                
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

                // For more detailed error messages on BadRequest
                if (response.StatusCode == System.Net.HttpStatusCode.BadRequest)
                {
                    var content = await response.Content.ReadAsStringAsync();
                    try
                    {
                        var errorResponse = JsonSerializer.Deserialize<ApiResponse<object>>(content, _jsonOptions);
                        if (errorResponse != null && errorResponse.Errors.Length > 0)
                        {
                            return (false, errorResponse.Errors);
                        }
                    }
                    catch { /* If deserialization fails, use the generic message below */ }
                }
                
                return (false, new[] { $"API request failed with status code {response.StatusCode}" });
            }
            catch (Exception ex)
            {
                return (false, new[] { $"Exception occurred: {ex.Message}" });
            }
        }

        public async Task<(bool Success, string[] Errors)> ResendOTPAsync(string email)
        {
            try
            {
                var apiModel = new
                {
                    Email = email  // Changed to match API's expected format
                };

                var response = await _apiService.PostAsync("api/auth/resend-otp", apiModel, null);
                
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

                // For more detailed error messages on BadRequest
                if (response.StatusCode == System.Net.HttpStatusCode.BadRequest)
                {
                    var content = await response.Content.ReadAsStringAsync();
                    try
                    {
                        var errorResponse = JsonSerializer.Deserialize<ApiResponse<object>>(content, _jsonOptions);
                        if (errorResponse != null && errorResponse.Errors.Length > 0)
                        {
                            return (false, errorResponse.Errors);
                        }
                    }
                    catch { /* If deserialization fails, use the generic message below */ }
                }
                
                return (false, new[] { $"API request failed with status code {response.StatusCode}" });
            }
            catch (Exception ex)
            {
                return (false, new[] { $"Exception occurred: {ex.Message}" });
            }
        }

        public async Task<(bool Success, UserDto? Data, string[] Errors)> GetUserByIdAsync(int id, string token)
        {
            try
            {
                var response = await _apiService.GetAsync($"api/users/{id}", token);
                
                if (response.IsSuccessStatusCode)
                {
                    var content = await response.Content.ReadAsStringAsync();
                    var result = JsonSerializer.Deserialize<ApiResponse<UserDto>>(content, _jsonOptions);
                    
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
                var response = await _apiService.GetAsync("api/users", token);
                
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

        public async Task<(bool Success, string[] Errors)> LogoutAsync(string token)
        {
            try
            {
                var response = await _apiService.PostAsync("api/auth/logout", new {}, token);
                
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
    }

    // Add AuthResponseDto to match API's response structure
    public class AuthResponseDto
    {
        public bool Success { get; set; }
        public string Token { get; set; } = string.Empty;
        public UserDto? User { get; set; }
        public string[] Errors { get; set; } = Array.Empty<string>();
    }
} 