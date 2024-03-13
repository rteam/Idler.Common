using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Idler.Common.Core.Tree;

/// <summary>
/// 树结构对象扩展
/// </summary>
public static class TreeExtensions
{
    /// <summary>
    /// 构建BaseTree
    /// </summary>
    /// <param name="treeData">树形结构数据</param>
    /// <param name="checkedIds">已选中的Id列表</param>
    /// <param name="firstLevelParentId">首层对象的ParentId值</param>
    /// <typeparam name="TIdType">树形结构Id类型</typeparam>
    /// <typeparam name="TValue">树形结构对象类型</typeparam>
    /// <returns></returns>
    public static ICollection<BaseTree<TValue>> BuildBaseTree<TIdType, TValue>(
        this IEnumerable<ITreeData<TIdType>> treeData, string checkedIds = "",
        TIdType firstLevelParentId = default(TIdType))
        where TValue : ITreeData<TIdType>
    {
        var lookup = treeData.ToLookup(x => x.ParentId);
        return BuildBaseTree<TIdType, TValue>(lookup, firstLevelParentId, checkedIds.GetSearchString());
    }

    /// <summary>
    /// 构建HashTree
    /// </summary>
    /// <param name="treeData">树形结构数据</param>
    /// <param name="checkedIds">已选中的Id列表</param>
    /// <param name="firstLevelParentId">首层对象的ParentId值</param>
    /// <typeparam name="TIdType">树形结构Id类型</typeparam>
    /// <typeparam name="TValue">树形结构对象类型</typeparam>
    /// <returns></returns>
    public static HashSet<HashTree<TValue>> BuildHashTree<TIdType, TValue>(
        this IEnumerable<ITreeData<TIdType>> treeData, string checkedIds = "",
        TIdType firstLevelParentId = default(TIdType))
        where TValue : ITreeData<TIdType>
    {
        var lookup = treeData.ToLookup(x => x.ParentId);
        return BuildHashTree<TIdType, TValue>(lookup, firstLevelParentId, checkedIds.GetSearchString());
    }

    private static HashSet<HashTree<TValue>> BuildHashTree<TIdType, TValue>(
        ILookup<TIdType, ITreeData<TIdType>> lookup,
        TIdType parentId, string checkedIds = "")
        where TValue : ITreeData<TIdType>
    {
        return new HashSet<HashTree<TValue>>(
            lookup[parentId]
                .Select(t => new HashTree<TValue>(t.Name, t.Id.ToString(), (TValue)t,
                        checkedIds.MatchString(t.Id.ToString().GetSearchString()))
                    { Children = BuildHashTree<TIdType, TValue>(lookup, t.Id, checkedIds) }));
    }

    private static ICollection<BaseTree<TValue>> BuildBaseTree<TIdType, TValue>(
        ILookup<TIdType, ITreeData<TIdType>> lookup,
        TIdType parentId, string checkedIds = "")
        where TValue : ITreeData<TIdType>
    {
        return lookup[parentId].Select(t => new BaseTree<TValue>(t.Name, t.Id.ToString(), (TValue)t,
                checkedIds.MatchString(t.Id.ToString().GetSearchString()))
            { Children = BuildBaseTree<TIdType, TValue>(lookup, t.Id, checkedIds) }).ToList();
    }
}