using System;
using System.Drawing;
using System.Windows.Forms;
using NarlonLib.Control;
using TaleofMonsters.Controler.Loader;
using TaleofMonsters.Core;
using TaleofMonsters.DataType.Equips;
using TaleofMonsters.DataType.Items;
using TaleofMonsters.Forms.Items.Regions;
using TaleofMonsters.Forms.Pops;
using ConfigDatas;
using TaleofMonsters.Config;
using TaleofMonsters.DataType.Others;

namespace TaleofMonsters.Forms.Items
{
    internal class GameShopItem
    {
        private bool show;
        private ImageToolTip tooltip = MainItem.SystemToolTip.Instance;
        private VirtualRegion virtualRegion;

        private int productId;
        private int x, y, width, height;
        private Control parent;
        private BitmapButton bitmapButtonBuy;

        public GameShopItem(UserControl prt, int x, int y, int width, int height)
        {
            parent = prt;
            this.x = x;
            this.y = y;
            this.width = width;
            this.height = height;
            this.bitmapButtonBuy = new BitmapButton();
            bitmapButtonBuy.Location = new Point(x + 125, y + 70);
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

        public void Init()
        {
            virtualRegion = new VirtualRegion(parent);
            virtualRegion.AddRegion(new PictureAnimRegion(1, x + 11, y + 19, 56, 56, PictureRegionCellType.Item, 0));
            virtualRegion.RegionEntered += new VirtualRegion.VRegionEnteredEventHandler(virtualRegion_RegionEntered);
            virtualRegion.RegionLeft += new VirtualRegion.VRegionLeftEventHandler(virtualRegion_RegionLeft);
        }

        public void RefreshData(int id)//商品id
        {
            productId = id;
            GameShopConfig gameShopConfig = ConfigData.GetGameShopConfig(id);
            bitmapButtonBuy.Visible = id != 0;
            show = id != 0;

            if (id != 0)
            {
                var eid = HItemBook.GetItemId(gameShopConfig.Item);
                virtualRegion.SetRegionKey(1, eid);
                var isEquip = ConfigIdManager.IsEquip(eid);
                virtualRegion.SetRegionType(1, !isEquip ? PictureRegionCellType.Item : PictureRegionCellType.Equip);
            }

            parent.Invalidate(new Rectangle(x, y, width, height));
        }


        private void virtualRegion_RegionEntered(int info, int mx, int my, int key)
        {
            if (info == 1 && productId > 0)
            {
                GameShopConfig gameShopConfig = ConfigData.GetGameShopConfig(productId);
                Image image =null;
                var eid = HItemBook.GetItemId(gameShopConfig.Item);
                var isEquip = ConfigIdManager.IsEquip(eid);
                if (!isEquip)
                {
                    image = HItemBook.GetPreview(eid);
                }
                else
                {
                    Equip equip = new Equip(eid);
                    image = equip.GetPreview();
                }
                tooltip.Show(image, parent, mx, my, eid);
            }
        }

        private void virtualRegion_RegionLeft()
        {
            GameShopConfig gameShopConfig = ConfigData.GetGameShopConfig(productId);
            var eid = HItemBook.GetItemId(gameShopConfig.Item);
            tooltip.Hide(parent, eid);
        }

        private void pictureBoxBuy_Click(object sender, EventArgs e)
        {
            GameShopConfig gameShopConfig = ConfigData.GetGameShopConfig(productId);
            var eid = HItemBook.GetItemId(gameShopConfig.Item);
            var itmConfig = ConfigData.GetHItemConfig(eid);
            var itemPrice = GameResourceBook.OutGoldSellItem(itmConfig.Rare, itmConfig.ValueFactor);
            PopBuyProduct.Show(eid, (int)Math.Max(1, itemPrice / GameConstants.DiamondToGold));
        }

        public void Draw(Graphics g)
        {
            Image back = PicLoader.Read("System", "ShopItemBack.JPG");
            g.DrawImage(back, x, y, width - 1, height - 1);
            back.Dispose();

            if (show)
            {
                GameShopConfig gameShopConfig = ConfigData.GetGameShopConfig(productId);
                string name;
                string fontcolor;
                uint price = 0;
                var eid = HItemBook.GetItemId(gameShopConfig.Item);
                var isEquip = ConfigIdManager.IsEquip(eid);
                if (isEquip)
                {
                    EquipConfig equipConfig = ConfigData.GetEquipConfig(eid);
                    name = equipConfig.Name;
                    fontcolor = HSTypes.I2QualityColor(equipConfig.Quality);
                    price = GameResourceBook.OutGoldSellItem(equipConfig.Quality, 100);
                }
                else
                {
                    HItemConfig itemConfig = ConfigData.GetHItemConfig(eid);
                    name = itemConfig.Name;
                    fontcolor = HSTypes.I2RareColor(itemConfig.Rare);
                    price = GameResourceBook.OutGoldSellItem(itemConfig.Rare, itemConfig.ValueFactor);
                }
                Font fontsong = new Font("宋体", 9*1.33f, FontStyle.Regular, GraphicsUnit.Pixel);
                Brush brush = new SolidBrush(Color.FromName(fontcolor));
                g.DrawString(name, fontsong, brush, x + 76, y + 9);
                brush.Dispose();
                g.DrawString(string.Format("{0,3:D}", Math.Max(1, price / GameConstants.DiamondToGold)), fontsong, Brushes.PaleTurquoise, x + 80, y + 37);
                fontsong.Dispose();
                g.DrawImage(HSIcons.GetIconsByEName("res8"), x + 110, y + 35, 16, 16);

                virtualRegion.Draw(g);
            }
        }
    }
}
