using System.Drawing;
using System.IO;

namespace BattleSceneEditor
{
    internal struct BattleMap
    {
        public int XCount;
        public int YCount;
        public int[,] Cells;
        public BattleMapUnitInfo[] LeftUnits;
        public BattleMapUnitInfo[] RightUnits;

        public static BattleMap GetMapFromFile(string name)
        {
            StreamReader sr = new StreamReader(name);
            BattleMap map = new BattleMap();
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
            var unitCount = int.Parse(sr.ReadLine());//左边单位布置
            map.LeftUnits = new BattleMapUnitInfo[unitCount];
            for (int i = 0; i < unitCount; i++)
            {
                string[] unitinfos = sr.ReadLine().Split('\t');
                map.LeftUnits[i] = new BattleMapUnitInfo
                {
                    X = int.Parse(unitinfos[0]),
                    Y = int.Parse(unitinfos[1]),
                    UnitId = int.Parse(unitinfos[2])
                };
            }

            unitCount = int.Parse(sr.ReadLine());//右边单位布置
            map.RightUnits = new BattleMapUnitInfo[unitCount];
            for (int i = 0; i < unitCount; i++)
            {
                string[] unitinfos = sr.ReadLine().Split('\t');
                map.RightUnits[i] = new BattleMapUnitInfo
                {
                    X = int.Parse(unitinfos[0]),
                    Y = int.Parse(unitinfos[1]),
                    UnitId = int.Parse(unitinfos[2])
                };
            }
            sr.Close();
            return map;
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