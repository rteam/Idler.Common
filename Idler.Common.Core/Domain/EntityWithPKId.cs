using System;

namespace Idler.Common.Core.Domain
{
    public abstract class EntityWithPKId : BaseEntity, IEntity<int>
    {
        /// <summary>
        /// Id
        /// </summary>
        public int Id { get; set; }
    }
}