using System;
using System.Collections.Generic;

namespace Bashinda.ViewModels
{
    // View model for user accounts
    public class UserViewModel
    {
        public int Id { get; set; }
        public string UserName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string PhoneNumber { get; set; } = string.Empty;
        public string Role { get; set; } = string.Empty;
        public bool IsVerified { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? LastLogin { get; set; }
        
        // Add missing properties
        public List<string> Roles { get; set; } = new List<string>();
        public bool EmailConfirmed { get; set; }
    }
} 