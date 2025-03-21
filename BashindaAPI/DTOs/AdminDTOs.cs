using System.ComponentModel.DataAnnotations;
using BashindaAPI.Models;

namespace BashindaAPI.DTOs
{
    public class AdminDashboardDto
    {
        public int PendingRenters { get; set; }
        public int PendingOwners { get; set; }
        public int TotalUsers { get; set; }
        public int ApprovedRenters { get; set; }
        public int TotalRenters { get; set; }
        public int ApprovedOwners { get; set; }
        public int TotalOwners { get; set; }
        public int TotalApartments { get; set; }
        public int RenterApprovalPercentage { get; set; }
        public int OwnerApprovalPercentage { get; set; }
    }
    
    public class ApproveProfileDto
    {
        public bool IsApproved { get; set; }
        public string? Reason { get; set; }
    }

    public class CreateAdminDto
    {
        [Required]
        [StringLength(50, MinimumLength = 3)]
        public string UserName { get; set; } = string.Empty;

        [Required]
        [EmailAddress]
        public string Email { get; set; } = string.Empty;

        [Required]
        [Phone]
        public string PhoneNumber { get; set; } = string.Empty;

        [Required]
        [StringLength(100, MinimumLength = 6)]
        public string Password { get; set; } = string.Empty;

        [Required]
        [Compare("Password")]
        public string ConfirmPassword { get; set; } = string.Empty;
        
        // Permission settings
        [Required]
        public AdminPermissionDto Permissions { get; set; } = new AdminPermissionDto();
    }
    
    public class AdminPermissionDto
    {
        // Location-based access restrictions
        public string? Division { get; set; }
        public string? District { get; set; }
        public string? Upazila { get; set; }
        public string? Ward { get; set; }
        public string? Village { get; set; }
        
        // Field-level access permissions
        public bool CanViewUserName { get; set; } = true;
        public bool CanViewEmail { get; set; } = true;
        public bool CanViewPhone { get; set; } = true;
        public bool CanViewAddress { get; set; } = true;
        public bool CanViewProfileImage { get; set; } = true;
        public bool CanViewNationalId { get; set; } = false;
        public bool CanViewBirthRegistration { get; set; } = false;
        public bool CanViewDateOfBirth { get; set; } = true;
        public bool CanViewFamilyInfo { get; set; } = false;
        public bool CanViewProfession { get; set; } = true;
        
        // Action permissions
        public bool CanApproveRenters { get; set; } = false;
        public bool CanApproveOwners { get; set; } = false;
        public bool CanManageApartments { get; set; } = false;
    }
    
    public class AdminDto
    {
        public int Id { get; set; }
        public string UserName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string PhoneNumber { get; set; } = string.Empty;
        public AdminPermissionDto Permissions { get; set; } = new AdminPermissionDto();
    }
    
    public class UpdateAdminPermissionsDto
    {
        [Required]
        public int AdminId { get; set; }
        
        [Required]
        public AdminPermissionDto Permissions { get; set; } = new AdminPermissionDto();
    }
    
    public class AdminListDto
    {
        public int Id { get; set; }
        public string UserName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string PhoneNumber { get; set; } = string.Empty;
        
        // Summary of permissions for UI display
        public string? LocationAccess { get; set; }
        public bool HasApprovalPermissions { get; set; }
    }
} 