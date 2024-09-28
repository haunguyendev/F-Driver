using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace F_Driver.Service.BusinessModels
{
    public class CreateUserModel
    {
        public int Id { get; set; }

        public string Name { get; set; } = string.Empty;

        public string Email { get; set; } = string.Empty;

        public bool IsMailValid { get; set; } = false;

        public string PhoneNumber { get; set; } = string.Empty;

        public string PasswordHash { get; set; } = string.Empty;

        public IFormFile? ProfileImageUrl { get; set; }

        public IFormFile? StudentIdCardUrl { get; set; }

        public string Role { get; set; } = string.Empty;

        public string? StudentId { get; set; }

        public bool Verified { get; set; } = false;

        public string? VerificationStatus { get; set; }

        public DateTime? CreatedAt { get; set; }

        public CreateDriverModel? Driver { get; set; }
        //public List<FeedbackBusinessModel> Feedbacks { get; set; } = new List<FeedbackBusinessModel>();
        //public List<MessageBusinessModel> Messages { get; set; } = new List<MessageBusinessModel>();
        //public List<PaymentBusinessModel> Payments { get; set; } = new List<PaymentBusinessModel>();
        //public List<TripRequestBusinessModel> TripRequests { get; set; } = new List<TripRequestBusinessModel>();
        //public WalletBusinessModel? Wallet { get; set; }
    }
}
