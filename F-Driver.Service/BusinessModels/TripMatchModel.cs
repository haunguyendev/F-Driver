using F_Driver.DataAccessObject.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace F_Driver.Service.BusinessModels
{
    public class TripMatchModel
    {
        public int? TripRequestId { get; set; }  // Chuyến đi liên quan đến ghép cặp này

        public int? DriverId { get; set; }       // Tài xế nào đang ghép cặp


        public DateTime? MatchedAt { get; set; }

        public string? Status { get; set; }

        public List<Cancellation> Cancellations { get; set; } = new List<Cancellation>();


        public List<Feedback> Feedbacks { get; set; } = new List<Feedback>();

        public List<Message> Messages { get; set; } = new List<Message>();

        public User? Driver { get; set; }

        public TripRequest? TripRequest { get; set; }

        public List<Payment> Payments { get; set; } = new List<Payment>();
    }
}
