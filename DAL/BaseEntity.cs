using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using DAL.Attributes;

namespace DAL
{
    public abstract class BaseEntity
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [ListView(Name = "ID", Visible = false)]
        public int Id { get; set; }
        public bool IsDeleted { get; set; }
        [Timestamp]
        [DatabaseGenerated(DatabaseGeneratedOption.Computed)]
        public byte[] Version { get; set; }
    }
}
