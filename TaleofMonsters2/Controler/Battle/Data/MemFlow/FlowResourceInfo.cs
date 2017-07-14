using System.Drawing;
using TaleofMonsters.Core;

namespace TaleofMonsters.Controler.Battle.Data.MemFlow
{
    class FlowResourceInfo : FlowWord
    {
        private int resource;
        internal FlowResourceInfo(int res, int add, Point point, int offX, int offY) 
            : base("", point, -2, "Gold", offX, offY, 1, 3, 30)
        {
            resource = res;
            word = string.Format("+{0}", add);
        }

        public override void Draw(Graphics g)
        {
            g.DrawImage(HSIcons.GetIconsByEName("res" + resource), Position.X, Position.Y, 20, 20);

            g.DrawString(word, font, Brushes.Black, Position.X + 23, Position.Y + 1);
            Brush brush = new SolidBrush(color);
            g.DrawString(word, font, brush, Position.X + 21, Position.Y);
            brush.Dispose();
        } 
    }
}
