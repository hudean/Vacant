using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Vacant.Domain.Entites;
using Vacant.Domain.Uow;

namespace Vacant.Domain.Repositories
{

    public class EfCoreRepository<TDbContext, TEntity> : IRepository<TEntity>,
       IEfCoreRepository<TEntity>
       where TDbContext : DbContext
       where TEntity : class, IEntity
    {
        private readonly DbContext _dbContext;
        private readonly DbSet<TEntity> _dbSet;
        public IGuidGenerator GuidGenerator { get; set; }
        public EfCoreRepository(DbContext dbContext)
        {
            _dbContext = dbContext;
            _dbSet = dbContext.Set<TEntity>();
        }

        public EfCoreRepository(IServiceProvider serviceProvider, IUnitOfWorkManager unitOfWorkManager)
        {
            var dbContextName = DbContextNameAttribute.GetNameOrDefault(typeof(TDbContext));
            if (!unitOfWorkManager.TryGetUnitOfWork(dbContextName, out var unitOfWork))
            {
                var unitOfWorkType = typeof(EfCoreUnitOfWork<>).MakeGenericType(typeof(TDbContext));
                unitOfWork = (IUnitOfWork)ActivatorUtilities.CreateInstance(serviceProvider, unitOfWorkType);
                unitOfWorkManager.TryAddUnitOfWork(dbContextName, unitOfWork);
            }
            var unitOfWork1 = (EfCoreUnitOfWork<TDbContext>)unitOfWork;
            _dbContext = unitOfWork1.DbContext;
            _dbSet = unitOfWork1.DbContext.Set<TEntity>();
        }

        public DbContext GetDbContext()
        {
            return _dbContext;
        }

        public DbSet<TEntity> GetDbSet()
        {
            return _dbSet;
        }

        public IQueryable<TEntity> GetQueryable()
        {
            return _dbSet.AsQueryable();
        }

        #region query

        public bool Exists(Expression<Func<TEntity, bool>> expression)
        {
            return _dbSet.Any(expression);
        }

        public Task<bool> ExistsAsync(Expression<Func<TEntity, bool>> expression, CancellationToken cancellationToken = default)
        {
            return _dbSet.AnyAsync(expression, cancellationToken);
        }

        public TEntity Find(Expression<Func<TEntity, bool>> expression)
        {
            return _dbSet.Where(expression).FirstOrDefault();
        }

        public IEnumerable<TEntity> FindAll(Expression<Func<TEntity, bool>> expression)
        {
            return _dbSet.Where(expression).ToList();
        }

        public Task<List<TEntity>> FindAllAsync(Expression<Func<TEntity, bool>> expression, CancellationToken cancellationToken = default)
        {
            return _dbSet.Where(expression).ToListAsync(cancellationToken);
        }

        public Task<TEntity> FindAsync(Expression<Func<TEntity, bool>> expression, CancellationToken cancellationToken = default)
        {
            return _dbSet.Where(expression).FirstOrDefaultAsync(cancellationToken);
        }

        public long GetCount(Expression<Func<TEntity, bool>> expression)
        {
            return _dbSet.LongCount(expression);
        }

        public long GetCount()
        {
            return _dbSet.LongCount();
        }

        public Task<long> GetCountAsync(Expression<Func<TEntity, bool>> expression, CancellationToken cancellationToken = default)
        {
            return _dbSet.LongCountAsync(expression, cancellationToken);
        }

        public Task<long> GetCountAsync(CancellationToken cancellationToken = default)
        {
            return _dbSet.LongCountAsync(cancellationToken);
        }


        public (IEnumerable<TEntity> DataEnumerable, int Total) PageFind(int pageIndex, int pageSize, Expression<Func<TEntity, bool>> expression)
        {
            if (pageIndex < 0)
            {
                throw new ArgumentException("InvalidPageIndex");
            }

            if (pageSize <= 0)
            {
                throw new ArgumentException("InvalidPageCount");
            }

            var query = _dbSet.AsQueryable();
            if (expression != null)
            {
                query = query.Where(expression);
            }

            var total = query.Count();
            var list = query.Skip((pageIndex - 1) * pageSize).Take(pageSize).ToList();
            return (list, total);
        }

        public (IEnumerable<TEntity> DataEnumerable, int Total) PageFind(int pageIndex, int pageSize, IQueryable<TEntity> queryable)
        {
            if (pageIndex < 0)
            {
                throw new ArgumentException("InvalidPageIndex");
            }

            if (pageSize <= 0)
            {
                throw new ArgumentException("InvalidPageCount");
            }
            var total = queryable.Count();
            var list = queryable.Skip((pageIndex - 1) * pageSize).Take(pageSize).ToList();
            return (list, total);
        }

        public async Task<(IEnumerable<TEntity> DataEnumerable, int Total)> PageFindAsync(int pageIndex, int pageSize, Expression<Func<TEntity, bool>> expression, CancellationToken cancellationToken = default)
        {
            if (pageIndex < 0)
            {
                throw new ArgumentException("InvalidPageIndex");
            }

            if (pageSize <= 0)
            {
                throw new ArgumentException("InvalidPageCount");
            }
            var query = _dbSet.AsQueryable();
            if (expression != null)
            {
                query = query.Where(expression);
            }
            var total = await query.CountAsync(cancellationToken);
            var list = await query.Skip((pageIndex - 1) * pageSize).Take(pageSize).ToListAsync(cancellationToken);
            return await Task.FromResult((list, total));
        }

        public async Task<(IEnumerable<TEntity> DataEnumerable, int Total)> PageFindAsync(int pageIndex, int pageSize, IQueryable<TEntity> queryable, CancellationToken cancellationToken = default)
        {
            if (pageIndex < 0)
            {
                throw new ArgumentException("InvalidPageIndex");
            }

            if (pageSize <= 0)
            {
                throw new ArgumentException("InvalidPageCount");
            }
            var total = await queryable.CountAsync(cancellationToken);
            var list = await queryable.Skip((pageIndex - 1) * pageSize).Take(pageSize).ToListAsync(cancellationToken);
            return await Task.FromResult((list, total));
        }

        public void Reload(TEntity entity)
        {
            _dbContext.Entry(entity).Reload();
        }

        public Task ReloadAsync(TEntity entity, CancellationToken cancellationToken = default)
        {
            return _dbContext.Entry(entity)
                 .ReloadAsync(cancellationToken);
        }


        #endregion

        #region insert

        public void Insert(TEntity entity, bool autoSave = false)
        {
            _dbSet.Add(entity);
            if (autoSave)
            {
                _dbContext.SaveChanges();
            }
        }

        public async Task InsertAsync(TEntity entity, bool autoSave = false, CancellationToken cancellationToken = default)
        {
            CheckAndSetId(entity);

            //var savedEntity = (await _dbContext.Set<TEntity>().AddAsync(entity, cancellationToken)).Entity;
            await _dbContext.Set<TEntity>().AddAsync(entity, cancellationToken);
            if (autoSave)
            {
                await _dbContext.SaveChangesAsync(cancellationToken);
            }
        }

        public TEntity InsertAndGetEntity(TEntity entity, bool autoSave = false)
        {
            CheckAndSetId(entity);

            var savedEntity = (_dbContext.Set<TEntity>().Add(entity)).Entity;

            if (autoSave)
            {
                _dbContext.SaveChanges();
            }

            return savedEntity;
        }

        public async Task<TEntity> InsertAndGetEntityAsync(TEntity entity, bool autoSave = false, CancellationToken cancellationToken = default)
        {
            CheckAndSetId(entity);

            var savedEntity = (await _dbContext.Set<TEntity>().AddAsync(entity, cancellationToken)).Entity;

            if (autoSave)
            {
                await _dbContext.SaveChangesAsync(cancellationToken);
            }

            return savedEntity;
        }



        public void InsertMany(IEnumerable<TEntity> entities, bool autoSave = false)
        {
            var entityArray = entities.ToArray();
            foreach (var entity in entityArray)
            {
                CheckAndSetId(entity);
            }

            //if (BulkOperationProvider != null)
            //{
            //    await BulkOperationProvider.InsertManyAsync<TDbContext, TEntity>(
            //        this,
            //        entityArray,
            //        autoSave,
            //        GetCancellationToken(cancellationToken)
            //    );
            //    return;
            //}

            _dbContext.Set<TEntity>().AddRange(entityArray);

            if (autoSave)
            {
                _dbContext.SaveChanges();
            }
        }

        public async Task InsertManyAsync(IEnumerable<TEntity> entities, bool autoSave = false, CancellationToken cancellationToken = default)
        {
            var entityArray = entities.ToArray();
            foreach (var entity in entityArray)
            {
                CheckAndSetId(entity);
            }

            //if (BulkOperationProvider != null)
            //{
            //    await BulkOperationProvider.InsertManyAsync<TDbContext, TEntity>(
            //        this,
            //        entityArray,
            //        autoSave,
            //        GetCancellationToken(cancellationToken)
            //    );
            //    return;
            //}

            await _dbContext.Set<TEntity>().AddRangeAsync(entityArray, cancellationToken);

            if (autoSave)
            {
                await _dbContext.SaveChangesAsync(cancellationToken);
            }
        }

        #endregion

        #region update

        public void Update(TEntity entity, bool autoSave = false)
        {
            //var entry = _dbContext.ChangeTracker.Entries().FirstOrDefault(ent => ent.Entity == entity);
            //if (entry != null)
            //{
            //    return;
            //}

            _dbContext.Attach(entity);

            //_dbContext.Update(entity).Entity;

            if (autoSave)
            {
                _dbContext.SaveChanges();
            }
        }

        public TEntity UpdateAndGetEntity(TEntity entity, bool autoSave = false)
        {
            _dbContext.Attach(entity);

            var updatedEntity = _dbContext.Update(entity).Entity;

            if (autoSave)
            {
                _dbContext.SaveChanges();
            }

            return updatedEntity;
        }

        public async Task<TEntity> UpdateAndGetEntityAsync(TEntity entity, bool autoSave = false, CancellationToken cancellationToken = default)
        {
            _dbContext.Attach(entity);

            var updatedEntity = _dbContext.Update(entity).Entity;

            if (autoSave)
            {
                await _dbContext.SaveChangesAsync(cancellationToken);
            }

            return updatedEntity;
        }

        public async Task UpdateAsync(TEntity entity, bool autoSave = false, CancellationToken cancellationToken = default)
        {
            _dbContext.Attach(entity);

            var updatedEntity = _dbContext.Update(entity).Entity;

            if (autoSave)
            {
                await _dbContext.SaveChangesAsync(cancellationToken);
            }
        }

        public void UpdateMany(IEnumerable<TEntity> entities, bool autoSave = false)
        {
            _dbContext.Set<TEntity>().UpdateRange(entities);

            if (autoSave)
            {
                _dbContext.SaveChanges();
            }
        }

        public async Task UpdateManyAsync(IEnumerable<TEntity> entities, bool autoSave = false, CancellationToken cancellationToken = default)
        {
            //if (BulkOperationProvider != null)
            //{
            //    await BulkOperationProvider.UpdateManyAsync<TDbContext, TEntity>(
            //        this,
            //        entities,
            //        autoSave,
            //        GetCancellationToken(cancellationToken)
            //        );

            //    return;
            //}


            _dbContext.Set<TEntity>().UpdateRange(entities);

            if (autoSave)
            {
                await _dbContext.SaveChangesAsync(cancellationToken);
            }
        }

        #endregion

        #region delete

        public void Delete(TEntity entity, bool autoSave = false)
        {
            _dbContext.Set<TEntity>().Remove(entity);

            if (autoSave)
            {
                _dbContext.SaveChanges();
            }
        }

        public async Task DeleteAsync(TEntity entity, bool autoSave = false, CancellationToken cancellationToken = default)
        {
            _dbContext.Set<TEntity>().Remove(entity);

            if (autoSave)
            {
                await _dbContext.SaveChangesAsync(cancellationToken);
            }
        }

        public void DeleteMany(IEnumerable<TEntity> entities, bool autoSave = false)
        {
            _dbContext.RemoveRange(entities);

            if (autoSave)
            {
                _dbContext.SaveChanges();
            }
        }

        public async Task DeleteManyAsync(IEnumerable<TEntity> entities, bool autoSave = false, CancellationToken cancellationToken = default)
        {
            _dbContext.RemoveRange(entities);

            if (autoSave)
            {
                await _dbContext.SaveChangesAsync(cancellationToken);
            }
        }

        #endregion


        protected virtual void AttachIfNot(TEntity entity)
        {
            var entry = _dbContext.ChangeTracker.Entries().FirstOrDefault(ent => ent.Entity == entity);
            if (entry != null)
            {
                return;
            }

            _dbSet.Attach(entity);
        }


        protected virtual void CheckAndSetId(TEntity entity)
        {
            if (entity is IEntity<Guid> entityWithGuidId)
            {
                TrySetGuidId(entityWithGuidId);
            }
        }

        protected virtual void TrySetGuidId(IEntity<Guid> entity)
        {
            if (entity.Id != default)
            {
                return;
            }
            entity.Id = GuidGenerator.Create();
            //EntityHelper.TrySetId(
            //    entity,
            //    () => GuidGenerator.Create(),
            //    true
            //);
        }

    }


    public class EfCoreRepository<TDbContext, TEntity, TKey> : EfCoreRepository<TDbContext, TEntity>,
        IEfCoreRepository<TEntity, TKey>, IRepository<TEntity, TKey>
       where TDbContext : DbContext
       where TEntity : class, IEntity<TKey>
    {

        public EfCoreRepository(DbContext dbContext) : base(dbContext)
        {

        }

        public EfCoreRepository(IServiceProvider serviceProvider, IUnitOfWorkManager unitOfWorkManager)
       : base(serviceProvider, unitOfWorkManager)
        {
        }

        public void Delete(TKey id, bool autoSave = false)
        {
            var entity = Find(id);
            if (entity != null)
            {
                Delete(entity, false);
            }

        }

        public async Task DeleteAsync(TKey id, bool autoSave = false, CancellationToken cancellationToken = default)
        {
            var entity = await FindAsync(id, cancellationToken: cancellationToken);
            if (entity == null)
            {
                return;
            }

            await DeleteAsync(entity, autoSave, cancellationToken);
        }

        public void DeleteMany([NotNull] IEnumerable<TKey> ids, bool autoSave = false)
        {
            var entities = GetDbSet().Where(x => ids.Contains(x.Id)).ToList();
            DeleteMany(entities, autoSave);
        }

        public async Task DeleteManyAsync([NotNull] IEnumerable<TKey> ids, bool autoSave = false, CancellationToken cancellationToken = default)
        {
            var entities = await GetDbSet().Where(x => ids.Contains(x.Id)).ToListAsync(cancellationToken);

            await DeleteManyAsync(entities, autoSave, cancellationToken);
        }

        public TEntity Find(TKey key)
        {
            return GetDbSet().FirstOrDefault(p => p.Id.Equals(key));
        }

        public async Task<TEntity> FindAsync(TKey key, CancellationToken cancellationToken = default)
        {
            return await GetDbSet().FirstOrDefaultAsync(p => p.Id.Equals(key));
        }

        public async Task<TEntity> GetAsync(TKey id, CancellationToken cancellationToken = default)
        {
            return await GetDbSet().FirstOrDefaultAsync(p => p.Id.Equals(id));
        }

        public TKey InsertAndGetId(TEntity entity)
        {
            entity = InsertAndGetEntity(entity, true);
            return entity.Id;
        }

        public async Task<TKey> InsertAndGetIdAsync(TEntity entity, CancellationToken cancellationToken = default)
        {
            entity = await InsertAndGetEntityAsync(entity, true, cancellationToken);
            return entity.Id;
        }
    }

}
