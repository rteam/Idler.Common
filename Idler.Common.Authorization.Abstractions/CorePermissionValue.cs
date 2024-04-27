using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using Idler.Common.Core.Domain;
using Idler.Common.Core.Tree;

namespace Idler.Common.Authorization;

/// <summary>
/// 权限
/// </summary>
public class CorePermissionValue : EntityWithStringNoTrace, ITreeData<string>
{
    /// <summary>
    /// 权限名称
    /// </summary>
    [Required(ErrorMessage = "权限名称必须填写")]
    [StringLength(50, ErrorMessage = "权限名称不能超过50个字")]
    public string Name { get; set; }

    /// <summary>
    /// 描述
    /// </summary>
    [StringLength(150, ErrorMessage = "描述不能超过150个字")]
    public string Description { get; set; }

    /// <summary>
    /// 菜单
    /// </summary>
    public bool IsMenu { get; set; }

    /// <summary>
    /// 该节点只起结构作用
    /// </summary>
    public bool IsStructure { get; set; }

    /// <summary>
    /// 父节点Id
    /// </summary>
    [StringLength(196)]
    public string ParentId { get; set; }

    /// <summary>
    /// 关联的资源Id列表
    /// </summary>
    public string ResourceIds { get; set; }

    private HashSet<string> resourceIdList;

    /// <summary>
    /// 所属角色Key
    /// </summary>
    public HashSet<string> ResourceIdList
    {
        get
        {
            if (this.resourceIdList == null)
                this.resourceIdList = this.ResourceIds.IsEmpty()
                    ? new HashSet<string>()
                    : this.ResourceIds.Split(',').ToHashSet();

            return this.resourceIdList;
        }
    }
}