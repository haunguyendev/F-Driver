using F_Driver.Service.BusinessModels;
using System.ComponentModel.DataAnnotations;

namespace F_Driver.API.Payloads.Request
{
    public class UpdatePassengerRequest
    {
        public string? Name { get; set; } = string.Empty;

        [EmailAddress(ErrorMessage = "Invalid email address format.")]
        public string? Email { get; set; } = string.Empty;


        [RegularExpression(@"^0\d{9}$", ErrorMessage = "Phone number must start with 0 and be exactly 10 digits long.")]
        public string? PhoneNumber { get; set; } = string.Empty;
        

        public string? ProfileImageUrl { get; set; }

        public IFormFile? StudentIdCardUrl { get; set; }

        public string? StudentId { get; set; }
        public UpdatePassengerModel MapToPassengerModel()
        {
            return new UpdatePassengerModel
            {
                Name = this.Name,
                Email = this.Email,
                PhoneNumber = this.PhoneNumber,
                ProfileImageUrl = this.ProfileImageUrl,
                StudentIdCardUrl = this.StudentIdCardUrl,
                StudentId = this.StudentId
            };
        }
    }
}
