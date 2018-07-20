using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;

namespace BattleSceneEditor
{
    internal struct BattleMapInfo
    {
        public int XCount;
        public int YCount;
        public int[,] Cells;
        public Dictionary<string, string> Attrs;

        public static BattleMapInfo GetMapFromFile(string name)
        {
            StreamReader sr = new StreamReader(name);
            BattleMapInfo map = new BattleMapInfo();
            var datas = sr.ReadLine().Split('\t');
            map.XCount = int.Parse(datas[0]);
            map.YCount = int.Parse(datas[1]);
            map.Cells = new int[map.XCount, map.YCount];
            for (int i = 0; i < map.YCount; i++)
            {
                string line = sr.ReadLine();
                if (line != null)
                {
                    string[] mapinfos = line.Split('\t');
                    for (int j = 0; j < map.XCount; j++)
                    {
                        map.Cells[j, i] = int.Parse(mapinfos[j]);
                    }
                }
            }

            map.Attrs = new Dictionary<string, string>();
            string ln;
            while ((ln = sr.ReadLine()) != null)
            {
                datas = ln.Split('=');
                if (datas.Length != 2)
                    continue;
                map.Attrs[datas[0]] = datas[1];
            }
            sr.Close();
            return map;
        }

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

    internal struct BattleMapUnitInfo
    {
        public int X;
        public int Y;
        public int UnitId;
        public Color Color;
    }
}