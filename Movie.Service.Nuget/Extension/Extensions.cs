using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Movie.Service.Nuget.Interface;
using Movie.Service.Nuget.Model;
using Movie.Service.Nuget.Repository;

namespace Movie.Service.Nuget.Extension
{
	public static class Extensions
	{
        public static IServiceCollection AddDatabase<TDbContext>(this IServiceCollection services, string connectionString)
         where TDbContext : DbContext
        {
            services.AddDbContext<TDbContext>(opt => opt.UseSqlServer(connectionString));
            return services;
        }

        public static IServiceCollection AddGenericRepository<TDbContext,T>(this IServiceCollection services)
            where TDbContext: DbContext
            where T : class,IEntity
        {
            services.AddScoped<IGenericRepository<T>>(serviceProvider =>
            {
                var database = serviceProvider.GetService<TDbContext>();

                return new GenericRepository<T,TDbContext>(database);
            });


            return services;
        }

        public static IServiceCollection AddMessageBusClient(this IServiceCollection services)
        {
            services.AddSingleton<IMessageBusClient, MessageBusClient>();

            return services;
        }

        public static IServiceCollection AddMessageBusConsumer<T>(this IServiceCollection services) where T : IEventProcessor
        {
            services.AddSingleton<MessageBusConsumer<T>>();

            return services;
        }
    }
}

