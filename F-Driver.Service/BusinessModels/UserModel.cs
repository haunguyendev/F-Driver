using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace F_Driver.Service.BusinessModels
{
    public class UserModel
    {
        public int Id { get; set; }

        public string Name { get; set; } = string.Empty;

        public string Email { get; set; } = string.Empty;

        public bool IsMailValid { get; set; } = false;

        public string PhoneNumber { get; set; } = string.Empty;

        public string PasswordHash { get; set; } = string.Empty;

        public string? ProfileImageUrl { get; set; }

        public string? StudentIdCardUrl { get; set; }

        public string Role { get; set; } = string.Empty;

        public string? StudentId { get; set; }

        public bool? Verified { get; set; }

        public string? VerificationStatus { get; set; }

        public DateTime? CreatedAt { get; set; }
    }
}
