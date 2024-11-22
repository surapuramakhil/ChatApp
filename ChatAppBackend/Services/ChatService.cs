using ChatAppBackend.Models;
using ChatAppBackend.Repositories;

namespace ChatAppBackend.Services
{
    public class ChatService : IChatService
    {
        private readonly IChatRepository _repository;
        private readonly WebSockets.ChatWebSocketManager _webSocketManager;

        public ChatService(IChatRepository repository, WebSockets.ChatWebSocketManager webSocketManager)
        {
            _repository = repository;
            _webSocketManager = webSocketManager;
        }

        public async Task SendMessageAsync(ChatMessage message)
        {
            Console.WriteLine($"Sending message: {message.Body}");
            await _repository.InsertMessage(message);
            await _webSocketManager.BroadcastMessageAsync(message);
        }

        public async Task<IEnumerable<ChatMessage>> GetLastMessagesAsync(Guid chatId)
        {
            return await _repository.GetLastMessages(chatId, 50);
        }
    }
}