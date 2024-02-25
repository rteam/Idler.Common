using System.Collections.Generic;

namespace Idler.Common.Core
{
    public class ReturnPaging<T> where T : class
    {
        public ReturnPaging(int pageNum, int pageSize, int total)
            : this()
        {
            this.PageNum = pageNum;
            this.PageSize = pageSize;
            this.Total = total;
            this.Compute();
        }

        public ReturnPaging()
        {
            this.PageListInfos = new List<T>();
        }

        /// <summary>
        /// 分页数据
        /// </summary>
        public IList<T> PageListInfos { get; set; }

        /// <summary>
        /// 第几页
        /// </summary>
        public int PageNum { get; set; } = 1;
        /// <summary>
        /// 每页显示几条信息
        /// </summary>
        public int PageSize { get; set; } = 10;
        /// <summary>
        /// 总页数
        /// </summary>
        public int PageCount { get; set; }
        /// <summary>
        /// 信息总数
        /// </summary>
        public int Total { get; set; }
        /// <summary>
        /// 跳过几条
        /// </summary>
        public int Skip { get; set; }
        /// <summary>
        /// 读几条
        /// </summary>
        public int Take { get; set; }

        /// <summary>
        /// 计算分页信息
        /// </summary>
        public virtual void Compute()
        {
            if (this.Total == 0)
            {
                this.PageCount = 1;
                this.Take = this.PageSize;
                return;
            }

            this.PageCount = this.Total % this.PageSize > 0 ? this.Total / this.PageSize + 1 : this.Total / this.PageSize;

            if (this.PageNum > this.PageCount)
                this.PageNum = this.PageCount;

            this.Skip = (this.PageNum - 1) * this.PageSize;
            this.Take = this.PageSize;
        }
    }
}