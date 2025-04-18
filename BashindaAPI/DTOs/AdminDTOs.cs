using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

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
        [JsonPropertyName("division")]
        public string? Division { get; set; }

        [JsonPropertyName("district")]
        public string? District { get; set; }

        [JsonPropertyName("upazila")]
        public string? Upazila { get; set; }

        [JsonPropertyName("ward")]
        public string? Ward { get; set; }

        [JsonPropertyName("village")]
        public string? Village { get; set; }

        // Field-level access permissions
        [JsonPropertyName("canViewUserName")]
        public bool CanViewUserName { get; set; } = true;

        [JsonPropertyName("canViewEmail")]
        public bool CanViewEmail { get; set; } = true;

        [JsonPropertyName("canViewPhone")]
        public bool CanViewPhone { get; set; } = true;

        [JsonPropertyName("canViewAddress")]
        public bool CanViewAddress { get; set; } = true;

        [JsonPropertyName("canViewProfileImage")]
        public bool CanViewProfileImage { get; set; } = true;

        [JsonPropertyName("canViewNationalId")]
        public bool CanViewNationalId { get; set; } 

        [JsonPropertyName("canViewBirthRegistration")]
        public bool CanViewBirthRegistration { get; set; } 

        [JsonPropertyName("canViewDateOfBirth")]
        public bool CanViewDateOfBirth { get; set; } 

        [JsonPropertyName("canViewFamilyInfo")]
        public bool CanViewFamilyInfo { get; set; } 

        [JsonPropertyName("canViewProfession")]
        public bool CanViewProfession { get; set; } 

        // Action permissions
        [JsonPropertyName("canApproveRenters")]
        public bool CanApproveRenters { get; set; } 

        [JsonPropertyName("canApproveOwners")]
        public bool CanApproveOwners { get; set; } 

        [JsonPropertyName("canManageApartments")]
        public bool CanManageApartments { get; set; } 
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