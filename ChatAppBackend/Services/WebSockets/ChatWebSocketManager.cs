using System.Net.WebSockets;
using System.Text;
using System.Text.Json;
using ChatAppBackend.Models;

namespace ChatAppBackend.WebSockets
{
    public class ChatWebSocketManager
    {
        private readonly Dictionary<WebSocket, ChatContext> _socketContexts = new();

        public async Task AddWebSocketAsync(WebSocket socket, ChatContext context)
        {
            if (_socketContexts.ContainsKey(socket))
            {
                Console.WriteLine("WebSocket connection already exists.");
                return;
            }

            // Add socket to dictionary with context
            _socketContexts[socket] = context;
            Console.WriteLine($"New WebSocket connection added. User: {context.UserId}, Chat: {context.ChatId}");

            var buffer = new byte[1024 * 4];
            
            try
            {
                var result = await socket.ReceiveAsync(new ArraySegment<byte>(buffer), CancellationToken.None);

                while (!result.CloseStatus.HasValue)
                {
                    // Broadcast received message to all connected clients in the same chat room
                    foreach (var (s, ctx) in _socketContexts)
                    {
                        if (ctx.ChatId == context.ChatId && s.State == WebSocketState.Open)
                        {
                            await s.SendAsync(new ArraySegment<byte>(buffer, 0, result.Count), result.MessageType, result.EndOfMessage, CancellationToken.None);
                        }
                    }

                    result = await socket.ReceiveAsync(new ArraySegment<byte>(buffer), CancellationToken.None);
                }

                // Close socket connection gracefully
                await socket.CloseAsync(result.CloseStatus.Value, result.CloseStatusDescription, CancellationToken.None);
                Console.WriteLine($"WebSocket closed. User: {context.UserId}, Chat: {context.ChatId}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in WebSocket communication: {ex.Message}");
                throw;
            }
            finally
            {
                // Remove socket from dictionary on disconnection or error
                _socketContexts.Remove(socket);
                Console.WriteLine($"WebSocket removed. Current connections: {_socketContexts.Count}");
            }
        }

        public async Task SendMessageToAllAsync(ChatMessage message)
        {
            var options = new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.CamelCase };
            
            var messageString = JsonSerializer.Serialize(message, options);
            
            Console.WriteLine($"Broadcasting message: {messageString}. Number of connections: {_socketContexts.Count}");

            var buffer = Encoding.UTF8.GetBytes(messageString);

            foreach (var (socket, _) in _socketContexts)
            {
                if (socket.State == WebSocketState.Open)
                {
                    await socket.SendAsync(new ArraySegment<byte>(buffer), WebSocketMessageType.Text, true, CancellationToken.None);
                }
            }
        }
    }

    public class ChatContext
    {
        public Guid UserId { get; }
        public Guid ChatId { get; }

        public ChatContext(Guid userId, Guid chatId)
        {
            UserId = userId != Guid.Empty ? userId : throw new ArgumentException("User ID cannot be empty.");
            ChatId = chatId != Guid.Empty ? chatId : throw new ArgumentException("Chat ID cannot be empty.");
        }
    }
}