using System;
using System.Drawing;
using System.Windows.Forms;
using ConfigDatas;
using ControlPlus;
using TaleofMonsters.Core;
using TaleofMonsters.Core.Loader;
using TaleofMonsters.Datas.Items;
using TaleofMonsters.Datas.Others;
using TaleofMonsters.Datas.User;
using TaleofMonsters.Forms.CMain;
using TaleofMonsters.Forms.Items.Regions;

namespace TaleofMonsters.Forms.Items
{
    internal class PieceItem : IDisposable
    {
        private int index;
        private BuyPieceForm.BuyPieceData pieceData;
        private int price;
        private bool show;
        private ImageToolTip tooltip = SystemToolTip.Instance;
        private VirtualRegion vRegion;

        private int x, y, width, height;
        private BasePanel parent;
        private BitmapButton bitmapButtonBuy;
        private Color backColor;

        public PieceItem(BasePanel prt, int x, int y, int width, int height)
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
            vRegion.AddRegion(new PictureAnimRegion(1, x + 5, y + 8, 40, 40, PictureRegionCellType.Item, 0));
            vRegion.RegionEntered += new VirtualRegion.VRegionEnteredEventHandler(virtualRegion_RegionEntered);
            vRegion.RegionLeft += new VirtualRegion.VRegionLeftEventHandler(virtualRegion_RegionLeft);
        }

        public void RefreshData(BuyPieceForm.BuyPieceData pieceD)
        {
            pieceData = pieceD;
            if (!pieceD.IsEmpty())
            {
                bitmapButtonBuy.Visible = !pieceData.Used;
                vRegion.SetRegionKey(1, pieceData.Id);
                var itmConfig = ConfigData.GetHItemConfig(pieceData.Id);
                price = (int)GameResourceBook.OutGoldSellItem(itmConfig.Rare, itmConfig.ValueFactor) * pieceData.Count * 2;//素材价格x2
                show = true;
            }
            else
            {
                pieceData.Id = 0;
                vRegion.SetRegionKey(1, 0);
                bitmapButtonBuy.Visible = false;
                show = false;
            }

            parent.Invalidate(new Rectangle(x, y, width, height));
        }


        private void virtualRegion_RegionEntered(int info, int mx, int my, int key)
        {
            if (pieceData.Id > 0)
            {
                Image image = HItemBook.GetPreview(pieceData.Id);
                tooltip.Show(image, parent, mx, my, pieceData.Id);
            }
        }

        private void virtualRegion_RegionLeft()
        {
            tooltip.Hide(parent, pieceData.Id);
        }

        private void pictureBoxBuy_Click(object sender, EventArgs e)
        {
            if (UserProfile.InfoBag.Resource.Gold >= price)
            {
                UserProfile.InfoBag.Resource.Gold = (uint) (UserProfile.InfoBag.Resource.Gold - price);
                UserProfile.InfoBag.AddItem(pieceData.Id, pieceData.Count);
                pieceData.Used = true;
                parent.RefreshInfo();

                parent.AddFlowCenter("+1", "Lime", HItemBook.GetHItemImage(pieceData.Id));
            }
            else
            {
                parent.AddFlowCenter("金钱不足", "Red");
            }
        }

        public void Draw(Graphics g)
        {
            SolidBrush sb = new SolidBrush(backColor);
            g.FillRectangle(sb, x, y, width, height);
            sb.Dispose();
            g.DrawRectangle(Pens.White, x, y, width - 1, height - 1);

            if (show)
            {
                HItemConfig itemConfig = ConfigData.GetHItemConfig(pieceData.Id);

                Font font = new Font("微软雅黑", 10*1.33f, FontStyle.Regular, GraphicsUnit.Pixel);
                Brush brush = new SolidBrush(Color.FromName(HSTypes.I2RareColor(itemConfig.Rare)));
                g.DrawString(itemConfig.Name, font, brush, x + 57, y + 7);
                brush.Dispose();

                g.DrawString(price.ToString(), font, Brushes.Gold, x+57, y+30);
                var wid = TextRenderer.MeasureText(g, price.ToString(), font, new Size(0, 0), TextFormatFlags.NoPadding).Width;
                g.DrawImage(HSIcons.GetIconsByEName("res1"), wid + 57+x, y+32, 16, 16);

                vRegion.Draw(g);
                g.DrawString(pieceData.Count.ToString(), font, Brushes.Black, x+30, y+29);
                g.DrawString(pieceData.Count.ToString(), font, Brushes.White, x+29, y+28);

                font.Dispose();

                if (!bitmapButtonBuy.Visible)
                {
                    font = new Font("微软雅黑", 10*1.33f, FontStyle.Regular, GraphicsUnit.Pixel);
                    g.DrawString("完成", font, Brushes.LightGreen, x + 152, y + 30);
                    font.Dispose();
                }
            }
        }
        public void Dispose()
        {
        }
    }
}
