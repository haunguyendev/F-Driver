using System.ComponentModel.DataAnnotations;

namespace F_Driver.API.Payloads.Request
{
    public class StartTripRequest
    {
        [Required]
        public int TripMatchId { get; set; }
    }

}
