using System;
using System.IO;
using Microsoft.Extensions.Options;

namespace Idler.Common.Core.Upload;

/// <summary>
/// 上传文件到本地
/// </summary>
public class LocalUploadFile : IUploadFile
{
    public LocalUploadFile(IOptions<UploadConfig> option)
    {
        this.UploadConfigOption = option;
    }

    private readonly IOptions<UploadConfig> UploadConfigOption;

    /// <summary>
    /// 配置文件
    /// </summary>
    public UploadConfig Config
    {
        get { return this.UploadConfigOption.Value; }
    }

    /// <summary>
    /// 上传文件
    /// </summary>
    /// <param name="uploadType">上传类型</param>
    /// <param name="fileName">文件名</param>
    /// <param name="fileStream">文件流</param>
    /// <returns></returns>
    public APIReturnInfo<UploadFilInfo> Upload(string uploadType, string fileName,
        Stream fileStream)
    {
        APIReturnInfo<UploadFilInfo> uploadFileInfo = this.Preparatory(uploadType, fileName, fileStream.Length);
        if (!uploadFileInfo.State)
            return uploadFileInfo;

        string fullPath = Path.Combine(uploadFileInfo.Data.SavePath, uploadFileInfo.Data.SaveFileName);

        using (FileStream fs = new FileStream(fullPath, FileMode.Create))
        {
            fileStream.CopyTo(fs);
        }

        return uploadFileInfo;
    }

    /// <summary>
    /// 上传准备
    /// </summary>
    /// <param name="uploadType">上传类型</param>
    /// <param name="fileName">文件名</param>
    /// <param name="fileSize">文件流</param>
    /// <returns></returns>
    public APIReturnInfo<UploadFilInfo> Preparatory(string uploadType, string fileName,
        long fileSize)
    {
        uploadType.ThrowIfNull(nameof(uploadType));
        fileName.ThrowIfNull(nameof(fileName));

        UploadFilInfo fileInfo = new UploadFilInfo(
            Path.Combine(AppContext.BaseDirectory, string.Concat(this.Config.UpdatePath, uploadType, "/")),
            fileName, fileSize, uploadType);

        APIReturnInfo<UploadConfigItem> verifyResult =
            this.Config.Verify(uploadType, fileInfo.FileExt, fileInfo.FileSize);
        if (!verifyResult.State)
            return APIReturnInfo<UploadFilInfo>.Error(verifyResult.Message);

        fileInfo.RootUrl = verifyResult.Data.RootUrl;

        if (!Directory.Exists(fileInfo.SavePath))
            Directory.CreateDirectory(fileInfo.SavePath);

        return APIReturnInfo<UploadFilInfo>.Success(fileInfo);
    }
}