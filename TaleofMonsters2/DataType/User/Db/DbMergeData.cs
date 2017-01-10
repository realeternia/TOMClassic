using System.Collections.Generic;
using TaleofMonsters.Core;
using ConfigDatas;

namespace TaleofMonsters.DataType.User.Db
{
    public class DbMergeData
    {
        [FieldIndex(Index = 1)]
        public int Target;//武器id
        [FieldIndex(Index =2)]
        public List<List<IntPair>> Methods;//公式

        public DbMergeData()
        {
            Methods = new List<List<IntPair>>();
        }

        public void Add(List<IntPair> mthd)
        {
            Methods.Add(mthd);
        }

        public int Count
        {
            get { return Methods.Count; }
        }

        public List<IntPair> this[int index]
        {
            get { return Methods[index]; }
            set { Methods[index] = value; }
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
            if (ea.Level != eb.Level)
            {
                return ea.Level.CompareTo(eb.Level);
            }
            return ea.Id.CompareTo(eb.Id);
        }

        #endregion
    }
}
