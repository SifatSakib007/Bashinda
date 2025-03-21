using Bashinda.Services;
using Bashinda.ViewModels;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace Bashinda.Controllers
{
    [Authorize(Roles = "Admin")]
    public class AdminController : Controller
    {
        private readonly IAdminApiService _adminService;
        private readonly ILocationDataService _locationService;
        private readonly IAdminLocationService _adminLocationService;
        private readonly ILogger<AdminController> _logger;


        public AdminController(
            IAdminApiService adminService,
            ILocationDataService locationService,
            IAdminLocationService adminLocationService,
            ILogger<AdminController> logger)
        {
            _adminService = adminService;
            _locationService = locationService;
            _adminLocationService = adminLocationService;
            _logger = logger;
        }

        public async Task<IActionResult> Dashboard()
        {
            try
            {
                var token = GetAuthToken();
                if (string.IsNullOrEmpty(token))
                {
                    _logger.LogWarning("No valid authentication token found. Redirecting to login.");
                    // Clear session and sign out
                    await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
                    HttpContext.Session.Remove("AuthToken");
                    TempData["ErrorMessage"] = "Your session has expired. Please log in again.";
                    return RedirectToAction("Login", "Account");
                }
                var filters = await GetAdminLocationFilters();

                _logger.LogInformation("Dashboard filters: {@Filters}", filters); 

                var (success, dashboard, errors) = await _adminService.GetDashboardAsync(token, filters);
                if (!success && errors.Any(e => e.Contains("Unauthorized") || e.Contains("401")))
                {
                    // Clear session, token, and sign out
                    await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
                    HttpContext.Session.Remove("AuthToken");
                    TempData["ErrorMessage"] = "Your session has expired. Please log in again to access the admin dashboard.";
                    return RedirectToAction("Login", "Account");
                }
                //if (!success) return HandleError(errors, "Dashboard");
                if (!success)
                {
                    _logger.LogError("Error loading admin dashboard: {Errors}", string.Join(", ", errors));
                    TempData["ErrorMessage"] = "An error occurred while loading the dashboard.";
                    return RedirectToAction("Index", "Home"); // Redirect to a non-login page to avoid l

                }
                ViewBag.LocationScope = _adminLocationService.FormatLocationScope(filters);
                ViewBag.Divisions = await _locationService.GetDivisionsAsync();

                //        // Pass data to view
                ViewBag.PendingRenters = dashboard.PendingRenters;
                ViewBag.PendingOwners = dashboard.PendingOwners;
                ViewBag.TotalUsers = dashboard.TotalUsers;
                ViewBag.ApprovedRenters = dashboard.ApprovedRenters;
                ViewBag.TotalRenters = dashboard.TotalRenters;
                ViewBag.ApprovedOwners = dashboard.ApprovedOwners;
                ViewBag.TotalOwners = dashboard.TotalOwners;
                ViewBag.TotalApartments = dashboard.TotalApartments;
                ViewBag.RenterPercentage = dashboard.RenterApprovalPercentage;
                ViewBag.OwnerPercentage = dashboard.OwnerApprovalPercentage;

                // Get recent activity (placeholder for now)
                ViewBag.RecentActivity = new List<dynamic>
                        {
                            new { Title = "New User Registration", Description = "A new user has registered on the platform.", UserName = "System", Timestamp = DateTime.Now.AddHours(-1) },
                            new { Title = "Profile Approved", Description = "Admin approved a renter profile.", UserName = "Admin", Timestamp = DateTime.Now.AddHours(-3) },
                            new { Title = "New Apartment Listed", Description = "A new apartment has been listed.", UserName = "ApartmentOwner", Timestamp = DateTime.Now.AddHours(-5) }
                        };

                return View("Dashboard");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Dashboard error");
                return RedirectToAction("Index", "Home");
            }
        }

        // GET: /Admin
        //public async Task<IActionResult> Index()
        //{
        //    try
        //    {
        //        var token = GetAuthToken();
                
        //        if (string.IsNullOrEmpty(token))
        //        {
        //            _logger.LogWarning("No valid authentication token found. Redirecting to login.");
        //            TempData["ErrorMessage"] = "Your session has expired. Please log in again.";
        //            return RedirectToAction("Login", "Account");
        //        }
                
        //        _logger.LogInformation("Making API request with token: {TokenPrefix}", token.Substring(0, Math.Min(10, token.Length)) + "...");
                
        //        // Try to make the API request
        //        var (success, dashboard, errors) = await _adminService.GetDashboardAsync(token);
                
        //        // If unauthorized, attempt to get a fresh token through the current HttpContext identity
        //        if (!success && errors.Any(e => e.Contains("Unauthorized") || e.Contains("401")))
        //        {
        //            _logger.LogWarning("Received 401 Unauthorized, attempting alternative authorization approach...");
                    
        //            // Log all headers that were sent
        //            _logger.LogInformation("Checking if we need to force API URL override...");
                    
        //            // Clear the token from session
        //            HttpContext.Session.Remove("AuthToken");
                    
        //            // Create direct notification for user
        //            TempData["ErrorMessage"] = "Your session has expired. Please log in again to access the admin dashboard.";
                    
        //            // Redirect to login
        //            return RedirectToAction("Login", "Account");
        //        }
                
        //        if (!success)
        //        {
        //            _logger.LogError("Error loading admin dashboard: {Errors}", string.Join(", ", errors));
        //            TempData["ErrorMessage"] = "An error occurred while loading the dashboard.";
        //            return RedirectToAction("Index", "Home");
        //        }
                
        //        // Pass data to view
        //        ViewBag.PendingRenters = dashboard.PendingRenters;
        //        ViewBag.PendingOwners = dashboard.PendingOwners;
        //        ViewBag.TotalUsers = dashboard.TotalUsers;
        //        ViewBag.ApprovedRenters = dashboard.ApprovedRenters;
        //        ViewBag.TotalRenters = dashboard.TotalRenters;
        //        ViewBag.ApprovedOwners = dashboard.ApprovedOwners;
        //        ViewBag.TotalOwners = dashboard.TotalOwners;
        //        ViewBag.TotalApartments = dashboard.TotalApartments;
        //        ViewBag.RenterPercentage = dashboard.RenterApprovalPercentage;
        //        ViewBag.OwnerPercentage = dashboard.OwnerApprovalPercentage;
                
        //        // Get recent activity (placeholder for now)
        //        ViewBag.RecentActivity = new List<dynamic>
        //        {
        //            new { Title = "New User Registration", Description = "A new user has registered on the platform.", UserName = "System", Timestamp = DateTime.Now.AddHours(-1) },
        //            new { Title = "Profile Approved", Description = "Admin approved a renter profile.", UserName = "Admin", Timestamp = DateTime.Now.AddHours(-3) },
        //            new { Title = "New Apartment Listed", Description = "A new apartment has been listed.", UserName = "ApartmentOwner", Timestamp = DateTime.Now.AddHours(-5) }
        //        };
                
        //        return View("Dashboard");
        //    }
        //    catch (Exception ex)
        //    {
        //        _logger.LogError(ex, "Error loading admin dashboard");
        //        TempData["ErrorMessage"] = "An error occurred while loading the dashboard.";
        //        return RedirectToAction("Index", "Home");
        //    }
        //}
        public async Task<IActionResult> PendingApprovals()
        {
            try
            {
                var token = GetAuthToken();
                var filters = await GetAdminLocationFilters();
                _logger.LogInformation("PendingApprovals filters: {@Filters}", filters);
                var (success, profiles, errors) = await _adminService.GetPendingProfilesAsync(token, filters);

                if (!success)
                {
                    _logger.LogError("PendingApprovals errors: {Errors}", string.Join(", ", errors));
                    return HandleError(errors, "Pending Approvals");
                }

                await PopulateLocationDropdowns();
                ViewBag.LocationScope = _adminLocationService.FormatLocationScope(filters);

                // Cast to correct type
                var renterProfiles = profiles.Select(p => new RenterProfileListDto
                {
                    Id = p.Id,
                    FullName = p.FullName,
                    MobileNo = p.MobileNo,
                    Email = p.Email,
                    NationalId = p.NationalId,
                    BirthRegistrationNo = p.BirthRegistrationNo,
                    DateOfBirth = p.DateOfBirth,
                    SelfImagePath = p.SelfImagePath,
                    IsApproved = p.IsApproved // Explicit conversion with null-coalescing operator
                }).ToList();

                return View(renterProfiles);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Pending approvals error");
                return ErrorRedirect("Error loading pending approvals");
            }
        }

        private async Task PopulateLocationDropdowns()
        {
            ViewBag.Divisions = await _locationService.GetDivisionsAsync();
            ViewBag.AreaTypes = await _locationService.GetAreaTypesAsync();
        }
        // GET: /Admin/PendingApprovals
        //public async Task<IActionResult> PendingApprovals()
        //{
        //    try
        //    {
        //        var token = GetAuthToken();

        //        if (string.IsNullOrEmpty(token))
        //        {
        //            _logger.LogWarning("No valid authentication token found. Redirecting to login.");
        //            TempData["ErrorMessage"] = "Your session has expired. Please log in again.";
        //            return RedirectToAction("Login", "Account");
        //        }

        //        var (success, pendingProfiles, errors) = await _adminService.GetPendingRenterProfilesAsync(token);

        //        if (!success)
        //        {
        //            // Check if this is an authentication error
        //            if (errors.Any(e => e.Contains("Unauthorized") || e.Contains("401")))
        //            {
        //                return HandleUnauthorizedResponse(errors, "pending approvals");
        //            }

        //            _logger.LogError("Error retrieving pending renter profiles: {Errors}", string.Join(", ", errors));
        //            TempData["ErrorMessage"] = "An error occurred while retrieving pending renter profiles.";
        //            return RedirectToAction("Index");
        //        }

        //        ViewBag.PendingCount = pendingProfiles.Count;
        //        ViewBag.Title = "Pending Renter Approvals";

        //        return View(pendingProfiles);
        //    }
        //    catch (Exception ex)
        //    {
        //        _logger.LogError(ex, "Error retrieving pending renter profiles");
        //        TempData["ErrorMessage"] = "An error occurred while retrieving pending renter profiles.";
        //        return RedirectToAction("Index");
        //    }
        //}

        // GET: /Admin/PendingOwnerApprovals
        public async Task<IActionResult> PendingOwnerApprovals()
        {
            try
            {
                var token = GetAuthToken();
                
                if (string.IsNullOrEmpty(token))
                {
                    _logger.LogWarning("No valid authentication token found. Redirecting to login.");
                    TempData["ErrorMessage"] = "Your session has expired. Please log in again.";
                    return RedirectToAction("Login", "Account");
                }
                
                var (success, pendingProfiles, errors) = await _adminService.GetPendingOwnerProfilesAsync(token);
                
                if (!success)
                {
                    // Check if this is an authentication error
                    if (errors.Any(e => e.Contains("Unauthorized") || e.Contains("401")))
                    {
                        return HandleUnauthorizedResponse(errors, "pending owner approvals");
                    }
                    
                    _logger.LogError("Error retrieving pending owner profiles: {Errors}", string.Join(", ", errors));
                    TempData["ErrorMessage"] = "An error occurred while retrieving pending owner profiles.";
                    return RedirectToAction("Index");
                }

                ViewBag.PendingCount = pendingProfiles.Count;
                ViewBag.Title = "Pending Apartment Owner Approvals";

                return View(pendingProfiles);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving pending owner profiles");
                TempData["ErrorMessage"] = "An error occurred while retrieving pending owner profiles.";
                return RedirectToAction("Index");
            }
        }

        // GET: /Admin/ApprovalDetails/{id}
        public async Task<IActionResult> ApprovalDetails(int id)
        {
            try
            {
                var token = GetAuthToken();
                if (string.IsNullOrEmpty(token))
                {
                    return RedirectToAction("Login", "Account");
                }
                
                var renterProfileApiService = HttpContext.RequestServices.GetRequiredService<IRenterProfileApiService>();
                var (success, profile, errors) = await renterProfileApiService.GetByIdAsync(id, token);
                
                if (!success || profile == null)
                {
                    _logger.LogError("Error retrieving profile details: {Errors}", string.Join(", ", errors));
                    TempData["ErrorMessage"] = "Profile not found.";
                    return RedirectToAction(nameof(PendingApprovals));
                }

                return View(profile);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving profile details");
                TempData["ErrorMessage"] = "An error occurred while retrieving profile details.";
                return RedirectToAction(nameof(PendingApprovals));
            }
        }

        // POST: /Admin/ApproveProfile/{id}
        //[HttpPost]
        //[ValidateAntiForgeryToken]
        //public async Task<IActionResult> ApproveProfile(int id)
        //{
        //    try
        //    {
        //        var token = GetAuthToken();
        //        if (string.IsNullOrEmpty(token))
        //        {
        //            return RedirectToAction("Login", "Account");
        //        }

        //        var (success, errors) = await _adminService.ApproveRenterProfileAsync(id, true, null, token);

        //        if (!success)
        //        {
        //            _logger.LogError("Error approving renter profile: {Errors}", string.Join(", ", errors));
        //            TempData["ErrorMessage"] = "An error occurred while approving the renter profile.";
        //            return RedirectToAction(nameof(PendingApprovals));
        //        }

        //        TempData["SuccessMessage"] = "Renter profile approved successfully.";
        //        return RedirectToAction(nameof(PendingApprovals));
        //    }
        //    catch (Exception ex)
        //    {
        //        _logger.LogError(ex, "Error approving renter profile");
        //        TempData["ErrorMessage"] = "An error occurred while approving the renter profile.";
        //        return RedirectToAction(nameof(PendingApprovals));
        //    }
        //}

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ApproveProfile(int id, bool isApproved, string reason = null)
        {
            try
            {
                var token = GetAuthToken();
                var filters = await GetAdminLocationFilters();

                if (!await _adminService.VerifyProfileAccess(id, token, filters))
                    return ErrorRedirect("Unauthorized access attempt");

                var result = await _adminService.ProcessApproval(id, isApproved, reason, token, filters);

                return result.Success
                    ? RedirectToAction(nameof(PendingApprovals))
                    : HandleError(result.Errors, "Approval");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Approval error");
                return ErrorRedirect("Approval processing failed");
            }
        }

        private async Task<AdminLocationFilters> GetAdminLocationFilters()
        {
            // Parse the string claim value to integer
            var adminIdClaim = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (!int.TryParse(adminIdClaim, out int adminId))
            {
                _logger.LogError("Invalid admin ID format in claims: {AdminIdClaim}", adminIdClaim);
                return new AdminLocationFilters(); // Return empty filters or throw exception
            }

            return await _adminLocationService.GetAdminFiltersAsync(adminId);
        }

        // POST: /Admin/DeclineProfile/{id}
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeclineProfile(int id, string reason)
        {
            try
            {
                var token = GetAuthToken();
                if (string.IsNullOrEmpty(token))
                {
                    return RedirectToAction("Login", "Account");
                }
                
                var (success, errors) = await _adminService.ApproveRenterProfileAsync(id, false, reason, token);
                
                if (!success)
                {
                    _logger.LogError("Error declining renter profile: {Errors}", string.Join(", ", errors));
                    TempData["ErrorMessage"] = "An error occurred while declining the renter profile.";
                    return RedirectToAction(nameof(PendingApprovals));
                }

                TempData["SuccessMessage"] = "Renter profile declined successfully.";
                return RedirectToAction(nameof(PendingApprovals));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error declining renter profile");
                TempData["ErrorMessage"] = "An error occurred while declining the renter profile.";
                return RedirectToAction(nameof(PendingApprovals));
            }
        }
        
        // POST: /Admin/ApproveOwnerProfile/{id}
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ApproveOwnerProfile(int id)
        {
            try
            {
                var token = GetAuthToken();
                if (string.IsNullOrEmpty(token))
                {
                    return RedirectToAction("Login", "Account");
                }
                
                var (success, errors) = await _adminService.ApproveOwnerProfileAsync(id, true, null, token);
                
                if (!success)
                {
                    _logger.LogError("Error approving owner profile: {Errors}", string.Join(", ", errors));
                    TempData["ErrorMessage"] = "An error occurred while approving the owner profile.";
                    return RedirectToAction(nameof(PendingOwnerApprovals));
                }

                TempData["SuccessMessage"] = "Apartment owner profile approved successfully.";
                return RedirectToAction(nameof(PendingOwnerApprovals));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error approving owner profile");
                TempData["ErrorMessage"] = "An error occurred while approving the owner profile.";
                return RedirectToAction(nameof(PendingOwnerApprovals));
            }
        }
        
        // POST: /Admin/DeclineOwnerProfile/{id}
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeclineOwnerProfile(int id, string reason)
        {
            try
            {
                var token = GetAuthToken();
                if (string.IsNullOrEmpty(token))
                {
                    return RedirectToAction("Login", "Account");
                }
                
                var (success, errors) = await _adminService.ApproveOwnerProfileAsync(id, false, reason, token);
                
                if (!success)
                {
                    _logger.LogError("Error declining owner profile: {Errors}", string.Join(", ", errors));
                    TempData["ErrorMessage"] = "An error occurred while declining the owner profile.";
                    return RedirectToAction(nameof(PendingOwnerApprovals));
                }

                TempData["SuccessMessage"] = "Apartment owner profile declined successfully.";
                return RedirectToAction(nameof(PendingOwnerApprovals));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error declining owner profile");
                TempData["ErrorMessage"] = "An error occurred while declining the owner profile.";
                return RedirectToAction(nameof(PendingOwnerApprovals));
            }
        }
        
        // GET: /Admin/UserAccounts
        public async Task<IActionResult> UserAccounts()
        {
            try
            {
                var token = GetAuthToken();
                if (string.IsNullOrEmpty(token))
                {
                    return RedirectToAction("Login", "Account");
                }
                
                var (success, users, errors) = await _adminService.GetUsersAsync(token);
                
                if (!success)
                {
                    _logger.LogError("Error retrieving users: {Errors}", string.Join(", ", errors));
                    TempData["ErrorMessage"] = "An error occurred while retrieving user accounts.";
                    return RedirectToAction(nameof(Index));
                }

                return View(users);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving user accounts");
                TempData["ErrorMessage"] = "An error occurred while retrieving user accounts.";
                return RedirectToAction(nameof(Index));
            }
        }
        
        // POST: /Admin/DeleteUser/{id}
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteUser(int id)
        {
            try
            {
                var token = GetAuthToken();
                if (string.IsNullOrEmpty(token))
                {
                    return RedirectToAction("Login", "Account");
                }
                
                var (success, errors) = await _adminService.DeleteUserAsync(id, token);
                
                if (!success)
                {
                    _logger.LogError("Error deleting user: {Errors}", string.Join(", ", errors));
                    TempData["ErrorMessage"] = "An error occurred while deleting the user.";
                    return RedirectToAction(nameof(UserAccounts));
                }

                TempData["SuccessMessage"] = "User deleted successfully.";
                return RedirectToAction(nameof(UserAccounts));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting user");
                TempData["ErrorMessage"] = "An error occurred while deleting the user.";
                return RedirectToAction(nameof(UserAccounts));
            }
        }

        // Helper method to get the authentication token
        private string GetAuthToken()
        {
            //// Get the token from session first
            //var token = HttpContext.Session.GetString("AuthToken");

            //// Token debugging
            //if (!string.IsNullOrEmpty(token))
            //{
            //    _logger.LogInformation("Found token in session: {TokenLength} characters", token.Length);
            //}

            //// If token is not in session, try to get it from claims
            //if (string.IsNullOrEmpty(token))
            //{
            //    _logger.LogWarning("Token not found in session, checking claims");
            //    var tokenClaim = User.FindFirst("Token");
            //    if (tokenClaim != null)
            //    {
            //        token = tokenClaim.Value;
            //        _logger.LogInformation("Found token in claims: {TokenLength} characters", token.Length);

            //        // Store it back in session
            //        HttpContext.Session.SetString("AuthToken", token);
            //        _logger.LogInformation("Retrieved token from claims and stored in session");
            //    }
            //    else
            //    {
            //        _logger.LogWarning("Token not found in claims either");
            //    }
            //}

            //if (!string.IsNullOrEmpty(token))
            //{
            //    // Basic validation check for token format
            //    if (!token.Contains('.') || token.Count(c => c == '.') != 2)
            //    {
            //        _logger.LogWarning("Token does not appear to be a valid JWT format");
            //    }
            //}

            //return token;
            return HttpContext.Session.GetString("AuthToken")
                ?? User.FindFirst("Token")?.Value
                ?? throw new UnauthorizedAccessException();
        }

        // Helper method to handle unauthorized responses
        private IActionResult HandleUnauthorizedResponse(string[] errors, string action)
        {
            _logger.LogWarning("Authentication error when accessing {Action}: {Errors}", action, string.Join(", ", errors));
            TempData["ErrorMessage"] = "Your session has expired. Please log in again.";
            // Clear the invalid token
            HttpContext.Session.Remove("AuthToken");
            return RedirectToAction("Login", "Account");
        }

        private IActionResult HandleError(string[] errors, string context)
        {
            _logger.LogError("{Context} errors: {Errors}", context, string.Join(", ", errors));
            TempData["ErrorMessage"] = string.Join(", ", errors);
            return RedirectToAction(nameof(Dashboard));
        }

        private IActionResult ErrorRedirect(string message)
        {
            TempData["ErrorMessage"] = message;
            return RedirectToAction(nameof(Dashboard));
        }

    }
} 