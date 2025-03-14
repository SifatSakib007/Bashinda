using Bashinda.ViewModels;

namespace Bashinda.Services
{
    public interface IRenterProfileService
    {
        Task<ServiceResponse<RenterProfileViewModel>> SaveRenterProfileAsync(int userId, RenterProfileViewModel model);
        Task<ServiceResponse<RenterProfileViewModel>> GetRenterProfileAsync(int userId);
        Task<ServiceResponse<RenterProfileViewModel>> CreateRenterProfileAsync(int userId, RenterProfileViewModel model);
    }
}
