using System.Collections.Generic;
using ConfigDatas;
using TaleofMonsters.Core;

namespace TaleofMonsters.Datas.User.Db
{
    public class DbMergeData
    {
        [FieldIndex(Index = 1)] public int Target;//武器id
        [FieldIndex(Index =2)] public List<IntPair> Methods;//公式

        public DbMergeData()
        {
            Methods = new List<IntPair>();
        }

        public void Set(List<IntPair> mthd)
        {
            Methods = mthd;
        }
    }

    internal class CompareByMethod : IComparer<DbMergeData>
    {
        #region IComparer<DbMergeData> 成员

        public int Compare(DbMergeData x, DbMergeData y)
        {
            EquipConfig ea = ConfigData.GetEquipConfig(x.Target);
            EquipConfig eb = ConfigData.GetEquipConfig(y.Target);
            if (ea.Quality != eb.Quality)
            {
                return ea.Quality.CompareTo(eb.Quality);
            }
            return ea.Id.CompareTo(eb.Id);
        }

        #endregion
    }
}
