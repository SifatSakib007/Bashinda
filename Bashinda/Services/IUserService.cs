using Bashinda.Models;
using Bashinda.ViewModels;

namespace Bashinda.Services
{
    public interface IUserService
    {
        Task<(bool Success, string[] Errors)> RegisterUserAsync(RegisterViewModel model);
        Task<(bool Success, string[] Errors)> ConfirmOTPAsync(string email, string otp);
        Task<User> AuthenticateUserAsync(LoginViewModel model);
    }
}
