using Bashinda.Services;
using Bashinda.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using System.Text.Json;

namespace Bashinda.Controllers
{
    [Authorize(Roles = "ApartmentOwner")]
    public class OwnerController : Controller
    {
        private readonly IApartmentOwnerProfileApiService _apartmentOwnerProfileApiService;
        private readonly ILogger<OwnerController> _logger;

        public OwnerController(IApartmentOwnerProfileApiService apartmentOwnerProfileApiService, ILogger<OwnerController> logger)
        {
            _apartmentOwnerProfileApiService = apartmentOwnerProfileApiService;
            _logger = logger;
        }

        public IActionResult Index()
        {
            return View();
        }

        public async Task<IActionResult> Dashboard()
        {
            try
            {
                // Get token with unified handling
                var token = GetToken();
                if (string.IsNullOrEmpty(token))
                {
                    TempData["ErrorMessage"] = "Your session has expired. Please login again.";
                    return RedirectToAction("Login", "Account");
                }

                // Use the API service to get the current owner profile
                _logger.LogInformation("Attempting to get owner profile with token: {TokenPrefix}...", token.Substring(0, Math.Min(15, token.Length)));
                var (success, profile, errors) = await _apartmentOwnerProfileApiService.GetCurrentAsync(token);

                if (!success)
                {
                    _logger.LogWarning("Failed to get owner profile: {Errors}", string.Join(", ", errors));
                    _logger.LogInformation("Trying alternative approach to get profile");

                    // Try an alternative approach - get by user ID
                    var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                    if (!string.IsNullOrEmpty(userId))
                    {
                        _logger.LogInformation("Looking up profile for user ID: {UserId}", userId);
                         (success, profile, errors) = await _apartmentOwnerProfileApiService.OwnerProfilesInfo(userId, token);


                        if (profile != null)
                        {
                            _logger.LogInformation("Found profile in database, returning to view");

                            // Create a view model from the database profile
                            var ViewModel = new ApartmentOwnerProfileDto
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

                            return View(ViewModel);
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
                    return View(new ApartmentOwnerProfileDto());
                }

                // Map the API DTO to view model
                var viewModel = new ApartmentOwnerProfileDto
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
                return View(new ApartmentOwnerProfileDto());
            }
        }

        [HttpGet]
        public async Task<IActionResult> Create()
        {
            try
            {
                // Get token with unified handling
                var token = GetToken();
                _logger.LogInformation($"Using token for profile creation: {token.Substring(0, Math.Min(15, token.Length))}...");
                if (string.IsNullOrEmpty(token))
                {
                    _logger.LogWarning("Token is null or empty, redirecting to login");
                    TempData["ErrorMessage"] = "Your session has expired. Please login again.";
                    return RedirectToAction("Login", "Account");
                }

                // Create a new view model
                var model = new OwnerProfileViewModel();
                _logger.LogInformation("Creating new OwnerProfileViewModel");

                // Pre-fill with user's email if available
                var emailClaim = User.FindFirst(ClaimTypes.Email);
                _logger.LogInformation("Checking for email claim");
                if (emailClaim != null)
                {
                    _logger.LogInformation("Email claim found: {Email}", emailClaim.Value);
                    model.Email = emailClaim.Value;
                }

                // Pre-fill with user's name if available
                var nameClaim = User.FindFirst(ClaimTypes.Name);
                _logger.LogInformation("Checking for name claim");
                if (nameClaim != null)
                {
                    _logger.LogInformation("Name claim found: {Name}", nameClaim.Value);
                    model.FullName = nameClaim.Value;
                }
                _logger.LogInformation("Returning Create view with model: {Model}", JsonSerializer.Serialize(model));
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
        public async Task<IActionResult> Create(OwnerProfileViewModel model)
        {
            if (!ModelState.IsValid)
            {
                // If the model state is invalid, return the view with the current model
                return View(model);
            }
            try
            {
                // Get token with unified handling
                var token = GetToken();
                _logger.LogInformation($"Using token for profile creation: {token.Substring(0, Math.Min(15, token.Length))}...");
                if (string.IsNullOrEmpty(token))
                {
                    _logger.LogWarning("Token is null or empty, redirecting to login");
                    TempData["ErrorMessage"] = "Your session has expired. Please login again.";
                    return RedirectToAction("Login", "Account");
                }
                _logger.LogInformation($"Using token for profile creation: {token.Substring(0, Math.Min(15, token.Length))}...");

                //check if the user already has a profile
                var existingProfile = await _apartmentOwnerProfileApiService.GetCurrentAsync(token);
                _logger.LogInformation($"Profile retrieval success: {existingProfile.Success}, Profile: {string.Join(", ", existingProfile.Data)}, Errors: {string.Join(", ", existingProfile.Errors)}");
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
                var dto = new CreateApartmentOwnerProfileDto
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

                //Send the data to the API to create the profile
                var response = await _apartmentOwnerProfileApiService.CreateAsync(dto, token);

                if (response.Success)
                {
                    // If successful, redirect to the dashboard
                    _logger.LogInformation("Profile created successfully");
                    TempData["SuccessMessage"] = "Profile created successfully.";
                    return RedirectToAction("Dashboard");
                }
                else
                {
                    // If there are errors, add them to the model state
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

        public async Task<IActionResult> ViewProfile()
        {
            try
            {
                // Get token with unified handling
                var token = GetToken();
                _logger.LogInformation($"Using token for profile view: {token.Substring(0, Math.Min(15, token.Length))}...");
                if (string.IsNullOrEmpty(token))
                {
                    _logger.LogWarning("Token is null or empty, redirecting to login");
                    TempData["ErrorMessage"] = "Your session has expired. Please login again.";
                    return RedirectToAction("Login", "Account");
                }

                var (success, profile, errors) = await _apartmentOwnerProfileApiService.GetCurrentAsync(token);
                _logger.LogInformation($"Profile retrieval success: {success},Profile: {string.Join(", ", profile)}, Errors: {string.Join(", ", errors)}");
                if (!success)
                {
                    _logger.LogWarning("Failed to get owner profile: {Errors}", string.Join(", ", errors));

                    // Handle specific error cases
                    if (IsProfileNotFoundError(errors))
                    {
                        _logger.LogInformation("User doesn't have a profile yet, redirecting to Create action");
                        TempData["InfoMessage"] = "Please create your profile to continue.";
                        return RedirectToAction("Create");
                    }

                    if (IsAuthorizationError(errors))
                    {
                        _logger.LogWarning("Unauthorized access, clearing cookies and redirecting to login");
                        ClearAuthCookies();
                        TempData["ErrorMessage"] = "Your session has expired. Please login again.";
                        return RedirectToAction("Login", "Account");
                    }

                    _logger.LogWarning("Unknown error occurred: {Errors}", string.Join(", ", errors));
                    TempData["ErrorMessage"] = errors.FirstOrDefault() ?? "Failed to load profile";
                    return View(new OwnerProfileViewModel());
                }


                if (profile == null)
                {
                    _logger.LogWarning("Profile is null, redirecting to Create action");
                    TempData["ErrorMessage"] = "Profile not found.";
                    return View(new OwnerProfileViewModel());
                }
                _logger.LogInformation("Profile retrieved successfully, mapping to view model");
                return View(MapToViewModel(profile));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in ViewProfile action");
                TempData["ErrorMessage"] = "An error occurred while loading your profile.";
                return View(new OwnerProfileViewModel());
            }
        }

        private string? GetToken()
        {
            const string cookieName = "JwtToken";

            // Try cookie first
            var token = HttpContext.Request.Cookies[cookieName];

            // Fallback to claims
            if (string.IsNullOrEmpty(token))
            {
                token = User.FindFirst("Token")?.Value;

                // Persist to cookie if found in claims
                if (!string.IsNullOrEmpty(token))
                {
                    Response.Cookies.Append(cookieName, token, new CookieOptions
                    {
                        HttpOnly = true,
                        Secure = true,
                        SameSite = SameSiteMode.Strict,
                        Expires = DateTimeOffset.UtcNow.AddHours(1),
                        Path = "/"
                    });
                }
            }

            return token;
        }

        private bool IsProfileNotFoundError(IEnumerable<string> errors)
        {
            return errors.Any(e => e.Contains("not found", StringComparison.OrdinalIgnoreCase) ||
                                 e.Contains("404", StringComparison.OrdinalIgnoreCase));
        }

        private bool IsAuthorizationError(IEnumerable<string> errors)
        {
            return errors.Any(e => e.Contains("unauthorized", StringComparison.OrdinalIgnoreCase) ||
                                 e.Contains("401", StringComparison.OrdinalIgnoreCase));
        }

        private void ClearAuthCookies()
        {
            Response.Cookies.Delete("JwtToken");
            HttpContext.Session.Remove("token");
        }

        private OwnerProfileViewModel MapToViewModel(ApartmentOwnerProfileDto profile)
        {
            return new OwnerProfileViewModel
            {
                Id = profile.Id,
                UserId = profile.UserId,
                IsAdult = profile.IsAdult,
                FullName = profile.FullName,
                MobileNo = profile.MobileNo,
                Email = profile.Email,
                NationalId = profile.NationalId,
                BirthRegistrationNo = profile.BirthRegistrationNo,
                FatherName = profile.FatherName,
                Division = profile.Division,
                District = profile.District,
                Upazila = profile.Upazila,
                AreaType = profile.AreaType,
                Ward = profile.Ward,
                Village = profile.Village,
                PostCode = profile.PostCode,
                HoldingNo = profile.HoldingNo,
                UniqueId = profile.UniqueId,
                IsApproved = profile.IsApproved
            };
        }
    }
}
