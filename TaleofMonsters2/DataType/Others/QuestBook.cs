using ConfigDatas;
using TaleofMonsters.DataType.User;

namespace TaleofMonsters.DataType.Others
{
    internal static class QuestBook
    {
        public static bool HasFlag(string f)
        {
            foreach (var questId in UserProfile.InfoQuest.QuestFinish)
            {
                var config = ConfigData.GetQuestConfig(questId);
                if (config.Ename == f)
                {
                    return true;
                }
            }
            return false;
        }
    }
}