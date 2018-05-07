using System;
using System.Drawing;
using TaleofMonsters.Core;
using TaleofMonsters.Forms.Items.Regions;
using ConfigDatas;
using ControlPlus;
using TaleofMonsters.Core.Config;
using TaleofMonsters.Core.Loader;
using TaleofMonsters.Datas;
using TaleofMonsters.Datas.Items;
using TaleofMonsters.Datas.Others;
using TaleofMonsters.Datas.User;
using TaleofMonsters.Forms.CMain;

namespace TaleofMonsters.Forms.Items
{
    internal class GameShopItem : ICellItem
    {
        public int X { get; set; }
        public int Y { get; set; }
        public int Width { get { return 170; } }
        public int Height { get { return 101; } }

        private bool show;
        private ImageToolTip tooltip = SystemToolTip.Instance;
        private VirtualRegion vRegion;

        private int productId;
        private BasePanel parent;
        private BitmapButton bitmapButtonBuy;


        public GameShopItem(BasePanel prt)
        {
            parent = prt;
        }

        public void Init(int idx)
        {
            vRegion = new VirtualRegion(parent);
            vRegion.AddRegion(new PictureAnimRegion(1, X + 11, Y + 19, 56, 56, PictureRegionCellType.Item, 0));
            vRegion.RegionEntered += new VirtualRegion.VRegionEnteredEventHandler(virtualRegion_RegionEntered);
            vRegion.RegionLeft += new VirtualRegion.VRegionLeftEventHandler(virtualRegion_RegionLeft);

            this.bitmapButtonBuy = new BitmapButton();
            bitmapButtonBuy.Location = new Point(X + 125, Y + 70);
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

        public void RefreshData(object data)//商品id
        {
            var id = (int)data;
            productId = id;
            GameShopConfig gameShopConfig = ConfigData.GetGameShopConfig(id);
            bitmapButtonBuy.Visible = id != 0;
            show = id != 0;

            if (id != 0)
            {
                var eid = HItemBook.GetItemId(gameShopConfig.Item);
                vRegion.SetRegionKey(1, eid);
                var isEquip = ConfigIdManager.IsEquip(eid);
                vRegion.SetRegionType(1, !isEquip ? PictureRegionCellType.Item : PictureRegionCellType.Equip);
            }

            parent.Invalidate(new Rectangle(X, Y, Width, Height));
        }


        private void virtualRegion_RegionEntered(int info, int mx, int my, int key)
        {
            if (info == 1 && productId > 0)
            {
                GameShopConfig gameShopConfig = ConfigData.GetGameShopConfig(productId);
                Image image =null;
                var eid = HItemBook.GetItemId(gameShopConfig.Item);
                image = HItemBook.GetPreview(eid);
                tooltip.Show(image, parent, mx, my, eid);
            }
        }

        private void virtualRegion_RegionLeft()
        {
            if (productId == 0)
            {
                tooltip.Hide(parent, 0);
            }
            else
            {
                GameShopConfig gameShopConfig = ConfigData.GetGameShopConfig(productId);
                var eid = HItemBook.GetItemId(gameShopConfig.Item);
                tooltip.Hide(parent, eid);
            }
        }

        private void pictureBoxBuy_Click(object sender, EventArgs e)
        {
            if (UserProfile.InfoBag.GetBlankCount() <= 0)
            {
                parent.AddFlowCenter(HSErrors.GetDescript(ErrorConfig.Indexer.BagIsFull), "Red");
                return;
            }

            var gameShopConfig = ConfigData.GetGameShopConfig(productId);
            var eid = HItemBook.GetItemId(gameShopConfig.Item);
            var itmConfig = ConfigData.GetHItemConfig(eid);
            var goldPrice = GameResourceBook.OutGoldSellItem(itmConfig.Rare, itmConfig.ValueFactor)*2;
            bool buyFin = false;
            if (gameShopConfig.UseDiamond)
            {
                var diamondPrice = (int)Math.Max(1, goldPrice / GameConstants.DiamondToGold);
                if (UserProfile.InfoBag.PayDiamond(diamondPrice))
                {
                    UserProfile.InfoBag.AddItem(eid, 1);
                    buyFin = true;
                }
                else
                    parent.AddFlowCenter(HSErrors.GetDescript(ErrorConfig.Indexer.BagNotEnoughDimond), "Red");
            }
            else
            {
                if (UserProfile.InfoBag.HasResource(GameResourceType.Gold, goldPrice))
                {
                    UserProfile.InfoBag.SubResource(GameResourceType.Gold, goldPrice);
                    UserProfile.InfoBag.AddItem(eid, 1);
                    buyFin = true;
                }
                else
                {
                    parent.AddFlowCenter(HSErrors.GetDescript(ErrorConfig.Indexer.BagNotEnoughResource), "Red");
                }
            }
            
            if(buyFin)
            {
                parent.AddFlowCenter("+1", "Lime", HItemBook.GetHItemImage(eid));
            }
            //PopBuyProduct.Show(eid, (int)Math.Max(1, itemPrice / GameConstants.DiamondToGold));
        }

        public void Draw(Graphics g)
        {
            Image back = PicLoader.Read("System", "ShopItemBack.JPG");
            g.DrawImage(back, X, Y, Width - 1, Height - 1);
            back.Dispose();

            if (show)
            {
                GameShopConfig gameShopConfig = ConfigData.GetGameShopConfig(productId);
                var eid = HItemBook.GetItemId(gameShopConfig.Item);
                HItemConfig itemConfig = ConfigData.GetHItemConfig(eid);
                var name = itemConfig.Name;
                var fontcolor = HSTypes.I2RareColor(itemConfig.Rare);
                uint price = GameResourceBook.OutGoldSellItem(itemConfig.Rare, itemConfig.ValueFactor)*2;
                if (gameShopConfig.UseDiamond)
                    price = Math.Max(1, price/GameConstants.DiamondToGold);
                Font fontsong = new Font("宋体", 10*1.33f, FontStyle.Regular, GraphicsUnit.Pixel);
                Brush brush = new SolidBrush(Color.FromName(fontcolor));
                g.DrawString(name, fontsong, brush, X + 76, Y + 9);
                brush.Dispose();
                g.DrawString(string.Format("{0,3:D}", price), fontsong, Brushes.PaleTurquoise, X + 80, Y + 37);
                fontsong.Dispose();
                if (gameShopConfig.UseDiamond)
                    g.DrawImage(HSIcons.GetIconsByEName("res8"), X + 110, Y + 35, 16, 16);
                else
                    g.DrawImage(HSIcons.GetIconsByEName("res1"), X + 110, Y + 35, 16, 16);

                vRegion.Draw(g);
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
