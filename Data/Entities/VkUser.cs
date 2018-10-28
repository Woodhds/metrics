using DAL;

namespace Data.Entities
{
    public class VkUser : BaseEntity
    {
        public int UserId { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
    }
}