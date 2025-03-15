using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BashindaAPI.Models
{
    public class ApartmentOwnerProfile
    {
        public int Id { get; set; }
        
        [Required]
        public int UserId { get; set; }
        
        [Required]
        [StringLength(100)]
        [Display(Name = "Full Name")]
        public string FullName { get; set; } = string.Empty;
        
        [Required]
        [DataType(DataType.Date)]
        [Display(Name = "Date of Birth")]
        public DateTime DateOfBirth { get; set; }
        
        [Required]
        [StringLength(20)]
        [Display(Name = "National ID")]
        public string NationalId { get; set; } = string.Empty;
        
        [Display(Name = "National ID Image")]
        public string? NationalIdImagePath { get; set; }
        
        [Display(Name = "Profile Picture")]
        public string? SelfImagePath { get; set; }
        
        [Display(Name = "Contact Number")]
        [Phone]
        [StringLength(15)]
        public string? MobileNo { get; set; }
        
        [Display(Name = "Email Address")]
        [EmailAddress]
        [StringLength(100)]
        public string? Email { get; set; }
        
        // Permanent Resident Information
        [Required]
        [Display(Name = "Division")]
        public int DivisionId { get; set; }
        
        [Required]
        [Display(Name = "District")]
        public int DistrictId { get; set; }
        
        [Required]
        [Display(Name = "Upazila")]
        public int UpazilaId { get; set; }
        
        [Required]
        [Display(Name = "Area Type")]
        public AreaType AreaType { get; set; }
        
        [Required]
        [Display(Name = "Ward")]
        public int WardId { get; set; }
        
        [Required]
        [Display(Name = "Village/Area")]
        public int VillageId { get; set; }
        
        [Required]
        [StringLength(10)]
        [Display(Name = "Post Code")]
        public string PostCode { get; set; } = string.Empty;
        
        [Required]
        [StringLength(50)]
        [Display(Name = "Holding No")]
        public string HoldingNo { get; set; } = string.Empty;
        
        [Display(Name = "Profession")]
        [StringLength(100)]
        public string? Profession { get; set; }
        
        public bool IsApproved { get; set; } = false;
        
        // Reason for rejection if not approved
        public string? RejectionReason { get; set; }
        
        // Navigation properties
        [ForeignKey("UserId")]
        public User? User { get; set; }
        
        [ForeignKey("DivisionId")]
        public Division? Division { get; set; }
        
        [ForeignKey("DistrictId")]
        public District? District { get; set; }
        
        [ForeignKey("UpazilaId")]
        public Upazila? Upazila { get; set; }
        
        [ForeignKey("WardId")]
        public Ward? Ward { get; set; }
        
        [ForeignKey("VillageId")]
        public Village? Village { get; set; }
    }
} 