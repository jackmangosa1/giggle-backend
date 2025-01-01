using ServiceManagementAPI.Enums;

namespace ServiceManagementAPI.Dtos
{
    public class NotificationDto
    {
        public int Id { get; set; }
        public NotificationTypes Type { get; set; }
        public string Date { get; set; } = string.Empty;
        public string Status { get; set; } = string.Empty;
        public int? BookingStatus { get; set; }
    }
}
