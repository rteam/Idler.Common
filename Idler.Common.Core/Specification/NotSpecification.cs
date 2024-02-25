using System;
using System.Linq.Expressions;
namespace Idler.Common.Core.Specification
{
    /// <summary>
    /// 非
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class NotSpecification<T> : BaseSpecification<T>
    {
        private ISpecification<T> Other { get; set; }

        public NotSpecification(ISpecification<T> other)
        {
            this.Other = other;
        }

        /// <summary>
        /// 取得完整的表达式
        /// </summary>
        public override Expression<Func<T, bool>> Expressions
        {
            get
            {
                UnaryExpression ue = Expression.Not(this.Other.Expressions.Body);
                return Expression.Lambda<Func<T, bool>>(ue, this.Other.Expressions.Parameters);
            }
        }
    }
}