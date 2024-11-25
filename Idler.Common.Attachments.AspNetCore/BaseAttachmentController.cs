using System;
using System.Collections.Generic;
using Idler.Common.Core;
using Microsoft.AspNetCore.Mvc;

namespace Idler.Common.Attachments.AspNetCore
{
    [Route("api/[controller]")]
    [ApiController]
    public abstract class BaseAttachmentController(IAttachmentDomainService attachmentDomainService) : ControllerBase
    {
        /// <summary>
        /// 新建
        /// </summary>
        /// <param name="addInfo"></param>
        /// <returns></returns>
        [HttpPost("")]
        public ActionResult<APIReturnInfo<AttachmentValue>> Create(AttachmentValue addInfo)
        {
            return attachmentDomainService.Create(addInfo);
        }

        /// <summary>
        /// 编辑
        /// </summary>
        /// <param name="editInfo">要编辑的信息</param>
        /// <param name="id">要编辑信息的Id</param>
        /// <returns></returns>
        [HttpPut("id")]
        public ActionResult<APIReturnInfo<AttachmentValue>> Edit(AttachmentValue editInfo, Guid id)
        {
            return attachmentDomainService.Edit(editInfo, id);
        }


        /// <summary>
        /// 删除
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpDelete("{id}")]
        public ActionResult<APIReturnInfo<AttachmentValue>> Remove(Guid id)
        {
            return attachmentDomainService.Remove(id);
        }

        /// <summary>
        /// 分页显示附件
        /// </summary>
        /// <param name="keyword">关键字</param>
        /// <param name="uploadType">上传类型</param>
        /// <param name="dependentId">依赖Id</param>
        /// <param name="pageNum">第几页</param>
        /// <param name="pageSize">每页显示几条信息</param>
        /// <returns></returns>
        [HttpGet("{uploadType}/{dependentId}/Paging")]
        public ActionResult<APIReturnInfo<ReturnPaging<AttachmentValue>>> Search(string keyword, string uploadType,
            string dependentId, int pageNum = 1, int pageSize = 20)
        {
            return attachmentDomainService.Search(keyword, uploadType, dependentId, pageNum, pageSize);
        }

        /// <summary>
        /// 查找附件
        /// </summary>
        /// <param name="uploadType">附件类型</param>
        /// <param name="dependentId">依赖Id</param>
        /// <returns></returns>
        [HttpGet("{uploadType}/{dependentId}")]
        public ActionResult<APIReturnInfo<IList<AttachmentValue>>> Find(string uploadType, string dependentId)
        {
            return attachmentDomainService.Find(uploadType, dependentId);
        }

        /// <summary>
        /// 从服务器删除指定依赖的附件
        /// </summary>
        /// <param name="uploadType">附件类型</param>
        /// <param name="dependentId">依赖Id</param>
        /// <returns></returns>
        [HttpDelete("{uploadType}/{dependentId}/Server")]
        public ActionResult<APIReturnInfo<string>> RemoveFromServer(string uploadType, string dependentId)
        {
            return attachmentDomainService.RemoveFromServer(uploadType, dependentId);
        }

        /// <summary>
        /// 从服务器删除指定附件
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpDelete("{id}/Server")]
        public ActionResult<APIReturnInfo<string>> RemoveFromServer(Guid id)
        {
            return attachmentDomainService.RemoveFromServer(id);
        }

        /// <summary>
        /// 处理临时附件
        /// </summary>
        /// <param name="uploadType">附件类型</param>
        /// <param name="tempDependentId">临时依赖Id</param>
        /// <param name="dependentId">依赖Id</param>
        /// <returns></returns>
        [HttpPut("{uploadType}/{dependentId}/Bind/{tempDependentId}")]
        public ActionResult<APIReturnInfo<string>> HandleTemp(string uploadType, string tempDependentId,
            string dependentId)
        {
            return attachmentDomainService.HandleTemp(uploadType, tempDependentId, dependentId);
        }
    }
}