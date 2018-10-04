using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using DAL;

namespace Data.Entities
{
    public class Product : BaseEntity
    {
        [Required]
        [MaxLength(255)]
        public string Name { get; set; }
        [Required]
        public decimal Price { get; set; }
        public int Weight { get; set; }
        public int Discount { get; set; }
        public string Description { get; set; }
        public string Composition { get; set; }
        public string Slug { get; set; }
        public DateTime CreateDate { get; set; }
        public string UpdateDate { get; set; }
        public virtual ProductCategory ProductCategory { get; set; }
        public virtual ICollection<ProductImage> ProductImages { get; set; }
    }
}
