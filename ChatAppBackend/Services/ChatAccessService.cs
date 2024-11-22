using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Caching;

public class ChatAccessService : IChatAccessService
{
    private readonly IChatAccessRepository _repository;

    // LRU Cache for sender_id -> List<chat_id>
    private readonly LRUCache<Guid, List<Guid>> _userAccessChatList;

    public ChatAccessService(IChatAccessRepository repository, int cacheCapacity = 1000)
    {
        _repository = repository ?? throw new ArgumentNullException(nameof(repository));

        // Initialize LRU Cache with capacity and optional eviction count
        int evictionCount = (int)(cacheCapacity * 0.05); // Evict 5% of entries when full
        _userAccessChatList = new LRUCache<Guid, List<Guid>>(cacheCapacity, evictionCount);
    }

    /// <summary>
    /// Fetches chat IDs for a given sender ID, using the cache if possible.
    /// </summary>
    public async Task<List<Guid>> GetAccessibleChatIdsAsync(Guid senderId)
    {
        if (senderId == Guid.Empty)
            throw new ArgumentException("Sender ID cannot be empty.", nameof(senderId));

        // Check if senderId is in the cache
        if (_userAccessChatList.TryGet(senderId, out var chatIds))
        {
            return chatIds;
        }

        // If not in cache, fetch from the repository (Cassandra)
        try
        {
            chatIds = await _repository.GetChatIdsForSenderAsync(senderId);

            // Add to cache
            _userAccessChatList.AddReplace(senderId, chatIds);

            return chatIds;
        }
        catch (Exception ex)
        {
            // Log and rethrow exception (logging omitted for brevity)
            throw new InvalidOperationException($"Failed to fetch chat IDs for sender ID {senderId}.", ex);
        }
    }

    /// <summary>
    /// Validates whether a sender has access to a specific chat room.
    /// </summary>
    public async Task<bool> HasAccessAsync(Guid senderId, Guid chatId)
    {
        if (senderId == Guid.Empty || chatId == Guid.Empty)
            throw new ArgumentException("Sender ID and Chat ID cannot be empty.");

        var accessibleChats = await GetAccessibleChatIdsAsync(senderId);
        return accessibleChats.Contains(chatId);
    }

    /// <summary>
    /// Removes a sender ID from the cache (e.g., on logout or disconnection).
    /// </summary>
    public void RemoveFromCache(Guid senderId)
    {
        if (senderId == Guid.Empty)
            throw new ArgumentException("Sender ID cannot be empty.", nameof(senderId));

        try
        {
            _userAccessChatList.Remove(senderId);
        }
        catch (KeyNotFoundException)
        {
            // Ignore if the key does not exist in the cache
        }
    }

    /// <summary>
    /// Clears the entire cache (e.g., during application shutdown).
    /// </summary>
    public void ClearCache()
    {
        _userAccessChatList.Clear();
    }
}