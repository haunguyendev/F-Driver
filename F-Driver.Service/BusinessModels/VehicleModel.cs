using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace F_Driver.Service.BusinessModels
{
    public class VehicleModel
    {
        public int Id { get; set; }
        public string? LicensePlate { get; set; }
        public string? VehicleType { get; set; }
        public bool? IsVerified { get; set; }
        public string? VehicleImageUrl { get; set; }
        public string? RegistrationImageUrl { get; set; }
    }
}
