using Bashinda.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace Bashinda.Data
{
    // This is a simplified DbContext for the MVC project
    // The real database operations happen in the WebAPI
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        // Local representation of database tables
        public DbSet<AdminPermission> AdminPermissions { get; set; }
        
        // Location tables
        public DbSet<LocationBase> Divisions { get; set; }
        public DbSet<LocationBase> Districts { get; set; }
        public DbSet<LocationBase> Upazilas { get; set; }
        public DbSet<LocationBase> Wards { get; set; }
        public DbSet<LocationBase> Villages { get; set; }
        
        // Added missing DbSet properties
        public DbSet<RenterProfile> RenterProfiles { get; set; }
        public DbSet<UserOTP> UserOTPs { get; set; }
    }

    // Simple location model for the MVC UI
    public class LocationBase
    {
        public string Id { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public string? ParentId { get; set; }
        
        // Navigation properties
        public string? DivisionId { get; set; }
        public string? DistrictId { get; set; }
        public string? UpazilaId { get; set; }
        public string? WardId { get; set; }
    }
} 