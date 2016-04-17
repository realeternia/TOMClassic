using TaleofMonsters.Controler.Battle.Data.MemCard;
using TaleofMonsters.Controler.Loader;
using System.Drawing;
using TaleofMonsters.DataType.Decks;

namespace TaleofMonsters.DataType.Cards
{
    sealed class SpecialCard : Card
    {
        private string cardName;
        private Image cardImg;

        public SpecialCard(string name)
        {
            cardName = name;
            cardImg = PicLoader.Read("System", cardName);
        }

        public override int CardId
        {
            get { return 0; }
        }

        public override int Star
        {
            get { return 0; }
        }

        public override int Type
        {
            get { return 99; }
        }

        public override int Cost
        {
            get { return 0; }
        }

        public override string Name
        {
            get { return ""; }
        }

        public override Image GetCardImage(int width, int height)
        {
            return cardImg;
        }

        public override void DrawOnCardDetail(Graphics g, int offX, int offY)
        {
        }

        public override CardTypes GetCardType()
        {
            return CardTypes.Null;
        }

        public override void SetData(ActiveCard card)
        {            
        }

        public override void SetData(DeckCard card1)
        {
        }

        public override void DrawOnStateBar(Graphics g)
        {
        }

        public override Image GetPreview(CardPreviewType type, int[] parms)
        {
            return null;
        }
    }

    static class SpecialCards 
	{
        static readonly SpecialCard nullCard = new SpecialCard("CardBack.JPG");

        public static SpecialCard NullCard
        {
            get { return nullCard; }
        }
    }
}
