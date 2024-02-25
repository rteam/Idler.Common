using System;
using System.Linq.Expressions;

namespace Idler.Common.Core.Specification
{
    public class Specification<T> : BaseSpecification<T>
    {
        /// <summary>
        /// 构造规则
        /// </summary>
        /// <param name="expressions">表达式</param>
        public Specification(Expression<Func<T, bool>> expressions)
        {
            this.Expressions = expressions;
        }

        /// <summary>
        /// 取得完整表达式
        /// </summary>
        public override Expression<Func<T, bool>> Expressions { get; }
    }
}