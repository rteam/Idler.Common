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
    public abstract class BaseDBContextRepository<TEntity, TKey> : ReadonlyBaseDbContextRepository<TEntity,TKey>, IRepository<TEntity, TKey>
        where TEntity : class, IEntity<TKey>
    {
        protected BaseDBContextRepository(IDbContextFactory databaseFactory):base(databaseFactory)
        {

        }

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

        protected virtual void AttachIfNot(TEntity entity)
        {
            if (!this.DbSet.Local.Contains(entity))
            {
                this.DbSet.Attach(entity);
            }
        }
    }
}