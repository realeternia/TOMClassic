using System;
using TaleofMonsters.Core.Config;
using TaleofMonsters.Datas;
using TaleofMonsters.Datas.Decks;

namespace TaleofMonsters.Controler.Battle.Data.MemCard
{
    internal class ActiveCard
    {
        internal static ActiveCard NoneCard = new ActiveCard();

        public DeckCard Card { get; private set; }
        
        public int Mp { get; set; }
        public int Lp { get; set; }
        public int Pp { get; set; }

        public byte Level { get; set; }

        public int CostModify { get; set; }

        public ActiveCard()
        {
            Card = new DeckCard(0, 0, 0);
        }

        public ActiveCard(DeckCard card)
        {
            Card = card;
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
            return new ActiveCard(new DeckCard(Card.BaseId, Card.Level, Card.Exp));
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
