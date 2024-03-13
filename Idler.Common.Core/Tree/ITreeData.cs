namespace Idler.Common.Core.Tree;

public interface ITreeData<TIdType>
{
    /// <summary>
    /// Id
    /// </summary>
    TIdType Id { get; }

    /// <summary>
    /// 名称
    /// </summary>
    string Name { get; }

    /// <summary>
    /// 父Id
    /// </summary>
    TIdType ParentId { get; }
}