using System;
using System.Collections.Generic;

namespace TaleofMonsters.Controler.Battle.DataTent
{
    internal class BattleStatisticData
    {
        internal class BattleStatisticDataPlayer
        {
            public int MonsterAdd { get; set; }
            public int WeaponAdd { get; set; }
            public int SpellAdd { get; set; }
            public int Kill { get; set; }
            public int DamageTotal { get; set; }
        }

        public BattleStatisticData()
        {
            Left = new BattleStatisticDataPlayer();
            Right = new BattleStatisticDataPlayer();
            Items = new List<int>();
        }

        public DateTime StartTime { get; set; }
        public DateTime EndTime { get; set; }
        public int Round { get; set; }
        public BattleStatisticDataPlayer Left { get; private set; }
        public BattleStatisticDataPlayer Right { get; private set; }
        public int ExpRatePlus { get; set; }
        public int GoldRatePlus { get; set; }

        public List<int> Items { get; private set; }


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

        public void AddItemGet(int id)
        {
            Items.Add(id);
        }

        public BattleStatisticDataPlayer GetPlayer(bool isLeft)
        {
            return isLeft ? Left : Right;
        }
    }

}
