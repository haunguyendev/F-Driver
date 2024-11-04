using System.ComponentModel.DataAnnotations;

namespace F_Driver.API.Validation
{
    public class RoleValidatorAttribute : ValidationAttribute
    {
        private readonly List<string> _validRoles = new List<string> { "passenger", "driver", "admin" };

        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            if (value == null || string.IsNullOrWhiteSpace(value.ToString()))
            {
                return new ValidationResult("Role is required.");
            }

            var roleValue = value.ToString().ToLower();
            if (!_validRoles.Contains(roleValue))
            {
                return new ValidationResult($"Invalid role. Valid roles are: {string.Join(", ", _validRoles)}.");
            }

            return ValidationResult.Success;
        }
    }
}
