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
            await Clients.User(receiverId).SendAsync("ReceiveMessage", messageDto);
        }

        public async Task LoadChatHistory(string userId1, string userId2)
        {
            var chatHistory = await _chatService.GetChatHistoryAsync(userId1, userId2);
            await Clients.Caller.SendAsync("ReceiveChatHistory", chatHistory);
        }

        public async Task MarkAsRead(string senderId, string receiverId)
        {
            await _chatService.MarkMessagesAsReadAsync(senderId, receiverId);
            await Clients.User(senderId).SendAsync("MessagesMarkedAsRead", receiverId);
        }

        public async Task GetUnreadCount(string userId)
        {
            var unreadCount = await _chatService.GetUnreadMessageCountAsync(userId);
            await Clients.Caller.SendAsync("ReceiveUnreadCount", unreadCount);
        }

        public override async Task OnConnectedAsync()
        {
            var userId = Context.UserIdentifier;
            if (userId != null)
            {
                await Groups.AddToGroupAsync(Context.ConnectionId, userId);
            }
            await base.OnConnectedAsync();
        }

        public override async Task OnDisconnectedAsync(Exception? exception)
        {
            var userId = Context.UserIdentifier;
            if (userId != null)
            {
                await Groups.RemoveFromGroupAsync(Context.ConnectionId, userId);
            }
            await base.OnDisconnectedAsync(exception);
        }
    }
}
