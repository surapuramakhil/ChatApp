public interface IChatAccessService
{
    Task<List<Guid>> GetAccessibleChatIdsAsync(Guid senderId);
    Task<bool> HasAccessAsync(Guid senderId, Guid chatId);
    
}
