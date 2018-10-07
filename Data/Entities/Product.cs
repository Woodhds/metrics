using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
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
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public DateTime CreateDate { get; set; }
        [DatabaseGenerated(DatabaseGeneratedOption.Computed)]
        public DateTime? UpdateDate { get; set; }
        [NotMapped] 
        public string Thumbnail => ProductImages.FirstOrDefault(c => c.IsActive && c.IsMain)?.ThumbnailPath;
        public virtual ProductCategory ProductCategory { get; set; }
        public virtual ICollection<ProductImage> ProductImages { get; set; }
    }
}
