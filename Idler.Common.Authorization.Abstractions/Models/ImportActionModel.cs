using System;
using System.Collections.Generic;
using System.Linq;
using Idler.Common.Core;

namespace Idler.Common.Authorization.Models;

public class ImportActionModel
{
    public ImportActionModel(Type[] Types, string ControllerKey)
    {
        Type ControllerType = Types.SingleOrDefault(t =>
            t.Name.Replace("Controller", "").Equals(ControllerKey, StringComparison.OrdinalIgnoreCase));
        this.ControllerKey = ControllerKey;
        this.ControllerIsExist = ControllerType != null;
        if (!this.ControllerIsExist)
            return;

        this.CoreResources = ControllerType.GetMethods().Where(t =>
                t.IsPublic && t.CustomAttributes.Count(c => c.AttributeType.BaseType.Name == "HttpMethodAttribute") > 0)
            .Select(t => t.Name).Distinct()
            .Select(t => new CoreResourceValue(string.Concat("/", this.ControllerKey, "/", t), t,
                string.Concat("/", this.ControllerKey, "/", t))).ToList();
    }

    /// <summary>
    /// 控制器是否存在
    /// </summary>
    private bool ControllerIsExist { get; set; }

    /// <summary>
    /// 控制器Key
    /// </summary>
    public string ControllerKey { get; set; }

    /// <summary>
    /// 资源列表
    /// </summary>
    public IList<CoreResourceValue> CoreResources { get; set; }

    /// <summary>
    /// 验证
    /// </summary>
    /// <param name="validationContext"></param>
    /// <returns></returns>
    public APIReturnInfo<string> Validate()
    {
        if (this.ControllerKey.IsEmpty())
            return APIReturnInfo<string>.Error("一般系统保护性错误");

        if (!this.ControllerIsExist)
            return APIReturnInfo<string>.Error("控制器不存在");

        if (this.CoreResources.Count == 0)
            return APIReturnInfo<string>.Error("控制器为空");

        return APIReturnInfo<string>.Success("成功");
    }
}