using System.Collections.Generic;
using ConfigDatas;
using NarlonLib.Log;
using NarlonLib.Math;
using NarlonLib.Tools;
using TaleofMonsters.DataType.Quests;
using TaleofMonsters.DataType.User;

namespace TaleofMonsters.MainItem.Scenes.SceneRules
{
    public class SceneRuleCommon : ISceneRule
    {
        private int mapId;

        public void Init(int id, int minute)
        {
            mapId = id;
            UserProfile.InfoWorld.SavedDungeonQuests.Clear();
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

            if (randQuestList.Count > questCellCount)
            {
                randQuestList.RemoveRange(questCellCount, randQuestList.Count - questCellCount);
                NLog.Warn("Generate id={0} size too big {1}", mapId, randQuestList.Count);
            }
            else
            {
                ArraysUtils.Fill(randQuestList, 0, questCellCount);
            }
            ArraysUtils.RandomShuffle(randQuestList);
        }
    }
}