using System;

namespace WT.Components.Common.Utility
{
    public static class DateTimeUtil
    {
        /// <summary>
        /// 获取时间戳
        /// </summary>
        /// <returns></returns>
        public static long GetTimeStemp()
        {
            TimeSpan ts = DateTime.Now - Convert.ToDateTime("1970-01-01");
            return (long)ts.TotalSeconds;
        }

        /// <summary>
        /// 时间戳转为C#时间
        /// </summary>
        /// <param name="timeStemp">Unix时间戳格式</param>
        /// <returns>C#格式时间</returns>
        public static DateTime GetTime(long timeStemp)
        {
            DateTime dtStart = TimeZone.CurrentTimeZone.ToLocalTime(new DateTime(1970, 1, 1));
            long lTime = long.Parse(timeStemp + "0000000");
            TimeSpan toNow = new TimeSpan(lTime);
            return dtStart.Add(toNow);
        }

        public static DateTime FROM_UNIXTIME(long timeStamp)  //转时间
        {
            return DateTime.Parse("1970-01-01 00:00:00").AddSeconds(timeStamp);
        }
        public static long UNIX_TIMESTAMP(DateTime dateTime)  //转时间戳
        {
            return (dateTime.Ticks - DateTime.Parse("1970-01-01 00:00:00").Ticks) / 10000000;
        }
    }
}
