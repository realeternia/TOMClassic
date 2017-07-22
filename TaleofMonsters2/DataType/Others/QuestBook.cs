using System.Drawing;
using ConfigDatas;
using NarlonLib.Drawing;
using TaleofMonsters.Core;
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
                    return questData.State >= (byte) QuestStates.Accomplish;
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

        public static Image GetPreview(int id)
        {
            QuestConfig questConfig = ConfigData.GetQuestConfig(id);
            ControlPlus.TipImage tipData = new ControlPlus.TipImage();
            tipData.AddTextNewLine(questConfig.Name, "Lime", 20);
            tipData.AddLine();
            tipData.AddTextNewLine("难度:" + GetTaskHardness(questConfig.Y), "White");
            if (questConfig.NpcId > 0)
            {
                SceneQuestConfig npcConfig = ConfigData.GetSceneQuestConfig(questConfig.NpcId);
                tipData.AddTextNewLine("委托人:" + npcConfig.Name, "White");
            }
            tipData.AddTextLines(questConfig.Descript, "White", 20, true);
            return tipData.Image;
        }

        private static string GetTaskHardness(int level)
        {
            if (level <= 3)
                return "新手";
            if (level <= 6)
                return "容易";
            if (level <= 9)
                return "适中";
            if (level <= 12)
                return "困难";
            if (level <= 14)
                return "噩梦";
            return "地狱";
        }
    }
}