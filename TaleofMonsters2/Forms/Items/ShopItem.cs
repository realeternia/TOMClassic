using System;
using System.Drawing;
using ConfigDatas;
using NarlonLib.Control;
using TaleofMonsters.Config;
using TaleofMonsters.Controler.Loader;
using TaleofMonsters.Core;
using TaleofMonsters.DataType;
using TaleofMonsters.DataType.Equips;
using TaleofMonsters.DataType.Items;
using TaleofMonsters.DataType.Others;
using TaleofMonsters.DataType.User;
using TaleofMonsters.Forms.Items.Regions;

namespace TaleofMonsters.Forms.Items
{
    internal class ShopItem
    {
        private int itemId;
        private int priceType;//货币类型
        private int price;
        private bool show;
        private ImageToolTip tooltip = MainItem.SystemToolTip.Instance;
        private VirtualRegion virtualRegion;

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

        public void Init(int type)
        {
            priceType = type;
            virtualRegion = new VirtualRegion(parent);
            virtualRegion.AddRegion(new PictureAnimRegion(1, x + 5, y + 8, 40, 40, PictureRegionCellType.Card, 0));
            virtualRegion.RegionEntered += new VirtualRegion.VRegionEnteredEventHandler(virtualRegion_RegionEntered);
            virtualRegion.RegionLeft += new VirtualRegion.VRegionLeftEventHandler(virtualRegion_RegionLeft);
        }

        public void RefreshData(int id)
        {
            bitmapButtonBuy.Visible = id != 0;
            show = id != 0;
            itemId = id;
            if (id != 0)
            {
                var isEquip = ConfigIdManager.IsEquip(id);
                virtualRegion.SetRegionKey(1, id);
                virtualRegion.SetRegionType(1, !isEquip ? PictureRegionCellType.Item : PictureRegionCellType.Equip);
                if (!isEquip)
                {
                    var itmConfig = ConfigData.GetHItemConfig(itemId);
                    price = (int)GameResourceBook.OutGoldSellItem(itmConfig.Rare, itmConfig.ValueFactor);
                }
                else
                {
                    price = ConfigData.GetEquipConfig(itemId).Value;
                }
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
                Image image = null;
                var isEquip = ConfigIdManager.IsEquip(itemId);
                if (!isEquip)
                {
                    image = HItemBook.GetPreview(itemId);
                }
                else
                {
                    Equip equip = new Equip(itemId);
                    image = equip.GetPreview();
                }
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
                parent.AddFlowCenter("资源不足", "Red");
                return;
            }

            UserProfile.InfoBag.SubResource((GameResourceType) priceType, (uint)price);
            var isEquip = ConfigIdManager.IsEquip(itemId);
            if (!isEquip)
            {
                UserProfile.InfoBag.AddItem(itemId, 1);
            }
            else
            {
                UserProfile.InfoEquip.AddEquip(itemId, 0);
            }
        }

        public void Draw(Graphics g)
        {
            g.DrawRectangle(Pens.White, x, y, width - 1, height - 1);

            if (show)
            {
                Font font = new Font("微软雅黑", 10*1.33f, FontStyle.Regular, GraphicsUnit.Pixel);
                var isEquip = ConfigIdManager.IsEquip(itemId);
                if (!isEquip)
                {
                    HItemConfig itemConfig = ConfigData.GetHItemConfig(itemId);

                    Brush brush = new SolidBrush(Color.FromName(HSTypes.I2RareColor(itemConfig.Rare)));
                    g.DrawString(itemConfig.Name, font, brush, x+50, y+7);
                    brush.Dispose();
                }
                else
                {
                    EquipConfig equipConfig = ConfigData.GetEquipConfig(itemId);

                    Brush brush = new SolidBrush(Color.FromName(HSTypes.I2RareColor(equipConfig.Quality)));
                    g.DrawString(equipConfig.Name, font, brush, x + 50, y + 7);
                    brush.Dispose();
                }
                g.DrawString(price.ToString(), font, Brushes.Gold,x+ 50,y+ 30);
                g.DrawImage(HSIcons.GetIconsByEName("res"+(priceType+1)), g.MeasureString(price.ToString(), font).Width + 50+x, 32+y, 16, 16);
                font.Dispose();

                virtualRegion.Draw(g);
            }
        }
    }
}
