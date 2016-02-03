using System.Drawing;
using TaleofMonsters.Core;

namespace TaleofMonsters.Controler.Battle.Data.MemFlow
{
    class FlowErrInfo : FlowWord
    {
        internal FlowErrInfo(int err, Point point, int offX, int offY) 
            : base("", point, -2, "White", offX, offY, 0, 2, 30)
        {
            word = HSErrorTypes.GetDescript(err);
            color = Color.Red;
        }

        internal override void Draw(Graphics g)
        {
            g.DrawImage(HSIcons.GetIconsByEName("npc1"), position.X, position.Y, 20, 20);

            g.DrawString(word, font, Brushes.Black, position.X + 23, position.Y + 1);
            Brush brush = new SolidBrush(color);
            g.DrawString(word, font, brush, position.X + 21, position.Y);
            brush.Dispose();
        } 
    }
}
