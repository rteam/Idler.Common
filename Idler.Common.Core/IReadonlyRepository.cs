using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;
using Idler.Common.Core.Specification;

namespace Idler.Common.Core;

/// <summary>
/// 只读仓库
/// </summary>
/// <typeparam name="TEntity"></typeparam>
/// <typeparam name="TKey"></typeparam>
public interface IReadonlyRepository<TEntity, TKey>
    : IDisposable where TEntity : class, Domain.IEntity<TKey>
{
    #region Single
    /// <summary>
    /// 取一条
    /// </summary>
    /// <param name="id">id</param>
    /// <returns></returns>
    TEntity Single(TKey id);

    /// <summary>
    /// 取一条
    /// </summary>
    /// <param name="id">id</param>
    /// <returns></returns>
    Task<TEntity> SingleAsync(TKey id,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 根据条件取一条数据
    /// </summary>
    /// <param name="where"></param>
    /// <returns></returns>
    TEntity Single(Expression<Func<TEntity, bool>> where);

    /// <summary>
    /// 根据条件取一条数据
    /// </summary>
    /// <param name="where"></param>
    /// <param name="cancellationToken">取消令牌</param>
    /// <returns></returns>
    Task<TEntity> SingleAsync(Expression<Func<TEntity, bool>> where,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 根据条件取一条数据（忽略过滤器）
    /// </summary>
    /// <param name="where"></param>
    /// <param name="cancellationToken">取消令牌</param>
    /// <returns></returns>
    Task<TEntity> SingleAsyncIgnoreQueryFilters(Expression<Func<TEntity, bool>> where,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 按条件查询一条信息（忽略过滤器）
    /// </summary>
    /// <param name="where"></param>
    /// <returns></returns>
    TEntity SingleIgnoreQueryFilters(Expression<Func<TEntity, bool>> where);

    #endregion
    
    #region Query

    /// <summary>
    /// 返回全部信息
    /// </summary>
    /// <returns></returns>
    IQueryable<TEntity> All();

    /// <summary>
    /// 返回指定包含的全部信息
    /// </summary>
    /// <param name="propertySelectors"></param>
    /// <returns></returns>
    IQueryable<TEntity> AllIncluding(params Expression<Func<TEntity, object>>[] propertySelectors);

    /// <summary>
    /// 返回全部信息
    /// </summary>
    /// <param name="cancellationToken">取消令牌</param>
    /// <returns></returns>
    Task<List<TEntity>> AllAsync(
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 返回全部信息（无跟踪功能）
    /// </summary>
    /// <returns></returns>
    IQueryable<TEntity> AllAsNoTracking();

    /// <summary>
    /// 按条件返回IQueryable
    /// </summary>
    /// <param name="where"></param>
    /// <returns></returns>
    IQueryable<TEntity> Find(Expression<Func<TEntity, bool>> where);

    /// <summary>
    /// 按条件返回包含指定子集的IQueryable
    /// </summary>
    /// <param name="where"></param>
    /// <param name="propertySelectors"></param>
    /// <returns></returns>
    IQueryable<TEntity> FindIncluding(Expression<Func<TEntity, bool>> where,
        params Expression<Func<TEntity, object>>[] propertySelectors);

    /// <summary>
    /// 按条件返回IQueryable
    /// </summary>
    /// <param name="where"></param>
    /// <param name="cancellationToken">取消令牌</param>
    /// <returns></returns>
    Task<List<TEntity>> FindAsync(Expression<Func<TEntity, bool>> where,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 按条件查询并返回无跟踪功能的对象
    /// </summary>
    /// <param name="where">查询条件</param>
    /// <returns></returns>
    IQueryable<TEntity> FindAsNoTracking(Expression<Func<TEntity, bool>> where);

    /// <summary>
    /// 是否存在（按条件查询）
    /// </summary>
    /// <param name="where"></param>
    /// <returns></returns>
    bool IsExist(Expression<Func<TEntity, bool>> where);

    /// <summary>
    /// 是否存在（按条件查询）
    /// </summary>
    /// <param name="where"></param>
    /// <param name="cancellationToken">取消令牌</param>
    /// <returns></returns>
    Task<bool> IsExistAsync(Expression<Func<TEntity, bool>> where,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 返回全部信息（忽略过滤器）
    /// </summary>
    /// <returns></returns>
    IQueryable<TEntity> AllIgnoreQueryFilters();

    /// <summary>
    /// 返回指定包含的全部信息（忽略过滤器）
    /// </summary>
    /// <param name="propertySelectors"></param>
    /// <returns></returns>
    IQueryable<TEntity>
        AllIncludingIgnoreQueryFilters(params Expression<Func<TEntity, object>>[] propertySelectors);

    /// <summary>
    /// 返回全部信息（忽略过滤器）
    /// </summary>
    /// <param name="cancellationToken">取消令牌</param>
    /// <returns></returns>
    Task<List<TEntity>> AllAsyncIgnoreQueryFilters(
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 按条件返回IQueryable（忽略过滤器）
    /// </summary>
    /// <param name="where"></param>
    /// <returns></returns>
    IQueryable<TEntity> FindIgnoreQueryFilters(Expression<Func<TEntity, bool>> where);

    /// <summary>
    /// 按条件返回包含指定子集的IQueryable（忽略过滤器）
    /// </summary>
    /// <param name="where"></param>
    /// <param name="propertySelectors"></param>
    /// <returns></returns>
    IQueryable<TEntity> FindIncludingIgnoreQueryFilters(Expression<Func<TEntity, bool>> where,
        params Expression<Func<TEntity, object>>[] propertySelectors);

    /// <summary>
    /// 按条件返回IQueryable（忽略过滤器）
    /// </summary>
    /// <param name="where"></param>
    /// <param name="cancellationToken">取消令牌</param>
    /// <returns></returns>
    Task<List<TEntity>> FindAsyncIgnoreQueryFilters(Expression<Func<TEntity, bool>> where,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 是否存在（按条件查询）（忽略过滤器）
    /// </summary>
    /// <param name="where"></param>
    /// <returns></returns>
    bool IsExistIgnoreQueryFilters(Expression<Func<TEntity, bool>> where);

    /// <summary>
    /// 是否存在（按条件查询）（忽略过滤器）
    /// </summary>
    /// <param name="where"></param>
    /// <param name="cancellationToken">取消令牌</param>
    /// <returns></returns>
    Task<bool> IsExistAsyncIgnoreQueryFilters(Expression<Func<TEntity, bool>> where,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 按规约返回IQueryable
    /// </summary>
    /// <param name="specification">规约</param>
    /// <returns></returns>
    IQueryable<TEntity> Specification(ISpecification<TEntity> specification);

    /// <summary>
    /// 按规约返回IQueryable（忽略过滤器）
    /// </summary>
    /// <param name="specification">规约</param>
    /// <returns></returns>
    IQueryable<TEntity> SpecificationIgnoreQueryFilters(ISpecification<TEntity> specification);

    #endregion

    #region Count

    /// <summary>
    /// 总数量
    /// </summary>
    /// <returns></returns>
    int Count();

    /// <summary>
    /// 总数量
    /// </summary>
    /// <param name="cancellationToken">取消令牌</param>
    /// <returns></returns>
    Task<int> CountAsync(
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 按条件统计数量
    /// </summary>
    /// <param name="where">查询条件</param>
    /// <returns></returns>
    int Count(Expression<Func<TEntity, bool>> where);

    /// <summary>
    /// 按条件统计数量
    /// </summary>
    /// <param name="where">查询条件</param>
    /// <param name="cancellationToken">取消令牌</param>
    /// <returns></returns>
    Task<int> CountAsync(Expression<Func<TEntity, bool>> where,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 总数量（忽略过滤器）
    /// </summary>
    /// <returns></returns>
    int CountIgnoreQueryFilters();

    /// <summary>
    /// 总数量（忽略过滤器）
    /// </summary>
    /// <param name="cancellationToken">取消令牌</param>
    /// <returns></returns>
    Task<int> CountAsyncIgnoreQueryFilters(CancellationToken cancellationToken = default);

    /// <summary>
    /// 按条件统计数量（忽略过滤器）
    /// </summary>
    /// <param name="where">查询条件</param>
    /// <returns></returns>
    int CountIgnoreQueryFilters(Expression<Func<TEntity, bool>> where);

    /// <summary>
    /// 按条件统计数量（忽略过滤器）
    /// </summary>
    /// <param name="where">查询条件</param>
    /// <param name="cancellationToken">取消令牌</param>
    /// <returns></returns>
    Task<int> CountAsyncIgnoreQueryFilters(Expression<Func<TEntity, bool>> where,
        CancellationToken cancellationToken = default);

    #endregion
}