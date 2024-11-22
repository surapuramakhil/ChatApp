using Cassandra;
using ChatAppBackend.Models;

namespace ChatAppBackend.Repositories
{
    public class CassandraChatRepository : IChatRepository
    {
        private readonly Cassandra.ISession _session;

        public CassandraChatRepository(Cassandra.ISession session)
        {
            _session = session;
        }

        public async Task InsertMessage(ChatMessage message)
        {
            var query = "INSERT INTO messages (chat_id, message_id, sender_id, body, timestamp) VALUES (?, ?, ?, ?, ?)";
            var statement = new SimpleStatement(query, message.ChatId, TimeUuid.NewId(), message.SenderId, message.Body, DateTime.UtcNow);
            await _session.ExecuteAsync(statement);
        }

        public async Task<IEnumerable<ChatMessage>> GetLastMessages(Guid chatId, int limit)
        {
            var query = "SELECT * FROM messages WHERE chat_id = ? LIMIT ?";
            var statement = new SimpleStatement(query, chatId, limit);
            var resultSet = await _session.ExecuteAsync(statement);

            return resultSet.Select(row => new ChatMessage
            {
                ChatId = row.GetValue<Guid>("chat_id"),
                SenderId = row.GetValue<Guid>("sender_id"),
                Body = row.GetValue<string>("body"),
                Timestamp = row.GetValue<DateTime>("timestamp")
            });
        }
    }
}