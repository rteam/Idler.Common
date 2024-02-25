using System;
using System.Collections.Generic;
using System.Linq;

namespace Idler.Common.Core
{
    public class BaseCheckTree
    {
        public BaseCheckTree()
        {
        }

        public BaseCheckTree(string name, string value, bool @checked = false, bool @static = false)
        {
            this.Name = name;
            this.Value = value;
            this.Checked = @checked;
            this.Static = @static;
        }

        /// <summary>
        /// 名称
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// 值
        /// </summary>
        public string Value { get; set; }

        /// <summary>
        /// 是否选中
        /// </summary>
        public bool Checked { get; set; }

        /// <summary>
        /// 静态文本
        /// </summary>
        public bool Static { get; set; }

        /// <summary>
        /// 子节点列表
        /// </summary>
        public IList<BaseCheckTree> Children { get; set; } = new List<BaseCheckTree>();
    }
}