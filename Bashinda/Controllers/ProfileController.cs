using Bashinda.Data;
using Bashinda.Models;
using Bashinda.Services;
using Bashinda.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics;

namespace Bashinda.Controllers
{
    [Authorize]
    public class ProfileController : Controller
    {
        private readonly IRenterProfileService _renterProfileService;
        private readonly ILogger<ProfileController> _logger;
        private readonly ApplicationDbContext _context;

        public ProfileController(IRenterProfileService renterProfileService, ILogger<ProfileController> logger, ApplicationDbContext context)
        {
            _renterProfileService = renterProfileService;
            _logger = logger;
            _context = context;
        }

        // GET: /Profile/Renter
        public IActionResult Renter()
        {
            _logger.LogInformation("Redirecting from ProfileController.Renter to RenterController.ViewProfile");
            // Redirect to the new RenterController.ViewProfile action
            return RedirectToAction("ViewProfile", "Renter");
        }

        // POST: /Profile/Renter
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Renter(RenterProfileViewModel model)
        {
            // Remove file validation errors regardless of IsAdult status
            // These are handled specially in the service
            ModelState.Remove("NationalIdImage");
            ModelState.Remove("BirthRegistrationImage");
            ModelState.Remove("SelfImage");
            
            // Custom validation for conditional fields
            if (model.IsAdult)
            {
                // If adult, validate NationalId but clear BirthRegistrationNo validation errors
                if (string.IsNullOrWhiteSpace(model.NationalId))
                {
                    ModelState.AddModelError("NationalId", "National ID is required for adults.");
                }
                // Remove validation errors for non-adult fields
                ModelState.Remove("BirthRegistrationNo");
            }
            else
            {
                // If not adult, validate BirthRegistrationNo but clear NationalId validation errors
                if (string.IsNullOrWhiteSpace(model.BirthRegistrationNo))
                {
                    ModelState.AddModelError("BirthRegistrationNo", "Birth Registration Number is required for non-adults.");
                }
                // Remove validation errors for adult fields
                ModelState.Remove("NationalId");
            }

            if (!ModelState.IsValid)
            {
                _logger.LogWarning("Model validation failed when saving profile");
                foreach (var error in ModelState.Values.SelectMany(v => v.Errors))
                {
                    _logger.LogWarning("Validation error: {Error}", error.ErrorMessage);
                }
                return View(model);
            }

            try
            {
                // Check if UserId claim exists
                var userIdClaim = User.FindFirst("UserId");
                if (userIdClaim == null)
                {
                    _logger.LogWarning("UserId claim not found for user");
                    ModelState.AddModelError("", "User ID not found in claims. Please log out and log in again.");
                    return View(model);
                }

                if (!int.TryParse(userIdClaim.Value, out int userId))
                {
                    _logger.LogWarning("Invalid UserId format: {UserId}", userIdClaim.Value);
                    ModelState.AddModelError("", "Invalid user ID format. Please log out and log in again.");
                    return View(model);
                }

                _logger.LogInformation("Attempting to save profile for user {UserId}", userId);

                var result = await _renterProfileService.SaveRenterProfileAsync(userId, model);
                if (result.Success)
                {
                    _logger.LogInformation("Profile saved successfully for user {UserId}", userId);
                    TempData["Message"] = "Profile saved successfully! Awaiting admin approval.";
                    
                    // Redirect to the same action which will now display the profile view
                    return RedirectToAction("Renter");
                }
                else
                {
                    _logger.LogWarning("Failed to save profile for user {UserId}. Errors: {Errors}", 
                        userId, string.Join(", ", result.Errors));
                    
                    foreach (var error in result.Errors)
                    {
                        ModelState.AddModelError("", error);
                    }
                    return View(model);
                }
            }
            catch (Exception ex)
            {
                // Get the complete exception chain
                string allErrors = GetFullExceptionMessage(ex);
                
                // Log the complete exception chain
                _logger.LogError(ex, "Error saving renter profile: {ErrorMessage}", allErrors);
                
                // Add detailed error message in development environment
                #if DEBUG
                ModelState.AddModelError("", $"Error details: {allErrors}");
                #else
                ModelState.AddModelError("", "An error occurred while saving your profile. Please try again.");
                #endif
                
                return View(model);
            }
        }
        
        // Helper method to get the full exception message including inner exceptions
        private string GetFullExceptionMessage(Exception ex)
        {
            var message = ex.Message;
            var innerException = ex.InnerException;
            
            while (innerException != null)
            {
                message += " | Inner exception: " + innerException.Message;
                innerException = innerException.InnerException;
            }
            
            return message;
        }
    }
}
