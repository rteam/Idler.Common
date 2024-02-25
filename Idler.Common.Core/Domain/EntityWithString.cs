using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel.DataAnnotations;
namespace Idler.Common.Core.Domain
{
    [Serializable]
    public class EntityWithString : BaseEntity<string>
    {
        /// <summary>
        /// Id
        /// </summary>
        [Required]
        public override string Id { get; protected set; }
    }
}