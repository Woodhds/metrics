using System.Linq;
using System.Linq.Dynamic.Core;
using System.Linq.Expressions;
using DAL.Core;

namespace DAL.Extensions
{
    public static class QueryableExtensions
    {
        public static IQueryable Select(this IQueryable source, string[] columns)
        {
            var builder = new SelectBuilder(columns);
            var select = builder.Build();
            var lambda = DynamicExpressionParser.ParseLambda(source.ElementType, null, select);
            var e = Expression.Call(typeof(Queryable), nameof(Queryable.Select), new[]
            {
                source.ElementType,
                lambda.Body.Type
            }, source.Expression, Expression.Quote(lambda));

            var query = source.Provider.CreateQuery(e);

            return query;
        }
    }
}