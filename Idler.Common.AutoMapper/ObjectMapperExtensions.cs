using System.Collections.Generic;
using System.Linq;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using Idler.Common.Core;

namespace Idler.Common.AutoMapper
{
    public static class ObjectMapperExtensions
    {
        /// <summary>
        /// 默认实例
        /// </summary>
        public static IMapper Instance { get; set; }

        /// <summary>
        /// 两对象之间映射
        /// </summary>
        /// <typeparam name="TForm">来源类型</typeparam>
        /// <typeparam name="TTo">目标类型</typeparam>
        /// <param name="Source">来源对象实例</param>
        /// <returns>映射好的目标对象实例</returns>
        public static TTo Map<TFrom, TTo>(this TFrom Source)
        {
            return Instance.Map<TFrom, TTo>(Source);
        }

        /// <summary>
        /// 将来源对象的值映射到目标对象
        /// </summary>
        /// <typeparam name="TFrom">来源对象数据类型</typeparam>
        /// <typeparam name="TTo">目标对象数据类型</typeparam>
        /// <param name="Source">来源对象实例</param>
        /// <param name="Target">目标对象实例</param>
        /// <returns>映射好的目标对象实例</returns>
        public static TTo Map<TFrom, TTo>(this TFrom Source, TTo Target)
        {
            return Instance.Map<TFrom, TTo>(Source, Target);
        }

        /// <summary>
        /// 将动态类型转换为静态类型
        /// </summary>
        /// <typeparam name="TTo">目标类型</typeparam>
        /// <param name="Source">要转换的对象</param>
        /// <returns></returns>
        public static TTo Map<TTo>(this object Source)
        {
            return Instance.Map<TTo>(Source);
        }

        /// <summary>
        /// 转换APIReturnInfo中Data的类型
        /// </summary>
        /// <param name="input">待转换对象</param>
        /// <typeparam name="F">原类型</typeparam>
        /// <typeparam name="T">目标类型</typeparam>
        /// <returns></returns>
        public static APIReturnInfo<T> To<F, T>(this APIReturnInfo<F> input)
        {
            return new APIReturnInfo<T>()
            {
                State = input.State,
                Message = input.Message,
                Data = input.Data.Map<F, T>(),
                StateCode = input.StateCode
            };
        }

        /// <summary>
        /// 转换ReturnPaging中的PageListInfos类型
        /// </summary>
        /// <param name="input">待转换对象</param>
        /// <typeparam name="F">原类型</typeparam>
        /// <typeparam name="T">目标类型</typeparam>
        /// <returns></returns>
        public static ReturnPaging<T> To<F, T>(this ReturnPaging<F> input)
            where F : class
            where T : class
        {
            return new ReturnPaging<T>()
            {
                Total = input.Total,
                PageCount = input.PageCount,
                PageSize = input.PageSize,
                PageNum = input.PageNum,
                Skip = input.Skip,
                Take = input.Take,
                PageListInfos = input.PageListInfos.Map<IList<F>, IList<T>>()
            };
        }

        /// <summary>
        /// 转换APIReturnInfo<ReturnPaging>中的PageListInfos类型
        /// </summary>
        /// <param name="input">待转换对象</param>
        /// <typeparam name="F">原类型</typeparam>
        /// <typeparam name="T">目标类型</typeparam>
        /// <returns></returns>
        public static APIReturnInfo<ReturnPaging<T>> To<F, T>(this APIReturnInfo<ReturnPaging<F>> input)
            where F : class
            where T : class
        {
            return new APIReturnInfo<ReturnPaging<T>>()
            {
                State = input.State,
                Message = input.Message,
                Data = new ReturnPaging<T>()
                {
                    Total = input.Data.Total,
                    PageCount = input.Data.PageCount,
                    PageSize = input.Data.PageSize,
                    PageNum = input.Data.PageNum,
                    Skip = input.Data.Skip,
                    Take = input.Data.Take,
                    PageListInfos = input.Data.PageListInfos.Map<IList<F>, IList<T>>()
                },
                StateCode = input.StateCode
            };
        }

        /// <summary>
        /// 转换IQueryable中的对象
        /// </summary>
        /// <param name="source">待转换对象</param>
        /// <typeparam name="TDestination">目标类型</typeparam>
        /// <returns></returns>
        public static IQueryable<TDestination> ProjectTo<TDestination>(this IQueryable source) =>
            source.ProjectTo<TDestination>(Instance.ConfigurationProvider);

        /// <summary>
        /// 配置文件
        /// </summary>
        public static IConfigurationProvider Configuration => Instance.ConfigurationProvider;
    }
}