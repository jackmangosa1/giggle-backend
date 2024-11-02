using ServiceManagementAPI.Dtos;

namespace ServiceManagementAPI.Repositories.ChatRepository
{
    public interface IChatRepository
    {
        Task<MessageDto> SendMessageAsync(string senderId, string receiverId, string content);
        Task<IEnumerable<MessageDto>> GetChatHistoryAsync(string userId1, string userId2);
        Task MarkMessagesAsReadAsync(string senderId, string receiverId);
        Task<int> GetUnreadMessageCountAsync(string userId);
    }
}
