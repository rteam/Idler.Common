using System;

namespace Idler.Common.Core.Domain
{
    [Serializable]
    public abstract class EntityWithGuid : BaseEntity<Guid>
    {
        protected Guid id;
        /// <summary>
        /// Id
        /// </summary>
        public override Guid Id
        {
            get
            {
                if (this.id.IsEmpty())
                    this.id = Guid.NewGuid();

                return this.id;
            }

            protected set
            {
                this.id = value;
            }
        }
    }
}