using System.Drawing;
using TaleofMonsters.DataType.Items;

namespace TaleofMonsters.Forms.MagicBook
{
    internal class ItemDetail
    {
        private int itemId = -1;
        private int type = -1;
        private int x;
        private int y;
        private int height;

        #region 属性
        public int ItemId
        {
            get { return itemId; }
            set { itemId = value; }
        }

        public int Type
        {
            get { return type; }
            set { type = value; }
        }
        #endregion

        public ItemDetail(int x, int y, int height)
        {
            this.x = x;
            this.y = y;
            this.height = height;
        }

        public void Draw(Graphics g)
        {
            g.FillRectangle(Brushes.Thistle, x, y, 200, height);
            if (ItemId != -1)
            {
                if (type == 1)
                {
                    HItemBook.DrawOnDeck(itemId, g, x, y);
                }
            }
        }
    }
}
