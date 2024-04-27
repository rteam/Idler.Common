using System.Runtime.CompilerServices;
using System.Web;
using Idler.Common.Authorization.Config;
using Idler.Common.Authorization.Models;
using Idler.Common.Authorization.Services;
using Idler.Common.Authorization.Values;
using Idler.Common.Core;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using IAuthorizationService = Idler.Common.Authorization.Services.IAuthorizationService;

namespace Idler.Common.Authorization;

/// <summary>
/// 授权设计器基类
/// </summary>
public abstract class BaseAuthorizationDesignController : ControllerBase
{
    private readonly IAuthorizationService _authorizationService;

    /// <summary>
    /// 授权服务
    /// </summary>
    /// <param name="authorizationService"></param>
    public BaseAuthorizationDesignController(IAuthorizationService authorizationService)
    {
        this._authorizationService = authorizationService;
    }

    #region 角色

    /// <summary>
    /// 全部角色
    /// </summary>
    /// <returns></returns>
    [HttpGet("CoreRole/All")]
    public ActionResult<APIReturnInfo<IList<CoreRoleValue>>> AllCoreRole()
    {
        return this._authorizationService.AllCoreRole();
    }

    /// <summary>
    /// 创建角色
    /// </summary>
    /// <param name="modelInfo">角色</param>
    /// <returns></returns>
    [HttpPost("CoreRole")]
    public ActionResult<APIReturnInfo<CoreRoleValue>> CreateCoreRole(AddRoleModel modelInfo)
    {
        if (!this.ModelState.IsValid)
            return APIReturnInfo<CoreRoleValue>.Error("数据填写有误");

        return this._authorizationService.CreateCoreRole(modelInfo);
    }

    /// <summary>
    /// 编辑角色
    /// </summary>
    /// <param name="editInfo">角色</param>
    /// <param name="id">角色Id</param>
    /// <returns></returns>
    [HttpPut("CoreRole/{Id}")]
    public ActionResult<APIReturnInfo<CoreRoleValue>> EditCoreRole(EditRoleModel editInfo, string id)
    {
        if (!this.ModelState.IsValid)
            return APIReturnInfo<CoreRoleValue>.Error("数据填写有误");

        return this._authorizationService.EditCoreRole(editInfo, id);
    }

    /// <summary>
    /// 删除角色
    /// </summary>
    /// <param name="id">角色Id</param>
    /// <returns></returns>
    [HttpDelete("CoreRole/{Id}")]
    public ActionResult<APIReturnInfo<CoreRoleValue>> RemoveCoreRole(string id)
    {
        return this._authorizationService.RemoveCoreRole(id);
    }

    /// <summary>
    /// 获取指定角色信息
    /// </summary>
    /// <param name="id">角色Id</param>
    /// <returns></returns>
    [HttpGet("CoreRole/{Id}")]
    public ActionResult<APIReturnInfo<CoreRoleValue>> SingleCoreRole(string id)
    {
        return this._authorizationService.SingleCoreRole(id);
    }

    /// <summary>
    /// 检查Id是否存在
    /// </summary>
    /// <param name="id">Id</param>
    /// <returns></returns>
    [AllowAnonymous]
    [HttpGet("CoreRole/{Id}/Exist")]
    public ActionResult<bool> CheckRoleIdIsExist(string id)
    {
        return !this._authorizationService.CheckRoleIdIsExist(id);
    }

    /// <summary>
    /// 预览指定角色的菜单
    /// </summary>
    /// <param name="id">角色Id</param>
    /// <returns></returns>
    [HttpGet("CoreRole/{Id}/Menu/Preview")]
    public ActionResult<APIReturnInfo<IList<MenuValue>>> MenuPreview(string id)
    {
        return this._authorizationService.BuildMenu(new List<string>() { id });
    }

    #endregion

    #region 权限

    /// <summary>
    /// 全部权限
    /// </summary>
    /// <returns></returns>
    [HttpGet("CorePermission/All")]
    public ActionResult<APIReturnInfo<IList<CorePermissionValue>>> AllCorePermission()
    {
        return this._authorizationService.AllCorePermission();
    }

    /// <summary>
    /// 创建权限
    /// </summary>
    /// <param name="modelInfo">权限</param>
    /// <returns></returns>
    [HttpPost("CorePermission")]
    public ActionResult<APIReturnInfo<CorePermissionValue>> CreateCorePermission(AddPermissionModel modelInfo)
    {
        if (!this.ModelState.IsValid)
            return APIReturnInfo<CorePermissionValue>.Error("数据填写有误");

        return this._authorizationService.CreateCorePermission(modelInfo);
    }

    /// <summary>
    /// 编辑权限
    /// </summary>
    /// <param name="editInfo">权限</param>
    /// <param name="id">权限Id</param>
    /// <returns></returns>
    [HttpPut("CorePermission/{Id}")]
    public ActionResult<APIReturnInfo<CorePermissionValue>> EditCorePermission(EditPermissionModel editInfo, string id)
    {
        if (!this.ModelState.IsValid)
            return APIReturnInfo<CorePermissionValue>.Error("数据填写有误");

        return this._authorizationService.EditCorePermission(editInfo, id);
    }

    /// <summary>
    /// 获取指定权限信息
    /// </summary>
    /// <param name="id">权限Id</param>
    /// <returns></returns>
    [HttpGet("CorePermission/{Id}")]
    public ActionResult<APIReturnInfo<CorePermissionValue>> SingleCorePermission(string id)
    {
        if (!this.ModelState.IsValid)
            return APIReturnInfo<CorePermissionValue>.Error("数据填写有误");

        return this._authorizationService.SingleCorePermission(id);
    }

    /// <summary>
    /// 删除权限
    /// </summary>
    /// <param name="id">权限Id</param>
    /// <returns></returns>
    [HttpDelete("CorePermission/{Id}")]
    public ActionResult<APIReturnInfo<CorePermissionValue>> RemoveCorePermission(string id)
    {
        return this._authorizationService.RemoveCorePermission(id);
    }

    /// <summary>
    /// 检查Id是否存在
    /// </summary>
    /// <param name="id">Id</param>
    /// <returns></returns>
    [AllowAnonymous]
    [HttpGet("CorePermission/{Id}/Exist")]
    public ActionResult<bool> CheckPermissionIdIsExist(string id)
    {
        return !this._authorizationService.CheckPermissionIdIsExist(id);
    }

    /// <summary>
    /// 给指定权限关联资源
    /// </summary>
    /// <param name="assignResourceToPermissionModel">权限关联资源模型</param>
    /// <param name="id">权限Id</param>
    [HttpPut("CorePermission/{Id}/CoreResource")]
    public ActionResult<APIReturnInfo<CorePermissionValue>> AssignResourceToPermission(
        AssignResourceToPermissionModel assignResourceToPermissionModel, string id)
    {
        if (!this.ModelState.IsValid)
            return APIReturnInfo<CorePermissionValue>.Error("数据填写有误");

        return this._authorizationService.AssignResourceToPermission(assignResourceToPermissionModel, id);
    }

    #endregion

    #region 资源

    /// <summary>
    /// 全部资源
    /// </summary>
    /// <returns></returns>
    [HttpGet("CoreResource/All")]
    public ActionResult<APIReturnInfo<IList<CoreResourceValue>>> AllCoreResource()
    {
        return this._authorizationService.AllCoreResource();
    }

    /// <summary>
    /// 创建资源
    /// </summary>
    /// <param name="modelInfo">资源</param>
    /// <returns></returns>
    [HttpPost("CoreResource")]
    public ActionResult<APIReturnInfo<CoreResourceValue>> CreateCoreResource(AddCoreResourceModel modelInfo)
    {
        if (!this.ModelState.IsValid)
            return APIReturnInfo<CoreResourceValue>.Error("数据填写有误");

        return this._authorizationService.CreateCoreResource(modelInfo);
    }

    /// <summary>
    /// 编辑资源
    /// </summary>
    /// <param name="editInfo">资源</param>
    /// <param name="id">资源Id</param>
    /// <returns></returns>
    [HttpPut("CoreResource/{Id}")]
    public ActionResult<APIReturnInfo<CoreResourceValue>> EditCoreResource(EditCoreResourceModel editInfo, string id)
    {
        if (!this.ModelState.IsValid)
            return APIReturnInfo<CoreResourceValue>.Error("数据填写有误");

        return this._authorizationService.EditCoreResource(editInfo, HttpUtility.UrlDecode(id));
    }

    /// <summary>
    /// 删除资源
    /// </summary>
    /// <param name="id">资源Id</param>
    /// <returns></returns>
    [HttpDelete("CoreResource/{Id}")]
    public ActionResult<APIReturnInfo<CoreResourceValue>> RemoveCoreResource(string id)
    {
        return this._authorizationService.RemoveCoreResource(HttpUtility.UrlDecode(id));
    }

    /// <summary>
    /// 获取指定资源信息
    /// </summary>
    /// <param name="id">资源Id</param>
    /// <returns></returns>
    [HttpGet("CoreResource/{Id}")]
    public ActionResult<APIReturnInfo<CoreResourceValue>> SingleCoreResource(string id)
    {
        return this._authorizationService.SingleCoreResource(HttpUtility.UrlDecode(id));
    }

    /// <summary>
    /// 检查Id是否存在
    /// </summary>
    /// <param name="id">Id</param>
    /// <returns></returns>
    [AllowAnonymous]
    [HttpGet("CoreResource/{Id}/Exist")]
    public ActionResult<bool> CheckResourceIdIsExist(string id)
    {
        return !this._authorizationService.CoreResourceIdIsExist(HttpUtility.UrlDecode(id));
    }

    #endregion

    #region 用户角色

    /// <summary>
    /// 关联角色到用户
    /// </summary>
    /// <param name="model">关联信息</param>
    /// <param name="id">用户Id</param>
    /// <returns></returns>
    [HttpPut("User/{Id}/CoreRole")]
    public ActionResult<APIReturnInfo<CoreUserRoleValue>> AssignRoleToUser(AssignRoleToUserModel model, string id)
    {
        if (!this.ModelState.IsValid)
            return APIReturnInfo<CoreUserRoleValue>.Error("数据填写有误");

        return this._authorizationService.AssignRoleToUser(model, id);
    }

    /// <summary>
    /// 删除用户角色
    /// </summary>
    /// <param name="id">用户角色Id</param>
    /// <returns></returns>
    [HttpDelete("CoreUserRole/{Id}")]
    public ActionResult<APIReturnInfo<CoreUserRoleValue>> RemoveCoreUserRoleValue(string id)
    {
        return this._authorizationService.RemoveCoreUserRole(id);
    }

    /// <summary>
    /// 返回用户关联的所有角色
    /// </summary>
    /// <param name="id">用户Id</param>
    /// <returns></returns>
    [HttpGet("User/{Id}/CoreRole")]
    public ActionResult<APIReturnInfo<IList<CoreRoleValue>>> QueryAllCoreUserRoleValue(string id)
    {
        return this._authorizationService.CoreUserRoles(id);
    }

    /// <summary>
    /// 给指定角色分配权限
    /// </summary>
    /// <param name="assignPermissionToRoleModel">角色分配权限模型</param>
    /// <param name="id">用户Id</param>
    [HttpPut("CoreRole/{id}/Permission")]
    public ActionResult<APIReturnInfo<CoreRoleValue>> AssignPermissionToRole(
        AssignPermissionToRoleModel assignPermissionToRoleModel,
        string id)
    {
        if (!this.ModelState.IsValid)
            return APIReturnInfo<CoreRoleValue>.Error("数据填写有误");

        return this._authorizationService.AssignPermissionToRole(assignPermissionToRoleModel, id);
    }

    /// <summary>
    /// 分页返回用户与角色的关联信息
    /// </summary>
    /// <param name="keyword">按名称查询</param>
    /// <param name="pageSize">每页几条数据</param>
    /// <param name="pageNum">第几页</param>
    /// <returns></returns>
    [HttpGet("CoreUserRole/Paging")]
    public ActionResult<APIReturnInfo<ReturnPaging<CoreUserRoleValue>>> CoreUserRolePaging(string keyword, int pageSize,
        int pageNum)
    {
        return this._authorizationService.CoreUserRolePaging(keyword, pageSize, pageNum);
    }

    #endregion


    /// <summary>
    /// 读取本地控制器
    /// </summary>
    /// <returns></returns>
    protected abstract Type[] LoadLocalControllers();

    /// <summary>
    /// 导入控制器
    /// </summary>
    /// <param name="controllerKey"></param>
    /// <returns></returns>
    [HttpPost("ImportController/{ControllerKey}")]
    public ActionResult<APIReturnInfo<string>> ImportController(string controllerKey)
    {
        ImportActionModel modelInfo = new ImportActionModel(this.LoadLocalControllers(), controllerKey);
        APIReturnInfo<string> tReturnInfo = modelInfo.Validate();
        if (!tReturnInfo.State)
            return tReturnInfo;

        this._authorizationService.ImportCoreResource(controllerKey, modelInfo.CoreResources);
        return APIReturnInfo<string>.Success("ok");
    }

    /// <summary>
    /// 加载控制器
    /// </summary>
    /// <returns></returns>
    [HttpGet("LoadControllers")]
    public ActionResult<APIReturnInfo<IList<ControllerItemModel>>> LoadControllers()
    {
        return APIReturnInfo<IList<ControllerItemModel>>.Success(
            new ControllerModel(LoadLocalControllers()).Controllers);
    }

    /// <summary>
    /// 导出基础数据
    /// </summary>
    /// <returns></returns>
    [HttpGet("Output")]
    public ActionResult<APIReturnInfo<AuthorizationBasicData>> GenerateBaseData()
    {
        return new APIReturnInfo<AuthorizationBasicData>
        {
            Data = this._authorizationService.CreateBasicData(),
            State = true
        };
    }

    /// <summary>
    /// 检查基础数据
    /// </summary>
    /// <returns></returns>
    [HttpGet("Check")]
    public virtual ActionResult<APIReturnInfo<string>> CheckBaseData()
    {
        this._authorizationService.CheckBaseData();
        return APIReturnInfo<string>.Success("成功");
    }

    /// <summary>
    /// 刷新缓存
    /// </summary>
    /// <returns></returns>
    [HttpGet("RefreshCache")]
    public virtual ActionResult<APIReturnInfo<string>> RefreshCache()
    {
        this._authorizationService.RefreshCache();
        return APIReturnInfo<string>.Success("成功");
    }
}