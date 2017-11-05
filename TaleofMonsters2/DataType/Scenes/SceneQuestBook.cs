using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using ConfigDatas;
using NarlonLib.Math;
using TaleofMonsters.Controler.Loader;
using TaleofMonsters.Controler.Resource;
using TaleofMonsters.DataType.Quests;
using TaleofMonsters.DataType.User;
using TaleofMonsters.MainItem.Quests.SceneQuests;
using TaleofMonsters.MainItem.Scenes;

namespace TaleofMonsters.DataType.Scenes
{
    internal static class SceneQuestBook
    {
        private static Dictionary<string, int> sceneQuestNameDict = null;
        public static int GetSceneQuestByName(string name)
        {
            if (sceneQuestNameDict == null)
            {
                sceneQuestNameDict = new Dictionary<string, int>();
                foreach (SceneQuestConfig questConfig in ConfigData.SceneQuestDict.Values)
                {
                    sceneQuestNameDict.Add(questConfig.Ename, questConfig.Id);
                }
            }
            int questId;
            if (!sceneQuestNameDict.TryGetValue(name, out questId))
            {
                throw new KeyNotFoundException("scene quest name not found " + name);
            }
            return questId;
        }

        public static Image GetSceneQuestImage(int id)
        {
            string fname = string.Format("SceneQuest/{0}.PNG", ConfigData.SceneQuestDict[id].Figue);
            if (!ImageManager.HasImage(fname))
            {
                Image image = PicLoader.Read("SceneQuest", string.Format("{0}.JPG", ConfigData.SceneQuestDict[id].Figue));
                ImageManager.AddImage(fname, image);
            }
            return ImageManager.GetImage(fname);
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
            if (!string.IsNullOrEmpty(questConfig.TriggerQuestNotReceive))
            {
                return UserProfile.InfoQuest.IsQuestNotReceive(QuestBook.GetQuestIdByName(questConfig.TriggerQuestNotReceive));
            }
            if (!string.IsNullOrEmpty(questConfig.TriggerQuestReceived))
            {
                return UserProfile.InfoQuest.IsQuestCanProgress(QuestBook.GetQuestIdByName(questConfig.TriggerQuestReceived));
            }
            if (!string.IsNullOrEmpty(questConfig.TriggerQuestFinished))
            {
                var qid = QuestBook.GetQuestIdByName(questConfig.TriggerQuestFinished);
                return UserProfile.InfoQuest.IsQuestFinish(qid) || UserProfile.InfoQuest.IsQuestCanReward(qid);
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
            if (config.QWheel > 0)//轮盘
                datas.Add(new RLIdValue { Id = 42000008, Value = config.QWheel });
            if (config.QRes > 0)//期货
                datas.Add(new RLIdValue { Id = 42000009, Value = config.QRes });

            if (!string.IsNullOrEmpty(config.Quest))
            {
                string[] infos = config.Quest.Split('|');
                foreach (var info in infos)
                {
                    string[] questData = info.Split(';');
                    int qid = GetSceneQuestByName(questData[0]);
                    if(IsQuestAvail(qid))
                        datas.Add(new RLIdValue { Id = qid, Value = int.Parse(questData[1]) });
                }
            }
            if (!string.IsNullOrEmpty(config.QuestRandom))
            {
                string[] infos = config.QuestRandom.Split('|');
                foreach (var info in infos)
                {
                    string[] questData = info.Split(';');
                    int rate = int.Parse(questData[1]);
                    if (MathTool.GetRandom(100)<rate)//概率事件
                    {
                        int qid = GetSceneQuestByName(questData[0]);
                        if (IsQuestAvail(qid))
                            datas.Add(new RLIdValue { Id = qid, Value = 1 });
                    }
                }
            }

            return datas;
        }

        /// <summary>
        /// 统计场景内随机事件的数量使用
        /// </summary>
        public static float GetQuestCount(int mapId)
        {
            var config = ConfigData.GetSceneConfig(mapId);
            float questCount = 0;
            if (config.QPortal > 0)//地磁反转
                questCount++;
            if (config.QCardChange > 0)//卡牌商人
                questCount++;
            if (config.QPiece > 0)//素材商人
                questCount++;
            if (config.QMerchant > 0)//商人
                questCount++;
            if (config.QDoctor > 0)//医生
                questCount++;
            if (config.QAngel > 0)//天使
                questCount++;
            if (config.QWheel > 0)//轮盘
                questCount++;
            if (config.QRes > 0)//期货
                questCount++;

            if (!string.IsNullOrEmpty(config.Quest))
            {
                string[] infos = config.Quest.Split('|');
                foreach (var info in infos)
                {
                    string[] questData = info.Split(';');
                    questCount += int.Parse(questData[1]);
                }
            }
            if (!string.IsNullOrEmpty(config.QuestRandom))
            {
                string[] infos = config.QuestRandom.Split('|');
                foreach (var info in infos)
                {
                    string[] questData = info.Split(';');
                    int rate = int.Parse(questData[1]);
                    questCount += (float)rate/100;
                }
            }

            return questCount;
        }


        public static List<RLIdValue> GetDungeonQuestConfigData(int dungeonId)
        {
            var config = ConfigData.GetDungeonConfig(dungeonId);
            List<RLIdValue> datas = new List<RLIdValue>();
            if (!string.IsNullOrEmpty(config.QuestDungeon))
            {
                string[] infos = config.QuestDungeon.Split('|');
                foreach (var info in infos)
                {
                    string[] questData = info.Split(';');
                    int qid = GetSceneQuestByName(questData[0]);
                    if (IsQuestAvail(qid))
                        datas.Add(new RLIdValue { Id = qid, Value = int.Parse(questData[1]) });
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
                    default: throw new Exception(string.Format("GetQuestData unknown type {0} {1}", name, lineCount));
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
