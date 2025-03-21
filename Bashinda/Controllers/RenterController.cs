using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Bashinda.Data;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using Microsoft.Extensions.Logging;
using System;
using System.Threading.Tasks;
using Bashinda.Services;
using Bashinda.ViewModels;
using System.Linq;
using Bashinda.Models;
using System.Text.Json;

namespace Bashinda.Controllers
{
    [Authorize(Roles = "ApartmentRenter")]
    public class RenterController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<RenterController> _logger;
        private readonly IRenterProfileService _renterProfileService;
        private readonly IRenterProfileApiService _renterProfileApiService;

        public RenterController(
            ApplicationDbContext context, 
            ILogger<RenterController> logger, 
            IRenterProfileService renterProfileService,
            IRenterProfileApiService renterProfileApiService)
        {
            _context = context;
            _logger = logger;
            _renterProfileService = renterProfileService;
            _renterProfileApiService = renterProfileApiService;
        }

        // Action for the first dummy page
        public async Task<IActionResult> DummyPage1()
        {
            if (!await IsProfileSubmittedAsync())
            {
                ViewBag.Message = "Please fill out the profile infos first.";
                return View("ProfileIncomplete");
            }
            
            if (!await IsApprovedAsync())
            {
                ViewBag.Message = "You can see the content on this page when you get approved.";
                return View("NotApproved");
            }
            
            ViewBag.WelcomeMessage = "Welcome to the exclusive content for approved renters!";
            return View();
        }

        // Action for the second dummy page
        public async Task<IActionResult> DummyPage2()
        {
            if (!await IsProfileSubmittedAsync())
            {
                ViewBag.Message = "Please fill out the profile infos first.";
                return View("ProfileIncomplete");
            }
            
            if (!await IsApprovedAsync())
            {
                ViewBag.Message = "You can see the content on this page when you get approved.";
                return View("NotApproved");
            }
            
            ViewBag.WelcomeMessage = "Welcome to our exclusive second page for approved renters!";
            return View();
        }

        // Check if the profile is submitted
        private async Task<bool> IsProfileSubmittedAsync()
        {
            if (User.Identity?.IsAuthenticated != true)
                return false;

            // Try to get token from cookie first
            var token = HttpContext.Request.Cookies["JwtToken"];

            // If token not in cookie, try from claims
            if (string.IsNullOrEmpty(token))
            {
                var tokenClaim = User.FindFirst("Token");
                if (tokenClaim != null && !string.IsNullOrEmpty(tokenClaim.Value))
                {
                    token = tokenClaim.Value;

                    // Recreate the cookie for future requests
                    Response.Cookies.Append("JwtToken", token, new CookieOptions
                    {
                        HttpOnly = true,
                        Secure = true,
                        SameSite = SameSiteMode.Lax,
                        Expires = DateTimeOffset.UtcNow.AddHours(1),
                        Path = "/"
                    });
                }
                else
                {
                    // No token found, log and return false
                    _logger.LogWarning("No JWT token found in cookie or claims");
                    return false;
                }
            }

            try
            {
                // First try the direct API call
                var (success, profile, errors) = await _renterProfileApiService.GetCurrentAsync(token);

                // If that succeeds, we're done
                if (success && profile != null)
                {
                    return true;
                }

                // If we get a NotFound error, it's possible the profile exists but can't be retrieved via the "current" endpoint
                if (!success && errors.Any(e => e.Contains("NotFound", StringComparison.OrdinalIgnoreCase) ||
                                           e.Contains("404", StringComparison.OrdinalIgnoreCase)))
                {
                    // Try using the user ID approach that's used in the Index action
                    var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                    if (!string.IsNullOrEmpty(userId))
                    {
                        // Check if there's a profile for this user in the database
                        var dbProfile = await _context.RenterProfiles
                            .FirstOrDefaultAsync(p => p.UserId.ToString() == userId);

                        return dbProfile != null;
                    }
                }

                return false;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error checking if profile is submitted");
                return false;
            }
        }

        // Check if the renter is approved
        private async Task<bool> IsApprovedAsync()
        {
            if (User.Identity?.IsAuthenticated != true)
                return false;

            // Try to get token from cookie first
            var token = HttpContext.Request.Cookies["JwtToken"];

            // If token not in cookie, try from claims
            if (string.IsNullOrEmpty(token))
            {
                var tokenClaim = User.FindFirst("Token");
                if (tokenClaim != null && !string.IsNullOrEmpty(tokenClaim.Value))
                {
                    token = tokenClaim.Value;

                    // Recreate the cookie for future requests
                    Response.Cookies.Append("JwtToken", token, new CookieOptions
                    {
                        HttpOnly = true,
                        Secure = true,
                        SameSite = SameSiteMode.Lax,
                        Expires = DateTimeOffset.UtcNow.AddHours(1),
                        Path = "/"
                    });
                }
                else
                {
                    // No token found, log and return false
                    _logger.LogWarning("No JWT token found in cookie or claims");
                    return false;
                }
            }

            try
            {
                // First try the direct API call
                var (success, profile, errors) = await _renterProfileApiService.GetCurrentAsync(token);

                // If that succeeds, check if approved
                if (success && profile != null)
                {
                    return profile.IsApproved;
                }

                // If we get a NotFound error, it's possible the profile exists but can't be retrieved via the "current" endpoint
                if (!success && errors.Any(e => e.Contains("NotFound", StringComparison.OrdinalIgnoreCase) ||
                                           e.Contains("404", StringComparison.OrdinalIgnoreCase)))
                {
                    // Try using the user ID approach that's used in the Index action
                    var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                    if (!string.IsNullOrEmpty(userId))
                    {
                        // Check if there's an approved profile for this user in the database
                        var dbProfile = await _context.RenterProfiles
                            .FirstOrDefaultAsync(p => p.UserId.ToString() == userId);

                        return dbProfile != null && dbProfile.IsApproved;
                    }
                }

                return false;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error checking if profile is approved");
                return false;
            }
        }

        public async Task<IActionResult> Index()
        {
            try
            {
                // Get the auth token from the cookie
                var token = HttpContext.Request.Cookies["JwtToken"];
                
                if (string.IsNullOrEmpty(token))
                {
                    _logger.LogWarning("JWT token not found in cookie");
                    
                    // Check if we can get a token from claims as a fallback
                    var tokenClaim = User.FindFirst("Token");
                    if (tokenClaim != null && !string.IsNullOrEmpty(tokenClaim.Value))
                    {
                        token = tokenClaim.Value;
                        _logger.LogInformation("Using token from claims instead of cookie");
                        
                        // Recreate the cookie for future requests
                        Response.Cookies.Append("JwtToken", token, new CookieOptions
                        {
                            HttpOnly = true,
                            Secure = true,
                            SameSite = SameSiteMode.Lax,
                            Expires = DateTimeOffset.UtcNow.AddHours(1),
                            Path = "/"
                        });
                    }
                    else
                    {
                        // Instead of redirecting to login (which can cause loops), 
                        // show a message asking the user to login again
                        TempData["ErrorMessage"] = "Your session has expired. Please login again.";
                        return View("~/Views/Shared/SessionExpired.cshtml");
                    }
                }
                
                // Use the API service to get the current renter profile
                _logger.LogInformation("Attempting to get renter profile with token: {TokenPrefix}...", token.Substring(0, Math.Min(15, token.Length)));
                var (success, profile, errors) = await _renterProfileApiService.GetCurrentAsync(token);
                
                if (!success)
                {
                    _logger.LogWarning("Failed to get renter profile: {Errors}", string.Join(", ", errors));
                    _logger.LogInformation("Trying alternative approach to get profile");
                    
                    // Try an alternative approach - get by user ID
                    var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                    if (!string.IsNullOrEmpty(userId))
                    {
                        _logger.LogInformation("Looking up profile for user ID: {UserId}", userId);
                        var dbProfile = await _context.RenterProfiles
                            .FirstOrDefaultAsync(p => p.UserId.ToString() == userId);
                            
                        if (dbProfile != null)
                        {
                            _logger.LogInformation("Found profile in database, returning to view");
                            
                            // Create a view model from the database profile
                            var dbViewModel = new RenterProfileViewModel
                            {
                                Id = dbProfile.Id,
                                UserId = dbProfile.UserId,
                                IsAdult = dbProfile.IsAdult,
                                NationalId = dbProfile.NationalId,
                                BirthRegistrationNo = dbProfile.BirthRegistrationNo,
                                DateOfBirth = dbProfile.DateOfBirth,
                                FullName = dbProfile.FullName,
                                FatherName = dbProfile.FatherName,
                                MotherName = dbProfile.MotherName,
                                MobileNo = dbProfile.MobileNo,
                                Email = dbProfile.Email,
                                Division = dbProfile.Division,
                                District = dbProfile.District,
                                Upazila = dbProfile.Upazila,
                                IsApproved = dbProfile.IsApproved,
                                // Default values for enums as strings
                                Nationality = "Bangladeshi",
                                BloodGroup = "APositive",
                                Gender = "Male",
                                Profession = "Other",
                                // Default values for missing fields
                                AreaType = "City",
                                Ward = "Default Ward",
                                Village = "Default Village",
                                PostCode = "1000",
                                HoldingNo = "Default"
                            };
                            
                            // Add a message when profile is not yet approved
                            if (!dbProfile.IsApproved)
                            {
                                TempData["InfoMessage"] = "Your profile has been submitted but is waiting for admin approval. You'll get full access once approved.";
                            }
                            
                            return View(dbViewModel);
                        }
                    }
                    
                    // Check if unauthorized (likely invalid/expired token)
                    if (errors.Any(e => e.Contains("unauthorized", StringComparison.OrdinalIgnoreCase) || 
                                      e.Contains("token", StringComparison.OrdinalIgnoreCase)))
                    {
                        TempData["ErrorMessage"] = "Your session has expired. Please login again.";
                        return View("~/Views/Shared/SessionExpired.cshtml");
                    }
                    
                    // If error is 404 Not Found, it means the user doesn't have a profile yet
                    // Redirect to the Create action to create a new profile
                    if (errors.Any(e => e.Contains("NotFound", StringComparison.OrdinalIgnoreCase) || 
                                     e.Contains("404", StringComparison.OrdinalIgnoreCase)))
                    {
                        _logger.LogInformation("User doesn't have a profile yet, redirecting to Create action");
                        TempData["InfoMessage"] = "Please create your profile to continue.";
                        return RedirectToAction("Create");
                    }
                    
                    TempData["ErrorMessage"] = errors.FirstOrDefault() ?? "Failed to load profile";
                    return View(new RenterProfileViewModel());
                }
                
                // Map the API DTO to view model
                var viewModel = new RenterProfileViewModel
                {
                    Id = profile.Id,
                    UserId = profile.UserId,
                    IsAdult = profile.IsAdult,
                    NationalId = profile.NationalId,
                    BirthRegistrationNo = profile.BirthRegistrationNo,
                    DateOfBirth = profile.DateOfBirth,
                    FullName = profile.FullName,
                    FatherName = profile.FatherName,
                    MotherName = profile.MotherName,
                    MobileNo = profile.MobileNo,
                    Email = profile.Email,
                    Division = profile.Division,
                    District = profile.District,
                    Upazila = profile.Upazila,
                    IsApproved = profile.IsApproved,
                    // Default values for enums as strings
                    Nationality = "Bangladeshi",
                    BloodGroup = "APositive",
                    Gender = "Male",
                    Profession = "Other",
                    // Default values for missing fields
                    AreaType = "City",
                    Ward = "Default Ward",
                    Village = "Default Village",
                    PostCode = "1000",
                    HoldingNo = "Default"
                };
                
                // Add a message when profile is not yet approved
                if (!profile.IsApproved)
                {
                    TempData["InfoMessage"] = "Your profile has been submitted but is waiting for admin approval. You'll get full access once approved.";
                }
                
                return View(viewModel);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error loading renter profile");
                TempData["ErrorMessage"] = "An error occurred while loading your profile";
                return View(new RenterProfileViewModel());
            }
        }

        // GET action for creating a new renter profile
        public async Task<IActionResult> Create()
        {
            try
            {
                // Get the token from the cookie
                var token = HttpContext.Request.Cookies["JwtToken"];
                
                // If token not in cookie, try from claims
                if (string.IsNullOrEmpty(token))
                {
                    var tokenClaim = User.FindFirst("Token");
                    if (tokenClaim != null && !string.IsNullOrEmpty(tokenClaim.Value))
                    {
                        token = tokenClaim.Value;
                    }
                    else
                    {
                        _logger.LogWarning("JWT token not found");
                        TempData["ErrorMessage"] = "Your session has expired. Please login again.";
                        return View("~/Views/Shared/SessionExpired.cshtml");
                    }
                }
                
                // Create a new view model
                var model = new RenterProfileViewModel();
                
                // Pre-fill with user's email if available
                var emailClaim = User.FindFirst(ClaimTypes.Email);
                if (emailClaim != null)
                {
                    model.Email = emailClaim.Value;
                }
                
                // Pre-fill with user's name if available
                var nameClaim = User.FindFirst(ClaimTypes.Name);
                if (nameClaim != null)
                {
                    model.FullName = nameClaim.Value;
                }
                
                return View(model);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error loading Create form");
                TempData["ErrorMessage"] = "An error occurred while loading the form.";
                return RedirectToAction("Index", "Home");
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(RenterProfileViewModel model)
        {
            if (!ModelState.IsValid)
            {
                // If model validation fails, return the form with errors
                return View(model);
            }

            try
            {
                // Get the JWT token from cookie
                var token = User.FindFirstValue("Token");
                _logger.LogInformation($"Using token for profile creation: {token?.Substring(0, 20)}...");

                // Check if the user already has a profile
                var existingProfile = await _renterProfileApiService.GetCurrentAsync(token);
                if (existingProfile.Success && existingProfile.Data != null)
                {
                    _logger.LogWarning("User already has a profile, redirecting to Dashboard");
                    return RedirectToAction("Dashboard");
                }

                // Upload files first if provided
                if (model.NationalIdImage != null && model.IsAdult)
                {
                    // Handle NID image upload
                    // This would be implemented in a real application
                }

                if (model.BirthRegistrationImage != null && !model.IsAdult)
                {
                    // Handle birth registration image upload
                    // This would be implemented in a real application
                }

                if (model.SelfImage != null)
                {
                    // Handle self image upload
                    // This would be implemented in a real application
                }

                // Create a new DTO to send to the API
                var dto = new CreateRenterProfileDto
                {
                    IsAdult = model.IsAdult,
                    NationalId = model.IsAdult ? model.NationalId : null,
                    BirthRegistrationNo = !model.IsAdult ? model.BirthRegistrationNo : null,
                    DateOfBirth = model.DateOfBirth,
                    FullName = model.FullName ?? string.Empty,
                    FatherName = model.FatherName ?? string.Empty,
                    MotherName = model.MotherName ?? string.Empty,
                    // Use string values directly
                    Nationality = model.Nationality,
                    BloodGroup = model.BloodGroup,
                    Profession = model.Profession,
                    Gender = model.Gender,
                    MobileNo = model.MobileNo ?? string.Empty,
                    Email = model.Email ?? string.Empty,
                    
                    // Use string-based location data directly from form
                    Division = model.Division ?? string.Empty,
                    District = model.District ?? string.Empty,
                    Upazila = model.Upazila ?? string.Empty,
                    // Use string value directly
                    AreaType = model.AreaType,
                    Ward = model.Ward ?? string.Empty,
                    Village = model.Village ?? string.Empty,
                    PostCode = model.PostCode ?? string.Empty,
                    HoldingNo = model.HoldingNo ?? string.Empty
                };

                _logger.LogInformation($"Submitting profile data: {JsonSerializer.Serialize(dto)}");

                // Send the data to the API to create the profile
                var response = await _renterProfileApiService.CreateAsync(dto, token);

                if (response.Success)
                {
                    _logger.LogInformation("Profile created successfully");
                    TempData["SuccessMessage"] = "Your profile has been created successfully. It is now pending approval by an administrator.";
                    return RedirectToAction("Dashboard");
                }
                else
                {
                    _logger.LogWarning($"Profile creation failed: {string.Join(", ", response.Errors)}");
                    foreach (var error in response.Errors)
                    {
                        ModelState.AddModelError(string.Empty, error);
                    }
                    return View(model);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating renter profile");
                ModelState.AddModelError(string.Empty, "An error occurred while creating your profile. Please try again.");
                return View(model);
            }
        }

        // Keep these sync versions for backward compatibility but update to use safe fallbacks
        private bool IsProfileSubmitted()
        {
            if (User.Identity?.IsAuthenticated != true)
                return false;

            var userIdClaim = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userIdClaim))
            {
                _logger.LogWarning("Invalid or missing user ID: {UserId}", userIdClaim);
                return false;
            }

            // Handle both string and int user IDs
            var renterProfile = _context.RenterProfiles.FirstOrDefault(p => p.UserId.ToString() == userIdClaim);
            return renterProfile != null;
        }

        private bool IsApproved()
        {
            if (User.Identity?.IsAuthenticated != true)
                return false;

            var userIdClaim = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userIdClaim))
            {
                _logger.LogWarning("Invalid or missing user ID: {UserId}", userIdClaim);
                return false;
            }

            // Handle both string and int user IDs
            var renterProfile = _context.RenterProfiles.FirstOrDefault(p => p.UserId.ToString() == userIdClaim);
            return renterProfile != null && renterProfile.IsApproved;
        }

        // Action to view the renter's profile
        public async Task<IActionResult> ViewProfile()
        {
            try
            {
                // Try to get token from cookie first
                var token = HttpContext.Request.Cookies["JwtToken"];
                
                // If token not in cookie, try from claims
                if (string.IsNullOrEmpty(token))
                {
                    var tokenClaim = User.FindFirst("Token");
                    if (tokenClaim != null && !string.IsNullOrEmpty(tokenClaim.Value))
                    {
                        token = tokenClaim.Value;
                        
                        // Recreate the cookie for future requests
                        Response.Cookies.Append("JwtToken", token, new CookieOptions
                        {
                            HttpOnly = true,
                            Secure = true,
                            SameSite = SameSiteMode.Lax,
                            Expires = DateTimeOffset.UtcNow.AddHours(1),
                            Path = "/"
                        });
                    }
                    else
                    {
                        // No token found, redirect to login
                        _logger.LogWarning("No JWT token found in cookie or claims for ViewProfile");
                        TempData["ErrorMessage"] = "Your session has expired. Please login again.";
                        return View("~/Views/Shared/SessionExpired.cshtml");
                    }
                }
                
                // Use the API service to get the current renter profile
                var (success, profile, errors) = await _renterProfileApiService.GetCurrentAsync(token);
                
                if (!success)
                {
                    _logger.LogWarning("Failed to get renter profile: {Errors}", string.Join(", ", errors));
                    
                    // Check if unauthorized (likely invalid/expired token)
                    if (errors.Any(e => e.Contains("unauthorized", StringComparison.OrdinalIgnoreCase) || 
                                    e.Contains("token", StringComparison.OrdinalIgnoreCase)))
                    {
                        TempData["ErrorMessage"] = "Your session has expired. Please login again.";
                        return View("~/Views/Shared/SessionExpired.cshtml");
                    }
                    
                    // If error is 404 Not Found, it means the user doesn't have a profile yet
                    // Redirect to the Create action to create a new profile
                    if (errors.Any(e => e.Contains("NotFound", StringComparison.OrdinalIgnoreCase) || 
                                     e.Contains("404", StringComparison.OrdinalIgnoreCase)))
                    {
                        _logger.LogInformation("User doesn't have a profile yet, redirecting to Create action");
                        TempData["InfoMessage"] = "Please create your profile to continue.";
                        return RedirectToAction("Create");
                    }
                    
                    TempData["ErrorMessage"] = errors.FirstOrDefault() ?? "Failed to load profile";
                    return View(new RenterProfileViewModel());
                }
                
                // Map the API DTO to view model
                var viewModel = new RenterProfileViewModel
                {
                    Id = profile.Id,
                    UserId = profile.UserId,
                    IsAdult = profile.IsAdult,
                    NationalId = profile.NationalId,
                    BirthRegistrationNo = profile.BirthRegistrationNo,
                    DateOfBirth = profile.DateOfBirth,
                    FullName = profile.FullName,
                    FatherName = profile.FatherName,
                    MotherName = profile.MotherName,
                    MobileNo = profile.MobileNo,
                    Email = profile.Email,
                    Division = profile.Division,
                    District = profile.District,
                    Upazila = profile.Upazila,
                    IsApproved = profile.IsApproved,
                    // Default values for enums as strings
                    Nationality = "Bangladeshi",
                    BloodGroup = "APositive",
                    Gender = "Male",
                    Profession = "Other",
                    // Default values for missing fields
                    AreaType = "City",
                    Ward = "Default Ward",
                    Village = "Default Village",
                    PostCode = "1000",
                    HoldingNo = "Default"
                };
                
                // Add a message when profile is not yet approved
                if (!profile.IsApproved)
                {
                    TempData["InfoMessage"] = "Your profile has been submitted but is waiting for admin approval. You'll get full access once approved.";
                }
                
                return View(viewModel);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in ViewProfile action");
                TempData["ErrorMessage"] = "An error occurred while loading your profile.";
                return View(new RenterProfileViewModel());
            }
        }
    }
} 