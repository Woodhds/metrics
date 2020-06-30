using System;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace metrics.Data.Abstractions.Converters
{
    public class DateTimeValueConverter : ValueConverter<DateTime, DateTimeOffset>
    {
        public DateTimeValueConverter() : base(time => new DateTimeOffset(time.ToUniversalTime()),
            offset => offset.UtcDateTime)
        {
        }
    }
}