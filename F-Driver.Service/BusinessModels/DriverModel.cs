using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace F_Driver.Service.BusinessModels
{
    public class DriverModel
    {
        public int Id { get; set; }

        public int? UserId { get; set; }

        public string LicenseNumber { get; set; } = string.Empty;

        public bool? Verified { get; set; }

        public IFormFile? LicenseImageUrl { get; set; }

        // Related Entities
        //public UserModel? User { get; set; }
        //public List<FeedbackBusinessModel> Feedbacks { get; set; } = new List<FeedbackBusinessModel>();
        //public List<PaymentBusinessModel> Payments { get; set; } = new List<PaymentBusinessModel>();
        
        public List<VehicleModel> Vehicles { get; set; } = new List<VehicleModel>();
    }
}
