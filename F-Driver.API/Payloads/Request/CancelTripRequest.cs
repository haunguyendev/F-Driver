using System.ComponentModel.DataAnnotations;

namespace F_Driver.API.Payloads.Request
{
    public class CancelTripRequest
    {
        [Required]
        public int TripMatchId { get; set; }

        [Required]
        public int ReasonId { get; set; }
    }
}
