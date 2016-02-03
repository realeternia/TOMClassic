using System.Drawing;
using TaleofMonsters.Core;

namespace TaleofMonsters.Controler.Battle.Data.MemFlow
{
    class FlowExpInfo : FlowWord
    {
        internal FlowExpInfo(int add, Point point, int offX, int offY) 
            : base("", point, -2, "BlueViolet", offX, offY, 1, 2, 20)
        {
            word = string.Format("+{0}", add);
        }

        internal override void Draw(Graphics g)
        {
            g.DrawImage(HSIcons.GetIconsByEName("oth5"), position.X, position.Y, 20, 20);

            g.DrawString(word, font, Brushes.Black, position.X + 23, position.Y + 1);
            Brush brush = new SolidBrush(color);
            g.DrawString(word, font, brush, position.X + 21, position.Y);
            brush.Dispose();
        } 
    }
}
