using System.Collections.Generic;
using TaleofMonsters.Core;
using ConfigDatas;
using TaleofMonsters.Core.Interface;

namespace TaleofMonsters.DataType.User.Mem
{
    public class MemMergeData
    {
         [FieldIndex(Index = 1)]
        public int Target;
         [FieldIndex(Index =2)]
        public List<List<IntPair>> Methods;

        public MemMergeData()
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

    internal class CompareByMethod : IComparer<MemMergeData>
    {
        #region IComparer<MemMergeData> 成员

        public int Compare(MemMergeData x, MemMergeData y)
        {
            EquipConfig ea = ConfigData.GetEquipConfig(x.Target);
            EquipConfig eb = ConfigData.GetEquipConfig(y.Target);

            if (ea.Level != eb.Level)
            {
                return ea.Level.CompareTo(eb.Level);
            }
            if (ea.Quality != eb.Quality)
            {
                return ea.Quality.CompareTo(eb.Quality);
            }
            return ea.Id.CompareTo(eb.Id);
        }

        #endregion
    }
}
