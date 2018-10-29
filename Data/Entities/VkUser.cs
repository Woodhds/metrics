using System.ComponentModel.DataAnnotations.Schema;
using DAL;
using DAL.Attributes;

namespace Data.Entities
{
    [ViewConfig(nameof(FullName))]
    public class VkUser : BaseEntity
    {
        public int UserId { get; set; }
        [ListView(Name = "Имя")]
        public string FirstName { get; set; }
        [ListView(Name = "Фамилия")]
        public string LastName { get; set; }

        [NotMapped]
        public string FullName => FirstName + ' ' + LastName;
    }
}