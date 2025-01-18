using System.ComponentModel.DataAnnotations;

namespace ServiceManagementAPI.Dtos
{
    public class CreateReviewDto
    {
        [Required]
        public string UserId { get; set; } = string.Empty;

        [Required]
        public int CompletedServiceId { get; set; }

        [Range(1, 5, ErrorMessage = "Rating must be between 1 and 5")]
        public int Rating { get; set; }

        public string? Comment { get; set; }
    }
}
