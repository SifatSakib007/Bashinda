using Bashinda.Models;
using System.ComponentModel.DataAnnotations;

namespace Bashinda.ViewModels
{
    public class RenterProfileViewModel
    {
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
        public Nationality Nationality { get; set; }

        [Required]
        public BloodGroup BloodGroup { get; set; }

        [Required]
        public Profession Profession { get; set; }

        [Required]
        public Gender Gender { get; set; }

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
