using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;

namespace Bashinda.ViewModels
{
    public class AdminViewModel
    {
        public int Id { get; set; } 
        public string UserName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string PhoneNumber { get; set; } = string.Empty;
        public DateTime CreatedDate { get; set; } = DateTime.Now;
        public AdminPermissionViewModel Permissions { get; set; } = new AdminPermissionViewModel();
        
        // Additional properties for UI display - not from API
        public string? LocationAccess { get; set; }
        public string? DivisionName { get; set; }
        public string? DistrictName { get; set; }
        public string? UpazilaName { get; set; }
        public string? WardName { get; set; }
        public string? VillageName { get; set; }
    }

    public class AdminListViewModel
    {
        public string Id { get; set; } = string.Empty;
        public string UserName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string PhoneNumber { get; set; } = string.Empty;
        public string? LocationAccess { get; set; }
        public bool HasApprovalPermissions { get; set; }
    }

    public class AdminPermissionViewModel
    {
        // Location-based access restrictions
        [Display(Name = "Division")]
        public string? Division { get; set; }
        
        [Display(Name = "District")]
        public string? District { get; set; }
        
        [Display(Name = "Upazila")]
        public string? Upazila { get; set; }
        
        [Display(Name = "Ward")]
        public string? Ward { get; set; }
        
        [Display(Name = "Village")]
        public string? Village { get; set; }
        
        // Field-level access permissions
        [Display(Name = "Can View User Name")]
        public bool CanViewUserName { get; set; } = true;
        
        [Display(Name = "Can View Email")]
        public bool CanViewEmail { get; set; } = true;
        
        [Display(Name = "Can View Phone")]
        public bool CanViewPhone { get; set; } = true;
        
        [Display(Name = "Can View Address")]
        public bool CanViewAddress { get; set; } = true;
        
        [Display(Name = "Can View Profile Image")]
        public bool CanViewProfileImage { get; set; } = true;
        
        [Display(Name = "Can View National ID")]
        public bool CanViewNationalId { get; set; } = false;
        
        [Display(Name = "Can View Birth Registration")]
        public bool CanViewBirthRegistration { get; set; } = false;
        
        [Display(Name = "Can View Date of Birth")]
        public bool CanViewDateOfBirth { get; set; } = true;
        
        [Display(Name = "Can View Family Info")]
        public bool CanViewFamilyInfo { get; set; } = false;
        
        [Display(Name = "Can View Profession")]
        public bool CanViewProfession { get; set; } = true;
        
        // Action permissions
        [Display(Name = "Can Approve Renters")]
        public bool CanApproveRenters { get; set; } = false;
        
        [Display(Name = "Can Approve Owners")]
        public bool CanApproveOwners { get; set; } = false;
        
        [Display(Name = "Can Manage Apartments")]
        public bool CanManageApartments { get; set; } = false;
    }

    public class CreateAdminViewModel
    {
        [Required]
        [StringLength(50, MinimumLength = 3)]
        [Display(Name = "User Name")]
        public string UserName { get; set; } = string.Empty;

        [Required]
        [EmailAddress]
        [Display(Name = "Email")]
        public string Email { get; set; } = string.Empty;

        [Required]
        [Phone]
        [Display(Name = "Phone Number")]
        public string PhoneNumber { get; set; } = string.Empty;

        [Required]
        [StringLength(100, MinimumLength = 6)]
        [DataType(DataType.Password)]
        [Display(Name = "Password")]
        public string Password { get; set; } = string.Empty;

        [Required]
        [DataType(DataType.Password)]
        [Display(Name = "Confirm Password")]
        [Compare("Password", ErrorMessage = "The password and confirmation password do not match.")]
        public string ConfirmPassword { get; set; } = string.Empty;
        
        // Permissions settings
        [Required]
        [Display(Name = "Permissions")]
        public AdminPermissionViewModel Permissions { get; set; } = new AdminPermissionViewModel();
        
        // Dropdown lists for locations (used in the view)
        public IEnumerable<SelectListItem> Divisions { get; set; } = new List<SelectListItem>();
        public IEnumerable<SelectListItem> Districts { get; set; } = new List<SelectListItem>();
        public IEnumerable<SelectListItem> Upazilas { get; set; } = new List<SelectListItem>();
        public IEnumerable<SelectListItem> Wards { get; set; } = new List<SelectListItem>();
        public IEnumerable<SelectListItem> Villages { get; set; } = new List<SelectListItem>();
    }

    public class UpdateAdminViewModel
    {
        public string Id { get; set; } = string.Empty;
        
        [Display(Name = "User Name")]
        public string UserName { get; set; } = string.Empty;
        
        [Display(Name = "Email")]
        public string Email { get; set; } = string.Empty;
        
        [Display(Name = "Phone Number")]
        public string PhoneNumber { get; set; } = string.Empty;
        
        [Required]
        [Display(Name = "Permissions")]
        public AdminPermissionViewModel Permissions { get; set; } = new AdminPermissionViewModel();
        
        // Dropdown lists for locations (used in the view)
        public IEnumerable<SelectListItem> Divisions { get; set; } = new List<SelectListItem>();
        public IEnumerable<SelectListItem> Districts { get; set; } = new List<SelectListItem>();
        public IEnumerable<SelectListItem> Upazilas { get; set; } = new List<SelectListItem>();
        public IEnumerable<SelectListItem> Wards { get; set; } = new List<SelectListItem>();
        public IEnumerable<SelectListItem> Villages { get; set; } = new List<SelectListItem>();
    }
} 