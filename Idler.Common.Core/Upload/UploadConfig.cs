using System.Collections.Generic;
using Idler.Common.Core.Config;

namespace Idler.Common.Core.Upload;

/// <summary>
/// 上传配置文件
/// </summary>
public class UploadConfig : AutoBaseConfig<UploadConfig>
{
    /// <summary>
    /// 配置列表
    /// </summary>
    public IDictionary<string, UploadConfigItem> Items { get; set; } = new Dictionary<string, UploadConfigItem>();

    /// <summary>
    /// 上传路径
    /// </summary>
    public string UpdatePath { get; set; } = "Upload/";

    /// <summary>
    /// 校验上传文件是否合法
    /// </summary>
    /// <param name="UploadType">上传类别</param>
    /// <param name="FileExt">扩展名</param>
    /// <param name="FileSize">文件尺寸</param>
    /// <returns></returns>
    public APIReturnInfo<UploadConfigItem> Verify(string UploadType, string FileExt, long FileSize)
    {
        if (!this.Items.TryGetValue(UploadType, out UploadConfigItem item))
            return APIReturnInfo<UploadConfigItem>.Error("不支持此类别文件上传");

        APIReturnInfo<string> status = item.Verify(FileExt, FileSize);
        if (status.State)
            return APIReturnInfo<UploadConfigItem>.Success(item);

        return APIReturnInfo<UploadConfigItem>.Error(status.Message);
    }
}