using System;
using System.Linq.Expressions;

namespace Idler.Common.Core
{
    /// <summary>
    /// 提供用于处理表达式的扩展方法。
    /// </summary>
    public static class ExpressionExtensions
    {
        /// <summary>
        /// 使用And运算符将两个表达式组合成一个新表达式。
        /// </summary>
        /// <typeparam name="T">表达式中的参数类型.</typeparam>
        /// <param name="expression1">被合并的第一个表达式</param>
        /// <param name="expression2">被合并的第二个表达式.</param>
        /// <returns>新的表达式.</returns>
        public static Expression<Func<T, bool>> Combine<T>(this Expression<Func<T, bool>> expression1, Expression<Func<T, bool>> expression2)
        {
            if (expression1 == null && expression2 == null)
            {
                return null;
            }

            if (expression1 == null)
            {
                return expression2;
            }

            if (expression2 == null)
            {
                return expression1;
            }

            var parameter = Expression.Parameter(typeof(T));

            var leftVisitor = new ReplaceExpressionVisitor(expression1.Parameters[0], parameter);
            var left = leftVisitor.Visit(expression1.Body);

            var rightVisitor = new ReplaceExpressionVisitor(expression2.Parameters[0], parameter);
            var right = rightVisitor.Visit(expression2.Body);

            return Expression.Lambda<Func<T, bool>>(Expression.AndAlso(left, right), parameter);
        }

        class ReplaceExpressionVisitor : ExpressionVisitor
        {
            private readonly Expression _oldValue;
            private readonly Expression _newValue;

            public ReplaceExpressionVisitor(Expression oldValue, Expression newValue)
            {
                _oldValue = oldValue;
                _newValue = newValue;
            }

            public override Expression Visit(Expression node)
            {
                if (node == _oldValue)
                {
                    return _newValue;
                }

                return base.Visit(node);
            }
        }
    }
}