using System;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using Microsoft.EntityFrameworkCore;

using VShop.SharedKernel.Infrastructure.Parameters.Paging.Contracts;
using VShop.SharedKernel.Infrastructure.Parameters.Sorting.Contracts;
using VShop.SharedKernel.Infrastructure.Parameters.Options.Contracts;

namespace VShop.SharedKernel.Infrastructure.Extensions
{
    public static class QueryableExtensions
    {
        public static IQueryable<TSource> Include<TSource>
        (
            this IQueryable<TSource> source,
            IOptionsParameters optionsParameters
        ) where TSource : class
        {
            if (optionsParameters?.Include is null) return source;

            IQueryable<TSource> query = source;

            foreach (string param in optionsParameters.Include)
            {
                if (!string.IsNullOrWhiteSpace(param))
                    query = source.Include(param);
            }

            return query;
        }

        public static IQueryable<TSource> SkipAndTake<TSource>
        (
            this IQueryable<TSource> source,
            IPagingParameters pagingParameters
        ) => source.Skip((pagingParameters.PageIndex - 1) * pagingParameters.PageSize).Take(pagingParameters.PageSize);
        
        public static IQueryable<TSource> Filter<TSource>
        (
            this IQueryable<TSource> source,
            Expression<Func<TSource, bool>> filterExpression
        ) => filterExpression is not null ? source.Where(filterExpression) : source;

        private static IQueryable<TSource> OrderBy<TSource>
        (
            this IQueryable<TSource> source,
            string orderBy,
            bool isFirstParam
        ) => OrderBy(source, orderBy, false, isFirstParam);
        
        private static IQueryable<TSource> OrderByDescending<TSource>
        (
            this IQueryable<TSource> source,
            string orderBy,
            bool isFirstParam
        ) => OrderBy(source, orderBy, true, isFirstParam);

        public static IQueryable<TSource> OrderBy<TSource>
        (
            this IQueryable<TSource> source,
            ISortingParameters sortingParameters
        )
        {
            if (sortingParameters?.Sorters is null) return source;

            int sortingCount = sortingParameters.Sorters.Count;
            if (sortingCount is 0) return source;

            IQueryable<TSource> query = source;
            for (int i = 0; i < sortingCount; i++)
            {
                ISortingPair item = sortingParameters.Sorters[i];

                query = item.Ascending ? 
                    query.OrderBy(item.OrderBy, i is 0) : 
                    query.OrderByDescending(item.OrderBy, i is 0);
            }

            return query;
        }

        private static IQueryable<TSource> OrderBy<TSource>
        (
            IQueryable<TSource> source,
            string orderBy,
            bool descending,
            bool isFirstParam
        )
        {
            try
            {
                Type type = typeof(TSource);
                
                string method = isFirstParam ? 
                    (descending ? "OrderByDescending" : "OrderBy") : 
                    (descending ? "ThenByDescending" : "ThenBy");
                
                string propNameExpression = char.ToUpper(orderBy[0]) + orderBy[1..];
                string[] props = propNameExpression.Split('.');
                
                PropertyInfo firstProperty = type.GetProperty(props.First());
                if (firstProperty is null) throw new Exception("Sorting expression is invalid.");
                
                ParameterExpression parameter = Expression.Parameter(type, "p");
                MemberExpression firstPropertyAccess = Expression.MakeMemberAccess(parameter, firstProperty);

                MemberExpression propertyAccess = null;
                MemberExpression previousPropertyAccess = null;
                Type propertyType = firstProperty.PropertyType;

                for (int i = 1; i < props.Length; i++)
                {
                    PropertyInfo property = propertyType.GetProperty(props[i]);
                    if (property is null) throw new Exception("Sorting expression is invalid.");

                    propertyAccess = Expression.MakeMemberAccess(previousPropertyAccess ?? firstPropertyAccess, property);

                    propertyType = property.PropertyType;
                    previousPropertyAccess = propertyAccess;
                }

                propertyAccess ??= firstPropertyAccess;

                LambdaExpression orderByExp = Expression.Lambda(propertyAccess, parameter);
                MethodCallExpression resultExp = Expression.Call
                (
                    typeof(Queryable),
                    method, 
                    new[] { type, propertyType },
                    source.Expression, 
                    Expression.Quote(orderByExp)
                );

                return source.Provider.CreateQuery<TSource>(resultExp);
            }
            catch (Exception ex)
            {
                throw new ArgumentException($"Invalid sort expression [{orderBy}].", ex);
            }
        }
    }
}