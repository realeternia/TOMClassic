using System;
using System.Drawing;
using ConfigDatas;
using NarlonLib.Control;
using TaleofMonsters.Controler.Loader;
using TaleofMonsters.Core;
using TaleofMonsters.DataType;
using TaleofMonsters.DataType.Equips;
using TaleofMonsters.DataType.Others;
using TaleofMonsters.DataType.User;
using TaleofMonsters.Forms.Items.Regions;

namespace TaleofMonsters.Forms.Items
{
    internal class EquipComposeItem
    {
        private int index;
        private int equipId;
        private bool show;
        private ImageToolTip tooltip = MainItem.SystemToolTip.Instance;
        private VirtualRegion virtualRegion;

        private int x, y, width, height;
        private BasePanel parent;
        private BitmapButton bitmapButtonBuy;

        public EquipComposeItem(BasePanel prt, int x, int y, int width, int height)
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
            this.bitmapButtonBuy.ImageNormal = PicLoader.Read("ButtonBitmap", "ButtonBack2.PNG");
            bitmapButtonBuy.Font = new Font("宋体", 8 * 1.33f, FontStyle.Regular, GraphicsUnit.Pixel);
            bitmapButtonBuy.ForeColor = Color.White;
            bitmapButtonBuy.IconImage = TaleofMonsters.Core.HSIcons.GetIconsByEName("hatt2");
            bitmapButtonBuy.IconSize = new Size(16,16);
            bitmapButtonBuy.IconXY = new Point(4,4);
            bitmapButtonBuy.TextOffX = 8;
            this.bitmapButtonBuy.Text = @"锻造";
            parent.Controls.Add(bitmapButtonBuy);
        }

        public void Init(int idx)
        {
            index = idx;

            virtualRegion = new VirtualRegion(parent);
            virtualRegion.AddRegion(new PictureRegion(1, x + 3, y + 3, 76, 75, PictureRegionCellType.Equip, 0));
            virtualRegion.RegionEntered += new VirtualRegion.VRegionEnteredEventHandler(virtualRegion_RegionEntered);
            virtualRegion.RegionLeft += new VirtualRegion.VRegionLeftEventHandler(virtualRegion_RegionLeft);
        }

        public void RefreshData(int eid)
        {
            equipId = eid;
            if (eid > 0)
            {
                bitmapButtonBuy.Visible = true;
                virtualRegion.SetRegionKey(1, eid);
                show = true;
            }
            else
            {
                virtualRegion.SetRegionKey(1, 0);
                bitmapButtonBuy.Visible = false;
                show = false;
            }

            parent.Invalidate(new Rectangle(x, y, width, height));
        }

        private void virtualRegion_RegionEntered(int info, int mx, int my, int key)
        {
            if (equipId > 0)
            {
                Equip equip = new Equip(equipId);
                Image image = equip.GetPreview();
                tooltip.Show(image, parent, mx, my, equipId);
            }
        }

        private void virtualRegion_RegionLeft()
        {
            tooltip.Hide(parent);
        }

        private void pictureBoxBuy_Click(object sender, EventArgs e)
        {
            var equipConfig = ConfigData.GetEquipConfig(equipId);
            var cost = GameResourceBook.OutStoneMerge(equipConfig.Quality + 1, equipConfig.Level);
            if (UserProfile.InfoBag.HasResource(GameResourceType.Stone, cost))
            {
                UserProfile.InfoBag.SubResource(GameResourceType.Stone, cost);
                UserProfile.InfoEquip.AddEquip(equipId, 60 * 3);
                parent.Invalidate();

                parent.AddFlowCenter("锻造成功", "Lime");
            }
            else
            {
                parent.AddFlowCenter("资源不足", "Red");
            }
        }

        public void Draw(Graphics g)
        {
            SolidBrush sb = new SolidBrush(Color.FromArgb(20, 20, 20));
            g.FillRectangle(sb, x + 2, y + 2, width - 4, height - 4);
            sb.Dispose();
            g.DrawRectangle(Pens.Teal, x + 2, y + 2, width - 4, height - 4);

            if (show)
            {
                var equipConfig = ConfigData.GetEquipConfig(equipId);

                virtualRegion.Draw(g);

                var cost = GameResourceBook.OutStoneMerge(equipConfig.Quality + 1, equipConfig.Level);
                Font ft = new Font("宋体", 9*1.33f, FontStyle.Regular, GraphicsUnit.Pixel);
                Brush b = new SolidBrush(Color.FromName(HSTypes.I2QualityColor(equipConfig.Quality)));
                g.DrawString(equipConfig.Name, ft, b, x + 90, y + 10);
                b.Dispose();
                g.DrawString(string.Format("{0}", cost), ft, Brushes.White, x + 90+20, y + 32);
                ft.Dispose();

                g.DrawImage(HSIcons.GetIconsByEName("res3"), x + 90, y + 32 -3, 18, 18);
            }
        }
    }
}
