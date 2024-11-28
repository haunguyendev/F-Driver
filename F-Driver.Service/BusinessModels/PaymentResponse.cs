using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace F_Driver.Service.BusinessModels
{
    public class PaymentResponse
    {
        public int PaymentId { get; set; }
        public int? MatchId { get; set; }
        public int? PassengerId { get; set; }
        public string? PassengerName { get; set; }
        public int? DriverId { get; set; }
        public string? DriverName { get; set; }
        public decimal Amount { get; set; }
        public string PaymentMethod { get; set; }
        public string? Status { get; set; }
        public DateTime? PaidAt { get; set; }

        public PaymentResponse(int paymentId, int? matchId, int? passengerId, string? passengerName, int? driverId, string? driverName, decimal amount, string paymentMethod, string? status, DateTime? paidAt)
        {
            PaymentId = paymentId;
            MatchId = matchId;
            PassengerId = passengerId;
            PassengerName = passengerName;
            DriverId = driverId;
            DriverName = driverName;
            Amount = amount;
            PaymentMethod = paymentMethod;
            Status = status;
            PaidAt = paidAt;
        }
    }
}
