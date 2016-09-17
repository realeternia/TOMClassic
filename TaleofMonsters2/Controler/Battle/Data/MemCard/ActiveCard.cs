using System;
using System.Collections.Generic;
using TaleofMonsters.DataType.Cards;
using TaleofMonsters.DataType.Decks;
using ConfigDatas;
using TaleofMonsters.DataType;

namespace TaleofMonsters.Controler.Battle.Data.MemCard
{
    internal class ActiveCard
    {
        private int id;//唯一id

        private readonly DeckCard card;
        public int Mp {
            get
            {
                var lpCost = card.Lp == 0 ? 0 : Math.Max(0, card.Lp + LpCostChange);
                if (Lp2Mp && lpCost > 0)
                {
                    return lpCost;
                }
                return card.Mp==0?0: Math.Max(0, card.Mp + MpCostChange);
            }
        }
        public int Lp
        {
            get {
                if (Lp2Mp)
                {
                    return 0;
                }
                return card.Lp == 0 ? 0 : Math.Max(0,card.Lp + LpCostChange); }
        }
        public int Pp
        {
            get { return card.Pp == 0 ? 0 : Math.Max(0, card.Pp + PpCostChange); }
        }

        public IEnumerable<PlayerManaTypes> CostList
        {
            get
            {
                List<PlayerManaTypes> l = new List<PlayerManaTypes>();
                for (int i = 0; i < Lp; i++)
                {
                    l.Add(PlayerManaTypes.LeaderShip);
                }
                for (int i = 0; i < Mp; i++)
                {
                    l.Add(PlayerManaTypes.Mana);
                }
                for (int i = 0; i < Pp; i++)
                {
                    l.Add(PlayerManaTypes.Power);
                }
                return l;
            }
        }
        public int MpCostChange { get; set; }//可以被技能修改
        public int LpCostChange { get; set; }//可以被技能修改
        public int PpCostChange { get; set; }//可以被技能修改
        public bool Lp2Mp { get; set; } //可以被技能修改
        public byte Level { get; set; }

        public ActiveCard()
        {
            card = new DeckCard(0,0,0);
        }

        public ActiveCard(DeckCard card)
        {
            this.card = card;
            id = World.WorldInfoManager.GetCardFakeId();
            Level = card.Level;
        }

        public ActiveCard(int baseid, byte level, ushort exp)
        {
            id = World.WorldInfoManager.GetCardFakeId();
            card = new DeckCard(baseid, level, exp);
            Level = card.Level;
        }

        public int Id //唯一的id
        {
            get { return id; }
        }

        public int CardId //卡片配置的id
        {
            get { return Card.BaseId; }
        }

        public CardTypes CardType
        {
            get
            {
                return CardAssistant.GetCardType(Card.BaseId);
            }
        }

        public DeckCard Card
        {
            get { return card; }
        }

        public ActiveCard GetCopy()
        {
            DeckCard dc = new DeckCard(Card.BaseId, Card.Level, Card.Exp);
            return new ActiveCard(dc);
        }

        public void ChangeLevel(byte level)
        {
            Level = level;
            card.Level = level;
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
            {
                return true;
            }
            return false;
        }

        public override string ToString()
        {
            return string.Format("id={0} cid={1}", Id, CardId);
        }
    }   
}
