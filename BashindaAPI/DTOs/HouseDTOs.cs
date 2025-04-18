using BashindaAPI.Models;

namespace BashindaAPI.DTOs
{
    public class HouseDTO
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string? Description { get; set; } = string.Empty;
        public string? ImagePath { get; set; } = string.Empty;
        public int? NumberOfFloors { get; set; }
        public int? ApartmentsPerFloor { get; set; }
        public DateTime? CreatedAt { get; set; }

        // Address information
        public string Division { get; set; } = string.Empty;
        public string District { get; set; } = string.Empty;
        public string Upazila { get; set; } = string.Empty;
        public string Ward { get; set; } = string.Empty;
        public string Village { get; set; } = string.Empty;
        public AreaType AreaType { get; set; }

        // Creator information
        public string OwnerName { get; set; } = string.Empty;
    }
    public class CreateHouseDto
    {
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public int NumberOfFloors { get; set; }
        public int ApartmentsPerFloor { get; set; }
        public DateTime? CreatedAt { get; set; } = DateTime.UtcNow;
        public string? ImagePath { get; set; } = string.Empty;

        // Address IDs
        public int DivisionId { get; set; }
        public int DistrictId { get; set; }
        public int UpazilaId { get; set; }
        public AreaType AreaType { get; set; }
        public int WardId { get; set; }
        public int VillageId { get; set; }
    }
}
