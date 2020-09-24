using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Base.Contracts
{
    public class VkUserModel
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int Id { get; set;}
        public string Avatar { get; set; }
        public string FullName { get; set; }
    }
}