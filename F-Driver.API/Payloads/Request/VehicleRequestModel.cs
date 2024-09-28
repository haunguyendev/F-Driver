using F_Driver.Service.BusinessModels;
using System.ComponentModel.DataAnnotations;

namespace F_Driver.API.Payloads.Request
{
    public class VehicleRequestModel
    {
        public int? DriverId { get; set; }
        [Required]
        public string LicensePlate { get; set; } = string.Empty;
        [Required]
        public string VehicleType { get; set; } = string.Empty;
        public bool? IsVerified { get; set; }
        [Required]
        public string Registration { get; set; } = string.Empty;
        [Required]
        public IFormFile? VehicleImageUrl { get; set; }
        [Required]
        public IFormFile? RegistrationImageUrl { get; set; }

        public CreateVehicleModel MapToVehicleModel()
        {
            return new CreateVehicleModel
            {
                DriverId = DriverId,
                LicensePlate = LicensePlate,
                VehicleType = VehicleType,
                IsVerified = IsVerified,
                Registration = Registration,
                VehicleImageUrl = VehicleImageUrl,
                RegistrationImageUrl = RegistrationImageUrl
            };
        }
    }
}
