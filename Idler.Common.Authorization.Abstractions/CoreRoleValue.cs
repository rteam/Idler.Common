using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using Idler.Common.Core.Domain;

namespace Idler.Common.Authorization;

/// <summary>
/// 角色
/// </summary>
public class CoreRoleValue : EntityWithStringNoTrace
{
    /// <summary>
    /// 名称
    /// </summary>
    [StringLength(100)]
    [Required]
    public string Name { get; set; }

    /// <summary>
    /// 大写的名称
    /// </summary>
    [StringLength(100)]
    [Required]
    public string NormalizedRoleName { get; set; }

    /// <summary>
    /// 是否为受限角色
    /// 受限角色只能通过设计界面添加、编辑、删除
    /// </summary>
    public bool IsLimited { get; set; }

    /// <summary>
    /// 描述
    /// </summary>
    [StringLength(150, ErrorMessage = "描述不能超过150个字")]
    public string Description { get; set; }

    /// <summary>
    /// 权限列表
    /// </summary>
    public string PermissionIds { get; set; }

    private HashSet<string> permissionIdList;

    /// <summary>
    /// 所属角色Key
    /// </summary>
    public HashSet<string> PermissionIdList
    {
        get
        {
            if (this.permissionIdList == null)
                this.permissionIdList = this.PermissionIds.IsEmpty()
                    ? new HashSet<string>()
                    : this.PermissionIds.Split(',').ToHashSet();

            return this.permissionIdList;
        }
    }
}