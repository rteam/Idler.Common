using System.Collections.Generic;
using Idler.Common.Authorization.Models;
using Idler.Common.Core;

namespace Idler.Common.Authorization.Services;

public interface IRoleAuthorizationService
{
    /// <summary>
    /// 通过Id获取角色
    /// </summary>
    /// <param name="id">Id</param>
    /// <returns></returns>
    APIReturnInfo<CoreRoleValue> SingleCoreRole(string id);

    /// <summary>
    /// 检查Id是否存在
    /// </summary>
    /// <param name="id">Id</param>
    /// <returns></returns>
    bool CheckRoleIdIsExist(string id);

    /// <summary>
    /// 全部角色
    /// </summary>
    /// <returns></returns>
    APIReturnInfo<IList<CoreRoleValue>> AllCoreRole();

    /// <summary>
    /// 创建角色
    /// </summary>
    /// <param name="addModel">要创建的信息</param>
    /// <returns></returns>
    APIReturnInfo<CoreRoleValue> CreateCoreRole(AddRoleModel addModel);

    /// <summary>
    /// 编辑角色
    /// </summary>
    /// <param name="editModel">要编辑的信息</param>
    /// <param name="id">要编辑信息的Id</param>
    /// <returns></returns>
    APIReturnInfo<CoreRoleValue> EditCoreRole(EditRoleModel editModel, string id);

    /// <summary>
    /// 删除角色
    /// </summary>
    /// <param name="id">角色Id</param>
    /// <returns></returns>
    APIReturnInfo<CoreRoleValue> RemoveCoreRole(string id);
    /// <summary>
    /// 给指定角色分配权限
    /// </summary>
    /// <param name="assignPermissionToRoleModel">角色分配权限模型</param>
    /// <param name="roleId">角色Id</param>
    APIReturnInfo<CoreRoleValue> AssignPermissionToRole(
        AssignPermissionToRoleModel assignPermissionToRoleModel,
        string roleId);
}