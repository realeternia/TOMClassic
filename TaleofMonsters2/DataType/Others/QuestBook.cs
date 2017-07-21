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
            foreach (var questData in UserProfile.InfoQuest.QuestRunning)
            {
                var config = ConfigData.GetQuestConfig(questData.QuestId);
                if (config.Ename == f)
                {
                    return questData.State >= (byte)QuestStates.Accomplish;
                }
            }
            return false;
        }

        public static void SetFlag(string f)
        {
            foreach (var questData in UserProfile.InfoQuest.QuestRunning)
            {
                var config = ConfigData.GetQuestConfig(questData.QuestId);
                if (config.Ename == f)
                {
                    UserProfile.InfoQuest.SetQuestState(config.Id, QuestStates.Accomplish);
                    return;
                }
            }
        }
    }
}