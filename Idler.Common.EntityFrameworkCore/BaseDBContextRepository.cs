using System.Linq.Expressions;
using Idler.Common.Core;
using Idler.Common.Core.Domain;
using Idler.Common.Core.Specification;
using Microsoft.EntityFrameworkCore;

namespace Idler.Common.EntityFrameworkCore
{
    /// <summary>
    /// 存储器基本对象
    /// </summary>
    /// <typeparam name="TEntity"></typeparam>
    /// <typeparam name="TKey"></typeparam>
    public abstract class BaseDBContextRepository<TEntity, TKey> : IRepository<TEntity, TKey>
        where TEntity : class, IEntity<TKey>
    {
        protected BaseDBContextRepository(IDbContextFactory databaseFactory)
        {
            this.DatabaseFactory = databaseFactory;
            this.DbSet = this.DBContext.Set<TEntity>();
        }

        /// <summary>
        /// 数据工厂
        /// </summary>
        protected IDbContextFactory DatabaseFactory { get; private set; }

        protected DbContext DbContext;

        /// <summary>
        /// 数据库对象
        /// </summary>
        protected DbContext DBContext
        {
            get { return this.DbContext ??= this.DatabaseFactory.Instance(); }
        }

        /// <summary>
        /// 数据集
        /// </summary>
        protected DbSet<TEntity> DbSet { get; }

        #region Create

        /// <summary>
        /// 批量插入数据
        /// 此方法无法应用上下文
        /// </summary>
        /// <param name="entitys">要插入数据的对象</param>
        public virtual void BulkInsert(IEnumerable<TEntity> entitys)
        {
            if (entitys == null || entitys.Count() == 0)
                return;

            this.DbSet.AddRange(entitys);
        }

        /// <summary>
        /// 批量插入数据
        /// 此方法无法应用上下文
        /// </summary>
        /// <param name="entitys">要插入数据的对象</param>
        /// <param name="cancellationToken">取消令牌</param>
        public virtual async Task BulkInsertAsync(IEnumerable<TEntity> entitys,
            CancellationToken cancellationToken = default)
        {
            if (entitys == null || entitys.Count() == 0)
                return;

            await this.DbSet.AddRangeAsync(entitys, cancellationToken).ConfigureAwait(false);
        }

        /// <summary>
        /// 加入一条
        /// </summary>
        /// <param name="entityInfo">加入一条</param>
        /// <returns></returns>
        public virtual TEntity Add(TEntity entityInfo)
        {
            return this.DbSet.Add(entityInfo).Entity;
        }

        /// <summary>
        /// 加入一条
        /// </summary>
        /// <param name="entityInfo">实体信息</param>
        /// <param name="cancellationToken">取消令牌</param>
        /// <returns></returns>
        public virtual async Task<TEntity> AddAsync(TEntity entityInfo,
            CancellationToken cancellationToken = default)
        {
            return (await this.DbSet.AddAsync(entityInfo, cancellationToken).ConfigureAwait(false)).Entity;
        }

        #endregion

        #region Single

        /// <summary>
        /// 取得一条
        /// </summary>
        /// <param name="id">Id</param>
        /// <returns></returns>
        public virtual TEntity Single(TKey id)
        {
            return this.DbSet.Find(id);
        }


        /// <summary>
        /// 根据条件取一条数据
        /// </summary>
        /// <param name="where">查询条件</param>
        /// <returns></returns>
        public virtual TEntity Single(Expression<Func<TEntity, bool>> where)
        {
            return this.DbSet.FirstOrDefault<TEntity>(where);
        }

        /// <summary>
        /// 根据条件取一条数据（忽略过滤器）
        /// </summary>
        /// <param name="where">查询条件</param>
        /// <returns></returns>
        public virtual TEntity SingleIgnoreQueryFilters(Expression<Func<TEntity, bool>> where)
        {
            return this.DbSet.IgnoreQueryFilters().FirstOrDefault<TEntity>(where);
        }

        /// <summary>
        /// 根据条件取一条数据（忽略过滤器）
        /// </summary>
        /// <param name="where">查询条件</param>
        /// <param name="cancellationToken">取消令牌</param>
        /// <returns></returns>
        public virtual async Task<TEntity> SingleAsyncIgnoreQueryFilters(Expression<Func<TEntity, bool>> where,
            CancellationToken cancellationToken = default)
        {
            return await this.DbSet.FirstOrDefaultAsync(where, cancellationToken).ConfigureAwait(false);
        }

        /// <summary>
        /// 取一条
        /// </summary>
        /// <param name="id">id</param>
        /// <param name="cancellationToken">取消令牌</param>
        /// <returns></returns>
        public virtual async Task<TEntity> SingleAsync(TKey id,
            CancellationToken cancellationToken = default)
        {
            return await this.DbSet.FindAsync(id, cancellationToken).ConfigureAwait(false);
        }

        /// <summary>
        /// 根据条件取一条数据
        /// </summary>
        /// <param name="where">查询条件</param>
        /// <param name="cancellationToken">取消令牌</param>
        /// <returns></returns>
        public virtual async Task<TEntity> SingleAsync(
            Expression<Func<TEntity, bool>> where,
            CancellationToken cancellationToken = default)
        {
            return await this.DbSet.FirstOrDefaultAsync(where, cancellationToken).ConfigureAwait(false);
        }

        #endregion

        #region 删除

        /// <summary>
        /// 删除指定条件的信息
        /// </summary>
        /// <param name="where">删除条件</param>
        public virtual void Remove(Expression<Func<TEntity, bool>> where)
        {
            if (where == null) //不能无条件删除所有信息
                return;

            this.DbSet.RemoveRange(this.DbSet.Where(where));
        }

        /// <summary>
        /// 移除一条
        /// </summary>
        /// <param name="id">要删除信息的Id</param>
        public virtual TEntity Remove(TKey id)
        {
            TEntity m = this.Single(id);
            if (m == null)
                return null;

            var entity = this.DbSet.Remove(m);

            return entity.Entity;
        }

        /// <summary>
        /// 移除多条数据
        /// </summary>
        /// <param name="ids">要移除的Id列表</param>
        public virtual void Remove(TKey[] ids)
        {
            IEnumerable<TEntity> items = this.DbSet.Where(t => ids.Contains(t.Id)).AsEnumerable<TEntity>();
            this.DbSet.RemoveRange(items);
        }

        /// <summary>
        /// 删除信息
        /// </summary>
        /// <param name="removeInfo">要删除的信息</param>
        /// <returns></returns>
        public virtual TEntity Remove(TEntity removeInfo)
        {
            if (removeInfo == null)
                return null;

            var entity = this.DbSet.Remove(removeInfo);
            return entity.Entity;
        }

        #endregion

        #region 更新

        /// <summary>
        /// 编辑数据
        /// </summary>
        /// <param name="editInfo">要编辑的信息</param>
        /// <returns></returns>
        public virtual void Update(TEntity editInfo)
        {
            if (editInfo == null)
                return;

            this.AttachIfNot(editInfo);
            this.DBContext.Entry<TEntity>(editInfo).State = EntityState.Modified;
        }

        /// <summary>
        /// 更新数据
        /// </summary>
        /// <param name="entityInfo">要更新的信息</param>
        /// <param name="cancellationToken">取消令牌</param>
        /// <returns></returns>
        public virtual async Task<TEntity> UpdateAsync(TEntity entityInfo,
            CancellationToken cancellationToken = default)
        {
            this.AttachIfNot(entityInfo);
            this.DBContext.Entry<TEntity>(entityInfo).State = EntityState.Modified;
            return entityInfo;
        }

        /// <summary>
        /// 更新指定内容
        /// </summary>
        /// <param name="id">要更新信息Id</param>
        /// <param name="updateAction">更新动作</param>
        /// <returns></returns>
        public virtual TEntity Update(TKey id, Action<TEntity> updateAction)
        {
            TEntity entityInfo = this.Single(id);
            if (entityInfo == null)
                return entityInfo;

            updateAction(entityInfo);
            return entityInfo;
        }

        /// <summary>
        /// 更新指定内容
        /// </summary>
        /// <param name="id">要更新信息Id</param>
        /// <param name="updateAction">更新动作</param>
        /// <param name="cancellationToken">取消令牌</param>
        /// <returns></returns>
        public virtual async Task<TEntity> UpdateAsync(TKey id, Func<TEntity, Task> updateAction,
            CancellationToken cancellationToken = default)
        {
            TEntity entityInfo = await this.SingleAsync(id).ConfigureAwait(false);
            await updateAction(entityInfo).ConfigureAwait(false);
            return entityInfo;
        }

        #endregion

        #region Query

        /// <summary>
        /// 按规约返回IQueryable
        /// </summary>
        /// <param name="specification">规约</param>
        /// <returns></returns>
        public virtual IQueryable<TEntity> Specification(ISpecification<TEntity> specification)
        {
            if (specification == null)
                return this.All();

            return this.DbSet.Where(specification.Expressions);
        }

        /// <summary>
        /// 按规约返回IQueryable（忽略过滤器）
        /// </summary>
        /// <param name="specification">规约</param>
        /// <returns></returns>
        public virtual IQueryable<TEntity> SpecificationIgnoreQueryFilters(ISpecification<TEntity> specification)
        {
            if (specification == null)
                return this.All();

            return this.DbSet.IgnoreQueryFilters().Where(specification.Expressions);
        }

        /// <summary>
        /// 返回全部信息
        /// </summary>
        /// <returns></returns>
        public virtual IQueryable<TEntity> All()
        {
            return this.DbSet.AsQueryable<TEntity>();
        }

        /// <summary>
        /// 返回指定包含的全部信息
        /// </summary>
        /// <param name="propertySelectors">返回字段</param>
        /// <returns></returns>
        public virtual IQueryable<TEntity> AllIncluding(params Expression<Func<TEntity, object>>[] propertySelectors)
        {
            if (propertySelectors == null || propertySelectors.Length == 0)
                return this.All();

            var query = this.All();

            foreach (var propertySelector in propertySelectors)
                query = query.Include(propertySelector);

            return query;
        }

        /// <summary>
        /// 返回全部信息（无跟踪功能）
        /// </summary>
        /// <returns></returns>
        public virtual IQueryable<TEntity> AllAsNoTracking()
        {
            return this.DbSet.AsNoTracking<TEntity>();
        }

        /// <summary>
        /// 按条件返回包含指定子集的IQueryable
        /// </summary>
        /// <param name="where">查询条件</param>
        /// <param name="propertySelectors">返回字段</param>
        /// <returns></returns>
        public virtual IQueryable<TEntity> FindIncluding(
            Expression<Func<TEntity, bool>> where,
            params Expression<Func<TEntity, object>>[] propertySelectors
        )
        {
            if (propertySelectors == null || propertySelectors.Length == 0)
                return this.Find(where);

            var query = this.Find(where);

            foreach (var propertySelector in propertySelectors)
                query = query.Include(propertySelector);

            return query;
        }

        /// <summary>
        /// 按条件返回IQueryable
        /// </summary>
        /// <param name="where">查询条件</param>
        /// <returns></returns>
        public virtual IQueryable<TEntity> Find(Expression<Func<TEntity, bool>> where)
        {
            return where == null ? this.DbSet.AsQueryable<TEntity>() : this.DbSet.Where(where);
        }

        /// <summary>
        /// 按条件查询并返回无跟踪功能的对象
        /// </summary>
        /// <param name="where">查询条件</param>
        /// <returns></returns>
        public virtual IQueryable<TEntity> FindAsNoTracking(Expression<Func<TEntity, bool>> where)
        {
            return this.Find(where).AsNoTracking<TEntity>();
        }


        /// <summary>
        /// 是否存在
        /// </summary>
        /// <param name="where">查询条件</param>
        /// <returns></returns>
        public virtual bool IsExist(Expression<Func<TEntity, bool>> where)
        {
            return this.DbSet.Count(where) > 0;
        }

        /// <summary>
        /// 返回全部信息（忽略过滤器）
        /// </summary>
        /// <returns></returns>
        public virtual IQueryable<TEntity> AllIgnoreQueryFilters()
        {
            return this.DbSet.IgnoreQueryFilters().AsQueryable<TEntity>();
        }

        /// <summary>
        /// 返回指定包含的全部信息（忽略过滤器）
        /// </summary>
        /// <param name="propertySelectors">返回字段</param>
        /// <returns></returns>
        public virtual IQueryable<TEntity> AllIncludingIgnoreQueryFilters(
            params Expression<Func<TEntity, object>>[] propertySelectors)
        {
            if (propertySelectors == null || propertySelectors.Length == 0)
                return this.AllIgnoreQueryFilters();

            var query = this.AllIgnoreQueryFilters();

            foreach (var propertySelector in propertySelectors)
                query = query.Include(propertySelector);

            return query;
        }

        /// <summary>
        /// 按条件返回包含指定子集的IQueryable（忽略过滤器）
        /// </summary>
        /// <param name="where">查询条件</param>
        /// <param name="propertySelectors">返回字段</param>
        /// <returns></returns>
        public virtual IQueryable<TEntity> FindIncludingIgnoreQueryFilters(Expression<Func<TEntity, bool>> where,
            params Expression<Func<TEntity, object>>[] propertySelectors)
        {
            if (propertySelectors == null || propertySelectors.Length == 0)
                return this.FindIgnoreQueryFilters(where);

            var query = this.FindIgnoreQueryFilters(where);

            foreach (var propertySelector in propertySelectors)
                query = query.Include(propertySelector);

            return query;
        }


        /// <summary>
        /// 返回全部信息（忽略过滤器）
        /// </summary>
        /// <param name="cancellationToken">取消令牌</param>
        /// <returns></returns>
        public virtual async Task<List<TEntity>> AllAsyncIgnoreQueryFilters(
            CancellationToken cancellationToken = default)
        {
            return await this.DbSet.IgnoreQueryFilters().AsQueryable().ToListAsync(cancellationToken)
                .ConfigureAwait(false);
        }

        /// <summary>
        /// 按条件返回IQueryable（忽略过滤器）
        /// </summary>
        /// <param name="where">查询条件</param>
        /// <param name="cancellationToken">取消令牌</param>
        /// <returns></returns>
        public virtual async Task<List<TEntity>> FindAsyncIgnoreQueryFilters(
            Expression<Func<TEntity, bool>> where,
            CancellationToken cancellationToken = default
        )
        {
            return await this.Find(where).ToListAsync(cancellationToken).ConfigureAwait(false);
        }

        /// <summary>
        /// 是否存在（按条件查询）（忽略过滤器）
        /// </summary>
        /// <param name="where">查询条件</param>
        /// <param name="cancellationToken">取消令牌</param>
        /// <returns></returns>
        public virtual async Task<bool> IsExistAsyncIgnoreQueryFilters(
            Expression<Func<TEntity, bool>> where,
            CancellationToken cancellationToken = default
        )
        {
            return await this.DbSet.IgnoreQueryFilters().CountAsync(where, cancellationToken).ConfigureAwait(false) > 0;
        }


        /// <summary>
        /// 按条件返回IQueryable（忽略过滤器）
        /// </summary>
        /// <param name="where">查询条件</param>
        /// <returns></returns>
        public virtual IQueryable<TEntity> FindIgnoreQueryFilters(Expression<Func<TEntity, bool>> where)
        {
            return where == null
                ? this.DbSet.IgnoreQueryFilters().AsQueryable<TEntity>()
                : this.DbSet.IgnoreQueryFilters().Where(where);
        }

        /// <summary>
        /// 是否存在（忽略过滤器）
        /// </summary>
        /// <param name="where">查询条件</param>
        /// <returns></returns>
        public virtual bool IsExistIgnoreQueryFilters(Expression<Func<TEntity, bool>> where)
        {
            return this.DbSet.IgnoreQueryFilters().Count(where) > 0;
        }

        /// <summary>
        /// 返回全部信息
        /// </summary>
        /// <param name="cancellationToken">取消令牌</param>
        /// <returns></returns>
        public virtual async Task<List<TEntity>> AllAsync(CancellationToken cancellationToken = default)
        {
            return await this.DbSet.ToListAsync(cancellationToken).ConfigureAwait(false);
        }

        /// <summary>
        /// 按条件返回IQueryable
        /// </summary>
        /// <param name="where">查询条件</param>
        /// <param name="cancellationToken">取消令牌</param>
        /// <returns></returns>
        public virtual async Task<List<TEntity>> FindAsync(Expression<Func<TEntity, bool>> where,
            CancellationToken cancellationToken = default)
        {
            return await this.Find(where).ToListAsync(cancellationToken).ConfigureAwait(false);
        }

        /// <summary>
        /// 是否存在（按条件查询）
        /// </summary>
        /// <param name="where">查询条件</param>
        /// <param name="cancellationToken">取消令牌</param>
        /// <returns></returns>
        public virtual async Task<bool> IsExistAsync(
            Expression<Func<TEntity, bool>> where,
            CancellationToken cancellationToken = default)
        {
            return await this.DbSet.CountAsync(where).ConfigureAwait(false) > 0;
        }

        #endregion

        #region Count

        /// <summary>
        /// 总数量
        /// </summary>
        /// <returns></returns>
        public virtual int Count()
        {
            return this.DbSet.Count();
        }

        /// <summary>
        /// 按条件统计数量
        /// </summary>
        /// <param name="where">查询条件</param>
        /// <returns></returns>
        public virtual int Count(Expression<Func<TEntity, bool>> where)
        {
            return where == null ? this.DbSet.Count() : this.DbSet.Count(where);
        }


        /// <summary>
        /// 总数量（忽略过滤器）
        /// </summary>
        /// <returns></returns>
        public virtual int CountIgnoreQueryFilters()
        {
            return this.DbSet.IgnoreQueryFilters().Count();
        }

        /// <summary>
        /// 按条件统计数量（忽略过滤器）
        /// </summary>
        /// <param name="where">查询条件</param>
        /// <returns></returns>
        public virtual int CountIgnoreQueryFilters(Expression<Func<TEntity, bool>> where)
        {
            return where == null
                ? this.DbSet.IgnoreQueryFilters().Count()
                : this.DbSet.IgnoreQueryFilters().Count(where);
        }

        /// <summary>
        /// 总数量
        /// </summary>
        /// <param name="cancellationToken">取消令牌</param>
        /// <returns></returns>
        public virtual async Task<int> CountAsync(CancellationToken cancellationToken = default)
        {
            return await this.DbSet.CountAsync(cancellationToken).ConfigureAwait(false);
        }

        /// <summary>
        /// 按条件统计数量
        /// </summary>
        /// <param name="where">查询条件</param>
        /// <param name="cancellationToken">取消令牌</param>
        /// <returns></returns>
        public virtual async Task<int> CountAsync(Expression<Func<TEntity, bool>> where,
            CancellationToken cancellationToken = default)
        {
            return where == null
                ? await this.CountAsync(cancellationToken).ConfigureAwait(false)
                : await this.DbSet.CountAsync(where, cancellationToken).ConfigureAwait(false);
        }

        /// <summary>
        /// 总数量（忽略过滤器）
        /// </summary>
        /// <param name="cancellationToken">取消令牌</param>
        /// <returns></returns>
        public virtual async Task<int> CountAsyncIgnoreQueryFilters(CancellationToken cancellationToken = default)
        {
            return await this.DbSet.IgnoreQueryFilters().CountAsync(cancellationToken).ConfigureAwait(false);
        }

        /// <summary>
        /// 按条件统计数量（忽略过滤器）
        /// </summary>
        /// <param name="where">查询条件</param>
        /// <returns></returns>
        public virtual async Task<int> CountAsyncIgnoreQueryFilters(Expression<Func<TEntity, bool>> where,
            CancellationToken cancellationToken = default)
        {
            return where == null
                ? await this.DbSet.IgnoreQueryFilters().CountAsync(cancellationToken).ConfigureAwait(false)
                : await this.DbSet.IgnoreQueryFilters().CountAsync(where, cancellationToken).ConfigureAwait(false);
        }

        #endregion

        #region IDisposable 成员

        /// <summary>
        /// 释放资源
        /// </summary>
        public void Dispose()
        {
            this.DbContext.Dispose();
        }

        #endregion

        protected virtual void AttachIfNot(TEntity entity)
        {
            if (!this.DbSet.Local.Contains(entity))
            {
                this.DbSet.Attach(entity);
            }
        }
    }
}