using System.Diagnostics;
using Bashinda.Models;
using Bashinda.Services;
using Bashinda.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Bashinda.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly IApartmentApiService _apartmentService;

        public HomeController(
            ILogger<HomeController> logger,
            IApartmentApiService apartmentService)
        {
            _logger = logger;
            _apartmentService = apartmentService;
        }

        public async Task<IActionResult> Index()
        {
            // Redirect unauthenticated users to Login
            if (!User.Identity.IsAuthenticated)
            {
                return RedirectToAction("Login", "Account");
            }

            // Redirect authenticated users to Dashboard
            return RedirectToAction(nameof(Dashboard));
        }

        [Authorize]
        public IActionResult Dashboard()
        {
            // Get user information from claims
            var userId = User.FindFirst("UserId")?.Value;
            var userRole = User.FindFirst(System.Security.Claims.ClaimTypes.Role)?.Value;

            // Set ViewBag data to use in the view
            ViewBag.UserId = userId;
            ViewBag.UserRole = userRole;
            ViewBag.Message = TempData["Message"]; // Display any messages from redirects
            
            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
        
        public async Task<IActionResult> ApartmentDetails(int id)
        {
            var (success, apartment, errors) = await _apartmentService.GetByIdAsync(id);
            
            if (!success || apartment == null)
            {
                _logger.LogError("Failed to get apartment details: {Errors}", string.Join(", ", errors));
                return NotFound();
            }
            
            return View(apartment);
        }
    }
}
