using System;
using ConfigDatas;
using TaleofMonsters.Controler.Battle;
using TaleofMonsters.Controler.Battle.Tool;
using TaleofMonsters.DataType.Others;
using TaleofMonsters.DataType.Peoples;
using TaleofMonsters.DataType.User;
using TaleofMonsters.DataType.User.Db;
using TaleofMonsters.Forms;
using TaleofMonsters.MainItem.Scenes;

namespace TaleofMonsters.Controler.GM
{
    internal class GMCommand
    {
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
                    case "atp": if (data.Length == 2) UserProfile.InfoBasic.AttrPoint += int.Parse(data[1]); break;
                    case "mov": if (data.Length == 2) Scene.Instance.ChangeMap(int.Parse(data[1]), true); break;
                    case "eqp": if (data.Length == 2) UserProfile.InfoEquip.AddEquip(int.Parse(data[1])); break;
                    case "itm": if (data.Length == 3) UserProfile.InfoBag.AddItem(int.Parse(data[1]), int.Parse(data[2])); break;
                    case "emys": foreach (int peopleId in ConfigData.PeopleDict.Keys)
                        {
                            DbRivalState memRival = new DbRivalState(peopleId);
                            memRival.Avail = true;
                            UserProfile.InfoRival.Rivals[peopleId] = memRival;
                        }
                        break;
                    case "gold": if (data.Length == 2)
                        {
                            GameResource res = new GameResource();
                            res.Gold = int.Parse(data[1]);
                            UserProfile.InfoBag.AddResource(res.ToArray());
                        } break;
                    case "dmd": if (data.Length == 2) UserProfile.InfoBag.AddDiamond(int.Parse(data[1])); break;
                    case "tsk": if (data.Length == 2) UserProfile.InfoTask.BeginTask(int.Parse(data[1])); break;
                    case "acv": if (data.Length == 2) UserProfile.Profile.InfoAchieve.SetAchieve(int.Parse(data[1])); break;
                    case "view": if (data.Length == 3) PeopleBook.ViewMatch(int.Parse(data[1]), int.Parse(data[2]), "default", TileConfig.Indexer.DefaultTile); break;
                    case "fbat": if (data.Length == 3)
                        {
                            FastBattle.Instance.StartGame(int.Parse(data[1]), int.Parse(data[2]), "default", TileConfig.Indexer.DefaultTile);
                            MainForm.Instance.AddTip(string.Format("{0} {1}合", FastBattle.Instance.LeftWin ? "左胜" : "右胜", BattleManager.Instance.StatisticData.Round), "White");
                        } break;
                    case "cbat":
                        if (data.Length == 4)
                        {
                            var result = CardFastBattle.Instance.StartGame(int.Parse(data[1]), int.Parse(data[2]), int.Parse(data[3]));
                            MainForm.Instance.AddTip(string.Format("{0} {1}合", result, BattleManager.Instance.StatisticData.Round), "White");
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
                        sw.EventId = int.Parse(data[1]);
                        sw.NeedBlackForm = true;
                        MainForm.Instance.DealPanel(sw); break;
                    case "cure": UserProfile.InfoBasic.MentalPoint=100; UserProfile.InfoBasic.HealthPoint=100;
                        UserProfile.InfoBasic.FoodPoint = 100;break;
                }
            }
            catch (FormatException) { }
            catch (IndexOutOfRangeException) { }
        }
    }
}
