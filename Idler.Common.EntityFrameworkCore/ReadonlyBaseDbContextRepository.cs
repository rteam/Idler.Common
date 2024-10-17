using System.Linq.Expressions;
using Idler.Common.Core;
using Idler.Common.Core.Domain;
using Idler.Common.Core.Specification;
using Microsoft.EntityFrameworkCore;

namespace Idler.Common.EntityFrameworkCore;

public abstract class ReadonlyBaseDbContextRepository<TEntity, TKey> : IReadonlyRepository<TEntity, TKey>
    where TEntity : class, IEntity<TKey>
{
    protected ReadonlyBaseDbContextRepository(IDbContextFactory databaseFactory)
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
}