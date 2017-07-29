using System;
using ConfigDatas;
using TaleofMonsters.DataType;
using TaleofMonsters.DataType.Others;
using TaleofMonsters.DataType.Peoples;
using TaleofMonsters.DataType.User;
using TaleofMonsters.MainItem.Scenes;

namespace TaleofMonsters.MainItem.Quests.SceneQuests
{
    internal class SceneQuestAnswer : SceneQuestBlock
    {
        public SceneQuestAnswer(int eid, int lv, string s, int depth, int line)
            : base(eid, lv, s, depth, line)
        {
            CheckScript();
        }

        private void CheckScript()
        {
            if (Script[0] == '|')
            {
                string[] infos = Script.Split('|');
                Script = infos[infos.Length - 1];
                for (int i = 1; i < infos.Length-1; i++)
                {
                    CheckCondition(infos[i]);
                    if (Disabled)
                    {
                        return; //有一个条件不满足就结束
                    }
                }
                if (infos.Length > 1)
                {
                    Prefix = infos[1];
                }
            }
        }

        private void CheckCondition(string info)
        {
            string[] parms = info.Split('-');
            var config = ConfigData.GetSceneQuestConfig(eventId);

            if (parms[0] == "rivalavail")
            {
                Disabled = !UserProfile.InfoRival.GetRivalState(config.EnemyId).Avail;
            }
            else if (parms[0] == "rivalavailno")
            {
                Disabled = UserProfile.InfoRival.GetRivalState(config.EnemyId).Avail;
            }
            else if (parms[0] == "sceneno")
            {
                Disabled = Scene.Instance.HasSceneItemWithName(parms[1]);
            }
            else if (parms[0] == "cantrade")
            {
                int multi = int.Parse(parms[1]);
                uint goldNeed = 0;
                if (config.TradeGold < 0)
                    goldNeed = GameResourceBook.OutGoldSceneQuest(level, -config.TradeGold*multi, true);
                uint foodNeed = 0;
                if (config.TradeFood < 0)
                    foodNeed = Math.Min(100, GameResourceBook.OutFoodSceneQuest(-config.TradeFood*multi, true));
                uint healthNeed = 0;
                if (config.TradeHealth < 0)
                    healthNeed = Math.Min(100, GameResourceBook.OutHealthSceneQuest(-config.TradeHealth*multi, true));
                uint mentalNeed = 0;
                if (config.TradeMental < 0)
                    mentalNeed = Math.Min(100, GameResourceBook.OutMentalSceneQuest(-config.TradeMental*multi, true));
                Disabled = !UserProfile.Profile.InfoBag.HasResource(GameResourceType.Gold, goldNeed) ||
                           UserProfile.Profile.InfoBasic.FoodPoint < foodNeed ||
                           UserProfile.Profile.InfoBasic.HealthPoint < healthNeed ||
                           UserProfile.Profile.InfoBasic.MentalPoint < mentalNeed;

                uint goldAdd = 0;
                if (config.TradeGold > 0)
                    goldAdd = GameResourceBook.InGoldSceneQuest(level, config.TradeGold*multi, true);
                uint foodAdd = 0;
                if (config.TradeFood > 0)
                    foodAdd = Math.Min(100, GameResourceBook.InFoodSceneQuest(config.TradeFood*multi, true));
                uint healthAdd = 0;
                if (config.TradeHealth > 0)
                    healthAdd = Math.Min(100, GameResourceBook.InHealthSceneQuest(config.TradeHealth*multi, true));
                uint mentalAdd = 0;
                if (config.TradeMental > 0)
                    mentalAdd = Math.Min(100, GameResourceBook.InMentalSceneQuest(config.TradeMental*multi, true));
                Script = string.Format("增加{0}(消耗{1})",
                                       GetTradeStr(goldAdd, foodAdd, healthAdd, mentalAdd),
                                       GetTradeStr(goldNeed, foodNeed, healthNeed, mentalNeed));
            }
            else if (parms[0] == "hasrival")
            {
                int pid = PeopleBook.GetRandomPeopleId(level - 3, level + 3);
                Disabled = UserProfile.InfoRival.GetRivalState(pid).Avail;
                UserProfile.InfoRecord.SetRecordById((int) MemPlayerRecordTypes.SceneQuestRandPeopleId, pid);
                Script = string.Format("偶遇了{0}，我们来切磋一下吧", ConfigData.GetPeopleConfig(pid).Name);
            }
        }

        private string GetTradeStr(uint goldNeed, uint foodNeed, uint healthNeed, uint mentalNeed)
        {
            string addStr = "";
            if (goldNeed > 0) addStr += goldNeed + "点金币 ";
            if (foodNeed > 0) addStr += foodNeed + "点食物 ";
            if (healthNeed > 0) addStr += healthNeed + "点生命 ";
            if (mentalNeed > 0) addStr += mentalNeed + "点精神 ";

            return addStr;
        }
    }
}