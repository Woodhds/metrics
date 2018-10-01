using System.ComponentModel.DataAnnotations;

namespace DAL
{
    public class BaseEntity
    {
        [Key]
        int Id { get; set; }
        bool IsDeleted { get; set; }
    }
}
