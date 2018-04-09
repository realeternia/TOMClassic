using System.Drawing;
using ConfigDatas;
using TaleofMonsters.Core;
using TaleofMonsters.Datas.Items;

namespace TaleofMonsters.Controler.Battle.Data.MemFlow
{
    class FlowItemInfo : FlowWord
    {
        private int id;
        public override bool NoOverlap { get { return true; } }

        internal FlowItemInfo(int itemId, Point point, int offX, int offY) 
            : base("", point, -2, "White", offX, offY, 1, 3, 30)
        {
            id = itemId;
            HItemConfig itemConfig = ConfigData.GetHItemConfig(id);
            word = string.Format("{0}", itemConfig.Name);
            color = Color.FromName(HSTypes.I2RareColor(itemConfig.Rare));
        }

        public override void Draw(Graphics g)
        {
            g.DrawImage(HItemBook.GetHItemImage(id), Position.X, Position.Y, 20, 20);

            g.DrawString(word, font, Brushes.Black, Position.X + 23, Position.Y + 1);
            Brush brush = new SolidBrush(color);
            g.DrawString(word, font, brush, Position.X + 21, Position.Y);
            brush.Dispose();
        } 
    }
}
