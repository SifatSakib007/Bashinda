using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BashindaAPI.Models
{
    public class AdminPermission
    {
        public int Id { get; set; }
        
        
        public int UserId { get; set; }
        
        [ForeignKey("UserId")]
        public User? User { get; set; }
        
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
} 