using System.Collections.Generic;
using ConfigDatas;
using TaleofMonsters.Core;
using TaleofMonsters.DataType.Scenes;
using TaleofMonsters.MainItem;

namespace TaleofMonsters.DataType.User
{
    public class InfoGismo
    {
        [FieldIndex(Index = 1)] public Dictionary<int, bool> Gismos;

        private int nowDungeonId;
        private List<int> gismoList;

        public InfoGismo()
        {
            Gismos = new Dictionary<int, bool>();
        }

        public void AddGismo(int id)
        {
            if (!Gismos.ContainsKey(id))
            {
                Gismos.Add(id, true);
                MainTipManager.AddTip(string.Format("|��óɾ�|Gold|{0}", ConfigData.GetDungeonGismoConfig(id).Name), "White");
            }
        }

        public bool GetGismo(int id)
        {
            if (Gismos.ContainsKey(id))
            {
                return true;
            }
            return false;
        }

        public void CheckWinCount()
        {
            if (CheckState())
            {
                foreach (var gismoId in gismoList)
                {
                    if (GetGismo(gismoId)) //�Ѿ�����
                        continue;

                    var gismoConfig = ConfigData.GetDungeonGismoConfig(gismoId);
                    if (!CheckEnvironmentState(gismoConfig))
                        return;

                    if (gismoConfig.WinCount > 0 && UserProfile.InfoDungeon.FightWin >= gismoConfig.WinCount)
                        AddGismo(gismoId);
                }
            }
        }

        public void CheckEventList()
        {
            if (CheckState())
            {
                foreach (var gismoId in gismoList)
                {
                    if (GetGismo(gismoId)) //�Ѿ�����
                        continue;

                    var gismoConfig = ConfigData.GetDungeonGismoConfig(gismoId);
                    if (!CheckEnvironmentState(gismoConfig))
                        return;
                                        
                    if (!string.IsNullOrEmpty(gismoConfig.FinishSceneQuest))
                    {
                        var questId = SceneQuestBook.GetSceneQuestByName(gismoConfig.FinishSceneQuest);
                        if (gismoConfig.FinishContinueCount > 0)
                        {
                            if (UserProfile.InfoDungeon.CheckQuestCount(questId, gismoConfig.FinishState, gismoConfig.FinishContinueCount, true))
                                AddGismo(gismoId);
                        }
                        else if (gismoConfig.FinishCount > 0)
                        {
                            if (UserProfile.InfoDungeon.CheckQuestCount(questId, gismoConfig.FinishState, gismoConfig.FinishCount, false))
                                AddGismo(gismoId);
                        }
                    }
                }
            }
        }

        private bool CheckState()
        {
            if (UserProfile.InfoDungeon.DungeonId <= 0)
                return false;

            if (gismoList == null || UserProfile.InfoDungeon.DungeonId != nowDungeonId)
            {
                nowDungeonId = UserProfile.InfoDungeon.DungeonId;
                gismoList = new List<int>();
                foreach (var gismoConfig in ConfigData.DungeonGismoDict.Values)
                {
                    if(gismoConfig.DungeonId == nowDungeonId)
                        gismoList.Add(gismoConfig.Id);
                }
            }

            return true;
        }

        private bool CheckEnvironmentState(DungeonGismoConfig config)
        {
            if (config.StepCost > 0)
            {
                if (UserProfile.InfoDungeon.Step > config.StepCost)
                    return false;
            }
            return true;
        }
    }
}