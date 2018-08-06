using System.Drawing;
using ControlPlus;
using TaleofMonsters.Datas.Decks;

namespace TaleofMonsters.Datas.Cards
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
        public abstract Image GetPreview(TipImage.TipOwnerDrawDelegate ownerDraw);
        public abstract CardTypes GetCardType();
        public abstract void SetData(IMemCardData dc);
    }

}
