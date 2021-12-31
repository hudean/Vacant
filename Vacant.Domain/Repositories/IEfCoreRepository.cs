using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vacant.Domain.Entites;

namespace Vacant.Domain.Repositories
{
    //public interface IEfCoreRepository
    //{
    //}

    //public interface IEfCoreRepository<TEntity> : IEfCoreRepository
    //    where TEntity : class, IEntity
    public interface IEfCoreRepository<TEntity> : IRepository<TEntity>
        where TEntity : class, IEntity
    {
        DbContext GetDbContext();

        DbSet<TEntity> GetDbSet();
    }

    public interface IEfCoreRepository<TEntity, TKey> : IEfCoreRepository<TEntity>, IRepository<TEntity, TKey>
       where TEntity : class, IEntity<TKey>
    {

    }
}
