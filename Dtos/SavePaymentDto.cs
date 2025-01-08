using ServiceManagementAPI.Enums;

namespace ServiceManagementAPI.Dtos
{
    public class SavePaymentDto
    {
        public int BookingId { get; set; }
        public string TransactionId { get; set; } = string.Empty;
        public decimal Amount { get; set; }
        public string CustomerId { get; set; } = string.Empty;
        public DateTime PaymentDate { get; set; }
        public PaymentMethod PaymentMethod { get; set; }
        public PaymentStatus PaymentStatus { get; set; }
    }
}
