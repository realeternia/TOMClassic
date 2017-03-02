using System.Collections.Generic;
using ConfigDatas;
using NarlonLib.Math;
using TaleofMonsters.DataType;
using TaleofMonsters.DataType.User;

namespace TaleofMonsters.MainItem.Scenes.SceneRules
{
    public class SceneRuleDungeon : ISceneRule
    {
        private int mapId;
        private int minutes;
        
        public void Init(int id, int minute)
        {
            mapId = id;
            minutes = minute;
            if (UserProfile.InfoWorld.SavedDungeonQuests.Count == 0)
            {
                UserProfile.InfoRecord.SetRecordById((int)MemPlayerRecordTypes.DungeonQuestOffside, 0);
                foreach (var questData in SceneManager.GetDungeonQuestConfigData(mapId, minute))
                {
                    for (int j = 0; j < questData.Value; j++)
                    {
                        UserProfile.InfoWorld.SavedDungeonQuests.Add(questData.Id);
                    }
                }
                ListTool.RandomShuffle(UserProfile.InfoWorld.SavedDungeonQuests);
            }
        }

        public void Generate(List<int> randQuestList, int questCellCount)
        {
            foreach (var questData in SceneManager.GetQuestConfigData(mapId, minutes))
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
                    ListTool.RandomShuffle(UserProfile.InfoWorld.SavedDungeonQuests);
                    offset = 0;
                    randQuestList.Add(UserProfile.InfoWorld.SavedDungeonQuests[offset]);
                }
            }
            UserProfile.InfoRecord.SetRecordById((int)MemPlayerRecordTypes.DungeonQuestOffside, offset);
            ListTool.RandomShuffle(randQuestList);
        }
    }
}