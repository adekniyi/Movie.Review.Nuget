using System;
using Microsoft.EntityFrameworkCore;
using Movie.Service.Nuget.Interface;
using Movie.Service.Nuget.Model;

namespace Movie.Service.Nuget.Repository
{
	public class GenericRepository<T, TDbContext> : IGenericRepository<T>
        where T : class,IEntity
        where TDbContext: DbContext
    {
        private readonly TDbContext _dbContext;
        private DbSet<T> objContext;

        public GenericRepository(TDbContext dbContext)
        {
            _dbContext = dbContext;
            objContext = dbContext.Set<T>();
        }
        public async Task<bool> CreateAsync(T model)
        {
            try
            {
                await objContext.AddAsync(model);

                return SaveChanges();
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        public IQueryable<T> IQueryableOfT()
        {
            return objContext.AsQueryable();
        }

        public bool SaveChanges()
        {
            return  _dbContext.SaveChanges() > 0;
        }
    }
}

