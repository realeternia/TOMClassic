using System.Collections.Generic;
using ConfigDatas;
using TaleofMonsters.Core;
using TaleofMonsters.Datas.Scenes;
using TaleofMonsters.Forms.CMain;

namespace TaleofMonsters.Datas.User
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
                MainTipManager.AddTip(string.Format("|获得成就|Gold|{0}", ConfigData.GetDungeonGismoConfig(id).Name), "White");
                UserProfile.InfoRecord.AddRecordById((int)MemPlayerRecordTypes.GismoGet, 1);
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
                    if (GetGismo(gismoId)) //已经有了
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
                    if (GetGismo(gismoId)) //已经有了
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
                    else if (!string.IsNullOrEmpty(gismoConfig.FinishSceneQuestTag))
                    {
                        if (gismoConfig.FinishContinueCount > 0)
                        {
                            if (UserProfile.InfoDungeon.CheckQuestTagCount(gismoConfig.FinishSceneQuestTag, gismoConfig.FinishState, gismoConfig.FinishContinueCount, true))
                                AddGismo(gismoId);
                        }
                        else if (gismoConfig.FinishCount > 0)
                        {
                            if (UserProfile.InfoDungeon.CheckQuestTagCount(gismoConfig.FinishSceneQuestTag, gismoConfig.FinishState, gismoConfig.FinishCount, false))
                                AddGismo(gismoId);
                        }
                    }
                }
            }
        }

        public void CheckDungeonItem()
        {
            if (CheckState())
            {
                foreach (var gismoId in gismoList)
                {
                    if (GetGismo(gismoId)) //已经有了
                        continue;

                    var gismoConfig = ConfigData.GetDungeonGismoConfig(gismoId);
                    if (!CheckEnvironmentState(gismoConfig))
                        return;

                    if (!string.IsNullOrEmpty(gismoConfig.NeedDungeonItemId) 
                        && UserProfile.InfoDungeon.HasDungeonItem(DungeonBook.GetDungeonItemId(gismoConfig.NeedDungeonItemId), gismoConfig.NeedDungeonItemCount))
                        AddGismo(gismoId);
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
            if (config.NeedStory > 0)
            {
                if (UserProfile.InfoDungeon.StoryId != config.NeedStory)
                    return false;
            }
            return true;
        }
    }
}
