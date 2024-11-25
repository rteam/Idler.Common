using Microsoft.AspNetCore.Http;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc;

namespace Idler.Common.Attachments.AspNetCore.Models
{
    /// <summary>
    /// 文件上传
    /// </summary>
    public class UploadModel : IValidatableObject
    {
        /// <summary>
        /// 上传类型
        /// </summary>
        [StringLength(50, ErrorMessage = "上传类型不能超过50个字")]
        [FromForm(Name = "UploadType")]
        public string UploadType { get; set; }

        /// <summary>
        /// 依赖Id
        /// </summary>
        [StringLength(128, ErrorMessage = "依赖Id不能超过128个字")]
        [FromForm(Name = "DependentId")]
        public string DependentId { get; set; }

        /// <summary>
        /// 待上传的文件
        /// </summary>
        [FromForm(Name = "UploadFile")]
        public IFormFile UploadFile { get; set; }

        /// <summary>
        /// 验证
        /// </summary>
        /// <param name="validationContext"></param>
        /// <returns></returns>
        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            if (UploadFile == null || UploadFile.Length <= 0)
                yield return new ValidationResult("请选择上传文件");
        }

        ///// <summary>
        ///// 检查文件是否允许上传
        ///// </summary>
        ///// <param name="config"></param>
        ///// <returns></returns>
        //public APIReturnInfo<UploadFilInfo> Verify(UploadConfig config)
        //{
        //    UploadFilInfo FileInfo = new UploadFilInfo(string.Concat(UploadType, "/"), this.UploadFile.FileName, this.UploadFile.Length);

        //    BaseReturnInfo rinfo = config.Verify(this.UploadType, FileInfo.FileExt, FileInfo.FileSize);

        //    return new APIReturnInfo<UploadFilInfo>() { State = rinfo.State, Message = rinfo.Message, Data = FileInfo };
        //}
    }
}