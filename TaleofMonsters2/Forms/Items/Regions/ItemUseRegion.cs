using System.Drawing;
using TaleofMonsters.Core.Interface;
using TaleofMonsters.DataType.User;

namespace TaleofMonsters.Forms.Items.Regions
{
    internal class ItemUseRegion : SubVirtualRegion
    {
        protected int nid;
        private int need;

        public ItemUseRegion(int id, int x, int y, int width, int height, int info, int nid, int need)
            : base(id, x, y, width, height, info)
        {
            this.nid = nid;
            this.need = need;//需求的道具
        }

        public override void Draw(Graphics g)
        {
            if (nid > 0)
            {
                int itemCount = UserProfile.Profile.InfoBag.GetItemCount(nid);
                bool isEnough = itemCount >= need;

                g.DrawImage(DataType.Items.HItemBook.GetHItemImage(nid), x, y, width, height);
                if (IsIn && isEnough)
                {
                    g.DrawImage(DataType.Items.HItemBook.GetHItemImage(nid), x+3, y+3, width-6, height-6);
                }
                if (!isEnough)
                {//灰色遮罩
                    Brush brush = new SolidBrush(Color.FromArgb(120, Color.Black));
                    g.FillRectangle(brush, x, y, width, height);
                    brush.Dispose();
                }

                Font fontsong = new Font("宋体", 7*1.33f, FontStyle.Regular, GraphicsUnit.Pixel);
                g.DrawString(string.Format("{0}/{1}", need, itemCount), fontsong, isEnough ? Brushes.White : Brushes.Red, x + 1, y + 18);
                fontsong.Dispose();
            }

            foreach (IRegionDecorator decorator in decorators)
            {
                decorator.Draw(g, x, y, width, height);
            }
        }

        public override void SetKeyValue(int value)
        {
            base.SetKeyValue(value);
            nid = value;
        }

        public override int GetKeyValue()
        {
            return nid;
        }

        public override void Enter()
        {
            base.Enter();
            if (Parent != null)
            {
                Parent.Invalidate(new Rectangle(x, y, width, height));
            }
        }

        public override void Left()
        {
            base.Left();
            if (Parent != null)
            {
                Parent.Invalidate(new Rectangle(x, y, width, height));
            }
        }
    }
}
