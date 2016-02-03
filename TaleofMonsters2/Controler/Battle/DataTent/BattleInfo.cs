using System;
using System.Collections.Generic;
using TaleofMonsters.Core;

namespace TaleofMonsters.Controler.Battle.DataTent
{
    internal class BattleInfo
    {
        public BattleInfo()
        {
            Left = new BattleInfoPlayer();
            Right = new BattleInfoPlayer();
            Items = new List<int>();
            CardRatePlus = new List<IntPair>();
        }

        public DateTime StartTime { get; set; }
        public DateTime EndTime { get; set; }
        public int Round { get; set; }
        public BattleInfoPlayer Left { get; private set; }
        public BattleInfoPlayer Right { get; private set; }
        public int ExpRatePlus { get; set; }
        public int GoldRatePlus { get; set; }

        public List<int> Items { get; private set; }

        public List<IntPair> CardRatePlus { get; private set; }


        public int FastWin
        {
            get { return (!PlayerWin || Round == 0 || Round > 50) ? 0 : 1; }
        }

        public int ZeroDie
        {
            get { return (!PlayerWin || Round == 0 || Right.Kill > 0) ? 0 : 1; }
        }

        public int OnlyMagic
        {
            get { return (!PlayerWin || Round == 0 || Left.MonsterAdd > 0 || Left.WeaponAdd > 0) ? 0 : 1; }
        }

        public int OnlySummon
        {
            get { return (!PlayerWin || Round == 0 || Left.MonsterAdd > 0 || Left.SpellAdd > 0) ? 0 : 1; }
        }

        public int AlmostLost
        {
            get { return (!PlayerWin) ? 0 : 1; }
        }

        public bool PlayerWin { get; set; }

        public void AddCardRate(int cid, int value)
        {
            int val = 0;
            for (int i = 0; i < CardRatePlus.Count; i++)
            {
                if (CardRatePlus[i].Type == cid)
                {
                    val = CardRatePlus[i].Value;
                    CardRatePlus.RemoveAt(i);
                    break;
                }
            }
            val += value;
            if (val != 0)
            {
                IntPair pair = new IntPair();
                pair.Type = cid;
                pair.Value = Math.Min(val, 30);
                CardRatePlus.Add(pair);
            }
        }

        public void AddItemGet(int id)
        {
            Items.Add(id);
        }

        public BattleInfoPlayer GetPlayer(bool isLeft)
        {
            return isLeft ? Left : Right;
        }
    }

    internal class BattleInfoPlayer
    {
        public int MonsterAdd { get; set; }
        public int WeaponAdd { get; set; }
        public int SpellAdd { get; set; }
        public int HeroAdd { get; set; }
        public int Kill { get; set; }
        public int HeroKill { get; set; }
    }
}
