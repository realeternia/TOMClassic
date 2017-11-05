using System.Collections.Generic;
using ConfigDatas;
using NarlonLib.Log;
using TaleofMonsters.Core;
using TaleofMonsters.DataType.User.Db;

namespace TaleofMonsters.DataType.User
{
    public class InfoDungeon
    {
        [FieldIndex(Index = 1)] public int DungeonId; //副本id

        [FieldIndex(Index = 2)] public int Str; //力量
        [FieldIndex(Index = 3)] public int Agi; //敏捷
        [FieldIndex(Index = 4)] public int Intl; //智慧
        [FieldIndex(Index = 5)] public int Perc; //感知
        [FieldIndex(Index = 6)] public int Endu; //耐力

        [FieldIndex(Index = 7)] public List<DbGismoState> EventList; //完成的任务列表，如果0，表示任务失败
        [FieldIndex(Index = 11)] public int FightWin;
        [FieldIndex(Index = 12)] public int FightLoss;

        public InfoDungeon()
        {
            EventList = new List<DbGismoState>();
        }

        public void Enter(int dungeonId) //进入副本需要初始化
        {
            DungeonId = dungeonId;
            var dungeonConfig = ConfigData.GetDungeonConfig(dungeonId);
            Str = dungeonConfig.Str;
            Agi = dungeonConfig.Agi;
            Intl = dungeonConfig.Intl;
            Perc = dungeonConfig.Perc;
            Endu = dungeonConfig.Endu;

            EventList = new List<DbGismoState>();
            FightWin = 0;
            FightLoss = 0;
        }

        public void Leave()
        {
            DungeonId = 0;
        }

        public int Step { get { return EventList.Count; } }

        public void OnEventEnd(int id, string type)
        {
            if (DungeonId > 0)
            {
                EventList.Add(new DbGismoState(id, type));
                UserProfile.InfoGismo.CheckEventList();
                NLog.Debug("OnEventEnd " +id.ToString() + " " + type);
            }
        }

        public bool CheckQuestCount(int qid, string state, int countNeed, bool needContinue)
        {
            if (EventList.Count == 0)
                return false;

            if (EventList[EventList.Count - 1].BaseId != qid)
                return false;

            int count = 0;
            for (int i = EventList.Count-1; i >=0 ; i--)
            {
                var checkData = EventList[i];
                if (checkData.BaseId == qid)
                {
                    if (state != "" && state != checkData.ResultName)
                        break;

                    count++;
                }
                else
                {
                    if(needContinue)
                        break;
                }
            }

            return count >= countNeed;
        }

        public int GetAttrByStr(string type)
        {
            switch (type)
            {
                case "str": return Str;
                case "agi": return Agi;
                case "intl": return Intl;
                case "perc": return Perc;
                case "endu": return Endu;
            }
            return -1;
        }

        public int GetRequireAttrByStr(string type, int biasData)
        {
            var dungeonConfig = ConfigData.GetDungeonConfig(DungeonId);
            switch (type)
            {
                case "str": return dungeonConfig.Str + biasData;
                case "agi": return dungeonConfig.Agi + biasData;
                case "intl": return dungeonConfig.Intl + biasData;
                case "perc": return dungeonConfig.Perc + biasData;
                case "endu": return dungeonConfig.Endu + biasData;
            }
            return 1;
        }
    }
}
