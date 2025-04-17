using System.Security.Cryptography;
using System.Text;
using BashindaAPI.Data;
using BashindaAPI.DTOs;
using BashindaAPI.Models;
using Microsoft.EntityFrameworkCore;

namespace BashindaAPI.Services
{
    public class AdminService : IAdminService
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<AdminService> _logger;

        public AdminService(ApplicationDbContext context, ILogger<AdminService> logger)
        {
            _context = context;
            _logger = logger;
        }
        public async Task<(bool Success, List<RenterProfileDTO> Renters, string[] Errors)> GetAllRentersAsync()
        {
            try
            {
                var renters = await _context.RenterProfiles.ToListAsync();

                var renterDtos = renters.Select(u => new RenterProfileDTO
                {
                    Id = u.Id,
                    UserId = u.UserId,
                    IsAdult = u.IsAdult,
                    NationalId = u.NationalId,
                    BirthRegistrationNo = u.BirthRegistrationNo,
                    DateOfBirth = u.DateOfBirth,
                    FullName = u.FullName,
                    FatherName = u.FatherName,
                    MotherName = u.MotherName,
                    Nationality = u.Nationality.ToString(), // Convert Nationality enum to string
                    BloodGroup = u.BloodGroup,
                    Profession = u.Profession,
                    Gender = u.Gender,
                    MobileNo = u.MobileNo,
                    Email = u.Email,
                    SelfImagePath = u.SelfImagePath,
                    Division = u.Division,
                    District = u.District,
                    Upazila = u.Upazila,
                    AreaType = u.AreaType.ToString(), // Convert AreaType enum to string
                    Ward = u.Ward,
                    Village = u.Village,
                    PostCode = u.PostCode,
                    HoldingNo = u.HoldingNo,
                    IsApproved = u.IsApproved,
                    RejectionReason = u.RejectionReason
                }).ToList();

                return (true, renterDtos, Array.Empty<string>());
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting all renters: {Message}", ex.Message);
                return (false, new List<RenterProfileDTO>(), new[] { "An error occurred while retrieving renters" });
            }
        }
        public async Task<(bool Success, List<ApartmentOwnerProfileDto> Owners, string[] Errors)> GetAllOwnersAsync()
        {
            try
            {
                var owners = await _context.ApartmentOwnerProfiles.ToListAsync();

                var ownterDtos = owners.Select(u => new ApartmentOwnerProfileDto
                {
                    Id = u.Id,
                    UserId = u.UserId,
                    IsAdult = u.IsAdult,
                    NationalId = u.NationalId,
                    BirthRegistrationNo = u.BirthRegistrationNo,
                    DateOfBirth = u.DateOfBirth,
                    FullName = u.FullName,
                    FatherName = u.FatherName,
                    MotherName = u.MotherName,
                    Nationality = u.Nationality,
                    BloodGroup = u.BloodGroup,
                    Profession = u.Profession, 
                    Gender = u.Gender,
                    MobileNo = u.MobileNo ?? string.Empty,
                    Email = u.Email ?? string.Empty,
                    SelfImagePath = u.SelfImagePath,
                    Division = u.Division,
                    District = u.District,
                    Upazila = u.Upazila,
                    AreaType = u.AreaType, // Convert AreaType enum to string
                    Ward = u.Ward,
                    Village = u.Village,
                    PostCode = u.PostCode,
                    HoldingNo = u.HoldingNo,
                    IsApproved = u.IsApproved,
                    RejectionReason = u.RejectionReason
                }).ToList();

                return (true, ownterDtos, Array.Empty<string>());
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting all renters: {Message}", ex.Message);
                return (false, new List<ApartmentOwnerProfileDto>(), new[] { "An error occurred while retrieving renters" });
            }
        }
        public async Task<(bool Success, AdminDto? Admin, string[] Errors)> CreateAdminAsync(CreateAdminDto model)
        {
            try
            {
                // Check if email already exists
                if (await _context.Users.AnyAsync(u => u.Email == model.Email))
                {
                    return (false, null, new[] { "Email already exists" });
                }

                // Check if username already exists
                if (await _context.Users.AnyAsync(u => u.UserName == model.UserName))
                {
                    return (false, null, new[] { "Username already exists" });
                }

                // Create new admin user
                var passwordHash = HashPassword(model.Password);
                var user = new User
                {
                    UserName = model.UserName,
                    Email = model.Email,
                    PhoneNumber = model.PhoneNumber,
                    PasswordHash = passwordHash,
                    Role = UserRole.Admin,
                    IsVerified = true // Admin users are verified by default
                };

                _context.Users.Add(user);
                await _context.SaveChangesAsync();

                // Create admin permissions
                var permissions = new AdminPermission
                {
                    UserId = user.Id,
                    Division = model.Permissions.Division,
                    District = model.Permissions.District,
                    Upazila = model.Permissions.Upazila,
                    Ward = model.Permissions.Ward,
                    Village = model.Permissions.Village,
                    CanViewUserName = model.Permissions.CanViewUserName,
                    CanViewEmail = model.Permissions.CanViewEmail,
                    CanViewPhone = model.Permissions.CanViewPhone,
                    CanViewAddress = model.Permissions.CanViewAddress,
                    CanViewProfileImage = model.Permissions.CanViewProfileImage,
                    CanViewNationalId = model.Permissions.CanViewNationalId,
                    CanViewBirthRegistration = model.Permissions.CanViewBirthRegistration,
                    CanViewDateOfBirth = model.Permissions.CanViewDateOfBirth,
                    CanViewFamilyInfo = model.Permissions.CanViewFamilyInfo,
                    CanViewProfession = model.Permissions.CanViewProfession,
                    CanApproveRenters = model.Permissions.CanApproveRenters,
                    CanApproveOwners = model.Permissions.CanApproveOwners,
                    CanManageApartments = model.Permissions.CanManageApartments
                };
                // Add this validation before saving permissions
                if (!ValidatePermissionHierarchy(model.Permissions))
                {
                    return (false, null, new[] { "Invalid location hierarchy - higher levels must be specified" });
                }
                _context.AdminPermissions.Add(permissions);
                await _context.SaveChangesAsync();

                // Return admin DTO
                var adminDto = new AdminDto
                {
                    Id = user.Id,
                    UserName = user.UserName,
                    Email = user.Email,
                    PhoneNumber = user.PhoneNumber,
                    Permissions = new AdminPermissionDto
                    {
                        Division = permissions.Division,
                        District = permissions.District,
                        Upazila = permissions.Upazila,
                        Ward = permissions.Ward,
                        Village = permissions.Village,
                        CanViewUserName = permissions.CanViewUserName,
                        CanViewEmail = permissions.CanViewEmail,
                        CanViewPhone = permissions.CanViewPhone,
                        CanViewAddress = permissions.CanViewAddress,
                        CanViewProfileImage = permissions.CanViewProfileImage,
                        CanViewNationalId = permissions.CanViewNationalId,
                        CanViewBirthRegistration = permissions.CanViewBirthRegistration,
                        CanViewDateOfBirth = permissions.CanViewDateOfBirth,
                        CanViewFamilyInfo = permissions.CanViewFamilyInfo,
                        CanViewProfession = permissions.CanViewProfession,
                        CanApproveRenters = permissions.CanApproveRenters,
                        CanApproveOwners = permissions.CanApproveOwners,
                        CanManageApartments = permissions.CanManageApartments
                    }
                };

                return (true, adminDto, Array.Empty<string>());
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating admin: {Message}", ex.Message);
                return (false, null, new[] { "An error occurred while creating the admin" });
            }
        }

        public async Task<(bool Success, AdminDto? Admin, string[] Errors)> GetAdminByIdAsync(int id)
        {
            try
            {
                var user = await _context.Users
                    .Include(u => u.AdminPermission)
                    .FirstOrDefaultAsync(u => u.Id == id && u.Role == UserRole.Admin);

                if (user == null || user.AdminPermission == null)
                {
                    return (false, null, new[] { "Admin not found" });
                }

                var adminDto = new AdminDto
                {
                    Id = user.Id,
                    UserName = user.UserName,
                    Email = user.Email,
                    PhoneNumber = user.PhoneNumber,
                    Permissions = new AdminPermissionDto
                    {
                        Division = user.AdminPermission.Division,
                        District = user.AdminPermission.District,
                        Upazila = user.AdminPermission.Upazila,
                        Ward = user.AdminPermission.Ward,
                        Village = user.AdminPermission.Village,
                        CanViewUserName = user.AdminPermission.CanViewUserName,
                        CanViewEmail = user.AdminPermission.CanViewEmail,
                        CanViewPhone = user.AdminPermission.CanViewPhone,
                        CanViewAddress = user.AdminPermission.CanViewAddress,
                        CanViewProfileImage = user.AdminPermission.CanViewProfileImage,
                        CanViewNationalId = user.AdminPermission.CanViewNationalId,
                        CanViewBirthRegistration = user.AdminPermission.CanViewBirthRegistration,
                        CanViewDateOfBirth = user.AdminPermission.CanViewDateOfBirth,
                        CanViewFamilyInfo = user.AdminPermission.CanViewFamilyInfo,
                        CanViewProfession = user.AdminPermission.CanViewProfession,
                        CanApproveRenters = user.AdminPermission.CanApproveRenters,
                        CanApproveOwners = user.AdminPermission.CanApproveOwners,
                        CanManageApartments = user.AdminPermission.CanManageApartments
                    }
                };

                return (true, adminDto, Array.Empty<string>());
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting admin: {Message}", ex.Message);
                return (false, null, new[] { "An error occurred while retrieving the admin" });
            }
        }

        public async Task<(bool Success, List<AdminListDto> Admins, string[] Errors)> GetAllAdminsAsync()
        {
            try
            {
                var admins = await _context.Users
                    .Include(u => u.AdminPermission)
                    .Where(u => u.Role == UserRole.Admin)
                    .ToListAsync();

                var adminDtos = admins.Select(u => new AdminListDto
                {
                    Id = u.Id,
                    UserName = u.UserName,
                    Email = u.Email,
                    PhoneNumber = u.PhoneNumber,
                    LocationAccess = FormatLocationAccess(u.AdminPermission),
                    HasApprovalPermissions = u.AdminPermission != null && 
                                            (u.AdminPermission.CanApproveRenters || 
                                             u.AdminPermission.CanApproveOwners || 
                                             u.AdminPermission.CanManageApartments)
                }).ToList();

                return (true, adminDtos, Array.Empty<string>());
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting all admins: {Message}", ex.Message);
                return (false, new List<AdminListDto>(), new[] { "An error occurred while retrieving admins" });
            }
        }

        //public async Task<(bool Success, string[] Errors)> UpdateAdminPermissionsAsync(UpdateAdminPermissionsDto model)
        //{
        //    try
        //    {
        //        var admin = await _context.Users
        //            .Include(u => u.AdminPermission)
        //            .FirstOrDefaultAsync(u => u.Id == model.AdminId && u.Role == UserRole.Admin);

        //        if (admin == null || admin.AdminPermission == null)
        //        {
        //            return (false, new[] { "Admin not found" });
        //        }

        //        // Update permissions
        //admin.AdminPermission.Division = model.Permissions.Division;
        //        admin.AdminPermission.District = model.Permissions.District;
        //        admin.AdminPermission.Upazila = model.Permissions.Upazila;
        //        admin.AdminPermission.Ward = model.Permissions.Ward;
        //        admin.AdminPermission.Village = model.Permissions.Village;
        //        admin.AdminPermission.CanViewUserName = model.Permissions.CanViewUserName;
        //        admin.AdminPermission.CanViewEmail = model.Permissions.CanViewEmail;
        //        admin.AdminPermission.CanViewPhone = model.Permissions.CanViewPhone;
        //        admin.AdminPermission.CanViewAddress = model.Permissions.CanViewAddress;
        //        admin.AdminPermission.CanViewProfileImage = model.Permissions.CanViewProfileImage;
        //        admin.AdminPermission.CanViewNationalId = model.Permissions.CanViewNationalId;
        //        admin.AdminPermission.CanViewBirthRegistration = model.Permissions.CanViewBirthRegistration;
        //        admin.AdminPermission.CanViewDateOfBirth = model.Permissions.CanViewDateOfBirth;
        //        admin.AdminPermission.CanViewFamilyInfo = model.Permissions.CanViewFamilyInfo;
        //        admin.AdminPermission.CanViewProfession = model.Permissions.CanViewProfession;
        //        admin.AdminPermission.CanApproveRenters = model.Permissions.CanApproveRenters;
        //        admin.AdminPermission.CanApproveOwners = model.Permissions.CanApproveOwners;
        //        admin.AdminPermission.CanManageApartments = model.Permissions.CanManageApartments;

        //        await _context.SaveChangesAsync();
        //        return (true, Array.Empty<string>());
        //    }
        //    catch (Exception ex)
        //    {
        //        _logger.LogError(ex, "Error updating admin permissions: {Message}", ex.Message);
        //        return (false, new[] { "An error occurred while updating admin permissions" });
        //    }
        //}
        public async Task<(bool Success, string[] Errors)> UpdateAdminPermissionsAsync(UpdateAdminPermissionsDto model)
        {
            try
            {
                var admin = await _context.Users
                    .Include(u => u.AdminPermission)
                    .FirstOrDefaultAsync(u => u.Id == model.AdminId && u.Role == UserRole.Admin);

                if (admin == null || admin.AdminPermission == null)
                {
                    return (false, new[] { "Admin not found" });
                }

                // SuperAdmin protection
                if (admin.Role == UserRole.SuperAdmin)
                {
                    return (false, new[] { "Cannot modify SuperAdmin permissions" });
                }

                // Hierarchy validation
                if (!ValidatePermissionHierarchy(model.Permissions))
                {
                    return (false, new[] { "Invalid location hierarchy: Higher levels must be specified" });
                }

                // Update permissions
                admin.AdminPermission.Division = model.Permissions.Division;
                admin.AdminPermission.District = model.Permissions.District;
                admin.AdminPermission.Upazila = model.Permissions.Upazila;
                admin.AdminPermission.Ward = model.Permissions.Ward;
                admin.AdminPermission.Village = model.Permissions.Village;
                admin.AdminPermission.CanViewUserName = model.Permissions.CanViewUserName;
                admin.AdminPermission.CanViewEmail = model.Permissions.CanViewEmail;
                admin.AdminPermission.CanViewPhone = model.Permissions.CanViewPhone;
                admin.AdminPermission.CanViewAddress = model.Permissions.CanViewAddress;
                admin.AdminPermission.CanViewProfileImage = model.Permissions.CanViewProfileImage;
                admin.AdminPermission.CanViewNationalId = model.Permissions.CanViewNationalId;
                admin.AdminPermission.CanViewBirthRegistration = model.Permissions.CanViewBirthRegistration;
                admin.AdminPermission.CanViewDateOfBirth = model.Permissions.CanViewDateOfBirth;
                admin.AdminPermission.CanViewFamilyInfo = model.Permissions.CanViewFamilyInfo;
                admin.AdminPermission.CanViewProfession = model.Permissions.CanViewProfession;
                admin.AdminPermission.CanApproveRenters = model.Permissions.CanApproveRenters;
                admin.AdminPermission.CanApproveOwners = model.Permissions.CanApproveOwners;
                admin.AdminPermission.CanManageApartments = model.Permissions.CanManageApartments;
                

                await _context.SaveChangesAsync();
                return (true, Array.Empty<string>());
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating admin permissions");
                return (false, new[] { ex.Message });
            }
        }

        public async Task<(bool Success, string[] Errors)> DeleteAdminAsync(int id)
        {
            try
            {
                var admin = await _context.Users
                    .FirstOrDefaultAsync(u => u.Id == id && u.Role == UserRole.Admin);

                if (admin == null)
                {
                    return (false, new[] { "Admin not found" });
                }

                _context.Users.Remove(admin);
                await _context.SaveChangesAsync();
                return (true, Array.Empty<string>());
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting admin: {Message}", ex.Message);
                return (false, new[] { "An error occurred while deleting the admin" });
            }
        }



        //public async Task<bool> CanAdminAccessUserAsync(int adminId, int userId)
        //{
        //    try
        //    {
        //        // Get admin permissions
        //        var admin = await _context.Users
        //            .Include(u => u.AdminPermission)
        //            .FirstOrDefaultAsync(u => u.Id == adminId && u.Role == UserRole.Admin);

        //        if (admin == null || admin.AdminPermission == null)
        //        {
        //            return false;
        //        }

        //        // SuperAdmin can access all users
        //        if (admin.Role == UserRole.SuperAdmin)
        //        {
        //            return true;
        //        }

        //        // Find the user to check
        //        var userToCheck = await FindUserWithProfileAsync(userId);
        //        if (userToCheck == null)
        //        {
        //            return false;
        //        }

        //        // Get location information from either RenterProfile or ApartmentOwnerProfile
        //        string? division = null, district = null, upazila = null, ward = null, village = null;

        //        if (userToCheck.Role == UserRole.ApartmentRenter && userToCheck.RenterProfile != null)
        //        {
        //            division = userToCheck.RenterProfile.Division;
        //            district = userToCheck.RenterProfile.District;
        //            upazila = userToCheck.RenterProfile.Upazila;
        //            ward = userToCheck.RenterProfile.Ward;
        //            village = userToCheck.RenterProfile.Village;
        //        }
        //        else if (userToCheck.Role == UserRole.ApartmentOwner && userToCheck.OwnerProfile != null)
        //        {
        //            division = userToCheck.OwnerProfile.Division;
        //            district = userToCheck.OwnerProfile.District;
        //            upazila = userToCheck.OwnerProfile.Upazila;
        //            ward = userToCheck.OwnerProfile.Ward;
        //            village = userToCheck.OwnerProfile.Village;
        //        }


        //        // If user has no location, admin cannot access it based on location
        //        if (string.IsNullOrEmpty(division))
        //        {
        //            return false;
        //        }

        //        // Check location-based access
        //        return CanAccessLocation(admin.AdminPermission, division, district, upazila, ward, village);
        //    }
        //    catch (Exception ex)
        //    {
        //        _logger.LogError(ex, "Error checking admin access to user: {Message}", ex.Message);
        //        return false;
        //    }
        //}

        public async Task<bool> CanAdminAccessUserAsync(int adminId, int userId)
        {
            try
            {
                // Get admin with permissions
                var admin = await _context.Users
                    .Include(u => u.AdminPermission)
                    .FirstOrDefaultAsync(u => u.Id == adminId && u.Role == UserRole.Admin);

                if (admin?.AdminPermission == null) return false;

                // SuperAdmin bypass all checks
                if (admin.Role == UserRole.SuperAdmin) return true;

                // Get user with profile
                var userToCheck = await FindUserWithProfileAsync(userId);
                if (userToCheck == null) return false;

                // Validate permission type first
                if (!HasRelevantPermission(admin.AdminPermission, userToCheck.Role))
                {
                    _logger.LogWarning("Admin {AdminId} lacks permissions for {UserRole} access", adminId, userToCheck.Role);
                    return false;
                }

                // Get user's location hierarchy
                var userLocation = GetUserLocation(userToCheck);
                if (userLocation == null)
                {
                    _logger.LogWarning("User {UserId} has no location data", userId);
                    return false;
                }

                // Check location hierarchy access
                return IsWithinAdminScope(admin.AdminPermission, userLocation);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error checking admin access for Admin:{AdminId} to User:{UserId}", adminId, userId);
                return false;
            }
        }

        private bool HasRelevantPermission(AdminPermission permission, UserRole userRole)
        {
            return userRole switch
            {
                UserRole.ApartmentRenter => permission.CanApproveRenters,
                UserRole.ApartmentOwner => permission.CanApproveOwners,
                _ => false
            };
        }

        private UserLocation? GetUserLocation(UserWithProfile user)
        {
            switch (user.Role)
            {
                case UserRole.ApartmentRenter when user.RenterProfile != null:
                    return new UserLocation(
                        user.RenterProfile.Division,
                        user.RenterProfile.District,
                        user.RenterProfile.Upazila,
                        user.RenterProfile.Ward,
                        user.RenterProfile.Village
                    );

                case UserRole.ApartmentOwner when user.OwnerProfile != null:
                    return new UserLocation(
                        user.OwnerProfile.Division,
                        user.OwnerProfile.District,
                        user.OwnerProfile.Upazila,
                        user.OwnerProfile.Ward,
                        user.OwnerProfile.Village
                    );

                default:
                    return null;
            }
        }

        private bool IsWithinAdminScope(AdminPermission admin, UserLocation userLocation)
        {
            // Admin with no location restrictions can access all
            if (string.IsNullOrEmpty(admin.Division)) return true;

            // Check hierarchy from top to bottom
            var scopeMatch = admin.Division == userLocation.Division
                && (string.IsNullOrEmpty(admin.District) || admin.District == userLocation.District)
                && (string.IsNullOrEmpty(admin.Upazila) || admin.Upazila == userLocation.Upazila)
                && (string.IsNullOrEmpty(admin.Ward) || admin.Ward == userLocation.Ward)
                && (string.IsNullOrEmpty(admin.Village) || admin.Village == userLocation.Village);

            _logger.LogDebug("Location scope match: {Match} for Admin:{Division}/{District} and User:{Division}/{District}",
                scopeMatch, admin.Division, admin.District, userLocation.Division, userLocation.District);

            return scopeMatch;
        }

        // Helper record for location data
        private record UserLocation(
            string Division,
            string District,
            string Upazila,
            string Ward,
            string Village
        );

        public async Task<bool> CanAdminAccessLocationAsync(int adminId, string division, string district, string upazila, string? ward = null, string? village = null)
        {
            try
            {
                // Get admin permissions
                var admin = await _context.Users
                    .Include(u => u.AdminPermission)
                    .FirstOrDefaultAsync(u => u.Id == adminId && u.Role == UserRole.Admin);

                if (admin == null || admin.AdminPermission == null)
                {
                    return false;
                }

                // SuperAdmin can access all locations
                if (admin.Role == UserRole.SuperAdmin)
                {
                    return true;
                }

                return CanAccessLocation(admin.AdminPermission, division, district, upazila, ward, village);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error checking admin access to location: {Message}", ex.Message);
                return false;
            }
        }

        // Helper methods
        private string HashPassword(string password)
        {
            using var hmac = new HMACSHA512();
            var salt = hmac.Key; // 64-byte salt
            var hash = hmac.ComputeHash(Encoding.UTF8.GetBytes(password));

            // Store salt and hash separately
            return Convert.ToBase64String(salt) + "|" + Convert.ToBase64String(hash);
        }

        private string? FormatLocationAccess(AdminPermission? permission)
        {
            if (permission == null) return "No location restrictions";

            var parts = new List<string>();
            if (!string.IsNullOrEmpty(permission.Division))
            {
                parts.Add(permission.Division);
            }
            if (!string.IsNullOrEmpty(permission.District))
            {
                parts.Add(permission.District);
            }
            if (!string.IsNullOrEmpty(permission.Upazila))
            {
                parts.Add(permission.Upazila);
            }
            if (!string.IsNullOrEmpty(permission.Ward))
            {
                parts.Add(permission.Ward);
            }
            if (!string.IsNullOrEmpty(permission.Village))
            {
                parts.Add(permission.Village);
            }

            return parts.Count > 0 ? string.Join(", ", parts) : "All Locations";
        }

        private bool CanAccessLocation(AdminPermission permission, string division, string district, string upazila, string? ward, string? village)
        {
            // SuperAdmin bypass
            if (permission.User != null && permission.User.Role == UserRole.SuperAdmin) return true;
            // If admin has no location restriction, they can access all locations
            if (string.IsNullOrEmpty(permission.Division))
            {
                return true;
            }

            // Check if admin's division matches user's division
            if (permission.Division != division)
            {
                return false;
            }

            // If admin has district restriction, check if it matches
            if (!string.IsNullOrEmpty(permission.District) && permission.District != district)
            {
                return false;
            }

            // If admin has upazila restriction, check if it matches
            if (!string.IsNullOrEmpty(permission.Upazila) && permission.Upazila != upazila)
            {
                return false;
            }

            // If admin has ward restriction, check if it matches
            if (!string.IsNullOrEmpty(permission.Ward) && permission.Ward != ward)
            {
                return false;
            }

            // If admin has village restriction, check if it matches
            if (!string.IsNullOrEmpty(permission.Village) && permission.Village != village)
            {
                return false;
            }

            // All checks passed
            return true;
        }
        
        private class UserWithProfile
        {
            public User User { get; set; }
            public UserRole Role { get; set; }
            public RenterProfile? RenterProfile { get; set; }
            public ApartmentOwnerProfile? OwnerProfile { get; set; }
        }
        
        private async Task<UserWithProfile?> FindUserWithProfileAsync(int userId)
        {
            var user = await _context.Users.FindAsync(userId);
            if (user == null) return null;
            
            if (user.Role == UserRole.ApartmentRenter)
            {
                var renterProfile = await _context.RenterProfiles
                    .FirstOrDefaultAsync(r => r.UserId == userId);
                    
                return new UserWithProfile 
                { 
                    User = user, 
                    Role = user.Role, 
                    RenterProfile = renterProfile 
                };
            }
            else if (user.Role == UserRole.ApartmentOwner)
            {
                var ownerProfile = await _context.ApartmentOwnerProfiles
                    .FirstOrDefaultAsync(o => o.UserId == userId);
                    
                return new UserWithProfile 
                { 
                    User = user, 
                    Role = user.Role, 
                    OwnerProfile = ownerProfile 
                };
            }
            
            return new UserWithProfile { User = user, Role = user.Role };
        }
        private bool ValidatePermissionHierarchy(AdminPermissionDto permissions)
        {
            if (!string.IsNullOrEmpty(permissions.Village) && string.IsNullOrEmpty(permissions.Ward)) return false;
            if (!string.IsNullOrEmpty(permissions.Ward) && string.IsNullOrEmpty(permissions.Upazila)) return false;
            if (!string.IsNullOrEmpty(permissions.Upazila) && string.IsNullOrEmpty(permissions.District)) return false;
            if (!string.IsNullOrEmpty(permissions.District) && string.IsNullOrEmpty(permissions.Division)) return false;
            return true;
        }

        public async Task<(bool Success, ApartmentOwnerProfileDto? Profile, string[] Errors)> GetOwnerByUserIdAsync(string userId)
        {
            try
            {
                // Convert userId to int since ApartmentOwnerProfile.Id is of type int
                if (!int.TryParse(userId, out var userIdInt))
                {
                    return (false, null, new[] { "Invalid user ID format." });
                }

                var profile = await _context.ApartmentOwnerProfiles
                    .FirstOrDefaultAsync(r => r.Id == userIdInt);

                if (profile == null)
                {
                    return (false, null, new[] { "Owner profile not found." });
                }

                var dto = new ApartmentOwnerProfileDto
                {
                    Id = profile.Id,
                    UserId = profile.UserId,
                    // Map other properties as needed
                    IsAdult = profile.IsAdult,
                    NationalId = profile.NationalId,
                    BirthRegistrationNo = profile.BirthRegistrationNo,
                    DateOfBirth = profile.DateOfBirth,
                    FullName = profile.FullName,
                    FatherName = profile.FatherName,
                    MotherName = profile.MotherName,
                    MobileNo = profile.MobileNo,
                    BirthRegistrationImagePath = profile.BirthRegistrationImagePath
                    
                };

                return (true, dto, Array.Empty<string>());
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving owner profile for user ID: {UserId}", userId);
                return (false, null, new[] { "An error occurred while retrieving the owner profile." });
            }
        }

    }
} 