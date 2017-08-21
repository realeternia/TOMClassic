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
        public static SceneInfo LoadSceneFile(int id, int mapWidth, int mapHeight, string filePath, Random r)
        {
            StreamReader sr = new StreamReader(DataLoader.Read("Scene", string.Format("{0}.txt", filePath)));
            SceneInfo info = new SceneInfo(id);

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

            return info;
        }

        private static void ReadBody(StreamReader sr, int mapWidth, int mapHeight, Random r, SceneInfo info)
        {
            int cellWidth = GameConstants.SceneTileStandardWidth * mapWidth / 1422;
            int cellHeight = GameConstants.SceneTileStandardHeight * mapHeight / 855;
            Dictionary<int, List<SceneInfo.SceneScriptPosData>> randomGroup = new Dictionary<int, List<SceneInfo.SceneScriptPosData>>();
            for (int i = 0; i < info.YCount; i++)
            {
                string[] datas = sr.ReadLine().Split('\t');
                for (int j = 0; j < info.XCount; j++)
                {
                    string numberStr = datas[j];
                    char cellTag = (char)0;
                    if (numberStr[0] >= 'a' && numberStr[0] <= 'z') //是个字母
                    {
                        cellTag = numberStr[0];
                        numberStr = numberStr.Substring(1);
                    }
                    int cellIndex = int.Parse(numberStr);
                    if (cellIndex == 0)
                    {
                        continue;
                    }

                    int lineOff = (int)(cellWidth * (info.YCount - i - 1) * GameConstants.SceneTileGradient);
                    SceneInfo.SceneScriptPosData so = new SceneInfo.SceneScriptPosData
                    {
                        Id = cellIndex,
                        X = info.Xoff + j * cellWidth + lineOff,
                        Y = info.Yoff + i * cellHeight,
                        Width = cellWidth,
                        Height = cellHeight
                    };
                    if (cellTag == 'r') //随机组
                    {
                        so.Id = (info.YCount - i) * 1000 + j + 1;
                        if (!randomGroup.ContainsKey(cellIndex))
                            randomGroup[cellIndex] = new List<SceneInfo.SceneScriptPosData>();
                        randomGroup[cellIndex].Add(so);
                    }
                    else if (cellTag == 'h') //隐藏组
                    {
                        so.HiddenIndex = cellIndex;
                        info.MapData.Add(so);
                        info.HiddenCellCount++;
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

                var posData = new SceneInfo.SceneScriptSpecialData();
                posData.Id = int.Parse(data[0]);
                posData.Type = data[1];
                if (data.Length > 2)
                    posData.Info = int.Parse(data[2]);
                info.SpecialData.Add(posData);
            }
        }

    }
}
