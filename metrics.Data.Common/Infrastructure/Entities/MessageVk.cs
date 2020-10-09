using System.ComponentModel.DataAnnotations.Schema;

namespace metrics.Data.Common.Infrastructure.Entities
{
    public class MessageVk
    {
        public int MessageId { get; set; }
        public int OwnerId { get; set; }
        
        public int MessageCategoryId { get; set; }
        public MessageCategory MessageCategory { get; set; }

        [NotMapped]
        public int Identifier => (OwnerId ^ MessageId).GetHashCode();
    }
}