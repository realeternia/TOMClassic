using System.Drawing;
using TaleofMonsters.Core.Interface;

namespace TaleofMonsters.Forms.Items.Regions
{
    internal class ImageRegion : SubVirtualRegion
    {
        protected ImageRegionCellType type;
        private Image image;

        public ImageRegion(int id, int x, int y, int width, int height, ImageRegionCellType t, Image img)
            : base(id, x, y, width, height)
        {
            type = t;
            image = img;
            Scale = 1;
        }

        public override void Draw(Graphics g)
        {
            if (image != null)
            {
                if (Scale == 1)
                {
                    g.DrawImage(image, X, Y, Width, Height);
                }
                else
                {
                    int realWidth = (int)(Width*Scale);
                    int realHeight = (int)(Height * Scale);
                    g.DrawImage(image, X + (Width- realWidth)/2, Y + (Height- realHeight)/2, realWidth, realHeight);
                }
            }

            foreach (IRegionDecorator decorator in decorators)
            {
                decorator.Draw(g, X, Y, Width, Height);
            }
        }
        public ImageRegionCellType GetVType()
        {
            return type;
        }

        public float Scale { get; set; } //ÖÐÐÄÍ¼Æ¬Ëõ·Å
    }

    internal enum ImageRegionCellType
    {
        None,
        Gold,
        Exp,
        Food,
        Health,
        Mental
    }
}
