using BashindaAPI.DTOs;
using BashindaAPI.Models;

namespace BashindaAPI.Services
{
    public interface IAdminService
    {
        Task<(bool Success, AdminDto? Admin, string[] Errors)> CreateAdminAsync(CreateAdminDto model);
        Task<(bool Success, AdminDto? Admin, string[] Errors)> GetAdminByIdAsync(int id);
        Task<(bool Success, List<AdminListDto> Admins, string[] Errors)> GetAllAdminsAsync();
        Task<(bool Success, List<RenterProfileDTO> Renters, string[] Errors)> GetAllRentersAsync();
        Task<(bool Success, List<ApartmentOwnerProfileDto> Owners, string[] Errors)> GetAllOwnersAsync();
        Task<(bool Success, string[] Errors)> UpdateAdminPermissionsAsync(UpdateAdminPermissionsDto model);
        Task<(bool Success, string[] Errors)> DeleteAdminAsync(int id);
        
        // Methods for filtering users based on admin permissions
        Task<bool> CanAdminAccessUserAsync(int adminId, int userId);
        Task<bool> CanAdminAccessLocationAsync(int adminId, string division, string district, string upazila, string? ward = null, string? village = null);
        Task<(bool Success, ApartmentOwnerProfileDto? Profile, string[] Errors)> GetOwnerByUserIdAsync(string userId);


    }
} 