using F_Driver.Service.BusinessModels;
using System.ComponentModel.DataAnnotations;

namespace F_Driver.API.Payloads.Request
{
    public class CancellationRequest
    {
        public int Id { get; set; }
        [Required]
        public int? TripMatchId { get; set; }
        [Required]
        public int? ReasonId { get; set; }
        public DateTime? CanceledAt { get; set; }

        public CancellationModel MapToModel()
        {
            return new CancellationModel
            {
                Id = Id,
                TripMatchId = TripMatchId,
                ReasonId = ReasonId,
                CanceledAt = CanceledAt
            };
        }
    }
}
