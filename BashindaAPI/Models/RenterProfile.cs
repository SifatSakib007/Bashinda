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

        // Permanent Resident Information - Basic (already in the model)
        [Required]
        public int DivisionId { get; set; }
        
        [Required]
        public int DistrictId { get; set; }
        
        [Required]
        public int UpazilaId { get; set; }
        
        // New Permanent Resident Information
        [Required]
        public AreaType AreaType { get; set; }
        
        [Required]
        public int WardId { get; set; }
        
        [Required]
        public int VillageId { get; set; }
        
        [Required]
        [StringLength(10)]
        public string PostCode { get; set; } = string.Empty;
        
        [Required]
        [StringLength(50)]
        public string HoldingNo { get; set; } = string.Empty;

        // Approval flag – user access granted after admin approval
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