using System.Collections.Generic;
using Idler.Common.Authorization.Values;
using Idler.Common.Core;

namespace Idler.Common.Authorization.Services;

public interface IUserPermissionAuthorizationService
{
    /// <summary>
    /// 生成指定角色的菜单
    /// </summary>
    /// <param name="roleIds">角色Id</param>
    /// <returns></returns>
    APIReturnInfo<IList<MenuValue>> BuildMenu(IList<string> roleIds);

    /// <summary>
    /// 生成指定用户的菜单
    /// </summary>
    /// <param name="userId">UserId</param>
    /// <returns></returns>
    APIReturnInfo<IList<MenuValue>> BuildMenu(string userId);

    /// <summary>
    /// 指定角色是否拥有访问指定地址的权限
    /// </summary>
    /// <param name="roleIds">角色Id列表</param>
    /// <param name="url">地址</param>
    /// <returns></returns>
    bool AccessAuthorization(IList<string> roleIds, string url);
}