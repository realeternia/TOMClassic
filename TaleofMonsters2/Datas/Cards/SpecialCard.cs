using System.Drawing;
using TaleofMonsters.Core.Loader;
using TaleofMonsters.Datas.Decks;

namespace TaleofMonsters.Datas.Cards
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

        public override int JobId
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

        public override void SetData(DeckCard card1)
        {
        }

        public override Image GetPreview(CardPreviewType type, uint[] parms)
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
