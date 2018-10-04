using System.Collections.Generic;
using DAL;

namespace Data.Entities
{
    public class ProductCategory : BaseEntity
    {
        public string Name { get; set; }
        public string Slug { get; set; }
        public virtual ICollection<Product> Products { get; set; }
    }
}