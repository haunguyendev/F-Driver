using F_Driver.Service.BusinessModels;
using System.ComponentModel.DataAnnotations;

namespace F_Driver.API.Payloads.Request
{
    public class CancellationReasonRequest
    {
        public int Id { get; set; }
        [Required]
        public string Content { get; set; } = null!;

        public CancellationReasonModel MapToModel()
        {
            return new CancellationReasonModel
            {
                Id = Id,
                Content = Content
            };
        }
    }
}
