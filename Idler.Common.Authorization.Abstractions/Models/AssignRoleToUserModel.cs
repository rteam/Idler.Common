using System.ComponentModel.DataAnnotations;

namespace Idler.Common.Authorization.Models;

public class AssignRoleToUserModel
{
    /// <summary>
    /// 用户名
    /// </summary>
    [Required]
    [StringLength(50)]
    public string Name { get; set; }

    /// <summary>
    /// 角色Id
    /// </summary>
    public string RoleKeys { get; set; }
}