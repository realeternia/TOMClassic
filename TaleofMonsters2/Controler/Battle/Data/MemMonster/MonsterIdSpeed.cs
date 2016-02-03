using System.Collections.Generic;

namespace TaleofMonsters.Controler.Battle.Data.MemMonster
{
    public struct MonsterIdSpeed
    {
        public int id;
        public int mark;
        public int speed;
    }

    internal class CompareBySpeed : IComparer<MonsterIdSpeed>
    {
        #region IComparer<MonsterIdSpeed> 成员

        public int Compare(MonsterIdSpeed x, MonsterIdSpeed y)
        {
            if (y.speed != x.speed)
                return x.speed.CompareTo(y.speed);

            if (y.mark != x.mark)
                return x.mark.CompareTo(y.mark);

            return y.id.CompareTo(x.id);
        }

        #endregion
    }
}
