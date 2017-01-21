using System;
using ConfigDatas;
using TaleofMonsters.DataType;
using TaleofMonsters.DataType.Others;
using TaleofMonsters.DataType.User;

namespace TaleofMonsters.MainItem.Quests.SceneQuests
{
    public class SceneQuestAnswer : SceneQuestBlock
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
                string[] parms = infos[1].Split('-');
                var config = ConfigData.GetSceneQuestConfig(eventId);

                if (parms[0] == "flagno")
                {
                    Disabled = UserProfile.InfoRecord.CheckFlag((uint)((MemPlayerFlagTypes)Enum.Parse(typeof(MemPlayerFlagTypes), parms[1])));
                }
                else if (parms[0] == "flag")
                {
                    Disabled = !UserProfile.InfoRecord.CheckFlag((uint)((MemPlayerFlagTypes)Enum.Parse(typeof(MemPlayerFlagTypes), parms[1])));
                }
                else if (parms[0] == "rivalavail")
                {
                    Disabled = !UserProfile.InfoRival.GetRivalState(config.EnemyId).Avail;
                }
                else if (parms[0] == "rivalavailno")
                {
                    Disabled = UserProfile.InfoRival.GetRivalState(config.EnemyId).Avail;
                }
                else if (parms[0] == "cantrade")
                {
                    int multi = int.Parse(parms[1]);
                    uint goldNeed=0;
                    if (config.TradeGold < 0)
                        goldNeed = GameResourceBook.OutGoldSceneQuest(level, -config.TradeGold * multi, true);
                    uint foodNeed = 0;
                    if (config.TradeFood < 0)
                        foodNeed = GameResourceBook.OutFoodSceneQuest(-config.TradeFood * multi, true);
                    uint healthNeed = 0;
                    if (config.TradeHealth < 0)
                        healthNeed = GameResourceBook.OutHealthSceneQuest(-config.TradeHealth * multi, true);
                    uint mentalNeed = 0;
                    if (config.TradeMental < 0)
                        mentalNeed = GameResourceBook.OutMentalSceneQuest(-config.TradeMental * multi, true);
                    Disabled = !UserProfile.Profile.InfoBag.HasResource(GameResourceType.Gold, goldNeed)||
                            UserProfile.Profile.InfoBasic.FoodPoint < foodNeed ||
                             UserProfile.Profile.InfoBasic.HealthPoint < healthNeed ||
                              UserProfile.Profile.InfoBasic.MentalPoint < mentalNeed;

                    uint goldAdd=0;
                    if (config.TradeGold > 0)
                        goldAdd = GameResourceBook.InGoldSceneQuest(level, config.TradeGold * multi, true);
                    uint foodAdd = 0;
                    if (config.TradeFood > 0)
                        foodAdd = GameResourceBook.InFoodSceneQuest(config.TradeFood * multi, true);
                    uint healthAdd = 0;
                    if (config.TradeHealth > 0)
                        healthAdd = GameResourceBook.InHealthSceneQuest(config.TradeHealth * multi, true);
                    uint mentalAdd = 0;
                    if (config.TradeMental > 0)
                        mentalAdd = GameResourceBook.InMentalSceneQuest(config.TradeMental * multi, true);
                    Script = string.Format("增加{0}(消耗{1})",
                        GetTradeStr(goldAdd, foodAdd, healthAdd, mentalAdd),
                        GetTradeStr(goldNeed, foodNeed, healthNeed, mentalNeed));
                }
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