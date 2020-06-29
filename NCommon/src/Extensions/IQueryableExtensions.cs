using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Linq.Expressions;
using System.Reflection;
using NCommon.Linq;

namespace NCommon.Extensions
{
    public static class IQueryableExtensions
    {

        public static IQueryable<TEntity> OrderBy<TEntity>(this IQueryable<TEntity> source, string orderByProperty,
                     bool desc) where TEntity : class
        {

            string command = desc ? "OrderByDescending" : "OrderBy";
            var type = typeof(TEntity);
            var property = type.GetProperty(orderByProperty);
            var parameter = Expression.Parameter(type, "p");
            var propertyAccess = Expression.MakeMemberAccess(parameter, property);
            var orderByExpression = Expression.Lambda(propertyAccess, parameter);
            var resultExpression = Expression.Call(typeof(Queryable), command, new Type[] { type, property.PropertyType },
                                   source.Expression, Expression.Quote(orderByExpression));
            return source.Provider.CreateQuery<TEntity>(resultExpression);

        }

        

        public static IQueryable<TSource> WhereIf<TSource>(this IQueryable<TSource> source, bool condition, Expression<Func<TSource, bool>> predicate)
        {
            if (condition)
                return source.Where(predicate);
            else
                return source;
        }

        public static IQueryable<TSource> WhereIf<TSource>(this IQueryable<TSource> source, bool condition, Expression<Func<TSource, int, bool>> predicate)
        {
            if (condition)
                return source.Where(predicate);
            else
                return source;
        }

        /*public static IEnumerable<TSource> Where<TSource>(this IEnumerable<TSource> source, Func<TSource, Boolean> predicate)
        {
            foreach (TSource element in source)
            {
                if (predicate(element))
                    yield return element;
            }
        }*/

        public static IQueryable<TSource> WhereLike<TSource>(this IQueryable<TSource> source, Expression<Func<TSource, string>> valueSelector, string value)
        {
            return source.Where(BuildLikeExpression(valueSelector, value, '%'));
        }

        public static Expression<Func<TElement, bool>> BuildLikeExpression<TElement>(Expression<Func<TElement, string>> valueSelector, string value, char wildcard)
        {
            if (valueSelector == null)
                throw new ArgumentNullException("valueSelector");

            var method = GetLikeMethod(value, wildcard);

            value = value.Trim(wildcard);
            var body = Expression.Call(valueSelector.Body, method, Expression.Constant(value));

            var parameter = valueSelector.Parameters.Single();
            return Expression.Lambda<Func<TElement, bool>>(body, parameter);
        }

        private static MethodInfo GetLikeMethod(string value, char wildcard)
        {
            var methodName = "Contains";

            var textLength = value.Length;
            value = value.TrimEnd(wildcard);
            if (textLength > value.Length)
            {
                methodName = "StartsWith";
                textLength = value.Length;
            }

            value = value.TrimStart(wildcard);
            if (textLength > value.Length)
            {
                //methodName = (methodName == "StartsWith") ? "Contains" : "EndsWith";//IF Business changes their mind to make it similar to LIKE function then uncomment this line.
                methodName = "Contains";
                textLength = value.Length;
            }

            var stringType = typeof(string);
            return stringType.GetMethod(methodName, new Type[] { stringType });
        }

        public static IQueryable<T> AsExpandable<T>(this IQueryable<T> query)
        {
            if (query is ExpandableQuery<T>) return (ExpandableQuery<T>)query;
            return new ExpandableQuery<T>(query);
        }

        

    }
}