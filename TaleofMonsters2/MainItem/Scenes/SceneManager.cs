using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using ConfigDatas;
using NarlonLib.Math;
using TaleofMonsters.Controler.Loader;
using TaleofMonsters.Core;
using TaleofMonsters.DataType.NPCs;
using TaleofMonsters.DataType.User;
using TaleofMonsters.DataType.User.Mem;
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
            StreamReader sr = new StreamReader(DataLoader.Read("Scene", string.Format("{0}.scn", filePath)));
            int xoff = int.Parse(sr.ReadLine().Split('=')[1])*mapWidth/1422;
            int yoff = int.Parse(sr.ReadLine().Split('=')[1])*mapHeight/855+50;//50为固定偏移
            int wid = int.Parse(sr.ReadLine().Split('=')[1]);
            int height = int.Parse(sr.ReadLine().Split('=')[1]);

            int cellWidth = GameConstants.SceneTileStandardWidth * mapWidth/1422;
            int cellHeight = GameConstants.SceneTileStandardHeight * mapHeight / 855;
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
                if (data.Length > 2)
                    posData.Info = data[2];
                cachedSpecialData[posData.Id] = posData;
            }
            sr.Close();
            #endregion

            if (isWarp || UserProfile.Profile.InfoWorld.PosInfos == null || UserProfile.Profile.InfoWorld.PosInfos.Count <= 0)
            {
                List<MemSceneSpecialPosData> posList = new List<MemSceneSpecialPosData>();
                foreach (var scenePosData in cachedMapData)
                {
                    MemSceneSpecialPosData specialData;
                    cachedSpecialData.TryGetValue(scenePosData.Id, out specialData);

                    if (specialData == null)
                    {
                        specialData = new MemSceneSpecialPosData(); //随机一个出来
                        specialData.Id = scenePosData.Id;
                        specialData.Type = "Quest";
                    }
                    specialData.RandomSeed = MathTool.GetRandom(10000);
                    cachedSpecialData[specialData.Id] = specialData;

                    posList.Add(specialData);
                }

                UserProfile.Profile.InfoWorld.PosInfos = posList;
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
                            so = new SceneQuest(scenePosData.Id, scenePosData.X, scenePosData.Y, scenePosData.Width, scenePosData.Height, specialData.Disabled); break;
                        case "Warp":
                            so = new SceneWarp(scenePosData.Id, scenePosData.X, scenePosData.Y, scenePosData.Width, scenePosData.Height, specialData.Disabled, specialData.Info); break;
                        default:
                            so = new SceneTile(scenePosData.Id, scenePosData.X, scenePosData.Y, scenePosData.Width, scenePosData.Height, specialData.Disabled); break;
                    }
                }
                else
                {
                    throw new Exception("GetSceneObjects error");
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

        public static Image GetPreview(int id)
        {
            SceneConfig sceneConfig = ConfigData.GetSceneConfig(id);

            ControlPlus.TipImage tipData = new ControlPlus.TipImage();
            tipData.AddTextNewLine(sceneConfig.Name, "Lime", 20);
            tipData.AddTextNewLine(string.Format("需要等级: {0}", sceneConfig.Level), "White");

            string[] icons = NPCBook.GetNPCIconsOnMap(id);
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
