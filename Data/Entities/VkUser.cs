using System.ComponentModel.DataAnnotations.Schema;
using DAL;
using DAL.Attributes;
using DAL.Models;

namespace Data.Entities
{
    [ViewConfig(nameof(FullName))]
    public class VkUser : BaseEntity
    {
        [ListView(Name = "Ид юзера", Required = true)]
        public string UserId { get; set; }
        [ListView(Name = "Имя", ReadOnly = true)]
        public string FirstName { get; set; }
        [ListView(Name = "Фамилия", ReadOnly = true)]
        public string LastName { get; set; }

        [ListView(Name = "Аватар", ReadOnly = true)]
        [PropertyDataType(PropertyDataType.Image)]
        public string Avatar { get; set; }

        [NotMapped]
        public string FullName => FirstName + ' ' + LastName;
    }
}