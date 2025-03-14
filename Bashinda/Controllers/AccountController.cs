using Bashinda.Services;
using Bashinda.ViewModels;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using Microsoft.Extensions.Logging;

namespace Bashinda.Controllers
{
    public class AccountController : Controller
    {
        private readonly IUserApiService _userService;
        private readonly ILogger<AccountController> _logger;

        public AccountController(IUserApiService userService, ILogger<AccountController> logger)
        {
            _userService = userService;
            _logger = logger;
        }

        // GET: /Account/Register
        public IActionResult Register()
        {
            return View(new RegisterUserDto());
        }

        // Post: /Account/Register
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Register(RegisterUserDto model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            try
            {
                var (success, user, errors) = await _userService.RegisterAsync(model);
                
                if (success)
                {
                    // Store email and password in TempData for the OTP confirmation and auto-login
                    TempData["RegisterEmail"] = model.Email;
                    TempData["RegisterPassword"] = model.Password;
                    
                    // Redirect to OTP confirmation page with email parameter
                    return RedirectToAction(nameof(ConfirmOTP), new { email = model.Email });
                }
                
                foreach (var error in errors)
                {
                    ModelState.AddModelError("", error);
                }
                
                return View(model);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Registration error");
                ModelState.AddModelError("", "An error occurred during registration. Please try again.");
                return View(model);
            }
        }
        
        // GET: /Account/ConfirmOTP
        public IActionResult ConfirmOTP(string email)
        {
            if (string.IsNullOrEmpty(email))
            {
                // Try to get email from TempData if not provided in URL
                email = TempData["RegisterEmail"] as string;
                
                if (string.IsNullOrEmpty(email))
                {
                    return RedirectToAction(nameof(Register));
                }
                
                // Preserve email for post action
                TempData.Keep("RegisterEmail");
            }
            
            var model = new OTPViewModel
            {
                Email = email
            };
            
            return View(model);
        }
        
        // POST: /Account/ConfirmOTP
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ConfirmOTP(OTPViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }
            
            try
            {
                // Call API to verify OTP
                var response = await _userService.VerifyOTPAsync(model.Email, model.OTP);
                
                if (response.Success)
                {
                    // After successful OTP verification, automatically log in the user
                    var storedPassword = TempData["RegisterPassword"] as string;
                    if (string.IsNullOrEmpty(storedPassword))
                    {
                        _logger.LogWarning("Password not found in TempData during auto-login");
                        return RedirectToAction(nameof(Login));
                    }

                    var loginModel = new LoginRequestDto
                    {
                        Email = model.Email,
                        Password = storedPassword
                    };

                    var (loginSuccess, loginResult, loginErrors) = await _userService.LoginAsync(loginModel);
                    
                    if (loginSuccess && loginResult != null)
                    {
                        _logger.LogInformation("Auto-login successful for user: {UserName} with roles: {Roles}", 
                            loginResult.User.UserName, loginResult.User.Roles != null ? string.Join(", ", loginResult.User.Roles) : "No roles");
                        
                        // Store the token in session
                        HttpContext.Session.SetString("AuthToken", loginResult.Token);
                        HttpContext.Session.SetInt32("UserId", loginResult.User.Id);
                        
                        // Create claims
                        var claims = new List<Claim>
                        {
                            new Claim(ClaimTypes.NameIdentifier, loginResult.User.UserName),
                            new Claim(ClaimTypes.Name, loginResult.User.UserName),
                            new Claim("UserId", loginResult.User.Id.ToString()),
                            new Claim("Token", loginResult.Token)
                        };
                        
                        // Add roles
                        if (loginResult.User.Roles != null && loginResult.User.Roles.Any())
                        {
                            foreach (var role in loginResult.User.Roles)
                            {
                                claims.Add(new Claim(ClaimTypes.Role, role));
                                _logger.LogInformation("Added role claim: {Role}", role);
                            }
                        }
                        else if (!string.IsNullOrEmpty(loginResult.User.Role))
                        {
                            claims.Add(new Claim(ClaimTypes.Role, loginResult.User.Role));
                            _logger.LogInformation("Added single role claim: {Role}", loginResult.User.Role);
                        }

                        var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
                        var authProperties = new AuthenticationProperties
                        {
                            IsPersistent = true,
                            ExpiresUtc = DateTimeOffset.UtcNow.AddDays(1),
                            AllowRefresh = true
                        };

                        await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, 
                            new ClaimsPrincipal(claimsIdentity), 
                            authProperties);

                        // Clear sensitive data from TempData
                        TempData.Remove("RegisterPassword");
                        TempData.Remove("RegisterEmail");
                        
                        // Force a session commit
                        await HttpContext.Session.CommitAsync();
                        
                        _logger.LogInformation("User authenticated successfully after registration, redirecting to dashboard");
                        return RedirectToDashboard();
                    }
                    
                    _logger.LogWarning("Auto-login failed after OTP verification. Errors: {Errors}", string.Join(", ", loginErrors));
                    foreach (var error in loginErrors)
                    {
                        ModelState.AddModelError("", error);
                    }
                    return View(model);
                }
                
                ModelState.AddModelError("", "Invalid OTP. Please try again.");
                return View(model);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error during OTP confirmation");
                ModelState.AddModelError("", "An error occurred during OTP confirmation. Please try again.");
                return View(model);
            }
        }
        
        // POST: /Account/ResendOTP
        [HttpPost]
        public async Task<IActionResult> ResendOTP(string email)
        {
            if (string.IsNullOrEmpty(email))
            {
                return BadRequest("Email is required");
            }
            
            try
            {
                // Call API to resend OTP
                var response = await _userService.ResendOTPAsync(email);
                
                if (response.Success)
                {
                    return Ok(new { message = "OTP sent successfully" });
                }
                
                return BadRequest(new { errors = response.Errors });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Resend OTP error");
                return StatusCode(500, new { error = "An error occurred while resending OTP" });
            }
        }

        // GET: /Account/Login
        public IActionResult Login()
        {
            // Check if user is already logged in
            if (User.Identity?.IsAuthenticated == true)
            {
                return RedirectToDashboard();
            }

            return View(new LoginRequestDto());
        }

        // POST: /Account/Login
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginRequestDto model)
        {
            _logger.LogInformation("Login attempt for email: {Email}", model.Email);
            
            if (!ModelState.IsValid)
            {
                _logger.LogWarning("Invalid model state for login attempt");
                return View(model);
            }

            try
            {
                var (success, loginResult, errors) = await _userService.LoginAsync(model);
                
                if (success && loginResult != null)
                {
                    _logger.LogInformation("Login successful for user: {UserName} with roles: {Roles}", 
                        loginResult.User.UserName, 
                        string.Join(", ", loginResult.User.Roles));
                    
                    // Validate token format
                    if (string.IsNullOrEmpty(loginResult.Token))
                    {
                        _logger.LogError("API returned empty token for successful login");
                        ModelState.AddModelError("", "Authentication error: Empty token received");
                        return View(model);
                    }
                    
                    // Basic JWT validation - should have 3 parts separated by dots
                    if (!loginResult.Token.Contains('.') || loginResult.Token.Count(c => c == '.') != 2)
                    {
                        _logger.LogError("API returned invalid token format: {TokenFormat}", 
                            loginResult.Token.Length > 20 ? 
                                $"{loginResult.Token.Substring(0, 10)}...{loginResult.Token.Substring(loginResult.Token.Length - 10)}" : 
                                loginResult.Token);
                        ModelState.AddModelError("", "Authentication error: Invalid token format");
                        return View(model);
                    }
                    
                    _logger.LogInformation("Valid token received, length: {TokenLength}", loginResult.Token.Length);
                    
                    // Store the token in session
                    HttpContext.Session.SetString("AuthToken", loginResult.Token);
                    // Also store the user ID in session for easy access
                    HttpContext.Session.SetInt32("UserId", loginResult.User.Id);
                    
                    // Temporarily store credentials in TempData for token refresh (This is relatively secure as TempData is server-side)
                    // Note: In a production environment, consider more secure approaches like ASP.NET Core Data Protection API
                    TempData["UserEmail"] = model.Email;
                    TempData["UserPassword"] = model.Password;
                    
                    // Create claims
                    var claims = new List<Claim>
                    {
                        new Claim(ClaimTypes.NameIdentifier, loginResult.User.UserName),
                        new Claim(ClaimTypes.Name, loginResult.User.UserName),
                        new Claim("UserId", loginResult.User.Id.ToString()),
                        new Claim("Token", loginResult.Token)
                    };
                    
                    // Add roles
                    if (loginResult.User.Roles != null && loginResult.User.Roles.Any())
                    {
                        foreach (var role in loginResult.User.Roles)
                        {
                            claims.Add(new Claim(ClaimTypes.Role, role));
                            _logger.LogInformation("Added role claim: {Role}", role);
                        }
                    }
                    else if (!string.IsNullOrEmpty(loginResult.User.Role))
                    {
                        // Fallback to single role if Roles list is empty
                        claims.Add(new Claim(ClaimTypes.Role, loginResult.User.Role));
                        _logger.LogInformation("Added single role claim: {Role}", loginResult.User.Role);
                    }

                    var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
                    var authProperties = new AuthenticationProperties
                    {
                        IsPersistent = true,
                        ExpiresUtc = DateTimeOffset.UtcNow.AddDays(1),
                        AllowRefresh = true
                    };

                    await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, 
                        new ClaimsPrincipal(claimsIdentity), 
                        authProperties);

                    _logger.LogInformation("User authenticated successfully, redirecting to dashboard");
                    
                    // Force a session commit
                    await HttpContext.Session.CommitAsync();
                    
                    // Log the final user identity for debugging
                    _logger.LogInformation("Final user identity: {Identity}", 
                        string.Join(", ", claims.Select(c => $"{c.Type}: {c.Value}")));
                    
                    return RedirectToDashboard();
                }
                
                _logger.LogWarning("Login failed with errors: {Errors}", string.Join(", ", errors));
                foreach (var error in errors)
                {
                    ModelState.AddModelError("", error);
                }
                
                return View(model);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Login error for email: {Email}", model.Email);
                ModelState.AddModelError("", "An error occurred during login. Please try again.");
                return View(model);
            }
        }
        
        // Helper method to redirect to appropriate dashboard based on role
        private IActionResult RedirectToDashboard()
        {
            _logger.LogInformation("Redirecting to dashboard. User roles: {Roles}", 
                string.Join(", ", User.Claims.Where(c => c.Type == ClaimTypes.Role).Select(c => c.Value)));

            // Check if user is admin
            if (User.IsInRole("Admin"))
            {
                _logger.LogInformation("Redirecting to Admin dashboard");
                return RedirectToAction("Index", "Admin");
            }
            
            // Check if user is apartment owner
            if (User.IsInRole("ApartmentOwner"))
            {
                _logger.LogInformation("Redirecting to ApartmentOwner dashboard");
                return RedirectToAction("Index", "Home");
            }
            
            // Check if user is apartment renter
            if (User.IsInRole("ApartmentRenter"))
            {
                _logger.LogInformation("Redirecting to Renter dashboard");
                return RedirectToAction("Index", "Renter");
            }
            
            _logger.LogWarning("No matching role found, redirecting to Home");
            return RedirectToAction("Index", "Home");
        }

        // GET: /Account/Logout
        public async Task<IActionResult> Logout()
        {
            try
            {
                // Get the token from session
                var token = HttpContext.Session.GetString("AuthToken");
                if (!string.IsNullOrEmpty(token))
                {
                    // Call the API to logout (revoke token)
                    await _userService.LogoutAsync(token);
                    
                    // Remove token from session
                    HttpContext.Session.Remove("AuthToken");
                }

                // Sign out from cookies
                await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
                
                return RedirectToAction("Login", "Account");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Logout error");
                return RedirectToAction("Login", "Account");
            }
        }

        // GET: /Account/AccessDenied
        public IActionResult AccessDenied()
        {
            return View();
        }
    }
}
