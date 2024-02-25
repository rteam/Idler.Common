using System;
using System.Linq.Expressions;

namespace Idler.Common.Core.Specification
{
    public abstract class BaseSpecification<T> : ISpecification<T>
    {
        /// <summary>
        /// 断言
        /// </summary>
        /// <param name="predicate">断言对象</param>
        /// <returns>断言结果</returns>
        public virtual bool IsSatisfiedBy(T predicate)
        {
            return this.Expressions.Compile()(predicate);
        }

        /// <summary>
        /// 逻辑与
        /// </summary>
        /// <param name="other">与谁？</param>
        /// <returns>返回组装结果</returns>
        public ISpecification<T> And(ISpecification<T> other)
        {
            if (other == null)
                return this;

            return new AndSpecification<T>(this, other);
        }
        /// <summary>
        /// 逻辑或
        /// </summary>
        /// <param name="other">或谁</param>
        /// <returns>返回组装结果</returns>
        public ISpecification<T> Or(ISpecification<T> other)
        {
            if (other == null)
                return this;

            return new OrSpecification<T>(this, other);
        }
        /// <summary>
        /// 逻辑非
        /// </summary>
        /// <returns>返回组装结果</returns>
        public ISpecification<T> Not()
        {
            return new NotSpecification<T>(this);
        }
        /// <summary>
        /// 取得完整表达式
        /// </summary>
        public abstract Expression<Func<T, bool>> Expressions { get; }
    }
}