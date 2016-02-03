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
            map.cells = new int[9,4];
            for (int i = 0; i < 4; i++)
            {
                string line = sr.ReadLine();
                if (line != null)
                {
                    string[] mapinfos = line.Split('\t');
                    for (int j = 0; j < 9; j++)
                    {
                        map.cells[j, i] = int.Parse(mapinfos[j]);
                    }
                }
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
                    Brush sBrush = new SolidBrush(Color.FromName(ConfigDatas.ConfigData.GetTileConfig(mapInfo.cells[i,j]).Color));
                    g.FillRectangle(sBrush, i * 11+1, j * 11 + 23+1, 9, 9);
                    sBrush.Dispose();
                }
            }
            g.Dispose();
            return img;
        }
    }
}
