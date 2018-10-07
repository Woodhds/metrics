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
                c.HasIndex(d => d.Slug).IsUnique();
                c.HasOne(d => d.ProductCategory).WithMany(d => d.Products);
                c.HasMany(d => d.ProductImages).WithOne(e => e.Product);
            });
            modelBuilder.Entity<Order>();
            modelBuilder.Entity<ProductCategory>(c =>
            {
                c.HasIndex(d => new { d.Id, d.Slug }).IsUnique();
            });
            modelBuilder.Entity<ProductImage>(d => { d.HasOne(z => z.Product).WithMany(z => z.ProductImages); });
        }
    }
}
