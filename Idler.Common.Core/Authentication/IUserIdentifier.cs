using Idler.Common.Core.Domain;

namespace Idler.Common.Core.Authentication;

public interface IUserIdentifier : IEntity<string>
{
    /// <summary>
    /// 用户名称
    /// 顺序为：昵称、用户名、Email
    /// 不作为唯一标识符
    /// </summary>
    string Name { get; set; }
}