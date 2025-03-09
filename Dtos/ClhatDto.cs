namespace ServiceManagementAPI.Dtos
{
    public class ChatDto
    {
        public string SenderId { get; set; } = null!;
        public string ReceiverId { get; set; } = null!;
        public string SenderName { get; set; } = null!;
        public string SenderProfilePicture { get; set; } = null!;
        public string LastMessage { get; set; } = null!;
        public DateTime LastMessageAt { get; set; }
        public bool HasUnreadMessages { get; set; }
        public List<MessageDto> Messages { get; set; } = new();
    }
}
