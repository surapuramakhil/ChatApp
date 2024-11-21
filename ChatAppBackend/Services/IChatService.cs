using ChatAppBackend.Models;


namespace ChatAppBackend.Services
{
    public interface IChatService
    {
        Task SendMessageAsync(ChatMessage message);
        Task<IEnumerable<ChatMessage>> GetLastMessagesAsync(Guid chatId);
    }
}