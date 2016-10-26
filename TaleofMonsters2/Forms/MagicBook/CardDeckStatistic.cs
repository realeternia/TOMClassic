using System.Drawing;
using System.Windows.Forms;

namespace TaleofMonsters.Forms.MagicBook
{
    internal class CardDeckStatistic
    {
        public int X { get; set; }
        public int Y { get; set; }
        public int Width { get; set; }
        public int Height { get; set; }
        private UserControl parent;

        public CardDeckStatistic(UserControl control, int x, int y, int height)
        {
            parent = control;
            X = x;
            Y = y;
            Width = 200;
            Height = height;
        }
        public void Draw(Graphics g)
        {
            g.FillRectangle(Brushes.Thistle, X, Y, Width, Height);
        }
    }
}
