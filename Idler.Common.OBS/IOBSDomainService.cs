using System;
using Idler.Common.Core;
using Idler.Common.Core.Logging;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using OBS;
using OBS.Model;

namespace Idler.Common.OBS
{
    public interface IOBSDomainService
    {
        /// <summary>
        /// 生成上传指定文件的授权地址
        /// </summary>
        /// <param name="ObjectKey">ObjectKey</param>
        /// <returns></returns>
        APIReturnInfo<string> GenerateUploadAuthorizationUrl(string ObjectKey);
    }

    internal class OBSDomainService(IOptions<OBSSetting> OBSConfigAccessHelper, ILogger<OBSDomainService> logger)
        : IOBSDomainService
    {
        private readonly ILogger<OBSDomainService> logger = logger;

        /// <summary>
        /// 生成上传指定文件的授权地址
        /// </summary>
        /// <param name="ObjectKey">ObjectKey</param>
        /// <returns></returns>
        public APIReturnInfo<string> GenerateUploadAuthorizationUrl(string ObjectKey)
        {
            if (ObjectKey.IsEmpty())
                throw new ArgumentNullException(nameof(ObjectKey));

            try
            {
                ObsClient client = new ObsClient(OBSConfigAccessHelper.Value.AccessKey,
                    OBSConfigAccessHelper.Value.SecretKey,
                    OBSConfigAccessHelper.Value.EndPoint);
                CreateTemporarySignatureRequest request = new CreateTemporarySignatureRequest();
                request.BucketName = OBSConfigAccessHelper.Value.BucketName;
                request.ObjectKey = ObjectKey;
                request.Method = HttpVerb.PUT;
                request.Expires = 60 * 60;

                CreateTemporarySignatureResponse response = client.CreateTemporarySignature(request);
                return new APIReturnInfo<string>() { State = true, Message = "ok", Data = response.SignUrl };
            }
            catch (ObsException e)
            {
                this.logger.HandleException<ObsException>(e);
                return APIReturnInfo<string>.Error(e.ErrorMessage);
            }
        }
    }
}