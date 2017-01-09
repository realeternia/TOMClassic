using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using ConfigDatas;
using NarlonLib.Math;
using TaleofMonsters.Controler.Loader;
using TaleofMonsters.Core;
using TaleofMonsters.DataType.Scenes;
using TaleofMonsters.DataType.User;
using TaleofMonsters.DataType.User.Mem;
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

        public static List<SceneObject> GetSceneObjects(int id, int mapWidth ,int mapHeight, bool isWarp)
        {
            List<ScenePosData> cachedMapData = new List<ScenePosData>();
            Dictionary<int, MemSceneSpecialPosData> cachedSpecialData = new Dictionary<int, MemSceneSpecialPosData>();
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

                MemSceneSpecialPosData posData = new MemSceneSpecialPosData();
                posData.Id = int.Parse(data[0]);
                posData.Type = data[1];
                if (posData.Type == "Warp")
                    posData.Disabled = true;//传送门默认是关闭的
                if (data.Length > 2)
                    posData.Info = int.Parse(data[2]);
                if (data.Length > 3)
                    posData.Info2 = int.Parse(data[3]);
                cachedSpecialData[posData.Id] = posData;
            }
            sr.Close();

            questCellCount = cachedMapData.Count - cachedSpecialData.Count;
            #endregion

            if (isWarp || UserProfile.Profile.InfoWorld.PosInfos == null || UserProfile.Profile.InfoWorld.PosInfos.Count <= 0)
            {//重新生成
                var sceneConfig = ConfigData.GetSceneConfig(id);
                List<int> questList = new List<int>();
                for (int i = 0; i < sceneConfig.Quest.Count; i++)
                {
                    for (int j = 0; j < sceneConfig.Quest[i].Value; j++)
                    {
                        questList.Add(sceneConfig.Quest[i].Id);
                    }
                }
                ListTool.Fill(questList, 0, questCellCount);
                ListTool.RandomShuffle(questList);

                List<MemSceneSpecialPosData> posList = new List<MemSceneSpecialPosData>();
                int index = 0;
                foreach (var scenePosData in cachedMapData)
                {
                    MemSceneSpecialPosData specialData;
                    cachedSpecialData.TryGetValue(scenePosData.Id, out specialData);

                    if (specialData == null)
                    {
                        specialData = new MemSceneSpecialPosData(); //随机一个出来
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
                MemSceneSpecialPosData specialData;
                cachedSpecialData.TryGetValue(scenePosData.Id, out specialData);

                SceneObject so;
                if (specialData != null)
                {
                    switch (specialData.Type)
                    {
                        case "Quest":
                            so = new SceneQuest(scenePosData.Id, scenePosData.X, scenePosData.Y, scenePosData.Width, scenePosData.Height, specialData.Disabled, specialData.Info); break;
                        case "Warp":
                            so = new SceneWarp(scenePosData.Id, scenePosData.X, scenePosData.Y, scenePosData.Width, scenePosData.Height, specialData.Disabled, specialData.Info, specialData.Info2); break;
                        default:
                            so = new SceneTile(scenePosData.Id, scenePosData.X, scenePosData.Y, scenePosData.Width, scenePosData.Height, specialData.Disabled); break;
                    }
                }
                else
                {
                    so = new SceneTile(scenePosData.Id, scenePosData.X, scenePosData.Y, scenePosData.Width, scenePosData.Height, true); break;
                    //throw new Exception("GetSceneObjects error");
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

        public static SceneQuestBlock GetQuestData(string name)
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
                    case 's': data = new SceneQuestSay(script, lineDepth, lineCount); break;
                    case 'a': data = new SceneQuestAnswer(script, lineDepth, lineCount); break;
                    case 'e': data = new SceneQuestEvent(script, lineDepth, lineCount); break;
                    case 'r': data = new SceneQuestRollItem(script, lineDepth, lineCount); break;
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
