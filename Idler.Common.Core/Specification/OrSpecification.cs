using System;
using System.Linq.Expressions;

namespace Idler.Common.Core.Specification
{
    /// <summary>
    /// 逻辑或
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class OrSpecification<T> : CombinationSpecification<T>
    {
        public OrSpecification(ISpecification<T> left, ISpecification<T> right)
            : base(left, right)
        {
            
        }

        /// <summary>
        /// 取得完整表达式
        /// </summary>
        public override Expression<Func<T, bool>> Expressions
        {
            get
            {
                ParameterExpression pe = Expression.Parameter(typeof(T), "Parameters");
                ParameterReplacer pr = new ParameterReplacer(pe);

                BinaryExpression be = Expression.OrElse(
                    pr.Replace(base.Left.Expressions.Body),
                    pr.Replace(base.Right.Expressions.Body
                    ));
                return Expression.Lambda<Func<T, bool>>(be, pe);
            }
        }
    }
}