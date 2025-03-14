using Microsoft.AspNet.Identity.EntityFramework;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Bashinda.Data
{
    public class ApartmentOwnerProfile
    {
        public int Id { get; set; }
        
        [Required]
        public string UserId { get; set; } = string.Empty;
        
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
        
        [Display(Name = "Address")]
        [StringLength(200)]
        public string? Address { get; set; }
        
        [Display(Name = "Profession")]
        [StringLength(100)]
        public string? Profession { get; set; }
        
        public bool IsApproved { get; set; } = false;
        
        // Navigation property
        [ForeignKey("UserId")]
        public virtual IdentityUser? User { get; set; }
    }
} 