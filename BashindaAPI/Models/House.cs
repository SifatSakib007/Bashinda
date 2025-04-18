using System.ComponentModel.DataAnnotations;

namespace BashindaAPI.Models
{
    public class House
    {
        [Key]
        public int Id { get; set; }
        public required string Name { get; set; } 
        public string? Description { get; set; } = string.Empty;
        public string? ImagePath { get; set; } = string.Empty;
        public int? NumberOfFloors { get; set; }
        public int? ApartmentsPerFloor { get; set; }
        public DateTime? CreatedAt { get; set; } = DateTime.UtcNow;

        // Address relationships
        public int DivisionId { get; set; }
        public Division Division { get; set; } = null!;
        public int DistrictId { get; set; }
        public District District { get; set; } = null!;
        public int UpazilaId { get; set; }
        public Upazila Upazila { get; set; } = null!;
        public AreaType AreaType { get; set; }
        public int WardId { get; set; }
        public Ward Ward { get; set; } = null!;
        public int VillageId { get; set; }
        public Village Village { get; set; } = null!;

        // User relationship
        public int? OwnerId { get; set; }
        public User? Owner { get; set; } = null!;
    }
}
