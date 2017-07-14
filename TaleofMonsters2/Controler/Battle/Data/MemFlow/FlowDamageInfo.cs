using System.Drawing;
using ConfigDatas;
using TaleofMonsters.Core;

namespace TaleofMonsters.Controler.Battle.Data.MemFlow
{
    internal class FlowDamageInfo : FlowWord
    {
        private HitDamage damage;
        public FlowDamageInfo(HitDamage dam, Point point) 
            : base("", point, 3, "Coral", 0, -10, 0, 2, 30)
        {
            damage = dam;
            word = damage.Value.ToString();
            if (damage.IsCrt)
            {
                size += 6;//暴击字体放大，移动速度减慢
                font.Dispose();
                font = new Font("微软雅黑", this.size * 1.33f, FontStyle.Bold, GraphicsUnit.Pixel);
                speedY --;//-4
                word = damage.Value.ToString() + "!";
            }
        }

        public override void Draw(Graphics g)
        {
            int xOff = Position.X-10;

            if (damage.Dtype == DamageTypes.Magic)
            {
                xOff -= 16;
                g.DrawImage(HSIcons.GetIconsByEName("atr"+damage.Element), xOff, Position.Y + 8, 18, 18);
                xOff += 16;
                color = PaintTool.GetColorByAttribute(damage.Element);
            }

            Brush brush = new SolidBrush(color);
            g.DrawString(word, font, brush, xOff, Position.Y);
            brush.Dispose();
        } 
    }
}
