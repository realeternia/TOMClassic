using System;
using System.Drawing;
using ControlPlus;
using ControlPlus.Drawing;
using TaleofMonsters.Core;
using TaleofMonsters.Core.Loader;
using TaleofMonsters.Datas;
using TaleofMonsters.Datas.User;
using TaleofMonsters.Forms.CMain;
using TaleofMonsters.Forms.Items.Regions;

namespace TaleofMonsters.Forms.Items
{
    internal class ChangeResItem : IDisposable
    {
        private int index;
        private ChangeResForm.ChangeResData changeInfo;
        private bool show;
        private ImageToolTip tooltip = SystemToolTip.Instance;
        private VirtualRegion vRegion;

        private int x, y, width, height;
        private BasePanel parent;
        private BitmapButton bitmapButtonBuy;
        private Color backColor;

        public ChangeResItem(BasePanel prt, int x, int y, int width, int height)
        {
            parent = prt;
            this.x = x;
            this.y = y;
            this.width = width;
            this.height = height;
            this.bitmapButtonBuy = new BitmapButton();
            bitmapButtonBuy.Location = new Point(x + 152, y + 30);
            bitmapButtonBuy.Size = new Size(35, 20);
            this.bitmapButtonBuy.Click += new System.EventHandler(this.pictureBoxBuy_Click);
            this.bitmapButtonBuy.ImageNormal = PicLoader.Read("Button.Panel", "ButtonBack2.PNG");
            bitmapButtonBuy.Font = new Font("宋体", 8 * 1.33f, FontStyle.Regular, GraphicsUnit.Pixel);
            bitmapButtonBuy.ForeColor = Color.White;
            bitmapButtonBuy.IconImage = TaleofMonsters.Core.HSIcons.GetIconsByEName("oth9");
            bitmapButtonBuy.IconSize = new Size(16, 16);
            bitmapButtonBuy.IconXY = new Point(10, 4);
            parent.Controls.Add(bitmapButtonBuy);
        }

        public void Init(int idx)
        {
            index = idx;

            switch (index)
            {
                case 0:
                case 1: backColor = Color.Black; break;
                case 2:
                case 3: backColor = Color.FromArgb(0, 0, 60); break;
                case 4:
                case 5: backColor = Color.FromArgb(0, 60, 0); break;
                case 6:
                case 7: backColor = Color.FromArgb(60, 60, 0); break;
            }

            vRegion = new VirtualRegion(parent);
            vRegion.RegionEntered += new VirtualRegion.VRegionEnteredEventHandler(virtualRegion_RegionEntered);
            vRegion.RegionLeft += new VirtualRegion.VRegionLeftEventHandler(virtualRegion_RegionLeft);
        }

        public void RefreshData(ChangeResForm.ChangeResData change)
        {
            if (!change.IsEmpty())
            {
                changeInfo = change;
                bitmapButtonBuy.Visible = !change.Used;
                bitmapButtonBuy.Enabled = UserProfile.InfoBag.HasResource((GameResourceType)changeInfo.Id1, changeInfo.Count1);

                vRegion.ClearRegion();
                var region = ComplexRegion.GetResShowRegion(1, new Point(x + 5, y + 8), 40, GetRegionType(changeInfo.Id1), (int)changeInfo.Count1);
                vRegion.AddRegion(region);
                region = ComplexRegion.GetResShowRegion(2, new Point(x + 97, y + 8), 40, GetRegionType(changeInfo.Id2), (int)changeInfo.Count2);
                vRegion.AddRegion(region);
                show = true;
            }
            else
            {
                changeInfo.Id1 = 0;
                changeInfo.Count1 = 0;
                changeInfo.Id2 = 0;
                changeInfo.Count2 = 0;
                vRegion.ClearRegion();
                bitmapButtonBuy.Visible = false;
                show = false;
            }

            parent.Invalidate(new Rectangle(x, y, width, height));
        }

        private static ImageRegionCellType GetRegionType(int res)
        {
            ImageRegionCellType regionType = ImageRegionCellType.Gold;
            switch ((GameResourceType)res)
            {
                case GameResourceType.Gold:
                    regionType = ImageRegionCellType.Gold;
                    break;
                case GameResourceType.Lumber:
                    regionType = ImageRegionCellType.Lumber;
                    break;
                case GameResourceType.Stone:
                    regionType = ImageRegionCellType.Stone;
                    break;
                case GameResourceType.Mercury:
                    regionType = ImageRegionCellType.Mercury;
                    break;
                case GameResourceType.Carbuncle:
                    regionType = ImageRegionCellType.Carbuncle;
                    break;
                case GameResourceType.Sulfur:
                    regionType = ImageRegionCellType.Sulfur;
                    break;
                case GameResourceType.Gem:
                    regionType = ImageRegionCellType.Gem;
                    break;
            }
            return regionType;
        }


        private void virtualRegion_RegionEntered(int info, int mx, int my, int key)
        {
            if (info == 1 && changeInfo.Id1 >= 0)
            {
                ShowResTip(mx, my, changeInfo.Id1, changeInfo.Count1);
            }
            else if (info == 2 && changeInfo.Id2 >= 0)
            {
                ShowResTip(mx, my, changeInfo.Id2, changeInfo.Count2);
            }
        }

        private void virtualRegion_RegionLeft()
        {
            tooltip.Hide(parent);
        }

        private void pictureBoxBuy_Click(object sender, EventArgs e)
        {
            if (!UserProfile.InfoBag.HasResource((GameResourceType)changeInfo.Id1, changeInfo.Count1))
                return;

            UserProfile.InfoBag.SubResource((GameResourceType)changeInfo.Id1, changeInfo.Count1);
            UserProfile.InfoBag.SubResource((GameResourceType)changeInfo.Id2, changeInfo.Count2);
            changeInfo.Used = true;

            parent.RefreshInfo();
        }

        private void ShowResTip(int mx, int my, int res, uint amount)
        {
            var resName = HSTypes.I2Resource(res);
            string resStr = string.Format("{0}:{1}", resName, amount);
            Image image = DrawTool.GetImageByString(resStr, 100);
            tooltip.Show(image, parent, mx, my);
        }

        public void Draw(Graphics g)
        {
            SolidBrush sb = new SolidBrush(backColor);
            g.FillRectangle(sb, x, y, width, height);
            sb.Dispose();
            g.DrawRectangle(Pens.White, x, y, width - 1,height - 1);

            if (show)
            {
                Image img = PicLoader.Read("System", "ArrowS.PNG");
                g.DrawImage(img, x + 54,y+19, 35, 20);
                img.Dispose();

                if (!bitmapButtonBuy.Visible)
                {
                    Font font = new Font("微软雅黑", 10*1.33f, FontStyle.Regular, GraphicsUnit.Pixel);
                    g.DrawString("完成", font, Brushes.LightGreen, x + 152, y + 30);
                    font.Dispose();
                }

                vRegion.Draw(g);
            }
        }
        public void Dispose()
        {
        }
    }
}
