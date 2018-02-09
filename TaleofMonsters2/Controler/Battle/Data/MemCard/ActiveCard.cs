using System;
using System.Collections.Generic;
using TaleofMonsters.DataType.Decks;
using ConfigDatas;
using TaleofMonsters.Config;
using TaleofMonsters.DataType;

namespace TaleofMonsters.Controler.Battle.Data.MemCard
{
    internal class ActiveCard
    {
        public DeckCard Card { get; private set; }

        public int Mp
        {
            get
            {
                var lpCost = Card.Lp == 0 ? 0 : Math.Max(0, Card.Lp + LpCostChange + CostModify);
                if (Lp2Mp && lpCost > 0)
                    return lpCost;
                return Card.Mp==0?0: Math.Max(0, Card.Mp + MpCostChange+ CostModify);
            }
        }
        public int Lp
        {
            get
            {
                if (Lp2Mp) return 0;
                return Card.Lp == 0 ? 0 : Math.Max(0,Card.Lp + LpCostChange+ CostModify);
            }
        }
        public int Pp
        {
            get { return Card.Pp == 0 ? 0 : Math.Max(0, Card.Pp + PpCostChange+ CostModify); }
        }

        public IEnumerable<PlayerManaTypes> CostList
        {
            get
            {
                List<PlayerManaTypes> l = new List<PlayerManaTypes>();
                for (int i = 0; i < Lp; i++)
                    l.Add(PlayerManaTypes.Lp);
                for (int i = 0; i < Mp; i++)
                    l.Add(PlayerManaTypes.Mp);
                for (int i = 0; i < Pp; i++)
                    l.Add(PlayerManaTypes.Pp);
                return l;
            }
        }
        public int MpCostChange { get; set; }//可以被技能修改
        public int LpCostChange { get; set; }//可以被技能修改
        public int PpCostChange { get; set; }//可以被技能修改
        public bool Lp2Mp { get; set; } //可以被技能修改
        public byte Level { get; set; }

        public int CostModify { get; set; }
        public bool IsHeroSkill { get; set; }

        public ActiveCard()
        {
            Card = new DeckCard(0,0,0);
        }

        public ActiveCard(DeckCard card)
        {
            this.Card = card;
            Level = card.Level;
        }

        public ActiveCard(int baseid, byte level, ushort exp)
        {
            Card = new DeckCard(baseid, level, exp);
            Level = Card.Level;
        }
        

        public int CardId //卡片配置的id
        {
            get { return Card.BaseId; }
        }

        public CardTypes CardType
        {
            get { return ConfigIdManager.GetCardType(Card.BaseId); }
        }
        
        public ActiveCard GetCopy()
        {
            var ac = new ActiveCard(new DeckCard(Card.BaseId, Card.Level, Card.Exp));
            ac.MpCostChange = MpCostChange;
            ac.LpCostChange = LpCostChange;
            ac.PpCostChange = PpCostChange;
            ac.Lp2Mp = Lp2Mp;
            return ac;
        }

        public void ChangeLevel(byte level)
        {
            Level = level;
            Card.Level = level;
        }

        public static bool operator ==(ActiveCard rec1, ActiveCard rec2)
        {
            return Equals(rec1, rec2);
        }

        public static bool operator !=(ActiveCard rec1, ActiveCard rec2)
        {
            return !Equals(rec1, rec2);
        }

        public override int GetHashCode()
        {
            return Card.GetHashCode();
        }

        public override bool Equals(object obj)
        {
            if (obj == null)
                return false;
            if (GetType() != obj.GetType())
                return false;
            ActiveCard rec = (ActiveCard) obj;
            if (rec.Card == Card)
                return true;
            if (rec.CardId != CardId) 
                return false;
            if (rec.Level != Level) //todo 还有其他可能性，暂时只有这个
                return false;
            return true;
        }

        public override string ToString()
        {
            return string.Format("id={0}", CardId);
        }
    }   
}
