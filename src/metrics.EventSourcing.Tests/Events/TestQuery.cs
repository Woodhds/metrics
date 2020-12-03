using System;
using metrics.EventSourcing.Abstractions.Query;

namespace metrics.EventSourcing.Tests.Events
{
    public class TestQuery : IQuery<string>
    {
        public DateTimeOffset Date { get; set; }
        public decimal Price { get; set; }
    }
}