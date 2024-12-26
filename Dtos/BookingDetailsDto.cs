namespace ServiceManagementAPI.Dtos
{
    public class BookingDetailsDto
    {
        public int BookingId { get; set; }
        public string CustomerName { get; set; } = string.Empty;
        public string ServiceName { get; set; } = string.Empty;
        public decimal Price { get; set; }
        public DateOnly Date { get; set; }
        public TimeOnly Time { get; set; }
        public string PaymentStatus { get; set; } = string.Empty;
        public string BookingStatus { get; set; } = string.Empty;
    }
}
