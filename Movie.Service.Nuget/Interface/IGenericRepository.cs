using System;
using Movie.Service.Nuget.Model;

namespace Movie.Service.Nuget.Interface
{
	public interface IGenericRepository<T> where T : IEntity
	{
        Task<bool> CreateAsync(T model);

        IQueryable<T> IQueryableOfT();
    }
}

