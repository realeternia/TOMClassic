using System.Collections.Generic;
using ConfigDatas;
using NarlonLib.Math;
using TaleofMonsters.DataType;
using TaleofMonsters.DataType.Scenes;
using TaleofMonsters.DataType.User;

namespace TaleofMonsters.MainItem.Scenes.SceneRules
{
    public class SceneRuleDungeon : ISceneRule
    {
        private int mapId;
        
        public void Init(int id)
        {
            mapId = id;
            if (UserProfile.InfoWorld.SavedDungeonQuests.Count == 0)
            {
                UserProfile.InfoRecord.SetRecordById((int)MemPlayerRecordTypes.DungeonQuestOffside, 0);
                foreach (var questData in GetDungeonQuest())
                {
                    for (int j = 0; j < questData.Value; j++)
                    {
                        UserProfile.InfoWorld.SavedDungeonQuests.Add(questData.Id);
                    }
                }
                ListTool.RandomShuffle(UserProfile.InfoWorld.SavedDungeonQuests);
            }
        }

        private List<RLIdValue> GetDungeonQuest()
        {
            var config = ConfigData.GetSceneConfig(mapId);
            List<RLIdValue> datas = new List<RLIdValue>();
            if (!string.IsNullOrEmpty(config.QuestDungeon))
            {
                string[] infos = config.QuestDungeon.Split('|');
                foreach (var info in infos)
                {
                    string[] questData = info.Split(';');
                    datas.Add(new RLIdValue
                    {
                        Id = SceneBook.GetSceneQuestByName(questData[0]),
                        Value = int.Parse(questData[1])
                    });
                }
            }
            return datas;
        }

        public void Generate(List<int> randQuestList, int questCellCount)
        {
            foreach (var questData in SceneManager.GetQuestConfigData(mapId))
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