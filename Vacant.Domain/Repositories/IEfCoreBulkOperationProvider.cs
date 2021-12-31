using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Vacant.Domain.Entites;

namespace Vacant.Domain.Repositories
{
    public interface IEfCoreBulkOperationProvider
    {
        Task InsertManyAsync<TDbContext, TEntity>(
            IEfCoreRepository<TEntity> repository,
            IEnumerable<TEntity> entities,
            bool autoSave,
            CancellationToken cancellationToken
        )
            where TDbContext : DbContext//IEfCoreDbContext
            where TEntity : class, IEntity;


        Task UpdateManyAsync<TDbContext, TEntity>(
            IEfCoreRepository<TEntity> repository,
            IEnumerable<TEntity> entities,
            bool autoSave,
            CancellationToken cancellationToken
        )
            where TDbContext : DbContext//IEfCoreDbContext
            where TEntity : class, IEntity;


        Task DeleteManyAsync<TDbContext, TEntity>(
            IEfCoreRepository<TEntity> repository,
            IEnumerable<TEntity> entities,
            bool autoSave,
            CancellationToken cancellationToken
        )
            where TDbContext : DbContext//IEfCoreDbContext
            where TEntity : class, IEntity;
    }
}
