using F_Driver.Service.BusinessModels;
using System.ComponentModel.DataAnnotations;

namespace F_Driver.API.Payloads.Request
{
    public class FeedbackRequest
    {
        [Required(ErrorMessage = "MatchId is required.")]
        public int MatchId { get; set; }

        [Required(ErrorMessage = "DriverId is required.")]
        public int DriverId { get; set; }

        [Required(ErrorMessage = "PassengerId is required.")]
        public int PassengerId { get; set; }

        [Range(1, 5, ErrorMessage = "Rating must be between 1 and 5.")]
        public int Rating { get; set; }  // Đánh giá từ 1 đến 5

        [MaxLength(500, ErrorMessage = "Comment must not exceed 500 characters.")]
        public string? Comment { get; set; }


        public FeedbackCreateModel MapToModel()
        {
            return new FeedbackCreateModel
            {
                MatchId = this.MatchId,
                DriverId = this.DriverId,
                PassengerId = this.PassengerId,
                Rating = this.Rating,
                Comment = this.Comment,
                CreatedAt = DateTime.UtcNow
            };
        }
    }
}
