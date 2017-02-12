using System.Collections.Generic;
using ConfigDatas;

namespace TaleofMonsters.DataType.Quests
{
    internal static class QuestBook
    {
        public static List<int> GetQuestIdByChapter(int cid)
        {
            List<int> ids = new List<int>();
            foreach (var questData in ConfigData.QuestDict.Values)
            {
                if (questData.Chapter == cid)
                {
                    ids.Add(questData.Id);
                }
            }
            return ids;
        }
    }
}
