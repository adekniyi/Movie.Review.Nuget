using System;
using System.Text;
#if NET7_0
using Microsoft.AspNetCore.Authentication.JwtBearer;
#endif
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
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

        public static IServiceCollection AddCustomJWTAuthentication(this IServiceCollection services)
        {
            var key = Encoding.ASCII.GetBytes(JWTHandler.JwtSecret);

#if NET7_0
            services.AddAuthentication(x =>
            {
                x.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                x.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;

            }).AddJwtBearer(x =>
            {
                x.RequireHttpsMetadata = false;
                x.SaveToken = true;
                x.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(key),
                    ValidateIssuer = false,
                    ValidateAudience = false
                };

            });
#endif
            return services;
        }

        public static IServiceCollection AddServiceLifeScope(this IServiceCollection services)
        {
            services.AddScoped<IJWTRepo, JWTRepo>();

            return services;
        }
    }
}

