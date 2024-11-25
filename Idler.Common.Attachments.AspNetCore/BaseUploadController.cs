using Idler.Common.Core;
using Idler.Common.Core.Upload;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Idler.Common.Attachments.AspNetCore
{

    /// <summary>
    /// 文件上传
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    public abstract class BaseUploadController(IAttachmentDomainService attachmentDomainService) : ControllerBase
    {
        /// <summary>
        /// 文件上传
        /// </summary>
        /// <param name="dependentId">依赖Id</param>
        /// <param name="uploadFile">要上传的文件</param>
        /// <param name="uploadType">文件类型</param>
        /// <returns></returns>
        [HttpPost("{uploadType}/{dependentId}/Upload")]
        [Consumes("application/json", "multipart/form-data")]
        [RequestSizeLimit(1_024_000_000)]
        public ActionResult<APIReturnInfo<UploadFilInfo>> Upload(string uploadType, string dependentId,
            [FromForm(Name = "UploadFile")] IFormFile uploadFile)
        {
            if (!this.ModelState.IsValid)
                return APIReturnInfo<UploadFilInfo>.Error("请提供必要的参数");

            return attachmentDomainService.Upload(uploadType, dependentId,
                System.Web.HttpUtility.UrlDecode(uploadFile.FileName), uploadFile.Length,
                uploadFile.OpenReadStream());
        }

        /// <summary>
        /// 生成上传指定文件的授权地址
        /// </summary>
        /// <param name="objectKey">ObjectKey</param>
        /// <returns></returns>
        [HttpGet("AuthorizationUrl")]
        public ActionResult<APIReturnInfo<string>> GenerateUploadAuthorizationUrl(string objectKey)
        {
            return attachmentDomainService.GenerateUploadAuthorizationUrl(objectKey);
        }


        /// <summary>
        /// 上传配置
        /// </summary>
        /// <returns></returns>
        [HttpGet("[action]")]
        public ActionResult<UploadConfig> Config()
        {
            return attachmentDomainService.Config();
        }
    }

}
