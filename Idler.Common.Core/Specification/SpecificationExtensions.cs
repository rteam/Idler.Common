using System;
using System.Linq.Expressions;
namespace Idler.Common.Core.Specification
{
    public static class SpecificationExtensions
    {
        /// <summary>
        /// And
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="original">规约对象一</param>
        /// <param name="target">规约对象二</param>
        /// <returns>
        /// 如果规约对象一为null则返回规约对象二
        /// 如果规约对象二也为null，最终将返回null
        /// </returns>
        public static ISpecification<T> And<T>(
            this ISpecification<T> original, Expression<Func<T, bool>> target)
        {
            if (original == null && target == null)
                throw new ArgumentNullException("表达式两侧不能都是null");

            if (original == null)
                return new Specification<T>(target);

            if (target == null)
                return original;

            return new AndSpecification<T>(new Specification<T>(target), original);
        }

        /// <summary>
        /// Or
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="original">规约对象一</param>
        /// <param name="target">规约对象二</param>
        /// <returns>
        /// 如果规约对象一为null则返回规约对象二
        /// 如果规约对象二也为null，最终将返回null
        /// </returns>
        public static ISpecification<T> Or<T>(
            this ISpecification<T> original, Expression<Func<T, bool>> target)
        {
            if (original == null && target == null)
                throw new ArgumentNullException("表达式两侧不能都是null");

            if (original == null)
                return new Specification<T>(target);

            if (target == null)
                return original;

            return new OrSpecification<T>(new Specification<T>(target), original);
        }
    }
}