using System;

namespace Idler.Common.Core.Upload;

/// <summary>
/// 待上传文件的信息
/// </summary>
public class UploadFilInfo
{
    public UploadFilInfo(string rootPath, string fileName, long fileSize, string uploadType)
        : this(rootPath, fileName, fileSize, uploadType, Guid.NewGuid())
    {
    }

    public UploadFilInfo(string rootPath, string fileName, long fileSize, string uploadType, Guid id)
    {
        fileName.ThrowIfNull(nameof(fileName));

        this.FileName = fileName;
        this.FileSize = fileSize;
        this.RootPath = rootPath;
        this.UploadType = uploadType;
        this.Id = id;
        this.FileExt = this.GetFileExt(this.FileName);
        this.SavePath = $"{this.RootPath}{DateTime.Now:yyyy/MM/dd/}";
        this.SaveFileName = this.GetUniqueFileName(this.FileExt);
    }

    public UploadFilInfo()
    {
    }

    /// <summary>
    /// 文件名
    /// </summary>
    public string FileName { get; set; }

    /// <summary>
    /// 文件扩展名
    /// </summary>
    public string FileExt { get; set; }

    /// <summary>
    /// 保存后的路径
    /// </summary>
    public string SavePath { get; set; }

    /// <summary>
    /// 保存后的文件名
    /// </summary>
    public string SaveFileName { get; set; }

    /// <summary>
    /// 根路径
    /// </summary>
    public string RootPath { get; set; }

    /// <summary>
    /// 文件大小
    /// </summary>
    public long FileSize { get; set; }

    /// <summary>
    /// 上传类型
    /// </summary>
    public string UploadType { get; set; }

    /// <summary>
    /// RootUrl
    /// </summary>
    public string RootUrl { get; set; }

    /// <summary>
    /// 文件唯一标识符
    /// </summary>
    public Guid Id { get; set; }

    /// <summary>
    /// 获取文件名中的扩展名
    /// </summary>
    /// <param name="fileName">文件名</param>
    /// <returns></returns>
    private string GetFileExt(string fileName)
    {
        return fileName.Substring(fileName.LastIndexOf('.'), fileName.Length - fileName.LastIndexOf('.'))
            .Replace(".", "");
    }

    /// <summary>
    /// 生成指定扩展名的随即文件名
    /// </summary>
    /// <param name="fileExt">扩展名</param>
    /// <returns></returns>
    private string GetUniqueFileName(string fileExt)
    {
        return $"{Guid.NewGuid().ToString()}.{fileExt}";
    }
}