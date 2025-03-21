using Bashinda.ViewModels;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;

namespace Bashinda.Services
{
    public interface IAdminLocationService
    {
        Task<AdminLocationFilters> GetAdminFiltersAsync(int adminId);

        string FormatLocationScope(AdminLocationFilters filters);
    }

    public class AdminLocationService : IAdminLocationService
    {
        private readonly IAdminApiService _adminApiService;

        public AdminLocationService(IAdminApiService adminApiService)
        {
            _adminApiService = adminApiService;
        }

        public async Task<AdminLocationFilters> GetAdminFiltersAsync(int adminId)
        {
            var (success, permissions, errors) = await _adminApiService.GetAdminPermissionsAsync(adminId);

            if (!success || permissions == null)
            {
                return new AdminLocationFilters();
            }

            return new AdminLocationFilters
            {
                Division = permissions.Division,
                District = permissions.District,
                Upazila = permissions.Upazila,
                Ward = permissions.Ward,
                Village = permissions.Village
            };
        }


        public string FormatLocationScope(AdminLocationFilters filters)
        {
            var scope = new List<string>();

            if (!string.IsNullOrEmpty(filters.Division))
                scope.Add($"Division: {filters.Division}");

            if (!string.IsNullOrEmpty(filters.District))
                scope.Add($"District: {filters.District}");

            if (!string.IsNullOrEmpty(filters.Upazila))
                scope.Add($"Upazila: {filters.Upazila}");

            if (!string.IsNullOrEmpty(filters.Ward))
                scope.Add($"Ward: {filters.Ward}");

            if (!string.IsNullOrEmpty(filters.Village))
                scope.Add($"Village: {filters.Village}");

            return scope.Count > 0
                ? string.Join(" > ", scope)
                : "Full System Access";
        }
    }
}