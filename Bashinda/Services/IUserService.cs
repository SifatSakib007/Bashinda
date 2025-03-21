using Bashinda.Models;
using Bashinda.ViewModels;
using System.Threading.Tasks;

namespace Bashinda.Services
{
    public interface IUserService
    {
        Task<(bool Success, string[] Errors)> RegisterUserAsync(RegisterViewModel model);
        Task<(bool Success, string[] Errors)> ConfirmOTPAsync(string email, string otp);
        Task<ApplicationUser?> AuthenticateUserAsync(LoginViewModel model);
    }
}
