using System.ComponentModel.DataAnnotations;

namespace Idler.Common.Authorization.Models;
public abstract class BasePermissionModel
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
}

public class AddPermissionModel : BasePermissionModel
{
    /// <summary>
    /// Id
    /// </summary>
    [Required(ErrorMessage = "Id不能为空")]
    [StringLength(196, ErrorMessage = "Id不能超过196个字")]
    public string Id { get; set; }
}

public class EditPermissionModel : BasePermissionModel
{
}