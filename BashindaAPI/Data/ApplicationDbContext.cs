using BashindaAPI.Models;
using Microsoft.EntityFrameworkCore;

namespace BashindaAPI.Data
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

        // Location tables
        public DbSet<Division> Divisions { get; set; }
        public DbSet<District> Districts { get; set; }
        public DbSet<Upazila> Upazilas { get; set; }
        public DbSet<Ward> Wards { get; set; }
        public DbSet<Village> Villages { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Configure UserOTP relationship
            modelBuilder.Entity<UserOTP>()
                .HasOne(u => u.User)
                .WithMany()
                .HasForeignKey(u => u.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            // Configure RenterProfile relationships
            modelBuilder.Entity<RenterProfile>(entity =>
            {
                entity.HasOne(r => r.User)
                    .WithMany()
                    .HasForeignKey(r => r.UserId)
                    .OnDelete(DeleteBehavior.Cascade);

                entity.HasOne(r => r.Division)
                    .WithMany()
                    .HasForeignKey(r => r.DivisionId)
                    .OnDelete(DeleteBehavior.NoAction);

                entity.HasOne(r => r.District)
                    .WithMany()
                    .HasForeignKey(r => r.DistrictId)
                    .OnDelete(DeleteBehavior.NoAction);

                entity.HasOne(r => r.Upazila)
                    .WithMany()
                    .HasForeignKey(r => r.UpazilaId)
                    .OnDelete(DeleteBehavior.NoAction);

                entity.HasOne(r => r.Ward)
                    .WithMany()
                    .HasForeignKey(r => r.WardId)
                    .OnDelete(DeleteBehavior.NoAction);

                entity.HasOne(r => r.Village)
                    .WithMany()
                    .HasForeignKey(r => r.VillageId)
                    .OnDelete(DeleteBehavior.NoAction);
            });

            // Configure ApartmentOwnerProfile relationships
            modelBuilder.Entity<ApartmentOwnerProfile>(entity =>
            {
                entity.HasOne(a => a.User)
                    .WithMany()
                    .HasForeignKey(a => a.UserId)
                    .OnDelete(DeleteBehavior.Cascade);

                entity.HasOne(a => a.Division)
                    .WithMany()
                    .HasForeignKey(a => a.DivisionId)
                    .OnDelete(DeleteBehavior.NoAction);

                entity.HasOne(a => a.District)
                    .WithMany()
                    .HasForeignKey(a => a.DistrictId)
                    .OnDelete(DeleteBehavior.NoAction);

                entity.HasOne(a => a.Upazila)
                    .WithMany()
                    .HasForeignKey(a => a.UpazilaId)
                    .OnDelete(DeleteBehavior.NoAction);

                entity.HasOne(a => a.Ward)
                    .WithMany()
                    .HasForeignKey(a => a.WardId)
                    .OnDelete(DeleteBehavior.NoAction);

                entity.HasOne(a => a.Village)
                    .WithMany()
                    .HasForeignKey(a => a.VillageId)
                    .OnDelete(DeleteBehavior.NoAction);
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