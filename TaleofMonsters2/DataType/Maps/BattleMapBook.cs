using System.Collections.Generic;
using System.IO;
using TaleofMonsters.Controler.Loader;
using System.Drawing;

namespace TaleofMonsters.DataType.Maps
{
    static class BattleMapBook
    {
        static Dictionary<string, BattleMap> mapType = new Dictionary<string, BattleMap>();

        static public BattleMap GetMap(string name)
        {
            if (!mapType.ContainsKey(name))
            {
                mapType.Add(name, GetMapFromFile(string.Format("{0}.map", name)));
            }
            return mapType[name];
        }

        static private BattleMap GetMapFromFile(string name)
        {
            StreamReader sr = new StreamReader(DataLoader.Read("Map", name));
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
            var unitCount = int.Parse(sr.ReadLine());//单位布置
            map.Info = new BattleMapUnitInfo[unitCount];
            for (int i = 0; i < unitCount; i++)
            {
                string[] unitinfos = sr.ReadLine().Split('\t');
                map.Info[i] = new BattleMapUnitInfo
                    {
                        X = int.Parse(unitinfos[0]),
                        Y = int.Parse(unitinfos[1]),
                        UnitId = int.Parse(unitinfos[2])
                    };
            }
			sr.Close();
            return map;
        }

        static public Image GetMapImage(string name)
        {
            Image img = new Bitmap(100, 100);
            BattleMap mapInfo = GetMap(name);
            Graphics g = Graphics.FromImage(img);
            for(int i=0;i<9;i++)
            {
                for (int j = 0; j < 4; j++)
                {
                    Brush sBrush = new SolidBrush(Color.FromName(ConfigDatas.ConfigData.GetTileConfig(mapInfo.Cells[i,j]).Color));
                    g.FillRectangle(sBrush, i * 11+1, j * 11 + 23+1, 9, 9);
                    sBrush.Dispose();
                }
            }
            g.Dispose();
            return img;
        }
    }
}
