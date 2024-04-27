using System.Collections.Generic;
using Idler.Common.Authorization.Config;

namespace Idler.Common.Authorization.Services;

public interface IDataAuthorizationService
{
    /// <summary>
    /// 导入资源
    /// </summary>
    /// <param name="parentKey">Key</param>
    /// <param name="resources">资源</param>
    void ImportCoreResource(string parentKey, IList<CoreResourceValue> resources);

    /// <summary>
    /// 创建基础数据
    /// </summary>
    /// <returns></returns>
    AuthorizationBasicData CreateBasicData();

    /// <summary>
    /// 检查基础数据
    /// </summary>
    /// <returns></returns>
    void CheckBaseData();

    /// <summary>
    /// 刷新缓存
    /// </summary>
    /// <returns></returns>
    void RefreshCache();

    /// <summary>
    /// 删除全部缓存
    /// </summary>
    void ClearAllCache();
}