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
            if (!User.Identity.IsAuthenticated)
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
                var (success, profile, _) = await _renterProfileApiService.GetCurrentAsync(token);
                return success && profile != null;
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
            if (!User.Identity.IsAuthenticated)
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
                var (success, profile, _) = await _renterProfileApiService.GetCurrentAsync(token);
                return success && profile != null && profile.IsApproved;
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
                
                // Map the API DTO to view model, parsing enum values
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
                    IsApproved = profile.IsApproved
                };
                
                // Parse enum values
                if (Enum.TryParse<Nationality>(profile.Nationality, out var nationality))
                    viewModel.Nationality = nationality;
                    
                if (Enum.TryParse<BloodGroup>(profile.BloodGroup, out var bloodGroup))
                    viewModel.BloodGroup = bloodGroup;
                    
                if (Enum.TryParse<Profession>(profile.Profession, out var profession))
                    viewModel.Profession = profession;
                    
                if (Enum.TryParse<Gender>(profile.Gender, out var gender))
                    viewModel.Gender = gender;
                
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
                return View(model);
            }

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
                        _logger.LogWarning("JWT token not found");
                        TempData["ErrorMessage"] = "Your session has expired. Please login again.";
                        return View("~/Views/Shared/SessionExpired.cshtml");
                    }
                }
                
                // Get the user ID from the claims
                var userIdClaim = User.FindFirst("UserId") ?? User.FindFirst(ClaimTypes.NameIdentifier);
                if (userIdClaim == null)
                {
                    _logger.LogWarning("User ID claim not found");
                    TempData["ErrorMessage"] = "User ID not found. Please login again.";
                    return View("~/Views/Shared/SessionExpired.cshtml");
                }

                if (!int.TryParse(userIdClaim.Value, out int userId))
                {
                    _logger.LogWarning("Invalid user ID format: {UserId}", userIdClaim.Value);
                    
                    // Try to get the user profile via API to get the ID
                    var profileResult = await _renterProfileApiService.GetCurrentAsync(token);
                    bool profileSuccess = profileResult.Success;
                    RenterProfileDto? profile = profileResult.Data;
                    
                    if (profileSuccess && profile != null)
                    {
                        userId = profile.UserId;
                        _logger.LogInformation("Retrieved user ID from API: {UserId}", userId);
                    }
                    else
                    {
                        TempData["ErrorMessage"] = "Could not determine your user ID. Please login again.";
                        return View("~/Views/Shared/SessionExpired.cshtml");
                    }
                }

                // Get the required services
                var locationDataService = HttpContext.RequestServices.GetRequiredService<ILocationDataService>();
                var locationApiService = HttpContext.RequestServices.GetRequiredService<ILocationApiService>();
                
                // Create a logger factory and create a logger for the IRenterProfileApiService
                var loggerFactory = HttpContext.RequestServices.GetRequiredService<ILoggerFactory>();
                var serviceLogger = loggerFactory.CreateLogger<IRenterProfileApiService>();
                
                // Use the extension method to create the profile with location IDs
                var result = await _renterProfileApiService.CreateWithLocationIdsAsync(
                    model, 
                    locationDataService, 
                    locationApiService, 
                    serviceLogger, 
                    token);
                    
                bool success = result.Success;
                RenterProfileDto? createdProfile = result.Data;
                string[] errors = result.Errors;
                
                if (!success)
                {
                    foreach (var error in errors)
                    {
                        ModelState.AddModelError("", error);
                    }
                    return View(model);
                }

                TempData["SuccessMessage"] = "Profile created successfully";
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating renter profile: {Message}", ex.Message);
                ModelState.AddModelError("", "An error occurred while creating your profile");
                return View(model);
            }
        }

        // Keep these sync versions for backward compatibility but update to use safe fallbacks
        private bool IsProfileSubmitted()
        {
            if (!User.Identity.IsAuthenticated)
                return false;

            var userIdClaim = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userIdClaim) || !int.TryParse(userIdClaim, out int userId))
            {
                _logger.LogWarning("Invalid or missing user ID: {UserId}", userIdClaim);
                return false;
            }
            
            var renterProfile = _context.RenterProfiles.FirstOrDefault(p => p.UserId == userId);
            return renterProfile != null;
        }

        private bool IsApproved()
        {
            if (!User.Identity.IsAuthenticated)
                return false;

            var userIdClaim = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userIdClaim) || !int.TryParse(userIdClaim, out int userId))
            {
                _logger.LogWarning("Invalid or missing user ID: {UserId}", userIdClaim);
                return false;
            }
            
            var renterProfile = _context.RenterProfiles.FirstOrDefault(p => p.UserId == userId);
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
                    
                    TempData["ErrorMessage"] = errors.FirstOrDefault() ?? "Failed to load profile";
                    return View(new RenterProfileViewModel());
                }
                
                // Map the API DTO to view model, parsing enum values
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
                    IsApproved = profile.IsApproved
                };
                
                // Parse enum values
                if (Enum.TryParse<Nationality>(profile.Nationality, out var nationality))
                    viewModel.Nationality = nationality;
                    
                if (Enum.TryParse<BloodGroup>(profile.BloodGroup, out var bloodGroup))
                    viewModel.BloodGroup = bloodGroup;
                    
                if (Enum.TryParse<Gender>(profile.Gender, out var gender))
                    viewModel.Gender = gender;
                    
                if (Enum.TryParse<Profession>(profile.Profession, out var profession))
                    viewModel.Profession = profession;

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