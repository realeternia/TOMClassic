namespace TaleofMonsters.Controler.World
{
    static class TimeManager
    {
        //public static void OnDayFinish(int day)
        //{
        //    UserProfile.MemData.CheckAllTournament(day);

        //    UserProfile.Profile.dayItems.Clear();//清除所有日效果物品
        //}

        //public static void OnDayStart(int day)
        //{
        //    UserProfile.Profile.digCount = 0;
        //    UserProfile.Profile.miniGames = new List<int>();
        //}

        public static bool IsDifferDay(int time, int nowTime)
        {
            return time/86400 != nowTime/86400;
        }

        public static int GetTimeOnNextInterval(int baseTime, int nowTime,int interval)
        {
            if (baseTime == 0)
            {
                return nowTime;
            }

            return (nowTime - baseTime)/interval*interval + baseTime;
        }
    }
}
