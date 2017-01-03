using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;
using NarlonLib.Control;
using TaleofMonsters.Controler.Loader;
using TaleofMonsters.Core;
using TaleofMonsters.Forms.Items.Regions;
using TaleofMonsters.DataType.Achieves;
using ConfigDatas;

namespace TaleofMonsters.Forms.Items
{
    internal class AchieveItem
    {
        private int index;
        private int aid;
        private bool show;
        private ImageToolTip tooltip = MainItem.SystemToolTip.Instance;
        private VirtualRegion virtualRegion;

        private int x, y, width, height;
        private Control parent;

        public AchieveItem(UserControl prt, int x, int y, int width, int height)
        {
            parent = prt;
            this.x = x;
            this.y = y;
            this.width = width;
            this.height = height;
        }

        public void Init(int idx)
        {
            index = idx;

            virtualRegion = new VirtualRegion(parent);
            virtualRegion.AddRegion(new PictureRegion(1, x + 3, y + 3, 76, 75, PictureRegionCellType.Achieve, 0));
            virtualRegion.RegionEntered += new VirtualRegion.VRegionEnteredEventHandler(virtualRegion_RegionEntered);
            virtualRegion.RegionLeft += new VirtualRegion.VRegionLeftEventHandler(virtualRegion_RegionLeft);
        }

        public void RefreshData(int acid)
        {
            aid = acid;
            if (aid > 0)
            {
                virtualRegion.SetRegionKey(1, aid);
                show = true;
            }
            else
            {
                virtualRegion.SetRegionKey(1, 0);
                show = false;
            }

            parent.Invalidate(new Rectangle(x, y, width, height));
        }

        private void virtualRegion_RegionEntered(int info, int mx, int my, int key)
        {
            if (aid > 0)
            {
                Image image = AchieveBook.GetPreview(aid);
                tooltip.Show(image, parent, mx, my, aid);
            }
        }

        private void virtualRegion_RegionLeft()
        {
            tooltip.Hide(parent, aid);
        }

        public void Draw(Graphics g)
        {
            if (show)
            {
                AchieveConfig achieveConfig = ConfigData.GetAchieveConfig(aid);

                virtualRegion.Draw(g);

                int bound = achieveConfig.Condition.Value;
                int get = DataType.User.UserProfile.Profile.GetAchieveState(aid);

                Font ft = new Font("宋体", 11.5f*1.33f, FontStyle.Bold, GraphicsUnit.Pixel);
                Image back = PicLoader.Read("System", "AchieveBack.JPG");
                if (get >= bound)
                {
                    get = bound;
                    g.DrawImage(back, x, y, width, height);
                    g.DrawImage(AchieveBook.GetAchieveImage(aid), x + 14, y + 12, 50, 50);
                    g.DrawString(achieveConfig.Name, ft, Brushes.Gold, x + 98, y + 12);
                }
                else
                {
                    Rectangle destBack = new Rectangle(x, y, width, height);
                    g.DrawImage(back, destBack, 0, 0, back.Width, back.Height, GraphicsUnit.Pixel, HSImageAttributes.ToGray);

                    Rectangle dest = new Rectangle(x + 14, y + 12, 50, 50);
                    g.DrawImage(AchieveBook.GetAchieveImage(aid), dest, 0, 0, 64, 64, GraphicsUnit.Pixel, HSImageAttributes.ToGray);
                    g.DrawString(achieveConfig.Name, ft, Brushes.Gray, x + 98, y + 12);
                }
                back.Dispose();
                ft.Dispose();
                LinearGradientBrush b1 = new LinearGradientBrush(new Rectangle(x + 102, y + 53, 100, 9), Color.White, Color.Gray, LinearGradientMode.Vertical);
                g.FillRectangle(b1, x + 87, y + 44, get * 88 / bound, 9);
                b1.Dispose();
            }
        }
    }
}
