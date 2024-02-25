using System;

namespace Idler.Common.Core.Domain
{
    /// <summary>
    /// 不跟踪对象
    /// </summary>
    [Serializable]
    public abstract class EntityWithGuidNoTrace : IEntity<Guid>
    {
        protected Guid id;
        /// <summary>
        /// Id
        /// </summary>
        public Guid Id
        {
            get
            {
                if (this.id.IsEmpty())
                    this.id = Guid.NewGuid();

                return this.id;
            }

            set
            {
                this.id = value;
            }
        }
    }
}