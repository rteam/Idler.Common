using System;
using System.ComponentModel.DataAnnotations;

namespace Idler.Common.Core.Domain
{
    [Serializable]
    public class EntityWithStringNoTrace : IEntity<string>
    {
        /// <summary>
        /// Id
        /// </summary>
        [Required]
        public string Id { get; set; }
    }
}