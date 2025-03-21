using BashindaAPI.Models;

namespace BashindaAPI.DTOs
{
    // DTOs for simple list responses
    public class DivisionDto
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
    }
    
    public class DistrictDto
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public int DivisionId { get; set; }
    }
    
    public class UpazilaDto
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public int DistrictId { get; set; }
    }
    
    public class WardDto
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public int UpazilaId { get; set; }
        public AreaType AreaType { get; set; }
    }
    
    public class VillageDto
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public int WardId { get; set; }
    }
    
    // DTOs for creating new records
    public class CreateDivisionDto
    {
        public string Name { get; set; } = string.Empty;
    }
    
    public class CreateDistrictDto
    {
        public string Name { get; set; } = string.Empty;
        public int DivisionId { get; set; }
    }
    
    public class CreateUpazilaDto
    {
        public string Name { get; set; } = string.Empty;
        public int DistrictId { get; set; }
    }
    
    public class CreateWardDto
    {
        public string Name { get; set; } = string.Empty;
        public int UpazilaId { get; set; }
        public AreaType AreaType { get; set; }
    }
    
    public class CreateVillageDto
    {
        public string Name { get; set; } = string.Empty;
        public int WardId { get; set; }
    }
    
    // Location item DTO used for dropdown lists
    public class LocationItemDto
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
    }
} 