using System;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;
using Movie.Service.Nuget.Model;

namespace Movie.Service.Nuget.Extension
{
	public static class CustomLinq
	{
        //public static IQueryable<T> ConvertModelToDto(this IQueryable<T> authors, string searchParam)
        //{
        //    return authors.Where(x => string.IsNullOrEmpty(searchParam) || x.AuthorName.Contains(searchParam));
        //}


        public static IQueryable<TEntity> ApplyIncludesOnQuery<TEntity>(this IQueryable<TEntity> query, params Expression<Func<TEntity, object>>[] includeProperties) where TEntity : class
        {
            // Return Applied Includes query
            return (includeProperties.Aggregate(query, (current, include) => current.Include(include)));
        }

        public static T ApplySinglePredicate<T>(this IQueryable<T> query, Expression<Func<T, bool>> predicate) where T : IEntity
        {
            return query.Where(predicate).FirstOrDefault();
        }
    }
}

