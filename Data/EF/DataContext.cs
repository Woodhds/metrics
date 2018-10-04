using Data.Entities;
using DAL;
using Microsoft.EntityFrameworkCore;

namespace Data.EF
{
    public sealed class DataContext : BaseContext
    {
        public DataContext(DbContextOptions options) : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.Entity<Product>(c => 
            { 
                c.HasOne(d => d.ProductCategory).WithMany(d => d.Products);
            });
            modelBuilder.Entity<Order>();
            modelBuilder.Entity<ProductCategory>(c =>
            {
                c.HasIndex(d => new { d.Id, d.Slug });
            });
            modelBuilder.Entity<ProductImage>(d => { d.HasOne(z => z.Product).WithMany(z => z.ProductImages); });
        }
    }
}
