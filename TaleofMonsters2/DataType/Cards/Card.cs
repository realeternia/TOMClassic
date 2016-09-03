using System;
using System.Drawing;
using TaleofMonsters.Controler.Battle.Data.MemCard;

namespace TaleofMonsters.DataType.Cards
{
    internal abstract class Card
    {
        public abstract int CardId { get; }
        public abstract int Star { get; }
        public abstract int Cost { get; }
        public abstract int Type { get; }
        public abstract int JobId { get; }
        public abstract string Name { get; }
        public abstract Image GetCardImage(int width, int height);
        public abstract void DrawOnCardDetail(Graphics g, int offX, int offY);
        public abstract void DrawOnStateBar(Graphics g);
        public abstract Image GetPreview(CardPreviewType type, int[] parms);
        public abstract CardTypes GetCardType();
        public abstract void SetData(Decks.DeckCard dc);
        public abstract void SetData(ActiveCard dc);
    }

}
