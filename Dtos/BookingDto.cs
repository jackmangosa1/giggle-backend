using System.ComponentModel.DataAnnotations;

namespace ServiceManagementAPI.Dtos
{
    public class BookingDto
    {
        [Required(ErrorMessage = "CustomerId is required.")]
        public string CustomerId { get; set; } = string.Empty;

        [Required(ErrorMessage = "ServiceId is required.")]
        [Range(1, int.MaxValue, ErrorMessage = "ServiceId must be greater than 0.")]
        public int ServiceId { get; set; }

        [Required(ErrorMessage = "Service date is required.")]
        public string Date { get; set; } = string.Empty;

        [Required(ErrorMessage = "Service time is required.")]
        public string Time { get; set; } = string.Empty;
    }
}

