using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using DAL;

namespace Data.Entities
{
    public class ProductImage : BaseEntity
    {
        [Required]
        public string Path { get; set; }
        [Required]
        public string ThumbnailPath { get; set; }
        public bool IsMain { get; set; }
        public bool IsActive { get; set; }
        public virtual Product Product { get; set; }
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public DateTime CreateDate { get; set; }
        [DatabaseGenerated(DatabaseGeneratedOption.Computed)]
        public DateTime? UpdateDate { get; set; }
    }
}