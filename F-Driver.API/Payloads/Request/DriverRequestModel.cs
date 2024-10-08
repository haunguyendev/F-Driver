﻿using F_Driver.Service.BusinessModels;
using System.ComponentModel.DataAnnotations;

namespace F_Driver.API.Payloads.Request
{
    public class DriverRequestModel
    {
        public int? UserId { get; set; }
        [Required]
        public string LicenseNumber { get; set; } = string.Empty;
        [Required]
        public bool Verified { get; set; } = false;
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
                UserId = UserId,

                // Map Vehicles
                Vehicles = new List<CreateVehicleModel>
                {
                    vehicleRequestModel.MapToVehicleModel() // Giả sử có phương thức này
                }
            };
        }
    }
}
