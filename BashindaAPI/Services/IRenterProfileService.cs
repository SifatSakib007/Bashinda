using BashindaAPI.DTOs;

namespace BashindaAPI.Services
{
    public interface IRenterProfileService
    {
        Task<(bool Success, RenterProfileDTO? Data, string[] Errors)> GetByIdAsync(int id);
        Task<(bool Success, RenterProfileDTO? Data, string[] Errors)> GetByUserIdAsync(int userId);
        Task<(bool Success, List<RenterProfileDTO> Data, int TotalCount, string[] Errors)> GetAllAsync(int page = 1, int pageSize = 10, bool approvedOnly = false);
        Task<(bool Success, List<RenterProfileDTO> Data, int TotalCount, string[] Errors)> GetPendingApprovalsAsync(int page = 1, int pageSize = 10);
        Task<(bool Success, RenterProfileDTO? Data, string[] Errors)> CreateAsync(CreateRenterProfileDto model);
        Task<(bool Success, RenterProfileDTO? Data, string[] Errors)> UpdateAsync(int id, UpdateRenterProfileDto model);
        Task<(bool Success, string[] Errors)> UpdateApprovalStatusAsync(int id, bool isApproved);
        Task<(bool Success, string[] Errors)> DeleteAsync(int id);
    }
} 