using System.Drawing;
using TaleofMonsters.Core.Interface;

namespace TaleofMonsters.Forms.Items.Regions.Decorators
{
    internal class RegionTextDecorator : IRegionDecorator
    {
        private int xOff;
        private int yOff;
        private int size;
        private string text;
        private bool bold;
        private Color foreColor;
        private SubVirtualRegion parent;

        public RegionTextDecorator(SubVirtualRegion parent, int xOff, int yOff, int size, Color color, bool bold)
        {
            this.xOff = xOff;
            this.yOff = yOff;
            this.size = size;
            foreColor = color;
            this.parent = parent;
            this.bold = bold;
        }

        public RegionTextDecorator(SubVirtualRegion parent, int xOff, int yOff, int size, Color color)
            : this(parent, xOff, yOff, size, color, true)
        {
        }

        public RegionTextDecorator(SubVirtualRegion parent, int x, int y, int size)
            : this(parent, x, y, size, Color.White, true)
        {
        }

        public void SetState(object info)
        {
            text = info.ToString();
        }

        public void Draw(Graphics g)
        {
            if (parent.GetKeyValue() == 0)
            {
                return;
            }

            Font fontsong = new Font("ËÎÌו", size*1.33f, bold ? FontStyle.Bold : FontStyle.Regular, GraphicsUnit.Pixel);
            g.DrawString(text, fontsong, foreColor == Color.Black ? Brushes.White : Brushes.Black, parent.x+ xOff,parent.y+ yOff);
            using (Brush brush = new SolidBrush(foreColor))
            {
                g.DrawString(text, fontsong, brush, parent.x + xOff - 1, parent.y + yOff - 1);
            }
            fontsong.Dispose();
        }
    }
}
