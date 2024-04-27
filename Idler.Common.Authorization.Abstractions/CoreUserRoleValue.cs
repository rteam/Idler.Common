using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using Idler.Common.Core.Domain;

namespace Idler.Common.Authorization;

/// <summary>
/// 角色与用户的关联关系
/// </summary>
public class CoreUserRoleValue : EntityWithStringNoTrace
{
    /// <summary>
    /// 用户Id
    /// </summary>
    [Required]
    [StringLength(196)]
    public string UserId { get; set; }

    /// <summary>
    /// 角色Id列表
    /// </summary>
    public string RoleKeys { get; set; }

    /// <summary>
    /// 用户名称
    /// </summary>
    [Required]
    [StringLength(50)]
    public string Name { get; set; }

    private IList<string> roleIdList;

    /// <summary>
    /// 所属角色Key
    /// </summary>
    public IList<string> RoleKeyList
    {
        get
        {
            if (this.roleIdList == null)
                this.roleIdList = this.RoleKeys.IsEmpty() ? new List<string>() : this.RoleKeys.Split(',').ToList();

            return this.roleIdList;
        }
    }

    /// <summary>
    /// 移除角色
    /// </summary>
    /// <param name="RoleKey">角色Key</param>
    /// <returns></returns>
    public bool RemoveRole(string RoleKey)
    {
        IList<string> tRoleIds = this.RoleKeys.Split(',').ToList();
        tRoleIds.Remove(RoleKey);

        this.RoleKeys = string.Join(",", tRoleIds.ToArray());
        return true;
    }

    /// <summary>
    /// 增加角色
    /// </summary>
    /// <param name="RoleKey">角色Key</param>
    /// <returns></returns>
    public bool AddRole(string RoleKey)
    {
        if (this.RoleKeys.MatchString(RoleKey))
            return false;

        this.RoleKeys = this.RoleKeys.IsEmpty() ? RoleKey : string.Concat(this.RoleKeys, ",", RoleKey);
        return true;
    }
}