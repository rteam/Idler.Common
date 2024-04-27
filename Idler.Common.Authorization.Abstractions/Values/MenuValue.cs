using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Idler.Common.Authorization.Values;
/// <summary>
/// 菜单
/// </summary>
public class MenuValue
{
    public MenuValue()
    {
    }

    public MenuValue(string title, string url, string icon, bool authorized, int orderBy)
    {
        Title = title;
        Url = url;
        Icon = icon;
        Authorized = authorized;
        OrderBy = orderBy;
    }

    public MenuValue(string title, string url, string icon, int orderBy, IList<MenuValue> children)
    {
        Title = title;
        Url = url;
        Icon = icon;
        OrderBy = orderBy;
        Children = children ?? new List<MenuValue>();
    }

    /// <summary>
    /// 名称
    /// </summary>
    public string Title { get; set; }

    /// <summary>
    /// 地址
    /// </summary>
    public string Url { get; set; }

    /// <summary>
    /// 图标
    /// </summary>
    public string Icon { get; set; }

    /// <summary>
    /// 该地址已授权
    /// </summary>
    public bool Authorized { get; set; }

    /// <summary>
    /// 排序
    /// </summary>
    public int OrderBy { get; set; }

    /// <summary>
    /// 子菜单列表
    /// </summary>
    public IList<MenuValue> Children { get; set; } = new List<MenuValue>();
}