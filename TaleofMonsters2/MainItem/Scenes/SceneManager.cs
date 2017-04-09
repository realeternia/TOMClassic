using System;
using System.Collections.Generic;
using ConfigDatas;
using NarlonLib.Math;
using TaleofMonsters.DataType;
using TaleofMonsters.DataType.Quests;
using TaleofMonsters.DataType.User;
using TaleofMonsters.DataType.User.Db;
using TaleofMonsters.MainItem.Scenes.SceneObjects;

namespace TaleofMonsters.MainItem.Scenes
{
    internal static class SceneManager
    {
        internal struct ScenePosData
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

            List<DbSceneSpecialPosData> specialDataList = new List<DbSceneSpecialPosData>();
            if (reason != SceneFreshReason.Load || UserProfile.Profile.InfoWorld.PosInfos == null || UserProfile.Profile.InfoWorld.PosInfos.Count <= 0)
            {//重新生成
                UserProfile.InfoBasic.DungeonRandomSeed = MathTool.GetRandom(int.MaxValue);
                Random r = new Random(UserProfile.InfoBasic.DungeonRandomSeed);
                SceneQuestBook.LoadSceneFile(mapWidth, mapHeight, filePath, r, cachedMapData, specialDataList);
                FilterSpecialData(specialDataList, cachedSpecialData);
                var questCellCount = cachedMapData.Count - cachedSpecialData.Count;
                GenerateSceneRandomInfo(id, questCellCount, cachedMapData, cachedSpecialData);
            }
            else
            {//从存档加载
                Random r = new Random(UserProfile.InfoBasic.DungeonRandomSeed);
                SceneQuestBook.LoadSceneFile(mapWidth, mapHeight, filePath, r, cachedMapData, specialDataList);
                foreach (var posData in UserProfile.Profile.InfoWorld.PosInfos)
                {
                    cachedSpecialData[posData.Id] = posData;
                }
            }

            List<SceneObject> sceneObjects = new List<SceneObject>();
            foreach (var scenePosData in cachedMapData)
            {
                DbSceneSpecialPosData cachedData;
                cachedSpecialData.TryGetValue(scenePosData.Id, out cachedData);

                SceneObject so;
                if (cachedData != null)
                {
                    switch (cachedData.Type)
                    {
                        case "Quest":
                            so = new SceneQuest(scenePosData.Id, scenePosData.X, scenePosData.Y, scenePosData.Width, scenePosData.Height, cachedData.Info); 
                              so.Disabled = cachedData.Disabled; break;
                        case "Warp":
                            so = new SceneWarp(scenePosData.Id, scenePosData.X, scenePosData.Y, scenePosData.Width, scenePosData.Height, cachedData.Info);
                            so.Disabled = cachedData.Disabled;
                            if (ConfigData.GetSceneConfig(id).Type == SceneTypes.Common && reason == SceneFreshReason.Warp)
                            {
                                cachedData.Disabled = true;
                                so.Disabled = true;//如果是切场景，切到战斗场景，所有传送门自动关闭
                            }
                            break;
                        default:
                            so = new SceneTile(scenePosData.Id, scenePosData.X, scenePosData.Y, scenePosData.Width, scenePosData.Height); break;
                    }
                    so.Flag = cachedData.Flag;
                    so.MapSetting = cachedData.MapSetting;
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

        private static void FilterSpecialData(List<DbSceneSpecialPosData> specialDataList, 
            Dictionary<int, DbSceneSpecialPosData> cachedSpecialData)
        {
            foreach (var specialPosData in specialDataList)
            {
                if (specialPosData.Type == "Quest")
                {
                    if (!SceneQuestBook.IsQuestAvail(specialPosData.Info))
                        continue;
                }
                cachedSpecialData[specialPosData.Id] = specialPosData;
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

        public static bool CanPlayerMove(int id1, int id2)
        {
            var dis = GetDistance(id1, id2);
            return dis > 0 && dis <= UserProfile.InfoBasic.GetSceneMove();
        }

        public static int GetDistance(int id1, int id2)
        {
            int differ = Math.Abs(id1/1000 - id2/1000) + Math.Abs(id1 % 1000 - id2 % 1000);
            return differ;
        }
    }
}
