using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Vacant.Domain.Entites;

namespace Vacant.Domain.Repositories
{
    public interface IRepository
    {

    }

    public interface IRepository<TEntity> : IRepository where TEntity : class, IEntity
    {
        #region Query
        IQueryable<TEntity> GetQueryable();

        long GetCount(Expression<Func<TEntity, bool>> expression);

        Task<long> GetCountAsync(Expression<Func<TEntity, bool>> expression, CancellationToken cancellationToken = default);

        long GetCount();
        Task<long> GetCountAsync(CancellationToken cancellationToken = default);

        TEntity Find(Expression<Func<TEntity, bool>> expression);

        IEnumerable<TEntity> FindAll(Expression<Func<TEntity, bool>> expression);

        Task<TEntity> FindAsync(Expression<Func<TEntity, bool>> expression, CancellationToken cancellationToken = default);

        Task<List<TEntity>> FindAllAsync(Expression<Func<TEntity, bool>> expression, CancellationToken cancellationToken = default);

        bool Exists(Expression<Func<TEntity, bool>> expression);

        Task<bool> ExistsAsync(Expression<Func<TEntity, bool>> expression, CancellationToken cancellationToken = default);



        void Reload(TEntity entity);

        Task ReloadAsync(TEntity entity, CancellationToken cancellationToken = default);

        (IEnumerable<TEntity> DataEnumerable, int Total) PageFind(
            int pageIndex,
            int pageSize,
            Expression<Func<TEntity, bool>> expression);

        Task<(IEnumerable<TEntity> DataEnumerable, int Total)> PageFindAsync(
            int pageIndex,
            int pageSize,
            Expression<Func<TEntity, bool>> expression,
             CancellationToken cancellationToken = default);


        (IEnumerable<TEntity> DataEnumerable, int Total) PageFind(
            int pageIndex,
            int pageSize,
            IQueryable<TEntity> queryable);

        Task<(IEnumerable<TEntity> DataEnumerable, int Total)> PageFindAsync(
            int pageIndex,
            int pageSize,
            IQueryable<TEntity> queryable,
            CancellationToken cancellationToken = default);



        #endregion


        #region Insert

        void Insert(TEntity entity, bool autoSave = false);

        TEntity InsertAndGetEntity(TEntity entity, bool autoSave = false);

        void InsertMany(IEnumerable<TEntity> entities, bool autoSave = false);


        Task InsertAsync(TEntity entity, bool autoSave = false, CancellationToken cancellationToken = default);

        Task<TEntity> InsertAndGetEntityAsync(TEntity entity, bool autoSave = false, CancellationToken cancellationToken = default);

        Task InsertManyAsync(IEnumerable<TEntity> entities, bool autoSave = false, CancellationToken cancellationToken = default);


        #endregion

        #region Update

        void Update(TEntity entity, bool autoSave = false);

        TEntity UpdateAndGetEntity(TEntity entity, bool autoSave = false);

        void UpdateMany(IEnumerable<TEntity> entities, bool autoSave = false);

        Task UpdateAsync(TEntity entity, bool autoSave = false, CancellationToken cancellationToken = default);

        Task<TEntity> UpdateAndGetEntityAsync(TEntity entity, bool autoSave = false, CancellationToken cancellationToken = default);

        Task UpdateManyAsync(IEnumerable<TEntity> entities, bool autoSave = false, CancellationToken cancellationToken = default);

        #endregion


        #region Delete

        void Delete(TEntity entity, bool autoSave = false);

        void DeleteMany(IEnumerable<TEntity> entities, bool autoSave = false);

        Task DeleteAsync(TEntity entity, bool autoSave = false, CancellationToken cancellationToken = default);

        Task DeleteManyAsync(IEnumerable<TEntity> entities, bool autoSave = false, CancellationToken cancellationToken = default);

        #endregion

    }

    public interface IRepository<TEntity, TKey> : IRepository<TEntity>
       where TEntity : class, IEntity<TKey>
    {
        TKey InsertAndGetId(TEntity entity);

        Task<TKey> InsertAndGetIdAsync(TEntity entity, CancellationToken cancellationToken = default);

        void Delete(TKey key, bool autoSave = false);  //TODO: Return true if deleted

        void DeleteMany([NotNull] IEnumerable<TKey> ids, bool autoSave = false);


        Task DeleteAsync(TKey id, bool autoSave = false, CancellationToken cancellationToken = default);  //TODO: Return true if deleted

        Task DeleteManyAsync([NotNull] IEnumerable<TKey> ids, bool autoSave = false, CancellationToken cancellationToken = default);


        TEntity Find(TKey key);

        Task<TEntity> FindAsync(TKey key, CancellationToken cancellationToken = default);

        Task<TEntity> GetAsync(TKey id, CancellationToken cancellationToken = default);
    }

}
