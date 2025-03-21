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
        [StringLength(100)]
        public string Division { get; set; } = string.Empty;
        
        [Required]
        [Display(Name = "District")]
        [StringLength(100)]
        public string District { get; set; } = string.Empty;
        
        [Required]
        [Display(Name = "Upazila")]
        [StringLength(100)]
        public string Upazila { get; set; } = string.Empty;
        
        [Required]
        [Display(Name = "Area Type")]
        public AreaType AreaType { get; set; }
        
        [Required]
        [Display(Name = "Ward")]
        [StringLength(100)]
        public string Ward { get; set; } = string.Empty;
        
        [Required]
        [Display(Name = "Village/Area")]
        [StringLength(100)]
        public string Village { get; set; } = string.Empty;
        
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
    }
} 