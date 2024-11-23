using Microsoft.AspNetCore.Mvc;
using ChatAppBackend.Models;
using ChatAppBackend.Services;
using ChatAppBackend.WebSockets;

namespace ChatAppBackend.Controllers
{
    [ApiController]
    [Route("api/chat")]
    public class ChatController : ControllerBase
    {
        private readonly IChatService _chatService;
        private readonly ChatWebSocketManager _webSocketManager;
        private readonly IChatAccessService _accessControlService;

        public ChatController(
            IChatService chatService,
            ChatWebSocketManager webSocketManager,
            IChatAccessService accessControlService)
        {
            _chatService = chatService;
            _webSocketManager = webSocketManager;
            _accessControlService = accessControlService;
        }

        [HttpPost("send")]
        public async Task<IActionResult> SendMessage([FromBody] ChatMessage message)
        {
            var hasAccess = await _accessControlService.HasAccessAsync(message.SenderId, message.ChatId);
            if (!hasAccess)
            {
                return Forbid();
            }
            await _chatService.SendMessageAsync(message);
            return Ok();
        }

        [HttpGet("{chatId}/messages")]
        public async Task<IActionResult> GetLastMessages(Guid chatId, Guid userId)
        {
            var hasAccess = await _accessControlService.HasAccessAsync(userId, chatId);
            if (!hasAccess)
            {
                return Forbid();
            }
            var messages = await _chatService.GetLastMessagesAsync(chatId);
            return Ok(messages);
        }

        [HttpGet("ws")]
        public async Task GetWebSocket(Guid userId, Guid chatId)
        {
            if (!HttpContext.WebSockets.IsWebSocketRequest)
            {
                HttpContext.Response.StatusCode = 400; // Bad Request
                return;
            }

            // Perform access check
            var hasAccess = await _accessControlService.HasAccessAsync(userId, chatId);
            if (!hasAccess)
            {
                HttpContext.Response.StatusCode = 403; // Forbidden
                return;
            }

            // Accept WebSocket connection
            var webSocket = await HttpContext.WebSockets.AcceptWebSocketAsync();

            // Add WebSocket to manager with context (UserId and ChatId)
            var chatContext = new ChatContext(userId, chatId);
            await _webSocketManager.AddWebSocketAsync(webSocket, chatContext);
        }
    }
}