using System.Collections.Generic;
using Idler.Common.Authorization.Models;
using Idler.Common.Core;

namespace Idler.Common.Authorization.Services;

public interface IPermissionAuthorizationService
{
    /// <summary>
    /// 通过Id获取权限
    /// </summary>
    /// <param name="Id">Id</param>
    /// <returns></returns>
    APIReturnInfo<CorePermissionValue> SingleCorePermission(string Id);

    /// <summary>
    /// 全部权限
    /// </summary>
    /// <returns></returns>
    APIReturnInfo<IList<CorePermissionValue>> AllCorePermission();

    /// <summary>
    /// 检查Id是否存在
    /// </summary>
    /// <param name="Id">Id</param>
    /// <returns></returns>
    bool CheckPermissionIdIsExist(string Id);

    /// <summary>
    /// 创建权限
    /// </summary>
    /// <param name="AddModel">要创建的信息</param>
    /// <returns></returns>
    APIReturnInfo<CorePermissionValue> CreateCorePermission(AddPermissionModel AddModel);

    /// <summary>
    /// 编辑权限
    /// </summary>
    /// <param name="EditModel">权限</param>
    /// <param name="Id">要编辑的信息</param>
    /// <returns></returns>
    APIReturnInfo<CorePermissionValue> EditCorePermission(EditPermissionModel EditModel, string Id);

    /// <summary>
    /// 删除权限
    /// </summary>
    /// <param name="Id">权限Id</param>
    /// <returns></returns>
    APIReturnInfo<CorePermissionValue> RemoveCorePermission(string Id);
    /// <summary>
    /// 给指定权限关联资源
    /// </summary>
    /// <param name="assignResourceToPermissionModel">权限关联资源模型</param>
    /// <param name="permissionId">权限Id</param>
    APIReturnInfo<CorePermissionValue> AssignResourceToPermission(
        AssignResourceToPermissionModel assignResourceToPermissionModel, string permissionId);
}