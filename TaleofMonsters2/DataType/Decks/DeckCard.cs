using System;
using TaleofMonsters.Config;
using TaleofMonsters.Core;
using TaleofMonsters.DataType.Cards;
using TaleofMonsters.DataType.Others;

namespace TaleofMonsters.DataType.Decks
{
    public class DeckCard
    {
        [FieldIndex(Index = 2)]
        public int BaseId ;
        [FieldIndex(Index = 3)]
        public byte Level ;
        [FieldIndex(Index = 4)]
        public ushort Exp ;

        public DeckCard()
        {
        }

        public DeckCard(int baseId, byte level, ushort exp)
        {
            BaseId = baseId;
            Level = level;
            Exp = exp;
        }

        public void AddExp(int addon)
        {
            if (Level>= ExpTree.MaxLevel)
            {
                return;
            }

            Exp = (ushort)(Exp + addon);
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
