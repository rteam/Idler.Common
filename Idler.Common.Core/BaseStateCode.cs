namespace Idler.Common.Core
{
    /// <summary>
    /// 基础状态代码
    /// </summary>
    public class BaseStateCode
    {
        /// <summary>
        /// 成功
        /// </summary>
        public const int STATE_CODE_SUCCESS = 0;

        /// <summary>
        /// 失败（因未知原因）
        /// </summary>
        public const int STATE_CODE_ERROR = 1001;
        /// <summary>
        /// 一般系统保护性错误
        /// </summary>
        public const int STATE_CODE_1002 = 1002;
        /// <summary>
        /// 对象为空
        /// </summary>
        public const int STATE_CODE_1004 = 1004;
        /// <summary>
        /// 数据格式不正确
        /// </summary>
        public const int STATE_CODE_1008 = 1008;
        /// <summary>
        /// 认证失败
        /// </summary>
        public const int STATE_CODE_1101 = 1101;

        /// <summary>
        /// 服务器端模型验证失败
        /// 调用RESTAPI时模型验证失败时返回
        /// </summary>
        public const int STATE_CODE_2001 = 2001;
        /// <summary>
        /// 要修改的信息不存在
        /// 调用Put方法更新信息时被更新的信息不存在
        /// </summary>
        public const int STATE_CODE_2002 = 2002;
        /// <summary>
        /// 信息删除失败
        /// 调用Delete方法删除信息出现错误时
        /// </summary>
        public const int STATE_CODE_2003 = 2003;
        /// <summary>
        /// 信息未添加，该信息已存在。
        /// RESTAPI 的Post操作时返回
        /// </summary>
        public const int STATE_CODE_2004 = 2004;
        /// <summary>
        /// 请求的信息不存在
        /// 调用API时返回
        /// </summary>
        public const int STATE_CODE_2005 = 2005;

        /// <summary>
        /// 授权失败
        /// </summary>
        public const int STATE_CODE_AuthorizationFailed = 101;
    }
}