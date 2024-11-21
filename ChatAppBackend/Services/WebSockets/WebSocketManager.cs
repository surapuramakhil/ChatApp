using System.Net.WebSockets;
using System.Text;

namespace ChatAppBackend.WebSockets
{
    public class WebSocketManager
    {
        private readonly List<WebSocket> _sockets = new List<WebSocket>();

        public async Task AddWebSocketAsync(WebSocket socket)
        {
            _sockets.Add(socket);

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
        }

        public async Task SendMessageToAllAsync(string message)
        {
            var buffer = Encoding.UTF8.GetBytes(message);

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