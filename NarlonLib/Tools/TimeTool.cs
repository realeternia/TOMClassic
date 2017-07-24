using System;

namespace NarlonLib.Tools
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
        public static DateTime GetNowDateTime()
        {
            return DateTime.Now;
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

    public class RoundCounter
    {
        public int Round { get; set; }
        private float counter;

        public RoundCounter()
        {
            counter = 0;
        }

        public bool OnTick(float step)
        {
            counter += step;
            if (counter > 1)
            {
                counter -= 1;
                Round++;
                return true;
            }
            return false;
        }
    }

    public class TimeCounter
    {
        private DateTime lastTime;
        private readonly float interval;

        public double PastSeconds { get; set; }

        public TimeCounter(float interval)
        {
            this.interval = interval;
            lastTime = TimeTool.GetNowDateTime();
        }

        public void SetNow()
        {
            lastTime = DateTime.Now;
        }


        public bool OnTick()
        {
            if (TimeTool.GetNowDateTime().Subtract(lastTime).TotalSeconds > interval)
            {
                PastSeconds = (DateTime.Now - lastTime).TotalSeconds;
                lastTime = TimeTool.GetNowDateTime();
                return true;
            }
            return false;
        }
    }
}
