namespace Idler.Common.Core.Specification
{
    public interface ICombinationSpecification<T> : ISpecification<T>
    {
        /// <summary>
        /// 左侧对象
        /// </summary>
        ISpecification<T> Left { get; }
        /// <summary>
        /// 右侧对象
        /// </summary>
        ISpecification<T> Right { get; }

    }
}