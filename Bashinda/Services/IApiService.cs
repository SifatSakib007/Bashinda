using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using System.Net.Http.Json;

namespace Bashinda.Services
{
    public interface IApiService
    {
        Task<HttpResponseMessage> GetAsync(string endpoint, string token);
        Task<HttpResponseMessage> PostAsync<T>(string endpoint, T data, string token);
        Task<HttpResponseMessage> PutAsync<T>(string endpoint, T data, string token);
        Task<HttpResponseMessage> DeleteAsync(string endpoint, string token);
        Task<HttpResponseMessage> PatchAsync<T>(string endpoint, T data, string? token = null);
    }

    public class ApiService : IApiService
    {
        private readonly HttpClient _httpClient;
        private readonly IConfiguration _configuration;
        private readonly ILogger<ApiService> _logger;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly JsonSerializerOptions _jsonOptions;

        public ApiService(HttpClient httpClient, IConfiguration configuration, ILogger<ApiService> logger, IHttpContextAccessor httpContextAccessor)
        {
            _httpClient = httpClient;
            _configuration = configuration;
            _logger = logger;
            _httpContextAccessor = httpContextAccessor;
            _jsonOptions = new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true,
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
                WriteIndented = false
            };
            
            // Set the base address from configuration
            string apiBaseUrl = _configuration["ApiBaseUrl"] ?? "https://localhost:5001/";
            _httpClient.BaseAddress = new Uri(apiBaseUrl);
        }

        // Helper method to get token from cookie if not provided
        private string? GetEffectiveToken(string? providedToken)
        {
            if (!string.IsNullOrEmpty(providedToken))
            {
                return providedToken;
            }

            // Try to get token from cookie
            if (_httpContextAccessor.HttpContext != null)
            {
                var tokenFromCookie = _httpContextAccessor.HttpContext.Request.Cookies["JwtToken"];
                if (!string.IsNullOrEmpty(tokenFromCookie))
                {
                    _logger.LogDebug("Using JWT token from cookie");
                    return tokenFromCookie;
                }
            }

            _logger.LogWarning("No token provided and no token found in cookie");
            return null;
        }

        public async Task<HttpResponseMessage> GetAsync(string endpoint, string token)
        {
            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
            return await _httpClient.GetAsync(endpoint);
        }

        public async Task<HttpResponseMessage> PostAsync<T>(string endpoint, T data, string token)
        {
            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
            var json = JsonSerializer.Serialize(data, _jsonOptions);
            var content = new StringContent(json, Encoding.UTF8, "application/json");
            return await _httpClient.PostAsync(endpoint, content);
        }

        public async Task<HttpResponseMessage> PutAsync<T>(string endpoint, T data, string token)
        {
            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
            var json = JsonSerializer.Serialize(data, _jsonOptions);
            var content = new StringContent(json, Encoding.UTF8, "application/json");
            return await _httpClient.PutAsync(endpoint, content);
        }

        public async Task<HttpResponseMessage> DeleteAsync(string endpoint, string token)
        {
            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
            return await _httpClient.DeleteAsync(endpoint);
        }

        public async Task<HttpResponseMessage> PatchAsync<T>(string endpoint, T data, string? token = null)
        {
            token = GetEffectiveToken(token);
            
            if (!string.IsNullOrEmpty(token))
            {
                _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
            }
            
            try 
            {
                string json = JsonSerializer.Serialize(data, _jsonOptions);
                _logger.LogDebug("PATCH Request to {Endpoint} with data: {Json}", endpoint, json);
                
                var content = new StringContent(json, Encoding.UTF8, "application/json");
                var response = await _httpClient.PatchAsync(endpoint, content);
                
                if (!response.IsSuccessStatusCode)
                {
                    var responseContent = await response.Content.ReadAsStringAsync();
                    _logger.LogWarning("API returned {StatusCode} for PATCH {Endpoint}. Response: {Response}", 
                        response.StatusCode, endpoint, responseContent);
                }
                
                return response;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error calling PATCH {Endpoint}", endpoint);
                throw;
            }
        }
    }
} 