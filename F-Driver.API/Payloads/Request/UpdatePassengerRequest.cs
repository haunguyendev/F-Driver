using F_Driver.Service.BusinessModels;
using System.ComponentModel.DataAnnotations;

namespace F_Driver.API.Payloads.Request
{
    public class UpdatePassengerRequest
    {
        [Required]
        public int Id { get; set; }

        [Required]
        public string Name { get; set; } = string.Empty;

        [Required]
        [EmailAddress(ErrorMessage = "Invalid email address format.")]
        public string Email { get; set; } = string.Empty;

        [Required]
        public bool IsMailValid { get; set; } = false;

        [Required]
        [RegularExpression(@"^0\d{9}$", ErrorMessage = "Phone number must start with 0 and be exactly 10 digits long.")]
        public string PhoneNumber { get; set; } = string.Empty;
        
        [Required]
        public string PasswordHash { get; set; } = string.Empty;

        public IFormFile? ProfileImageUrl { get; set; }

        public IFormFile? StudentIdCardUrl { get; set; }
        [Required]
        public string Role { get; set; } = string.Empty;

        [Required]
        public string? StudentId { get; set; }
        public bool Verified { get; set; } = false;
        public string VerificationStatus { get; set; } = "Pending";

        public CreateUserModel MapToPassengerModel()
        {
            var userModel = new CreateUserModel
            {
                Id = Id,
                Name = Name,
                Email = Email,
                PhoneNumber = PhoneNumber,
                PasswordHash = PasswordHash,
                ProfileImageUrl = ProfileImageUrl,
                StudentIdCardUrl = StudentIdCardUrl,
                Role = Role,
                StudentId = StudentId,
                Verified = Verified,
                VerificationStatus = VerificationStatus,
                IsMailValid = IsMailValid

            };
            return userModel;
        }
    }
}
