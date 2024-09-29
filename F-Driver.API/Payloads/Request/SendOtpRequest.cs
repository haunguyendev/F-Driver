using System.ComponentModel.DataAnnotations;
using System.Diagnostics.Eventing.Reader;

namespace F_Driver.API.Payloads.Request
{
    public class SendOtpRequest
    {
        [Required]
        public int UserId { get; set; }
        [Required]
        public string Email { get; set; }
        [Required]
        public bool IsResend { get; set; }
    }
}
