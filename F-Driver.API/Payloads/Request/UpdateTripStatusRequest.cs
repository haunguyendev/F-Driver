using F_Driver.Service.Shared;
using System.ComponentModel.DataAnnotations;
using System.Runtime.CompilerServices;

namespace F_Driver.API.Payloads.Request
{
    public class UpdateTripStatusRequest
    {
        [Required]
        public string Status { get; set; }
        public int? ReasonId { get; set; }
        [Required]
        public bool IsTripMatchUpdate { get; set; }
    }
}
