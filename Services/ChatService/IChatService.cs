using ServiceManagementAPI.Dtos;

namespace ServiceManagementAPI.Services.ChatService
{
    public interface IChatService
    {
        Task<MessageDto> SendMessageAsync(string senderId, string receiverId, string message);
        Task<IEnumerable<MessageDto>> GetChatHistoryAsync(string senderId, string receiverId);
        Task MarkMessagesAsReadAsync(string senderId, string receiverId);
        Task<int> GetUnreadMessageCountAsync(string userId);

        Task<List<ChatDto>> GetUserChatsAsync(string userId);

        Task<List<ChatDto>> GetProviderChatsAsync(string userId);

        Task<(string Name, string ProfilePictureUrl)> GetUserProfileInfoAsync(string userId);

        Task<ReceiverData> GetReceiverDataAsync(string receiverId);
    }
}
