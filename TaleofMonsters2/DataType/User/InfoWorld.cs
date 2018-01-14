using System;
using System.Collections.Generic;
using ConfigDatas;
using NarlonLib.Core;
using NarlonLib.Math;
using NarlonLib.Tools;
using TaleofMonsters.Config;
using TaleofMonsters.Controler.World;
using TaleofMonsters.Core;
using TaleofMonsters.DataType.Cards.Monsters;
using TaleofMonsters.DataType.Cards.Spells;
using TaleofMonsters.DataType.Cards.Weapons;
using TaleofMonsters.DataType.Equips;
using TaleofMonsters.DataType.Peoples;
using TaleofMonsters.DataType.Items;
using TaleofMonsters.DataType.Others;
using TaleofMonsters.DataType.Tournaments;
using TaleofMonsters.DataType.User.Db;
using TaleofMonsters.Forms.TourGame;

namespace TaleofMonsters.DataType.User
{
    public class InfoWorld
    {
        [FieldIndex(Index = 3)] public List<DbCardProduct> CardProducts;
        [FieldIndex(Index = 4)] public Dictionary<int, DbTournamentData> Tournaments;
        [FieldIndex(Index = 5)] public Dictionary<int, int> Ranks;
        [FieldIndex(Index = 6)] public List<DbMergeData> MergeMethods;
        [FieldIndex(Index = 7)] public List<DbSceneSpecialPosData> PosInfos; //记录当前场景随机后的格子信息
        [FieldIndex(Index = 8)] public Dictionary<int, int> Blesses;
        [FieldIndex(Index = 9)] public List<int> BlessShopItems;
        [FieldIndex(Index = 10)] public List<int> SavedDungeonQuests;
        [FieldIndex(Index = 11)] public List<int> DailyCardData;

        public InfoWorld()
        {
            CardProducts = new List<DbCardProduct>();
            Tournaments = new Dictionary<int, DbTournamentData>();
            Ranks = new Dictionary<int, int>();
            MergeMethods = new List<DbMergeData>();
            PosInfos = new List<DbSceneSpecialPosData>();
            Blesses = new Dictionary<int, int>();
            BlessShopItems = new List<int>();
            SavedDungeonQuests = new List<int>();
            DailyCardData = new List<int>();
        }

        internal DbCardProduct[] GetCardProductsByType(CardTypes type)
        {
            int time = TimeTool.DateTimeToUnixTime(DateTime.Now);
            if (CardProducts == null || UserProfile.InfoRecord.GetRecordById((int)MemPlayerRecordTypes.LastCardShopTime) < time - GameConstants.CardShopDura)
            {
                CardProducts = new List<DbCardProduct>();
                ReinstallCardProducts();
                UserProfile.InfoRecord.SetRecordById((int)MemPlayerRecordTypes.LastCardShopTime, TimeManager.GetTimeOnNextInterval(UserProfile.InfoRecord.GetRecordById((int)MemPlayerRecordTypes.LastCardShopTime), time, GameConstants.CardShopDura));
            }

            List<DbCardProduct> pros = new List<DbCardProduct>();
            foreach (DbCardProduct cardProduct in CardProducts)
            {
                if (ConfigIdManager.GetCardType(cardProduct.Cid) == type)
                    pros.Add(cardProduct);
            }

            return pros.ToArray();
        }

        public void RemoveCardProduct(int id)
        {
            foreach (DbCardProduct cardProduct in CardProducts)
            {
                if (cardProduct.Cid == id)
                {
                    CardProducts.Remove(cardProduct);
                    break;
                }
            }
        }

        private void ReinstallCardProducts()
        {
            int index = 1;
            foreach (var monsterConfig in ConfigData.MonsterDict.Values)
            {
                if (monsterConfig.IsSpecial > 0)
                    continue;
                Monster mon = new Monster(monsterConfig.Id);
                int rate = GetSellRate(monsterConfig.Id);
                CardProductMarkTypes mark = CardProductMarkTypes.Null;
                if (monsterConfig.IsNew > 0)
                {
                    rate = 100;
                    mark = CardProductMarkTypes.New;
                }
                if (MathTool.GetRandom(100) < rate)
                {
                    if (mark == 0)
                        mark = mon.GetSellMark();
                    CardProducts.Add(new DbCardProduct(index++, mon.Id, (int)mark));
                }
            }
            foreach (var weaponConfig in ConfigData.WeaponDict.Values)
            {
                if (weaponConfig.IsSpecial > 0)
                    continue;
                Weapon wpn = new Weapon(weaponConfig.Id);
                int rate = GetSellRate(weaponConfig.Id);
                CardProductMarkTypes mark = CardProductMarkTypes.Null;
                if (weaponConfig.IsNew > 0)
                {
                    rate = 100;
                    mark = CardProductMarkTypes.New;
                }
                if (MathTool.GetRandom(100) < rate)
                {
                    if (mark == 0)
                        mark = wpn.GetSellMark();
                    CardProducts.Add(new DbCardProduct(index++, wpn.Id, (int)mark));
                }
            }
            foreach (var spellConfig in ConfigData.SpellDict.Values)
            {
                if (spellConfig.IsSpecial > 0)
                    continue;
                Spell spl = new Spell(spellConfig.Id);
                int rate = GetSellRate(spellConfig.Id);
                CardProductMarkTypes mark = CardProductMarkTypes.Null;
                if (spellConfig.IsNew > 0)
                {
                    rate = 100;
                    mark = CardProductMarkTypes.New;
                }
                if (MathTool.GetRandom(100) < rate)
                {
                    if (mark == 0)
                        mark = spl.GetSellMark();
                    CardProducts.Add(new DbCardProduct(index++, spl.Id, (int)mark));
                }
            }
        }

        private int GetSellRate(int cid)
        {
            int qual = (int)CardConfigManager.GetCardConfig(cid).Quality;
            return 11 - qual * 2;
        }

        public DbTournamentData GetTournamentData(int tid)
        {
            if (Tournaments == null)
                Tournaments = new Dictionary<int, DbTournamentData>();
            if (!Tournaments.ContainsKey(tid))
            {
                DbTournamentData tourdata = new DbTournamentData(tid);
                Tournaments.Add(tid, tourdata);
            }
            return Tournaments[tid];
        }

        public void CheckAllTournament(int day)
        {
            foreach (var tournamentConfig in ConfigData.TournamentDict.Values)
            {
                if (tournamentConfig.ApplyDate == day)
                {
                    DbTournamentData tourdata = GetTournamentData(tournamentConfig.Id);
                    tourdata.Pids = PeopleBook.GetRandNPeople(tournamentConfig.PlayerCount, tournamentConfig.MinLevel, tournamentConfig.MaxLevel);
                    if (tourdata.Engage)
                        tourdata.Pids[MathTool.GetRandom(tournamentConfig.PlayerCount)] = -1; //player
                    tourdata.Results = new MatchResult[tournamentConfig.MaxLevel];
                }

                if (tournamentConfig.BeginDate <= day && tournamentConfig.EndDate >= day)
                {
                    foreach (int mid in TournamentBook.GetTournamentMatchIds(tournamentConfig.Id))
                    {
                        TournamentMatchConfig tournamentMatchConfig = ConfigData.GetTournamentMatchConfig(mid);
                        if (tournamentMatchConfig.Date == day && Tournaments[tournamentConfig.Id].Results[tournamentMatchConfig.Offset].Winner == 0)
                            Tournaments[tournamentConfig.Id].CheckMatch(tournamentMatchConfig.Offset, true);
                    }
                }
                if (tournamentConfig.EndDate == day)
                {
                    Tournaments[tournamentConfig.Id].Award();
                }
            }
        }

        public void UpdatePeopleRank(int personid, int mark)
        {
            if (!Ranks.ContainsKey(personid))
            {
                if (personid > 0)
                    Ranks.Add(personid, ConfigData.GetPeopleConfig(personid).Level * 10);
                else
                    Ranks.Add(personid, 0);
            }
            Ranks[personid] += mark;
        }

        public RankData[] GetAllPeopleRank()
        {
            foreach (PeopleConfig peopleConfig in ConfigData.PeopleDict.Values)
            {
                if (PeopleBook.IsPeople(peopleConfig.Id))
                {
                    if (!Ranks.ContainsKey(peopleConfig.Id))
                        Ranks.Add(peopleConfig.Id, peopleConfig.Level * 10);
                }
            }
            List<RankData> rks = new List<RankData>();
            foreach (var key in Ranks.Keys)
            {
                RankData data = new RankData {Id = key, Mark = Ranks[key]};
                rks.Add(data);
            }
            return rks.ToArray();
        }

        public DbMergeData[] GetAllMergeData()
        {
            int time = TimeTool.DateTimeToUnixTime(DateTime.Now);
            if (MergeMethods == null || UserProfile.InfoRecord.GetRecordById((int)MemPlayerRecordTypes.LastMergeTime) < time - GameConstants.MergeWeaponDura)
            {
                int[] ids = EquipBook.GetCanMergeId(UserProfile.InfoBasic.Level);
                List<int> newids = new List<int>(ids);
                ArraysUtils.RandomShuffle(newids);
                MergeMethods = new List<DbMergeData>();
                for (int i = 0; i < 8; i++)
                    MergeMethods.Add(CreateMergeMethod(newids[i]));
                UserProfile.InfoRecord.SetRecordById((int)MemPlayerRecordTypes.LastMergeTime, TimeManager.GetTimeOnNextInterval(UserProfile.InfoRecord.GetRecordById((int)MemPlayerRecordTypes.LastMergeTime), time, GameConstants.MergeWeaponDura));
            }

            return MergeMethods.ToArray();
        }

        public DbMergeData CreateMergeMethod(int mid)
        {
            EquipConfig equipConfig = ConfigData.GetEquipConfig(mid);
            DbMergeData mthds = new DbMergeData();
            mthds.Target = mid;
            {
                List<IntPair> mthd = new List<IntPair>();
                int icount = equipConfig.Quality == (int)EquipQualityTypes.Legend ? 4 : 3;
                int itrare = Math.Max(1, Math.Min(6, equipConfig.Quality * 2 - 1));//第一个素材品质和装备品质挂钩
                Dictionary<int, bool> existFormula = new Dictionary<int, bool>();
                for (int j = 0; j < icount; j++)
                {
                    IntPair pv = new IntPair();
                    if (j == 0)
                    {
                        pv.Type = HItemBook.GetRandRareItemId(itrare);
                        pv.Value = MathTool.GetRandom(3, 6);
                    }
                    else
                    {
                        int nrare = MathTool.GetRandom(Math.Max(0, itrare - 3), itrare) + 1;
                        pv.Type = HItemBook.GetRandRareItemId(nrare);
                        pv.Value = MathTool.GetRandom(2, 5);
                    }
                    if (existFormula.ContainsKey(pv.Type))
                    {
                        j--;//相当于做redo
                    }
                    else
                    {
                        existFormula.Add(pv.Type, true);
                        mthd.Add(pv);
                    }
                }

                mthds.Set(mthd);
            }
            return mthds;
        }

        public void AddPos(DbSceneSpecialPosData pos)
        {
            PosInfos.Add(pos);
        }
        public void UpdatePosEnable(int id, bool isEnable)
        {
            foreach (var posData in PosInfos)
            {
                if (posData.Id == id)
                {
                    posData.Disabled = !isEnable;
                    break;
                }
            }
        }
        public void UpdatePosInfo(int id, int info)
        {
            foreach (var posData in PosInfos)
            {
                if (posData.Id == id)
                {
                    posData.Info = info;
                    break;
                }
            }
        }
        public void UpdatePosMapSetting(int id, bool mapsetting)
        {
            foreach (var posData in PosInfos)
            {
                if (posData.Id == id)
                {
                    posData.MapSetting = mapsetting;
                    break;
                }
            }
        }
        public void UpdatePosFlag(int id, uint flag)
        {
            foreach (var posData in PosInfos)
            {
                if (posData.Id == id)
                {
                    posData.Flag = flag;
                    break;
                }
            }
        }

        public List<int> GetBlessShopData()
        {
            int time = TimeTool.DateTimeToUnixTime(DateTime.Now);
            if (BlessShopItems == null || UserProfile.InfoRecord.GetRecordById((int)MemPlayerRecordTypes.LastBlessShopTime) < time - GameConstants.BlessShopDura)
            {
                BlessShopItems = new List<int>();

                BlessShopItems.Clear();
                foreach (var blessData in ConfigData.BlessDict.Values)
                {
                    if (blessData.Type == (int)BlessTypes.Active)
                        BlessShopItems.Add(blessData.Id);
                }
                ArraysUtils.RandomShuffle(BlessShopItems);
                BlessShopItems = BlessShopItems.GetRange(0, 5);
                UserProfile.InfoRecord.SetRecordById((int)MemPlayerRecordTypes.LastBlessShopTime, TimeManager.GetTimeOnNextInterval(UserProfile.InfoRecord.GetRecordById((int)MemPlayerRecordTypes.LastBlessShopTime), time, GameConstants.BlessShopDura));
            }

            return BlessShopItems;
        }

        public List<int> GetDailyCardData()
        {
            int time = TimeTool.DateTimeToUnixTime(DateTime.Now);
            if (DailyCardData == null || TimeManager.IsDifferDay(UserProfile.InfoRecord.GetRecordById((int)MemPlayerRecordTypes.LastDailyCardTime) ,time))
            {
                DailyCardData = new List<int>();

                DailyCardData.Clear();
                DailyCardData.Add(HItemBook.GetRandRareItemIdWithGroup(HItemRandomGroups.Gather, MathTool.GetRandom(1, 6)));
                DailyCardData.Add(HItemBook.GetRandRareItemIdWithGroup(HItemRandomGroups.Fight, MathTool.GetRandom(1, 6)));
                DailyCardData.Add(HItemBook.GetRandRareItemIdWithGroup(HItemRandomGroups.Fight, MathTool.GetRandom(1, 5)));
                DailyCardData.Add(HItemBook.GetRandRareItemIdWithGroup(HItemRandomGroups.Shopping, 2));
                DailyCardData.Add(CardConfigManager.GetRandomCard(0, MathTool.GetRandom(1, 4)));
                UserProfile.InfoRecord.SetRecordById((int)MemPlayerRecordTypes.LastDailyCardTime, time);
            }

            return DailyCardData;
        }
    }
}
