using System;
using System.Linq;

namespace Idler.Common.Core
{
    /// <summary>
    /// 标准返回体
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class APIReturnInfo<T>
    {

        /// <summary>
        /// 成功信息并返回相关对象
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public static APIReturnInfo<T> Success(T data)
        {
            return new APIReturnInfo<T>() { State = true, StateCode = BaseStateCode.STATE_CODE_SUCCESS, Data = data };
        }

        /// <summary>
        /// 成功信息（默认信息）
        /// </summary>
        /// <returns></returns>
        public static APIReturnInfo<T> Success()
        {
            return new APIReturnInfo<T>() { State = true, StateCode = BaseStateCode.STATE_CODE_SUCCESS, Message = "ok" };
        }

        /// <summary>
        /// 成功信息
        /// </summary>
        /// <param name="message">成功信息</param>
        /// <param name="args">信息中的参数</param>
        /// <returns></returns>
        public static APIReturnInfo<T> Success(string message, params object[] args)
        {
            return new APIReturnInfo<T>() { State = true, StateCode = BaseStateCode.STATE_CODE_SUCCESS, Message = string.Format(message, args) };
        }

        /// <summary>
        /// 成功信息
        /// </summary>
        /// <param name="message">成功信息</param>
        /// <returns></returns>
        public static APIReturnInfo<T> Success(string message)
        {
            return new APIReturnInfo<T>() { State = true, StateCode = BaseStateCode.STATE_CODE_SUCCESS, Message = message };
        }

        /// <summary>
        /// 错误信息
        /// </summary>
        /// <param name="message">错误信息</param>
        /// <param name="StateCode">错误代码</param>
        /// <param name="args">信息参数</param>
        /// <returns></returns>
        public static APIReturnInfo<T> Error(string message, int StateCode = BaseStateCode.STATE_CODE_ERROR, params object[] args)
        {
            return new APIReturnInfo<T>() { State = false, StateCode = BaseStateCode.STATE_CODE_ERROR, Message = string.Format(message, args) };
        }

        /// <summary>
        /// 错误信息
        /// </summary>
        /// <param name="message">错误信息</param>
        /// <param name="args">信息参数</param>
        /// <returns></returns>
        public static APIReturnInfo<T> Error(string message, params object[] args)
        {
            return new APIReturnInfo<T>() { State = false, StateCode = BaseStateCode.STATE_CODE_ERROR, Message = string.Format(message, args) };
        }

        /// <summary>
        /// 错误信息
        /// </summary>
        /// <param name="message">错误信息</param>
        /// <param name="stateCode">错误代码</param>
        /// <returns></returns>
        public static APIReturnInfo<T> Error(string message, int stateCode = BaseStateCode.STATE_CODE_ERROR)
        {
            return new APIReturnInfo<T>() { State = false, Message = message, StateCode = stateCode };
        }

        /// <summary>
        /// 授权失败
        /// </summary>
        /// <param name="message"></param>
        /// <returns></returns>
        public static APIReturnInfo<T> AuthorizationFailed(string message)
        {
            return new APIReturnInfo<T>()
            {
                State = false,
                Message = message,
                StateCode = BaseStateCode.STATE_CODE_AuthorizationFailed,
                Authorization = false
            };
        }

        /// <summary>
        /// 是否授权成功
        /// </summary>
        public bool Authorization { get; set; } = true;

        /// <summary>
        /// 状态代码
        /// </summary>
        public int StateCode { get; set; }
        /// <summary>
        /// 状态
        /// </summary>
        public bool State { get; set; }
        /// <summary>
        /// 验证返回的信息
        /// </summary>
        public string Message { get; set; }

        /// <summary>
        /// 相关对象
        /// </summary>
        public T Data { get; set; } = default(T);

    }
}
