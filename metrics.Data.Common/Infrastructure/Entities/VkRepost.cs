using System.ComponentModel.DataAnnotations.Schema;

namespace metrics.Data.Common.Infrastructure.Entities
{
    public class VkRepost
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        
        public int UserId { get; set; }
        
        public int OwnerId { get; set; }
        public int MessageId { get; set; }
        
        public VkRepostStatus Status { get; set; }
    }
}