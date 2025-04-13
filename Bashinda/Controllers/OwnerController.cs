using Bashinda.Services;
using Bashinda.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using System.Security.Cryptography.Xml;

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

        [HttpGet]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(OwnerProfileViewModel ownerProfileViewModel)
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
                var model = new OwnerProfileViewModel();

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


        public async Task<IActionResult> ViewProfile()
        {
            try
            {
                // Try to get token from cookie first
                var token = HttpContext.Request.Cookies["token"];

                // If token is not found in cookie, try to get it from the session
                if (string.IsNullOrEmpty(token))
                {
                    token = HttpContext.Session.GetString("token");
                }

                // If token not in cookie, try from claims
                if (string.IsNullOrEmpty(token))
                {
                    var tokenClaim = User.FindFirst("token");
                    if (tokenClaim != null && !string.IsNullOrEmpty(tokenClaim.Value))
                    {
                        token = tokenClaim.Value;

                        // Set the token in the cookie for future requests
                        Response.Cookies.Append("token", token, new CookieOptions
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
                        // No token found, redirect to login
                        _logger.LogWarning("No JWT token found in cookie or claims for ViewProfile");
                        TempData["ErrorMessage"] = "Your session has expired. Please login again.";
                        return RedirectToAction("Login", "Account");
                    }
                }

                //Use the api service to get the current owner profile
                var (success, profile, errors) = await _apartmentOwnerProfileApiService.GetCurrentAsync(token);

                if(!success)
                {
                    _logger.LogWarning("Failed to get owner profile: {Errors}", string.Join(", ", errors));

                    // Check if unauthorized (likely invalid/expired token)
                    if (errors.Any(e => e.Contains("unauthorized", StringComparison.OrdinalIgnoreCase) ||
                                    e.Contains("token", StringComparison.OrdinalIgnoreCase)))
                    {
                        _logger.LogInformation("User unauthorized, redirecting to login");

                        TempData["ErrorMessage"] = "Your session has expired. Please login again.";
                        return RedirectToAction("Login", "Account");
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
                    return View(new OwnerProfileViewModel());
                }
                if(profile == null)
                {
                    TempData["ErrorMessage"] = "Profile not found.";
                    return View(new OwnerProfileViewModel());
                }
                //Map the API DTO to view model
                var viewModel = new OwnerProfileViewModel
                {
                    Id = profile.Id,
                    UserId = profile.UserId,
                    IsAdult = profile.IsAdult,
                    FullName = profile.FullName,
                    MobileNo = profile.MobileNo,
                    Email = profile.Email,
                    NationalId = profile.NationalId,
                    //NationalIdImage = profile.NationalIdImagePath,
                    BirthRegistrationNo = profile.BirthRegistrationNo,
                    //BirthRegistrationImage = profile.BirthRegistrationImagePath,
                    FatherName = profile.FatherName,
                    //SelfImage = profile.SelfImagePath,
                    Division = profile.Division,
                    District = profile.District,
                    Upazila = profile.Upazila,
                    AreaType = profile.AreaType,
                    Ward = profile.Ward,
                    Village = profile.Village,
                    PostCode = profile.PostCode,
                    HoldingNo = profile.HoldingNo,
                    UniqueId = profile.UniqueId,
                };

                //Add a message when profile is not yet approved
                if (!profile.IsApproved)
                {
                    TempData["InfoMessage"] = "Your profile is not yet approved. Please wait for the admin to approve it.";
                }


                // Add your logic here to handle the token and return a view
                return View(viewModel); // Placeholder for actual implementation
            }
            catch (Exception ex)
            {
                // Handle exceptions
                _logger.LogError(ex, "Error in ViewProfile action");
                TempData["ErrorMessage"] = "An error occurred while loading your profile.";
                return View(new OwnerProfileViewModel());
            }
        }
    }
}
