using ServiceManagementAPI.Dtos;
using ServiceManagementAPI.Repositories.ChatRepository;


namespace ServiceManagementAPI.Services.ChatService
{
    public class ChatService : IChatService
    {
        private readonly IChatRepository _chatRepository;
        public ChatService(IChatRepository chatRepository)
        {
            _chatRepository = chatRepository;
        }

        public async Task<MessageDto> SendMessageAsync(string senderId, string receiverId, string messageContent)
        {
            var message = await _chatRepository.SendMessageAsync(senderId, receiverId, messageContent);
            return message;
        }

        public async Task<IEnumerable<MessageDto>> GetChatHistoryAsync(string userId1, string userId2)
        {
            return await _chatRepository.GetChatHistoryAsync(userId1, userId2);
        }

        public async Task MarkMessagesAsReadAsync(string senderId, string receiverId)
        {
            await _chatRepository.MarkMessagesAsReadAsync(senderId, receiverId);
        }

        public async Task<int> GetUnreadMessageCountAsync(string userId)
        {
            return await _chatRepository.GetUnreadMessageCountAsync(userId);
        }

        public async Task<List<ChatDto>> GetUserChatsAsync(string userId)
        {
            return await _chatRepository.GetUserChatsAsync(userId);
        }

        public async Task<List<ChatDto>> GetProviderChatsAsync(string userId)
        {
            return await _chatRepository.GetProviderChatsAsync(userId);
        }

        public async Task<(string Name, string ProfilePictureUrl)> GetUserProfileInfoAsync(string userId)
        {
            return await _chatRepository.GetUserProfileInfoAsync(userId);
        }

        public async Task<ReceiverData> GetReceiverDataAsync(string receiverId)
        {
            return await _chatRepository.GetReceiverDataAsync(receiverId);
        }
    }
}
