using System.Drawing;
using TaleofMonsters.Core;

namespace TaleofMonsters.Controler.Battle.Data.MemFlow
{
    class FlowErrInfo : FlowWord
    {
        internal FlowErrInfo(int err, Point point, int offX, int offY) 
            : base("", point, -2, "White", offX, offY, 0, 2, 30)
        {
            word = HSErrors.GetDescript(err);
            color = Color.Red;
        }

        public override void Draw(Graphics g)
        {
            g.DrawImage(HSIcons.GetIconsByEName("npc1"), Position.X, Position.Y, 20, 20);

            g.DrawString(word, font, Brushes.Black, Position.X + 23, Position.Y + 1);
            Brush brush = new SolidBrush(color);
            g.DrawString(word, font, brush, Position.X + 21, Position.Y);
            brush.Dispose();
        } 
    }
}
