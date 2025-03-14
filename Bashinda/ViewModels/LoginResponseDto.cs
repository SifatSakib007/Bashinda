namespace Bashinda.ViewModels
{
    public class LoginResponseDto
    {
        public bool Success { get; set; }
        public string? Message { get; set; }
        public string? Token { get; set; }
        public string? RefreshToken { get; set; }
        public DateTime Expiration { get; set; }
        public string? UserName { get; set; }
        public string? Roles { get; set; }
        public UserDto? User { get; set; }
    }
} 