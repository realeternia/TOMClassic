using System.Drawing;
using TaleofMonsters.Controler.Loader;
using TaleofMonsters.Core;
using TaleofMonsters.DataType.Items;
using TaleofMonsters.DataType.User;

namespace TaleofMonsters.Forms.Items
{
    internal class MiniItemViewItem
    {
        public int itemPos;
        private int x;
        private int y;
        private int id;
        private Rectangle rect;
        private int percent;

        public int Id
        {
            get { return id; }
        }

        public Rectangle Rectangle
        {
            get {return rect;}
        }

        public int Percent
        {
            set { percent = value; }
        }

        public MiniItemViewItem(int id, int x, int y)
        {
            this.id = id;
            this.x = x;
            this.y = y;
            rect = new Rectangle(x, y, 35, 35);
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

            if (itemPos >= 0)
            {
                Font font = new Font("Aril", 11*1.33f, FontStyle.Bold, GraphicsUnit.Pixel);
                if (enable)
                {
                    g.DrawImage(HItemBook.GetHItemImage(UserProfile.InfoBag.Items[itemPos].Type), x + 3, y + 3, 32, 32);
                }
                else
                {
                    Rectangle ret = new Rectangle(x + 3, y + 3, 32, 32);
                    g.DrawImage(HItemBook.GetHItemImage(UserProfile.InfoBag.Items[itemPos].Type), ret, 0, 0, 64, 64, GraphicsUnit.Pixel, HSImageAttributes.ToGray);
                }

                g.DrawString(UserProfile.InfoBag.Items[itemPos].Value.ToString(), font, Brushes.Black, x + 4, y + 4);
                g.DrawString(UserProfile.InfoBag.Items[itemPos].Value.ToString(), font, Brushes.White, x + 3, y + 3);

                if (percent>1)
                {
                    Brush brush = new SolidBrush(Color.FromArgb(200, Color.Black));
                    g.FillRectangle(brush, x, y, 35, 35*percent/100);
                    brush.Dispose();
                }

                font.Dispose();
            }
        }
    }
}
