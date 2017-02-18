using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;

namespace SceneMaker
{
    internal static class SceneManager
    {
        private struct ScenePosData
        {
            public int Id;
            public int X;
            public int Y;
            public int Width;
            public int Height;
        }

        internal enum SceneFreshReason
        {
            Load,
            Warp,
            Reset
        }

        public static List<SceneObject> RefreshSceneObjects(string filePath, int mapWidth, int mapHeight, SceneFreshReason reason)
        {
            List<ScenePosData> cachedMapData = new List<ScenePosData>();

            try
            {
                LoadSceneFile(mapWidth, mapHeight, filePath, cachedMapData);
            }
            catch (Exception e)
            {
                
                throw;
            }
         

            List<SceneObject> sceneObjects = new List<SceneObject>();
            foreach (var scenePosData in cachedMapData)
            {
                SceneObject so;
                so = new SceneObject(scenePosData.Id, scenePosData.X, scenePosData.Y, scenePosData.Width, scenePosData.Height);
                sceneObjects.Add(so);
            }

            return sceneObjects;
        }

        private static void LoadSceneFile(int mapWidth, int mapHeight, string filePath, 
            List<ScenePosData> cachedMapData)
        {
            StreamReader sr = new StreamReader(filePath);
            int xoff = 0, yoff = 0, wid = 0, height = 0;

            string line;
            while ((line = sr.ReadLine()) != null)
            {
                Debug.Print(line);
                string[] datas = line.Split('=');
                string tp = datas[0].Trim();
                string parm = datas[1].Trim();
                switch (tp)
                {
                    case "startx": xoff = int.Parse(parm) * mapWidth / 1422; break;
                    case "starty": yoff = int.Parse(parm) * mapHeight / 855 + 50; break; //50为固定偏移
                    case "width": wid = int.Parse(parm); break;
                    case "height": height = int.Parse(parm); break;
                   // case "startpoint": Scene.Instance.StartPos = int.Parse(parm); break;
                 //   case "revivepoint": Scene.Instance.RevivePos = int.Parse(parm); break;
                    case "data": ReadBody(sr, mapWidth, mapHeight, cachedMapData, wid, height, xoff, yoff); break;
                }
            }

            sr.Close();
        }

        private static void ReadBody(StreamReader sr, int mapWidth, int mapHeight, 
            List<ScenePosData> cachedMapData, int wid, int height, int xoff, int yoff)
        {
            int cellWidth = GameConstants.SceneTileStandardWidth * mapWidth / 1422;
            int cellHeight = GameConstants.SceneTileStandardHeight * mapHeight / 855;
            for (int i = 0; i < height; i++)
            {
                string[] data = sr.ReadLine().Split('\t');
                for (int j = 0; j < wid; j++)
                {
                    int val = int.Parse(data[j]);
                    if (val == 0)
                    {
                        continue;
                    }

                    int lineOff = (int)(cellWidth * (height - i - 1) * GameConstants.SceneTileGradient);
                    ScenePosData so = new ScenePosData
                    {
                        Id = val,
                        X = xoff + j * cellWidth + lineOff,
                        Y = yoff + i * cellHeight,
                        Width = cellWidth,
                        Height = cellHeight
                    };
                    cachedMapData.Add(so);
                }
            }

            string line;
            while ((line = sr.ReadLine()) != null)
            {
                //读完所有航
            }
        }

        private static int GetStringDepth(ref string str)
        {
            int tabCount = 0;
            for (int i = 0; i < str.Length; i++)
            {
                if (str[i] != '\t')
                {
                    break;
                }
                tabCount++;
            }
            str = str.Substring(tabCount);
            return tabCount;
        }

    }
}