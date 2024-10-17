using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;
using Idler.Common.Attachments;
using Idler.Common.Core;
using Idler.Common.Core.Config;
using Idler.Common.Core.Upload;
using Idler.Common.OBS;
using JetBrains.Annotations;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

namespace Idler.Common.OSS.Test;

[TestClass]
[TestSubject(typeof(OBSAttachmentDomainService))]
public class OBSAttachmentDomainServiceTest
{
    private static Lazy<IAttachmentDomainService> LazyService = new Lazy<IAttachmentDomainService>(() =>
    {
        var attachmentRepositoryMock = new Mock<IRepository<Attachment, Guid>>();
        var loggerMock = new Mock<ILogger<OSSAttachmentDomainService>>();
        var uploadConfigAccessHelperMock = new Mock<IConfigAccessHelper<UploadConfig>>();
        var unitOfWorkMock = new Mock<IUnitOfWork>();
        Guid id = Guid.NewGuid();

        attachmentRepositoryMock.Setup(t => t.Add(new Attachment() { Id = id })).Returns(new Attachment() { Id = id });

        uploadConfigAccessHelperMock.Setup(o => o.ConfigEntity).Returns(new UploadConfig()
        {
            Items = new Dictionary<string, UploadConfigItem>()
            {
                {
                    "test", new UploadConfigItem()
                    {
                        Key = "test",
                        Name = "测试",
                        RootUrl = "https://crtvup-ai.oss-cn-beijing.aliyuncs.com/",
                        AllowFileTypes = new Dictionary<string, long>()
                        {
                            { "jpg", 410221112 },
                            { "png", 410221112 },
                            { "gif", 410221112 }
                        }
                    }
                }
            }
        });
        var ossSettingMock = new Mock<IOptions<OSSSetting>>();
        using (var steam = File.OpenText("Config/OSSSetting.json"))
        {
            ossSettingMock.Setup(o => o.Value).Returns(steam.ReadToEnd().FromJson<OSSSetting>());

            return new OSSAttachmentDomainService(
                attachmentRepositoryMock.Object,
                loggerMock.Object,
                uploadConfigAccessHelperMock.Object,
                ossSettingMock.Object,
                unitOfWorkMock.Object);
        }
    });

    private IAttachmentDomainService Service => LazyService.Value;

    [TestMethod]
    public void InitializeLargeFileUploadTaskTest()
    {
        APIReturnInfo<string> result =
            this.Service.InitializeLargeFileUploadTask("test", "test", "test.jpg", 40000, 41022112);

        Assert.IsTrue(result.State);
    }


    [TestMethod]
    public void GenerateUploadAuthorizationUrlTest()
    {
        APIReturnInfo<string> result =
            this.Service.GenerateUploadAuthorizationUrl("test.jpg");

        Assert.IsTrue(result.State);
    }

    [TestMethod]
    public void UploadTest()
    {
        using (FileStream fileStream = new FileStream("testfile.png", FileMode.Open, FileAccess.Read))
        {
            APIReturnInfo<UploadFilInfo> result = this.Service.Upload("test", "test1", "testfile.png",
                fileStream.Length,
                fileStream);

            Assert.IsTrue(result.State);
        }
    }
}