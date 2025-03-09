using Microsoft.EntityFrameworkCore;
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
            return await _context.Messages
                .Where(m => (m.SenderId == userId1 && m.ReceiverId == userId2) ||
                            (m.SenderId == userId2 && m.ReceiverId == userId1))
                .OrderBy(m => m.SentAt)
                .Select(m => new MessageDto
                {
                    SenderId = m.SenderId,
                    ReceiverId = m.ReceiverId,
                    Content = m.Content
                })
                .ToListAsync();
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

        //public async Task<List<MessageDto>> GetUserMessagesAsync(string userId)
        //{
        //    return await _context.Messages
        //        .Where(m => m.SenderId == userId || m.ReceiverId == userId)
        //        .OrderBy(m => m.SentAt)
        //        .Select(m => new MessageDto
        //        {
        //            Id = m.Id,
        //            SenderId = m.SenderId,
        //            ReceiverId = m.ReceiverId,
        //            Content = m.Content,
        //            SentAt = m.SentAt,
        //            IsRead = m.IsRead
        //        })
        //        .ToListAsync();
        //}

        public async Task<List<ChatDto>> GetUserChatsAsync(string userId)
        {
            var conversations = await _context.Messages
                .Where(m => m.ReceiverId == userId)
                .GroupBy(m => m.SenderId)
                .Select(g => new ChatDto
                {
                    SenderId = g.Key,
                    ReceiverId = g.First().SenderId,
                    SenderName = g.First().Sender.Customer.FullName,
                    SenderProfilePicture = g.First().Sender.Customer.ProfilePictureUrl ?? "",
                    LastMessage = g.OrderByDescending(m => m.SentAt).First().Content,
                    LastMessageAt = g.OrderByDescending(m => m.SentAt).First().SentAt,
                    HasUnreadMessages = g.Any(m => !m.IsRead),
                    Messages = g.OrderBy(m => m.SentAt)
                                .Select(m => new MessageDto
                                {
                                    Id = m.Id,
                                    SenderId = m.SenderId,
                                    ReceiverId = m.ReceiverId,
                                    Content = m.Content,
                                    SentAt = m.SentAt,
                                    IsRead = m.IsRead
                                }).ToList()
                })
                .OrderByDescending(c => c.LastMessageAt)
                .ToListAsync();

            return conversations;
        }



        public async Task<List<ChatDto>> GetProviderChatsAsync(string userId)
        {
            var providerUserIds = await _context.Messages
                .Where(m => m.ReceiverId == userId || m.SenderId == userId)
                .Select(m => m.SenderId == userId ? m.ReceiverId : m.SenderId)
                .Distinct()
                .ToListAsync();

            var chats = new List<ChatDto>();

            foreach (var providerUserId in providerUserIds)
            {
                var provider = await _context.Providers
                    .FirstOrDefaultAsync(p => p.UserId == providerUserId);

                if (provider == null) continue;
                var messages = await _context.Messages
                    .Where(m => (m.SenderId == userId && m.ReceiverId == providerUserId) ||
                                (m.SenderId == providerUserId && m.ReceiverId == userId))
                    .OrderBy(m => m.SentAt)
                    .Select(m => new MessageDto
                    {
                        Id = m.Id,
                        SenderId = m.SenderId,
                        ReceiverId = m.ReceiverId,
                        Content = m.Content,
                        SentAt = m.SentAt,
                        IsRead = m.IsRead
                    })
                    .ToListAsync();

                if (!messages.Any()) continue;
                var lastMessage = messages.OrderByDescending(m => m.SentAt).First();

                var chat = new ChatDto
                {
                    SenderId = provider.UserId,
                    SenderName = provider.DisplayName ?? "Unknown Provider",
                    SenderProfilePicture = provider.ProfilePictureUrl ?? "",
                    LastMessage = lastMessage.Content,
                    LastMessageAt = lastMessage.SentAt,
                    HasUnreadMessages = messages.Any(m => m.SenderId == provider.UserId && !m.IsRead),
                    Messages = messages
                };

                chats.Add(chat);
            }
            return chats.OrderByDescending(c => c.LastMessageAt).ToList();
        }


        public async Task<(string Name, string ProfilePictureUrl)> GetUserProfileInfoAsync(string userId)
        {
            var provider = await _context.Providers
                .FirstOrDefaultAsync(p => p.UserId == userId);

            if (provider != null)
            {
                return (provider.DisplayName ?? "Unknown Provider", provider.ProfilePictureUrl ?? "");
            }

            var customer = await _context.Customers
                .FirstOrDefaultAsync(c => c.UserId == userId);

            if (customer != null)
            {
                return (customer.FullName ?? "Unknown Customer", customer.ProfilePictureUrl ?? "");
            }

            return ("Unknown User", "");
        }

        public async Task<ReceiverData> GetReceiverDataAsync(string receiverId)
        {
            var user = await _context.AspNetUsers
                .Include(u => u.Customer)
                .Include(u => u.Providers)
                .FirstOrDefaultAsync(u => u.Id == receiverId);

            if (user == null)
            {
                return new ReceiverData
                {
                    Name = "Unknown User",
                    ProfilePictureUrl = null
                };
            }

            string name = user.Customer?.FullName ?? user.Providers.FirstOrDefault()?.DisplayName ?? "Unknown User";
            string? profilePictureUrl = user.Customer?.ProfilePictureUrl ?? user.Providers.FirstOrDefault()?.ProfilePictureUrl;

            return new ReceiverData
            {
                Name = name,
                ProfilePictureUrl = profilePictureUrl
            };
        }
    }
}
