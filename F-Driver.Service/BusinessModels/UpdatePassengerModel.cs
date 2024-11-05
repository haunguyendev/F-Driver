using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace F_Driver.Service.BusinessModels
{
    public class UpdatePassengerModel
    {
        public string? Name { get; set; }
        public string? Email { get; set; }
        public string? PhoneNumber { get; set; }
        public string? ProfileImageUrl { get; set; }
        public IFormFile? StudentIdCardUrl { get; set; }
        public string? StudentId { get; set; }
        public int PassengerId { get; set; } // ID của hành khách cần cập nhật
    }
}
