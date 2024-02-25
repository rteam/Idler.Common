using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;

namespace Idler.Common.Core.Domain
{
    [Serializable]
    public abstract class BaseEntity<TKey> : IEntity<TKey>
    {
        /// <summary>
        /// Id
        /// </summary>
        public abstract TKey Id { get; protected set; }

        /// <summary>
        /// 设置Id
        /// </summary>
        /// <param name="Id"></param>
        public void SetId(TKey Id)
        {
            this.Id = Id;
        }
    }

    public abstract class BaseEntity
    {

    }
}