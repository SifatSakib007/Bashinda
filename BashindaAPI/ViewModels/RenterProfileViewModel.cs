using System.ComponentModel.DataAnnotations;
using BashindaAPI.Models;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace BashindaAPI.ViewModels
{
    public class RenterProfileViewModel
    {
        public int Id { get; set; }
        
        // User Details
        public int UserId { get; set; }
        
        // Adult/Child Information
        public bool IsAdult { get; set; }
        
        // Adult identification
        public string? NationalId { get; set; }
        public string? NationalIdImagePath { get; set; }
        
        // Child identification
        public string? BirthRegistrationNo { get; set; }
        public string? BirthRegistrationImagePath { get; set; }
        
        // Common personal information
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
        
        // Location Information
        public LocationViewModel Location { get; set; } = new LocationViewModel();
        
        // Approval Status
        public bool IsApproved { get; set; }
        public string? RejectionReason { get; set; }
    }
} 