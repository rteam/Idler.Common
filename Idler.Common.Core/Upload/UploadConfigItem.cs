using System.Collections.Generic;

namespace Idler.Common.Core.Upload;

/// <summary>
/// 上传配置项
/// </summary>
public class UploadConfigItem
{
    /// <summary>
    /// Key
    /// </summary>
    public string Key { get; set; }

    /// <summary>
    /// 配置名称
    /// </summary>
    public string Name { get; set; }

    /// <summary>
    /// 上传模式
    /// </summary>
    public string Mode { get; set; }

    /// <summary>
    /// 根URL
    /// </summary>
    public string RootUrl { get; set; }

    /// <summary>
    /// 允许文件类型及大小限制
    /// </summary>
    public IDictionary<string, long> AllowFileTypes { get; set; } = new Dictionary<string, long>();

    /// <summary>
    /// 校验是否为支持文件
    /// </summary>
    /// <param name="FileExt">扩展名</param>
    /// <param name="FileSize">文件大小</param>
    /// <returns></returns>
    public APIReturnInfo<string> Verify(string FileExt, long FileSize)
    {
        if (!this.AllowFileTypes.TryGetValue(FileExt, out long MaxFileSize))
            return APIReturnInfo<string>.Error("不支持的文件类型");

        if (FileSize / 1024 > MaxFileSize)
            return APIReturnInfo<string>.Error($"文件大小超过限制，最大允许上传 {MaxFileSize} KB文件");

        return APIReturnInfo<string>.Success(this.Key);
    }
}