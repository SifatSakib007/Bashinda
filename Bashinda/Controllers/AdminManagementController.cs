using Bashinda.Models;
using Bashinda.Services;
using Bashinda.ViewModels;
using BashindaAPI.DTOs;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using AdminPermissionDto = BashindaAPI.DTOs.AdminPermissionDto;

namespace Bashinda.Controllers
{
    [Authorize(Roles = "SuperAdmin")]
    [Authorize(AuthenticationSchemes = CookieAuthenticationDefaults.AuthenticationScheme)]
    public class AdminManagementController : Controller
    {
        private readonly IAdminApiService _adminApiService;
        private readonly ILocationDataService _locationDataService;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly ILogger<AdminManagementController> _logger;

        public AdminManagementController(
            IAdminApiService adminApiService,
            ILocationDataService locationDataService,
            IHttpContextAccessor httpContextAccessor,
            ILogger<AdminManagementController> logger)
        {
            _adminApiService = adminApiService;
            _locationDataService = locationDataService;
            _httpContextAccessor = httpContextAccessor;
            _logger = logger;
        }

        private async Task<string> GetAccessToken()
        {
            // Check multiple token locations
            var token = await HttpContext.GetTokenAsync("access_token")
                        ?? HttpContext.Session.GetString("JwtToken")
                        ?? Request.Cookies["JwtToken"];

            if (string.IsNullOrEmpty(token))
            {
                _logger.LogWarning("JWT token not found in any storage location");
                throw new UnauthorizedAccessException("Authentication required");
            }
            return token;
        }

        // GET: AdminManagement
        public async Task<IActionResult> Index()
        {
            try
            {
                // Get token from cookies/session
                var token = await GetAccessToken();

                var (success, admins, errors) = await _adminApiService.GetAllAdminsAsync(token);
                if (!success)
                {
                    TempData["ErrorMessage"] = errors.FirstOrDefault() ?? "Failed to load.";
                    return NotFound();
                }
                return View(admins);

            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in AllRenters action");
                TempData["ErrorMessage"] = "An error occurred while loading renters.";
                return NotFound();
            }
        }
        public async Task<IActionResult> AllRenters()
        {
            try
            {
                var token = await GetAccessToken(); 
                var (success, renters, errors) = await _adminApiService.GetAllRentersAsync(token);
                if (!success)
                {
                    TempData["ErrorMessage"] = errors.FirstOrDefault() ?? "Failed to load renters.";
                    return View(new List<RenterProfileViewModel>());
                }

                // Map the DTOs to your view models
                var viewModels = renters.Select(dto => new RenterProfileViewModel
                {
                    Id = dto.Id,
                    UserId = dto.UserId,
                    IsAdult = dto.IsAdult,
                    FullName = dto.FullName,
                    Email = dto.Email,
                    MobileNo = dto.MobileNo,
                    Nationality = dto.Nationality,
                    BloodGroup = dto.BloodGroup,
                    Profession = dto.Profession,
                    Gender = dto.Gender,
                    DateOfBirth = dto.DateOfBirth,
                    IsApproved = dto.IsApproved,
                    Division = dto.Division,
                    District = dto.District,
                    Upazila = dto.Upazila,
                }).ToList();

                return View(viewModels);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in AllRenters action");
                TempData["ErrorMessage"] = "An error occurred while loading renters.";
                return View(new List<RenterProfileViewModel>());
            }
        }
        public async Task<IActionResult> AllOwners()
        {
            try
            {
                var token = await GetAccessToken(); 
                var (success, owners, errors) = await _adminApiService.GetAllOwnersAsync(token);
                if (!success)
                {
                    TempData["ErrorMessage"] = errors.FirstOrDefault() ?? "Failed to load renters.";
                    return View(new List<OwnerProfileListDto>());
                }

                // Map the DTOs to your view models
                var viewModels = owners.Select(dto => new OwnerProfileListDto
                {
                    Id = dto.Id,
                    UserId = dto.UserId,
                    IsAdult = dto.IsAdult,
                    FullName = dto.FullName,
                    Email = dto.Email,
                    MobileNo = dto.MobileNo,
                    Nationality = dto.Nationality,
                    BloodGroup = dto.BloodGroup,
                    Profession = dto.Profession,
                    Gender = dto.Gender,
                    DateOfBirth = dto.DateOfBirth,
                    SelfImagePath = dto.SelfImagePath,
                    IsApproved = dto.IsApproved,
                    Division = dto.Division,
                    District = dto.District,
                    Upazila = dto.Upazila,
                    RejectionReason = dto.RejectionReason
                }).ToList();

                return View(viewModels);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in AllRenters action");
                TempData["ErrorMessage"] = "An error occurred while loading renters.";
                return View(new List<OwnerProfileListDto>());
            }
        }


        // GET: AdminManagement/Details/5
        //[HttpGet]
        //public async Task<IActionResult> Details(string id)
        //{
        //    if (string.IsNullOrEmpty(id))
        //    {
        //        return NotFound();
        //    }

        //    // Retrieve the access token from the current HTTP context.
        //    var token = await HttpContext.GetTokenAsync("access_token");

        //    // Call API to get admin details
        //    var adminResponse = await _adminApiService.GetAdminByIdAsync(id, token);
        //    if (!adminResponse.Success || adminResponse.Data == null)
        //    {
        //        return NotFound();
        //    }

        //    // Since the API already returns AdminPermissionViewModel, assign it directly.
        //    var admin = new AdminViewModel
        //    {
        //        Id = adminResponse.Data.Id.ToString(),
        //        UserName = adminResponse.Data.UserName,
        //        Email = adminResponse.Data.Email,
        //        PhoneNumber = adminResponse.Data.PhoneNumber,
        //        CreatedDate = adminResponse.Data.CreatedDate,
        //        Permissions = adminResponse.Data.Permissions ?? new AdminPermissionViewModel()
        //    };

        //    // Update SetLocationNames to accept AdminPermissionViewModel
        //    await SetLocationNames(admin, adminResponse.Data.Permissions);

        //    return View(admin);
        //}


        // GET: AdminManagement/Create
        public async Task<IActionResult> Create()
        {
            var model = new CreateAdminViewModel
            {
                Divisions = await GetDivisionsSelectList()
            };

            return View(model);
        }

        // POST: AdminManagement/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(CreateAdminViewModel model)
        {
            try
            {
                _logger.LogInformation("Attempting to create new admin");

                if (!ModelState.IsValid)
                {
                    _logger.LogWarning("Model state invalid");
                    await RepopulateLocationLists(model);
                    return View(model);
                }
                var token = await GetAccessToken();
                    _logger.LogInformation("Using token: {TokenPrefix}...", token[..10]);
                    if (string.IsNullOrEmpty(token))
                    {
                        _logger.LogWarning("No access token found");
                        return Unauthorized();
                    }

                    // Map the view model to the API DTO
                    var createAdminDto = new CreateAdminDto
                {
                    UserName = model.UserName,
                    Email = model.Email,
                    PhoneNumber = model.PhoneNumber,
                    Password = model.Password,
                        ConfirmPassword = model.ConfirmPassword,
                        Permissions = new AdminPermissionDto
                        {
                        Division = model.Permissions.Division,
                        District = model.Permissions.District,
                        Upazila = model.Permissions.Upazila,
                        Ward = model.Permissions.Ward,
                        Village = model.Permissions.Village,
                        CanViewUserName = model.Permissions.CanViewUserName,
                        CanViewEmail = model.Permissions.CanViewEmail,
                        CanViewPhone = model.Permissions.CanViewPhone,
                        CanViewAddress = model.Permissions.CanViewAddress,
                        CanViewProfileImage = model.Permissions.CanViewProfileImage,
                        CanViewNationalId = model.Permissions.CanViewNationalId,
                        CanViewBirthRegistration = model.Permissions.CanViewBirthRegistration,
                        CanViewDateOfBirth = model.Permissions.CanViewDateOfBirth,
                        CanViewFamilyInfo = model.Permissions.CanViewFamilyInfo,
                        CanViewProfession = model.Permissions.CanViewProfession,
                        CanApproveRenters = model.Permissions.CanApproveRenters,
                        CanApproveOwners = model.Permissions.CanApproveOwners,
                        CanManageApartments = model.Permissions.CanManageApartments
                    }
                };

                    _logger.LogInformation("Calling API service");
                    var result = await _adminApiService.CreateAdminAsync(createAdminDto, token);

                    if (result.Success)
                    {
                        _logger.LogInformation("Admin created successfully");
                        return RedirectToAction(nameof(Index));
                    }

                    _logger.LogError("API errors: {Errors}", string.Join(", ", result.Errors));
                    ModelState.AddModelError("", string.Join(", ", result.Errors));
                }
    catch (UnauthorizedAccessException ex)
            {
                _logger.LogWarning(ex, "Authentication failure");
                return RedirectToAction("Login", "Account");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Admin creation error");
                ModelState.AddModelError("", "An unexpected error occurred");
            }

            await RepopulateLocationLists(model);
            return View(model);
        }

        private async Task RepopulateLocationLists(CreateAdminViewModel model)
        {
            model.Divisions = await GetDivisionsSelectList();

            if (!string.IsNullOrEmpty(model.Permissions.Division))
            {
                model.Districts = await GetDistrictsSelectList(model.Permissions.Division);
                if (!string.IsNullOrEmpty(model.Permissions.District))
                {
                    model.Upazilas = await GetUpazilasSelectList(model.Permissions.District);
                    if (!string.IsNullOrEmpty(model.Permissions.Upazila))
                    {
                        model.Wards = await GetWardsSelectList(model.Permissions.Upazila);
                        if (!string.IsNullOrEmpty(model.Permissions.Ward))
                        {
                            model.Villages = await GetVillagesSelectList(model.Permissions.Ward);
                        }
                    }
                }
            }
        }


        // GET: AdminManagement/Edit/5
        //public async Task<IActionResult> Edit(string id)
        //{
        //    if (id == null)
        //    {
        //        return NotFound();
        //    }

        //    var user = await _userManager.FindByIdAsync(id);
        //    if (user == null)
        //    {
        //        return NotFound();
        //    }

        //    var permissions = await _context.AdminPermissions
        //        .FirstOrDefaultAsync(p => p.UserId.ToString() == id);

        //    if (permissions == null)
        //    {
        //        permissions = new AdminPermission
        //        {
        //            UserId = id,
        //            CreatedDate = DateTime.UtcNow,
        //            CreatedBy = User.Identity.Name
        //        };
        //    }

        //    var model = new UpdateAdminViewModel
        //    {
        //        Id = user.Id,
        //        UserName = user.UserName,
        //        Email = user.Email,
        //        PhoneNumber = user.PhoneNumber,
        //        Permissions = MapToPermissionViewModel(permissions),
        //        Divisions = await GetDivisionsSelectList()
        //    };

        //    if (!string.IsNullOrEmpty(permissions.Division))
        //    {
        //        model.Districts = await GetDistrictsSelectList(permissions.Division);
        //    }

        //    if (!string.IsNullOrEmpty(permissions.District))
        //    {
        //        model.Upazilas = await GetUpazilasSelectList(permissions.District);
        //    }

        //    if (!string.IsNullOrEmpty(permissions.Upazila))
        //    {
        //        model.Wards = await GetWardsSelectList(permissions.Upazila);
        //    }

        //    if (!string.IsNullOrEmpty(permissions.Ward))
        //    {
        //        model.Villages = await GetVillagesSelectList(permissions.Ward);
        //    }

        //    return View(model);
        //}

        // POST: AdminManagement/Edit/5
        //[HttpPost]
        //[ValidateAntiForgeryToken]
        //public async Task<IActionResult> Edit(string id, UpdateAdminViewModel model)
        //{
        //    if (id != model.Id)
        //    {
        //        return NotFound();
        //    }

        //    if (ModelState.IsValid)
        //    {
        //        var permissions = await _context.AdminPermissions
        //            .FirstOrDefaultAsync(p => p.UserId.ToString() == id);

        //        if (permissions == null)
        //        {
        //            // Create new permissions if they don't exist
        //            permissions = new AdminPermission
        //            {
        //                UserId = id,
        //                CreatedDate = DateTime.UtcNow,
        //                CreatedBy = User.Identity.Name
        //            };
        //            _context.AdminPermissions.Add(permissions);
        //        }

        //        // Update permissions
        //        permissions.Division = model.Permissions.Division;
        //        permissions.District = model.Permissions.District;
        //        permissions.Upazila = model.Permissions.Upazila;
        //        permissions.Ward = model.Permissions.Ward;
        //        permissions.Village = model.Permissions.Village;
        //        permissions.CanViewUserName = model.Permissions.CanViewUserName;
        //        permissions.CanViewEmail = model.Permissions.CanViewEmail;
        //        permissions.CanViewPhone = model.Permissions.CanViewPhone;
        //        permissions.CanViewAddress = model.Permissions.CanViewAddress;
        //        permissions.CanViewProfileImage = model.Permissions.CanViewProfileImage;
        //        permissions.CanViewNationalId = model.Permissions.CanViewNationalId;
        //        permissions.CanViewBirthRegistration = model.Permissions.CanViewBirthRegistration;
        //        permissions.CanViewDateOfBirth = model.Permissions.CanViewDateOfBirth;
        //        permissions.CanViewFamilyInfo = model.Permissions.CanViewFamilyInfo;
        //        permissions.CanViewProfession = model.Permissions.CanViewProfession;
        //        permissions.CanApproveRenters = model.Permissions.CanApproveRenters;
        //        permissions.CanApproveOwners = model.Permissions.CanApproveOwners;
        //        permissions.CanManageApartments = model.Permissions.CanManageApartments;
        //        permissions.UpdatedDate = DateTime.UtcNow;
        //        permissions.UpdatedBy = User.Identity.Name;

        //        try
        //        {
        //            await _context.SaveChangesAsync();
        //            return RedirectToAction(nameof(Index));
        //        }
        //        catch (DbUpdateConcurrencyException)
        //        {
        //            if (!AdminExists(id))
        //            {
        //                return NotFound();
        //            }
        //            else
        //            {
        //                throw;
        //            }
        //        }
        //    }

        //    // If we got this far, something failed, redisplay form
        //    model.Divisions = await GetDivisionsSelectList();

        //    if (!string.IsNullOrEmpty(model.Permissions.Division))
        //    {
        //        model.Districts = await GetDistrictsSelectList(model.Permissions.Division);
        //    }

        //    if (!string.IsNullOrEmpty(model.Permissions.District))
        //    {
        //        model.Upazilas = await GetUpazilasSelectList(model.Permissions.District);
        //    }

        //    if (!string.IsNullOrEmpty(model.Permissions.Upazila))
        //    {
        //        model.Wards = await GetWardsSelectList(model.Permissions.Upazila);
        //    }

        //    if (!string.IsNullOrEmpty(model.Permissions.Ward))
        //    {
        //        model.Villages = await GetVillagesSelectList(model.Permissions.Ward);
        //    }

        //    return View(model);
        //}

        // GET: AdminManagement/Delete/5
        //public async Task<IActionResult> Delete(string id)
        //{
        //    if (id == null)
        //    {
        //        return NotFound();
        //    }

        //    var user = await _userManager.FindByIdAsync(id);
        //    if (user == null)
        //    {
        //        return NotFound();
        //    }

        //    var permissions = await _context.AdminPermissions
        //        .FirstOrDefaultAsync(p => p.UserId.ToString() == id);

        //    var admin = new AdminViewModel
        //    {
        //        Id = user.Id,
        //        UserName = user.UserName,
        //        Email = user.Email,
        //        PhoneNumber = user.PhoneNumber,
        //        CreatedDate = user.CreatedDate,
        //        Permissions = permissions != null ? MapToPermissionViewModel(permissions) : new AdminPermissionViewModel()
        //    };

        //    // Set location names
        //    if (permissions != null)
        //    {
        //        await SetLocationNames(admin, permissions);
        //    }

        //    return View(admin);
        //}

        // POST: AdminManagement/Delete/5
        //[HttpPost, ActionName("Delete")]
        //[ValidateAntiForgeryToken]
        //public async Task<IActionResult> DeleteConfirmed(string id)
        //{
        //    var user = await _userManager.FindByIdAsync(id);
        //    if (user == null)
        //    {
        //        return NotFound();
        //    }

        //    // Remove admin permissions
        //    var permissions = await _context.AdminPermissions
        //        .FirstOrDefaultAsync(p => p.UserId.ToString() == id);
        //    if (permissions != null)
        //    {
        //        _context.AdminPermissions.Remove(permissions);
        //    }

        //    // Remove from Admin role
        //    await _userManager.RemoveFromRoleAsync(user, "Admin");

        //    // Delete user
        //    await _userManager.DeleteAsync(user);

        //    await _context.SaveChangesAsync();
        //    return RedirectToAction(nameof(Index));
        //}

        #region Helper Methods

        //private bool AdminExists(string id)
        //{
        //    return _context.Users.Any(e => e.Id.ToString() == id);
        //}

        private async Task<IEnumerable<SelectListItem>> GetDivisionsSelectList()
        {
            var divisions = await _locationDataService.GetDivisionsAsync();
            return divisions.Select(d => new SelectListItem { Value = d, Text = d })
                            .Prepend(new SelectListItem { Value = "", Text = "Select Division" });
        }

        private async Task<IEnumerable<SelectListItem>> GetDistrictsSelectList(string division)
        {
            var districts = await _locationDataService.GetDistrictsAsync(division);
            return districts.Select(d => new SelectListItem { Value = d, Text = d })
                            .Prepend(new SelectListItem { Value = "", Text = "Select District" });
        }

        private async Task<IEnumerable<SelectListItem>> GetUpazilasSelectList(string district)
        {
            var upazilas = await _locationDataService.GetUpazilasAsync(district);
            return upazilas.Select(u => new SelectListItem { Value = u, Text = u })
                           .Prepend(new SelectListItem { Value = "", Text = "Select Upazila" });
        }

        private async Task<IEnumerable<SelectListItem>> GetWardsSelectList(string upazila)
        {
            // Get area type from your service if needed
            var areaType = await _locationDataService.GetAreaTypeAsync(upazila);
            var wards = await _locationDataService.GetWardsAsync(upazila, areaType);
            return wards.Select(w => new SelectListItem { Value = w, Text = w })
                        .Prepend(new SelectListItem { Value = "", Text = "Select Ward" });
        }

        private async Task<IEnumerable<SelectListItem>> GetVillagesSelectList(string ward)
        {
            var villages = await _locationDataService.GetVillagesAsync(ward);
            return villages.Select(v => new SelectListItem { Value = v, Text = v })
                           .Prepend(new SelectListItem { Value = "", Text = "Select Village" });
        }

        #endregion

        #region AJAX Endpoints for Cascading Dropdowns

        public async Task<JsonResult> GetDistrictsByDivision(string division)
        {
            var districts = await _locationDataService.GetDistrictsAsync(division);
            return Json(districts.Select(d => new { Value = d, Text = d }));
        }

        public async Task<JsonResult> GetUpazilasByDistrict(string district)
        {
            var upazilas = await _locationDataService.GetUpazilasAsync(district);
            return Json(upazilas.Select(u => new { Value = u, Text = u }));
        }

        public async Task<JsonResult> GetWardsByUpazila(string upazila)
        {
            var areaType = await _locationDataService.GetAreaTypeAsync(upazila);
            var wards = await _locationDataService.GetWardsAsync(upazila, areaType);
            return Json(wards.Select(w => new { Value = w, Text = w }));
        }

        public async Task<JsonResult> GetVillagesByWard(string ward)
        {
            var villages = await _locationDataService.GetVillagesAsync(ward);
            return Json(villages.Select(v => new { Value = v, Text = v }));
        }

        #endregion

        // Helper method to map AdminPermission DTO to ViewModel
        private AdminPermissionViewModel MapToPermissionViewModel(ViewModels.AdminPermissionDto dto)
        {
            return new AdminPermissionViewModel
            {
                // Location identifiers (could be names or codes depending on API)
                Division = dto.Division,
                District = dto.District,
                Upazila = dto.Upazila,
                Ward = dto.Ward,
                Village = dto.Village,

                // Permission flags
                CanViewUserName = dto.CanViewUserName,
                CanViewEmail = dto.CanViewEmail,
                CanViewPhone = dto.CanViewPhone,
                CanViewAddress = dto.CanViewAddress,
                CanViewProfileImage = dto.CanViewProfileImage,
                CanViewNationalId = dto.CanViewNationalId,
                CanViewBirthRegistration = dto.CanViewBirthRegistration,
                CanViewDateOfBirth = dto.CanViewDateOfBirth,
                CanViewFamilyInfo = dto.CanViewFamilyInfo,
                CanViewProfession = dto.CanViewProfession,
                CanApproveRenters = dto.CanApproveRenters,
                CanApproveOwners = dto.CanApproveOwners,
                CanManageApartments = dto.CanManageApartments
            };
        }

        // Helper method to set display names using ILocationDataService
        private async Task SetLocationNames(AdminViewModel admin, AdminPermissionViewModel permissions)
        {
            // If using location codes, get names from ILocationDataService
            admin.DivisionName = await GetLocationNameAsync(permissions.Division, _locationDataService.GetDivisionsAsync);
            admin.DistrictName = await GetLocationNameAsync(permissions.District,
                () => _locationDataService.GetDistrictsAsync(permissions.Division));
            admin.UpazilaName = await GetLocationNameAsync(permissions.Upazila,
                () => _locationDataService.GetUpazilasAsync(permissions.District));
            admin.WardName = await GetLocationNameAsync(permissions.Ward,
                () => _locationDataService.GetWardsAsync(permissions.Upazila, AreaType.CityCorporation)); // Adjust AreaType as needed
            admin.VillageName = await GetLocationNameAsync(permissions.Village,
                () => _locationDataService.GetVillagesAsync(permissions.Ward));
        }

        private async Task<string> GetLocationNameAsync(string code, Func<Task<List<string>>> getNamesFunc)
        {
            if (string.IsNullOrEmpty(code)) return string.Empty;

            var names = await getNamesFunc();
            return names.Contains(code) ? code : "Unknown Location";
        }

        private ViewModels.AdminPermissionDto MapToAdminPermissionDto(AdminPermissionViewModel viewModel)
        {
            return new ViewModels.AdminPermissionDto
            {
                Division = viewModel.Division,
                District = viewModel.District,
                Upazila = viewModel.Upazila,
                Ward = viewModel.Ward,
                Village = viewModel.Village,
                CanViewUserName = viewModel.CanViewUserName,
                CanViewEmail = viewModel.CanViewEmail,
                CanViewPhone = viewModel.CanViewPhone,
                CanViewAddress = viewModel.CanViewAddress,
                CanViewProfileImage = viewModel.CanViewProfileImage,
                CanViewNationalId = viewModel.CanViewNationalId,
                CanViewBirthRegistration = viewModel.CanViewBirthRegistration,
                CanViewDateOfBirth = viewModel.CanViewDateOfBirth,
                CanViewFamilyInfo = viewModel.CanViewFamilyInfo,
                CanViewProfession = viewModel.CanViewProfession,
                CanApproveRenters = viewModel.CanApproveRenters,
                CanApproveOwners = viewModel.CanApproveOwners,
                CanManageApartments = viewModel.CanManageApartments
            };
        }
    }
} 