using F_Driver.Service.BusinessModels;
using System.ComponentModel.DataAnnotations;

namespace F_Driver.API.Payloads.Request
{
    public class DriverRequestModel
    {
        public int? UserId { get; set; }
        [Required]
        public string LicenseNumber { get; set; } = string.Empty;
        public bool? Verified { get; set; }
        [Required]
        public IFormFile? LicenseImageUrl { get; set; }

        public List<VehicleRequestModel>? Vehicles { get; set; }

        public CreateDriverModel MapToDriverModel(VehicleRequestModel vehicleRequestModel)
        {
            return new CreateDriverModel
            {
                LicenseNumber = LicenseNumber,
                Verified = Verified,
                LicenseImageUrl = LicenseImageUrl,

                // Map Vehicles
                Vehicles = new List<CreateVehicleModel>
                {
                    vehicleRequestModel.MapToVehicleModel() // Giả sử có phương thức này
                }
            };
        }
    }
}
