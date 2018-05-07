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
    internal class BlessItem : ICellItem
    {
        private BasePanel parent;
        public int X { get; set; }
        public int Y { get; set; }
        public int Width { get { return 180; } }
        public int Height { get { return 77; } }

        private int index;
        private int blessId;
        private bool show;
        private ImageToolTip tooltip = SystemToolTip.Instance;
        private VirtualRegion vRegion;

        private BitmapButton bitmapButtonBuy;

        public BlessItem(BasePanel prt)
        {
            parent = prt;
        }

        public void Init(int idx)
        {
            index = idx;

            vRegion = new VirtualRegion(parent);
            vRegion.AddRegion(new PictureRegion(1, X + 3, Y + 3, 76, 75, PictureRegionCellType.Bless, 0));
            vRegion.RegionEntered += new VirtualRegion.VRegionEnteredEventHandler(virtualRegion_RegionEntered);
            vRegion.RegionLeft += new VirtualRegion.VRegionLeftEventHandler(virtualRegion_RegionLeft);

            this.bitmapButtonBuy = new BitmapButton();
            bitmapButtonBuy.Location = new Point(X + 125, Y + 53);
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

        public void RefreshData(object data)
        {
            int eid = (int) data;
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

            parent.Invalidate(new Rectangle(X, Y, Width, Height));
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
            g.FillRectangle(sb, X + 2, Y + 2, Width - 4, Height - 4);
            sb.Dispose();
            g.DrawRectangle(Pens.Teal, X + 2, Y + 2, Width - 4, Height - 4);

            if (show)
            {
                var blessConfig = ConfigData.GetBlessConfig(blessId);

                vRegion.Draw(g);

                var cost = GameResourceBook.OutMercuryBlessBuy(blessConfig.Level);
                Font ft = new Font("宋体", 9 * 1.33f, FontStyle.Regular, GraphicsUnit.Pixel);
                Brush b = new SolidBrush(Color.FromName(HSTypes.I2QualityColor(blessConfig.Level)));
                g.DrawString(blessConfig.Name, ft, b, X + 90, Y + 10);
                b.Dispose();
                g.DrawString(string.Format("{0}", cost), ft, Brushes.White, X + 90 + 20, Y + 32);
                ft.Dispose();

                g.DrawImage(HSIcons.GetIconsByEName("res4"), X + 90, Y + 32 - 3, 18, 18);
            }
        }

        public void OnFrame()
        {
        }

        public void Dispose()
        {
        }
    }
}
