using ChatAppBackend.Models;

namespace ChatAppBackend.Repositories
{
    public interface ICassandraRepository
    {
        Task InsertMessage(ChatMessage message);
        Task<IEnumerable<ChatMessage>> GetLastMessages(Guid chatId, int limit);
    }
}