using Idler.Common.Core.Auditing;
using Idler.Common.Core.Domain;
using Idler.Common.Core.Session;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;

namespace Idler.Common.EntityFrameworkCore
{
    /// <summary>
    /// BCDBContext扩展
    /// </summary>
    public static class CoreDBContextExtensions
    {
        /// <summary>
        /// 应用本框架的特性
        /// </summary>
        public static DbContext ApplyBCDBContextSpecial(this DbContext context, ICoreSession coreSession)
        {
            var entries = context.ChangeTracker.Entries().ToList();
            foreach (var entityItem in entries)
            {
                switch (entityItem.State)
                {
                    case EntityState.Deleted:
                        entityItem.CancelDeleteForSoftDelete(coreSession);
                        break;
                    case EntityState.Added:
                        entityItem
                            .ApplyCreateAudit(coreSession);
                        break;
                    case EntityState.Modified:
                        entityItem.ApplyModifyAudit(coreSession);
                        break;
                }
            }

            return context;
        }

        /// <summary>
        /// 应用删除审计
        /// </summary>
        /// <param name="entityInfo">对象</param>
        /// <param name="coreSession">Session</param>
        /// <returns></returns>
        public static EntityEntry ApplyDeleteAudit(this EntityEntry entityInfo, ICoreSession coreSession)
        {
            if (entityInfo.Entity is IHasDeleteTime)
                entityInfo.Entity.As<IHasDeleteTime>().DeleteTime = DateTime.Now;

            if (!(entityInfo.Entity is IHasDeleteUser))
                return entityInfo;

            entityInfo.Entity.As<IHasDeleteUser>().DeleteUser = coreSession.GetUserNameOrGuest();
            return entityInfo;
        }

        /// <summary>
        /// 应用创建审计
        /// </summary>
        /// <param name="entityInfo">对象</param>
        /// <param name="coreSession">Session</param>
        /// <returns></returns>
        public static EntityEntry ApplyCreateAudit(this EntityEntry entityInfo, ICoreSession coreSession)
        {
            if (entityInfo.Entity is IHasCreateTime)
                entityInfo.Entity.As<IHasCreateTime>().CreateTime = DateTime.Now;

            if (!(entityInfo.Entity is IHasCreateUser))
                return entityInfo;

            entityInfo.Entity.As<IHasCreateUser>().CreateUser = coreSession.GetUserNameOrGuest();

            return entityInfo;
        }

        /// <summary>
        /// 应用修改审计
        /// </summary>
        /// <param name="entityInfo">对象</param>
        /// <param name="coreSession">Session</param>
        /// <returns></returns>
        public static EntityEntry ApplyModifyAudit(this EntityEntry entityInfo, ICoreSession coreSession)
        {
            if (entityInfo.Entity is IHasModifyTime)
                entityInfo.Entity.As<IHasModifyTime>().ModifyTime = DateTime.Now;

            if (!(entityInfo.Entity is IHasModifyUser))
                return entityInfo;

            entityInfo.Entity.As<IHasModifyUser>().ModifyUser = coreSession.GetUserNameOrGuest();

            return entityInfo;
        }

        /// <summary>
        /// 应用软删除
        /// </summary>
        /// <param name="entityInfo">对象</param>
        /// <param name="coreSession">Session</param>
        public static EntityEntry CancelDeleteForSoftDelete(this EntityEntry entityInfo, ICoreSession coreSession)
        {
            if (!(entityInfo.Entity is ISoftDelete))
                return entityInfo;

            entityInfo.Reload();
            entityInfo.State = EntityState.Modified;
            entityInfo.Entity.As<ISoftDelete>().IsDelete = true;
            entityInfo.ApplyDeleteAudit(coreSession);

            return entityInfo;
        }


    }
}
