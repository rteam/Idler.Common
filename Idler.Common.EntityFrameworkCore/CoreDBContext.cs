using System.Linq.Expressions;
using System.Reflection;
using Idler.Common.Core;
using Idler.Common.Core.Domain;
using Idler.Common.Core.Session;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;

namespace Idler.Common.EntityFrameworkCore
{
    public abstract class CoreDBContext : DbContext
    {
        private static MethodInfo ConfigureGlobalFiltersMethodInfo = typeof(CoreDBContext).GetMethod(nameof(ConfigureGlobalFilters), BindingFlags.Instance | BindingFlags.NonPublic);

        /// <summary>
        /// Session
        /// </summary>
        public ICoreSession CoreSession { get; set; } = new NullCoreSection();

        /// <summary>
        /// 是否启用软删除过滤器
        /// </summary>
        public virtual bool IsSoftDeleteFilterEnabled { get; set; } = true;

        public CoreDBContext()
        {

        }
        
        public CoreDBContext(DbContextOptions options) : base(options)
        {

        }

        /// <summary>
        /// 保存更改
        /// </summary>
        /// <returns></returns>
        public override int SaveChanges()
        {
            this.ApplyBCDBContextSpecial(this.CoreSession);
            return base.SaveChanges();
        }

        /// <summary>
        /// 当上下文模型创建时
        /// </summary>
        /// <param name="modelBuilder"></param>
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            if (modelBuilder == null)
            {
                throw new ArgumentNullException("modelBuilder");
            }

            base.OnModelCreating(modelBuilder);

            foreach (var entityType in modelBuilder.Model.GetEntityTypes())
            {
                ConfigureGlobalFiltersMethodInfo
                    .MakeGenericMethod(entityType.ClrType)
                    .Invoke(this, new object[] { modelBuilder, entityType });
            }
        }

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

        protected virtual bool ShouldFilterEntity<TEntity>(IMutableEntityType entityType) where TEntity : class
        {
            if (typeof(ISoftDelete).IsAssignableFrom(typeof(TEntity)))
            {
                return true;
            }

            return false;
        }

        protected virtual Expression<Func<TEntity, bool>> CreateFilterExpression<TEntity>()
            where TEntity : class
        {
            Expression<Func<TEntity, bool>> expression = null;

            if (typeof(ISoftDelete).IsAssignableFrom(typeof(TEntity)))
            {
                Expression<Func<TEntity, bool>> softDeleteFilter = e => !IsSoftDeleteFilterEnabled || !((ISoftDelete)e).IsDelete;
                expression = expression == null ? softDeleteFilter : expression.Combine(softDeleteFilter);
            }

            return expression;
        }

    }
}
