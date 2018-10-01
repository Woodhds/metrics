using System;
using System.Collections.Generic;
using System.Text;
using Data.Entities;
using Microsoft.EntityFrameworkCore;

namespace Data.EF
{
    public sealed class DataContext : DbContext
    {
        public DbSet<Product> Products { get; set; }

        public DataContext(DbContextOptions<DataContext> options) : base(options)
        {
            Database.AutoTransactionsEnabled = true;
        }
    }
}
