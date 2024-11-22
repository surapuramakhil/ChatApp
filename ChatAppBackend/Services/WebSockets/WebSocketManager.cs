using System.Net.WebSockets;
using System.Text;
using ChatAppBackend.Models;
using System.Text.Json;

namespace ChatAppBackend.WebSockets
{
    public class WebSocketManager
    {
        private readonly List<WebSocket> _sockets = new List<WebSocket>();

        public async Task AddWebSocketAsync(WebSocket socket)
        {
            // Log the addition of a new socket connection
            if (_sockets.Contains(socket))
            {
            Console.WriteLine("Socket connection already exists.");
            return;
            }

            _sockets.Add(socket);
            Console.WriteLine($"New socket connection added. Current number of sockets: {_sockets.Count}");

            var buffer = new byte[1024 * 4];
            var result = await socket.ReceiveAsync(new ArraySegment<byte>(buffer), CancellationToken.None);

            while (!result.CloseStatus.HasValue)
            {
            // Broadcast received message to all connected clients
            foreach (var s in _sockets)
            {
                if (s.State == WebSocketState.Open)
                {
                    await s.SendAsync(new ArraySegment<byte>(buffer, 0, result.Count), result.MessageType, result.EndOfMessage, CancellationToken.None);
                }
            }

            result = await socket.ReceiveAsync(new ArraySegment<byte>(buffer), CancellationToken.None);
            }

            await socket.CloseAsync(result.CloseStatus.Value, result.CloseStatusDescription, CancellationToken.None);
            _sockets.Remove(socket);
            Console.WriteLine($"Socket closed. Current number of sockets: {_sockets.Count }");
        }

        public async Task SendMessageToAllAsync(ChatMessage message)
        {

            var options = new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase
            };

            var messageString = JsonSerializer.Serialize(message, options);

            Console.WriteLine($"Sending message: {messageString}. Number of socket connections: {_sockets.Count}");

            var buffer = Encoding.UTF8.GetBytes(messageString);

            foreach (var socket in _sockets)
            {
                if (socket.State == WebSocketState.Open)
                {
                    await socket.SendAsync(new ArraySegment<byte>(buffer), WebSocketMessageType.Text, true, CancellationToken.None);
                }
            }
        }
    }
}