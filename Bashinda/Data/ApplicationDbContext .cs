using Bashinda.Models;
using Microsoft.EntityFrameworkCore;

namespace Bashinda.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {

        }
        public DbSet<User> Users { get; set; }
        public DbSet<UserOTP> UserOTPs { get; set; }
        public DbSet<RenterProfile> RenterProfiles { get; set; }
        public DbSet<ApartmentOwnerProfile> ApartmentOwnerProfiles { get; set; }
        public DbSet<Apartment> Apartments { get; set; }
    }
}
