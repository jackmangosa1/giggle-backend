using ServiceManagementAPI.Dtos;

namespace ServiceManagementAPI.Services.ChatService
{
    public interface IChatService
    {
        Task<MessageDto> SendMessageAsync(string senderId, string receiverId, string message);
        Task<IEnumerable<MessageDto>> GetChatHistoryAsync(string senderId, string receiverId);
        Task MarkMessagesAsReadAsync(string senderId, string receiverId);
        Task<int> GetUnreadMessageCountAsync(string userId);
    }
}
