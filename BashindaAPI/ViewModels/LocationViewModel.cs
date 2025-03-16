using BashindaAPI.Models;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace BashindaAPI.ViewModels
{
    public class LocationViewModel
    {
        // Dropdown lists
        public List<SelectListItem> Divisions { get; set; } = new List<SelectListItem>();
        public List<SelectListItem> Districts { get; set; } = new List<SelectListItem>();
        public List<SelectListItem> Upazilas { get; set; } = new List<SelectListItem>();
        public List<SelectListItem> Wards { get; set; } = new List<SelectListItem>();
        public List<SelectListItem> Villages { get; set; } = new List<SelectListItem>();
        
        // Selected values
        public int DivisionId { get; set; }
        public int DistrictId { get; set; }
        public int UpazilaId { get; set; }
        public AreaType AreaType { get; set; }
        public int WardId { get; set; }
        public int VillageId { get; set; }
        public string PostCode { get; set; } = string.Empty;
        public string HoldingNo { get; set; } = string.Empty;
    }
} 