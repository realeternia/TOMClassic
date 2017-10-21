using System;
using System.Collections.Generic;
using ConfigDatas;
using NarlonLib.Math;
using TaleofMonsters.DataType;
using TaleofMonsters.DataType.Scenes;
using TaleofMonsters.DataType.User;
using TaleofMonsters.DataType.User.Db;
using TaleofMonsters.MainItem.Blesses;

namespace TaleofMonsters.MainItem.Scenes
{
    internal static class SceneManager
    {
        public static SceneInfoRT RefreshSceneObjects(int id, int mapWidth ,int mapHeight, SceneFreshReason reason)
        {
            SceneInfoRT info = new SceneInfoRT();

            var filePath = ConfigData.GetSceneConfig(id).TilePath;

            var cachedSpecialData = new Dictionary<int, DbSceneSpecialPosData>();
            if (reason != SceneFreshReason.Load || UserProfile.Profile.InfoWorld.PosInfos == null || UserProfile.Profile.InfoWorld.PosInfos.Count <= 0)
            {//重新生成
                UserProfile.InfoBasic.DungeonRandomSeed = MathTool.GetRandom(int.MaxValue);
                Random r = new Random(UserProfile.InfoBasic.DungeonRandomSeed);
                info.Script = SceneBook.LoadSceneFile(id, mapWidth, mapHeight, filePath, r);
                FilterSpecialData(info.Script.SpecialData, cachedSpecialData);
                var questCellCount = info.Script.MapData.Count - info.Script.HiddenCellCount - cachedSpecialData.Count;

                UserProfile.Profile.InfoWorld.PosInfos = GenerateSceneRandomInfo(questCellCount, info.Script.MapData, cachedSpecialData);
            }
            else
            {//从存档加载
                Random r = new Random(UserProfile.InfoBasic.DungeonRandomSeed);
                info.Script = SceneBook.LoadSceneFile(id, mapWidth, mapHeight, filePath, r);
                foreach (var posData in UserProfile.Profile.InfoWorld.PosInfos)
                {
                    cachedSpecialData[posData.Id] = posData;
                }
            }

            var mapCellDict = info.Script.GetCellDict();
            foreach (var specialPosData in cachedSpecialData.Values)
            {
                SceneInfo.SceneScriptPosData cellConfigData;
                mapCellDict.TryGetValue(specialPosData.Id, out cellConfigData);

                info.AddCellInitial(cellConfigData, specialPosData, id, reason);
            }

            return info;
        }

        private static void FilterSpecialData(List<SceneInfo.SceneScriptSpecialData> mapSpecialData, Dictionary<int, DbSceneSpecialPosData> mapMemSpecialData)
        {
            foreach (var specialPosData in mapSpecialData)
            {
                if (specialPosData.Type == "Quest")
                {
                    if (!SceneQuestBook.IsQuestAvail(specialPosData.Info))
                        continue;
                }
                mapMemSpecialData[specialPosData.Id] = new DbSceneSpecialPosData
                {
                    Id = specialPosData.Id,
                    Info = specialPosData.Info,
                    Type = specialPosData.Type,
                    MapSetting = true
                };
            }
        }

        /// <summary>
        /// 生成随机的格子，一般只有第一次进入场景才会调用
        /// </summary>
        private static List<DbSceneSpecialPosData> GenerateSceneRandomInfo(int questCellCount, List<SceneInfo.SceneScriptPosData> mapScriptData, Dictionary<int, DbSceneSpecialPosData> mapMemorySpecialData)
        {
            List<int> randQuestList = new List<int>();
            Scene.Instance.Rule.Generate(randQuestList, questCellCount);

            var posList = new List<DbSceneSpecialPosData>();
            int index = 0;
            foreach (var scenePosData in mapScriptData)
            {
                if (scenePosData.HiddenIndex > 0)
                {
                    continue; //隐藏格子需要后续触发
                }

                DbSceneSpecialPosData specialData;
                mapMemorySpecialData.TryGetValue(scenePosData.Id, out specialData);

                if (specialData == null)//表示不是预设的格子
                {
                    specialData = new DbSceneSpecialPosData();
                    specialData.Id = scenePosData.Id;
                    if (randQuestList.Count > index) //隐藏房间不随机任务
                    {
                        specialData.Type = "Quest";
                        specialData.Info = randQuestList[index++];    //随机一个出来
                    }
                    else
                    {
                        specialData.Type = "Tile";
                    }
                }
                mapMemorySpecialData[specialData.Id] = specialData;

                posList.Add(specialData);
            }

            return posList;
        }

        public static bool CanPlayerMove(int id1, int id2)
        {
            var dis = GetDistance(id1, id2);
            return dis > 0 && dis <= 1 + BlessManager.MoveDistance;
        }

        public static int GetDistance(int id1, int id2)
        {
            int differ = Math.Abs(id1/1000 - id2/1000) + Math.Abs(id1 % 1000 - id2 % 1000);
            return differ;
        }
    }
}
