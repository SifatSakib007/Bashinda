using BashindaAPI.DTOs;
using BashindaAPI.Models;

namespace BashindaAPI.Services
{
    public interface IAuthService
    {
        Task<(bool Success, string Token, User? User, string[] Errors)> LoginAsync(LoginDto model);
        Task<(bool Success, string[] Errors)> RegisterAsync(RegisterDto model);
        Task<(bool Success, string[] Errors)> VerifyOtpAsync(VerifyOtpDto model);
        Task<(bool Success, string[] Errors)> ResendOtpAsync(string email);
    }
} 