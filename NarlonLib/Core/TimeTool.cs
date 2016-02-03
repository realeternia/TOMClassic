using System;

namespace NarlonLib.Core
{
    public class TimeTool
    {
        public static long GetNowMiliSecond()
        {
            return DateTime.Now.Ticks/10000;
        }

        static DateTime start = TimeZone.CurrentTimeZone.ToLocalTime(new DateTime(1970, 1, 1));
        public static DateTime UnixTimeToDateTime(int sec)
        {
            return start.AddSeconds(sec);
        }

        public static int DateTimeToUnixTime(DateTime time)
        {
            return (int)(time - start).TotalSeconds;
        }

        public static int GetNowUnixTime()
        {
            return DateTimeToUnixTime(DateTime.Now);
        }

        public static double DateTimeToUnixTimeDouble(DateTime time)
        {
            return (time - start).TotalSeconds;
        }

        public static DateTime UnixTimeDoubleToDateTime(double sec)
        {
            return start.AddSeconds(sec);
        }
        
        public static bool IsNowAnotherDay(int targetTime, int offset)
        {
            DateTime previousTime = UnixTimeToDateTime(targetTime - offset);
            DateTime refreshTime = UnixTimeToDateTime(GetNowUnixTime() - offset);
            return previousTime.Year != refreshTime.Year || previousTime.DayOfYear != refreshTime.DayOfYear;
        }
    }
}
