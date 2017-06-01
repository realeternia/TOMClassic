using System;
using NarlonLib.Core;
using TaleofMonsters.Controler.World;
using TaleofMonsters.Core;
using NarlonLib.Math;
using ConfigDatas;
using TaleofMonsters.Controler.Battle.Tool;

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
        [FieldIndex(Index = 6)]
        public InfoSkill InfoSkill;
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
        public InfoAchieve InfoAchieve;
        [FieldIndex(Index = 14)]
        public InfoWorld InfoWorld;

        public Profile()
        {
            InfoBasic = new InfoBasic();
            InfoBag = new InfoBag();
            InfoEquip = new InfoEquip();
            InfoCard = new InfoCard();
            InfoSkill = new InfoSkill();
            InfoRival=new InfoRival();
            InfoFarm=new InfoFarm();
            InfoQuest=new InfoQuest();
            InfoRecord=new InfoRecord();
            InfoAchieve=new InfoAchieve();
            InfoWorld = new InfoWorld();
        }

        public int GetAchieveState(int id)
        {
            if (InfoAchieve.GetAchieve(id))
            {
                return int.MaxValue;
            }

            AchieveConfig achieveConfig = ConfigData.GetAchieveConfig(id);
            switch (achieveConfig.Condition.Id)
            {
                case 1: return InfoBasic.Level;
                case 2: return InfoRecord.GetRecordById((int)MemPlayerRecordTypes.TotalWin);
                case 3: return InfoRecord.GetRecordById((int)MemPlayerRecordTypes.TotalKill);
                case 4: return InfoRecord.GetRecordById((int)MemPlayerRecordTypes.TotalSummon);
                case 5: return InfoRecord.GetRecordById((int)MemPlayerRecordTypes.TotalWeapon);
                case 6: return InfoRecord.GetRecordById((int)MemPlayerRecordTypes.TotalSpell);
                case 8: return InfoRival.GetRivalAvailCount();
                case 9: return InfoRecord.GetRecordById((int)MemPlayerRecordTypes.TotalOnline);
                case 11: return InfoRecord.GetRecordById((int)MemPlayerRecordTypes.TotalPick);
                case 12: return InfoRecord.GetRecordById((int)MemPlayerRecordTypes.TotalDig);
                case 13: return InfoRecord.GetRecordById((int)MemPlayerRecordTypes.TotalFish);
                case 14: return InfoRecord.GetRecordById((int)MemPlayerRecordTypes.ContinueWin);
                case 21: return InfoBag.Resource.Gold;
                case 22: return InfoBag.Resource.Lumber;
                case 23: return InfoBag.Resource.Stone;
                case 24: return InfoBag.Resource.Mercury;
                case 25: return InfoBag.Resource.Sulfur;
                case 26: return InfoBag.Resource.Carbuncle;
                case 27: return InfoBag.Resource.Gem;
                case 30: return InfoCard.GetCardCountByType(CardTypes.Null);
                case 31: return InfoCard.GetCardCountByType(CardTypes.Monster);
                case 32: return InfoCard.GetCardCountByType(CardTypes.Weapon);
                case 33: return InfoCard.GetCardCountByType(CardTypes.Spell);
                case 50: return BattleManager.Instance.StatisticData.FastWin;
                case 51: return BattleManager.Instance.StatisticData.Round;
                case 52: return BattleManager.Instance.StatisticData.Left.MonsterAdd;
                case 53: return BattleManager.Instance.StatisticData.Left.WeaponAdd;
                case 54: return BattleManager.Instance.StatisticData.Left.SpellAdd;
                case 55: return BattleManager.Instance.StatisticData.Items.Count;
                case 56: return BattleManager.Instance.StatisticData.ZeroDie;
                case 57: return BattleManager.Instance.StatisticData.OnlyMagic;
                case 58: return BattleManager.Instance.StatisticData.OnlySummon;
                case 60: return BattleManager.Instance.StatisticData.AlmostLost;
            }

            if (MathTool.ValueBetween(achieveConfig.Condition.Id, 100,108))
            {
                return InfoRecord.GetRecordById((int)MemPlayerRecordTypes.TotalKillByType + achieveConfig.Condition.Id - 100);
            }
            if (MathTool.ValueBetween(achieveConfig.Condition.Id, 110, 125))
            {
                return InfoRecord.GetRecordById((int)MemPlayerRecordTypes.TotalKillByRace + achieveConfig.Condition.Id - 110);
            }
            if (MathTool.ValueBetween(achieveConfig.Condition.Id, 131, 138))
            {
                return InfoRecord.GetRecordById((int)MemPlayerRecordTypes.TotalKillByLevel + achieveConfig.Condition.Id - 130);
            }
            if (MathTool.ValueBetween(achieveConfig.Condition.Id, 200, 216))
            {
                return InfoRecord.GetRecordById((int)MemPlayerRecordTypes.TotalSummonByType + achieveConfig.Condition.Id - 200);
            }
            if (MathTool.ValueBetween(achieveConfig.Condition.Id, 220, 235))
            {
                return InfoRecord.GetRecordById((int)MemPlayerRecordTypes.TotalSummonByRace + achieveConfig.Condition.Id - 220);
            }
            if (MathTool.ValueBetween(achieveConfig.Condition.Id, 241, 248))
            {
                return InfoRecord.GetRecordById((int)MemPlayerRecordTypes.TotalSummonByLevel + achieveConfig.Condition.Id - 240);
            }

            return 0;
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
            InfoEquip.AddEquipCompose(21200101);
            InfoEquip.AddEquipCompose(21300101);
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
        }

        public void OnSwitchScene()
        {
            InfoEquip.CheckExpireAndDura(true);
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
