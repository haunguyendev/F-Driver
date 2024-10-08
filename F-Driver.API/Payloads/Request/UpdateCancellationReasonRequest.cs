using System.ComponentModel.DataAnnotations;

namespace F_Driver.API.Payloads.Request
{
    public class UpdateCancellationReasonRequest
    {
        [Required(ErrorMessage = "Content is required.")]
        [StringLength(255, ErrorMessage = "Content cannot be longer than 255 characters.")]
        public string Content { get; set; } = null!;
    }
}
