using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;

namespace BashindaAPI.Models
{
    public enum UserRole
    {
        SuperAdmin,
        Admin,
        ApartmentOwner,
        ApartmentRenter
    }
    public class User : IdentityUser<int>
    {
        public int Id { get; set; }
        public bool IsVerified { get; set; }
        public UserRole Role { get; set; }
        public AdminPermission? AdminPermission { get; set; }
    }
}