using System;
using System.Collections.Generic;
using System.IO;
using ConfigDatas;
using NarlonLib.Math;
using TaleofMonsters.Controler.Loader;
using TaleofMonsters.Core;
using TaleofMonsters.DataType;
using TaleofMonsters.DataType.Scenes;
using TaleofMonsters.DataType.User;
using TaleofMonsters.DataType.User.Db;
using TaleofMonsters.MainItem.Quests.SceneQuests;
using TaleofMonsters.MainItem.Scenes.SceneObjects;

namespace TaleofMonsters.MainItem.Scenes
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

        public static List<SceneObject> RefreshSceneObjects(int id, int mapWidth ,int mapHeight, SceneFreshReason reason)
        {
            List<ScenePosData> cachedMapData = new List<ScenePosData>();
            var cachedSpecialData = new Dictionary<int, DbSceneSpecialPosData>();
            var filePath = ConfigData.GetSceneConfig(id).TilePath;

            LoadSceneFile(mapWidth, mapHeight, filePath, cachedMapData, cachedSpecialData);
            var questCellCount = cachedMapData.Count - cachedSpecialData.Count;

            if (reason != SceneFreshReason.Load || UserProfile.Profile.InfoWorld.PosInfos == null || UserProfile.Profile.InfoWorld.PosInfos.Count <= 0)
            {//重新生成
                GenerateSceneRandomInfo(id, questCellCount, cachedMapData, cachedSpecialData);
            }
            else
            {//从存档加载
                foreach (var posData in UserProfile.Profile.InfoWorld.PosInfos)
                {
                    cachedSpecialData[posData.Id] = posData;
                }
            }

            List<SceneObject> sceneObjects = new List<SceneObject>();
            foreach (var scenePosData in cachedMapData)
            {
                DbSceneSpecialPosData specialData;
                cachedSpecialData.TryGetValue(scenePosData.Id, out specialData);

                SceneObject so;
                if (specialData != null)
                {
                    switch (specialData.Type)
                    {
                        case "Quest":
                            var questId = UserProfile.InfoQuest.CheckReplace(specialData.Info);
                            so = new SceneQuest(scenePosData.Id, scenePosData.X, scenePosData.Y, scenePosData.Width, scenePosData.Height, questId); 
                              so.Disabled = specialData.Disabled; break;
                        case "Warp":
                            so = new SceneWarp(scenePosData.Id, scenePosData.X, scenePosData.Y, scenePosData.Width, scenePosData.Height, specialData.Info);
                            so.Disabled = specialData.Disabled;
                            if (ConfigData.GetSceneConfig(id).Type == SceneTypes.Common && reason == SceneFreshReason.Warp)
                            {
                                specialData.Disabled = true;
                                so.Disabled = true;//如果是切场景，切到战斗场景，所有传送门自动关闭
                            }
                            break;
                        default:
                            so = new SceneTile(scenePosData.Id, scenePosData.X, scenePosData.Y, scenePosData.Width, scenePosData.Height); break;
                    }
                    so.MapSetting = specialData.MapSetting;
                }
                else
                {
                    so = new SceneTile(scenePosData.Id, scenePosData.X, scenePosData.Y, scenePosData.Width, scenePosData.Height); break;
                    so.Disabled = true;
                    //throw new Exception("RefreshSceneObjects error");
                }
                sceneObjects.Add(so);
            }

            return sceneObjects;
        }

        private static void LoadSceneFile(int mapWidth, int mapHeight, string filePath, List<ScenePosData> cachedMapData, Dictionary<int, DbSceneSpecialPosData> cachedSpecialData)
        {
            StreamReader sr = new StreamReader(DataLoader.Read("Scene", string.Format("{0}.txt", filePath)));
            int xoff = 0, yoff = 0, wid = 0, height = 0;

            string line;
            while ((line = sr.ReadLine())!= null)
            {
                string[] datas = line.Split('=');
                string tp = datas[0].Trim();
                string parm = datas[1].Trim();
                switch (tp)
                {
                    case "startx": xoff= int.Parse(parm) * mapWidth / 1422; break;
                    case "starty": yoff = int.Parse(parm) * mapHeight / 855 + 50; break; //50为固定偏移
                    case "width": wid = int.Parse(parm); break;
                    case "height": height = int.Parse(parm); break;
                    case "startpos": Scene.Instance.StartPos = int.Parse(parm); break;
                    case "data": ReadBody(sr, mapWidth, mapHeight, cachedMapData, cachedSpecialData, wid, height, xoff, yoff); break;
                }
            }
                     
            sr.Close();
        }

        private static void ReadBody(StreamReader sr, int mapWidth, int mapHeight, List<ScenePosData> cachedMapData, Dictionary<int, DbSceneSpecialPosData> cachedSpecialData, 
            int wid, int height, int xoff, int yoff)
        {
            int cellWidth = GameConstants.SceneTileStandardWidth*mapWidth/1422;
            int cellHeight = GameConstants.SceneTileStandardHeight*mapHeight/855;
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

                    int lineOff = (int) (cellWidth*(height - i - 1)*GameConstants.SceneTileGradient);
                    ScenePosData so = new ScenePosData
                    {
                        Id = val,
                        X = xoff + j*cellWidth + lineOff,
                        Y = yoff + i*cellHeight,
                        Width = cellWidth,
                        Height = cellHeight
                    };
                    cachedMapData.Add(so);
                }
            }

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
                cachedSpecialData[posData.Id] = posData;
            }
        }

        private static void GenerateSceneRandomInfo(int id, int questCellCount, List<ScenePosData> cachedMapData, Dictionary<int, DbSceneSpecialPosData> cachedSpecialData)
        {
            List<int> randQuestList = new List<int>();
            Scene.Instance.Rule.Generate(randQuestList, questCellCount);

            var posList = new List<DbSceneSpecialPosData>();
            int index = 0;
            foreach (var scenePosData in cachedMapData)
            {
                DbSceneSpecialPosData specialData;
                cachedSpecialData.TryGetValue(scenePosData.Id, out specialData);

                if (specialData == null)//表示不是预设的格子
                {
                    specialData = new DbSceneSpecialPosData();
                    specialData.Id = scenePosData.Id;
                    if (randQuestList.Count > index)
                    {
                        specialData.Type = "Quest";
                        specialData.Info = randQuestList[index++];    //随机一个出来
                    }
                    else
                    {
                        specialData.Type = "Tile";
                    }
                }
                cachedSpecialData[specialData.Id] = specialData;

                posList.Add(specialData);
            }

            UserProfile.Profile.InfoWorld.PosInfos = posList;
        }

        public static bool CanMove(int id1, int id2)
        {
            int differ = Math.Abs(id1 - id2);
            return differ == 1 || differ == 1000;
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

            if (!string.IsNullOrEmpty(config.Quest))
            {
                string[] infos = config.Quest.Split('|');
                foreach (var info in infos)
                {
                    string[] questData = info.Split(';');
                    datas.Add(new RLIdValue { Id = SceneBook.GetSceneQuestByName(questData[0]),
                        Value = int.Parse(questData[1]) });
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
                        datas.Add(new RLIdValue { Id = SceneBook.GetSceneQuestByName(questData[0]),
                                Value = 1 });
                    }
                }
            }

            return datas;
        }

        public static SceneQuestBlock GetQuestData(int eventId, int level, string name)
        {
            Dictionary<int, SceneQuestBlock> levelCachDict = new Dictionary<int, SceneQuestBlock>();//存下每一深度的最后节点
            SceneQuestBlock root = null;
            StreamReader sr = new StreamReader(DataLoader.Read("SceneQuest", string.Format("{0}.txt", name)));
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
