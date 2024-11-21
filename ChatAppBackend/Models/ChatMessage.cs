namespace ChatAppBackend.Models
{
    public class ChatMessage
    {
        public Guid ChatId { get; set; }
        public Guid SenderId { get; set; }
        public string? Body { get; set; }
        public DateTime Timestamp { get; set; }
    }
}