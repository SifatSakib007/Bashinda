namespace BashindaAPI.DTOs
{
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
} 