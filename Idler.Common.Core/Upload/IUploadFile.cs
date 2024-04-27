using System.IO;

namespace Idler.Common.Core.Upload;

/// <summary>
/// 上传文件
/// </summary>
public interface IUploadFile
{
    /// <summary>
    /// 上传文件
    /// </summary>
    /// <param name="uploadType">上传类型</param>
    /// <param name="fileName">文件名</param>
    /// <param name="fileStream">文件流</param>
    /// <returns></returns>
    APIReturnInfo<UploadFilInfo> Upload(string uploadType, string fileName,
        Stream fileStream);

    /// <summary>
    /// 上传准备
    /// </summary>
    /// <param name="uploadType">上传类型</param>
    /// <param name="fileName">文件名</param>
    /// <param name="fileSize">文件流</param>
    /// <returns></returns>
    APIReturnInfo<UploadFilInfo> Preparatory(string uploadType, string fileName,
        long fileSize);

    /// <summary>
    /// 配置文件
    /// </summary>
    UploadConfig Config { get; }
}