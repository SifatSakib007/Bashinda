using System;

namespace Bashinda.ViewModels
{
    public class OwnerProfileListDto
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public string? UniqueId { get; set; }
        public bool IsAdult { get; set; }

        public string FullName { get; set; } = string.Empty;
        public string MobileNo { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string? NationalId { get; set; }
        public string? NationalIdImagePath { get; set; }
        public string? BirthRegistrationNo { get; set; }
        public string? BirthRegistrationImagePath { get; set; }
        public string FatherName { get; set; } = string.Empty;
        public string MotherName { get; set; } = string.Empty;
        public string Nationality { get; set; } = string.Empty;
        public string BloodGroup { get; set; } = string.Empty;
        public string Profession { get; set; } = string.Empty;
        public string Gender { get; set; } = string.Empty;

        public DateTime DateOfBirth { get; set; }
        public string? SelfImagePath { get; set; }
        public bool IsApproved { get; set; }
        
        // Location information
        public string Division { get; set; } = string.Empty;
        public string District { get; set; } = string.Empty;
        public string Upazila { get; set; } = string.Empty;
        public string AreaType { get; set; } = string.Empty;
        public string Ward { get; set; } = string.Empty;
        public string Village { get; set; } = string.Empty;
        public string PostCode { get; set; } = string.Empty;
        public string HoldingNo { get; set; } = string.Empty;
        public string? UniqueId { get; set; }
        public string? RejectionReason { get; set; }
        public UserDto? User { get; set; }
    }
} 