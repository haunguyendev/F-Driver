using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace F_Driver.Service.BusinessModels
{
    public class DriverDetailModel
    {
        public int Id { get; set; }
        public string Name { get; set; } = null!;
        public string Email { get; set; } = null!;
        public string? PhoneNumber { get; set; }
        public string? ProfileImageUrl { get; set; }
        public string? StudentIdCardUrl { get; set; }
        public string Role { get; set; } = null!;
        public string? StudentId { get; set; }
        public bool? Verified { get; set; }
        public string? VerificationStatus { get; set; }
        public bool? IsMailValid { get; set; }
        public DriverModel? DriverInfo { get; set; }

        public WalletModel Wallet { get; set; }
    }
}
