using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace F_Driver.Service.BusinessModels
{
    public class TripMatchReponseModel
    {
        public int? Id { get; set; }
        public int? TripRequestId { get; set; }  
        public int? DriverId { get; set; } 


        public DateTime? MatchedAt { get; set; }

        public string? Status { get; set; }
        public DriverInfomation Driver { get; set; }
        public TripRequestInfomation TripRequest { get; set; }

    }


    public class DriverInfomation
    {
        public string Name { get; set; } = null!;
        public string Email { get; set; } = null!;
        public string? ProfileImageUrl { get; set; }
        public string LicenseNumber { get; set; }
        public string LicenseImageUrl { get; set; }
        public string? LicensePlate { get; set; }
        public string? VehicleImageUrl { get; set; }
    }
    public class TripRequestInfomation
    {
        public int UserId { get; set; }
        public int FromZoneId { get; set; } 
        public int ToZoneId { get; set; }
        public string FromZoneName { get; set; }
        public string ToZoneName { get; set; }
        public DateOnly TripDate { get; set; }

        public TimeOnly StartTime { get; set; }

    }
}