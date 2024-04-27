using System.ComponentModel.DataAnnotations;
using Idler.Common.Core.Domain;
using Idler.Common.Core.Tree;

namespace Idler.Common.Authorization;

/// <summary>
/// 资源
/// </summary>
public class CoreResourceValue : EntityWithStringNoTrace, ITreeData<string>
{
    public CoreResourceValue()
    {
    }

    public CoreResourceValue(string Id, string Name, string Url, string Description = "")
    {
        this.Id = Id;
        this.Name = Name;
        this.Url = Url;
        this.NormalizedUrl = Url.ToUpper();
        this.Description = Description;
    }

    /// <summary>
    /// 资源名称
    /// </summary>
    [Required]
    [StringLength(150)]
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

    [StringLength(255)] public string Icon { get; set; }

    /// <summary>
    /// 大写的Url
    /// </summary>
    [StringLength(500, ErrorMessage = "Url长度不超过500个字")]
    public string NormalizedUrl { get; set; }

    /// <summary>
    /// 父节点Id
    /// </summary>
    [StringLength(196)]
    public string ParentId { get; set; }
}