using System;

namespace System
{
    /// <summary>
    /// 日期助手
    /// </summary>
    public static class DateTimeExtensions
    {
        /// <summary>
        /// 返回两个时间的间距，用秒来表示
        /// </summary>
        /// <param name="dt1">当前时间</param>
        /// <param name="dt2">对比时间</param>
        /// <param name="inputtimetype">返回时间类型</param>
        /// <returns></returns>
        public static double GetTimeSpan(this DateTime dt1, DateTime dt2, TimeType inputtimetype)
        {
            TimeSpan ts1 = new TimeSpan(dt1.Ticks);
            TimeSpan ts2 = new TimeSpan(dt2.Ticks);
            TimeSpan ts = ts2.Subtract(ts1).Duration();
            switch (inputtimetype)
            {
                case TimeType.秒:
                    return ts.TotalSeconds;
                case TimeType.分:
                    return ts.TotalMinutes;
                case TimeType.小时:
                    return ts.TotalHours;
                default:
                    return ts.TotalDays;
            }
        }

        /// <summary>
        /// 是否为指定时间
        /// </summary>
        /// <param name="inputDateTime">参照日期</param>
        /// <param name="targetDateTime">比较目标</param>
        /// <param name="toleratedErroInSecs">容差（默认为1秒）</param>
        public static bool IsOnTime(this DateTime inputDateTime, DateTime targetDateTime, int toleratedErroInSecs = 1)
        {
            return (Math.Abs((targetDateTime - inputDateTime).TotalMilliseconds) <= (toleratedErroInSecs * 1000));
        }

        /// <summary>
        /// 是否为指定时间（当前日期作为参照日期）
        /// </summary>
        /// <param name="targetDateTime">比较目标</param>
        /// <param name="toleratedErroInSecs">容差（默认为1秒）</param>
        /// <returns></returns>
        public static bool IsOnTime(this DateTime targetDateTime, int toleratedErroInSecs = 1)
        {
            return DateTime.Now.IsOnTime(targetDateTime, toleratedErroInSecs);
        }

        /// <summary>
        /// 是否为今天（不精确判断）
        /// </summary>
        /// <param name="inputDate"></param>
        /// <returns></returns>
        public static bool ToDay(this DateTime inputDate)
        {
            return DateTime.Now.Day == inputDate.Day;
        }

        /// <summary>
        /// 是否为今年（不精确判断）
        /// </summary>
        /// <param name="inputDate"></param>
        /// <returns></returns>
        public static bool ThisYear(this DateTime inputDate)
        {
            return DateTime.Now.Year == inputDate.Year;
        }

        /// <summary>
        /// 是否为本月（不精确判断）
        /// </summary>
        /// <param name="inputDate"></param>
        /// <returns></returns>
        public static bool ThisMonth(this DateTime inputDate)
        {
            return DateTime.Now.Month == inputDate.Month;
        }

        /// <summary>
        /// 返回日期的字符串拼接形式yyyyMMdd
        /// </summary>
        /// <param name="dt1"></param>
        /// <returns></returns>
        public static string GetDateString(this DateTime dt1)
        {
            return $"{dt1:yyyyMMdd}";
        }

        /// <summary>
        /// 返回当前日期的字符串拼接形式yyyyMMdd
        /// </summary>
        /// <returns></returns>
        public static string GetDateString()
        {
            return DateTime.Now.GetDateString();
        }

        /// <summary>
        /// 返回指定时间日期的完整拼接字符串yyyyMMddHHmmss
        /// </summary>
        /// <param name="dt1"></param>
        /// <returns></returns>
        public static string GetDateTimeString(this DateTime dt1)
        {
            return $"{dt1:yyyyMMddHHmmss}";
        }

        /// <summary>
        /// 返回当前日期的完整拼接字符串yyyyMMddHHmmss
        /// </summary>
        /// <returns></returns>
        public static string GetDateTimeString()
        {
            return DateTime.Now.GetDateTimeString();
        }

        /// <summary>
        /// 返回指定时间的完整拼接字符串HHmmss
        /// </summary>
        /// <param name="dt1"></param>
        /// <returns></returns>
        public static string GetTimeString(this DateTime dt1)
        {
            return $"{dt1:HHmmss}";
        }

        /// <summary>
        /// 返回当前时间的完整拼接字符串HHmmss
        /// </summary>
        /// <returns></returns>
        public static string GetTimeString()
        {
            return DateTime.Now.GetTimeString();
        }

        /// <summary>
        ///  时间戳转本地时间-时间戳精确到秒
        /// </summary> 
        public static DateTime ToLocalTimeDateBySeconds(this long timestamp)
        {
            var dto = DateTimeOffset.FromUnixTimeSeconds(timestamp);
            return dto.ToLocalTime().DateTime;
        }

        /// <summary>
        ///  时间戳转本地时间-时间戳精确到秒
        /// </summary> 
        public static DateTime ToLocalTimeDateBySeconds(this string timestamp)
        {
            if (timestamp.IsEmpty())
                return DateTime.MinValue;

            return timestamp.LongByString().ToLocalTimeDateBySeconds();
        }

        /// <summary>
        ///  时间转时间戳Unix-时间戳精确到秒
        /// </summary> 
        public static long ToUnixTimestampBySeconds(this DateTime dt)
        {
            DateTimeOffset dto = new DateTimeOffset(dt);
            return dto.ToUnixTimeSeconds();
        }


        /// <summary>
        ///  时间戳转本地时间-时间戳精确到毫秒
        /// </summary> 
        public static DateTime ToLocalTimeDateByMilliseconds(this long timestamp)
        {
            var dto = DateTimeOffset.FromUnixTimeMilliseconds(timestamp);
            return dto.ToLocalTime().DateTime;
        }

        /// <summary>
        ///  时间戳转本地时间-时间戳精确到毫秒
        /// </summary> 
        public static DateTime ToLocalTimeDateByMilliseconds(this string timestamp)
        {
            if (timestamp.IsEmpty())
                return DateTime.MinValue;

            return timestamp.LongByString().ToLocalTimeDateByMilliseconds();
        }

        /// <summary>
        ///  时间转时间戳Unix-时间戳精确到毫秒
        /// </summary> 
        public static long ToUnixTimestampByMilliseconds(this DateTime dt)
        {
            DateTimeOffset dto = new DateTimeOffset(dt);
            return dto.ToUnixTimeMilliseconds();
        }
    }

    /// <summary>
    /// 时间类型
    /// </summary>
    public enum TimeType
    {
        秒,
        分,
        小时,
        日
    }
}