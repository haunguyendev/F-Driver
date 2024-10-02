using F_Driver.Service.BusinessModels;
using System.ComponentModel.DataAnnotations;

namespace F_Driver.API.Payloads.Request
{
    public class TripRequestRequest
    {
        [Required]
        public int UserId { get; set; }

        [Required]
        public int FromZoneId { get; set; }

        [Required]
        public int ToZoneId { get; set; }

        [Required]
        public DateOnly TripDate { get; set; }

        [Required]
        public TimeOnly StartTime { get; set; }

        [Required]
        public int Slot { get; set; }

        [Required]
        public string Status { get; set; } = "Pending";

        public DateTime? CreatedAt { get; set; } = new DateTime();

        public TripRequestModel MapToTripRequestModel()
        {
            return new TripRequestModel()
            {
                UserId = UserId,
                FromZoneId = FromZoneId,
                ToZoneId = ToZoneId,
                TripDate = TripDate,
                StartTime = StartTime,
                Slot = Slot,
                Status = Status,
                CreatedAt = CreatedAt
            };
        }
    }
}
