using System.ComponentModel.DataAnnotations;

namespace Idler.Common.Authorization.Models;
public abstract class BaseCoreResourceModel
{
    /// <summary>
    /// 资源名称
    /// </summary>
    [Required(ErrorMessage = "名称不能为空")]
    [StringLength(150, ErrorMessage = "资源名称不能超过150个字")]
    public string Name { get; set; }

    /// <summary>
    /// 是否为菜单
    /// </summary>
    public bool IsMenu { get; set; }

    /// <summary>
    /// 排序
    /// </summary>
    [Required(ErrorMessage = "必须设置排序")]
    public int Order { get; set; }

    /// <summary>
    /// 是否为受限资源
    /// 受限资源只能通过设计界面添加、编辑、删除
    /// </summary>
    public bool IsLimited { get; set; }

    /// <summary>
    /// 描述
    /// </summary>
    [StringLength(150, ErrorMessage = "描述不能超过150个字")]
    public string Description { get; set; }

    /// <summary>
    /// Url
    /// </summary>
    [StringLength(500, ErrorMessage = "Url长度不超过500个字")]
    public string Url { get; set; }

    /// <summary>
    /// 图标
    /// </summary>
    [StringLength(255, ErrorMessage = "图标地址不能超过255个字")]
    public string Icon { get; set; }

    /// <summary>
    /// 父节点Id
    /// </summary>
    [StringLength(196)]
    public string ParentId { get; set; }
}

public class AddCoreResourceModel : BaseCoreResourceModel
{
    /// <summary>
    /// Id
    /// </summary>
    [Required(ErrorMessage = "Id不能为空")]
    [StringLength(196, ErrorMessage = "Id不能超过196个字")]
    public string Id { get; set; }
}

public class EditCoreResourceModel : BaseCoreResourceModel
{
}