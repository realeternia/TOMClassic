﻿using System;
using ConfigDatas;
using NarlonLib.Math;
using TaleofMonsters.DataType;
using TaleofMonsters.DataType.Drops;
using TaleofMonsters.DataType.Others;
using TaleofMonsters.DataType.Peoples;
using TaleofMonsters.DataType.User;
using TaleofMonsters.MainItem.Blesses;
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
                        return; //有一个条件不满足就结束
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
                Disabled = !UserProfile.InfoRival.GetRivalState(PeopleBook.GetPeopleId(config.EnemyName)).Avail;
            }
            else if (parms[0] == "rivalavailno")
            {
                Disabled = UserProfile.InfoRival.GetRivalState(PeopleBook.GetPeopleId(config.EnemyName)).Avail;
            }
            else if (parms[0] == "eventcount")
            {
                Disabled = Scene.Instance.CountOpenedQuest(config.CheckQuest) != int.Parse(parms[1]);
            }
            else if (parms[0] == "cantrade")
            {
                int multi = int.Parse(parms[1]);
                double multiNeed = multi*MathTool.Clamp(1 + BlessManager.TradeNeedRate, 0.2, 5);
                double multiGet = multi * MathTool.Clamp(1 + BlessManager.TradeAddRate, 0.2, 5);
                uint goldNeed = 0;
                if (config.TradeGold < 0)
                    goldNeed = GameResourceBook.OutGoldSceneQuest(level, (int)(-config.TradeGold* multiNeed), true);
                uint foodNeed = 0;
                if (config.TradeFood < 0)
                    foodNeed = Math.Min(100, GameResourceBook.OutFoodSceneQuest((int)(-config.TradeFood* multiNeed), true));
                uint healthNeed = 0;
                if (config.TradeHealth < 0)
                    healthNeed = Math.Min(100, GameResourceBook.OutHealthSceneQuest((int)(-config.TradeHealth* multiNeed), true));
                uint mentalNeed = 0;
                if (config.TradeMental < 0)
                    mentalNeed = Math.Min(100, GameResourceBook.OutMentalSceneQuest((int)(-config.TradeMental* multiNeed), true));
                Disabled = !UserProfile.Profile.InfoBag.HasResource(GameResourceType.Gold, goldNeed) ||
                           UserProfile.Profile.InfoBasic.FoodPoint < foodNeed ||
                           UserProfile.Profile.InfoBasic.HealthPoint < healthNeed ||
                           UserProfile.Profile.InfoBasic.MentalPoint < mentalNeed;

                if (string.IsNullOrEmpty(config.TradeDropItem))
                {
                    uint goldAdd = 0;
                    if (config.TradeGold > 0)
                        goldAdd = GameResourceBook.InGoldSceneQuest(level, (int)(config.TradeGold* multiGet), true);
                    uint foodAdd = 0;
                    if (config.TradeFood > 0)
                        foodAdd = Math.Min(100, GameResourceBook.InFoodSceneQuest((int)(config.TradeFood* multiGet), true));
                    uint healthAdd = 0;
                    if (config.TradeHealth > 0)
                        healthAdd = Math.Min(100, GameResourceBook.InHealthSceneQuest((int)(config.TradeHealth* multiGet), true));
                    uint mentalAdd = 0;
                    if (config.TradeMental > 0)
                        mentalAdd = Math.Min(100, GameResourceBook.InMentalSceneQuest((int)(config.TradeMental* multiGet), true));
                    Script = string.Format("获得{0}(消耗{1})",
                        GetTradeStr(goldAdd, foodAdd, healthAdd, mentalAdd),
                        GetTradeStr(goldNeed, foodNeed, healthNeed, mentalNeed));
                }
                else
                {
                    var dropId = DropBook.GetDropId(config.TradeDropItem);
                    Script = string.Format("获得{0}(消耗{1})",
                        ConfigData.GetDropConfig(dropId).Name,
                        GetTradeStr(goldNeed, foodNeed, healthNeed, mentalNeed));

                }
            }
            else if (parms[0] == "hasrival")
            {
                int pid = PeopleBook.GetRandomPeopleId(level - 3, level + 3);
                Disabled = UserProfile.InfoRival.GetRivalState(pid).Avail;
                UserProfile.InfoRecord.SetRecordById((int) MemPlayerRecordTypes.SceneQuestRandPeopleId, pid);
                Script = string.Format("偶遇了{0}，我们来切磋一下吧", ConfigData.GetPeopleConfig(pid).Name);
            }
            else if (parms[0] == "cantest")
            {
                int type = int.Parse(parms[1]);
                bool canConvert = type == 1; //是否允许转换成幸运检测

                var testType = type == 1 ? config.TestType1 : config.TestType2;
                int sourceVal = UserProfile.InfoDungeon.GetAttrByStr(testType);
                Disabled = UserProfile.InfoDungeon.DungeonId <= 0 || sourceVal < 0;
                if (Disabled && canConvert)
                    Disabled = false;

                if (!Disabled)
                {
                    var biasData = type == 1 ? config.TestBias1 : config.TestBias2;
                    if (UserProfile.InfoDungeon.DungeonId > 0 && UserProfile.InfoDungeon.GetAttrByStr(testType) >= 0)
                    {
                        var attrNeed = UserProfile.InfoDungeon.GetRequireAttrByStr(testType, biasData);
                        Script = string.Format("进行{0}考验(判定{1} {2:0.0}%胜率)", GetTestAttrStr(testType), attrNeed, 
                            GetWinRate(UserProfile.InfoDungeon.GetAttrByStr(testType) +0.5f, attrNeed));
                    }
                    else //因为convert了
                    {
                        Script = string.Format("进行运气考验(判定{0} {1:0.0}%胜率)", 3 + biasData, 
                            GetWinRate(3.5f, 3 + biasData));
                    }
                }
            }
        }

        private float GetWinRate(float myData, float needData)
        {
            if (myData*2 < needData)
                return 0;
            if (myData*2 == needData)
                return (float)Math.Pow(1/3, myData) * 100;
            return myData*115/(myData + needData);
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

        private string GetTestAttrStr(string type)
        {
            switch (type)
            {
                case "str": return "力量";
                case "agi": return "敏捷";
                case "intl": return "智慧";
                case "perc": return "感知";
                case "endu": return "耐力";
            }
            return "未知";
        }
    }
}