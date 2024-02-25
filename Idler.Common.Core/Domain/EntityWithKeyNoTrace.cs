using System.ComponentModel.DataAnnotations;

namespace Idler.Common.Core.Domain
{
    public abstract class EntityWithKeyNoTrace : EntityWithGuidNoTrace
    {
        /// <summary>
        /// 初始化Id
        /// </summary>
        /// <param name="Key"></param>
        public EntityWithKeyNoTrace(string Key)
        {
            this.Key = Key;
        }

        /// <summary>
        /// 创建空的对象
        /// </summary>
        public EntityWithKeyNoTrace()
        {

        }

        /// <summary>
        /// Key
        /// </summary>
        [Required(ErrorMessage = "Key不能为空")]
        [StringLength(100, ErrorMessage = "Key长度不能超过100个字")]
        public virtual string Key { get; set; }
    }
}