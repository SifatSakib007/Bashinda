using BashindaAPI.DTOs;
using BashindaAPI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;

namespace BashindaAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;
        private readonly ILogger<AuthController> _logger;

        public AuthController(IAuthService authService, ILogger<AuthController> logger)
        {
            _authService = authService;
            _logger = logger;
        }

        [HttpPost("register")]
        public async Task<ActionResult<AuthResponseDto>> Register([FromBody] RegisterDto model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(new AuthResponseDto
                {
                    Success = false,
                    Errors = ModelState.Values.SelectMany(v => v.Errors.Select(e => e.ErrorMessage)).ToArray()
                });
            }

            var result = await _authService.RegisterAsync(model);
            return Ok(new AuthResponseDto
            {
                Success = result.Success,
                Errors = result.Errors
            });
        }

        [HttpPost("login")]
        public async Task<ActionResult<AuthResponseDto>> Login([FromBody] LoginDto model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(new AuthResponseDto
                {
                    Success = false,
                    Errors = ModelState.Values.SelectMany(v => v.Errors.Select(e => e.ErrorMessage)).ToArray()
                });
            }

            var result = await _authService.LoginAsync(model);
            if (!result.Success)
            {
                return Unauthorized(new AuthResponseDto
                {
                    Success = false,
                    Errors = result.Errors
                });
            }

            return Ok(new AuthResponseDto
            {
                Success = true,
                Token = result.Token,
                User = new UserDto
                {
                    Id = result.User.Id,
                    UserName = result.User.UserName,
                    Email = result.User.Email,
                    PhoneNumber = result.User.PhoneNumber,
                    Role = result.User.Role.ToString(),
                    IsVerified = result.User.IsVerified
                }
            });
        }

        [HttpPost("verify-otp")]
        public async Task<ActionResult<AuthResponseDto>> VerifyOtp([FromBody] VerifyOtpDto model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(new AuthResponseDto
                {
                    Success = false,
                    Errors = ModelState.Values.SelectMany(v => v.Errors.Select(e => e.ErrorMessage)).ToArray()
                });
            }

            var result = await _authService.VerifyOtpAsync(model);
            return Ok(new AuthResponseDto
            {
                Success = result.Success,
                Errors = result.Errors
            });
        }

        [HttpPost("resend-otp")]
        public async Task<ActionResult<AuthResponseDto>> ResendOtp([FromBody] string email)
        {
            if (string.IsNullOrEmpty(email))
            {
                return BadRequest(new AuthResponseDto
                {
                    Success = false,
                    Errors = new[] { "Email is required" }
                });
            }

            var result = await _authService.ResendOtpAsync(email);
            return Ok(new AuthResponseDto
            {
                Success = result.Success,
                Errors = result.Errors
            });
        }

        [HttpGet("validate-token")]
        [Authorize]
        public IActionResult ValidateToken()
        {
            // If we get here, the token is valid
            return Ok(new { valid = true, userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value });
        }
    }
}
