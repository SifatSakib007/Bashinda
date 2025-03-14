using System.ComponentModel.DataAnnotations;

namespace Bashinda.ViewModels
{
    // View model for user accounts
    public class UserViewModel
    {
        public int Id { get; set; }
        public string Email { get; set; } = string.Empty;
        public string UserName { get; set; } = string.Empty;
        public List<string> Roles { get; set; } = new List<string>();
        public bool EmailConfirmed { get; set; }
    }
} 