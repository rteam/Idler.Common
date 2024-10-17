using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;

namespace Idler.Common.Core
{
    public interface IRepository<TEntity, TKey>
        : IReadonlyRepository<TEntity,TKey> where TEntity : class, Domain.IEntity<TKey>
    {
        #region Create

        /// <summary>
        /// 加入一条
        /// </summary>
        /// <param name="entityInfo">实体信息</param>
        /// <returns></returns>
        TEntity Add(TEntity entityInfo);

        /// <summary>
        /// 加入一条
        /// </summary>
        /// <param name="entityInfo">实体信息</param>
        /// <param name="cancellationToken">取消令牌</param>
        /// <returns></returns>
        Task<TEntity> AddAsync(TEntity entityInfo,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// 批量插入数据
        /// 此方法无法应用上下文
        /// </summary>
        /// <param name="entitys">要插入数据的对象</param>
        void BulkInsert(IEnumerable<TEntity> entitys);

        /// <summary>
        /// 批量插入数据
        /// 此方法无法应用上下文
        /// </summary>
        /// <param name="entitys">要插入数据的对象</param>
        /// <param name="cancellationToken">取消令牌</param>
        Task BulkInsertAsync(IEnumerable<TEntity> entitys,
            CancellationToken cancellationToken = default);

        #endregion
        
        #region 删除

        /// <summary>
        /// 移除一条
        /// </summary>
        /// <param name="id">Id</param>
        TEntity Remove(TKey id);

        /// <summary>
        /// 删除指定条件的信息
        /// </summary>
        /// <param name="where">条件</param>
        void Remove(Expression<Func<TEntity, bool>> where);

        /// <summary>
        /// 移除列表
        /// </summary>
        /// <param name="ids"></param>
        void Remove(TKey[] ids);

        /// <summary>
        /// 删除一条信息
        /// </summary>
        /// <param name="removeInfo"></param>
        TEntity Remove(TEntity removeInfo);

        #endregion

        #region 更新

        /// <summary>
        /// 编辑数据
        /// </summary>
        /// <param name="editInfo">要编辑的信息</param>
        /// <returns></returns>
        void Update(TEntity editInfo);

        /// <summary>
        /// 更新数据
        /// </summary>
        /// <param name="entityInfo">要更新的信息</param>
        /// <param name="cancellationToken">取消令牌</param>
        /// <returns></returns>
        Task<TEntity> UpdateAsync(TEntity entityInfo,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// 更新指定内容
        /// </summary>
        /// <param name="id">要更新信息Id</param>
        /// <param name="updateAction">更新动作</param>
        /// <returns></returns>
        TEntity Update(TKey id, Action<TEntity> updateAction);

        /// <summary>
        /// 更新指定内容
        /// </summary>
        /// <param name="id">要更新信息Id</param>
        /// <param name="updateAction">更新动作</param>
        /// <param name="cancellationToken">取消令牌</param>
        /// <returns></returns>
        Task<TEntity> UpdateAsync(TKey id, Func<TEntity, Task> updateAction,
            CancellationToken cancellationToken = default);

        #endregion
    }
}