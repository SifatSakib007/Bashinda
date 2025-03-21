using Bashinda.Models;
using Bashinda.ViewModels;
using BashindaAPI.DTOs;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Bashinda.Services
{
    public interface IAdminApiService
    {
        Task<(bool Success, List<AdminViewModel> Data, string[] Errors)> GetAllAdminsAsync(string token);
        Task<(bool Success, AdminViewModel? Data, string[] Errors)> GetAdminByIdAsync(string id, string token);
        Task<(bool Success, AdminViewModel? Data, string[] Errors)> CreateAdminAsync(CreateAdminDto model, string token);
        Task<(bool Success, string[] Errors)> UpdateAdminPermissionsAsync(string id, AdminPermissionViewModel permissions, string token);
        Task<(bool Success, string[] Errors)> DeleteAdminAsync(string id, string token);

        // Added missing methods
        ///Task<(bool Success, AdminDashboardViewModel Dashboard, string[] Errors)> GetDashboardAsync(string token);
        Task<(bool Success, AdminDashboardViewModel Dashboard, string[] Errors)>
            GetDashboardAsync(string token, AdminLocationFilters filters);
        Task<(bool Success, List<Bashinda.ViewModels.RenterProfileListDto> PendingProfiles, string[] Errors)>
        GetPendingRenterProfilesAsync(string token);

        Task<(bool Success, List<Bashinda.ViewModels.OwnerProfileListDto> PendingProfiles, string[] Errors)>
            GetPendingOwnerProfilesAsync(string token);
        Task<(bool Success, string[] Errors)> ApproveRenterProfileAsync(int id, bool isApproved, string? rejectionReason, string token);
        Task<(bool Success, string[] Errors)> ApproveOwnerProfileAsync(int id, bool isApproved, string? rejectionReason, string token);
        Task<(bool Success, List<UserViewModel> Users, string[] Errors)> GetUsersAsync(string token);
        Task<(bool Success, string[] Errors)> DeleteUserAsync(int id, string token);

        Task<(bool Success, string[] Errors)> ProcessApproval(
            int profileId, bool isApproved, string reason, string token, AdminLocationFilters filters);

        Task<bool> VerifyProfileAccess(int profileId, string token, AdminLocationFilters filters);
        Task<(bool Success, AdminPermissionDto Permissions, string[] Errors)> GetAdminPermissionsAsync(int adminId);

        Task<(bool Success, List<ViewModels.RenterProfileListDto> Profiles, string[] Errors)>
        GetPendingProfilesAsync(string token, AdminLocationFilters filters);
    }
} 