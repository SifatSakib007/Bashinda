using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BashindaAPI.Models
{
    // Division (Top level: Dhaka, Khulna, etc.)
    public class Division
    {
        [Key]
        public int Id { get; set; }
        
        [Required]
        [StringLength(50)]
        public string Name { get; set; } = string.Empty;
        
        // Navigation properties
        public ICollection<District> Districts { get; set; } = new List<District>();
    }
    
    // District (Second level: depends on Division)
    public class District
    {
        [Key]
        public int Id { get; set; }
        
        [Required]
        [StringLength(50)]
        public string Name { get; set; } = string.Empty;
        
        // Foreign key
        [Required]
        public int DivisionId { get; set; }
        
        // Navigation properties
        [ForeignKey("DivisionId")]
        public Division? Division { get; set; }
        
        public ICollection<Upazila> Upazilas { get; set; } = new List<Upazila>();
    }
    
    // Upazila (Third level: depends on District)
    public class Upazila
    {
        [Key]
        public int Id { get; set; }
        
        [Required]
        [StringLength(50)]
        public string Name { get; set; } = string.Empty;
        
        // Foreign key
        [Required]
        public int DistrictId { get; set; }
        
        // Navigation properties
        [ForeignKey("DistrictId")]
        public District? District { get; set; }
        
        public ICollection<Ward> Wards { get; set; } = new List<Ward>();
    }
    
    // Type of area
    public enum AreaType
    {
        CityCorporation,
        Union,
        Pourasava
    }
    
    // Ward (Fourth level: depends on Upazila and AreaType)
    public class Ward
    {
        [Key]
        public int Id { get; set; }
        
        [Required]
        [StringLength(50)]
        public string Name { get; set; } = string.Empty;
        
        // Foreign key
        [Required]
        public int UpazilaId { get; set; }
        
        [Required]
        public AreaType AreaType { get; set; }
        
        // Navigation properties
        [ForeignKey("UpazilaId")]
        public Upazila? Upazila { get; set; }
        
        public ICollection<Village> Villages { get; set; } = new List<Village>();
    }
    
    // Village/Area (Fifth level: depends on Ward)
    public class Village
    {
        [Key]
        public int Id { get; set; }
        
        [Required]
        [StringLength(100)]
        public string Name { get; set; } = string.Empty;
        
        // Foreign key
        [Required]
        public int WardId { get; set; }
        
        // Navigation properties
        [ForeignKey("WardId")]
        public Ward? Ward { get; set; }
    }
} 