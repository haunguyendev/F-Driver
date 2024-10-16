using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace F_Driver.Service.BusinessModels
{
    public class DriverModel
    {
        public int Id { get; set; }
        public string LicenseNumber { get; set; }
        public string LicenseImageUrl { get; set; }
        public bool? Verified { get; set; }
        public List<VehicleModel> Vehicles { get; set; }
    }
}
