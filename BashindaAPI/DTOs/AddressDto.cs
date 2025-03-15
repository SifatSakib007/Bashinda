using BashindaAPI.Models;

namespace BashindaAPI.DTOs
{
    public class AddressDto
    {
        public int DivisionId { get; set; }
        public string DivisionName { get; set; } = string.Empty;
        
        public int DistrictId { get; set; }
        public string DistrictName { get; set; } = string.Empty;
        
        public int UpazilaId { get; set; }
        public string UpazilaName { get; set; } = string.Empty;
        
        public AreaType AreaType { get; set; }
        
        public int WardId { get; set; }
        public string WardName { get; set; } = string.Empty;
        
        public int VillageId { get; set; }
        public string VillageName { get; set; } = string.Empty;
        
        public string PostCode { get; set; } = string.Empty;
        public string HoldingNo { get; set; } = string.Empty;
    }
} 