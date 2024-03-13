using System.Collections.Generic;

namespace Idler.Common.Core.Tree;

/// <summary>
/// Hash树
/// </summary>
/// <typeparam name="TValue"></typeparam>
public class HashTree<TValue>
{
    public HashTree()
    {
    }

    public HashTree(string text, string value, TValue data, bool @checked = false)
    {
        this.Text = text;
        this.Value = value;
        this.Data = data;
        this.Checked = @checked;
    }

    /// <summary>
    /// 显示文本
    /// </summary>
    public string Text { get; set; }

    /// <summary>
    /// 值
    /// </summary>
    public string Value { get; set; }

    /// <summary>
    /// 数据
    /// </summary>
    public TValue Data { get; set; }

    /// <summary>
    /// 是否选中
    /// </summary>
    public bool Checked { get; set; }

    /// <summary>
    /// 子节点
    /// </summary>
    public HashSet<HashTree<TValue>> Children { get; set; } = new HashSet<HashTree<TValue>>();
}