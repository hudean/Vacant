using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Vacant.Domain.Entites;
using Vacant.Domain.Repositories;
using Vacant.EntityFrameworkCore.Repositories;

namespace Vacant.EntityFrameworkCore
{
    public static class EfCoreServiceCollectionExtensions
    {

        public static IServiceCollection AddDbContextAndEfRepositories<TDbContext>(this IServiceCollection services,
           Action<DbContextOptionsBuilder> optionsAction = null)
           where TDbContext : DbContext
        {
            services.AddDbContext<TDbContext>(optionsAction)
                .AddRepositories<TDbContext>();
            return services;
        }

        public static IServiceCollection AddRepositories<TDbContext>(this IServiceCollection services)
          where TDbContext : DbContext
        {
            return services.AddRepositories(typeof(TDbContext));
        }


        public static IServiceCollection AddRepositories(this IServiceCollection services, Type dbContextType)
        {
            if (!typeof(DbContext).IsAssignableFrom(dbContextType))
                throw new ArgumentException($"parameter type error,the type must inherit from [{nameof(DbContext)}]");

            var entityTypes = GetEntityTypes(dbContextType);
            foreach (var entityType in entityTypes)
            {
                services.AddRepositories(entityType, dbContextType);
            }
            return services;
        }

        public static IServiceCollection AddRepositories<TEntity, TDbContext>(this IServiceCollection services)
          where TEntity : class, IEntity
          where TDbContext : DbContext
        {
            return services.AddRepositories(typeof(TEntity), typeof(TDbContext));
        }

        public static IServiceCollection AddRepositories(this IServiceCollection services, Type entityType, Type dbContextType)
        {
            if (!typeof(IEntity).IsAssignableFrom(entityType))
                throw new ArgumentException($"parameter type error,the type must inherit from [{nameof(IEntity)}]");

            if (!typeof(DbContext).IsAssignableFrom(dbContextType))
                throw new ArgumentException($"parameter type error,the type must inherit from [{nameof(DbContext)}]");

            if (entityType.GetInterfaces().Any(t => t.IsGenericType && t.GetGenericTypeDefinition() == typeof(IEntity<>)))
            {
                var iKey = entityType
                    .GetInterfaces()
                    .First(t => t.IsGenericType && t.GetGenericTypeDefinition() == typeof(IEntity<>))
                    .GenericTypeArguments
                    .Single();
                var repositoryType = typeof(IRepository<,>).MakeGenericType(entityType, iKey);
                var efCoreRepositoryType = typeof(EfCoreRepository<,,>).MakeGenericType(dbContextType, entityType, iKey);
                services.TryAddTransient(repositoryType, efCoreRepositoryType);
            }

            var repositoryType1 = typeof(IRepository<>).MakeGenericType(entityType);
            var efCoreRepositoryType1 = typeof(EfCoreRepository<,>).MakeGenericType(dbContextType, entityType);
            services.TryAddTransient(repositoryType1, efCoreRepositoryType1);
            return services;

        }

        private static IEnumerable<Type> GetEntityTypes(Type dbContextType)
        {
            return
                from property in dbContextType.GetTypeInfo().GetProperties(BindingFlags.Public | BindingFlags.Instance)
                where
                    IsAssignableToGenericType(property.PropertyType, typeof(DbSet<>)) &&
                    typeof(IEntity).IsAssignableFrom(property.PropertyType.GenericTypeArguments[0])
                select property.PropertyType.GenericTypeArguments[0];
        }

        private static bool IsAssignableToGenericType(Type givenType, Type genericType)
        {
            var givenTypeInfo = givenType.GetTypeInfo();

            if (givenTypeInfo.IsGenericType && givenType.GetGenericTypeDefinition() == genericType)
            {
                return true;
            }

            foreach (var interfaceType in givenTypeInfo.GetInterfaces())
            {
                if (interfaceType.GetTypeInfo().IsGenericType && interfaceType.GetGenericTypeDefinition() == genericType)
                {
                    return true;
                }
            }

            if (givenTypeInfo.BaseType == null)
            {
                return false;
            }

            return IsAssignableToGenericType(givenTypeInfo.BaseType, genericType);
        }
    }
}
