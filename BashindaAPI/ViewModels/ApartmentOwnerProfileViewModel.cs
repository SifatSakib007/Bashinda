using System.ComponentModel.DataAnnotations;
using BashindaAPI.Models;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace BashindaAPI.ViewModels
{
    public class ApartmentOwnerProfileViewModel
    {
        public int Id { get; set; }
        
        // User Details
        public int UserId { get; set; }
        
        // Personal Information
        [Required]
        [StringLength(100)]
        public string FullName { get; set; } = string.Empty;
        
        [Required]
        public DateTime DateOfBirth { get; set; }
        
        [Required]
        [StringLength(20)]
        public string NationalId { get; set; } = string.Empty;
        
        public string? NationalIdImagePath { get; set; }
        
        public string? SelfImagePath { get; set; }
        
        [Required]
        [StringLength(20)]
        public string MobileNo { get; set; } = string.Empty;
        
        [Required]
        [EmailAddress]
        [StringLength(100)]
        public string Email { get; set; } = string.Empty;
        
        [Required]
        [StringLength(100)]
        public string Profession { get; set; } = string.Empty;
        
        // Location Information
        public LocationViewModel Location { get; set; } = new LocationViewModel();
        
        // Approval Status
        public bool IsApproved { get; set; }
        public string? RejectionReason { get; set; }
    }
} 