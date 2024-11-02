using Microsoft.AspNetCore.Mvc;
using ServiceManagementAPI.Dtos;
using ServiceManagementAPI.Services.ChatService;

namespace ServiceManagementAPI.Controllers
{
    [Route("api/[controller]")]
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
    }
}
