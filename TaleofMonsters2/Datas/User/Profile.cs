using System;
using ConfigDatas;
using NarlonLib.Tools;
using TaleofMonsters.Controler.World;
using TaleofMonsters.Core;
using TaleofMonsters.Datas.Blesses;
using TaleofMonsters.Rpc;

namespace TaleofMonsters.Datas.User
{
    public class Profile
    {
        [FieldIndex(Index = 1)] public int Pid;
        [FieldIndex(Index = 2)] public string Name; //玩家角色名
        [FieldIndex(Index = 3)] public InfoBasic InfoBasic;
        [FieldIndex(Index = 4)] public InfoBag InfoBag;
        [FieldIndex(Index = 5)] public InfoCard InfoCard;
        [FieldIndex(Index = 6)] public InfoDungeon InfoDungeon;
        [FieldIndex(Index = 8)] public InfoRival InfoRival;
        [FieldIndex(Index = 10)] public InfoCastle InfoCastle;
        [FieldIndex(Index = 11)] public InfoQuest InfoQuest;
        [FieldIndex(Index = 12)] public InfoRecord InfoRecord;
        [FieldIndex(Index = 13)] public InfoGismo InfoGismo;
        [FieldIndex(Index = 14)] public InfoWorld InfoWorld;

        public Profile()
        {
            InfoBasic = new InfoBasic();
            InfoBag = new InfoBag();
            InfoCastle = new InfoCastle();
            InfoCard = new InfoCard();
            InfoDungeon = new InfoDungeon();
            InfoRival = new InfoRival();
            InfoQuest = new InfoQuest();
            InfoRecord = new InfoRecord();
            InfoGismo = new InfoGismo();
            InfoWorld = new InfoWorld();
        }

        public void OnCreate(string name, uint dna, int headId)
        {
            Name = name;
            InfoBasic.Job = JobConfig.Indexer.NewBie;
            InfoBasic.Dna = dna;
            InfoBasic.Head = headId;
            InfoBasic.Level = 1;
            InfoBasic.MapId = 13010001;
            InfoBasic.Position = 1001;
            InfoBasic.HealthPoint = 100;
            InfoBasic.MentalPoint = 100;
            InfoBasic.FoodPoint = 100;
            InfoBag.BagCount = GameConstants.BagInitCount;
            InfoWorld.AddBless(BlessBook.GetBlessByName("newbie"), 50);
        }

        public void OnKillMonster(int tlevel, int trace, int tattr)
        {
            InfoRecord.AddRecordById((int)MemPlayerRecordTypes.TotalKillAttr + tattr, 1);
            InfoRecord.AddRecordById((int)MemPlayerRecordTypes.TotalKillRace + trace, 1);
            InfoRecord.AddRecordById((int)MemPlayerRecordTypes.TotalKillLevel + tlevel, 1);
            InfoRecord.AddRecordById((int)MemPlayerRecordTypes.TotalKill, 1);
        }

        internal void OnUseCard(CardTypes type, int tlevel, int trace, int tattr)
        {
            InfoRecord.AddRecordById((int)MemPlayerRecordTypes.TotalUseAttr + tattr, 1);
            InfoRecord.AddRecordById((int)MemPlayerRecordTypes.TotalUseLevel + tlevel, 1);
            if (type == CardTypes.Monster)
            {
                InfoRecord.AddRecordById((int)MemPlayerRecordTypes.TotalUseRace + trace, 1);
                InfoRecord.AddRecordById((int)MemPlayerRecordTypes.TotalSummon, 1);
            }
            if (type == CardTypes.Weapon)
                InfoRecord.AddRecordById((int)MemPlayerRecordTypes.TotalWeapon, 1);
            else
                InfoRecord.AddRecordById((int)MemPlayerRecordTypes.TotalSpell, 1);
        }

        public void OnLogin()
        {
            if (TimeManager.IsDifferDay(InfoBasic.LastLoginTime, TimeTool.DateTimeToUnixTime(DateTime.Now)))
                OnNewDay();                
            InfoBasic.LastLoginTime = TimeTool.DateTimeToUnixTime(DateTime.Now);
        }

        public void OnLogout()
        {
            InfoBasic.LastLoginTime = TimeTool.DateTimeToUnixTime(DateTime.Now);
            InfoQuest.OnLogout();
        }

        public void OnSwitchScene(bool isWarp)
        {
            InfoQuest.OnSwitchScene(isWarp);
            if(isWarp)
                TalePlayer.Save();//每次切场景存个档
        }

        public void OnNewDay()
        {
            InfoBasic.LastLoginTime = TimeTool.DateTimeToUnixTime(DateTime.Now);
        }

        public void OnDie()
        {
            InfoBasic.HealthPoint = 50;
            InfoBasic.MentalPoint = 50;
            InfoBag.SubResource(GameResourceType.Gold, (uint)Math.Ceiling((double)InfoBag.Resource.Gold/5));

            InfoRecord.AddRecordById((int)MemPlayerRecordTypes.TotalDie, 1);
        }
    }
}
