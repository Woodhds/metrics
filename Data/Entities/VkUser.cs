using System.ComponentModel.DataAnnotations.Schema;
using DAL;
using DAL.Attributes;

namespace Data.Entities
{
    [ViewConfig(nameof(FullName))]
    public class VkUser : BaseEntity
    {
        public int UserId { get; set; }
        [ListView(Name = "Имя", Required = true)]
        public string FirstName { get; set; }
        [ListView(Name = "Фамилия", Required = true)]
        public string LastName { get; set; }

        [ListView(Name = "Возраст")]
        public int Age { get; set; }

        [NotMapped]
        public string FullName => FirstName + ' ' + LastName;
    }
}