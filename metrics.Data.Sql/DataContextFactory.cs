﻿using metrics.Data.Abstractions;
using Microsoft.EntityFrameworkCore;

namespace metrics.Data.Sql
{
    public class DataContextFactory : IDataContextFactory
    {
        private readonly DbContextOptions _options;

        public DataContextFactory(DbContextOptions options)
        {
            _options = options;
        }

        public DbContext Create()
        {
            return new DataContext(_options);
        }
    }
}