using System;
using TaleofMonsters.Controler.World;
using TaleofMonsters.Core;
using ConfigDatas;
using NarlonLib.Tools;
using TaleofMonsters.DataType.Blesses;

namespace TaleofMonsters.DataType.User
{
    public class Profile
    {
        [FieldIndex(Index = 1)] public int Pid;
        [FieldIndex(Index = 2)] public string Name;
        [FieldIndex(Index = 3)] public InfoBasic InfoBasic;
        [FieldIndex(Index = 4)] public InfoBag InfoBag;
        [FieldIndex(Index = 5)] public InfoCard InfoCard;
        [FieldIndex(Index = 6)] public InfoDungeon InfoDungeon;
        [FieldIndex(Index = 8)] public InfoRival InfoRival;
        [FieldIndex(Index = 9)] public InfoFarm InfoFarm;
        [FieldIndex(Index = 10)] public InfoEquip InfoEquip;
        [FieldIndex(Index = 11)] public InfoQuest InfoQuest;
        [FieldIndex(Index = 12)] public InfoRecord InfoRecord;
        [FieldIndex(Index = 13)] public InfoGismo InfoGismo;
        [FieldIndex(Index = 14)] public InfoWorld InfoWorld;

        public Profile()
        {
            InfoBasic = new InfoBasic();
            InfoBag = new InfoBag();
            InfoEquip = new InfoEquip();
            InfoCard = new InfoCard();
            InfoDungeon = new InfoDungeon();
            InfoRival = new InfoRival();
            InfoFarm = new InfoFarm();
            InfoQuest = new InfoQuest();
            InfoRecord = new InfoRecord();
            InfoGismo = new InfoGismo();
            InfoWorld = new InfoWorld();
        }

        public void OnCreate(uint dna, int headId)
        {
            Name = UserProfile.ProfileName;
            InfoBasic.Job = JobConfig.Indexer.NewBie;
            InfoBasic.Dna = dna;
            InfoBasic.Head = headId;
            InfoBasic.Level = 1;
            InfoBasic.MapId = 13010001;
            InfoBasic.Position = 1001;
            InfoBasic.HealthPoint = 100;
            InfoBasic.MentalPoint = 100;
            InfoBasic.FoodPoint = 100;
            InfoBag.BagCount = 50;
            InfoEquip.AddEquipCompose(21100001);
            InfoEquip.AddEquipCompose(21300001);
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
            InfoEquip.CheckExpireAndDura(true);
            InfoQuest.OnSwitchScene(isWarp);
            if(isWarp)
                UserProfile.Save();//每次切场景存个档
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
