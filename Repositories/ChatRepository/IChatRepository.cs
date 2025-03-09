using ServiceManagementAPI.Dtos;

namespace ServiceManagementAPI.Repositories.ChatRepository
{
    public interface IChatRepository
    {
        Task<MessageDto> SendMessageAsync(string senderId, string receiverId, string content);
        Task<IEnumerable<MessageDto>> GetChatHistoryAsync(string userId1, string userId2);
        Task MarkMessagesAsReadAsync(string senderId, string receiverId);
        Task<int> GetUnreadMessageCountAsync(string userId);

        Task<List<ChatDto>> GetUserChatsAsync(string userId);
        Task<List<ChatDto>> GetProviderChatsAsync(string userId);

        Task<(string Name, string ProfilePictureUrl)> GetUserProfileInfoAsync(string userId);
        Task<ReceiverData> GetReceiverDataAsync(string receiverId);
    }
}
