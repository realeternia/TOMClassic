﻿using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using ConfigDatas;
using NarlonLib.Math;
using TaleofMonsters.Controler.Loader;
using TaleofMonsters.Core;
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

#region 读取文件信息
            StreamReader sr = new StreamReader(DataLoader.Read("Scene", string.Format("{0}.txt", filePath)));
            int xoff = int.Parse(sr.ReadLine().Split('=')[1])*mapWidth/1422;
            int yoff = int.Parse(sr.ReadLine().Split('=')[1])*mapHeight/855+50;//50为固定偏移
            int wid = int.Parse(sr.ReadLine().Split('=')[1]);
            int height = int.Parse(sr.ReadLine().Split('=')[1]);

            int cellWidth = GameConstants.SceneTileStandardWidth * mapWidth/1422;
            int cellHeight = GameConstants.SceneTileStandardHeight * mapHeight / 855;
            int questCellCount = 0;
            for (int i = 0; i < height; i++)
            {
                string[] data = sr.ReadLine().Split('\t');
                for (int j = 0; j < wid; j++)
                {
                    int val = int.Parse(data[j]);
                    if (val ==0)
                    {
                        continue;
                    }

                    int lineOff = (int)(cellWidth*(height-i-1)* GameConstants.SceneTileGradient);
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
            while ((line = sr.ReadLine())!=null)
            {
                string[] data = line.Split('\t');
                if (data.Length < 2)
                    continue;

                var posData = new DbSceneSpecialPosData();
                posData.Id = int.Parse(data[0]);
                posData.Type = data[1];
                posData.MapSetting = true;
                if (posData.Type == "Warp")
                    posData.Disabled = reason == SceneFreshReason.Warp;//传送门默认是关闭的
                if (data.Length > 2)
                    posData.Info = int.Parse(data[2]);
                if (data.Length > 3)
                    posData.Info2 = int.Parse(data[3]);
                cachedSpecialData[posData.Id] = posData;
            }
            sr.Close();

            questCellCount = cachedMapData.Count - cachedSpecialData.Count;
            #endregion

            if (reason != SceneFreshReason.Load || UserProfile.Profile.InfoWorld.PosInfos == null || UserProfile.Profile.InfoWorld.PosInfos.Count <= 0)
            {//重新生成
                List<int> questList = new List<int>();
                foreach (var questData in GetQuestConfigData(id) )
                {
                    for (int j = 0; j < questData.Value; j++)
                    {
                        questList.Add(questData.Id);
                    }
                }
                ListTool.Fill(questList, 0, questCellCount);
                ListTool.RandomShuffle(questList);

                var posList = new List<DbSceneSpecialPosData>();
                int index = 0;
                foreach (var scenePosData in cachedMapData)
                {
                    DbSceneSpecialPosData specialData;
                    cachedSpecialData.TryGetValue(scenePosData.Id, out specialData);

                    if (specialData == null)
                    {
                        specialData = new DbSceneSpecialPosData(); //随机一个出来
                        specialData.Id = scenePosData.Id;
                        specialData.Type = "Quest";
                        specialData.Info = questList[index++];
                    }
                    cachedSpecialData[specialData.Id] = specialData;

                    posList.Add(specialData);
                }

                UserProfile.Profile.InfoWorld.PosInfos = posList;
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
                            so = new SceneQuest(scenePosData.Id, scenePosData.X, scenePosData.Y, scenePosData.Width, scenePosData.Height, specialData.Info); break;
                        case "Warp":
                            so = new SceneWarp(scenePosData.Id, scenePosData.X, scenePosData.Y, scenePosData.Width, scenePosData.Height, specialData.Info, specialData.Info2); break;
                        default:
                            so = new SceneTile(scenePosData.Id, scenePosData.X, scenePosData.Y, scenePosData.Width, scenePosData.Height); break;
                    }
                    so.Disabled = specialData.Disabled;
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
            for (int i = 0; i < config.Quest.Count; i++)
                datas.Add(config.Quest[i]);
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

        public static Image GetPreview(int id)
        {
            SceneConfig sceneConfig = ConfigData.GetSceneConfig(id);

            ControlPlus.TipImage tipData = new ControlPlus.TipImage();
            tipData.AddTextNewLine(sceneConfig.Name, "Lime", 20);
            tipData.AddTextNewLine(string.Format("地图等级: {0}", sceneConfig.Level), "White");

            string[] icons = SceneBook.GetNPCIconsOnMap(id);
            if (icons.Length > 0)
            {
                tipData.AddTextNewLine("设施", "Green");
                foreach (string icon in icons)
                {
                    tipData.AddImage(HSIcons.GetIconsByEName(icon));
                }
            }

            if (sceneConfig.Func != "")
            {
                tipData.AddTextNewLine("特色", "Pink");
                string[] funcs = sceneConfig.Func.Split(';');
                foreach (string fun in funcs)
                {
                    tipData.AddImage(HSIcons.GetIconsByEName(string.Format("npc{0}", fun.ToLower())));
                }
            }
            return tipData.Image;
        }
    }
}
