using System;
using System.Collections;
using System.Linq;
using Idler.Common.Authorization.Caches;
using Idler.Common.Cache;

namespace Idler.Common.Authorization.Services;

public interface IAuthorizationService :
    IDataAuthorizationService,
    IRoleAuthorizationService,
    IUserRoleAuthorizationService,
    IPermissionAuthorizationService,
    IResourceAuthorizationService,
    IUserPermissionAuthorizationService
{
}