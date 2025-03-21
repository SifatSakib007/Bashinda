using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;

namespace Bashinda.Models
{
    // This is a view model representation of the ApplicationUser from the API
    public class ApplicationUser : IdentityUser
    {
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; } = DateTime.Now;
        public DateTime CreatedDate { get; set; } = DateTime.Now;
        public DateTime? LastLogin { get; set; }
        public bool IsVerified { get; set; }
        public string Role { get; set; } = string.Empty;
    }
} 