using F_Driver.Service.BusinessModels;
using System.ComponentModel.DataAnnotations;

namespace F_Driver.API.Payloads.Request
{
    public class UserRequestModel
    {
        [Required]
        public string Name { get; set; } = string.Empty;
        [Required]
        public string Email { get; set; } = string.Empty;

        [Required]
        public bool IsMailValid { get; set; } = false;
        [Required]
        public string PhoneNumber { get; set; } = string.Empty;
        [Required]
        public string PasswordHash { get; set; } = string.Empty;
        [Required]
        public IFormFile? ProfileImageUrl { get; set; }
        [Required]
        public IFormFile? StudentIdCardUrl { get; set; }
        [Required]
        public string Role { get; set; } = string.Empty;
        [Required]
        public string? StudentId { get; set; }
        public bool Verified { get; set; } = false;
        public string VerificationStatus { get; set; } = "Pending";
        [Required]
        public DriverRequestModel Driver { get; set; } = new DriverRequestModel();

        [Required]
        public VehicleRequestModel Vehicle { get; set; } = new VehicleRequestModel();

        public CreateUserModel MapToUserModel() {             
            var userModel = new CreateUserModel
            {
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
            // If the role is "driver", map the Driver model
            if (Role.ToLower() == "driver" && Driver != null)
            {
                userModel.Driver = Driver.MapToDriverModel(Vehicle);
            }
            return userModel;
        }
    }
}
