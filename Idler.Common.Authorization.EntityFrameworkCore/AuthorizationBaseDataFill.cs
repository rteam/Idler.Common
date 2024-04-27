using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Autofac;
using Idler.Common.Authorization.Config;
using Idler.Common.Core.Config;
using Idler.Common.EntityFrameworkCore;

namespace Idler.Common.Authorization.EntityFrameworkCore;

public class AuthorizationBaseDataFill<TContext>(
    IConfigAccessHelper<AuthorizationBasicData> AuthorizationBasicDataConfigAccessHelper,
    IDbContextFactory DbContextFactory)
    : IStartable
    where TContext : CoreDBContext, IAuthorizationDBContext
{
    public void Start()
    {
        using (var db = DbContextFactory.Get() as TContext)
        {
            this.FillAuthorizationBasicData(db);
        }
    }

    private void FillAuthorizationBasicData(TContext db)
    {
        if (db.Set<CoreRole>().Any())
            return;

        this.FillCoreRole(db);
        this.FillCoreResource(db, null);
        this.FillCorePermission(db, null);
        db.SaveChanges();
        this.SetAssociation(db);
        db.SaveChanges();
    }

    /// <summary>
    /// 填充角色
    /// </summary>
    private void FillCoreRole(TContext db)
    {
        if (this.AuthorizationBasicData.Roles.Count == 0)
            return;

        foreach (CoreRoleValue item in this.AuthorizationBasicData.Roles)
        {
            CoreRole coreRoleInfo = new CoreRole()
            {
                Id = item.Id,
                Name = item.Name,
                Description = item.Description,
                IsLimited = item.IsLimited,
                PermissionIds = item.PermissionIds,
                NormalizedRoleName = item.NormalizedRoleName
            };
            coreRoleInfo = db.Set<CoreRole>().Add(coreRoleInfo).Entity;
        }
    }

    /// <summary>
    /// 填充资源
    /// </summary>
    /// <param name="parentResourceInfo"></param>
    private void FillCoreResource(TContext db, CoreResource parentResourceInfo)
    {
        IList<CoreResourceValue> coreResourceBaseDataInfos = this.AuthorizationBasicData.Resources.Where(t =>
                (parentResourceInfo == null ? string.IsNullOrEmpty(t.ParentId) : t.ParentId == parentResourceInfo.Id))
            .ToList();

        if (coreResourceBaseDataInfos.Count == 0)
            return;

        foreach (CoreResourceValue item in coreResourceBaseDataInfos)
        {
            CoreResource coreResourceInfo = new CoreResource()
            {
                Id = item.Id,
                Name = item.Name,
                Description = item.Description,
                Url = item.Url,
                NormalizedUrl = item.NormalizedUrl,
                IsLimited = item.IsLimited,
                ParentId = item.ParentId,
                Order = item.Order,
                Icon = item.Icon,
                IsMenu = item.IsMenu,
            };

            coreResourceInfo.Parent = parentResourceInfo;
            coreResourceInfo = db.Set<CoreResource>().Add(coreResourceInfo).Entity;
            this.FillCoreResource(db, coreResourceInfo);
        }
    }

    /// <summary>
    /// 填充权限
    /// </summary>
    /// <param name="parentPermissionInfo"></param>
    private void FillCorePermission(TContext db, CorePermission parentPermissionInfo)
    {
        IList<CorePermissionValue> corePermissionBaseDataInfos = this.AuthorizationBasicData.Permissions.Where(t =>
                (parentPermissionInfo == null
                    ? string.IsNullOrEmpty(t.ParentId)
                    : t.ParentId == parentPermissionInfo.Id))
            .ToList();

        if (corePermissionBaseDataInfos.Count == 0)
            return;

        foreach (CorePermissionValue Item in corePermissionBaseDataInfos)
        {
            CorePermission corePermissionInfo = new CorePermission()
            {
                Id = Item.Id,
                Name = Item.Name,
                Description = Item.Description,
                ParentId = Item.ParentId,
                ResourceIds = Item.ResourceIds,
                IsStructure = Item.IsStructure,
                IsMenu = Item.IsMenu
            };
            corePermissionInfo.Parent = parentPermissionInfo;
            corePermissionInfo = db.Set<CorePermission>().Add(corePermissionInfo).Entity;
            this.FillCorePermission(db, corePermissionInfo);
        }
    }

    /// <summary>
    /// 设置关联
    /// </summary>
    private void SetAssociation(TContext db)
    {
        IList<CorePermissionValue> corePermissionBaseDataInfos = this.AuthorizationBasicData.Permissions.ToList();
        IDictionary<string, CorePermission> corePermissionInfos =
            db.Set<CorePermission>().ToDictionary(t => t.Id, t => t);
        IList<CoreResource> coreResourceInfos = db.Set<CoreResource>().ToList();

        foreach (CorePermissionValue Item in corePermissionBaseDataInfos)
        {
            if (!corePermissionInfos.TryGetValue(Item.Id, out CorePermission corePermissionInfo))
                continue;

            coreResourceInfos.Where(t => corePermissionInfo.ResourceIdList.Contains(t.Id)).ForEach(t =>
                corePermissionInfo.PermissionResources.Add(new CorePermissionCoreResource()
                    { PermissionId = Item.Id, ResourceId = t.Id }));
        }
    }

    /// <summary>
    /// 配置对象
    /// </summary>
    private AuthorizationBasicData AuthorizationBasicData => AuthorizationBasicDataConfigAccessHelper.ConfigEntity;
}