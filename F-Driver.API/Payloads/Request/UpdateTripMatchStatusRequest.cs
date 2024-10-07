namespace F_Driver.API.Payloads.Request
{
    public class UpdateTripMatchStatusRequest
    {
        public int TripMatchId { get; set; }
        public string Status { get; set; }
    }
}
