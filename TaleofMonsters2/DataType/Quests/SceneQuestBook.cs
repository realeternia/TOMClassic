using System;
using System.Collections.Generic;
using System.IO;
using ConfigDatas;
using NarlonLib.Math;
using TaleofMonsters.Controler.Loader;
using TaleofMonsters.Core;
using TaleofMonsters.DataType.User;
using TaleofMonsters.DataType.User.Db;
using TaleofMonsters.MainItem.Scenes;

namespace TaleofMonsters.DataType.Quests
{
    internal static class SceneQuestBook
    {

        public static void LoadSceneFile(int mapWidth, int mapHeight, string filePath, Random r,
            List<SceneManager.ScenePosData> cachedMapData, List<DbSceneSpecialPosData> cachedSpecialData)
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
                    case "startx": xoff = int.Parse(parm) * mapWidth / 1422; break;
                    case "starty": yoff = int.Parse(parm) * mapHeight / 855 + 50; break; //50为固定偏移
                    case "width": wid = int.Parse(parm); break;
                    case "height": height = int.Parse(parm); break;
                    case "startpoint": Scene.Instance.StartPos = int.Parse(parm); break;
                    case "revivepoint": Scene.Instance.RevivePos = int.Parse(parm); break;
                    case "data": ReadBody(sr, mapWidth, mapHeight, r, cachedMapData, cachedSpecialData, wid, height, xoff, yoff); break;
                }
            }

            sr.Close();
        }

        private static void ReadBody(StreamReader sr, int mapWidth, int mapHeight, Random r,
            List<SceneManager.ScenePosData> cachedMapData, List<DbSceneSpecialPosData> cachedSpecialData,
            int wid, int height, int xoff, int yoff)
        {
            int cellWidth = GameConstants.SceneTileStandardWidth * mapWidth / 1422;
            int cellHeight = GameConstants.SceneTileStandardHeight * mapHeight / 855;
            Dictionary<int, List<SceneManager.ScenePosData>> randomGroup = new Dictionary<int, List<SceneManager.ScenePosData>>();
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
                    SceneManager.ScenePosData so = new SceneManager.ScenePosData
                    {
                        Id = val,
                        X = xoff + j * cellWidth + lineOff,
                        Y = yoff + i * cellHeight,
                        Width = cellWidth,
                        Height = cellHeight
                    };
                    if (val < 1000) //随机组
                    {
                        so.Id = (height - i) * 1000 + j + 1;
                        if (!randomGroup.ContainsKey(val))
                            randomGroup[val] = new List<SceneManager.ScenePosData>();
                        randomGroup[val].Add(so);
                    }
                    else
                    {
                        cachedMapData.Add(so);
                    }
                }
            }

            RandomSequence rs = new RandomSequence(randomGroup.Count, r);
            for (int i = 0; i < Math.Ceiling(randomGroup.Keys.Count * 0.5f); i++)
                foreach (var randPos in randomGroup[rs.NextNumber() + 1])
                    cachedMapData.Add(randPos);

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
                if (data.Length > 3)
                    posData.Info2 = int.Parse(data[3]);
                cachedSpecialData.Add(posData);
            }
        }
        public static bool IsQuestAvail(int qid)
        {
            var questConfig = ConfigData.GetSceneQuestConfig(qid);
            return IsQuestTimeAvail(questConfig) && IsQuestFlagAvail(questConfig);
        }

        private static bool IsQuestTimeAvail(SceneQuestConfig questConfig)
        {
            var minutes = Scene.Instance.TimeMinutes;
            if (questConfig.TriggerHourBegin == questConfig.TriggerHourEnd)
                return true;
            if (questConfig.TriggerHourEnd > questConfig.TriggerHourBegin)
                return minutes >= questConfig.TriggerHourBegin && minutes < questConfig.TriggerHourEnd;
            else //后半夜到第二天
                return minutes < questConfig.TriggerHourEnd || minutes >= questConfig.TriggerHourBegin;
        }

        private static bool IsQuestFlagAvail(SceneQuestConfig questConfig)
        {
            if (!string.IsNullOrEmpty(questConfig.TriggerFlagExist))
            {
                return UserProfile.InfoRecord.CheckFlag(
                    (uint)(MemPlayerFlagTypes)Enum.Parse(typeof(MemPlayerFlagTypes), questConfig.TriggerFlagExist));
            }
            if (!string.IsNullOrEmpty(questConfig.TriggerFlagNoExist))
            {
                return !UserProfile.InfoRecord.CheckFlag(
                    (uint)(MemPlayerFlagTypes)Enum.Parse(typeof(MemPlayerFlagTypes), questConfig.TriggerFlagNoExist));
            }
            return true;
        }
    }
}
