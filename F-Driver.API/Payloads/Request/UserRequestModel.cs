using F_Driver.Helpers;
using F_Driver.Service.BusinessModels;
using F_Driver.Service.Shared;
using System.ComponentModel.DataAnnotations;

namespace F_Driver.API.Payloads.Request
{
    public class UserRequestModel
    {
        [Required]
        public string Name { get; set; } = string.Empty;
        [Required]
        [EmailAddress(ErrorMessage = "Invalid email address format.")]
        public string Email { get; set; } = string.Empty;

        [Required]
        [RegularExpression(@"^0\d{9}$", ErrorMessage = "Phone number must start with 0 and be exactly 10 digits long.")]
        public string PhoneNumber { get; set; } = string.Empty;
        [Required]
        public string Password { get; set; } = string.Empty;
        [Required]
        public IFormFile? ProfileImageUrl { get; set; }
        public IFormFile? StudentIdCardUrl { get; set; }
        public string? StudentId { get; set; }
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
                PasswordHash = SecurityUtil.Hash(Password),
                ProfileImageUrl = ProfileImageUrl,
                StudentIdCardUrl = StudentIdCardUrl,
                Role = UserRoleEnum.DRIVER,
                StudentId = StudentId,
                Verified = false,
                VerificationStatus = UserVerificationStatusEnum.PENDING,
                IsMailValid = false

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
