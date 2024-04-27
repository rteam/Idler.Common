using System.Collections.Generic;
using Idler.Common.Authorization.Models;
using Idler.Common.Core;

namespace Idler.Common.Authorization.Services;

public interface IUserRoleAuthorizationService
{
    /// <summary>
    /// 分页返回用户与角色的关联信息
    /// </summary>
    /// <param name="keyword">按名称查询</param>
    /// <param name="pageSize">每页几条数据</param>
    /// <param name="pageNum">第几页</param>
    /// <returns></returns>
    APIReturnInfo<ReturnPaging<CoreUserRoleValue>> CoreUserRolePaging(string keyword, int pageSize, int pageNum);
        
    /// <summary>
    /// 通过用户Id获取用户角色关联
    /// </summary>
    /// <param name="userId"></param>
    /// <returns></returns>
    APIReturnInfo<CoreUserRoleValue> SingleCoreUserRole(string userId);

    /// <summary>
    /// 删除用户角色设置
    /// </summary>
    /// <param name="id">要删除的设置Id</param>
    APIReturnInfo<CoreUserRoleValue> RemoveCoreUserRole(string id);

    /// <summary>
    /// 关联角色到用户
    /// </summary>
    /// <param name="model">关联数据</param>
    /// <param name="UserId">UserId</param>
    /// <returns></returns>
    APIReturnInfo<CoreUserRoleValue> AssignRoleToUser(AssignRoleToUserModel model, string UserId);

    /// <summary>
    /// 取得指定用户的所有角色
    /// </summary>
    /// <param name="userId">UserId</param>
    /// <returns></returns>
    APIReturnInfo<IList<CoreRoleValue>> CoreUserRoles(string userId);
}