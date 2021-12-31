using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Vacant.Domain.Entites;
using Vacant.Domain.Extensions;
using Vacant.Domain.LinqExpressions;

namespace Vacant.Domain
{
    public class EfCoreDbContext : DbContext
    {
        //private readonly string _connectionString;

        public IAuditPropertySetter AuditPropertySetter;


        protected virtual Guid? CurrentTenantId => default;//CurrentTenant?.Id;
        protected virtual bool IsSoftDeleteFilterEnabled => true;

        protected virtual bool IsMultiTenantFilterEnabled => false;


        private static MethodInfo ConfigureGlobalFiltersMethodInfo = typeof(EfCoreDbContext).GetMethod(nameof(ConfigureGlobalFilters), BindingFlags.Instance | BindingFlags.NonPublic);

        public IGuidGenerator GuidGenerator { get; set; }

        #region 实体

        //public DbSet<User> Users { get; set; }

        //public DbSet<Role> Roles { get; set; }

        //public DbSet<Permission> Permissions { get; set; }

        //public DbSet<Blog> Blogs { get; set; }

        #endregion



        public EfCoreDbContext(DbContextOptions<EfCoreDbContext> options) : base(options)
        {
            InitializeDbContext();
        }

        #region 直接使用New创建的DbContext

        //public EfCoreDbContext(string connectionString)
        //{
        //    _connectionString = connectionString;
        //    InitializeDbContext();
        //}

        ///// <summary>
        ///// https://docs.microsoft.com/zh-cn/ef/core/dbcontext-configuration/
        ///// </summary>
        ///// <param name="optionsBuilder"></param>
        //protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        //{
        //    //optionsBuilder.UseSqlServer(_connectionString);

        //    //optionsBuilder.UseSqlServer(@"Server=(localdb)\mssqllocaldb;Database=Test");
        //}
        #endregion

        #region OnModelCreating

        /// <summary>
        /// 使用 fluent API 配置模型
        /// 可在派生上下文中替代 OnModelCreating 方法，并使用 ModelBuilder API 来配置模型。
        /// 此配置方法最为有效，并可在不修改实体类的情况下指定配置。
        /// Fluent API 配置具有最高优先级，并将替代约定和数据注释。
        /// </summary>
        /// <param name="modelBuilder"></param>
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            //modelBuilder.Entity<User>().Property(u => u.UserName).IsRequired();
            //分组配置
            //为了减小 OnModelCreating 方法的大小，可以将实体类型的所有配置提取到实现 IEntityTypeConfiguration<TEntity> 的单独类中。
            //然后，只需从 OnModelCreating 调用 Configure 方法。
            //new RoleEntityTypeConfiguration().Configure(modelBuilder.Entity<Role>());
            //可以在给定程序集中应用实现 IEntityTypeConfiguration 的类型中指定的所有配置。
            //modelBuilder.ApplyConfigurationsFromAssembly(typeof(RoleEntityTypeConfiguration).Assembly);

            //也可以使用modelBuilder扩展方法进行每个类的实现

            //使用数据注释来配置模型


            //全局查询筛选器
            //全局查询筛选器是应用于元数据模型（通常为 OnModelCreating）中的实体类型的 LINQ 查询谓词。
            //查询谓词即通常传递给 LINQ Where 查询运算符的布尔表达式。 EF Core 会自动将此类筛选器应用于涉及这些实体类型的任何 LINQ 查询。
            //EF Core 还将其应用于使用 Include 或导航属性进行间接引用的实体类型。 此功能的一些常见应用如下：
            //软删除 - 实体类型定义 IsDeleted 属性。
            //多租户 - 实体类型定义 TenantId 属性。
            //https://docs.microsoft.com/zh-cn/ef/core/querying/filters

            //modelBuilder.Entity<Blog>().HasQueryFilter(b => EF.Property<string>(b, "_tenantId") == _tenantId);
            //modelBuilder.Entity<Post>().HasQueryFilter(p => !p.IsDeleted);
            foreach (var entityType in modelBuilder.Model.GetEntityTypes())
            {
                ConfigureGlobalFiltersMethodInfo
                    .MakeGenericMethod(entityType.ClrType)
                    .Invoke(this, new object[] { modelBuilder, entityType });

                //ConfigureGlobalValueConverterMethodInfo
                //    .MakeGenericMethod(entityType.ClrType)
                //    .Invoke(this, new object[] { modelBuilder, entityType });
            }
        }

        #endregion


        private void InitializeDbContext()
        {
            SetNullsForInjectedProperties();
        }

        private void SetNullsForInjectedProperties()
        {
            GuidGenerator = SequentialGuidGenerator.Instance;
            ClockOptions option = new ClockOptions();
            IOptions<ClockOptions> options = Options.Create(option);
            IClock clock = new Clock(options);
            AuditPropertySetter = new AuditPropertySetter(clock);
            // EventBus = NullEventBus.Instance;
        }



        public override int SaveChanges(bool acceptAllChangesOnSuccess)
        {
            return base.SaveChanges(acceptAllChangesOnSuccess);
        }

        public override Task<int> SaveChangesAsync(bool acceptAllChangesOnSuccess, CancellationToken cancellationToken = default)
        {
            return base.SaveChangesAsync(acceptAllChangesOnSuccess, cancellationToken);
        }


        #region 配置全局过滤器


        /// <summary>
        /// 配置全局过滤器
        /// </summary>
        /// <typeparam name="TEntity"></typeparam>
        /// <param name="modelBuilder"></param>
        /// <param name="entityType"></param>
        protected void ConfigureGlobalFilters<TEntity>(ModelBuilder modelBuilder, IMutableEntityType entityType)
          where TEntity : class
        {
            if (entityType.BaseType == null && ShouldFilterEntity<TEntity>(entityType))
            {
                var filterExpression = CreateFilterExpression<TEntity>();
                if (filterExpression != null)
                {
                    if (entityType.IsKeyless)
                    {
                        modelBuilder.Entity<TEntity>().HasQueryFilter(filterExpression);
                    }
                    else
                    {
                        modelBuilder.Entity<TEntity>().HasQueryFilter(filterExpression);
                    }
                }
            }
        }

        /// <summary>
        /// 应该过滤实体
        /// </summary>
        /// <typeparam name="TEntity"></typeparam>
        /// <param name="entityType"></param>
        /// <returns></returns>
        protected virtual bool ShouldFilterEntity<TEntity>(IMutableEntityType entityType) where TEntity : class
        {
            if (typeof(ISoftDelete).IsAssignableFrom(typeof(TEntity)))
            {
                return true;
            }

            if (typeof(IMultiTenant).IsAssignableFrom(typeof(TEntity)))
            {
                return true;
            }



            return false;
        }

        /// <summary>
        /// 创建过滤器表达式
        /// </summary>
        /// <typeparam name="TEntity"></typeparam>
        /// <returns></returns>
        protected virtual Expression<Func<TEntity, bool>> CreateFilterExpression<TEntity>()
           where TEntity : class
        {
            Expression<Func<TEntity, bool>> expression = null;

            if (typeof(ISoftDelete).IsAssignableFrom(typeof(TEntity)))
            {
                Expression<Func<TEntity, bool>> softDeleteFilter = e => !IsSoftDeleteFilterEnabled || !((ISoftDelete)e).IsDeleted;
                expression = expression == null ? softDeleteFilter : CombineExpressions(expression, softDeleteFilter);
            }

            if (typeof(IMultiTenant).IsAssignableFrom(typeof(TEntity)))
            {
                Expression<Func<TEntity, bool>> mayHaveTenantFilter = e => !IsMultiTenantFilterEnabled || ((IMultiTenant)e).TenantId == CurrentTenantId;
                expression = expression == null ? mayHaveTenantFilter : CombineExpressions(expression, mayHaveTenantFilter);
            }

            return expression;
        }

        ///组合表达式
        protected virtual Expression<Func<T, bool>> CombineExpressions<T>(Expression<Func<T, bool>> expression1, Expression<Func<T, bool>> expression2)
        {
            return ExpressionCombiner.Combine(expression1, expression2);
        }

        #endregion

        #region 保存之前，执行的操作记录

        public override int SaveChanges()
        {
            try
            {
                ApplyConcepts();
                return base.SaveChanges();
            }
            catch (DbUpdateConcurrencyException ex)
            {
                throw new Exception(ex.Message, ex);
            }

        }

        public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            try
            {
                ApplyConcepts();
                return base.SaveChangesAsync(cancellationToken);
            }
            catch (DbUpdateConcurrencyException ex)
            {
                throw new Exception(ex.Message, ex);
            }

        }

        /// <summary>
        /// 应用概念
        /// </summary>
        protected virtual void ApplyConcepts()
        {
            foreach (var entry in ChangeTracker.Entries().ToList())
            {
                if (entry.State != EntityState.Modified && entry.CheckOwnedEntityChange())
                {
                    Entry(entry.Entity).State = EntityState.Modified;
                }
                ApplyConcepts(entry);
            }
        }

        /// <summary>
        /// 应用概念
        /// </summary>
        /// <param name="entry"></param>
        protected virtual void ApplyConcepts(EntityEntry entry)
        {
            switch (entry.State)
            {
                case EntityState.Added:
                    ApplyConceptsForAddedEntity(entry);
                    break;
                case EntityState.Modified:
                    ApplyConceptsForModifiedEntity(entry);
                    break;
                case EntityState.Deleted:
                    ApplyConceptsForDeletedEntity(entry);
                    break;
            }

            //AddDomainEvents(changeReport.DomainEvents, entry.Entity);
        }

        /// <summary>
        /// 为添加的实体应用概念
        /// </summary>
        /// <param name="entry"></param>
        protected virtual void ApplyConceptsForAddedEntity(EntityEntry entry)
        {
            CheckAndSetId(entry);
            //SetConcurrencyStampIfNull(entry);
            SetCreationAuditProperties(entry);
        }

        /// <summary>
        /// 为修改后的实体应用概念
        /// </summary>
        /// <param name="entry"></param>
        protected virtual void ApplyConceptsForModifiedEntity(EntityEntry entry)
        {
            if (entry.State == EntityState.Modified && entry.Properties.Any(x => x.IsModified && x.Metadata.ValueGenerated == ValueGenerated.Never))
            {
                SetModificationAuditProperties(entry);

                if (entry.Entity is ISoftDelete && entry.Entity.As<ISoftDelete>().IsDeleted)
                {
                    SetDeletionAuditProperties(entry);
                }
            }
        }

        /// <summary>
        /// 为删除的实体应用概念
        /// </summary>
        /// <param name="entry"></param>
        protected virtual void ApplyConceptsForDeletedEntity(EntityEntry entry)
        {
            if (!(entry.Entity is ISoftDelete))
            {
                return;
            }

            entry.Reload();
            entry.Entity.As<ISoftDelete>().IsDeleted = true;
            entry.State = EntityState.Modified;
        }

        /// <summary>
        /// 设置创建审计属性
        /// </summary>
        /// <param name="entry"></param>
        protected virtual void SetCreationAuditProperties(EntityEntry entry)
        {
            AuditPropertySetter?.SetCreationProperties(entry.Entity);
        }

        /// <summary>
        /// 设置修改审计属性
        /// </summary>
        /// <param name="entry"></param>
        protected virtual void SetModificationAuditProperties(EntityEntry entry)
        {
            AuditPropertySetter?.SetModificationProperties(entry.Entity);
        }

        /// <summary>
        /// 设置删除审计属性
        /// </summary>
        /// <param name="entry"></param>
        protected virtual void SetDeletionAuditProperties(EntityEntry entry)
        {
            AuditPropertySetter?.SetDeletionProperties(entry.Entity);
        }

        /// <summary>
        /// 检查和设置 ID
        /// </summary>
        /// <param name="entry"></param>
        protected virtual void CheckAndSetId(EntityEntry entry)
        {
            if (entry.Entity is IEntity<Guid> entityWithGuidId)
            {
                TrySetGuidId(entry, entityWithGuidId);
            }
        }

        /// <summary>
        /// 尝试设置 Guid类型Id
        /// </summary>
        /// <param name="entry"></param>
        /// <param name="entity"></param>
        protected virtual void TrySetGuidId(EntityEntry entry, IEntity<Guid> entity)
        {
            if (entity.Id != default)
            {
                return;
            }
            var idProperty = entry.Property("Id");
            if (idProperty != null && idProperty.Metadata.ValueGenerated == ValueGenerated.Never)
            {
                entity.Id = GuidGenerator.Create();
            }
        }

        ///// <summary>
        ///// 设置并发标记如果为空
        ///// </summary>
        ///// <param name="entry"></param>
        //protected virtual void SetConcurrencyStampIfNull(EntityEntry entry)
        //{
        //    var entity = entry.Entity as IHasConcurrencyStamp;
        //    if (entity == null)
        //    {
        //        return;
        //    }

        //    if (entity.ConcurrencyStamp != null)
        //    {
        //        return;
        //    }

        //    entity.ConcurrencyStamp = Guid.NewGuid().ToString("N");
        //}

        #endregion

    }
}
