using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace F_Driver.Service.BusinessModels
{
    public class CreateVehicleModel
    {
        public int Id { get; set; }

        public int? DriverId { get; set; }

        public string LicensePlate { get; set; } = string.Empty;

        public string VehicleType { get; set; } = string.Empty;

        public bool? IsVerified { get; set; }


        public string Registration { get; set; } = string.Empty;

        public IFormFile? VehicleImageUrl { get; set; }

        public IFormFile? RegistrationImageUrl { get; set; }

        //// Related Entity
        //public DriverModel? Driver { get; set; }
    }
}
