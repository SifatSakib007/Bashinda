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

namespace Bashinda.Controllers
{
    [Authorize(Roles = "ApartmentRenter")]
    public class RenterController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<RenterController> _logger;
        private readonly IRenterProfileService _renterProfileService;

        public RenterController(ApplicationDbContext context, ILogger<RenterController> logger, IRenterProfileService renterProfileService)
        {
            _context = context;
            _logger = logger;
            _renterProfileService = renterProfileService;
        }

        // Action for the first dummy page
        public IActionResult DummyPage1()
        {
            if (!IsProfileSubmitted())
            {
                ViewBag.Message = "Please fill out the profile infos first.";
                return View("ProfileIncomplete");
            }
            
            if (!IsApproved())
            {
                ViewBag.Message = "You can see the content on this page when you get approved.";
                return View("NotApproved");
            }
            
            ViewBag.WelcomeMessage = "Welcome to the exclusive content for approved renters!";
            return View();
        }

        // Action for the second dummy page
        public IActionResult DummyPage2()
        {
            if (!IsProfileSubmitted())
            {
                ViewBag.Message = "Please fill out the profile infos first.";
                return View("ProfileIncomplete");
            }
            
            if (!IsApproved())
            {
                ViewBag.Message = "You can see the content on this page when you get approved.";
                return View("NotApproved");
            }
            
            ViewBag.WelcomeMessage = "Welcome to our exclusive second page for approved renters!";
            return View();
        }

        // Check if the profile is submitted
        private bool IsProfileSubmitted()
        {
            if (!User.Identity.IsAuthenticated)
                return false;

            var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));
            var renterProfile = _context.RenterProfiles.FirstOrDefault(p => p.UserId == userId);
            
            return renterProfile != null;
        }

        // Check if the renter is approved
        private bool IsApproved()
        {
            if (!User.Identity.IsAuthenticated)
                return false;

            var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));
            var renterProfile = _context.RenterProfiles
                .FirstOrDefault(p => p.UserId == userId);
            
            return renterProfile != null && renterProfile.IsApproved;
        }

        public async Task<IActionResult> Index()
        {
            try
            {
                var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
                if (userIdClaim == null)
                {
                    _logger.LogWarning("User ID claim not found");
                    return RedirectToAction("Login", "Account");
                }

                if (!int.TryParse(userIdClaim.Value, out int userId))
                {
                    _logger.LogWarning("Invalid user ID format: {UserId}", userIdClaim.Value);
                    return RedirectToAction("Login", "Account");
                }

                var response = await _renterProfileService.GetRenterProfileAsync(userId);
                if (!response.Success)
                {
                    TempData["ErrorMessage"] = response.Errors.FirstOrDefault() ?? "Failed to load profile";
                    return View(new RenterProfileViewModel());
                }

                return View(response.Data);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error loading renter profile");
                TempData["ErrorMessage"] = "An error occurred while loading your profile";
                return View(new RenterProfileViewModel());
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
                var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
                if (userIdClaim == null)
                {
                    _logger.LogWarning("User ID claim not found");
                    return RedirectToAction("Login", "Account");
                }

                if (!int.TryParse(userIdClaim.Value, out int userId))
                {
                    _logger.LogWarning("Invalid user ID format: {UserId}", userIdClaim.Value);
                    return RedirectToAction("Login", "Account");
                }

                var response = await _renterProfileService.CreateRenterProfileAsync(userId, model);
                if (!response.Success)
                {
                    ModelState.AddModelError("", response.Errors.FirstOrDefault() ?? "Failed to create profile");
                    return View(model);
                }

                TempData["SuccessMessage"] = "Profile created successfully";
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating renter profile");
                ModelState.AddModelError("", "An error occurred while creating your profile");
                return View(model);
            }
        }
    }
} 