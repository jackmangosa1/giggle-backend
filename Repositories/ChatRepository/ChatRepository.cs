using Microsoft.EntityFrameworkCore;
using ServiceManagementAPI.Data;
using ServiceManagementAPI.Dtos;
using ServiceManagementAPI.Entities;

namespace ServiceManagementAPI.Repositories.ChatRepository
{
    public class ChatRepository : IChatRepository
    {
        private readonly ServiceManagementDbContext _context;
        public ChatRepository(ServiceManagementDbContext context)
        {
            _context = context;
        }

        public async Task<MessageDto> SendMessageAsync(string senderId, string receiverId, string content)
        {
            var message = new Message
            {
                SenderId = senderId,
                ReceiverId = receiverId,
                Content = content,
                SentAt = DateTime.UtcNow
            };

            _context.Messages.Add(message);
            await _context.SaveChangesAsync();
            var messageDto = new MessageDto
            {
                SenderId = message.SenderId,
                ReceiverId = message.ReceiverId,
                Content = message.Content,
            };

            return messageDto;
        }
        public async Task<IEnumerable<MessageDto>> GetChatHistoryAsync(string userId1, string userId2)
        {
            var messages = await _context.Messages
                .Where(m => (m.SenderId == userId1 && m.ReceiverId == userId2) ||
                    (m.SenderId == userId2 && m.ReceiverId == userId1))
                .OrderBy(m => m.SentAt)
                .ToListAsync();

            var messageDtos = new List<MessageDto>();
            foreach (var message in messages)
            {
                messageDtos.Add(new MessageDto
                {
                    SenderId = message.SenderId,
                    ReceiverId = message.ReceiverId,
                    Content = message.Content
                });
            }

            return messageDtos;
        }

        public async Task MarkMessagesAsReadAsync(string senderId, string receiverId)
        {
            var unreadMessages = await _context.Messages
                .Where(m => m.SenderId == senderId && m.ReceiverId == receiverId && !m.IsRead)
                .ToListAsync();

            unreadMessages.ForEach(m => m.IsRead = true);
            await _context.SaveChangesAsync();
        }
        public async Task<int> GetUnreadMessageCountAsync(string userId)
        {
            return await _context.Messages
                .CountAsync(m => m.ReceiverId == userId && !m.IsRead);
        }
    }
}
