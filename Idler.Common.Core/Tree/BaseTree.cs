using System.Collections.Generic;

namespace Idler.Common.Core.Tree
{
    public class BaseTree<TValue>
    {
        public BaseTree()
        {
        }

        public BaseTree(string text, string value, TValue data, bool @checked = false)
        {
            this.Text = text;
            this.Data = data;
            this.Value = value;
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
        public ICollection<BaseTree<TValue>> Children { get; set; } = new List<BaseTree<TValue>>();
    }
}