using System;
using System.Collections.Generic;
using System.IO;
using NarlonLib.Math;
using TaleofMonsters.Controler.Loader;
using TaleofMonsters.Core;
using TaleofMonsters.DataType.User.Db;
using TaleofMonsters.MainItem.Scenes;

namespace TaleofMonsters.DataType.Scenes
{
    internal static class SceneBook
    {
        public static void LoadSceneFile(int mapWidth, int mapHeight, string filePath, Random r, SceneInfo info)
        {
            StreamReader sr = new StreamReader(DataLoader.Read("Scene", string.Format("{0}.txt", filePath)));
            int xoff = 0, yoff = 0, wid = 0, height = 0;

            string line;
            while ((line = sr.ReadLine()) != null)
            {
                string[] datas = line.Split('=');
                string tp = datas[0].Trim();
                string parm = datas[1].Trim();
                switch (tp)
                {
                    case "startx": info.Xoff = int.Parse(parm) * mapWidth / 1422; break;
                    case "starty": info.Yoff = int.Parse(parm) * mapHeight / 855 + 50; break; //50为固定偏移
                    case "width": info.XCount = int.Parse(parm); break;
                    case "height": info.YCount = int.Parse(parm); break;
                    case "startpoint": info.StartPos = int.Parse(parm); break;
                    case "revivepoint": info.RevivePos = int.Parse(parm); break;
                    case "data": ReadBody(sr, mapWidth, mapHeight, r, info); break;
                }
            }

            sr.Close();
        }

        private static void ReadBody(StreamReader sr, int mapWidth, int mapHeight, Random r, SceneInfo info)
        {
            int cellWidth = GameConstants.SceneTileStandardWidth * mapWidth / 1422;
            int cellHeight = GameConstants.SceneTileStandardHeight * mapHeight / 855;
            Dictionary<int, List<SceneManager.ScenePosData>> randomGroup = new Dictionary<int, List<SceneManager.ScenePosData>>();
            for (int i = 0; i < info.YCount; i++)
            {
                string[] data = sr.ReadLine().Split('\t');
                for (int j = 0; j < info.XCount; j++)
                {
                    int val = int.Parse(data[j]);
                    if (val == 0)
                    {
                        continue;
                    }

                    int lineOff = (int)(cellWidth * (info.YCount - i - 1) * GameConstants.SceneTileGradient);
                    SceneManager.ScenePosData so = new SceneManager.ScenePosData
                    {
                        Id = val,
                        X = info.Xoff + j * cellWidth + lineOff,
                        Y = info.Yoff + i * cellHeight,
                        Width = cellWidth,
                        Height = cellHeight
                    };
                    if (val < 1000) //随机组
                    {
                        so.Id = (info.YCount - i) * 1000 + j + 1;
                        if (!randomGroup.ContainsKey(val))
                            randomGroup[val] = new List<SceneManager.ScenePosData>();
                        randomGroup[val].Add(so);
                    }
                    else
                    {
                        info.MapData.Add(so);
                    }
                }
            }

            RandomSequence rs = new RandomSequence(randomGroup.Count, r);
            for (int i = 0; i < Math.Ceiling(randomGroup.Keys.Count * 0.5f); i++)
                foreach (var randPos in randomGroup[rs.NextNumber() + 1])
                    info.MapData.Add(randPos);

            string line;
            while ((line = sr.ReadLine()) != null)
            {
                string[] data = line.Split('\t');
                if (data.Length < 2)
                    continue;

                var posData = new DbSceneSpecialPosData();
                posData.Id = int.Parse(data[0]);
                posData.Type = data[1];
                posData.MapSetting = true;
                if (data.Length > 2)
                    posData.Info = int.Parse(data[2]);
                info.SpecialData.Add(posData);
            }
        }

    }
}
