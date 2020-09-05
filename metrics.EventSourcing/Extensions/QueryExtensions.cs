using System.Linq;
using System.Reflection;
using metrics.EventSourcing.Abstractions.Query;
using Microsoft.Extensions.DependencyInjection;

namespace metrics.EventSourcing.Extensions
{
    public static class QueryExtensions
    {
        public static IServiceCollection AddQueryProcessor(this IServiceCollection services, Assembly assembly)
        {
            services.AddSingleton<IQueryProcessor, QueryProcessor>();

            services.AddEvents(assembly);

            return services;
        }

        private static void AddEvents(this IServiceCollection services, Assembly assembly)
        {
            var queryDefinitions = assembly.GetTypes().SelectMany(f =>
                    f.GetInterfaces().Where(t =>
                        t.IsGenericType && t.GetGenericTypeDefinition() == typeof(IQueryHandler<,>)))
                .ToList();

            foreach (var query in queryDefinitions)
            {
                var implType =
                    assembly.GetTypes().FirstOrDefault(f => query.IsAssignableFrom(f));
                if (implType != null)
                {
                    services.AddTransient(query, implType);
                }
            }
        }
    }
}