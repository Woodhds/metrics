using System.ComponentModel.DataAnnotations.Schema;
using DAL;

namespace Data.Entities
{
    public class VkUser : BaseEntity
    {
        public string UserId { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Avatar { get; set; }
    }
}