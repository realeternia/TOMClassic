using System;
using System.Drawing;
using ControlPlus;
using TaleofMonsters.Core.Loader;
using TaleofMonsters.Datas;
using TaleofMonsters.Datas.Cards;
using TaleofMonsters.Datas.User;
using TaleofMonsters.Forms.CMain;
using TaleofMonsters.Forms.Items.Regions;

namespace TaleofMonsters.Forms.Items
{
    internal class ChangeCardItem : ICellItem
    {
        public int X { get; set; }
        public int Y { get; set; }
        public int Width { get { return 193; } }
        public int Height { get { return 56; } }

        private int index;
        private ChangeCardForm.ChangeCardData changeInfo;
        private bool show;
        private ImageToolTip tooltip = SystemToolTip.Instance;
        private VirtualRegion vRegion;

        private BasePanel parent;
        private BitmapButton bitmapButtonBuy;
        private Color backColor;

        public ChangeCardItem(BasePanel prt)
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
            vRegion.AddRegion(new PictureAnimRegion(1, X + 5, Y + 8, 40, 40, PictureRegionCellType.Card, 0));
            vRegion.AddRegion(new PictureAnimRegion(2, X + 97, Y + 8, 40, 40, PictureRegionCellType.Card, 0));
            vRegion.RegionEntered += new VirtualRegion.VRegionEnteredEventHandler(virtualRegion_RegionEntered);
            vRegion.RegionLeft += new VirtualRegion.VRegionLeftEventHandler(virtualRegion_RegionLeft);

            this.bitmapButtonBuy = new BitmapButton();
            bitmapButtonBuy.Location = new Point(X + 152, Y + 30);
            bitmapButtonBuy.Size = new Size(35, 20);
            this.bitmapButtonBuy.Click += new System.EventHandler(this.pictureBoxBuy_Click);
            this.bitmapButtonBuy.ImageNormal = PicLoader.Read("Button.Panel", "ButtonBack2.PNG");
            bitmapButtonBuy.Font = new Font("宋体", 8 * 1.33f, FontStyle.Regular, GraphicsUnit.Pixel);
            bitmapButtonBuy.ForeColor = Color.White;
            bitmapButtonBuy.IconImage = TaleofMonsters.Core.HSIcons.GetIconsByEName("tsk7");
            bitmapButtonBuy.IconSize = new Size(16, 16);
            bitmapButtonBuy.IconXY = new Point(10, 4);
            parent.Controls.Add(bitmapButtonBuy);
        }

        public void RefreshData(object data)
        {
            var change = (ChangeCardForm.ChangeCardData) data;
            if (!change.IsEmpty())
            {
                changeInfo = change;
                bitmapButtonBuy.Visible = !change.Used;
                bitmapButtonBuy.Enabled = UserProfile.InfoCard.GetCardExp(changeInfo.Id1) > 0;

                vRegion.SetRegionKey(1, changeInfo.Id1);
                vRegion.SetRegionKey(2, changeInfo.Id2);
                show = true;
            }
            else
            {
                changeInfo = new ChangeCardForm.ChangeCardData();
                vRegion.SetRegionKey(1, 0);
                vRegion.SetRegionKey(2, 0);
                bitmapButtonBuy.Visible = false;
                show = false;
            }

            parent.Invalidate(new Rectangle(X, Y, Width, Height));
        }
        
        private void virtualRegion_RegionEntered(int info, int mx, int my, int key)
        {
            if (info == 1 && changeInfo.Id1 != 0)
            {
                Image image = CardAssistant.GetCard(changeInfo.Id1).GetPreview(CardPreviewType.Normal, new uint[] { });
                tooltip.Show(image, parent, mx, my, changeInfo.Id1);
            }
            else if (info == 2 && changeInfo.Id2 != 0)
            {
                Image image = CardAssistant.GetCard(changeInfo.Id2).GetPreview(CardPreviewType.Normal, new uint[] { });
                tooltip.Show(image, parent, mx, my, changeInfo.Id2);
            }
        }

        private void virtualRegion_RegionLeft()
        {
            tooltip.Hide(parent);
        }

        private void pictureBoxBuy_Click(object sender, EventArgs e)
        {
            if (UserProfile.InfoCard.GetCardExp(changeInfo.Id1) <= 0)
                return;

            UserProfile.InfoCard.RemoveCardPiece(changeInfo.Id1);
            UserProfile.InfoCard.AddCard(changeInfo.Id2);

            changeInfo.Used = true;
            parent.RefreshInfo();
        }

        public void Draw(Graphics g)
        {
            SolidBrush sb= new SolidBrush(backColor);
            g.FillRectangle(sb, X, Y, Width, Height);
            sb.Dispose();
            g.DrawRectangle(Pens.White, X, Y, Width - 1, Height - 1);

            if (show)
            {
                Image img = PicLoader.Read("System", "ArrowS.PNG");
                g.DrawImage(img, X + 54, Y + 19, 35, 20);
                img.Dispose();

                if (!bitmapButtonBuy.Visible)
                {
                    Font font = new Font("微软雅黑", 10*1.33f, FontStyle.Regular, GraphicsUnit.Pixel);
                    g.DrawString("完成", font, Brushes.LightGreen, X + 152, Y + 30);
                    font.Dispose();
                }

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
