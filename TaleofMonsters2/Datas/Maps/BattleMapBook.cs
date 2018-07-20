using System.Collections.Generic;
using System.Drawing;
using System.IO;
using ConfigDatas;
using TaleofMonsters.Core.Loader;

namespace TaleofMonsters.Datas.Maps
{
    internal static class BattleMapBook
    {
        private static Dictionary<string, BattleMapInfo> mapType = new Dictionary<string, BattleMapInfo>();
        
        public static BattleMapInfo GetMap(string name)
        {
            if (!mapType.ContainsKey(name))
            {
                var mapData = GetMapFromFile(string.Format("{0}.map", name));
                mapData.Name = name;
                mapType.Add(name, mapData);
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

            mapInfo.Attrs = new Dictionary<string, string>();
            string ln;
            while ((ln = sr.ReadLine()) != null)
            {
                datas = ln.Split('=');
                if (datas.Length != 2)
                    continue;
                mapInfo.Attrs[datas[0]] = datas[1];
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
