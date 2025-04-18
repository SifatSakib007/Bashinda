using BashindaAPI.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace BashindaAPI.Data
{
    public class ApplicationDbContext : IdentityDbContext<User, IdentityRole<int>, int>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {
        }

        // 2. Remove the "new" keyword since we're properly overriding
        public override DbSet<User> Users { get; set; }
        public DbSet<UserOTP> UserOTPs { get; set; }
        public DbSet<RenterProfile> RenterProfiles { get; set; }
        public DbSet<ApartmentOwnerProfile> ApartmentOwnerProfiles { get; set; }
        public DbSet<Apartment> Apartments { get; set; }
        public DbSet<AdminPermission> AdminPermissions { get; set; }

        // Location tables
        public DbSet<Division> Divisions { get; set; }
        public DbSet<District> Districts { get; set; }
        public DbSet<Upazila> Upazilas { get; set; }
        public DbSet<Ward> Wards { get; set; }
        public DbSet<Village> Villages { get; set; }
        public DbSet<House> Houses { get; set; }
        

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Configure UserOTP relationship
            modelBuilder.Entity<UserOTP>()
                .HasOne(u => u.User)
                .WithMany()
                .HasForeignKey(u => u.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<ApartmentOwnerProfile>()
                .Property(p => p.Profession)
                .HasConversion<string>(); 
            modelBuilder.Entity<ApartmentOwnerProfile>()
                .Property(p => p.Nationality)
                .HasConversion<string>(); 
            modelBuilder.Entity<ApartmentOwnerProfile>()
                .Property(p => p.Gender)
                .HasConversion<string>(); 
            modelBuilder.Entity<ApartmentOwnerProfile>()
                .Property(p => p.BloodGroup)
                .HasConversion<string>(); 
            modelBuilder.Entity<ApartmentOwnerProfile>()
                .Property(p => p.AreaType)
                .HasConversion<string>(); 

            // 3.Configure AdminPermission relationship with correct types
            modelBuilder.Entity<AdminPermission>(entity =>
            {
                entity.HasOne(a => a.User)
                    .WithOne(u => u.AdminPermission)
                    .HasForeignKey<AdminPermission>(a => a.UserId)
                    .OnDelete(DeleteBehavior.Cascade);
            });
            // Configure RenterProfile relationships
            modelBuilder.Entity<RenterProfile>(entity =>
            {
                entity.HasOne(r => r.User)
                    .WithMany()
                    .HasForeignKey(r => r.UserId)
                    .OnDelete(DeleteBehavior.Cascade);

                // Remove location navigation properties since we're using string-based location fields now
            });

            // Configure ApartmentOwnerProfile relationships
            modelBuilder.Entity<ApartmentOwnerProfile>(entity =>
            {
                entity.HasOne(a => a.User)
                    .WithMany()
                    .HasForeignKey(a => a.UserId)
                    .OnDelete(DeleteBehavior.Cascade);

                // Remove location navigation properties since we're using string-based location fields now
            });

            // Configure Apartment relationships
            modelBuilder.Entity<Apartment>()
                .HasOne(a => a.Owner)
                .WithMany()
                .HasForeignKey(a => a.OwnerId)
                .OnDelete(DeleteBehavior.Cascade);

            // Add precision for MonthlyRent
            modelBuilder.Entity<Apartment>()
                .Property(a => a.MonthlyRent)
                .HasPrecision(18, 2);

            // Configure Location Hierarchy with NO CASCADE
            modelBuilder.Entity<District>()
                .HasOne(d => d.Division)
                .WithMany(d => d.Districts)
                .HasForeignKey(d => d.DivisionId)
                .OnDelete(DeleteBehavior.NoAction);

            modelBuilder.Entity<Upazila>()
                .HasOne(u => u.District)
                .WithMany(d => d.Upazilas)
                .HasForeignKey(u => u.DistrictId)
                .OnDelete(DeleteBehavior.NoAction);

            modelBuilder.Entity<Ward>()
                .HasOne(w => w.Upazila)
                .WithMany(u => u.Wards)
                .HasForeignKey(w => w.UpazilaId)
                .OnDelete(DeleteBehavior.NoAction);

            modelBuilder.Entity<Village>()
                .HasOne(v => v.Ward)
                .WithMany(w => w.Villages)
                .HasForeignKey(v => v.WardId)
                .OnDelete(DeleteBehavior.NoAction);
        }
    }
}