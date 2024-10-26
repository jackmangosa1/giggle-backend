using System.ComponentModel.DataAnnotations;

namespace ServiceManagementAPI.Dtos
{
    public class BookingDto
    {
        [Required(ErrorMessage = "ServiceId is required.")]
        [Range(1, int.MaxValue, ErrorMessage = "ServiceId must be greater than 0.")]
        public int ServiceId { get; set; }

        [Required(ErrorMessage = "CustomerId is required.")]
        [Range(1, int.MaxValue, ErrorMessage = "CustomerId must be greater than 0.")]
        public int CustomerId { get; set; }

        [Required(ErrorMessage = "ScheduledAt date is required.")]
        [DataType(DataType.DateTime, ErrorMessage = "ScheduledAt must be a valid date and time.")]
        public DateTime ScheduledAt { get; set; }

        [Required(ErrorMessage = "TotalPrice is required.")]
        [Range(0.01, double.MaxValue, ErrorMessage = "TotalPrice must be greater than 0.")]
        public decimal TotalPrice { get; set; }
    }
}
