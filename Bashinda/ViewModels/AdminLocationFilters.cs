namespace Bashinda.ViewModels
{
    public class AdminLocationFilters
    {
        public string? Division { get; set; }
        public string? District { get; set; }
        public string? Upazila { get; set; }
        public string? Ward { get; set; }
        public string? Village { get; set; }
    }

    //public class AdminDashboardViewModel
    //{
    //    public int PendingRenters { get; set; }
    //    public int PendingOwners { get; set; }
    //    public int TotalUsers { get; set; }
    //    public int ApprovedRenters { get; set; }
    //    public int ApprovedOwners { get; set; }
    //    public int TotalApartments { get; set; }
    //    public string LocationScope { get; set; }
    //}

    public class ProfileViewModel
    {
        public int Id { get; set; }
        public string FullName { get; set; } = string.Empty;
        public string MobileNo { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string? NationalId { get; set; }
        public string? BirthRegistrationNo { get; set; }
        public DateTime DateOfBirth { get; set; }
        public string? SelfImagePath { get; set; }
        public bool? IsApproved { get; set; }
    }
}