using System.ComponentModel.DataAnnotations;

namespace ServiceManagementAPI.Dtos
{
    public class BookingDto
    {
        [Required(ErrorMessage = "CustomerId is required.")]
        [Range(1, int.MaxValue, ErrorMessage = "CustomerId must be greater than 0.")]
        public int CustomerId { get; set; }

        [Required(ErrorMessage = "ServiceId is required.")]
        [Range(1, int.MaxValue, ErrorMessage = "ServiceId must be greater than 0.")]
        public int ServiceId { get; set; }

        [Required(ErrorMessage = "Service date is required.")]
        [DataType(DataType.Date, ErrorMessage = "Date must be a valid date.")]
        public DateOnly Date { get; set; }

        [Required(ErrorMessage = "Service time is required.")]
        [DataType(DataType.Time, ErrorMessage = "Time must be a valid time.")]
        public TimeOnly Time { get; set; }
    }
}
