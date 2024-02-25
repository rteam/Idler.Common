namespace Idler.Common.Core.Specification
{
    /// <summary>
    /// 组装表达式
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public abstract class CombinationSpecification<T> : BaseSpecification<T>, ICombinationSpecification<T>
    {
        /// <summary>
        /// 组装表达式
        /// </summary>
        /// <param name="left">左侧</param>
        /// <param name="right">右侧</param>
        public CombinationSpecification(ISpecification<T> left, ISpecification<T> right)
        {
            this.Left = left;
            this.Right = right;
        }

        /// <summary>
        /// 左侧表达式
        /// </summary>
        public ISpecification<T> Left { get; protected set; }
        /// <summary>
        /// 右侧表达式
        /// </summary>
        public ISpecification<T> Right { get; protected set; }
    }
}