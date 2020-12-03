using System.ComponentModel.DataAnnotations;

namespace Base.Contracts
{
    public class VkUserModel
    {
        public int Id { get; set;}
        [MaxLength(512)]
        public string Avatar { get; set; }
        [MaxLength(512)]
        public string FullName { get; set; }
    }
}