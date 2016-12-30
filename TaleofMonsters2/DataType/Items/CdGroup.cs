namespace TaleofMonsters.DataType.Items
{
    class CdGroup
    {
        private static int[] times = new int[] {0, //空
            300, //回怒气
            300, //回魔法
            500, //双回
            500, //其他道具
            200, 
            200, 
            200, 
            200, 
            200, 
            200};

        public static int GetCDTime(int gid)
        {
            return times[gid];
        }
    }
}
