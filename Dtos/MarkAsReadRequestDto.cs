namespace ServiceManagementAPI.Dtos
{
    public class MarkAsReadRequestDto
    {
        public string SenderId { get; set; } = null!;
        public string ReceiverId { get; set; } = null!;
    }
}
