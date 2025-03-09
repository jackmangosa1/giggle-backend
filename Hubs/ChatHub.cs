using Microsoft.AspNetCore.SignalR;
using ServiceManagementAPI.Services.ChatService;

namespace ServiceManagementAPI.Hubs
{
    public class ChatHub : Hub
    {
        private readonly ChatService _chatService;

        public ChatHub(ChatService chatService)
        {
            _chatService = chatService;
        }

        public async Task SendMessage(string senderId, string receiverId, string messageContent)
        {
            var messageDto = await _chatService.SendMessageAsync(senderId, receiverId, messageContent);
            Console.WriteLine($"Sending message: {messageDto.ReceiverId}  {messageDto.SenderId}");
            await Clients.All.SendAsync("ReceiveMessage", messageDto);
        }

        public async Task LoadChatHistory(string userId1, string userId2)
        {
            var chatHistory = await _chatService.GetChatHistoryAsync(userId1, userId2);
            await Clients.Caller.SendAsync("ReceiveChatHistory", chatHistory);
        }

        public async Task MarkAsRead(string senderId, string receiverId)
        {
            await _chatService.MarkMessagesAsReadAsync(senderId, receiverId);
            await Clients.All.SendAsync("MessagesMarkedAsRead", receiverId);
        }

        public async Task GetUnreadCount(string userId)
        {
            var unreadCount = await _chatService.GetUnreadMessageCountAsync(userId);
            await Clients.Caller.SendAsync("ReceiveUnreadCount", unreadCount);
        }

        public override async Task OnConnectedAsync()
        {
            await base.OnConnectedAsync();
        }

        public override async Task OnDisconnectedAsync(Exception? exception)
        {
            await base.OnDisconnectedAsync(exception);
        }
    }
}
