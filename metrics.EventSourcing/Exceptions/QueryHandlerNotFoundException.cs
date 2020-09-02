using System;

namespace metrics.EventSourcing.Exceptions
{
    public class QueryHandlerNotFoundException : Exception
    {
        public QueryHandlerNotFoundException(Type type) : base($"QueryHandler of type {type.FullName} not found")
        {
            
        }
    }
}