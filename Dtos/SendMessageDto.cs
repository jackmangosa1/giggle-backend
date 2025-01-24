namespace ServiceManagementAPI.Dtos
{
    public class SendMessageDto
    {
        public string SenderId { get; set; } = string.Empty;
        public string ReceiverId { get; set; } = string.Empty;
        public string MessageContent { get; set; } = string.Empty;
    }
}
