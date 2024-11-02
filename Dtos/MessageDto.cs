namespace ServiceManagementAPI.Dtos
{
    public class MessageDto
    {
        public string SenderId { get; set; } = null!;

        public string ReceiverId { get; set; } = null!;

        public string Content { get; set; } = null!;
    }
}
