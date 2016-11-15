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

        public RegionTextDecorator(int xOff, int yOff, int size, Color color, bool bold)
        {
            this.xOff = xOff;
            this.yOff = yOff;
            this.size = size;
            foreColor = color;
            this.bold = bold;
        }

        public RegionTextDecorator( int xOff, int yOff, int size, Color color)
            : this(xOff, yOff, size, color, true)
        {
        }

        public RegionTextDecorator(int x, int y, int size)
            : this(x, y, size, Color.White, true)
        {
        }

        public void SetState(object info)
        {
            text = info.ToString();
        }

        public void Draw(Graphics g, int x, int y, int width, int height)
        {
            Font fontsong = new Font("ËÎÌו", size*1.33f, bold ? FontStyle.Bold : FontStyle.Regular, GraphicsUnit.Pixel);
            g.DrawString(text, fontsong, foreColor == Color.Black ? Brushes.White : Brushes.Black, x+ xOff,y+ yOff);
            using (Brush brush = new SolidBrush(foreColor))
            {
                g.DrawString(text, fontsong, brush, x + xOff - 1, y + yOff - 1);
            }
            fontsong.Dispose();
        }
    }
}
