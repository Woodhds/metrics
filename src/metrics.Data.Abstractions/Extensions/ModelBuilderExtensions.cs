using System;
using System.Linq;
using metrics.Data.Abstractions.Converters;
using Microsoft.EntityFrameworkCore;

namespace metrics.Data.Abstractions.Extensions
{
    public static class ModelBuilderExtensions
    {
        public static ModelBuilder SetDateTimeConverter(this ModelBuilder modelBuilder)
        {
            var converter = new DateTimeValueConverter();

            foreach (var type in modelBuilder.Model.GetEntityTypes())
            {
                foreach (var property in type.GetProperties().Where(e => e.GetValueConverter() == null)
                    .Where(f => typeof(DateTime).IsAssignableFrom(f.ClrType) ||
                                typeof(DateTime?).IsAssignableFrom(f.ClrType)))
                {
                    property.SetValueConverter(converter);
                }
            }

            return modelBuilder;
        }
    }
}