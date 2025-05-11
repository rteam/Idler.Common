using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;
using Idler.Common.Attachments;
using Idler.Common.Core;
using Idler.Common.Core.Config;
using Idler.Common.Core.Upload;
using Idler.Common.Attachments.OBS;
using Idler.Common.Attachments.OSS;
using JetBrains.Annotations;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

namespace Idler.Common.Attachments.OSS.Test;

[TestClass]
[TestSubject(typeof(OBSAttachmentDomainService))]
public class OBSAttachmentDomainServiceTest
{
    private static Lazy<IAttachmentDomainService> LazyService = new Lazy<IAttachmentDomainService>(() =>
    {
        var attachmentRepositoryMock = new Mock<IRepository<Attachment, Guid>>();
        var loggerMock = new Mock<ILogger<OSSAttachmentDomainService>>();
        var uploadConfigAccessHelperMock = new Mock<IOptions<UploadConfig>>();
        var unitOfWorkMock = new Mock<IUnitOfWork>();
        Guid id = Guid.NewGuid();

        attachmentRepositoryMock.Setup(t => t.Add(new Attachment() { Id = id })).Returns(new Attachment() { Id = id });

        uploadConfigAccessHelperMock.Setup(o => o.Value).Returns(new UploadConfig()
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
                            { "gif", 410221112 },
                            { "zip", 100016719 }
                        }
                    }
                }
            }
        });
        var ossSettingMock = new Mock<IOptions<OSSSetting>>();

        string setting = File.ReadAllText("OSSSetting.json");
        ossSettingMock.Setup(o => o.Value).Returns(setting.FromJson<OSSSetting>());

        return new OSSAttachmentDomainService(
            attachmentRepositoryMock.Object,
            loggerMock.Object,
            uploadConfigAccessHelperMock.Object,
            ossSettingMock.Object,
            unitOfWorkMock.Object);
    });

    private IAttachmentDomainService Service => LazyService.Value;

    [TestMethod]
    public void InitializeLargeFileUploadTaskTest()
    {
        APIReturnInfo<string> result =
            this.Service.InitializeLargeFileUploadTask("test", "RootHome", "ChromeSetup.zip", 5242880, 9016719);

        Assert.IsTrue(result.State);
    }

    [TestMethod]
    public void TaskidTest()
    {
        string taskId =
            "JC4dIM0FO0Ng4ujFQDteFt8++q8DJUwYyKwD9bKgbVzlFcvAo4bdq69/7yhrrHSIMJQP7szseFZcCuhHtDIblz+W9QprFfweCGzCkFxbeHv4HW1Da9luK6tHJLJlG0zp7iz00/qpCZndhp4ccmSz86BWpmek+nLD59Q7jozJL7Ivd6coAOJxNdB0xOmNtdWJjSdvSEDvq8jtSfRmzzpQ6ThkAfJS71KT7UBeqKMGDFEcEL/vUzr5nFdUCc1+tbvtYgunplX7GErB3hOvkLeQbTX43DedLViwwUp1oKKX8jfjZIo5pb9WMIvtedEvNiQRoFamZ6T6csPn1DuOjMkvsi93pygA4nE10HTE6Y211Yn+ce11eDDizj2oPGZLXMvqXN+tbstQRHRJGRivclBWnV6QQ5qbrh3RaRPXhndVVSa6Q6ewLm3iZbcWiq3n+8kqU7uYLKjsd34/wnrRJaqz8ksOS13Kz08GakgLnDYyO4zcXnlx/SQkto79uoc3jbEqplz3JQQp4eoyM4MTLNk29Q==";
        
        APIReturnInfo<MultipartUploadResultValue> result =
            this.Service.Upload(taskId,null);
        
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