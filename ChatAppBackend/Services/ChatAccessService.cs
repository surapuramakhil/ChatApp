using System;
using System.Collections.Generic;
using System.Threading.Tasks;

public class ChatAccessService : IChatAccessService
{
    private readonly IChatAccessRepository _repository;
    private readonly IAsyncCache<Guid, List<Guid>> _userAccessChatList;

    public ChatAccessService(IChatAccessRepository repository)
    {
        _repository = repository ?? throw new ArgumentNullException(nameof(repository));
        _userAccessChatList = new InMemoryLRUCache<Guid, List<Guid>>(1000, 50);
    }

    public async Task<List<Guid>> GetAccessibleChatIdsAsync(Guid senderId)
    {
        if (senderId == Guid.Empty)
            throw new ArgumentException("Sender ID cannot be empty.", nameof(senderId));

        // Check cache
        var cachedData = await _userAccessChatList.GetAsync(senderId);
        if (cachedData != null)
        {
            return cachedData;
        }

        // Fetch from repository
        var chatIds = await _repository.GetChatIdsForSenderAsync(senderId);

        // Store in cache
        await _userAccessChatList.SetAsync(senderId, chatIds);

        return chatIds;
    }

    public async Task<bool> HasAccessAsync(Guid senderId, Guid chatId)
    {
        if (senderId == Guid.Empty || chatId == Guid.Empty)
            throw new ArgumentException("Sender ID and Chat ID cannot be empty.");

        var accessibleChats = await GetAccessibleChatIdsAsync(senderId);
        return accessibleChats.Contains(chatId);
    }

    public async Task RemoveFromCache(Guid senderId)
    {
        if (senderId == Guid.Empty)
            throw new ArgumentException("Sender ID cannot be empty.", nameof(senderId));

        await _userAccessChatList.RemoveAsync(senderId);
    }

    public async Task ClearCache()
    {
        await _userAccessChatList.ClearAsync();
    }
}