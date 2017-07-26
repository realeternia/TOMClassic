using System.Collections.Generic;
using NarlonLib.Tools;
using TaleofMonsters.DataType;
using TaleofMonsters.DataType.Quests;
using TaleofMonsters.DataType.User;

namespace TaleofMonsters.MainItem.Scenes.SceneRules
{
    internal class SceneRuleDungeon : ISceneRule
    {
        private int mapId;
        
        public void Init(int id, int minute)
        {
            mapId = id;
            if (UserProfile.InfoWorld.SavedDungeonQuests.Count == 0)
            {
                UserProfile.InfoRecord.SetRecordById((int)MemPlayerRecordTypes.DungeonQuestOffside, 0);
                foreach (var questData in SceneQuestBook.GetDungeonQuestConfigData(mapId))
                {
                    for (int j = 0; j < questData.Value; j++)
                    {
                        UserProfile.InfoWorld.SavedDungeonQuests.Add(questData.Id);
                    }
                }
                ArraysUtils.RandomShuffle(UserProfile.InfoWorld.SavedDungeonQuests);
            }
        }

        public void Generate(List<int> randQuestList, int questCellCount)
        {
            foreach (var questData in SceneQuestBook.GetQuestConfigData(mapId))
            {
                for (int j = 0; j < questData.Value; j++)
                {
                    randQuestList.Add(questData.Id);
                }
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
    }
}