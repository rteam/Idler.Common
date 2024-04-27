using AutoMapper;
using Idler.Common.Authorization.Models;

namespace Idler.Common.Authorization;
public class AuthorizationProfile : Profile
{
    public AuthorizationProfile()
    {
        this.CreateMap<CoreResource, CoreResourceValue>(MemberList.None).ReverseMap();
        this.CreateMap<AddCoreResourceModel, CoreResource>(MemberList.None);
        this.CreateMap<EditCoreResourceModel, CoreResource>(MemberList.None);
        this.CreateMap<AddCoreResourceModel, CoreResourceValue>(MemberList.None).ReverseMap();
        this.CreateMap<EditCoreResourceModel, CoreResourceValue>(MemberList.None).ReverseMap();
        this.CreateMap<CorePermission, CorePermissionValue>(MemberList.None).ReverseMap();
        this.CreateMap<AddPermissionModel, CorePermission>(MemberList.None).ReverseMap();
        this.CreateMap<EditPermissionModel, CorePermission>(MemberList.None).ReverseMap();
        this.CreateMap<AddPermissionModel, CorePermissionValue>(MemberList.None).ReverseMap();
        this.CreateMap<EditPermissionModel, CorePermissionValue>(MemberList.None).ReverseMap();
        this.CreateMap<CoreRole, CoreRoleValue>(MemberList.None).ReverseMap();
        this.CreateMap<AddRoleModel, CoreRole>(MemberList.None);
        this.CreateMap<EditRoleModel, CoreRole>(MemberList.None);
        this.CreateMap<AddRoleModel, CoreRoleValue>(MemberList.None).ReverseMap();
        this.CreateMap<EditRoleModel, CoreRoleValue>(MemberList.None).ReverseMap();
        this.CreateMap<CoreUserRole, CoreUserRoleValue>(MemberList.None).ReverseMap();
    }
}