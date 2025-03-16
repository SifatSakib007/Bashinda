using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Http;

namespace Bashinda.Services
{
    public interface IApiService
    {
        Task<HttpResponseMessage> GetAsync(string endpoint, string? token = null);
        Task<HttpResponseMessage> PostAsync<T>(string endpoint, T data, string? token = null);
        Task<HttpResponseMessage> PutAsync<T>(string endpoint, T data, string? token = null);
        Task<HttpResponseMessage> DeleteAsync(string endpoint, string? token = null);
        Task<HttpResponseMessage> PatchAsync<T>(string endpoint, T data, string? token = null);
    }

    public class ApiService : IApiService
    {
        private readonly IHttpClientFactory _clientFactory;
        private readonly JsonSerializerOptions _jsonOptions;
        private readonly ILogger<ApiService> _logger;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public ApiService(
            IHttpClientFactory clientFactory, 
            ILogger<ApiService> logger,
            IHttpContextAccessor httpContextAccessor)
        {
            _clientFactory = clientFactory;
            _logger = logger;
            _httpContextAccessor = httpContextAccessor;
            _jsonOptions = new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true,
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
                WriteIndented = false
            };
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

        public async Task<HttpResponseMessage> GetAsync(string endpoint, string? token = null)
        {
            var client = _clientFactory.CreateClient("BashindaAPI");
            token = GetEffectiveToken(token);
            
            if (!string.IsNullOrEmpty(token))
            {
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
                var tokenInfo = token.Length > 20 ? 
                    $"{token.Substring(0, 10)}...{token.Substring(token.Length - 10)}" : 
                    token;
                _logger.LogDebug("Setting authorization header with Bearer token: {TokenSample} (Length: {TokenLength})", 
                    tokenInfo, token.Length);
            }
            else
            {
                _logger.LogWarning("No token provided for API request to {Endpoint}", endpoint);
            }
            
            try
            {
                _logger.LogDebug("Making GET request to {Endpoint}", endpoint);
                var response = await client.GetAsync(endpoint);
                
                _logger.LogDebug("Response from {Endpoint}: {StatusCode}", endpoint, response.StatusCode);
                if (response.StatusCode == System.Net.HttpStatusCode.Unauthorized)
                {
                    var content = await response.Content.ReadAsStringAsync();
                    _logger.LogWarning("UNAUTHORIZED response from {Endpoint}. Content: '{Content}'. Token sample: {TokenSample}", 
                        endpoint, 
                        content,
                        !string.IsNullOrEmpty(token) ? token.Substring(0, Math.Min(30, token.Length)) + "..." : "No token");
                    
                    // Log the complete headers for debugging
                    _logger.LogInformation("Complete request headers sent:");
                    foreach (var header in client.DefaultRequestHeaders)
                    {
                        _logger.LogInformation("  {Key}: {Value}", header.Key, string.Join(", ", header.Value));
                    }
                }
                else if (!response.IsSuccessStatusCode)
                {
                    var content = await response.Content.ReadAsStringAsync();
                    _logger.LogWarning("Unsuccessful response from {Endpoint}: {StatusCode}, Content: {Content}", 
                        endpoint, response.StatusCode, content);
                }
                
                return response;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error calling GET {Endpoint}", endpoint);
                throw;
            }
        }

        public async Task<HttpResponseMessage> PostAsync<T>(string endpoint, T data, string? token = null)
        {
            var client = _clientFactory.CreateClient("BashindaAPI");
            token = GetEffectiveToken(token);
            
            if (!string.IsNullOrEmpty(token))
            {
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
            }
            
            try 
            {
                string json = JsonSerializer.Serialize(data, _jsonOptions);
                System.Diagnostics.Debug.WriteLine($"POST Request to {endpoint}");
                System.Diagnostics.Debug.WriteLine($"Request data: {json}");
                
                var content = new StringContent(json, Encoding.UTF8, "application/json");
                var response = await client.PostAsync(endpoint, content);
                
                var responseContent = await response.Content.ReadAsStringAsync();
                System.Diagnostics.Debug.WriteLine($"Response Status Code: {response.StatusCode}");
                System.Diagnostics.Debug.WriteLine($"Response Content: {responseContent}");
                
                if (!response.IsSuccessStatusCode)
                {
                    _logger.LogWarning("API returned {StatusCode} for POST {Endpoint}. Response: {Response}", 
                        response.StatusCode, endpoint, responseContent);
                }
                
                return response;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Exception in PostAsync: {ex.Message}");
                System.Diagnostics.Debug.WriteLine($"Stack trace: {ex.StackTrace}");
                _logger.LogError(ex, "Error calling POST {Endpoint}", endpoint);
                throw;
            }
        }

        public async Task<HttpResponseMessage> PutAsync<T>(string endpoint, T data, string? token = null)
        {
            var client = _clientFactory.CreateClient("BashindaAPI");
            token = GetEffectiveToken(token);
            
            if (!string.IsNullOrEmpty(token))
            {
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
            }
            
            try 
            {
                string json = JsonSerializer.Serialize(data, _jsonOptions);
                _logger.LogDebug("PUT Request to {Endpoint} with data: {Json}", endpoint, json);
                
                var content = new StringContent(json, Encoding.UTF8, "application/json");
                var response = await client.PutAsync(endpoint, content);
                
                if (!response.IsSuccessStatusCode)
                {
                    var responseContent = await response.Content.ReadAsStringAsync();
                    _logger.LogWarning("API returned {StatusCode} for PUT {Endpoint}. Response: {Response}", 
                        response.StatusCode, endpoint, responseContent);
                }
                
                return response;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error calling PUT {Endpoint}", endpoint);
                throw;
            }
        }

        public async Task<HttpResponseMessage> DeleteAsync(string endpoint, string? token = null)
        {
            var client = _clientFactory.CreateClient("BashindaAPI");
            token = GetEffectiveToken(token);
            
            if (!string.IsNullOrEmpty(token))
            {
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
            }
            
            try
            {
                var response = await client.DeleteAsync(endpoint);
                
                if (!response.IsSuccessStatusCode)
                {
                    var responseContent = await response.Content.ReadAsStringAsync();
                    _logger.LogWarning("API returned {StatusCode} for DELETE {Endpoint}. Response: {Response}", 
                        response.StatusCode, endpoint, responseContent);
                }
                
                return response;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error calling DELETE {Endpoint}", endpoint);
                throw;
            }
        }

        public async Task<HttpResponseMessage> PatchAsync<T>(string endpoint, T data, string? token = null)
        {
            var client = _clientFactory.CreateClient("BashindaAPI");
            token = GetEffectiveToken(token);
            
            if (!string.IsNullOrEmpty(token))
            {
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
            }
            
            try 
            {
                string json = JsonSerializer.Serialize(data, _jsonOptions);
                _logger.LogDebug("PATCH Request to {Endpoint} with data: {Json}", endpoint, json);
                
                var content = new StringContent(json, Encoding.UTF8, "application/json");
                var response = await client.PatchAsync(endpoint, content);
                
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