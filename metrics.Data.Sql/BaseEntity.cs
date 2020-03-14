using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace metrics.Data.Sql
{
    public abstract class BaseEntity
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        
        [ConcurrencyCheck]
        [DatabaseGenerated(DatabaseGeneratedOption.Computed)]
        [Timestamp]
        public uint RowVersion { get; set; }
    }
}