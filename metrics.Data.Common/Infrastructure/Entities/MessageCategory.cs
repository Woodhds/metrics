using System.ComponentModel.DataAnnotations;
using metrics.Data.Abstractions;
using metrics.Data.Sql;

namespace metrics.Data.Common.Infrastructure.Entities
{
    public class MessageCategory : BaseEntity
    {
        [MaxLength(255)]
        public string Title { get; set; }
        
        [MaxLength(100)]
        public string Color { get; set; }
    }
}