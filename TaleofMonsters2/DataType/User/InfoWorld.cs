using System;
using System.Collections.Generic;
using ConfigDatas;
using NarlonLib.Core;
using NarlonLib.Math;
using TaleofMonsters.Config;
using TaleofMonsters.Controler.World;
using TaleofMonsters.Core;
using TaleofMonsters.DataType.Cards;
using TaleofMonsters.DataType.Cards.Monsters;
using TaleofMonsters.DataType.Cards.Spells;
using TaleofMonsters.DataType.Cards.Weapons;
using TaleofMonsters.DataType.Equips;
using TaleofMonsters.DataType.Peoples;
using TaleofMonsters.DataType.Items;
using TaleofMonsters.DataType.Shops;
using TaleofMonsters.DataType.Tournaments;
using TaleofMonsters.DataType.User.Mem;
using TaleofMonsters.Forms.TourGame;

namespace TaleofMonsters.DataType.User
{
    public class InfoWorld
    {
        [FieldIndex(Index = 1)]
        public List<MemChangeCardData> Cards ;
        [FieldIndex(Index = 2)]
        public List<MemNpcPieceData> Pieces;
        [FieldIndex(Index = 3)]
        public List<CardProduct> CardProducts;
        [FieldIndex(Index = 4)]
        public Dictionary<int, MemTournamentData> Tournaments;
        [FieldIndex(Index = 5)]
        public Dictionary<int, int> Ranks;
        [FieldIndex(Index = 6)]
        public List<MemMergeData> MergeMethods;
        [FieldIndex(Index = 7)]
        public List<MemSceneSpecialPosData> PosInfos;


        public InfoWorld()
        {
            Cards = new List<MemChangeCardData>();
            Pieces = new List<MemNpcPieceData>();
            CardProducts = new List<CardProduct>();
            Tournaments = new Dictionary<int, MemTournamentData>();
            Ranks = new Dictionary<int, int>();
            MergeMethods = new List<MemMergeData>();
            PosInfos = new List<MemSceneSpecialPosData>();
        }

        public List<MemChangeCardData> GetChangeCardData()
        {
            int time = TimeTool.DateTimeToUnixTime(DateTime.Now);
            if (Cards != null && UserProfile.InfoRecord.GetRecordById((int)MemPlayerRecordTypes.LastCardChangeTime) < time - GameConstants.ChangeCardDura)
            {
                Cards.Clear();
                for (int i = 0; i < 5; i++)
                {
                    Cards.Add(CreateMethod(i));
                }
                UserProfile.InfoRecord.SetRecordById((int)MemPlayerRecordTypes.LastCardChangeTime, TimeManager.GetTimeOnNextInterval(UserProfile.InfoRecord.GetRecordById((int)MemPlayerRecordTypes.LastCardChangeTime), time, GameConstants.ChangeCardDura));
            }
            return Cards;
        }

        public void AddChangeCardData()
        {
            if (Cards.Count < 8)
            {
                Cards.Add(CreateMethod(Cards.Count));
            }
        }

        public MemChangeCardData GetChangeCardData(int index)
        {
            if (Cards.Count > index)
            {
                return Cards[index];
            }
            return new MemChangeCardData();
        }

        public void RemoveChangeCardData(int index)
        {            
            if (Cards.Count > index)
            {
                Cards[index].Used = true;
            }
        }

        public void RefreshAllChangeCardData()
        {
            int count = Cards.Count;
            Cards.Clear();
            for (int i = 0; i < count; i++)
            {
                Cards.Add(CreateMethod(i));
            }
        }

        private MemChangeCardData CreateMethod(int index)
        {
            MemChangeCardData chg = new MemChangeCardData();
            int level = MathTool.GetRandom(Math.Max(index/2, 1), index/2 + 3);
            chg.Id1 = MonsterBook.GetRandStarMid(level);
            while (true)
            {
                chg.Id2 = MonsterBook.GetRandStarMid(level);
                if (chg.Id2 != chg.Id1)
                {
                    break;
                }
            }
            chg.Type1 = 1;
            chg.Type2 = 1;

            return chg;
        }

        public List<MemNpcPieceData> GetPieceData()
        {
            int time = TimeTool.DateTimeToUnixTime(DateTime.Now);
            if (Cards != null && UserProfile.InfoRecord.GetRecordById((int)MemPlayerRecordTypes.LastNpcPieceTime) < time - GameConstants.NpcPieceDura)
            {
                Pieces.Clear();
                for (int i = 0; i < 5; i++)
                {
                    Pieces.Add(CreatePieceMethod(i));
                }
                UserProfile.InfoRecord.SetRecordById((int)MemPlayerRecordTypes.LastNpcPieceTime, TimeManager.GetTimeOnNextInterval(UserProfile.InfoRecord.GetRecordById((int)MemPlayerRecordTypes.LastNpcPieceTime), time, GameConstants.NpcPieceDura));
            }
            return Pieces;
        }

        public void AddPieceData()
        {
            if (Pieces.Count < 8)
            {
                Pieces.Add(CreatePieceMethod(Cards.Count));
            }
        }

        public MemNpcPieceData GetPieceData(int index)
        {
            if (Pieces.Count > index)
            {
                return Pieces[index];
            }
            return new MemNpcPieceData();
        }

        public void RemovePieceData(int index)
        {
            if (Pieces.Count > index)
            {
                Pieces[index].Used = true;
            }
        }

        public void RefreshAllPieceData()
        {
            int count = Pieces.Count;
            Pieces.Clear();
            for (int i = 0; i < count; i++)
            {
                Pieces.Add(CreatePieceMethod(i));
            }
        }

        public void DoubleAllPieceData()
        {
            foreach (var memNpcPieceData in Pieces)
            {
                memNpcPieceData.Count *= 2;
            }
        }

        private MemNpcPieceData CreatePieceMethod(int index)
        {
            MemNpcPieceData piece = new MemNpcPieceData();
            int rare = MathTool.GetRandom(Math.Max(index / 2, 1), index / 2 + 3);
            piece.Id = HItemBook.GetRandRareMid(rare);
            piece.Count = MathTool.GetRandom((8 - rare) / 2, 8 - rare);

            return piece;
        }

        internal CardProduct[] GetCardProductsByType(CardTypes type)
        {
            int time = TimeTool.DateTimeToUnixTime(DateTime.Now);
            if (CardProducts == null || UserProfile.InfoRecord.GetRecordById((int)MemPlayerRecordTypes.LastCardShopTime) < time - GameConstants.CardShopDura)
            {
                CardProducts = new List<CardProduct>();
                ReinstallCardProducts();
                UserProfile.InfoRecord.SetRecordById((int)MemPlayerRecordTypes.LastCardShopTime, TimeManager.GetTimeOnNextInterval(UserProfile.InfoRecord.GetRecordById((int)MemPlayerRecordTypes.LastCardShopTime), time, GameConstants.CardShopDura));
            }

            List<CardProduct> pros = new List<CardProduct>();
            foreach (CardProduct cardProduct in CardProducts)
            {
                if (CardAssistant.GetCardType(cardProduct.Cid) == type)
                    pros.Add(cardProduct);
            }

            return pros.ToArray();
        }

        public void RemoveCardProduct(int id)
        {
            foreach (CardProduct cardProduct in CardProducts)
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
                    {
                        mark = mon.GetSellMark();
                    }
                    CardProducts.Add(new CardProduct(index++, mon.Id, (int)mark));
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
                    {
                        mark = wpn.GetSellMark();
                    }
                    CardProducts.Add(new CardProduct(index++, wpn.Id, (int)mark));
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
                    {
                        mark = spl.GetSellMark();
                    }
                    CardProducts.Add(new CardProduct(index++, spl.Id, (int)mark));
                }
            }
        }

        private int GetSellRate(int cid)
        {
            int qual = CardConfigManager.GetCardConfig(cid).Quality;
            return 11 - qual * 2;
        }

        public MemTournamentData GetTournamentData(int tid)
        {
            if (Tournaments == null)
            {
                Tournaments = new Dictionary<int, MemTournamentData>();
            }
            if (!Tournaments.ContainsKey(tid))
            {
                MemTournamentData tourdata = new MemTournamentData(tid);
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
                    MemTournamentData tourdata = GetTournamentData(tournamentConfig.Id);
                    tourdata.Pids = PeopleBook.GetRandNPeople(tournamentConfig.PlayerCount, tournamentConfig.MinLevel, tournamentConfig.MaxLevel);
                    if (tourdata.Engage)
                    {
                        tourdata.Pids[MathTool.GetRandom(tournamentConfig.PlayerCount)] = -1; //player
                    }
                    tourdata.Results = new MatchResult[tournamentConfig.MaxLevel];
                }

                if (tournamentConfig.BeginDate <= day && tournamentConfig.EndDate >= day)
                {
                    foreach (int mid in TournamentBook.GetTournamentMatchIds(tournamentConfig.Id))
                    {
                        TournamentMatchConfig tournamentMatchConfig = ConfigData.GetTournamentMatchConfig(mid);
                        if (tournamentMatchConfig.Date == day && Tournaments[tournamentConfig.Id].Results[tournamentMatchConfig.Offset].Winner == 0)
                        {
                            Tournaments[tournamentConfig.Id].CheckMatch(tournamentMatchConfig.Offset, true);
                        }
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
                {
                    Ranks.Add(personid, ConfigData.GetPeopleConfig(personid).Level * 10);
                }
                else
                {
                    Ranks.Add(personid, 0);
                }
            }
            Ranks[personid] += mark;
        }

        public MemRankData[] GetAllPeopleRank()
        {
            foreach (PeopleConfig peopleConfig in ConfigData.PeopleDict.Values)
            {
                if (PeopleBook.IsPeople(peopleConfig.Id))
                {
                    if (!Ranks.ContainsKey(peopleConfig.Id))
                    {
                        Ranks.Add(peopleConfig.Id, peopleConfig.Level * 10);
                    }
                }
            }
            List<MemRankData> rks = new List<MemRankData>();
            foreach (var key in Ranks.Keys)
            {
                MemRankData data = new MemRankData {Id = key, Mark = Ranks[key]};
                rks.Add(data);
            }
            return rks.ToArray();
        }

        public MemMergeData[] GetAllMergeData()
        {
            int time = TimeTool.DateTimeToUnixTime(DateTime.Now);
            if (MergeMethods == null || UserProfile.InfoRecord.GetRecordById((int)MemPlayerRecordTypes.LastMergeTime) < time - GameConstants.MergeWeaponDura)
            {
                int[] ids = EquipBook.GetCanMergeId(UserProfile.InfoBasic.Level);
                List<int> newids = RandomShuffle.Process(ids);
                MergeMethods = new List<MemMergeData>();
                for (int i = 0; i < 8; i++)
                {
                    MergeMethods.Add(CreateMergeMethod(newids[i]));
                }
                UserProfile.InfoRecord.SetRecordById((int)MemPlayerRecordTypes.LastMergeTime, TimeManager.GetTimeOnNextInterval(UserProfile.InfoRecord.GetRecordById((int)MemPlayerRecordTypes.LastMergeTime), time, GameConstants.MergeWeaponDura));
            }

            return MergeMethods.ToArray();
        }

        public MemMergeData CreateMergeMethod(int mid)
        {
            EquipConfig equipConfig = ConfigData.GetEquipConfig(mid);
            MemMergeData mthds = new MemMergeData();
            mthds.Target = mid;
            int mcount = GetMethodCount(equipConfig.Quality);
            for (int i = 0; i < mcount; i++)
            {
                List<IntPair> mthd = new List<IntPair>();
                int icount = GetItemCount(equipConfig.Level);
                int itrare = MathTool.GetRandom(Math.Max(1, equipConfig.Quality*2-1), equipConfig.Quality*2+1);//第一个素材品质和装备品质挂钩
                Dictionary<int, bool> existFormula = new Dictionary<int, bool>();
                for (int j = 0; j < icount; j++)
                {
                    IntPair pv = new IntPair();
                    if (j == 0)
                    {
                        pv.Type = HItemBook.GetRandRareMid(itrare);
                        pv.Value = GetItemCountByEquipLevel(equipConfig.Level, itrare) + 1;
                    }
                    else
                    {
                        int nrare = MathTool.GetRandom(Math.Max(1, itrare - 3), itrare);
                        pv.Type = HItemBook.GetRandRareMid(nrare);
                        pv.Value = Math.Max(1, GetItemCountByEquipLevel(equipConfig.Level, nrare));
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

                mthds.Add(mthd);
            }
            return mthds;
        }

        private int GetMethodCount(int quality)
        {
            int rt = 0;
            switch (quality)
            {
                case EquipQualityTypes.Common: rt = 1; break;
                case EquipQualityTypes.Good: rt = 1; break;
                case EquipQualityTypes.Excel: rt = 2; break;
                case EquipQualityTypes.Epic: rt = 2; break;
                case EquipQualityTypes.Legend: rt = 2; break;
            }
            return rt;
        }

        private int GetItemCount(int elevel)
        {
            if (elevel < 10)
            {
                return 2;
            }
            if (elevel < 18)
            {
                return 3;
            }
            return 4;
        }


        private int GetItemCountByEquipLevel(int elevel, int rare)
        {
            int[] levelp = { 1, 2, 3, 4, 6, 10, 14, 20 };
            return elevel*MathTool.GetRandom(8, 12)/10/levelp[rare];
        }
    }
}
