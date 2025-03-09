using Microsoft.AspNetCore.Mvc;
using ServiceManagementAPI.Dtos;
using ServiceManagementAPI.Services.ChatService;

namespace ServiceManagementAPI.Controllers
{
    [Route("api/chat")]
    [ApiController]
    public class ChatController : ControllerBase
    {
        private readonly IChatService _chatService;

        public ChatController(IChatService chatService)
        {
            _chatService = chatService;
        }

        [HttpPost("send")]
        public async Task<ActionResult<MessageDto>> SendMessage([FromBody] MessageDto request)
        {
            var messageDto = await _chatService.SendMessageAsync(request.SenderId, request.ReceiverId, request.Content);
            return Ok(messageDto);
        }

        [HttpGet("history")]
        public async Task<ActionResult<IEnumerable<MessageDto>>> GetChatHistory([FromQuery] string userId1, [FromQuery] string userId2)
        {
            var chatHistory = await _chatService.GetChatHistoryAsync(userId1, userId2);
            return Ok(chatHistory);
        }

        [HttpPost("mark-as-read")]
        public async Task<IActionResult> MarkMessagesAsRead([FromBody] MarkAsReadRequestDto request)
        {
            await _chatService.MarkMessagesAsReadAsync(request.SenderId, request.ReceiverId);
            return NoContent();
        }

        [HttpGet("unread-count")]
        public async Task<ActionResult<int>> GetUnreadMessageCount([FromQuery] string userId)
        {
            var unreadCount = await _chatService.GetUnreadMessageCountAsync(userId);
            return Ok(unreadCount);
        }
        [HttpGet("chats/{userId}")]
        public async Task<ActionResult<List<ChatDto>>> GetUserChats(string userId)
        {
            var chats = await _chatService.GetUserChatsAsync(userId);
            if (!chats.Any())
            {
                return NotFound("No conversations found.");
            }
            return Ok(chats);
        }

        [HttpGet("chats/provider/{userId}")]
        public async Task<ActionResult<List<ChatDto>>> GetProviderChats(string userId)
        {
            var chats = await _chatService.GetProviderChatsAsync(userId);
            if (!chats.Any())
            {
                return NotFound("No conversations found.");
            }
            return Ok(chats);
        }

        [HttpGet("user-profile/{userId}")]
        public async Task<ActionResult<UserProfileDto>> GetUserProfile(string userId)
        {
            var (name, profilePictureUrl) = await _chatService.GetUserProfileInfoAsync(userId);

            var profileDto = new UserProfileDto
            {
                UserId = userId,
                Name = name,
                ProfilePictureUrl = profilePictureUrl
            };

            return Ok(profileDto);
        }

        [HttpGet("receiver/{receiverId}")]
        public async Task<ActionResult<ReceiverData>> GetReceiverData(string receiverId)
        {
            var receiverData = await _chatService.GetReceiverDataAsync(receiverId);
            return receiverData != null ? Ok(receiverData) : NotFound("Receiver not found.");
        }
    }
}
