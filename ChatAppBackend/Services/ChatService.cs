using ChatAppBackend.Models;
using ChatAppBackend.Repositories;

namespace ChatAppBackend.Services
{
    public class ChatService : IChatService
    {
        private readonly ICassandraRepository _repository;

        public ChatService(ICassandraRepository repository)
        {
            _repository = repository;
        }

        public async Task SendMessageAsync(ChatMessage message)
        {
            await _repository.InsertMessage(message);
        }

        public async Task<IEnumerable<ChatMessage>> GetLastMessagesAsync(Guid chatId)
        {
            return await _repository.GetLastMessages(chatId, 50);
        }
    }
}