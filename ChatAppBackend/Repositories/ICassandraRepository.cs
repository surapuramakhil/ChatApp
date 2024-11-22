using ChatAppBackend.Models;

namespace ChatAppBackend.Repositories
{
    public interface IChatRepository
    {
        Task InsertMessage(ChatMessage message);
        Task<IEnumerable<ChatMessage>> GetLastMessages(Guid chatId, int limit);
    }
}