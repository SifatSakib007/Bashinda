using Bashinda.Data;
using Bashinda.Models;
using Bashinda.ViewModels;
using Microsoft.EntityFrameworkCore;
using System.Security.Cryptography;
using System.Text;

namespace Bashinda.Services
{
    public class UserService : IUserService
    {
        private readonly ApplicationDbContext _context;
        private readonly IEmailSender _emailSender;

        public UserService(ApplicationDbContext context, IEmailSender emailSender)
        {
            _context = context;
            _emailSender = emailSender;
        }

        public async Task<(bool Success, string[] Errors)> RegisterUserAsync(RegisterViewModel model)
        {
            if (model == null)
                return (false, new[] { "Invalid registration data" });

            // Check if email or username already exists
            if (await _context.Users.AnyAsync(u => u.Email == model.Email))
                return (false, new[] { "Email already registered" });

            if (await _context.Users.AnyAsync(u => u.UserName == model.UserName))
                return (false, new[] { "Username already taken" });

            // Convert the string role to the enum
            if (!Enum.TryParse<UserRole>(model.Role, out var parsedRole))
            {
                return (false, new[] { "Invalid role selection" });
            }

            // Create user with hashed password
            var user = new User
            {
                UserName = model.UserName,
                Email = model.Email,
                PhoneNumber = model.PhoneNumber,
                PasswordHash = HashPassword(model.Password),
                IsVerified = false,
                Role = parsedRole
            };
            
            _context.Users.Add(user);
            await _context.SaveChangesAsync();

            // Generate OTP code
            var otpCode = GenerateOTP();
            var otpEntry = new UserOTP
            {
                UserId = user.Id,
                Code = otpCode,
                Expiry = DateTime.UtcNow.AddMinutes(10)
            };

            _context.UserOTPs.Add(otpEntry);
            await _context.SaveChangesAsync();

            // Send OTP via email
            await _emailSender.SendEmailAsync(user.Email, "Your OTP Code", $"Your OTP code is: {otpCode}");

            return (true, Array.Empty<string>());
        }

        public async Task<(bool Success, string[] Errors)> ConfirmOTPAsync(string email, string otp)
        {
            if (string.IsNullOrEmpty(email) || string.IsNullOrEmpty(otp))
                return (false, new[] { "Email and OTP are required" });

            var user = await _context.Users.FirstOrDefaultAsync(u => u.Email == email);
            if (user == null)
                return (false, new[] { "User not found" });

            var otpEntry = await _context.UserOTPs
                .Where(o => o.UserId == user.Id && o.Code == otp)
                .OrderByDescending(o => o.Expiry)
                .FirstOrDefaultAsync();

            if (otpEntry == null || otpEntry.Expiry < DateTime.UtcNow)
                return (false, new[] { "OTP is invalid or has expired" });

            // Mark the user as verified
            user.IsVerified = true;
            _context.Users.Update(user);

            // Remove the used OTP entry
            _context.UserOTPs.Remove(otpEntry);

            await _context.SaveChangesAsync();

            return (true, Array.Empty<string>());
        }

        public async Task<User?> AuthenticateUserAsync(LoginViewModel model)
        {
            if (model == null)
                return null;

            // Find the user by email
            var user = await _context.Users.FirstOrDefaultAsync(u => u.Email == model.Email);
            if (user == null || !user.IsVerified)
                return null;

            // Verify the password
            var hashedInput = HashPassword(model.Password);
            if (user.PasswordHash != hashedInput)
                return null;

            return user;
        }

        private string GenerateOTP()
        {
            // Generate a simple 6-digit OTP
            var random = new Random();
            return random.Next(100000, 999999).ToString();
        }

        private string HashPassword(string password)
        {
            // Use SHA256 for a simple example (consider stronger algorithms and salting in production)
            using (var sha256 = SHA256.Create())
            {
                var bytes = Encoding.UTF8.GetBytes(password);
                var hash = sha256.ComputeHash(bytes);
                return Convert.ToBase64String(hash);
            }
        }
    }
}
