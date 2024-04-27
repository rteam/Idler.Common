using System.ComponentModel.DataAnnotations;

namespace Idler.Common.Authorization.Models;

public abstract class BaseRoleModel
{
    /// <summary>
    /// 名称
    /// </summary>
    [StringLength(100, ErrorMessage = "角色名称不能超过100个字")]
    [Required(ErrorMessage = "名称不能为空")]
    public string Name { get; set; }

    [StringLength(150, ErrorMessage = "描述不能超过150个字")]
    public string Description { get; set; }
}

public class AddRoleModel : BaseRoleModel
{
    /// <summary>
    /// Id
    /// </summary>
    [Required(ErrorMessage = "Id不能为空")]
    [StringLength(196, ErrorMessage = "Id不能超过196个字")]
    public string Id { get; set; }
}

public class EditRoleModel : BaseRoleModel
{
}