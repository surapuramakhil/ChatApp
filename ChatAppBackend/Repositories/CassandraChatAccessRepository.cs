using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Cassandra;

public class CassandraChatAccessRepository : IChatAccessRepository
{
    private readonly Cassandra.ISession _cassandraSession;

    public CassandraChatAccessRepository(Cassandra.ISession cassandraSession)
    {
        _cassandraSession = cassandraSession ?? throw new ArgumentNullException(nameof(cassandraSession));
    }

    /// <summary>
    /// Fetches chat IDs for a given sender ID from the Cassandra database.
    /// </summary>
    public async Task<List<Guid>> GetChatIdsForSenderAsync(Guid senderId)
    {
        var query = "SELECT chat_id FROM chatapp.chat_access WHERE user_id = ?";
        var preparedStatement = await _cassandraSession.PrepareAsync(query);
        var boundStatement = preparedStatement.Bind(senderId);

        var resultSet = await _cassandraSession.ExecuteAsync(boundStatement);

        return resultSet.Select(row => row.GetValue<Guid>("chat_id")).ToList();
    }
}