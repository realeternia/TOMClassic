using System.Drawing;
using ConfigDatas;
using TaleofMonsters.DataType.User;

namespace TaleofMonsters.DataType.Quests
{
    internal static class QuestBook
    {
        public static int GetQuestIdByName(string f)
        {
            foreach (var questData in ConfigData.QuestDict.Values)
            {
                if (questData.Ename == f)
                {
                    return questData.Id;
                }
            }

            return 0;
        }

        public static bool HasQuest(string f)
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

        public static void SetQuestProgress(string f, byte progress)
        {
            foreach (var questData in UserProfile.InfoQuest.QuestRunning)
            {
                var config = ConfigData.GetQuestConfig(questData.QuestId);
                if (config.Ename == f)
                {
                    UserProfile.InfoQuest.AddQuestProgress(config.Id, progress);
                    return;
                }
            }
        }

        public static void CheckAllQuestWith(string mark)
        {
            foreach (var questData in UserProfile.InfoQuest.QuestRunning)
            {
                var config = ConfigData.GetQuestConfig(questData.QuestId);
                if (config.NeedAction == mark && UserProfile.InfoQuest.IsQuestCanProgress(config.Id))
                {
                    UserProfile.InfoQuest.AddQuestProgress(config.Id, 10);
                }
            }
        }

        public static string GetRewardStr(int qid)
        {
            string rt = "";
            var questConfig = ConfigData.GetQuestConfig(qid);
            if (questConfig.RewardGold > 0)
            {
                rt += "金钱 ";
            }
            if (questConfig.RewardExp > 0)
            {
                rt += "经验 ";
            }
            if (string.IsNullOrEmpty(questConfig.RewardItem) || string.IsNullOrEmpty(questConfig.RewardDrop))
            {
                rt += "道具 ";
            }
            if (questConfig.RewardFood > 0|| questConfig.RewardHealth > 0|| questConfig.RewardMental > 0)
            {
                rt += "生存 ";
            }
            if (questConfig.RewardBlessLevel > 0)
            {
                rt += "祝福 ";
            }
            return rt;
        }

        public static Image GetPreview(int id)
        {
            QuestConfig questConfig = ConfigData.GetQuestConfig(id);
            ControlPlus.TipImage tipData = new ControlPlus.TipImage();
            string nameStr = questConfig.Name;
            bool isFinish = UserProfile.InfoQuest.IsQuestFinish(id);
            bool isRecv = UserProfile.InfoQuest.IsQuestCanReceive(id);
            bool isReward = UserProfile.InfoQuest.IsQuestCanReward(id);
            string headColor = "Gray";
            if (isFinish)
            {
                nameStr += "(已完成)";
                headColor = "Gold";
            }
            if (isRecv)
            {
                nameStr += "(可接受)";
                headColor = "Cyan";
            }
            if (isReward)
            {
                nameStr += "(可提交)";
                headColor = "Lime";
            }
            tipData.AddTextNewLine(nameStr, headColor, 20);
            if (UserProfile.InfoQuest.IsQuestCanProgress(id))
                tipData.AddTextNewLine(string.Format(" 进度{0}%", UserProfile.InfoQuest.GetQuestProgress(id)*10), "White", 20);
            tipData.AddLine();
            tipData.AddTextNewLine("难度:" + GetTaskHardness(questConfig.Y), "White");
            if (questConfig.NpcId > 0)
            {
                SceneQuestConfig npcConfig = ConfigData.GetSceneQuestConfig(questConfig.NpcId);
                tipData.AddTextNewLine("委托人:" + npcConfig.Name, "White");
            }
            tipData.AddLine();
            tipData.AddRichTextLines(questConfig.Descript, "White", 20);
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