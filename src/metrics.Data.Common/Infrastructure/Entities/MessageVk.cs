namespace metrics.Data.Common.Infrastructure.Entities
{
    public class MessageVk
    {
        public int MessageId { get; set; }
        public int OwnerId { get; set; }
        public int MessageCategoryId { get; set; }
        public MessageCategory MessageCategory { get; set; }
        public bool IsExported { get; set; }
    }
}