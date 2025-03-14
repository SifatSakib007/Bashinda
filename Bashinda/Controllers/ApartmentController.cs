using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Bashinda.Services;
using Bashinda.ViewModels;
using System.Security.Claims;

namespace Bashinda.Controllers
{
    public class ApartmentController : Controller
    {
        private readonly IApartmentApiService _apartmentService;
        private readonly IRenterProfileApiService _renterService;
        private readonly ILogger<ApartmentController> _logger;

        public ApartmentController(
            IApartmentApiService apartmentService,
            IRenterProfileApiService renterService,
            ILogger<ApartmentController> logger)
        {
            _apartmentService = apartmentService;
            _renterService = renterService;
            _logger = logger;
        }

        [HttpGet]
        [Authorize(Roles = "ApartmentOwner,Admin")]
        public async Task<IActionResult> RentersList()
        {
            var token = HttpContext.Session.GetString("AuthToken");
            if (string.IsNullOrEmpty(token))
            {
                return RedirectToAction("Login", "Account");
            }

            var (success, renters, errors) = await _renterService.GetAllAsync(token);
            
            if (!success)
            {
                _logger.LogError("Failed to get renters: {Errors}", string.Join(", ", errors));
                TempData["ErrorMessage"] = "Failed to retrieve renter information.";
                return View(new List<RenterProfileListDto>());
            }

            // Only show approved renters
            var approvedRenters = renters.Where(r => r.IsApproved).ToList();
            
            return View(approvedRenters);
        }

        [HttpGet]
        [Authorize(Roles = "ApartmentOwner,Admin")]
        public async Task<IActionResult> RenterDetails(int id)
        {
            var token = HttpContext.Session.GetString("AuthToken");
            if (string.IsNullOrEmpty(token))
            {
                return RedirectToAction("Login", "Account");
            }

            var (success, renter, errors) = await _renterService.GetByIdAsync(id, token);
            
            if (!success || renter == null)
            {
                _logger.LogError("Failed to get renter details: {Errors}", string.Join(", ", errors));
                TempData["ErrorMessage"] = "Failed to retrieve renter details.";
                return RedirectToAction("RentersList");
            }

            // Only allow viewing approved renters
            if (!renter.IsApproved)
            {
                TempData["ErrorMessage"] = "This renter profile has not been approved yet.";
                return RedirectToAction("RentersList");
            }
            
            return View(renter);
        }
        
        [HttpGet]
        [Authorize(Roles = "ApartmentOwner")]
        public async Task<IActionResult> MyApartments()
        {
            var token = HttpContext.Session.GetString("AuthToken");
            if (string.IsNullOrEmpty(token))
            {
                return RedirectToAction("Login", "Account");
            }

            var (success, apartments, errors) = await _apartmentService.GetOwnerApartmentsAsync(token);
            
            if (!success)
            {
                _logger.LogError("Failed to get owner apartments: {Errors}", string.Join(", ", errors));
                TempData["ErrorMessage"] = "Failed to retrieve your apartments.";
                return View(new List<ApartmentListDto>());
            }
            
            return View(apartments);
        }
        
        [HttpGet]
        [Authorize(Roles = "ApartmentOwner")]
        public IActionResult CreateApartment()
        {
            return View();
        }
        
        [HttpPost]
        [Authorize(Roles = "ApartmentOwner")]
        public async Task<IActionResult> CreateApartment(CreateApartmentDto model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }
            
            var token = HttpContext.Session.GetString("AuthToken");
            if (string.IsNullOrEmpty(token))
            {
                return RedirectToAction("Login", "Account");
            }

            var (success, apartment, errors) = await _apartmentService.CreateAsync(model, token);
            
            if (!success)
            {
                _logger.LogError("Failed to create apartment: {Errors}", string.Join(", ", errors));
                ModelState.AddModelError("", string.Join(", ", errors));
                return View(model);
            }

            TempData["SuccessMessage"] = "Apartment created successfully.";
            return RedirectToAction("MyApartments");
        }
        
        [HttpGet]
        [Authorize(Roles = "ApartmentOwner")]
        public async Task<IActionResult> EditApartment(int id)
        {
            var token = HttpContext.Session.GetString("AuthToken");
            if (string.IsNullOrEmpty(token))
            {
                return RedirectToAction("Login", "Account");
            }

            var (success, apartment, errors) = await _apartmentService.GetByIdAsync(id, token);
            
            if (!success || apartment == null)
            {
                _logger.LogError("Failed to get apartment for editing: {Errors}", string.Join(", ", errors));
                TempData["ErrorMessage"] = "Failed to retrieve apartment details for editing.";
                return RedirectToAction("MyApartments");
            }
            
            // Map to the CreateApartmentDto for editing
            var model = new CreateApartmentDto
            {
                BuildingName = apartment.BuildingName,
                Address = apartment.Address,
                ApartmentNumber = apartment.ApartmentNumber,
                NumberOfRooms = apartment.NumberOfRooms,
                NumberOfBathrooms = apartment.NumberOfBathrooms,
                SquareFeet = apartment.SquareFeet,
                MonthlyRent = apartment.MonthlyRent,
                Description = apartment.Description
            };
            
            return View(model);
        }
        
        [HttpPost]
        [Authorize(Roles = "ApartmentOwner")]
        public async Task<IActionResult> EditApartment(int id, CreateApartmentDto model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }
            
            var token = HttpContext.Session.GetString("AuthToken");
            if (string.IsNullOrEmpty(token))
            {
                return RedirectToAction("Login", "Account");
            }

            var (success, errors) = await _apartmentService.UpdateAsync(id, model, token);
            
            if (!success)
            {
                _logger.LogError("Failed to update apartment: {Errors}", string.Join(", ", errors));
                ModelState.AddModelError("", string.Join(", ", errors));
                return View(model);
            }

            TempData["SuccessMessage"] = "Apartment updated successfully.";
            return RedirectToAction("MyApartments");
        }
        
        [HttpPost]
        [Authorize(Roles = "ApartmentOwner")]
        public async Task<IActionResult> DeleteApartment(int id)
        {
            var token = HttpContext.Session.GetString("AuthToken");
            if (string.IsNullOrEmpty(token))
            {
                return RedirectToAction("Login", "Account");
            }

            var (success, errors) = await _apartmentService.DeleteAsync(id, token);
            
            if (!success)
            {
                _logger.LogError("Failed to delete apartment: {Errors}", string.Join(", ", errors));
                TempData["ErrorMessage"] = "Failed to delete apartment.";
            }
            else
            {
                TempData["SuccessMessage"] = "Apartment deleted successfully.";
            }
            
            return RedirectToAction("MyApartments");
        }
        
        [HttpPost]
        [Authorize(Roles = "ApartmentOwner")]
        public async Task<IActionResult> UploadImage(int id, IFormFile file)
        {
            if (file == null || file.Length == 0)
            {
                TempData["ErrorMessage"] = "Please select a file to upload.";
                return RedirectToAction("EditApartment", new { id });
            }
            
            var token = HttpContext.Session.GetString("AuthToken");
            if (string.IsNullOrEmpty(token))
            {
                return RedirectToAction("Login", "Account");
            }

            var (success, imagePath, errors) = await _apartmentService.UploadImageAsync(id, file, token);
            
            if (!success)
            {
                _logger.LogError("Failed to upload apartment image: {Errors}", string.Join(", ", errors));
                TempData["ErrorMessage"] = "Failed to upload image.";
            }
            else
            {
                TempData["SuccessMessage"] = "Image uploaded successfully.";
            }
            
            return RedirectToAction("EditApartment", new { id });
        }
    }
} 