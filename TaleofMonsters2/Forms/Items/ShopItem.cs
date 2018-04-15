using System;
using System.Drawing;
using System.Windows.Forms;
using ConfigDatas;
using ControlPlus;
using NarlonLib.Math;
using TaleofMonsters.Core;
using TaleofMonsters.Core.Loader;
using TaleofMonsters.Datas;
using TaleofMonsters.Datas.Items;
using TaleofMonsters.Datas.Others;
using TaleofMonsters.Datas.User;
using TaleofMonsters.Forms.CMain;
using TaleofMonsters.Forms.Items.Regions;
using TaleofMonsters.Forms.Items.Regions.Decorators;

namespace TaleofMonsters.Forms.Items
{
    internal class ShopItem : IDisposable
    {
        private enum PriceRandTypes
        {
            None, Good1, Good2, Good3, Bad1, Bad2, Bad3
        }

        private int itemId;
        private int priceType;//货币类型
        private int price;
        private int limitCount; //销售上限
        private PriceRandTypes randomType;
        private bool show;
        private ImageToolTip tooltip = SystemToolTip.Instance;
        private VirtualRegion vRegion;

        private int x, y, width, height;
        private BasePanel parent;
        private BitmapButton bitmapButtonBuy;

        public ShopItem(BasePanel prt, int x, int y, int width, int height)
        {
            parent = prt;
            this.x = x;
            this.y = y;
            this.width = width;
            this.height = height;
            this.bitmapButtonBuy = new BitmapButton();
            bitmapButtonBuy.Location = new Point(x + 102, y + 30);
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

        public void Init(int type, bool isRandom, int limit)
        {
            priceType = type;
            if (isRandom)
                randomType = GetTypes();
            limitCount = limit;
            vRegion = new VirtualRegion(parent);
            vRegion.AddRegion(new PictureAnimRegion(1, x + 5, y + 8, 40, 40, PictureRegionCellType.Item, 0));
            vRegion.RegionEntered += new VirtualRegion.VRegionEnteredEventHandler(virtualRegion_RegionEntered);
            vRegion.RegionLeft += new VirtualRegion.VRegionLeftEventHandler(virtualRegion_RegionLeft);
        }

        public void RefreshData(int id)
        {
            bitmapButtonBuy.Visible = id != 0;
            show = id != 0;
            itemId = id;
            if (id != 0)
            {
                vRegion.SetRegionKey(1, id);
                var itmConfig = ConfigData.GetHItemConfig(itemId);
                price = (int)GameResourceBook.OutGoldSellItem(itmConfig.Rare, itmConfig.ValueFactor);
                RecheckPrice();
            }

            if (priceType > 0) //非金币购买
            {
                price = price / 10 + 1;
            }

            parent.Invalidate(new Rectangle(x, y, width, height));
        }


        private void virtualRegion_RegionEntered(int info, int mx, int my, int key)
        {
            if (info == 1 && itemId > 0)
            {
                Image image = HItemBook.GetPreview(itemId);
                tooltip.Show(image, parent, mx, my, itemId);
            }
        }

        private void virtualRegion_RegionLeft()
        {
            tooltip.Hide(parent, itemId);
        }

        private void pictureBoxBuy_Click(object sender, EventArgs e)
        {
            if (!UserProfile.InfoBag.HasResource((GameResourceType)priceType, (uint)price))
            {
                parent.AddFlowCenter(HSErrors.GetDescript(ErrorConfig.Indexer.BagNotEnoughResource), "Red");
                return;
            }

            if (UserProfile.InfoBag.GetBlankCount() <= 0)
            {
                parent.AddFlowCenter(HSErrors.GetDescript(ErrorConfig.Indexer.BagIsFull), "Red");
                return;
            }

            if (limitCount == 0)
                return;

            limitCount --;
            if (limitCount == 0)
            {
                vRegion.SetRegionDecorator(1, 1, new RegionCoverDecorator(Color.FromArgb(100, Color.Black)));
                bitmapButtonBuy.Hide();
            }

            UserProfile.InfoBag.SubResource((GameResourceType) priceType, (uint)price);
            UserProfile.InfoBag.AddItem(itemId, 1);
        }

        public void Draw(Graphics g)
        {
            g.DrawRectangle(Pens.White, x, y, width - 1, height - 1);

            if (show)
            {
                Font font = new Font("微软雅黑", 10*1.33f, FontStyle.Regular, GraphicsUnit.Pixel);
                HItemConfig itemConfig = ConfigData.GetHItemConfig(itemId);

                var sellName = itemConfig.Name;
                if (limitCount < 10)
                    sellName = string.Format("{0} (余{1})", itemConfig.Name, limitCount);
                Brush brush = new SolidBrush(Color.FromName(HSTypes.I2RareColor(itemConfig.Rare)));
                g.DrawString(sellName, font, brush, x + 50, y + 7);
                brush.Dispose();

                g.DrawString(price.ToString(), font, Brushes.Gold,x+ 50,y+ 30);
                var wid = TextRenderer.MeasureText(g, price.ToString(), font, new Size(0, 0), TextFormatFlags.NoPadding).Width;
                g.DrawImage(HSIcons.GetIconsByEName("res"+(priceType+1)), wid + 50+x, 32+y, 16, 16);
                font.Dispose();

                vRegion.Draw(g);

                string iconType = "";
                int repeat = 0;
                switch (randomType)
                {
                    case PriceRandTypes.Good1: iconType = "oth12"; repeat=1; break;
                    case PriceRandTypes.Good2: iconType = "oth12"; repeat = 2; break;
                    case PriceRandTypes.Good3: iconType = "oth12"; repeat = 3; break;
                    case PriceRandTypes.Bad1: iconType = "oth13"; repeat = 1; break;
                    case PriceRandTypes.Bad2: iconType = "oth13"; repeat = 2; break;
                    case PriceRandTypes.Bad3: iconType = "oth13"; repeat = 3; break;
                }

                for (int i = 0; i < repeat; i++)
                    g.DrawImage(HSIcons.GetIconsByEName(iconType), x + 5 + 5 + 7 * i, y + 8 + 20, 16, 16);
            }
        }

        private PriceRandTypes GetTypes()
        {
            var roll = MathTool.GetRandom(100);
            if (roll <= 40)
                return PriceRandTypes.None;
            if (roll <= 60)
                return PriceRandTypes.Good1;
            if (roll <= 75)
                return PriceRandTypes.Bad1;
            if (roll <= 85)
                return PriceRandTypes.Good2;
            if (roll <= 90)
                return PriceRandTypes.Good3;
            if (roll <= 95)
                return PriceRandTypes.Bad2;
            return PriceRandTypes.Bad3;
        }

        private void RecheckPrice()
        {
            var newPrice = price;
            switch (randomType)
            {
                case PriceRandTypes.Good1: newPrice = (int)Math.Min(price-1, price*MathTool.GetRandom(0.88,0.95)); break;
                case PriceRandTypes.Good2: newPrice = (int)Math.Min(price - 1, price * MathTool.GetRandom(0.70, 0.85)); break;
                case PriceRandTypes.Good3: newPrice = (int)Math.Min(price - 1, price * MathTool.GetRandom(0.50, 0.65)); break;
                case PriceRandTypes.Bad1: newPrice = (int)Math.Max(price + 1, price * MathTool.GetRandom(1.05, 1.15)); break;
                case PriceRandTypes.Bad2: newPrice = (int)Math.Max(price + 1, price * MathTool.GetRandom(1.2, 1.4)); break;
                case PriceRandTypes.Bad3: newPrice = (int)Math.Max(price + 1, price * MathTool.GetRandom(1.5, 1.8)); break;
            }
            price = newPrice;
        }
        public void Dispose()
        {
        }
    }
}
