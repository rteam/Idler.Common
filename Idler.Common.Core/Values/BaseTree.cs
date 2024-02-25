using System.Collections.Generic;

namespace Idler.Common.Core
{
    public class BaseTree<TValue>
    {
        public BaseTree()
        {
        }

        public BaseTree(string text, TValue data)
        {
            this.Text = text;
            this.Data = data;
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
        /// 子节点
        /// </summary>
        public ICollection<BaseTree<TValue>> Children { get; set; } = new List<BaseTree<TValue>>();
    }
}