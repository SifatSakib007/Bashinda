using Bashinda.Models;
using System.ComponentModel.DataAnnotations;

namespace Bashinda.ViewModels
{
    public class RenterProfileViewModel
    {
        // Added missing properties
        public int Id { get; set; }
        public string? UniqueId { get; set; }
        public int UserId { get; set; }
        public bool IsApproved { get; set; }

        // Radio option: I'm Adult or I'm not Adult
        [Required(ErrorMessage = "Please select your adult status.")]
        public bool IsAdult { get; set; }

        // If adult, these fields are required
        [RequiredIf("IsAdult", true, ErrorMessage = "National ID is required for adults.")]
        public string? NationalId { get; set; }
        
        // File upload fields shouldn't be required as they need special handling
        public IFormFile? NationalIdImage { get; set; }

        // If not adult, these fields are required
        [RequiredIf("IsAdult", false, ErrorMessage = "Birth Registration Number is required for non-adults.")]
        public string? BirthRegistrationNo { get; set; }
        
        // File upload fields shouldn't be required as they need special handling
        public IFormFile? BirthRegistrationImage { get; set; }

        [Required]
        [DataType(DataType.Date)]
        public DateTime DateOfBirth { get; set; }

        [Required]
        public string? FullName { get; set; }

        [Required]
        public string? FatherName { get; set; }

        [Required]
        public string? MotherName { get; set; }

        [Required]
        public string Nationality { get; set; } = string.Empty;

        [Required]
        public string BloodGroup { get; set; } = string.Empty;

        [Required]
        public string Profession { get; set; } = string.Empty;

        [Required]
        public string Gender { get; set; } = string.Empty;

        [Required]
        [Phone]
        public string? MobileNo { get; set; }

        [Required]
        [EmailAddress]
        public string? Email { get; set; }

        public IFormFile? SelfImage { get; set; }

        // Permanent Resident Information:
        [Required]
        public string? Division { get; set; }

        [Required]
        public string? District { get; set; }

        [Required]
        public string? Upazila { get; set; }
        
        // New fields for permanent residence
        [Required]
        public string AreaType { get; set; } = string.Empty;
        
        [Required]
        public string? Ward { get; set; }
        
        [Required]
        public string? Village { get; set; }
        
        [Required]
        [RegularExpression(@"^\d{4}$", ErrorMessage = "Post code must be a 4-digit number.")]
        public string? PostCode { get; set; }
        
        [Required]
        public string? HoldingNo { get; set; }
    }

    // Custom validation attribute for conditional validation
    public class RequiredIfAttribute : ValidationAttribute
    {
        private readonly string _propertyName;
        private readonly object _desiredValue;

        public RequiredIfAttribute(string propertyName, object desiredValue, string? errorMessage = null)
        {
            _propertyName = propertyName;
            _desiredValue = desiredValue;
            if (!string.IsNullOrEmpty(errorMessage))
            {
                ErrorMessage = errorMessage;
            }
        }

        protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
        {
            var instance = validationContext.ObjectInstance;
            var propertyValue = instance.GetType().GetProperty(_propertyName)?.GetValue(instance);

            // Compare the dependent property value with the desired value
            if (object.Equals(propertyValue, _desiredValue))
            {
                // If the value is null or empty, it's not valid
                if (value == null || (value is string str && string.IsNullOrWhiteSpace(str)))
                {
                    return new ValidationResult(ErrorMessage ?? $"This field is required.");
                }
            }

            return ValidationResult.Success;
        }
    }
}
