using System;
using System.Linq.Expressions;

namespace Idler.Common.Core.Specification
{
    /// <summary>
    /// 逻辑与
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class AndSpecification<T> : CombinationSpecification<T>
    {
        public AndSpecification(ISpecification<T> left, ISpecification<T> right)
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
                ParameterExpression parameterExpression = Expression.Parameter(typeof(T), "Parameters");
                ParameterReplacer parameterReplacer = new ParameterReplacer(parameterExpression);

                BinaryExpression binaryExpression = Expression.AndAlso(
                    parameterReplacer.Replace(base.Left.Expressions.Body),
                    parameterReplacer.Replace(base.Right.Expressions.Body
                    ));
                return Expression.Lambda<Func<T, bool>>(binaryExpression, parameterExpression);
            }
        }
    }
}