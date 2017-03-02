using System.Collections.Generic;
using ConfigDatas;
using NarlonLib.Log;
using NarlonLib.Math;
using TaleofMonsters.DataType.User;

namespace TaleofMonsters.MainItem.Scenes.SceneRules
{
    public class SceneRuleCommon : ISceneRule
    {
        private int mapId;
        private int minutes;

        public void Init(int id, int minute)
        {
            mapId = id;
            UserProfile.InfoWorld.SavedDungeonQuests.Clear();
            minutes = minute;
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

            if (randQuestList.Count > questCellCount)
            {
                randQuestList.RemoveRange(questCellCount, randQuestList.Count - questCellCount);
                NLog.Warn(string.Format("Generate id={0} size too big {1}", mapId, randQuestList.Count));
            }
            else
            {
                ListTool.Fill(randQuestList, 0, questCellCount);
            }
            ListTool.RandomShuffle(randQuestList);
        }
    }
}