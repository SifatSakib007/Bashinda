using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using Bashinda.Models;
using Bashinda.ViewModels;

namespace Bashinda.Services
{
    public class UserService : IUserService
    {
        private readonly HttpClient _httpClient;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IEmailSender _emailSender;

        public UserService(
            IHttpClientFactory httpClientFactory,
            IHttpContextAccessor httpContextAccessor,
            IEmailSender emailSender)
        {
            _httpClient = httpClientFactory.CreateClient("BashindaAPI");
            _httpContextAccessor = httpContextAccessor;
            _emailSender = emailSender;
        }

        public async Task<(bool Success, string[] Errors)> RegisterUserAsync(RegisterViewModel model)
        {
            try
            {
                // Call API registration endpoint
                var response = await _httpClient.PostAsJsonAsync("api/auth/register", new
                {
                    model.UserName,
                    model.Email,
                    model.PhoneNumber,
                    model.Password,
                    model.Role
                });

                if (!response.IsSuccessStatusCode)
                {
                    var errorContent = await response.Content.ReadAsStringAsync();
                    return (false, new[] { $"Registration failed: {errorContent}" });
                }

                // Trigger OTP resend through API
                var otpResponse = await _httpClient.PostAsync(
                    $"api/auth/resend-otp?email={model.Email}",
                    null
                );

                return (true, Array.Empty<string>());
            }
            catch (Exception ex)
            {
                return (false, new[] { ex.Message });
            }
        }

        public async Task<(bool Success, string[] Errors)> ConfirmOTPAsync(string email, string otp)
        {
            try
            {
                // Call API OTP verification endpoint
                var response = await _httpClient.PostAsJsonAsync("api/auth/verify-otp", new
                {
                    Email = email,
                    OtpCode = otp
                });

                if (response.IsSuccessStatusCode)
                {
                    return (true, Array.Empty<string>());
                }

                var errorContent = await response.Content.ReadAsStringAsync();
                return (false, new[] { $"OTP verification failed: {errorContent}" });
            }
            catch (Exception ex)
            {
                return (false, new[] { ex.Message });
            }
        }

        public async Task<AuthResponse> AuthenticateUserAsync(LoginViewModel model)
        {
            try
            {
                // Call API login endpoint
                var response = await _httpClient.PostAsJsonAsync("api/auth/login", new
                {
                    model.PhoneNumber,
                    model.Password
                });

                if (response.IsSuccessStatusCode)
                {
                    var content = await response.Content.ReadAsStringAsync();
                    return JsonSerializer.Deserialize<AuthResponse>(content);
                }

                return new AuthResponse { Success = false };
            }
            catch
            {
                return new AuthResponse { Success = false };
            }
        }

        Task<ApplicationUser?> IUserService.AuthenticateUserAsync(LoginViewModel model)
        {
            throw new NotImplementedException();
        }
    }

    public class AuthResponse
    {
        public bool Success { get; set; }
        public string Token { get; set; }
        public UserInfo User { get; set; }
    }

    public class UserInfo
    {
        public int Id { get; set; }
        public string UserName { get; set; }
        public string Email { get; set; }
        public string PhoneNumber { get; set; }
        public string Role { get; set; }
        public bool IsVerified { get; set; }
    }
}