using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Http;

namespace BashindaAPI.DTOs
{
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
        [Display(Name = "Building Name")]
        public string BuildingName { get; set; } = string.Empty;
        
        [Required]
        [StringLength(200)]
        [Display(Name = "Address")]
        public string Address { get; set; } = string.Empty;
        
        [Required]
        [StringLength(50)]
        [Display(Name = "Apartment Number")]
        public string ApartmentNumber { get; set; } = string.Empty;
        
        [Required]
        [Range(1, 20)]
        [Display(Name = "Number of Rooms")]
        public int NumberOfRooms { get; set; }
        
        [Required]
        [Range(1, 10)]
        [Display(Name = "Number of Bathrooms")]
        public int NumberOfBathrooms { get; set; }
        
        [Required]
        [Range(0, 10000)]
        [Display(Name = "Square Feet")]
        public double SquareFeet { get; set; }
        
        [Required]
        [Range(0, 1000000)]
        [Display(Name = "Monthly Rent")]
        [DataType(DataType.Currency)]
        public decimal MonthlyRent { get; set; }
        
        [Display(Name = "Description")]
        public string? Description { get; set; }
    }

    public class UpdateApartmentDto
    {
        [StringLength(100)]
        [Display(Name = "Building Name")]
        public string BuildingName { get; set; } = string.Empty;
        
        [StringLength(200)]
        [Display(Name = "Address")]
        public string Address { get; set; } = string.Empty;
        
        [StringLength(50)]
        [Display(Name = "Apartment Number")]
        public string ApartmentNumber { get; set; } = string.Empty;
        
        [Range(1, 20)]
        [Display(Name = "Number of Rooms")]
        public int NumberOfRooms { get; set; }
        
        [Range(1, 10)]
        [Display(Name = "Number of Bathrooms")]
        public int NumberOfBathrooms { get; set; }
        
        [Range(0, 10000)]
        [Display(Name = "Square Feet")]
        public double SquareFeet { get; set; }
        
        [Range(0, 1000000)]
        [Display(Name = "Monthly Rent")]
        [DataType(DataType.Currency)]
        public decimal MonthlyRent { get; set; }
        
        [Display(Name = "Is Available")]
        public bool IsAvailable { get; set; }
        
        [Display(Name = "Description")]
        public string? Description { get; set; }
    }

    public class UploadApartmentImageDto
    {
        [Required]
        public IFormFile Image { get; set; }
    }
}
