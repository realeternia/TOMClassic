using System.Drawing;
using ConfigDatas;
using TaleofMonsters.Core;
using TaleofMonsters.DataType.Items;

namespace TaleofMonsters.Controler.Battle.Data.MemFlow
{
    class FlowItemInfo : FlowWord
    {
        private int id;
        internal FlowItemInfo(int item_id, Point point, int offX, int offY) 
            : base("", point, -2, "White", offX, offY, 1, 3, 30)
        {
            id = item_id;
            HItemConfig itemConfig = ConfigData.GetHItemConfig(id);
            word = string.Format("{0}", itemConfig.Name);
            color = Color.FromName(HSTypes.I2RareColor(itemConfig.Rare));
        }

        internal override void Draw(Graphics g)
        {
            g.DrawImage(HItemBook.GetHItemImage(id), position.X, position.Y, 20, 20);

            g.DrawString(word, font, Brushes.Black, position.X + 23, position.Y + 1);
            Brush brush = new SolidBrush(color);
            g.DrawString(word, font, brush, position.X + 21, position.Y);
            brush.Dispose();
        } 
    }
}
