public interface IChatAccessRepository
{
    Task<List<Guid>> GetChatIdsForSenderAsync(Guid senderId);
}
