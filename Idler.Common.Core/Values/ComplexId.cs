using System;

namespace Idler.Common.Core
{
    public class ComplexId : IEquatable<ComplexId>
    {
        public const string SEPARATOR = "@";

        /// <summary>
        /// 组装Id
        /// </summary>
        /// <param name="mainId">主Id</param>
        /// <param name="limitId">范围限制Id</param>
        /// <returns></returns>
        public static string ConcatId(string mainId, string limitId)
        {
            mainId.ThrowIfNull(nameof(mainId));
            limitId.ThrowIfNull(nameof(limitId));
            
            return string.Concat(mainId, SEPARATOR, limitId);
        }

        /// <summary>
        /// 组装Id
        /// </summary>
        /// <param name="mainId">主Id</param>
        /// <param name="limitId">范围限制Id</param>
        /// <returns></returns>
        public static string ConcatId(string mainId, Guid limitId)
        {
            return string.Concat(mainId, SEPARATOR, limitId);
        }

        public ComplexId(string mainId, string limitId)
        {
            this.MainId = mainId;
            this.LimitId = limitId;
        }


        /// <summary>
        /// 拆分Id
        /// </summary>
        /// <param name="complexId">组装后的Id</param>
        public ComplexId(string complexId)
        {
            var t = complexId.Split(Convert.ToChar(SEPARATOR));
            if (t.Length != 2)
                throw new AggregateException("ComplexId格式异常");

            this.MainId = t[0];
            this.LimitId = t[1];
        }

        /// <summary>
        /// 范围限制Id
        /// </summary>
        public string LimitId { get; set; }

        /// <summary>
        /// 主Id
        /// </summary>
        public string MainId { get; set; }

        public bool Equals(ComplexId other)
        {
            return string.Equals(MainId, other.MainId, StringComparison.OrdinalIgnoreCase)
                   && string.Equals(LimitId, other.LimitId, StringComparison.OrdinalIgnoreCase);
        }

        public override string ToString()
        {
            return ConcatId(this.MainId, this.LimitId);
        }
    }
}