using System.Drawing;
using TaleofMonsters.Core;
using TaleofMonsters.Core.Loader;
using TaleofMonsters.DataType.Items;
using TaleofMonsters.DataType.User;

namespace TaleofMonsters.Forms.Items
{
    internal class MiniItemViewItem
    {
        private int x;
        private int y;

        public int Id { get; private set; }
        public int ItemPos { get; set; }

        public Rectangle Rectangle { get; private set; }

        public int Percent { get; set; }

        public MiniItemViewItem(int id, int x, int y)
        {
            Id = id;
            this.x = x;
            this.y = y;
            Rectangle = new Rectangle(x, y, 35, 35);
        }

        public bool IsInArea(int mx, int my)
        {
            return mx > x && mx < x + 35 && my > y && my < y + 35;
        }

        public void Draw(Graphics g, bool enable)
        {
            Image back = PicLoader.Read("System", "ItemGrid.JPG");
            g.DrawImage(back, x+3, y+3, 32, 32);
            back.Dispose();

            if (ItemPos >= 0)
            {
                Font font = new Font("Aril", 11*1.33f, FontStyle.Bold, GraphicsUnit.Pixel);
                int itemId = UserProfile.InfoBag.Items[ItemPos].Type;
                if (enable)
                {
                    g.DrawImage(HItemBook.GetHItemImage(itemId), x + 3, y + 3, 32, 32);
                }
                else
                {
                    Rectangle ret = new Rectangle(x + 3, y + 3, 32, 32);
                    g.DrawImage(HItemBook.GetHItemImage(itemId), ret, 0, 0, 64, 64, GraphicsUnit.Pixel, HSImageAttributes.ToGray);
                }

                int count = UserProfile.InfoBag.Items[ItemPos].Value;
                g.DrawString(count.ToString(), font, Brushes.Black, x + 4, y + 4);
                g.DrawString(count.ToString(), font, Brushes.White, x + 3, y + 3);

                if (Percent>1)
                {
                    Brush brush = new SolidBrush(Color.FromArgb(200, Color.Black));
                    g.FillRectangle(brush, x, y, 35, 35*(100-Percent)/100);
                    brush.Dispose();
                }

                font.Dispose();
            }
        }
    }
}
