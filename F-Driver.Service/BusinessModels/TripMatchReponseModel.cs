using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace F_Driver.Service.BusinessModels
{
    public class TripMatchReponseModel
    {
        public int? TripRequestId { get; set; }  
        public int? DriverId { get; set; } 


        public DateTime? MatchedAt { get; set; }

        public string? Status { get; set; }
    }
}
