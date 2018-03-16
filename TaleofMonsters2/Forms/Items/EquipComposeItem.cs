using System;
using System.Drawing;
using ConfigDatas;
using NarlonLib.Control;
using TaleofMonsters.Core;
using TaleofMonsters.Core.Loader;
using TaleofMonsters.Datas;
using TaleofMonsters.Datas.Equips;
using TaleofMonsters.Datas.Items;
using TaleofMonsters.Datas.Others;
using TaleofMonsters.Datas.User;
using TaleofMonsters.Forms.CMain;
using TaleofMonsters.Forms.Items.Regions;

namespace TaleofMonsters.Forms.Items
{
    internal class EquipComposeItem
    {
        private int index;
        private int equipId;
        private bool show;
        private ImageToolTip tooltip = SystemToolTip.Instance;
        private VirtualRegion vRegion;

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
            this.bitmapButtonBuy.ImageNormal = PicLoader.Read("Button.Panel", "ButtonBack2.PNG");
            bitmapButtonBuy.Font = new Font("宋体", 8 * 1.33f, FontStyle.Regular, GraphicsUnit.Pixel);
            bitmapButtonBuy.ForeColor = Color.White;
            bitmapButtonBuy.IconImage = TaleofMonsters.Core.HSIcons.GetIconsByEName("hatt2");
            bitmapButtonBuy.IconSize = new Size(16,16);
            bitmapButtonBuy.IconXY = new Point(4,4);
            bitmapButtonBuy.TextOffX = 8;
            this.bitmapButtonBuy.Text = @"建造";
            parent.Controls.Add(bitmapButtonBuy);
        }

        public void Init(int idx)
        {
            index = idx;

            vRegion = new VirtualRegion(parent);
            vRegion.AddRegion(new PictureRegion(1, x + 3 + 6, y + 3 + 6,  76 - 12, 75 - 12, PictureRegionCellType.Equip, 0));
            vRegion.AddRegion(new PictureRegion(2, x + 80, y + 50, 24,24, PictureRegionCellType.Item, 0));
            vRegion.RegionEntered += new VirtualRegion.VRegionEnteredEventHandler(virtualRegion_RegionEntered);
            vRegion.RegionLeft += new VirtualRegion.VRegionLeftEventHandler(virtualRegion_RegionLeft);
        }

        public void RefreshData(int eid)
        {
            equipId = eid;
            if (eid > 0)
            {
                bitmapButtonBuy.Visible = true;
                vRegion.SetRegionKey(1, eid);
                var equipConfig = ConfigData.GetEquipConfig(equipId);
                vRegion.SetRegionKey(2, equipConfig.ComposeItemId);
                show = true;
            }
            else
            {
                vRegion.SetRegionKey(1, 0);
                vRegion.SetRegionKey(2, 0);
                bitmapButtonBuy.Visible = false;
                show = false;
            }

            parent.Invalidate(new Rectangle(x, y, width, height));
        }

        private void virtualRegion_RegionEntered(int info, int mx, int my, int key)
        {
            if (info == 1 && key > 0)
            {
                Equip equip = new Equip(key);
                Image image = equip.GetPreview();
                tooltip.Show(image, parent, mx, my, equipId);
            }
            else if (info == 2 && key > 0)
            {
                Image image = HItemBook.GetPreview(key);
                tooltip.Show(image, parent, mx, my, equipId);
            }
        }

        private void virtualRegion_RegionLeft()
        {
            tooltip.Hide(parent, equipId);
        }

        private void pictureBoxBuy_Click(object sender, EventArgs e)
        {
            var equipConfig = ConfigData.GetEquipConfig(equipId);

            GameResource need = new GameResource();
            if (equipConfig.ComposeStone > 0)
                need.Add(GameResourceType.Stone, (uint)(GameResourceBook.OutStoneCompose(equipConfig.Quality + 1) * equipConfig.ComposeStone / 100));
            if (equipConfig.ComposeWood > 0)
                need.Add(GameResourceType.Lumber, (uint)(GameResourceBook.OutWoodCompose(equipConfig.Quality + 1) * equipConfig.ComposeWood / 100));

            if (!UserProfile.InfoBag.CheckResource(need.ToArray()))
            {
                parent.AddFlowCenter(HSErrors.GetDescript(ErrorConfig.Indexer.BagNotEnoughResource), "Red");
                return;
            }

            if (equipConfig.ComposeItemId > 0 && UserProfile.InfoBag.GetItemCount(equipConfig.ComposeItemId) <= 0)
            {
                parent.AddFlowCenter(HSErrors.GetDescript(ErrorConfig.Indexer.BagNotEnoughItems), "Red");
                return;
            }
            
            UserProfile.InfoBag.SubResource(need.ToArray());
            UserProfile.InfoBag.DeleteItem(equipConfig.ComposeItemId, 1);
            UserProfile.InfoEquip.AddEquip(equipId, 24*60*3);
            parent.Invalidate();

            parent.AddFlowCenter("建造成功", "Lime");
        }

        public void Draw(Graphics g)
        {
            SolidBrush sb = new SolidBrush(Color.FromArgb(20, 20, 20));
            g.FillRectangle(sb, x + 2, y + 2, width - 4, height - 4);
            sb.Dispose();
            g.DrawRectangle(Pens.Teal, x + 2, y + 2, width - 4, height - 4);

            if (show)
            {
                var back = PicLoader.Read("System", "MapBack.JPG");
                g.DrawImage(back, x + 2, y + 2, width - 4, height - 4);
                back.Dispose();

                vRegion.Draw(g);

                var equipConfig = ConfigData.GetEquipConfig(equipId);
           
                Font ft = new Font("宋体", 9*1.33f, FontStyle.Regular, GraphicsUnit.Pixel);
                Brush b = new SolidBrush(Color.FromName(HSTypes.I2QualityColor(equipConfig.Quality)));
                g.DrawString(equipConfig.Name, ft, Brushes.Black, x + 82 + 1, y + 10 + 1);
                g.DrawString(equipConfig.Name, ft, b, x + 82, y + 10);
                b.Dispose();

                bool costStone = false;
                if (equipConfig.ComposeStone > 0)
                {
                    int xOff = x + 82;
                    var cost = (uint)(GameResourceBook.OutStoneCompose(equipConfig.Quality + 1) * equipConfig.ComposeStone / 100);
                    g.DrawString(cost.ToString(), ft, Brushes.White, xOff + 20, y + 32);

                    g.DrawImage(HSIcons.GetIconsByEName("res3"), xOff, y + 32 - 3, 18, 18);
                    costStone = true;
                }

                if (equipConfig.ComposeWood > 0)
                {
                    int xOff = x + 82;
                    if (costStone)
                        xOff += 46;
                    var cost = (uint)(GameResourceBook.OutWoodCompose(equipConfig.Quality + 1) * equipConfig.ComposeWood / 100);
                    g.DrawString(cost.ToString(), ft, Brushes.White, xOff + 20, y + 32);

                    g.DrawImage(HSIcons.GetIconsByEName("res2"), xOff, y + 32 - 3, 18, 18);
                }

                ft.Dispose();
            }
        }
    }
}
