using TaleofMonsters.Config;
using TaleofMonsters.DataType.Cards;
using TaleofMonsters.DataType.User.Db;

namespace TaleofMonsters.DataType.Decks
{
    public class DeckCard
    {
        public int BaseId;
        public byte Level;
        public ushort Exp;

        public DeckCard(DbDeckCard dc)
        {
            BaseId = dc.BaseId;
            Level = dc.Level;
            Exp = dc.Exp;
        }

        public DeckCard(int baseId, byte level, ushort exp)
        {
            BaseId = baseId;
            Level = level;
            Exp = exp;
        }

        public int Mp
        {
            get
            {
                if (CardAssistant.GetCardType(BaseId) != CardTypes.Spell)
                {
                    return 0;
                }
                return CardConfigManager.GetCardConfig(BaseId).Cost;
            }
        }

        public int Lp
        {
            get
            {
                if (CardAssistant.GetCardType(BaseId) != CardTypes.Monster)
                {
                    return 0;
                }
                return CardConfigManager.GetCardConfig(BaseId).Cost;
            }
        }

        public int Pp
        {
            get
            {
                if (CardAssistant.GetCardType(BaseId) != CardTypes.Weapon)
                {
                    return 0;
                }
                return CardConfigManager.GetCardConfig(BaseId).Cost;
            }
        }

        public int Star
        {
            get
            {
                return CardConfigManager.GetCardConfig(BaseId).Star;
            }
        }

        public override string ToString()
        {
            return string.Format("{0}", BaseId);
        }
    }

}
