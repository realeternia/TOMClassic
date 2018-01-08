using System.Collections.Generic;
using System.IO;
using ConfigDatas;
using TaleofMonsters.Controler.Loader;
using System.Drawing;

namespace TaleofMonsters.DataType.Maps
{
    internal static class BattleMapBook
    {
        static Dictionary<string, BattleMapInfo> mapType = new Dictionary<string, BattleMapInfo>();

        public static BattleMapInfo GetMap(string name)
        {
            if (!mapType.ContainsKey(name))
            {
                mapType.Add(name, GetMapFromFile(string.Format("{0}.map", name)));
            }
            return mapType[name];
        }

        private static BattleMapInfo GetMapFromFile(string name)
        {
            StreamReader sr = new StreamReader(DataLoader.Read("Map", name));
            BattleMapInfo mapInfo = new BattleMapInfo();
            var datas = sr.ReadLine().Split('\t');
            mapInfo.XCount = int.Parse(datas[0]);
            mapInfo.YCount = int.Parse(datas[1]);
            mapInfo.Cells = new int[mapInfo.XCount, mapInfo.YCount];
            for (int i = 0; i < mapInfo.YCount; i++)
            {
                string line = sr.ReadLine();
                if (line != null)
                {
                    string[] mapinfos = line.Split('\t');
                    for (int j = 0; j < mapInfo.XCount; j++)
                        mapInfo.Cells[j, i] = int.Parse(mapinfos[j]);
                }
            }
            var unitCount = int.Parse(sr.ReadLine());//左边单位布置
            mapInfo.LeftUnits = new BattleMapInfo.BattleMapUnitInfo[unitCount];
            for (int i = 0; i < unitCount; i++)
            {
                string[] unitinfos = sr.ReadLine().Split('\t');
                mapInfo.LeftUnits[i] = new BattleMapInfo.BattleMapUnitInfo
                    {
                        X = int.Parse(unitinfos[0]),
                        Y = int.Parse(unitinfos[1]),
                        UnitId = int.Parse(unitinfos[2])
                    };
            }

            unitCount = int.Parse(sr.ReadLine());//右边单位布置
            mapInfo.RightUnits = new BattleMapInfo.BattleMapUnitInfo[unitCount];
            for (int i = 0; i < unitCount; i++)
            {
                string[] unitinfos = sr.ReadLine().Split('\t');
                mapInfo.RightUnits[i] = new BattleMapInfo.BattleMapUnitInfo
                {
                    X = int.Parse(unitinfos[0]),
                    Y = int.Parse(unitinfos[1]),
                    UnitId = int.Parse(unitinfos[2])
                };
            }
            sr.Close();
            return mapInfo;
        }

        public static Image GetMapImage(string name, int nowtile)
        {
            Image img = new Bitmap(100, 100);
            BattleMapInfo mapInfoInfo = GetMap(name);
            Graphics g = Graphics.FromImage(img);

            int cellSize = 100/mapInfoInfo.XCount;
            int yOff = (100 - cellSize*mapInfoInfo.YCount)/2;
            for (int i = 0; i < mapInfoInfo.XCount; i++)
            {
                for (int j = 0; j < mapInfoInfo.YCount; j++)
                {
                    var tile = mapInfoInfo.Cells[i, j];
                    if (tile == 0)
                    {
                        tile = nowtile == 0 ? TileConfig.Indexer.DefaultTile : nowtile;
                    }
                    Brush sBrush = new SolidBrush(Color.FromName(ConfigData.GetTileConfig(tile).Color));
                    g.FillRectangle(sBrush, i * cellSize + 1, j * cellSize + yOff + 1, cellSize, cellSize);
                    g.DrawRectangle(Pens.White, i * cellSize + 1, j * cellSize + yOff + 1, cellSize, cellSize);
                    sBrush.Dispose();
                }
            }
            g.Dispose();
            return img;
        }
    }
}
