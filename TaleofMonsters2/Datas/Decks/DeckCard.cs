using TaleofMonsters.Core.Config;
using TaleofMonsters.Datas.User.Db;

namespace TaleofMonsters.Datas.Decks
{
    public class DeckCard : IMemCardData
    {
        public int CardId { get; set; }
        public byte Level { get; set; }
        public ushort Exp { get; set; }

        public DeckCard(DbDeckCard dc)
        {
            CardId = dc.BaseId;
            Level = dc.Level;
            Exp = dc.Exp;
        }

        public DeckCard(int cardId, byte level, ushort exp)
        {
            CardId = cardId;
            Level = level;
            Exp = exp;
        }

        public int Star
        {
            get { return CardConfigManager.GetCardConfig(CardId).Star; }
        }

        public override string ToString()
        {
            return string.Format("{0}", CardId);
        }
    }

}
