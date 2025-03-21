namespace Bashinda.Models
{
    // This is a view model representation of the AdminPermission from the API
    public class AdminPermission
    {
        public string UserId { get; set; } = string.Empty;
        
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
        
        // Audit fields
        public DateTime CreatedDate { get; set; } = DateTime.UtcNow;
        public string? CreatedBy { get; set; }
        public DateTime? UpdatedDate { get; set; }
        public string? UpdatedBy { get; set; }
    }
} 