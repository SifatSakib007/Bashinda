using System;

namespace Bashinda.ViewModels
{
    public class OwnerProfileListDto
    {
        public int Id { get; set; }
        public string FullName { get; set; } = string.Empty;
        public string MobileNo { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string? NationalId { get; set; }
        public string? BirthRegistrationNo { get; set; }
        public DateTime DateOfBirth { get; set; }
        public string? SelfImagePath { get; set; }
        public bool IsApproved { get; set; }
        
        // Location information
        public string Division { get; set; } = string.Empty;
        public string District { get; set; } = string.Empty;
        public string Upazila { get; set; } = string.Empty;
    }
} 