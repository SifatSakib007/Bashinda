using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;

namespace Bashinda.Services
{
    public class ApiHealthService
    {
        private readonly IHttpClientFactory _clientFactory;
        private readonly ILogger<ApiHealthService> _logger;
        private readonly IUserApiService _userApiService;

        public ApiHealthService(
            IHttpClientFactory clientFactory, 
            ILogger<ApiHealthService> logger,
            IUserApiService userApiService)
        {
            _clientFactory = clientFactory;
            _logger = logger;
            _userApiService = userApiService;
        }

        public async Task<bool> IsApiAvailable()
        {
            try
            {
                var client = _clientFactory.CreateClient("BashindaAPI");
                var response = await client.GetAsync("api/health");
                
                if (response.IsSuccessStatusCode)
                {
                    _logger.LogInformation("API is available");
                    return true;
                }
                
                _logger.LogWarning("API health check failed with status: {StatusCode}", response.StatusCode);
                return false;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error checking API availability");
                return false;
            }
        }

        public async Task<string> RefreshTokenIfNeeded(string currentToken, string email, string password)
        {
            if (string.IsNullOrEmpty(currentToken))
            {
                _logger.LogWarning("Token is empty, can't refresh");
                return string.Empty;
            }

            try
            {
                // Try a simple token validation with the API
                var client = _clientFactory.CreateClient("BashindaAPI");
                client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", currentToken);
                
                var response = await client.GetAsync("api/auth/validate-token");
                
                if (response.IsSuccessStatusCode)
                {
                    _logger.LogInformation("Token is still valid");
                    return currentToken;
                }

                // Token is invalid, try to get a new one
                _logger.LogWarning("Token validation failed, attempting to get a new token");
                
                if (string.IsNullOrEmpty(email) || string.IsNullOrEmpty(password))
                {
                    _logger.LogWarning("Can't refresh token - missing credentials");
                    return string.Empty;
                }

                var loginRequest = new ViewModels.LoginRequestDto
                {
                    Email = email,
                    Password = password
                };

                var (success, loginResult, errors) = await _userApiService.LoginAsync(loginRequest);
                
                if (success && loginResult != null)
                {
                    _logger.LogInformation("Successfully refreshed token");
                    return loginResult.Token;
                }
                
                _logger.LogWarning("Failed to refresh token: {Errors}", string.Join(", ", errors));
                return string.Empty;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error refreshing token");
                return string.Empty;
            }
        }
    }
} 