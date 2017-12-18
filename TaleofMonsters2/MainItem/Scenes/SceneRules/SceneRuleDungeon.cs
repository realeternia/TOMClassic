using System.Collections.Generic;
using ConfigDatas;
using NarlonLib.Tools;
using TaleofMonsters.DataType;
using TaleofMonsters.DataType.Scenes;
using TaleofMonsters.DataType.User;
using TaleofMonsters.DataType.User.Db;

namespace TaleofMonsters.MainItem.Scenes.SceneRules
{
    internal class SceneRuleDungeon : ISceneRule
    {
        private int mapId;
        private Dictionary<int, int> questReplaceDict = new Dictionary<int, int>();//尝试替换一些quest的id
        
        public void Init(int id, int minute)
        {
            mapId = id;
            if (UserProfile.InfoWorld.SavedDungeonQuests.Count == 0 && UserProfile.InfoDungeon.DungeonId > 0)
            {
                UserProfile.InfoRecord.SetRecordById((int)MemPlayerRecordTypes.DungeonQuestOffside, 0);
                foreach (var questData in SceneQuestBook.GetDungeonQuestConfigData(UserProfile.InfoDungeon.DungeonId))
                    for (int j = 0; j < questData.Value; j++)
                        UserProfile.InfoWorld.SavedDungeonQuests.Add(questData.Id);
                ArraysUtils.RandomShuffle(UserProfile.InfoWorld.SavedDungeonQuests);
            }

            if (UserProfile.InfoDungeon.StoryId > 0)
            {
                var storyConfig = ConfigData.GetDungeonStoryConfig(UserProfile.InfoDungeon.StoryId);
                if (!string.IsNullOrEmpty(storyConfig.EventReplace))
                {
                    var items = storyConfig.EventReplace.Split(',');
                    foreach (var checkItem in items)
                    {
                        var checkDatas = checkItem.Split('=');
                        var questId1 = SceneQuestBook.GetSceneQuestByName(checkDatas[0]);
                        var questId2 = SceneQuestBook.GetSceneQuestByName(checkDatas[1]);
                        questReplaceDict[questId1] = questId2;
                    }
                }
            }
            
        }

        public void Generate(List<int> randQuestList, int questCellCount)
        {
            foreach (var questData in SceneQuestBook.GetQuestConfigData(mapId))
            {
                for (int j = 0; j < questData.Value; j++)
                    randQuestList.Add(questData.Id);
            }

            int offset = UserProfile.InfoRecord.GetRecordById((int)MemPlayerRecordTypes.DungeonQuestOffside);
            while (randQuestList.Count < questCellCount)
            {
                if (offset < UserProfile.InfoWorld.SavedDungeonQuests.Count - 1)
                {
                    randQuestList.Add(UserProfile.InfoWorld.SavedDungeonQuests[offset]);
                    offset++;
                }
                else
                {
                    ArraysUtils.RandomShuffle(UserProfile.InfoWorld.SavedDungeonQuests);
                    offset = 0;
                    randQuestList.Add(UserProfile.InfoWorld.SavedDungeonQuests[offset]);
                }
            }
            UserProfile.InfoRecord.SetRecordById((int)MemPlayerRecordTypes.DungeonQuestOffside, offset);
            ArraysUtils.RandomShuffle(randQuestList);
        }

        public void CheckReplace(DbSceneSpecialPosData cellData)
        {
            if (questReplaceDict.ContainsKey(cellData.Info))
            {
                cellData.Info = questReplaceDict[cellData.Info];
            }
        }
    }
}