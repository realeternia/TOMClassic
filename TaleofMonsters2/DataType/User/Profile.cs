using System;
using TaleofMonsters.Controler.World;
using TaleofMonsters.Core;
using ConfigDatas;
using NarlonLib.Tools;

namespace TaleofMonsters.DataType.User
{
    public class Profile
    {
        [FieldIndex(Index = 1)]
        public int Pid;
        [FieldIndex(Index = 2)]
        public string Name;
        [FieldIndex(Index = 3)]
        public InfoBasic InfoBasic;
        [FieldIndex(Index = 4)]
        public InfoBag InfoBag;
        [FieldIndex(Index = 5)]
        public InfoCard InfoCard;
        [FieldIndex(Index = 8)]
        public InfoRival InfoRival;
        [FieldIndex(Index = 9)]
        public InfoFarm InfoFarm;
        [FieldIndex(Index = 10)]
        public InfoEquip InfoEquip;
        [FieldIndex(Index = 11)]
        public InfoQuest InfoQuest;
        [FieldIndex(Index = 12)]
        public InfoRecord InfoRecord;
        [FieldIndex(Index = 13)]
        public InfoGismo InfoGismo;
        [FieldIndex(Index = 14)]
        public InfoWorld InfoWorld;

        public Profile()
        {
            InfoBasic = new InfoBasic();
            InfoBag = new InfoBag();
            InfoEquip = new InfoEquip();
            InfoCard = new InfoCard();
            InfoRival=new InfoRival();
            InfoFarm=new InfoFarm();
            InfoQuest=new InfoQuest();
            InfoRecord=new InfoRecord();
            InfoGismo=new InfoGismo();
            InfoWorld = new InfoWorld();
        }

        public void OnCreate(int constellation, int bldType, int headId)
        {
            Pid = WorldInfoManager.GetPlayerPid();
            Name = UserProfile.ProfileName;
            InfoBasic.Job = JobConfig.Indexer.NewBie;
            InfoBasic.Constellation = constellation + 1;
            InfoBasic.BloodType = bldType + 1;
            InfoBasic.Face = headId;
            InfoBasic.Level = 1;
            InfoBasic.MapId = 13010001;
            InfoBasic.Position = 1001;
            InfoBasic.HealthPoint = 100;
            InfoBasic.MentalPoint = 100;
            InfoBasic.FoodPoint = 100;
            InfoBag.BagCount = 50;
            InfoEquip.AddEquipCompose(21100001);
            InfoEquip.AddEquipCompose(21300001);
        }

        public void OnKillMonster(int tlevel, int trace, int ttype)
        {
            InfoRecord.AddRecordById((int)MemPlayerRecordTypes.TotalKillByType + ttype, 1);
            InfoRecord.AddRecordById((int)MemPlayerRecordTypes.TotalKillByRace + trace, 1);
            InfoRecord.AddRecordById((int)MemPlayerRecordTypes.TotalKillByLevel + tlevel, 1);
            InfoRecord.AddRecordById((int)MemPlayerRecordTypes.TotalKill, 1);
        }

        public void OnUseCard(int tlevel, int trace, int ttype)
        {
            //InfoRecord.AddRecordById((int)MemPlayerRecordTypes.TotalSummonByType + ttype, 1);
            //if (ttype <= 8)
            //{
            //    InfoRecord.AddRecordById((int)MemPlayerRecordTypes.TotalSummonByRace + trace, 1);
            //    InfoRecord.AddRecordById((int)MemPlayerRecordTypes.TotalSummonByLevel + tlevel, 1);
            //    InfoRecord.AddRecordById((int)MemPlayerRecordTypes.TotalSummon, 1);
            //}
            //else if (ttype <= 12)
            //{
            //    InfoRecord.AddRecordById((int)MemPlayerRecordTypes.TotalWeapon, 1);
            //}
            //else
            //{
            //    InfoRecord.AddRecordById((int)MemPlayerRecordTypes.TotalSpell, 1);
            //}
        }

        public void OnLogin()
        {
            if (TimeManager.IsDifferDay(InfoBasic.LastLoginTime, TimeTool.DateTimeToUnixTime(DateTime.Now)))
            {
                OnNewDay();                
            }
            InfoBasic.LastLoginTime = TimeTool.DateTimeToUnixTime(DateTime.Now);
        }

        public void OnLogout()
        {
            int inter = TimeTool.DateTimeToUnixTime(DateTime.Now) - InfoBasic.LastLoginTime;
            InfoRecord.AddRecordById((int)MemPlayerRecordTypes.TotalOnline, inter / 60);
            InfoBasic.LastLoginTime = TimeTool.DateTimeToUnixTime(DateTime.Now);
            InfoQuest.OnLogout();
        }

        public void OnSwitchScene(bool isWarp)
        {
            InfoEquip.CheckExpireAndDura(true);
            InfoQuest.OnSwitchScene(isWarp);
            if(isWarp)
                UserProfile.SaveToDB();//每次切场景存个档
        }

        public void OnNewDay()
        {
            int inter = TimeTool.DateTimeToUnixTime(DateTime.Now) - InfoBasic.LastLoginTime;
            InfoRecord.AddRecordById((int)MemPlayerRecordTypes.TotalOnline, inter / 60);
            InfoBasic.LastLoginTime = TimeTool.DateTimeToUnixTime(DateTime.Now);
        }

        public void OnDie()
        {
            InfoBasic.HealthPoint = 50;
            InfoBasic.MentalPoint = 50;
            InfoBag.SubResource(GameResourceType.Gold, (uint)Math.Ceiling((double)InfoBag.Resource.Gold/5));
        }
    }
}
