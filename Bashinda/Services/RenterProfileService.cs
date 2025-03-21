using Bashinda.Data;
using Bashinda.Models;
using Bashinda.ViewModels;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System.Data.SqlClient;

namespace Bashinda.Services
{
    public class RenterProfileService : IRenterProfileService
    {
        private readonly ApplicationDbContext _context;
        private readonly IWebHostEnvironment _env;
        private readonly ILogger<RenterProfileService> _logger;

        public RenterProfileService(ApplicationDbContext context, IWebHostEnvironment env, ILogger<RenterProfileService> logger)
        {
            _context = context;
            _env = env;
            _logger = logger;
        }

        public async Task<ServiceResponse<RenterProfileViewModel>> SaveRenterProfileAsync(int userId, RenterProfileViewModel model)
        {
            try
            {
                // Log the user ID we're working with
                _logger.LogInformation("Attempting to save profile for user ID: {UserId}", userId);

                // Verify that the user exists and is a renter
                //var user = await _context.Users.FirstOrDefaultAsync(u => u.Id.ToString() == userId.ToString());
                //if (user == null)
                //{
                //    _logger.LogWarning("User not found: {UserId}", userId);
                //    return new ServiceResponse<RenterProfileViewModel> { Success = false, Errors = new[] { "User not found." } };
                //}

                //_logger.LogInformation("User found: {UserId}, Role: {Role}", userId, user.Role);

                //// Compare using string representation or enum parsing
                //if (user.Role.ToString() != UserRole.ApartmentRenter.ToString())
                //{
                //    _logger.LogWarning("User is not a renter: {UserId}, Role: {Role}", userId, user.Role);
                //    return new ServiceResponse<RenterProfileViewModel> { Success = false, Errors = new[] { "User is not a renter." } };
                //}

                // Check if the user already has a profile - but don't return an error, just update if it exists
                var existingProfile = await _context.RenterProfiles.FirstOrDefaultAsync(p => p.UserId.ToString() == userId.ToString());
                if (existingProfile != null)
                {
                    _logger.LogInformation("Found existing profile for user {UserId}, updating instead of creating new", userId);
                    var result = await UpdateRenterProfileAsync(existingProfile, model);
                    bool success = result.Success;
                    string[] errors = result.Errors;
                    return new ServiceResponse<RenterProfileViewModel>
                    {
                        Success = success,
                        Errors = errors,
                        Data = success ? model : null
                    };
                }

                // Handle file uploads
                string? nationalIdImagePath = null;
                string? birthRegImagePath = null;
                string? selfImagePath = null;

                try
                {
                    if (model.IsAdult)
                    {
                        if (model.NationalIdImage != null)
                        {
                            nationalIdImagePath = await SaveFileAsync(model.NationalIdImage);
                            _logger.LogInformation("Saved National ID image: {Path}", nationalIdImagePath);
                        }
                    }
                    else
                    {
                        if (model.BirthRegistrationImage != null)
                        {
                            birthRegImagePath = await SaveFileAsync(model.BirthRegistrationImage);
                            _logger.LogInformation("Saved Birth Registration image: {Path}", birthRegImagePath);
                        }
                    }

                    if (model.SelfImage != null)
                    {
                        selfImagePath = await SaveFileAsync(model.SelfImage);
                        _logger.LogInformation("Saved Self image: {Path}", selfImagePath);
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error saving files: {ErrorMessage}", ex.Message);
                    return new ServiceResponse<RenterProfileViewModel> { Success = false, Errors = new[] { $"Error saving uploaded files: {ex.Message}" } };
                }

                // Create new profile object
                var profile = new RenterProfile
                {
                    UserId = userId,
                    IsAdult = model.IsAdult,
                    NationalId = model.IsAdult ? model.NationalId : null,
                    NationalIdImagePath = nationalIdImagePath,
                    BirthRegistrationNo = !model.IsAdult ? model.BirthRegistrationNo : null,
                    BirthRegistrationImagePath = birthRegImagePath,
                    DateOfBirth = model.DateOfBirth,
                    FullName = model.FullName ?? string.Empty,
                    FatherName = model.FatherName ?? string.Empty,
                    MotherName = model.MotherName ?? string.Empty,
                    Nationality = model.Nationality,
                    BloodGroup = model.BloodGroup,
                    Profession = model.Profession,
                    Gender = model.Gender,
                    MobileNo = model.MobileNo ?? string.Empty,
                    Email = model.Email ?? string.Empty,
                    SelfImagePath = selfImagePath,
                    Division = model.Division ?? string.Empty,
                    District = model.District ?? string.Empty,
                    Upazila = model.Upazila ?? string.Empty,
                    IsApproved = false
                };

                try
                {
                    // Log the profile data being saved
                    _logger.LogInformation("Adding new profile for user {UserId}", userId);
                    
                    // Add the profile to the context
                    _context.RenterProfiles.Add(profile);
                    
                    // Try to save changes
                    await _context.SaveChangesAsync();
                    
                    _logger.LogInformation("Successfully saved profile for user {UserId}", userId);
                    return new ServiceResponse<RenterProfileViewModel>
                    {
                        Success = true,
                        Data = model
                    };
                }
                catch (DbUpdateException dbEx)
                {
                    _logger.LogError(dbEx, "Database error saving profile for user {UserId}: {Message}", userId, dbEx.Message);
                    
                    // Get the inner exception details
                    string errorDetails = dbEx.InnerException?.Message ?? dbEx.Message;
                    
                    return new ServiceResponse<RenterProfileViewModel>
                    {
                        Success = false,
                        Errors = new[] { $"Database error: {errorDetails}" }
                    };
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in SaveRenterProfileAsync for user {UserId}: {ErrorMessage}", userId, ex.Message);
                return new ServiceResponse<RenterProfileViewModel>
                {
                    Success = false,
                    Errors = new[] { $"An error occurred: {ex.Message}" }
                };
            }
        }

        // Method to update an existing profile
        private async Task<(bool Success, string[] Errors)> UpdateRenterProfileAsync(RenterProfile existingProfile, RenterProfileViewModel model)
        {
            try
            {
                // Handle file uploads
                string? nationalIdImagePath = existingProfile.NationalIdImagePath;
                string? birthRegImagePath = existingProfile.BirthRegistrationImagePath;
                string? selfImagePath = existingProfile.SelfImagePath;

                if (model.IsAdult)
                {
                    if (model.NationalIdImage != null)
                    {
                        nationalIdImagePath = await SaveFileAsync(model.NationalIdImage);
                    }
                }
                else
                {
                    if (model.BirthRegistrationImage != null)
                    {
                        birthRegImagePath = await SaveFileAsync(model.BirthRegistrationImage);
                    }
                }

                if (model.SelfImage != null)
                {
                    selfImagePath = await SaveFileAsync(model.SelfImage);
                }

                // Update the existing profile
                existingProfile.IsAdult = model.IsAdult;
                existingProfile.NationalId = model.IsAdult ? model.NationalId : null;
                existingProfile.NationalIdImagePath = nationalIdImagePath;
                existingProfile.BirthRegistrationNo = !model.IsAdult ? model.BirthRegistrationNo : null;
                existingProfile.BirthRegistrationImagePath = birthRegImagePath;
                existingProfile.DateOfBirth = model.DateOfBirth;
                existingProfile.FullName = model.FullName ?? string.Empty;
                existingProfile.FatherName = model.FatherName ?? string.Empty;
                existingProfile.MotherName = model.MotherName ?? string.Empty;
                existingProfile.Nationality = model.Nationality;
                existingProfile.BloodGroup = model.BloodGroup;
                existingProfile.Profession = model.Profession;
                existingProfile.Gender = model.Gender;
                existingProfile.MobileNo = model.MobileNo ?? string.Empty;
                existingProfile.Email = model.Email ?? string.Empty;
                existingProfile.SelfImagePath = selfImagePath;
                existingProfile.Division = model.Division ?? string.Empty;
                existingProfile.District = model.District ?? string.Empty;
                existingProfile.Upazila = model.Upazila ?? string.Empty;

                try
                {
                    _context.RenterProfiles.Update(existingProfile);
                    await _context.SaveChangesAsync();
                    return (true, Array.Empty<string>());
                }
                catch (DbUpdateException dbEx)
                {
                    _logger.LogError(dbEx, "Database error updating profile: {Message}", dbEx.Message);
                    return (false, new[] { $"Database error: {dbEx.InnerException?.Message ?? dbEx.Message}" });
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating profile: {ErrorMessage}", ex.Message);
                return (false, new[] { $"An error occurred: {ex.Message}" });
            }
        }

        private async Task<string?> SaveFileAsync(IFormFile file)
        {
            if (file == null || file.Length == 0)
            {
                return null;
            }

            // Ensure uploads directory exists
            var uploadsFolder = Path.Combine(_env.WebRootPath, "uploads");
            if (!Directory.Exists(uploadsFolder))
            {
                try
                {
                    Directory.CreateDirectory(uploadsFolder);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error creating uploads directory: {ErrorMessage}", ex.Message);
                    throw new DirectoryNotFoundException($"Could not create uploads directory: {ex.Message}");
                }
            }

            try
            {
                // Generate a safe filename
                var uniqueFileName = Guid.NewGuid().ToString() + "_" + Path.GetFileName(file.FileName).Replace(" ", "_");
                var filePath = Path.Combine(uploadsFolder, uniqueFileName);
                
                // Save the file
                using (var fileStream = new FileStream(filePath, FileMode.Create))
                {
                    await file.CopyToAsync(fileStream);
                }
                
                // Return the relative path for database storage
                return Path.Combine("uploads", uniqueFileName).Replace("\\", "/");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error saving file {FileName}: {ErrorMessage}", file.FileName, ex.Message);
                throw;
            }
        }

        public async Task<ServiceResponse<RenterProfileViewModel>> GetRenterProfileAsync(int userId)
        {
            var profile = await _context.RenterProfiles.FirstOrDefaultAsync(p => p.UserId.ToString() == userId.ToString());
            if (profile == null)
                return new ServiceResponse<RenterProfileViewModel> { Success = false, Errors = new[] { "Profile not found" } };

            return new ServiceResponse<RenterProfileViewModel>
            {
                Success = true,
                Data = new RenterProfileViewModel
                {
                    IsAdult = profile.IsAdult,
                    NationalId = profile.NationalId,
                    BirthRegistrationNo = profile.BirthRegistrationNo,
                    DateOfBirth = profile.DateOfBirth,
                    FullName = profile.FullName,
                    FatherName = profile.FatherName,
                    MotherName = profile.MotherName,
                    Nationality = profile.Nationality,
                    BloodGroup = profile.BloodGroup,
                    Profession = profile.Profession,
                    Gender = profile.Gender,
                    MobileNo = profile.MobileNo,
                    Email = profile.Email,
                    Division = profile.Division,
                    District = profile.District,
                    Upazila = profile.Upazila
                }
            };
        }

        public async Task<ServiceResponse<RenterProfileViewModel>> CreateRenterProfileAsync(int userId, RenterProfileViewModel model)
        {
            return await SaveRenterProfileAsync(userId, model);
        }
    }
} 