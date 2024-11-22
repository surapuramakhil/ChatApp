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
        private readonly WebSockets.WebSocketManager _webSocketManager;

        public ChatController(IChatService chatService, WebSockets.WebSocketManager webSocketManager)
        {
            _chatService = chatService;
            _webSocketManager = webSocketManager;
        }

        [HttpPost("send")]
        public async Task<IActionResult> SendMessage([FromBody] ChatMessage message)
        {
            
            await _chatService.SendMessageAsync(message);
            return Ok();
        }

        [HttpGet("{chatId}/messages")]
        public async Task<IActionResult> GetLastMessages(Guid chatId)
        {
            var messages = await _chatService.GetLastMessagesAsync(chatId);
            return Ok(messages);
        }

        [HttpGet("ws")]
        public async Task GetWebSocket()
        {
            if (HttpContext.WebSockets.IsWebSocketRequest)
            {
                var webSocket = await HttpContext.WebSockets.AcceptWebSocketAsync();
                await _webSocketManager.AddWebSocketAsync(webSocket);
            }
            else
            {
                HttpContext.Response.StatusCode = 400;
            }
        }       
    }
}