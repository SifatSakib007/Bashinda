using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Bashinda.Models
{
    public class RenterProfile
    {
        [Key]
        public int Id { get; set; }
        
        public int UserId { get; set; }
        
        [ForeignKey("UserId")]
        public ApplicationUser User { get; set; }
        
        public bool IsAdult { get; set; }
        
        public string? NationalId { get; set; }
        
        public string? NationalIdImagePath { get; set; }
        
        public string? BirthRegistrationNo { get; set; }
        
        public string? BirthRegistrationImagePath { get; set; }
        
        public DateTime DateOfBirth { get; set; }
        
        [Required]
        [StringLength(100)]
        public string FullName { get; set; } = string.Empty;
        
        [Required]
        [StringLength(100)]
        public string FatherName { get; set; } = string.Empty;
        
        [Required]
        [StringLength(100)]
        public string MotherName { get; set; } = string.Empty;
        
        [Required]
        [StringLength(50)]
        public string Nationality { get; set; } = string.Empty;
        
        [Required]
        [StringLength(50)]
        public string BloodGroup { get; set; } = string.Empty;
        
        [Required]
        [StringLength(50)]
        public string Profession { get; set; } = string.Empty;
        
        [Required]
        [StringLength(50)]
        public string Gender { get; set; } = string.Empty;
        
        [Required]
        [StringLength(20)]
        public string MobileNo { get; set; } = string.Empty;
        
        [Required]
        [EmailAddress]
        [StringLength(100)]
        public string Email { get; set; } = string.Empty;
        
        public string? SelfImagePath { get; set; }
        
        // Location information
        [Required]
        [StringLength(100)]
        public string Division { get; set; } = string.Empty;
        
        [Required]
        [StringLength(100)]
        public string District { get; set; } = string.Empty;
        
        [Required]
        [StringLength(100)]
        public string Upazila { get; set; } = string.Empty;
        
        [Required]
        [StringLength(50)]
        public string AreaType { get; set; } = string.Empty;
        
        [Required]
        [StringLength(100)]
        public string Ward { get; set; } = string.Empty;
        
        [Required]
        [StringLength(100)]
        public string Village { get; set; } = string.Empty;
        
        [Required]
        [StringLength(10)]
        public string PostCode { get; set; } = string.Empty;
        
        [Required]
        [StringLength(50)]
        public string HoldingNo { get; set; } = string.Empty;
        
        // Approval status
        public bool IsApproved { get; set; }
        
        public string? RejectionReason { get; set; }
        
        // Audit fields
        public DateTime CreatedAt { get; set; } = DateTime.Now;
        
        public DateTime? ApprovedAt { get; set; }
    }
}
