using System.ComponentModel.DataAnnotations;

namespace F_Driver.API.Payloads.Request
{
    public class UpdateVerificationStatusRequest
    {
        [Required]
        public string VerificationStatus { get; set; } = null!;
    }
}
