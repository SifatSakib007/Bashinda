using System;
using System.ComponentModel.DataAnnotations;

namespace Bashinda.ViewModels
{
    public class DetailedRenterProfileDto
    {
        public int Id { get; set; }
        public required string FirstName { get; set; }
        public required string LastName { get; set; }
        public required string Email { get; set; }
        public required string PhoneNumber { get; set; }
        public required string Address { get; set; }
        public DateTime DateOfBirth { get; set; }
        public required string NationalId { get; set; }
        public required string ProfilePicture { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
    }
} 