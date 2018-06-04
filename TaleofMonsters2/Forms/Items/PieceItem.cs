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
    internal class PieceItem : ICellItem
    {
        public int X { get; set; }
        public int Y { get; set; }
        public int Width { get { return 193; } }
        public int Height { get { return 56; } }

        private int index;
        private BuyPieceForm.BuyPieceData pieceData;
        private int price;
        private bool show;
        private ImageToolTip tooltip = SystemToolTip.Instance;
        private VirtualRegion vRegion;

        private BasePanel parent;
        private BitmapButton bitmapButtonBuy;
        private Color backColor;

        public PieceItem(BasePanel prt)
        {
            parent = prt;
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
            vRegion.AddRegion(new PictureAnimRegion(1, X +  5, Y +  8, 40, 40, PictureRegionCellType.Item, 0));
            vRegion.RegionEntered += new VirtualRegion.VRegionEnteredEventHandler(virtualRegion_RegionEntered);
            vRegion.RegionLeft += new VirtualRegion.VRegionLeftEventHandler(virtualRegion_RegionLeft);

            this.bitmapButtonBuy = new BitmapButton();
            bitmapButtonBuy.Location = new Point(X + 152, Y + 30);
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

        public void RefreshData(object data)
        {
            var pieceD = (BuyPieceForm.BuyPieceData)data;
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

            parent.Invalidate(new Rectangle(X, Y, Width, Height));
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
                parent.AddFlowCenter(HSErrors.GetDescript(ErrorConfig.Indexer.BagNotEnoughGold), "Red");
            }
        }

        public void Draw(Graphics g)
        {
            SolidBrush sb = new SolidBrush(backColor);
            g.FillRectangle(sb, X, Y, Width, Height);
            sb.Dispose();
            g.DrawRectangle(Pens.White, X, Y, Width - 1, Height - 1);

            if (show)
            {
                HItemConfig itemConfig = ConfigData.GetHItemConfig(pieceData.Id);

                Font font = new Font("微软雅黑", 10*1.33f, FontStyle.Regular, GraphicsUnit.Pixel);
                Brush brush = new SolidBrush(Color.FromName(HSTypes.I2RareColor(itemConfig.Rare)));
                g.DrawString(itemConfig.Name, font, brush, X +  57, Y +  7);
                brush.Dispose();

                g.DrawString(price.ToString(), font, Brushes.Gold, X + 57, Y + 30);
                var wid = TextRenderer.MeasureText(g, price.ToString(), font, new Size(0, 0), TextFormatFlags.NoPadding).Width;
                g.DrawImage(HSIcons.GetIconsByEName("res1"), wid + 57 + X, Y + 32, 16, 16);

                vRegion.Draw(g);
                g.DrawString(pieceData.Count.ToString(), font, Brushes.Black, X+30, Y+29);
                g.DrawString(pieceData.Count.ToString(), font, Brushes.White, X+29, Y+28);

                font.Dispose();

                if (!bitmapButtonBuy.Visible)
                {
                    font = new Font("微软雅黑", 10*1.33f, FontStyle.Regular, GraphicsUnit.Pixel);
                    g.DrawString("完成", font, Brushes.LightGreen, X +  152, Y +  30);
                    font.Dispose();
                }
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
