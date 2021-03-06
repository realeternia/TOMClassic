﻿using System;
using ConfigDatas;
using TaleofMonsters.Controler.Battle;
using TaleofMonsters.Controler.Battle.Tool;
using TaleofMonsters.Datas;
using TaleofMonsters.Datas.Scenes;
using TaleofMonsters.Datas.User;
using TaleofMonsters.Forms;
using TaleofMonsters.Forms.CMain;
using TaleofMonsters.Forms.CMain.Blesses;
using TaleofMonsters.Forms.CMain.Scenes;

namespace TaleofMonsters.Controler.GM
{
    internal class GMCommand
    {
        public static bool IsCommand(string c)
        {
            switch (c)
            {
                case "exp": break;
                case "cad": break;
                case "atp": break;
                case "mov": break;
                case "eqp": break;
                case "eqps": break;
                case "itm": break;
                case "emys": break;
                case "gold": break;
                case "res": break;
                case "dmd": break;
                case "acv": break;
                case "view": break;
                case "fbat": break;
                case "cbat": break;
                case "scr": break;
                case "sceq": break;
                case "cure":  break;
                case "bls":  break;
                case "qst": break;
                default: return false;
            }
            return true;
        }

        public static void ParseCommand(string cmd)
        {
            string[] data = cmd.Split(' ');
            if (data.Length == 0) return;
            try
            {
                switch (data[0])
                {
                    case "exp": if (data.Length == 2) UserProfile.InfoBasic.AddExp(int.Parse(data[1])); break;
                    case "cad": if (data.Length == 2) UserProfile.InfoCard.AddCard(int.Parse(data[1])); break;
                    case "mov": if (data.Length == 2) UserProfile.InfoBasic.Position = 0;//如果是0，后面流程会随机一个位置
                        Scene.Instance.ChangeMap(int.Parse(data[1]), true); break;
                    case "eqp": if (data.Length == 2) UserProfile.InfoCastle.AddEquip(int.Parse(data[1]), 0); break;
                    case "eqps":
                        foreach (int eid in ConfigData.EquipDict.Keys)
                            UserProfile.InfoCastle.AddEquip(eid, 0);
                        break;
                    case "itm": if (data.Length == 3) UserProfile.InfoBag.AddItem(int.Parse(data[1]), int.Parse(data[2])); break;
                    case "emys": foreach (int peopleId in ConfigData.PeopleDict.Keys)
                        {
                            UserProfile.InfoRival.SetRivalAvail(peopleId);
                        }
                        break;
                    case "gold": if (data.Length == 2)
                        {
                            UserProfile.InfoBag.AddResource(GameResourceType.Gold, uint.Parse(data[1]));
                        } break;
                    case "res": if (data.Length == 2)
                    {
                        var v = uint.Parse(data[1]);
                            UserProfile.InfoBag.AddResource(new uint[] { 0, v, v, v, v, v, v });
                        } break;
                    case "dmd": if (data.Length == 2) UserProfile.InfoBag.AddDiamond(int.Parse(data[1])); break;
                    case "acv": if (data.Length == 2) UserProfile.Profile.InfoGismo.AddGismo(int.Parse(data[1])); break;
                    case "fbat": if (data.Length == 3)
                        {
                            FastBattle.Instance.StartGame(int.Parse(data[1]), int.Parse(data[2]), "default", TileConfig.Indexer.DefaultTile);
                            MainTipManager.AddTip(string.Format("{0} {1}合", FastBattle.Instance.LeftWin ? "左胜" : "右胜", BattleManager.Instance.StatisticData.Round), "White");
                        } break;
                    case "cbat":
                        if (data.Length == 4)
                        {
                            var result = CardFastBattle.Instance.StartGame(int.Parse(data[1]), int.Parse(data[2]), int.Parse(data[3]));
                            MainTipManager.AddTip(string.Format("{0} {1}合", result, BattleManager.Instance.StatisticData.Round), "White");
                        }break;
                    case "scr": if (data.Length == 2)
                        {
                            switch (data[1])
                            {
                                case "Vs": GmScript.MonsterVsBatch();break;
                            }
                        } break;
                    case "sceq":
                        NpcTalkForm sw = new NpcTalkForm();
                        sw.EventId = SceneQuestBook.GetSceneQuestByName(data[1]);
                        PanelManager.DealPanel(sw); break;
                    case "cure": UserProfile.InfoBasic.MentalPoint=100; UserProfile.InfoBasic.HealthPoint=100;
                        UserProfile.InfoBasic.FoodPoint = 100;break;
                    case "bls": if (data.Length == 2)
                        BlessManager.AddBless(int.Parse(data[1])); break;
                    case "qst":if (data.Length == 2)
                        UserProfile.InfoQuest.SetQuestState(int.Parse(data[1]), QuestStates.Receive); break;
                }
            }
            catch (FormatException) { }
            catch (IndexOutOfRangeException) { }
        }
    }
}
