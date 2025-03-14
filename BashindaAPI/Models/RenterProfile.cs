using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BashindaAPI.Models
{
    public class RenterProfile
    {
        [Key]
        public int Id { get; set; }

        // FK to User table
        [Required]
        public int UserId { get; set; }

        // Adult condition
        public bool IsAdult { get; set; }

        // If Adult: NID information
        public string? NationalId { get; set; }
        public string? NationalIdImagePath { get; set; }

        // If Not Adult: Birth Registration information
        public string? BirthRegistrationNo { get; set; }
        public string? BirthRegistrationImagePath { get; set; }

        // Common fields
        [Required]
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

        // Nationality, Blood Group, Profession, Gender as enums:
        [Required]
        public Nationality Nationality { get; set; }
        
        [Required]
        public BloodGroup BloodGroup { get; set; }
        
        [Required]
        public Profession Profession { get; set; }
        
        [Required]
        public Gender Gender { get; set; }

        [Required]
        [StringLength(20)]
        public string MobileNo { get; set; } = string.Empty;
        
        [Required]
        [EmailAddress]
        [StringLength(100)]
        public string Email { get; set; } = string.Empty;
        
        public string? SelfImagePath { get; set; }

        // Permanent Resident Information
        [Required]
        [StringLength(50)]
        public string Division { get; set; } = string.Empty;
        
        [Required]
        [StringLength(50)]
        public string District { get; set; } = string.Empty;
        
        [Required]
        [StringLength(50)]
        public string Upazila { get; set; } = string.Empty;

        // Approval flag – user access granted after admin approval
        public bool IsApproved { get; set; } = false;
        
        // Reason for rejection if not approved
        public string? RejectionReason { get; set; }

        // Navigation property
        [ForeignKey("UserId")]
        public User? User { get; set; }
    }

    // Enums for the dropdowns and bullet selections:
    public enum Nationality
    {
        Bangladeshi,
        Foreigner
    }

    public enum BloodGroup
    {
        APositive,
        ANegative,
        BPositive,
        BNegative,
        ABPositive,
        ABNegative,
        OPositive,
        ONegative
    }

    public enum Profession
    {
        Unemployed,
        Student,
        Employed,
        SelfEmployed,
        Other
    }

    public enum Gender
    {
        Male,
        Female,
        Other
    }
} 