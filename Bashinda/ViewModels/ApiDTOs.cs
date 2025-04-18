﻿using Bashinda.Models;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace Bashinda.ViewModels
{
    // Authentication DTOs
    public class LoginRequestDto
    {
        [Phone]
        public string? PhoneNumber { get; set; }

        [EmailAddress]
        public string? Email { get; set; }

        [Required]
        public string Password { get; set; } = string.Empty;
    }

    public class AdminPermissionDto
    {
        // Location-based access restrictions
        [JsonPropertyName("division")]
        public string? Division { get; set; }

        [JsonPropertyName("district")]
        public string? District { get; set; }

        [JsonPropertyName("upazila")]
        public string? Upazila { get; set; }

        [JsonPropertyName("ward")]
        public string? Ward { get; set; }

        [JsonPropertyName("village")]
        public string? Village { get; set; }

        // Field-level access permissions
        [JsonPropertyName("canViewUserName")]
        public bool CanViewUserName { get; set; } 

        [JsonPropertyName("canViewEmail")]
        public bool CanViewEmail { get; set; } 

        [JsonPropertyName("canViewPhone")]
        public bool CanViewPhone { get; set; } 

        [JsonPropertyName("canViewAddress")]
        public bool CanViewAddress { get; set; } 

        [JsonPropertyName("canViewProfileImage")]
        public bool CanViewProfileImage { get; set; } 

        [JsonPropertyName("canViewNationalId")]
        public bool CanViewNationalId { get; set; }

        [JsonPropertyName("canViewBirthRegistration")]
        public bool CanViewBirthRegistration { get; set; }

        [JsonPropertyName("canViewDateOfBirth")]
        public bool CanViewDateOfBirth { get; set; }

        [JsonPropertyName("canViewFamilyInfo")]
        public bool CanViewFamilyInfo { get; set; }

        [JsonPropertyName("canViewProfession")]
        public bool CanViewProfession { get; set; }

        // Action permissions
        [JsonPropertyName("canApproveRenters")]
        public bool CanApproveRenters { get; set; }

        [JsonPropertyName("canApproveOwners")]
        public bool CanApproveOwners { get; set; }

        [JsonPropertyName("canManageApartments")]
        public bool CanManageApartments { get; set; }



        // Add remaining properties with same pattern...
    }

    // User DTOs
    public class UserDto
    {
        public int Id { get; set; }
        public string UserName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string PhoneNumber { get; set; } = string.Empty;
        public string Role { get; set; } = string.Empty;
        public bool IsVerified { get; set; }
        public List<string> Roles { get; set; } = new List<string>();
    }

    public class RegisterUserDto
    {
        [Required]
        [StringLength(50, MinimumLength = 3)]
        public string UserName { get; set; } = string.Empty;

        [Required]
        [EmailAddress]
        public string Email { get; set; } = string.Empty;

        [Required]
        [Phone]
        public string PhoneNumber { get; set; } = string.Empty;

        [Required]
        [StringLength(100, MinimumLength = 6)]
        public string Password { get; set; } = string.Empty;

        [Required]
        [Compare("Password")]
        public string ConfirmPassword { get; set; } = string.Empty;

        [Required]
        public string Role { get; set; } = string.Empty;
    }

    /// <summary>
    /// RenterProfile DTOs
    /// </summary>
    public class RenterProfileDto
    {
        public int Id { get; set; }
        public string? UniqueId { get; set; }
        public int UserId { get; set; }
        public bool IsAdult { get; set; }
        public string? NationalId { get; set; }
        public string? NationalIdImagePath { get; set; }
        public string? BirthRegistrationNo { get; set; }
        public string? BirthRegistrationImagePath { get; set; }
        public DateTime DateOfBirth { get; set; }
        public string FullName { get; set; } = string.Empty;
        public string FatherName { get; set; } = string.Empty;
        public string MotherName { get; set; } = string.Empty;
        public string Nationality { get; set; } = string.Empty;
        public string BloodGroup { get; set; } = string.Empty;
        public string Profession { get; set; } = string.Empty;
        public string Gender { get; set; } = string.Empty;
        public string MobileNo { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string? SelfImagePath { get; set; }
        public string Division { get; set; } = string.Empty;
        public string District { get; set; } = string.Empty;
        public string Upazila { get; set; } = string.Empty;
        public bool IsApproved { get; set; }
        public UserDto? User { get; set; }
    }

    public class RenterProfileListDto
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

    public class CreateRenterProfileDto
    {
        [Required]
        public bool IsAdult { get; set; }
        
        public string? NationalId { get; set; }
        
        public string? BirthRegistrationNo { get; set; }
        
        [Required]
        public DateTime DateOfBirth { get; set; }
        
        [Required]
        public string FullName { get; set; } = string.Empty;
        
        [Required]
        public string FatherName { get; set; } = string.Empty;
        
        [Required]
        public string MotherName { get; set; } = string.Empty;
        
        [Required]
        public string Nationality { get; set; } = string.Empty;
        
        [Required]
        public string BloodGroup { get; set; } = string.Empty;
        
        [Required]
        public string Profession { get; set; } = string.Empty;
        
        [Required]
        public string Gender { get; set; } = string.Empty;
        
        [Required]
        public string MobileNo { get; set; } = string.Empty;
        
        [Required]
        public string Email { get; set; } = string.Empty;
        
        // Location information - string-based fields
        public string Division { get; set; } = string.Empty;
        public string District { get; set; } = string.Empty;
        public string Upazila { get; set; } = string.Empty;
        public string AreaType { get; set; } = string.Empty;
        public string Ward { get; set; } = string.Empty;
        public string Village { get; set; } = string.Empty;
        public string PostCode { get; set; } = string.Empty;
        public string HoldingNo { get; set; } = string.Empty;
    }

    public class ApproveRenterProfileDto
    {
        public bool IsApproved { get; set; }
    }

    // ApartmentOwnerProfile DTOs
    public class ApartmentOwnerProfileDto
    {
        public int Id { get; set; }
        public string? UniqueId { get; set; }
        public int UserId { get; set; }
        public bool IsAdult { get; set; }
        public string? NationalId { get; set; }
        public string? NationalIdImagePath { get; set; }
        public string? BirthRegistrationNo { get; set; }
        public string? BirthRegistrationImagePath { get; set; }
        public DateTime DateOfBirth { get; set; }
        public string FullName { get; set; } = string.Empty;
        public string FatherName { get; set; } = string.Empty;
        public string MotherName { get; set; } = string.Empty;
        public Nationality Nationality { get; set; } 
        public BloodGroup BloodGroup { get; set; }
        public Profession Profession { get; set; } 
        public Gender Gender { get; set; } 
        public string MobileNo { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string? SelfImagePath { get; set; }
        public string Division { get; set; } = string.Empty;
        public string District { get; set; } = string.Empty;
        public string Upazila { get; set; } = string.Empty;
        public AreaType AreaType { get; set; } 
        public string Ward { get; set; } = string.Empty;
        public string Village { get; set; } = string.Empty;
        public string PostCode { get; set; } = string.Empty;
        public string HoldingNo { get; set; } = string.Empty;

        public bool IsApproved { get; set; }
        public UserDto? User { get; set; }
    }

    public class ApartmentOwnerProfileListDto
    {
        public int Id { get; set; }
        public string FullName { get; set; } = string.Empty;
        public string? MobileNo { get; set; }
        public string? Email { get; set; }
        public string? NationalId { get; set; }
        public string? SelfImagePath { get; set; }
        public bool IsApproved { get; set; }
    }

    public class CreateApartmentOwnerProfileDto
    {
        [Required]
        public bool IsAdult { get; set; }

        public string? NationalId { get; set; }

        public string? BirthRegistrationNo { get; set; }

        [Required]
        public DateTime DateOfBirth { get; set; }

        [Required]
        public string FullName { get; set; } = string.Empty;

        [Required]
        public string FatherName { get; set; } = string.Empty;

        [Required]
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
        public string MobileNo { get; set; } = string.Empty;

        [Required]
        public string Email { get; set; } = string.Empty;

        // Location information - string-based fields
        public string Division { get; set; } = string.Empty;
        public string District { get; set; } = string.Empty;
        public string Upazila { get; set; } = string.Empty;
        [Required]
        public AreaType AreaType { get; set; } 
        public string Ward { get; set; } = string.Empty;
        public string Village { get; set; } = string.Empty;
        public string PostCode { get; set; } = string.Empty;
        public string HoldingNo { get; set; } = string.Empty;

    }

    public class ApproveApartmentOwnerProfileDto
    {
        public bool IsApproved { get; set; }
    }

    // Apartment DTOs
    public class ApartmentDto
    {
        public int Id { get; set; }
        public int OwnerId { get; set; }
        public string BuildingName { get; set; } = string.Empty;
        public string Address { get; set; } = string.Empty;
        public string ApartmentNumber { get; set; } = string.Empty;
        public int NumberOfRooms { get; set; }
        public int NumberOfBathrooms { get; set; }
        public double SquareFeet { get; set; }
        public decimal MonthlyRent { get; set; }
        public bool IsAvailable { get; set; }
        public string? ImagePath { get; set; }
        public string? Description { get; set; }
        public DateTime CreatedAt { get; set; }
        public ApartmentOwnerProfileListDto? Owner { get; set; }
    }

    public class ApartmentListDto
    {
        public int Id { get; set; }
        public string BuildingName { get; set; } = string.Empty;
        public string Address { get; set; } = string.Empty;
        public string ApartmentNumber { get; set; } = string.Empty;
        public int NumberOfRooms { get; set; }
        public decimal MonthlyRent { get; set; }
        public bool IsAvailable { get; set; }
        public string? ImagePath { get; set; }
        public string OwnerName { get; set; } = string.Empty;
    }

    public class CreateApartmentDto
    {
        [Required]
        [StringLength(100)]
        public string BuildingName { get; set; } = string.Empty;
        
        [Required]
        [StringLength(200)]
        public string Address { get; set; } = string.Empty;
        
        [Required]
        [StringLength(50)]
        public string ApartmentNumber { get; set; } = string.Empty;
        
        [Required]
        [Range(1, 20)]
        public int NumberOfRooms { get; set; }
        
        [Required]
        [Range(1, 10)]
        public int NumberOfBathrooms { get; set; }
        
        [Required]
        [Range(0, 10000)]
        public double SquareFeet { get; set; }
        
        [Required]
        [Range(0, 1000000)]
        public decimal MonthlyRent { get; set; }
        
        [Display(Name = "Description")]
        public string? Description { get; set; }
    }

    // Admin DTOs
    public class AdminDashboardDto
    {
        public int PendingRenters { get; set; }
        public int PendingOwners { get; set; }
        public int TotalUsers { get; set; }
        public int ApprovedRenters { get; set; }
        public int TotalRenters { get; set; }
        public int ApprovedOwners { get; set; }
        public int TotalOwners { get; set; }
        public int TotalApartments { get; set; }
        public int RenterApprovalPercentage { get; set; }
        public int OwnerApprovalPercentage { get; set; }
    }
    
    public class ApproveProfileDto
    {
        public bool IsApproved { get; set; }
        public string? Reason { get; set; }
    }

    // API Response
    public class ApiResponse<T>
    {
        public bool Success { get; set; }
        public T? Data { get; set; }
        public string[] Errors { get; set; } = Array.Empty<string>();
    }
    public class ValidationProblemDetailsDto
    {
        public string Title { get; set; } = string.Empty;
        public int Status { get; set; }
        public IDictionary<string, string[]> Errors { get; set; } = new Dictionary<string, string[]>();
    }

}