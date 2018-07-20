using System;
using System.Collections.Generic;

namespace TaleofMonsters.Datas.Maps
{
    internal struct BattleMapInfo
    {
        public string Name;

        public int XCount;
        public int YCount;
        public int[,] Cells;
        public Dictionary<string, string> Attrs;

        public int[] LeftMon
        {
            get
            {
                if (!Attrs.ContainsKey("LeftMon"))
                    return new int[0];
                return ToIntArray(Attrs["LeftMon"]);
            }
        }
        public int[] RightMon
        {
            get
            {
                if (!Attrs.ContainsKey("RightMon"))
                    return new int[0];
                return ToIntArray(Attrs["RightMon"]);
            }
        }
        public int[] ColumnMiddle
        {
            get
            {
                if (!Attrs.ContainsKey("ColumnMiddle"))
                    return new int[0];
                return ToIntArray(Attrs["ColumnMiddle"]);
            }
        }
        public int[] ColumnCompete
        {
            get
            {
                if (!Attrs.ContainsKey("ColumnCompete"))
                    return new int[0];
                return ToIntArray(Attrs["ColumnCompete"]);
            }
        }
        public int TowerStar
        {
            get
            {
                if (!Attrs.ContainsKey("TowerStar"))
                    return 0;
                return int.Parse(Attrs["TowerStar"]);
            }
        }
        private int[] ToIntArray(string s)
        {
            var datas = s.Split(';');
            return Array.ConvertAll(datas, s1 => int.Parse(s1));
        }
    }
}
