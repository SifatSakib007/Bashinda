namespace Bashinda.ViewModels
{
    public class AdminDashboardViewModel
    {
        public int PendingRenters { get; set; }
        public int PendingOwners { get; set; }
        public int TotalUsers { get; set; }
        public int ApprovedRenters { get; set; }
        public int TotalRenters { get; set; }
        public int ApprovedOwners { get; set; }
        public int TotalOwners { get; set; }
        public int TotalApartments { get; set; }
        public decimal RenterApprovalPercentage { get; set; }
        public decimal OwnerApprovalPercentage { get; set; }
    }
} 