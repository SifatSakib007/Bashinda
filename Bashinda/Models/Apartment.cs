using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Bashinda.Models
{
    public class Apartment
    {
        public int Id { get; set; }
        
        [Required]
        public int OwnerId { get; set; }  // References ApartmentOwnerProfile.Id
        
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
        
        [Display(Name = "Is Available")]
        public bool IsAvailable { get; set; } = true;
        
        [Display(Name = "Apartment Image")]
        public string? ImagePath { get; set; }
        
        [DataType(DataType.MultilineText)]
        [Display(Name = "Description")]
        public string? Description { get; set; }
        
        [DataType(DataType.DateTime)]
        [Display(Name = "Created At")]
        public DateTime CreatedAt { get; set; } = DateTime.Now;
        
        // Navigation property
        [ForeignKey("OwnerId")]
        public virtual ApartmentOwnerProfile? Owner { get; set; }
    }
} 