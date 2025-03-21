using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using BashindaAPI.Data;
using BashindaAPI.DTOs;
using BashindaAPI.Helpers;
using BashindaAPI.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;

namespace BashindaAPI.Services
{
    public class AuthService : IAuthService
    {
        private readonly ApplicationDbContext _context;
        private readonly IEmailService _emailService;
        private readonly JwtSettings _jwtSettings;
        private readonly ILogger<AuthService> _logger;

        public AuthService(
            ApplicationDbContext context,
            IEmailService emailService,
            IOptions<JwtSettings> jwtSettings,
            ILogger<AuthService> logger)
        {
            _context = context;
            _emailService = emailService;
            _jwtSettings = jwtSettings.Value;
            _logger = logger;
        }

        public async Task<(bool Success, string Token, User? User, string[] Errors)> LoginAsync(LoginDto model)
        {
            try
            {
                User? user = null;
                
                // Try to find user by phone number first, then by email
                if (!string.IsNullOrEmpty(model.PhoneNumber))
                {
                    user = await _context.Users.FirstOrDefaultAsync(u => u.PhoneNumber == model.PhoneNumber);
                    _logger.LogInformation("Login attempt with phone number: {PhoneNumber}", model.PhoneNumber);
                }
                else if (!string.IsNullOrEmpty(model.Email))
                {
                    user = await _context.Users.FirstOrDefaultAsync(u => u.Email == model.Email);
                    _logger.LogInformation("Login attempt with email: {Email}", model.Email);
                }
                else
                {
                    return (false, string.Empty, null, new[] { "Either phone number or email must be provided" });
                }
                
                if (user == null)
                {
                    return (false, string.Empty, null, new[] { "User not found" });
                }

                if (!VerifyPasswordHash(model.Password, user.PasswordHash))
                {
                    return (false, string.Empty, null, new[] { "Invalid password" });
                }

                if (!user.IsVerified)
                {
                    return (false, string.Empty, null, new[] { "Account not verified. Please verify your account first." });
                }

                var token = GenerateJwtToken(user);
                return (true, token, user, Array.Empty<string>());
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in LoginAsync: {ErrorMessage}", ex.Message);
                return (false, string.Empty, null, new[] { "An error occurred during login" });
            }
        }

        public async Task<(bool Success, string[] Errors)> RegisterAsync(RegisterDto model)
        {
            try
            {
                if (await _context.Users.AnyAsync(u => u.Email == model.Email))
                {
                    return (false, new[] { "Email already exists" });
                }

                if (await _context.Users.AnyAsync(u => u.UserName == model.UserName))
                {
                    return (false, new[] { "Username already exists" });
                }

                var passwordHash = HashPassword(model.Password);

                var user = new User
                {
                    UserName = model.UserName,
                    Email = model.Email,
                    PhoneNumber = model.PhoneNumber,
                    PasswordHash = passwordHash,
                    Role = model.Role,
                    IsVerified = false
                };

                _context.Users.Add(user);
                await _context.SaveChangesAsync();

                // Generate OTP
                var otpCode = GenerateOtp();
                var otpExpiry = DateTime.UtcNow.AddMinutes(15);

                var userOtp = new UserOTP
                {
                    UserId = user.Id,
                    Code = otpCode,
                    Expiry = otpExpiry
                };

                _context.UserOTPs.Add(userOtp);
                await _context.SaveChangesAsync();

                // Send OTP email
                await SendOtpEmail(user.Email, otpCode);

                return (true, Array.Empty<string>());
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in RegisterAsync: {ErrorMessage}", ex.Message);
                return (false, new[] { "An error occurred during registration" });
            }
        }

        public async Task<(bool Success, string[] Errors)> VerifyOtpAsync(VerifyOtpDto model)
        {
            try
            {
                var user = await _context.Users.FirstOrDefaultAsync(u => u.Email == model.Email);
                if (user == null)
                {
                    return (false, new[] { "User not found" });
                }

                var userOtp = await _context.UserOTPs
                    .Where(o => o.UserId == user.Id && o.Code == model.OtpCode && o.Expiry > DateTime.UtcNow)
                    .OrderByDescending(o => o.Expiry)
                    .FirstOrDefaultAsync();

                if (userOtp == null)
                {
                    return (false, new[] { "Invalid or expired OTP" });
                }

                user.IsVerified = true;
                _context.Users.Update(user);
                await _context.SaveChangesAsync();

                return (true, Array.Empty<string>());
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in VerifyOtpAsync: {ErrorMessage}", ex.Message);
                return (false, new[] { "An error occurred during OTP verification" });
            }
        }

        public async Task<(bool Success, string[] Errors)> ResendOtpAsync(string email)
        {
            try
            {
                var user = await _context.Users.FirstOrDefaultAsync(u => u.Email == email);
                if (user == null)
                {
                    return (false, new[] { "User not found" });
                }

                // Generate new OTP
                var otpCode = GenerateOtp();
                var otpExpiry = DateTime.UtcNow.AddMinutes(15);

                var userOtp = new UserOTP
                {
                    UserId = user.Id,
                    Code = otpCode,
                    Expiry = otpExpiry
                };

                _context.UserOTPs.Add(userOtp);
                await _context.SaveChangesAsync();

                // Send OTP email
                await SendOtpEmail(user.Email, otpCode);

                return (true, Array.Empty<string>());
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in ResendOtpAsync: {ErrorMessage}", ex.Message);
                return (false, new[] { "An error occurred while resending OTP" });
            }
        }

        private string GenerateJwtToken(User user)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(_jwtSettings.Key);

            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new Claim(ClaimTypes.Name, user.UserName),
                new Claim(ClaimTypes.Email, user.Email),
                new Claim(ClaimTypes.Role, user.Role.ToString())
            };

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(claims),
                Expires = DateTime.UtcNow.AddMinutes(_jwtSettings.DurationInMinutes),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature),
                Issuer = _jwtSettings.Issuer,
                Audience = _jwtSettings.Audience
            };

            var token = tokenHandler.CreateToken(tokenDescriptor);
            return tokenHandler.WriteToken(token);
        }

        private string HashPassword(string password)
        {
            using var hmac = new HMACSHA512();
            var salt = hmac.Key; // 64-byte salt
            var hash = hmac.ComputeHash(Encoding.UTF8.GetBytes(password));

            // Store salt and hash separately
            return Convert.ToBase64String(salt) + "|" + Convert.ToBase64String(hash);
        }

        private bool VerifyPasswordHash(string password, string storedHash)
        {
            var parts = storedHash.Split('|');
            if (parts.Length != 2) return false;

            var salt = Convert.FromBase64String(parts[0]);
            var storedHashBytes = Convert.FromBase64String(parts[1]);

            using var hmac = new HMACSHA512(salt);
            var computedHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(password));

            return computedHash.SequenceEqual(storedHashBytes);
        }

        private string GenerateOtp()
        {
            // Generate a 6-digit OTP
            var random = new Random();
            return random.Next(100000, 999999).ToString();
        }

        private async Task SendOtpEmail(string email, string otpCode)
        {
            var subject = "Verify Your Email - Bashinda";
            var message = $@"
                <html>
                <body style='font-family: Arial, sans-serif; line-height: 1.6; color: #333;'>
                    <div style='max-width: 600px; margin: 0 auto; padding: 20px; border: 1px solid #ddd; border-radius: 5px;'>
                        <h2 style='color: #4a6ee0;'>Email Verification</h2>
                        <p>Thank you for registering with Bashinda. Please use the following code to verify your email address:</p>
                        <div style='background-color: #f5f5f5; padding: 15px; border-radius: 5px; text-align: center; font-size: 24px; letter-spacing: 5px; font-weight: bold;'>
                            {otpCode}
                        </div>
                        <p>This code will expire in 15 minutes.</p>
                        <p>If you did not request this verification, please ignore this email.</p>
                        <p>Best regards,<br>The Bashinda Team</p>
                    </div>
                </body>
                </html>";

            await _emailService.SendEmailAsync(email, subject, message);
        }
    }
} 