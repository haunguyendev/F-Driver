using F_Driver.Service.BusinessModels;

namespace F_Driver.API.Payloads.Request
{
    public class TripRequestRequest
    {
        public int UserId { get; set; }

        public int FromZoneId { get; set; }

        public int ToZoneId { get; set; }

        public DateOnly TripDate { get; set; }

        public TimeOnly StartTime { get; set; }

        public int? Slot { get; set; }

        public string? Status { get; set; }

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
