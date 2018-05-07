using System;
using System.Drawing;
using ConfigDatas;
using ControlPlus;
using TaleofMonsters.Core;
using TaleofMonsters.Core.Loader;
using TaleofMonsters.Datas;
using TaleofMonsters.Datas.Blesses;
using TaleofMonsters.Datas.Others;
using TaleofMonsters.Datas.User;
using TaleofMonsters.Forms.CMain;
using TaleofMonsters.Forms.CMain.Blesses;
using TaleofMonsters.Forms.Items.Regions;

namespace TaleofMonsters.Forms.Items
{
    internal class BlessItem : IDisposable
    {
        private int index;
        private int blessId;
        private bool show;
        private ImageToolTip tooltip = SystemToolTip.Instance;
        private VirtualRegion vRegion;

        private int x, y, width, height;
        private BasePanel parent;
        private BitmapButton bitmapButtonBuy;

        public BlessItem(BasePanel prt, int x, int y, int width, int height)
        {
            parent = prt;
            this.x = x;
            this.y = y;
            this.width = width;
            this.height = height;
            this.bitmapButtonBuy = new BitmapButton();
            bitmapButtonBuy.Location = new Point(x + 125, y + 53);
            bitmapButtonBuy.Size = new Size(50, 24);
            this.bitmapButtonBuy.Click += new System.EventHandler(this.pictureBoxBuy_Click);
            this.bitmapButtonBuy.ImageNormal = PicLoader.Read("Button.Panel", "ButtonBack2.PNG");
            bitmapButtonBuy.Font = new Font("宋体", 8 * 1.33f, FontStyle.Regular, GraphicsUnit.Pixel);
            bitmapButtonBuy.ForeColor = Color.White;
            bitmapButtonBuy.IconImage = TaleofMonsters.Core.HSIcons.GetIconsByEName("hatt2");
            bitmapButtonBuy.IconSize = new Size(16, 16);
            bitmapButtonBuy.IconXY = new Point(4, 4);
            bitmapButtonBuy.TextOffX = 8;
            this.bitmapButtonBuy.Text = @"交易";
            parent.Controls.Add(bitmapButtonBuy);
        }

        public void Init(int idx)
        {
            index = idx;

            vRegion = new VirtualRegion(parent);
            vRegion.AddRegion(new PictureRegion(1, x + 3, y + 3, 76, 75, PictureRegionCellType.Bless, 0));
            vRegion.RegionEntered += new VirtualRegion.VRegionEnteredEventHandler(virtualRegion_RegionEntered);
            vRegion.RegionLeft += new VirtualRegion.VRegionLeftEventHandler(virtualRegion_RegionLeft);
        }

        public void RefreshData(int eid)
        {
            blessId = eid;
            if (eid > 0)
            {
                bitmapButtonBuy.Visible = true;
                vRegion.SetRegionKey(1, eid);
                show = true;
            }
            else
            {
                vRegion.SetRegionKey(1, 0);
                bitmapButtonBuy.Visible = false;
                show = false;
            }

            var config = ConfigData.GetBlessConfig(eid);
            if (config.Type == (int)BlessTypes.Active)
            {
                bitmapButtonBuy.IconImage = TaleofMonsters.Core.HSIcons.GetIconsByEName("oth9");
                this.bitmapButtonBuy.Text = @"购买";
            }
            else if (config.Type == (int)BlessTypes.Negative)
            {
                bitmapButtonBuy.IconImage = TaleofMonsters.Core.HSIcons.GetIconsByEName("hatt8");
                this.bitmapButtonBuy.Text = @"移除";
            }

            parent.Invalidate(new Rectangle(x, y, width, height));
        }

        private void virtualRegion_RegionEntered(int info, int mx, int my, int key)
        {
            if (blessId > 0)
            {
                Image image = BlessBook.GetPreview(key);
                tooltip.Show(image, parent, mx, my, blessId);
            }
        }

        private void virtualRegion_RegionLeft()
        {
            tooltip.Hide(parent, blessId);
        }

        private void pictureBoxBuy_Click(object sender, EventArgs e)
        {
            var config = ConfigData.GetBlessConfig(blessId);
            uint cost = GameResourceBook.OutMercuryBlessBuy(config.Level);

            if (!UserProfile.InfoBag.HasResource(GameResourceType.Mercury, cost))
            {
                parent.AddFlowCenter(HSErrors.GetDescript(ErrorConfig.Indexer.BagNotEnoughResource), "Red");
                return;
            }

            UserProfile.InfoBag.SubResource(GameResourceType.Mercury, cost);
            if (config.Type == (int)BlessTypes.Active) //买入
            {
                BlessManager.AddBless(blessId);
                UserProfile.InfoWorld.BlessShopItems.Remove(blessId);
                parent.AddFlowCenter("祝福成功", "Lime");
            }
            else if (config.Type == (int)BlessTypes.Negative)
            {
                BlessManager.RemoveBless(blessId);
                parent.AddFlowCenter("移除成功", "Lime");
            }

            parent.RefreshInfo();
        }

        public void Draw(Graphics g)
        {
            SolidBrush sb = new SolidBrush(Color.FromArgb(20, 20, 20));
            g.FillRectangle(sb, x + 2, y + 2, width - 4, height - 4);
            sb.Dispose();
            g.DrawRectangle(Pens.Teal, x + 2, y + 2, width - 4, height - 4);

            if (show)
            {
                var blessConfig = ConfigData.GetBlessConfig(blessId);

                vRegion.Draw(g);

                var cost = GameResourceBook.OutMercuryBlessBuy(blessConfig.Level);
                Font ft = new Font("宋体", 9 * 1.33f, FontStyle.Regular, GraphicsUnit.Pixel);
                Brush b = new SolidBrush(Color.FromName(HSTypes.I2QualityColor(blessConfig.Level)));
                g.DrawString(blessConfig.Name, ft, b, x + 90, y + 10);
                b.Dispose();
                g.DrawString(string.Format("{0}", cost), ft, Brushes.White, x + 90 + 20, y + 32);
                ft.Dispose();

                g.DrawImage(HSIcons.GetIconsByEName("res4"), x + 90, y + 32 - 3, 18, 18);
            }
        }
        public void Dispose()
        {
        }
    }
}
