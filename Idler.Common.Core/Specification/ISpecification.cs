using System;
using System.Linq.Expressions;
namespace Idler.Common.Core.Specification
{
    public interface ISpecification<T>
    {
        /// <summary>
        /// 断言
        /// </summary>
        /// <param name="predicate">断言对象</param>
        /// <returns></returns>
        bool IsSatisfiedBy(T predicate);
        /// <summary>
        /// 逻辑与
        /// </summary>
        /// <param name="other">与谁？</param>
        /// <returns>返回组装结果</returns>
        ISpecification<T> And(ISpecification<T> other);
        /// <summary>
        /// 逻辑或
        /// </summary>
        /// <param name="other">或谁</param>
        /// <returns>返回组装结果</returns>
        ISpecification<T> Or(ISpecification<T> other);
        /// <summary>
        /// 逻辑非
        /// </summary>
        /// <returns>返回组装结果</returns>
        ISpecification<T> Not();
        /// <summary>
        /// 取得完整表达式
        /// </summary>
        Expression<Func<T, bool>> Expressions { get; }
    }
}