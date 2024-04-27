using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using AutoMapper.QueryableExtensions;
using Idler.Common.Authorization.Caches;
using Idler.Common.Authorization.Config;
using Idler.Common.Authorization.Models;
using Idler.Common.Authorization.Values;
using Idler.Common.AutoMapper;
using Idler.Common.Cache;
using Idler.Common.Core;
using Idler.Common.Core.Config;
using Idler.Common.Core.Specification;

namespace Idler.Common.Authorization.Services;

public class AuthorizationService : BaseDomainService, IAuthorizationService
    {
        public AuthorizationService(
            IRepository<CoreRole, string> coreRoleRepository,
            IRepository<CorePermission, string> corePermissionRepository,
            IRepository<CoreResource, string> coreResourceRepository,
            IRepository<CoreRoleCorePermission, Guid> coreRoleCorePermissionRepository,
            IRepository<CorePermissionCoreResource, Guid> corePermissionCoreResourceRepository,
            IRepository<CoreUserRole, string> coreUserRoleRepository,
            ISimpleCacheManager<IList<CoreRoleValue>> coreRoleCache,
            ISimpleCacheManager<string> roleAccessCache,
            ISimpleCacheManager<RoleScopeCacheValue> roleScopeCache,
            ISimpleCacheManager<IList<CoreResourceValue>> coreResourceCache,
            ISimpleCacheManager<IList<CorePermissionValue>> corePermissionCache,
            IConfigAccessHelper<AuthorizationConfig> authorizationConfigAccessHelper,
            IUnitOfWork unitOfWork)
            : base(unitOfWork)
        {
            this.CoreRoleCorePermissionRepository = coreRoleCorePermissionRepository;
            this.CoreResourceRepository = coreResourceRepository;
            this.CorePermissionRepository = corePermissionRepository;
            this.CoreRoleRepository = coreRoleRepository;
            this.CoreUserRoleRepository = coreUserRoleRepository;
            this.CorePermissionCoreResourceRepository = corePermissionCoreResourceRepository;
            this.CoreRoleCache = coreRoleCache;
            this.RoleAccessCache = roleAccessCache;
            this.RoleScopeCache = roleScopeCache;
            this.CoreResourceCache = coreResourceCache;
            this.CorePermissionCache = corePermissionCache;
            this.AuthorizationConfigAccessHelper = authorizationConfigAccessHelper;
            this.CheckCache();
        }

        private readonly ISimpleCacheManager<IList<CoreRoleValue>> CoreRoleCache;
        private readonly ISimpleCacheManager<string> RoleAccessCache;
        private readonly ISimpleCacheManager<RoleScopeCacheValue> RoleScopeCache;
        private readonly ISimpleCacheManager<IList<CoreResourceValue>> CoreResourceCache;
        private readonly ISimpleCacheManager<IList<CorePermissionValue>> CorePermissionCache;
        private readonly IConfigAccessHelper<AuthorizationConfig> AuthorizationConfigAccessHelper;
        private readonly IRepository<CorePermissionCoreResource, Guid> CorePermissionCoreResourceRepository;
        private readonly IRepository<CoreRoleCorePermission, Guid> CoreRoleCorePermissionRepository;
        private readonly IRepository<CoreResource, string> CoreResourceRepository;
        private readonly IRepository<CorePermission, string> CorePermissionRepository;
        private readonly IRepository<CoreRole, string> CoreRoleRepository;
        private readonly IRepository<CoreUserRole, string> CoreUserRoleRepository;


        private const string MENU_CACHE = "Menus";
        private const string PERMiSSION_CACHE_KEY = "Permissions";
        private const string RESOURCE_CACHE_KEY = "Resources";
        private const string ROLE_CACHE_KEY = "Roles";
        private const string ROLE_ACCESS_URL_CACHE_KEY = "RoleAccessUrls";
        private const string ROLE_USER_CACHE_KEY = "RoleUsers";

        /// <summary>
        /// 缓存前缀 
        /// </summary>
        private string CachePrefix => this.AuthorizationConfigAccessHelper.ConfigEntity.CachePrefix;

        #region 角色

        /// <summary>
        /// 通过Id获取角色
        /// </summary>
        /// <param name="id">Id</param>
        /// <returns></returns>
        public APIReturnInfo<CoreRoleValue> SingleCoreRole(string id)
        {
            id.ThrowIfNull(nameof(id));

            return APIReturnInfo<CoreRoleValue>.Success(this.CoreRoleRepository.Single(id)
                .Map<CoreRole, CoreRoleValue>());
        }

        /// <summary>
        /// 检查Id是否存在
        /// </summary>
        /// <param name="id">Id</param>
        /// <returns></returns>
        public bool CheckRoleIdIsExist(string id)
        {
            return this.CoreRoleRepository.IsExist(t => t.Id == id);
        }

        /// <summary>
        /// 全部角色
        /// </summary>
        /// <returns></returns>
        public APIReturnInfo<IList<CoreRoleValue>> AllCoreRole()
        {
            return APIReturnInfo<IList<CoreRoleValue>>.Success(this.CoreRoleRepository.All()
                .ProjectTo<CoreRoleValue>(ObjectMapperExtensions.Configuration).ToList());
        }

        /// <summary>
        /// 创建角色
        /// </summary>
        /// <param name="addModel">要创建的信息</param>
        /// <returns></returns>
        public APIReturnInfo<CoreRoleValue> CreateCoreRole(AddRoleModel addModel)
        {
            addModel.ThrowIfNull(nameof(addModel));

            CoreRole coreRole = addModel.Map<AddRoleModel, CoreRole>();
            coreRole.NormalizedRoleName = coreRole.Name.ToLower();

            if (this.CoreRoleRepository.IsExist(t => t.Id == coreRole.Id))
                return APIReturnInfo<CoreRoleValue>.Error("角色Id已存在");

            if (this.CoreRoleRepository.IsExist(t => t.NormalizedRoleName == coreRole.NormalizedRoleName))
                return APIReturnInfo<CoreRoleValue>.Error("角色名称已存在");

            coreRole = this.CoreRoleRepository.Add(coreRole);
            this.SaveChange();

            return APIReturnInfo<CoreRoleValue>.Success(coreRole.Map<CoreRole, CoreRoleValue>());
        }

        /// <summary>
        /// 编辑角色
        /// </summary>
        /// <param name="editModel">要编辑的信息</param>
        /// <param name="id">要编辑信息的Id</param>
        /// <returns></returns>
        public APIReturnInfo<CoreRoleValue> EditCoreRole(EditRoleModel editModel, string id)
        {
            editModel.ThrowIfNull(nameof(editModel));

            CoreRole coreRole = this.CoreRoleRepository.Single(id);
            if (coreRole == null)
                return APIReturnInfo<CoreRoleValue>.Error("要编辑的信息不存在");

            editModel.Map(coreRole);

            this.CoreRoleRepository.Update(coreRole);
            this.SaveChange();

            return APIReturnInfo<CoreRoleValue>.Success(coreRole.Map<CoreRole, CoreRoleValue>());
        }

        /// <summary>
        /// 删除角色
        /// </summary>
        /// <param name="id">角色Id</param>
        /// <returns></returns>
        public APIReturnInfo<CoreRoleValue> RemoveCoreRole(string id)
        {
            id.ThrowIfNull(nameof(id));

            if (this.CoreRoleCorePermissionRepository.Count(t => t.RoleId == id) > 0)
                return APIReturnInfo<CoreRoleValue>.Error("该角色下存在权限关联，请先取消关联！");

            CoreRole removeInfo = this.CoreRoleRepository.Remove(id);
            this.SaveChange();

            return APIReturnInfo<CoreRoleValue>.Success(removeInfo.Map<CoreRole, CoreRoleValue>());
        }

        #endregion

        #region 用户角色

        /// <summary>
        /// 通过用户Id获取用户角色关联
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        public APIReturnInfo<CoreUserRoleValue> SingleCoreUserRole(string userId)
        {
            userId.ThrowIfNull(nameof(userId));

            return APIReturnInfo<CoreUserRoleValue>.Success(
                this.CoreUserRoleRepository.Single(t => t.UserId == userId).Map<CoreUserRole, CoreUserRoleValue>());
        }

        /// <summary>
        /// 删除用户角色设置
        /// </summary>
        /// <param name="id">要删除的设置Id</param>
        public APIReturnInfo<CoreUserRoleValue> RemoveCoreUserRole(string id)
        {
            id.ThrowIfNull(nameof(id));

            CoreUserRole coreUserRole = this.CoreUserRoleRepository.Remove(id);
            if (coreUserRole == null)
                return APIReturnInfo<CoreUserRoleValue>.Error("用户角色设置不存在");

            this.SaveChange();

            return APIReturnInfo<CoreUserRoleValue>.Success(coreUserRole.Map<CoreUserRole, CoreUserRoleValue>());
        }

        /// <summary>
        /// 关联角色到用户
        /// </summary>
        /// <param name="model">关联数据</param>
        /// <param name="userId">UserId</param>
        /// <returns></returns>
        public APIReturnInfo<CoreUserRoleValue> AssignRoleToUser(AssignRoleToUserModel model, string userId)
        {
            model.ThrowIfNull(nameof(model));
            if (!model.RoleKeys.IsEmpty())
            {
                string[] roleKeys = model.RoleKeys.Split(',');
                model.RoleKeys = string.Join(",",
                    this.CoreRoleRepository.Find(t => roleKeys.Contains(t.Id)).Select(t => t.Id));
            }

            CoreUserRole coreUserRole = this.CoreUserRoleRepository.Single(t => t.UserId == userId);
            if (coreUserRole == null)
            {
                coreUserRole = new CoreUserRole(userId, model.Name, model.RoleKeys);
                this.CoreUserRoleRepository.Add(coreUserRole);
            }
            else
            {
                coreUserRole.RoleKeys = model.RoleKeys;
                this.CoreUserRoleRepository.Update(coreUserRole);
            }

            this.SaveChange();
            return APIReturnInfo<CoreUserRoleValue>.Success(coreUserRole.Map<CoreUserRole, CoreUserRoleValue>());
        }

        /// <summary>
        /// 分页返回用户与角色的关联信息
        /// </summary>
        /// <param name="keyword">按名称查询</param>
        /// <param name="pageSize">每页几条数据</param>
        /// <param name="pageNum">第几页</param>
        /// <returns></returns>
        public APIReturnInfo<ReturnPaging<CoreUserRoleValue>> CoreUserRolePaging(string keyword, int pageSize,
            int pageNum)
        {
            ISpecification<CoreUserRole> specification = null;
            if (!keyword.IsEmpty())
                specification = specification.And(t => t.Name == keyword);

            ReturnPaging<CoreUserRoleValue> paging = new ReturnPaging<CoreUserRoleValue>()
            {
                PageSize = pageSize,
                PageNum = pageNum,
                Total = this.CoreUserRoleRepository.Count(specification?.Expressions)
            };

            paging.Compute();

            paging.PageListInfos = this.CoreUserRoleRepository.Find(specification?.Expressions).Skip(paging.Skip)
                .Take(paging.Take).ProjectTo<CoreUserRoleValue>().ToList();

            return APIReturnInfo<ReturnPaging<CoreUserRoleValue>>.Success(paging);
        }

        #endregion

        #region 权限

        /// <summary>
        /// 通过Id获取权限
        /// </summary>
        /// <param name="id">Id</param>
        /// <returns></returns>
        public APIReturnInfo<CorePermissionValue> SingleCorePermission(string id)
        {
            id.ThrowIfNull(nameof(id));

            return APIReturnInfo<CorePermissionValue>.Success(this.CorePermissionRepository.Single(id)
                .Map<CorePermission, CorePermissionValue>());
        }

        /// <summary>
        /// 全部权限
        /// </summary>
        /// <returns></returns>
        public APIReturnInfo<IList<CorePermissionValue>> AllCorePermission()
        {
            return APIReturnInfo<IList<CorePermissionValue>>.Success(this.CorePermissionRepository.All()
                .ProjectTo<CorePermissionValue>(ObjectMapperExtensions.Configuration).ToList());
        }

        /// <summary>
        /// 检查Id是否存在
        /// </summary>
        /// <param name="id">Id</param>
        /// <returns></returns>
        public bool CheckPermissionIdIsExist(string id)
        {
            return this.CorePermissionRepository.IsExist(t => t.Id == id);
        }

        /// <summary>
        /// 创建权限
        /// </summary>
        /// <param name="addModel">要创建的信息</param>
        /// <returns></returns>
        public APIReturnInfo<CorePermissionValue> CreateCorePermission(AddPermissionModel addModel)
        {
            addModel.ThrowIfNull(nameof(addModel));

            if (this.CorePermissionRepository.IsExist(t => t.Id == addModel.Id))
                return APIReturnInfo<CorePermissionValue>.Error("权限Id已存在");

            if (this.CorePermissionRepository.IsExist(t => t.Name == addModel.Name))
                return APIReturnInfo<CorePermissionValue>.Error("权限名称已存在");

            CorePermission corePermission = addModel.Map<AddPermissionModel, CorePermission>();

            corePermission = this.CorePermissionRepository.Add(corePermission);

            this.SaveChange();

            return APIReturnInfo<CorePermissionValue>.Success(corePermission
                .Map<CorePermission, CorePermissionValue>());
        }

        /// <summary>
        /// 编辑权限
        /// </summary>
        /// <param name="editModel">权限</param>
        /// <param name="id">要编辑的信息</param>
        /// <returns></returns>
        public APIReturnInfo<CorePermissionValue> EditCorePermission(EditPermissionModel editModel, string id)
        {
            editModel.ThrowIfNull(nameof(editModel));
            id.ThrowIfNull(nameof(id));

            CorePermission corePermission = this.CorePermissionRepository.Single(id);
            if (corePermission == null)
                return APIReturnInfo<CorePermissionValue>.Error("要编辑的信息不存在");

            editModel.Map(corePermission);

            this.CorePermissionRepository.Update(corePermission);

            this.SaveChange();

            return APIReturnInfo<CorePermissionValue>.Success(corePermission
                .Map<CorePermission, CorePermissionValue>());
        }

        /// <summary>
        /// 删除权限
        /// </summary>
        /// <param name="id">权限Id</param>
        /// <returns></returns>
        public APIReturnInfo<CorePermissionValue> RemoveCorePermission(string id)
        {
            id.ThrowIfNull(nameof(id));

            if (this.CorePermissionRepository.Count(t => t.ParentId == id) > 0)
                return APIReturnInfo<CorePermissionValue>.Error("请先删除子节点");

            if (this.CorePermissionCoreResourceRepository.Count(t => t.PermissionId == id) > 0)
                return APIReturnInfo<CorePermissionValue>.Error("该权限下存在资源关联，请先取消关联！");

            if (this.CoreRoleCorePermissionRepository.Count(t => t.PermissionId == id) > 0)
                return APIReturnInfo<CorePermissionValue>.Error("该权限下存在角色关联，请先取消关联！");

            CorePermission corePermission = this.CorePermissionRepository.Remove(id);

            this.SaveChange();

            return APIReturnInfo<CorePermissionValue>.Success(corePermission
                .Map<CorePermission, CorePermissionValue>());
        }

        #endregion

        #region 资源

        /// <summary>
        /// 通过Id获取资源
        /// </summary>
        /// <param name="id">Id</param>
        /// <returns></returns>
        public APIReturnInfo<CoreResourceValue> SingleCoreResource(string id)
        {
            id.ThrowIfNull(nameof(id));

            return APIReturnInfo<CoreResourceValue>.Success(this.CoreResourceRepository.Single(id)
                .Map<CoreResource, CoreResourceValue>());
        }

        /// <summary>
        /// 全部资源
        /// </summary>
        /// <returns></returns>
        public APIReturnInfo<IList<CoreResourceValue>> AllCoreResource()
        {
            return APIReturnInfo<IList<CoreResourceValue>>.Success(this.CoreResourceRepository.All()
                .ProjectTo<CoreResourceValue>(ObjectMapperExtensions.Configuration).ToList());
        }

        /// <summary>
        /// Id是否存在
        /// </summary>
        /// <param name="id">Id</param>
        /// <returns></returns>
        public bool CoreResourceIdIsExist(string id)
        {
            return this.CoreResourceRepository.IsExist(t => t.Id == id);
        }

        /// <summary>
        /// 创建资源
        /// </summary>
        /// <param name="addModel">要创建的信息</param>
        /// <returns></returns>
        public APIReturnInfo<CoreResourceValue> CreateCoreResource(AddCoreResourceModel addModel)
        {
            addModel.ThrowIfNull(nameof(addModel));

            CoreResource coreResource = addModel.Map<AddCoreResourceModel, CoreResource>();

            coreResource.NormalizedUrl = coreResource.Url.ToLower();

            if (this.CoreResourceRepository.IsExist(t => t.Id == coreResource.Id))
                return APIReturnInfo<CoreResourceValue>.Error("资源Id已存在");

            if (this.CoreResourceRepository.IsExist(t => t.NormalizedUrl == coreResource.NormalizedUrl))
                return APIReturnInfo<CoreResourceValue>.Error("资源地址已存在");

            coreResource = this.CoreResourceRepository.Add(coreResource);

            this.SaveChange();

            return APIReturnInfo<CoreResourceValue>.Success(coreResource.Map<CoreResource, CoreResourceValue>());
        }

        /// <summary>
        /// 编辑资源
        /// </summary>
        /// <param name="editInfo">要编辑的信息</param>
        /// <param name="id">要编辑信息的Id</param>
        /// <returns></returns>
        public APIReturnInfo<CoreResourceValue> EditCoreResource(EditCoreResourceModel editInfo, string id)
        {
            editInfo.ThrowIfNull(nameof(editInfo));
            id.ThrowIfNull(nameof(id));

            CoreResource coreResource = this.CoreResourceRepository.Single(id);
            if (coreResource == null)
                return APIReturnInfo<CoreResourceValue>.Error("要编辑的信息不存在");

            editInfo.Map(coreResource);

            this.CoreResourceRepository.Update(coreResource);

            this.SaveChange();

            return APIReturnInfo<CoreResourceValue>.Success(coreResource
                .Map<CoreResource, CoreResourceValue>());
        }

        /// <summary>
        /// 删除资源
        /// </summary>
        /// <param name="id">角色Id</param>
        /// <returns></returns>
        public APIReturnInfo<CoreResourceValue> RemoveCoreResource(string id)
        {
            id.ThrowIfNull(nameof(id));

            var temp = this.CoreResourceRepository.Single(id);

            if (this.CoreResourceRepository.Count(t => t.ParentId == id) > 0)
                return APIReturnInfo<CoreResourceValue>.Error("请先删除子节点");

            if (this.CorePermissionCoreResourceRepository.Count(t => t.ResourceId == id) > 0)
                return APIReturnInfo<CoreResourceValue>.Error("该资源下存在权限关联，请先取消关联！");

            CoreResource removeInfo = this.CoreResourceRepository.Remove(id);

            this.SaveChange();

            return APIReturnInfo<CoreResourceValue>.Success(removeInfo.Map<CoreResource, CoreResourceValue>());
        }

        #endregion

        #region 角色权限

        /// <summary>
        /// 给指定角色分配权限
        /// </summary>
        /// <param name="assignPermissionToRoleModel">角色分配权限模型</param>
        /// <param name="roleId">角色Id</param>
        public APIReturnInfo<CoreRoleValue> AssignPermissionToRole(
            AssignPermissionToRoleModel assignPermissionToRoleModel,
            string roleId)
        {
            assignPermissionToRoleModel.ThrowIfNull(nameof(assignPermissionToRoleModel));

            CoreRole role = this.CoreRoleRepository.Single(roleId);
            if (role == null)
                return APIReturnInfo<CoreRoleValue>.Error("角色不存在");

            IList<CorePermission> corePermissions = new List<CorePermission>();

            string[] ids = assignPermissionToRoleModel.PermissionIdList.IsEmpty()
                ? new string[] { }
                : assignPermissionToRoleModel.PermissionIdList.Split(',');

            ids = this.CorePermissionRepository.Find(t => ids.Contains(t.Id)).Select(t => t.Id).ToArray();

            this.CoreRoleCorePermissionRepository.Remove(t => t.RoleId == roleId);
            this.SaveChange();

            if (ids.IsEmpty())
                role.PermissionIds = string.Join(",", ids);
            else
            {
                this.CoreRoleCorePermissionRepository.BulkInsert(ids.Select(t => new CoreRoleCorePermission(role.Id, t))
                    .ToList());
                role.PermissionIds = assignPermissionToRoleModel.PermissionIdList;
            }

            this.SaveChange();

            return APIReturnInfo<CoreRoleValue>.Success(role.Map<CoreRole, CoreRoleValue>());
        }

        #endregion

        #region 权限资源

        /// <summary>
        /// 给指定权限关联资源
        /// </summary>
        /// <param name="assignResourceToPermissionModel">权限关联资源模型</param>
        /// <param name="permissionId">权限Id</param>
        public APIReturnInfo<CorePermissionValue> AssignResourceToPermission(
            AssignResourceToPermissionModel assignResourceToPermissionModel, string permissionId)
        {
            assignResourceToPermissionModel.ThrowIfNull(nameof(assignResourceToPermissionModel));

            CorePermission corePermission =
                this.CorePermissionRepository.Single(permissionId);
            if (corePermission == null)
                throw new AggregateException("权限不存在");

            this.CorePermissionCoreResourceRepository.Remove(t =>
                t.PermissionId == permissionId);

            this.SaveChange();

            if (assignResourceToPermissionModel.ResourceIdList.IsEmpty())
            {
                corePermission.ResourceIds = "";
                this.SaveChange();
                return APIReturnInfo<CorePermissionValue>.Success(corePermission);
            }

            string[] ids = assignResourceToPermissionModel.ResourceIdList.IsEmpty()
                ? new string[] { }
                : assignResourceToPermissionModel.ResourceIdList.Split(',');

            corePermission.ResourceIds = assignResourceToPermissionModel.ResourceIdList;

            this.CorePermissionCoreResourceRepository.BulkInsert(this.CoreResourceRepository
                .Find(t => ids.Contains(t.Id))
                .Select(t => new CorePermissionCoreResource(permissionId, t.Id)).ToList());

            this.SaveChange();

            return APIReturnInfo<CorePermissionValue>.Success(corePermission
                .Map<CorePermission, CorePermissionValue>());
        }

        #endregion

        #region UserPermission

        /// <summary>
        /// 生成菜单
        /// </summary>
        /// <param name="roleIds">角色Id</param>
        /// <returns></returns>
        public APIReturnInfo<IList<MenuValue>> BuildMenu(IList<string> roleIds)
        {
            if (roleIds.IsEmpty())
                return APIReturnInfo<IList<MenuValue>>.Success(new List<MenuValue>());

            IList<RoleScopeCacheValue> menuCacheValues = new List<RoleScopeCacheValue>();
            foreach (var roleId in roleIds)
            {
                if (this.RoleScopeCache.TryGet(roleId, out RoleScopeCacheValue menuCacheValue,
                        string.Concat(this.CachePrefix, MENU_CACHE)))
                    menuCacheValues.Add(menuCacheValue);
            }

            IDictionary<string, List<CorePermissionValue>> allPermissions =
                menuCacheValues.SelectMany(t => t.Permissions).Distinct().GroupBy(t => t.ParentId)
                    .ToDictionary(t => t.Key ?? string.Empty, t => t.ToList());

            IDictionary<string, CoreResourceValue> allResources = menuCacheValues.SelectMany(t => t.Resources)
                .Distinct().Where(t => t.IsMenu)
                .ToDictionary(t => t.Id, t => t);

            IList<MenuValue> menus = new List<MenuValue>();

            if (!allPermissions.TryGetValue(string.Empty, out List<CorePermissionValue> permissions))
                return APIReturnInfo<IList<MenuValue>>.Success(menus);

            foreach (CorePermissionValue permission in permissions)
                this.BuildChidrenMenu(permission, allPermissions, allResources, menus);

            return APIReturnInfo<IList<MenuValue>>.Success(menus);
        }


        /// <summary>
        /// 生成指定用户的菜单
        /// </summary>
        /// <param name="userId">UserId</param>
        /// <returns></returns>
        public APIReturnInfo<IList<MenuValue>> BuildMenu(string userId)
        {
            return this.BuildMenu(this.InternalCoreUserRoles(userId).Select(t => t.Id).ToList());
        }

        /// <summary>
        /// 生成子菜单
        /// </summary>
        /// <param name="permissionValue">要生成菜单的权限</param>
        /// <param name="allPermissions">所有权限</param>
        /// <param name="allMenuResources">所有资源</param>
        /// <param name="menus">菜单列表</param>
        private void BuildChidrenMenu(
            CorePermissionValue permissionValue,
            IDictionary<string, List<CorePermissionValue>> allPermissions,
            IDictionary<string, CoreResourceValue> allMenuResources,
            IList<MenuValue> menus
        )
        {
            if (permissionValue == null)
                return;

            MenuValue menuValue = null;

            IList<CoreResourceValue> resourceValues = new List<CoreResourceValue>();
            permissionValue.ResourceIdList.ForEach(t =>
            {
                if (allMenuResources.TryGetValue(t, out CoreResourceValue resourceValue))
                    resourceValues.Add(resourceValue);
            });

            if (resourceValues.Count == 0 && permissionValue.IsMenu)
            {
                menuValue = new MenuValue(title: permissionValue.Name, url: string.Empty, icon: string.Empty,
                    authorized: true, orderBy: 0);
                menus.Add(menuValue);
            }

            if (resourceValues.Count == 1 && resourceValues[0].IsMenu)
            {
                menuValue = new MenuValue(title: resourceValues[0].Name, url: resourceValues[0].Url,
                    icon: resourceValues[0].Icon, authorized: true, orderBy: resourceValues[0].Order);
                menus.Add(menuValue);
            }

            if (resourceValues.Count > 1)
            {
                if (permissionValue.IsMenu && permissionValue.IsStructure)
                {
                    menuValue = new MenuValue(title: permissionValue.Name, url: string.Empty, icon: string.Empty,
                        orderBy: 0, children: resourceValues.Select(t =>
                                new MenuValue(title: t.Name, url: t.Url, icon: t.Icon, authorized: true,
                                    orderBy: t.Order))
                            .ToList());

                    menuValue.Authorized = menuValue.Children.Count(t => t.Authorized) > 0;

                    menus.Add(menuValue);
                }
                else
                {
                    resourceValues.ForEach(t => menus.Add(new MenuValue(title: t.Name, url: t.Url, icon: t.Icon,
                        authorized: true, orderBy: t.Order)));
                }
            }

            if (allPermissions.TryGetValue(permissionValue.Id, out List<CorePermissionValue> permissionValues))
                permissionValues.ForEach(t => this.BuildChidrenMenu(t, allPermissions, allMenuResources,
                    menuValue == null ? menus : menuValue.Children));
        }

        /// <summary>
        /// 指定角色是否拥有访问指定地址的权限
        /// </summary>
        /// <param name="roleIds">角色Id列表</param>
        /// <param name="url">地址</param>
        /// <returns></returns>
        public bool AccessAuthorization(IList<string> roleIds, string url)
        {
            if (roleIds.IsEmpty())
                return false;

            if (url.IsEmpty())
                return false;

            foreach (var roleId in roleIds)
            {
                string cacheKey = string.Concat(roleId, "_", url.ToUpper());
                if (this.RoleAccessCache.Exist(cacheKey, string.Concat(this.CachePrefix, ROLE_ACCESS_URL_CACHE_KEY)))
                    return true;
            }

            return false;
        }

        /// <summary>
        /// 取得指定用户的所有角色
        /// </summary>
        /// <param name="userId">UserId</param>
        /// <returns></returns>
        public APIReturnInfo<IList<CoreRoleValue>> CoreUserRoles(string userId)
        {
            return APIReturnInfo<IList<CoreRoleValue>>.Success(this.InternalCoreUserRoles(userId));
        }

        /// <summary>
        /// 获取用户关联角色的内部方法
        /// </summary>
        /// <param name="userId">UserId</param>
        /// <returns></returns>
        private IList<CoreRoleValue> InternalCoreUserRoles(string userId)
        {
            if (userId.IsEmpty())
                return new List<CoreRoleValue>();

            CoreUserRole coreUserRole = this.CoreUserRoleRepository.Single(userId);
            if (coreUserRole == null || coreUserRole.RoleKeys.IsEmpty())
                return new List<CoreRoleValue>();

            string[] roleKeys = coreUserRole.RoleKeys.Split(',');

            string coreRoleCacheRegion = string.Concat(this.CachePrefix, ROLE_CACHE_KEY);

            if (!this.CoreRoleCache.TryGet(CacheConst.CACHE_KEY_ALL, out IList<CoreRoleValue> roles,
                    coreRoleCacheRegion))
                return new List<CoreRoleValue>();

            return roles.Where(t => roleKeys.Contains(t.Id)).ToList();
        }

        #endregion

        #region Data

        /// <summary>
        /// 导入资源
        /// </summary>
        /// <param name="parentKey">Key</param>
        /// <param name="resources">资源</param>
        public void ImportCoreResource(string parentKey, IList<CoreResourceValue> resources)
        {
            parentKey.ThrowIfNull(nameof(parentKey));

            if (resources == null || resources.Count == 0)
                return;

            CoreResource parentInfo = this.CoreResourceRepository.Single(parentKey);
            if (parentInfo == null)
                parentInfo = this.CoreResourceRepository.Add(new CoreResource(parentKey, parentKey, parentKey));

            foreach (CoreResourceValue item in resources)
            {
                if (this.CoreResourceIdIsExist(item.Id))
                    continue;

                item.ParentId = parentInfo.Id;
                this.CoreResourceRepository.Add(item.Map<CoreResourceValue, CoreResource>());
            }

            this.SaveChange();
        }

        /// <summary>
        /// 创建基础数据
        /// </summary>
        /// <returns></returns>
        public AuthorizationBasicData CreateBasicData()
        {
            return new AuthorizationBasicData
            {
                Permissions = this.CorePermissionRepository.All()
                    .ProjectTo<CorePermissionValue>(ObjectMapperExtensions.Configuration).ToList(),
                Resources = this.CoreResourceRepository.All()
                    .ProjectTo<CoreResourceValue>(ObjectMapperExtensions.Configuration).ToList(),
                Roles = this.CoreRoleRepository.All()
                    .ProjectTo<CoreRoleValue>(ObjectMapperExtensions.Configuration)
                    .ToList(),
            };
        }

        /// <summary>
        /// 检查基础数据
        /// </summary>
        /// <returns></returns>
        public void CheckBaseData()
        {
            this.ClearAllCache();
        }

        private void CheckCache()
        {
            if (!this.CoreRoleCache.Exist("All", string.Concat(this.CachePrefix, ROLE_CACHE_KEY)))
                this.RefreshCache();
        }

        private void LoadCache()
        {
            IList<CoreRoleValue> roles = this.CoreRoleRepository.All()
                .ProjectTo<CoreRoleValue>().ToList();

            if (roles.Count == 0)
                return;

            this.CoreRoleCache.Set(CacheConst.CACHE_KEY_ALL, roles, string.Concat(this.CachePrefix, ROLE_CACHE_KEY));

            IList<CorePermissionValue> permissions =
                this.CorePermissionRepository.All().ProjectTo<CorePermissionValue>().ToList();

            IList<CoreResourceValue> resources =
                this.CoreResourceRepository.All().ProjectTo<CoreResourceValue>().ToList();

            string menuRegion = string.Concat(this.CachePrefix, MENU_CACHE);
            string roleAccessUrlRegion = string.Concat(this.CachePrefix, ROLE_ACCESS_URL_CACHE_KEY);

            foreach (var role in roles)
            {
                RoleScopeCacheValue roleScopeCacheValue = new RoleScopeCacheValue()
                {
                    RoleId = role.Id,
                    Permissions = permissions.Where(t => role.PermissionIdList.Contains(t.Id)).ToList()
                };

                roleScopeCacheValue.Resources = resources.Where(t =>
                        roleScopeCacheValue.Permissions.SelectMany(r => r.ResourceIdList).Distinct().ToHashSet()
                            .Contains(t.Id))
                    .ToList();

                this.RoleScopeCache.Set(role.Id, roleScopeCacheValue, menuRegion);

                roleScopeCacheValue.Resources.Where(t => !t.NormalizedUrl.IsEmpty())
                    .Select(t => t.NormalizedUrl)
                    .Distinct()
                    .ForEach(t =>
                        this.RoleAccessCache.Set(string.Concat(role.Id, (string)"_", (string)t), t,
                            roleAccessUrlRegion));
            }
        }

        /// <summary>
        /// 删除全部缓存
        /// </summary>
        public void ClearAllCache()
        {
            this.CoreRoleCache.Remove("All", string.Concat(this.CachePrefix, ROLE_CACHE_KEY));
            this.CorePermissionCache.Remove(string.Concat(this.CachePrefix + PERMiSSION_CACHE_KEY, "_All"));
            this.CoreResourceCache.Remove(string.Concat(this.CachePrefix + RESOURCE_CACHE_KEY, "_All"));
            this.RoleAccessCache.RemoveRegion(string.Concat(this.CachePrefix, ROLE_ACCESS_URL_CACHE_KEY));
            this.RoleScopeCache.RemoveRegion(string.Concat(this.CachePrefix, MENU_CACHE));
        }

        /// <summary>
        /// 刷新缓存
        /// </summary>
        public void RefreshCache()
        {
            this.ClearAllCache();
            this.LoadCache();
        }

        #endregion
    }