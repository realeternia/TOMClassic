using System;
using System.Collections.Generic;
using System.IO;
using ConfigDatas;
using NarlonLib.Math;
using TaleofMonsters.Controler.Loader;
using TaleofMonsters.Core;
using TaleofMonsters.DataType.Scenes;
using TaleofMonsters.DataType.User;
using TaleofMonsters.DataType.User.Db;
using TaleofMonsters.MainItem.Quests.SceneQuests;
using TaleofMonsters.MainItem.Scenes;

namespace TaleofMonsters.DataType.Quests
{
    internal static class SceneQuestBook
    {
        public static void LoadSceneFile(int mapWidth, int mapHeight, string filePath, Random r,
            List<SceneManager.ScenePosData> cachedMapData, List<DbSceneSpecialPosData> cachedSpecialData)
        {
            StreamReader sr = new StreamReader(DataLoader.Read("Scene", String.Format("{0}.txt", filePath)));
            int xoff = 0, yoff = 0, wid = 0, height = 0;

            string line;
            while ((line = sr.ReadLine()) != null)
            {
                string[] datas = line.Split('=');
                string tp = datas[0].Trim();
                string parm = datas[1].Trim();
                switch (tp)
                {
                    case "startx": xoff = Int32.Parse(parm) * mapWidth / 1422; break;
                    case "starty": yoff = Int32.Parse(parm) * mapHeight / 855 + 50; break; //50为固定偏移
                    case "width": wid = Int32.Parse(parm); break;
                    case "height": height = Int32.Parse(parm); break;
                    case "startpoint": Scene.Instance.StartPos = Int32.Parse(parm); break;
                    case "revivepoint": Scene.Instance.RevivePos = Int32.Parse(parm); break;
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
                    int val = Int32.Parse(data[j]);
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
                posData.Id = Int32.Parse(data[0]);
                posData.Type = data[1];
                posData.MapSetting = true;
                if (data.Length > 2)
                    posData.Info = Int32.Parse(data[2]);
                cachedSpecialData.Add(posData);
            }
        }
        public static bool IsQuestAvail(int qid)
        {
            var questConfig = ConfigData.GetSceneQuestConfig(qid);
            return IsQuestTimeAvail(questConfig) && IsQuestFlagAvail(questConfig) && IsQuestRateAvail(questConfig);
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
            if (!String.IsNullOrEmpty(questConfig.TriggerFlagExist))
            {
                return UserProfile.InfoRecord.CheckFlag(
                    (uint)(MemPlayerFlagTypes)Enum.Parse(typeof(MemPlayerFlagTypes), questConfig.TriggerFlagExist));
            }
            if (!String.IsNullOrEmpty(questConfig.TriggerFlagNoExist))
            {
                return !UserProfile.InfoRecord.CheckFlag(
                    (uint)(MemPlayerFlagTypes)Enum.Parse(typeof(MemPlayerFlagTypes), questConfig.TriggerFlagNoExist));
            }
            return true;
        }

        private static bool IsQuestRateAvail(SceneQuestConfig questConfig)
        {
            if (questConfig.TriggerRate == 0)
                return true;
            return MathTool.GetRandom(100) >= questConfig.TriggerRate;
        }

        /// <summary>
        /// 获取一站地图的随机任务列表
        /// </summary>
        public static List<RLIdValue> GetQuestConfigData(int mapId)
        {
            var config = ConfigData.GetSceneConfig(mapId);
            List<RLIdValue> datas = new List<RLIdValue>();
            if (config.QPortal > 0)//地磁反转
                datas.Add(new RLIdValue { Id = 42000002, Value = config.QPortal });
            if (config.QCardChange > 0)//卡牌商人
                datas.Add(new RLIdValue { Id = 42000003, Value = config.QCardChange });
            if (config.QPiece > 0)//素材商人
                datas.Add(new RLIdValue { Id = 42000004, Value = config.QPiece });
            if (config.QMerchant > 0)//商人
                datas.Add(new RLIdValue { Id = 42000007, Value = config.QMerchant });
            if (config.QDoctor > 0)//医生
                datas.Add(new RLIdValue { Id = 42000005, Value = config.QDoctor });
            if (config.QAngel > 0)//天使
                datas.Add(new RLIdValue { Id = 42000006, Value = config.QAngel });

            if (!String.IsNullOrEmpty(config.Quest))
            {
                string[] infos = config.Quest.Split('|');
                foreach (var info in infos)
                {
                    string[] questData = info.Split(';');
                    int qid = SceneBook.GetSceneQuestByName(questData[0]);
                    if(IsQuestAvail(qid))
                        datas.Add(new RLIdValue { Id = qid, Value = Int32.Parse(questData[1]) });
                }
            }
            if (!String.IsNullOrEmpty(config.QuestRandom))
            {
                string[] infos = config.QuestRandom.Split('|');
                foreach (var info in infos)
                {
                    string[] questData = info.Split(';');
                    int rate = Int32.Parse(questData[1]);
                    if (MathTool.GetRandom(100)<rate)//概率事件
                    {
                        int qid = SceneBook.GetSceneQuestByName(questData[0]);
                        if (IsQuestAvail(qid))
                            datas.Add(new RLIdValue { Id = qid, Value = 1 });
                    }
                }
            }

            return datas;
        }

        public static List<RLIdValue> GetDungeonQuestConfigData(int mapId)
        {
            var config = ConfigData.GetSceneConfig(mapId);
            List<RLIdValue> datas = new List<RLIdValue>();
            if (!String.IsNullOrEmpty(config.QuestDungeon))
            {
                string[] infos = config.QuestDungeon.Split('|');
                foreach (var info in infos)
                {
                    string[] questData = info.Split(';');
                    int qid = SceneBook.GetSceneQuestByName(questData[0]);
                    if (IsQuestAvail(qid))
                        datas.Add(new RLIdValue {Id = qid, Value = Int32.Parse(questData[1])});
                }
            }
            return datas;
        }

        public static SceneQuestBlock GetQuestData(int eventId, int level, string name)
        {
            Dictionary<int, SceneQuestBlock> levelCachDict = new Dictionary<int, SceneQuestBlock>();//存下每一深度的最后节点
            SceneQuestBlock root = null;
            StreamReader sr = new StreamReader(DataLoader.Read("SceneQuest", String.Format("{0}.txt", name)));
            string line;
            int lineCount = 0;
            while ((line=sr.ReadLine())!= null)
            {
                lineCount++;
                int lineDepth = GetStringDepth(ref line);
                char type = line[0];
                string script = line.Substring(1);
                SceneQuestBlock data;
                switch (type)
                {
                    case 's': data = new SceneQuestSay(eventId, level, script, lineDepth, lineCount); break;
                    case 'a': data = new SceneQuestAnswer(eventId, level, script, lineDepth, lineCount); break;
                    case 'e': data = new SceneQuestEvent(eventId, level, script, lineDepth, lineCount); break;
                    case 'r': data = new SceneQuestRollItem(eventId, level, script, lineDepth, lineCount); break;
                    default: throw new Exception(String.Format("GetQuestData unknown type {0} {1}", name, lineCount));
                }

                levelCachDict[data.Depth] = data;
                if (root == null)
                {
                    root = data;
                }
                else
                {
                    levelCachDict[data.Depth-1].Children.Add(data);
                }
            }
            sr.Close();

            return root;
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
