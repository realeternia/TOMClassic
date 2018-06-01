using System.Drawing;
using TaleofMonsters.Core;

namespace TaleofMonsters.Controler.Battle.Data.MemFlow
{
    class FlowLukInfo : FlowWord
    {
        internal FlowLukInfo(int add, Point point, int offX, int offY) 
            : base("", point, -2, "Cyan", offX, offY, 1, 2, 20)
        {
            word = string.Format("+{0}", add);
        }

        public override void Draw(Graphics g)
        {
            g.DrawImage(HSIcons.GetIconsByEName("hatt6"), Position.X, Position.Y, 20, 20);

            g.DrawString(word, font, Brushes.Black, Position.X + 23, Position.Y + 1);
            Brush brush = new SolidBrush(color);
            g.DrawString(word, font, brush, Position.X + 21, Position.Y);
            brush.Dispose();
        } 
    }
}
