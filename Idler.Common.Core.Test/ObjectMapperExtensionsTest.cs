using System.Collections.Generic;
using AutoMapper;
using Idler.Common.AutoMapper;
using Idler.Common.Core;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Idler.Common.Core.Test
{
    [TestClass]
    public class ObjectMapperExtensionsTest
    {
        // Test cases for ProjectTo<TDestination>(this IQueryable source)
        [TestMethod]
        public void Map_ExpandoObject()
        {
            // Setup
            IQueryable<object> source = null;
            var configuration = new MapperConfiguration(cfg => { });

            IMapper mapper = configuration.CreateMapper();


            // Exercise
            var result = mapper.Map<SettingModel>(new Dictionary<string, object>
            {
                ["Temperature"] = 0.7F,
                ["MaxTokens"] = 10,
                ["PresencePenalty"] = 3,
                ["ResponseFormat"] = "Json"
            });

            // Assert
            Assert.IsTrue(result.MaxTokens.Value == (int)10);
            Assert.IsTrue(result.PresencePenalty.Value == (int)3);
            Assert.IsTrue(result.FrequencyPenalty.Value == (int)0);
            Assert.IsNull(result.TopP);
            Assert.IsTrue(result.Temperature.Value == 0.7F);
            Assert.IsTrue(result.ResponseFormat == "Json");
        }

        private class SettingModel
        {
            /// <summary>
            /// 温度
            /// </summary>
            public float? Temperature { get; set; } = 0.5F;

            /// <summary>
            /// 输出长度
            /// </summary>
            public int? MaxTokens { get; set; }

            /// <summary>
            /// 多样性
            /// </summary>
            public float? TopP { get; set; }

            /// <summary>
            /// 重复度
            /// </summary>
            public float? PresencePenalty { get; set; } = 0;

            /// <summary>
            /// 复现度
            /// </summary>
            public float? FrequencyPenalty { get; set; } = 0;

            /// <summary>
            /// 返回格式
            /// </summary>
            public string ResponseFormat { get; set; } = "Text";
        }
    }
}