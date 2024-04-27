using System.Collections.Generic;
using Idler.Common.Authorization.Models;
using Idler.Common.Core;

namespace Idler.Common.Authorization.Services;

public interface IResourceAuthorizationService
{
    /// <summary>
    /// 通过Id获取资源
    /// </summary>
    /// <param name="Id">Id</param>
    /// <returns></returns>
    APIReturnInfo<CoreResourceValue> SingleCoreResource(string Id);

    /// <summary>
    /// 全部资源
    /// </summary>
    /// <returns></returns>
    APIReturnInfo<IList<CoreResourceValue>> AllCoreResource();

    /// <summary>
    /// Id是否存在
    /// </summary>
    /// <param name="Id">Id</param>
    /// <returns></returns>
    bool CoreResourceIdIsExist(string Id);

    /// <summary>
    /// 创建资源
    /// </summary>
    /// <param name="AddModel">要创建的信息</param>
    /// <returns></returns>
    APIReturnInfo<CoreResourceValue> CreateCoreResource(AddCoreResourceModel AddModel);

    /// <summary>
    /// 编辑资源
    /// </summary>
    /// <param name="EditInfo">要编辑的信息</param>
    /// <param name="Id">要编辑信息的Id</param>
    /// <returns></returns>
    APIReturnInfo<CoreResourceValue> EditCoreResource(EditCoreResourceModel EditInfo, string Id);

    /// <summary>
    /// 删除资源
    /// </summary>
    /// <param name="Id">角色Id</param>
    /// <returns></returns>
    APIReturnInfo<CoreResourceValue> RemoveCoreResource(string Id);
}