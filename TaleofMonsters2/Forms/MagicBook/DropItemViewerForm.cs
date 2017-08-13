using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using TaleofMonsters.Controler.Loader;
using TaleofMonsters.DataType.CardPieces;
using TaleofMonsters.DataType.Items;
using TaleofMonsters.Forms.Items.Core;
using ConfigDatas;
using TaleofMonsters.Config;
using TaleofMonsters.Core;
using TaleofMonsters.DataType;

namespace TaleofMonsters.Forms.MagicBook
{
    internal sealed partial class DropItemViewerForm : BasePanel
    {
        private const int cardWidth = 50;
        private const int cardHeight = 50;
        private const int xCount = 10;
        private const int yCount = 9;
        private const int cardCount = xCount * yCount;
        private int totalCount;
        private int page;
        private bool show;
        private int tar = -1;
        private int sel = -1;
        private List<int> items;
        private Bitmap tempImage;
        private bool isDirty = true;
        private CardDetail cardDetail;
        private string itemDesStr = ""; //显示在下方的道具说明

        public DropItemViewerForm()
        {
            InitializeComponent();
            this.bitmapButtonClose.ImageNormal = PicLoader.Read("Button.Panel", "CloseButton1.JPG");
            bitmapButtonClose.NoUseDrawNine = true;
            this.bitmapButtonNext.ImageNormal = PicLoader.Read("Button.Panel", "NextButton.JPG");
            bitmapButtonNext.NoUseDrawNine = true;
            this.bitmapButtonPre.ImageNormal = PicLoader.Read("Button.Panel", "PreButton.JPG");
            bitmapButtonPre.NoUseDrawNine = true;
            tempImage = new Bitmap(cardWidth*xCount, cardHeight*yCount);

            cardDetail = new CardDetail(this, cardWidth * xCount + 65, 35, cardHeight * yCount + 93);
            nlClickLabel1.Location = new Point(75, cardHeight * yCount + 100);
            nlClickLabel1.Size = new Size(cardWidth * xCount - 20, 23);
        }

        public override void Init(int width, int height)
        {
            base.Init(width, height);
            show = true;
            page = 0;

            items = new List<int>();
            foreach (HItemConfig itemConfig in ConfigData.HItemDict.Values)
            {
                if (itemConfig.Type == (int)HItemTypes.Material)
                    items.Add(itemConfig.Id);
            }
            totalCount = items.Count;

            UpdateButtonState();
        }
        public override void OnFrame(int tick, float timePass)
        {
            cardDetail.OnFrame();
        }

        private void UpdateButtonState()
        {
            bitmapButtonPre.Enabled = page > 0;
            bitmapButtonNext.Enabled = (page + 1) * cardCount < totalCount;
        }

        private void buttonPre_Click(object sender, EventArgs e)
        {
            page--;
            if (page < 0)
            {
                page++;
                return;
            }
            tar = -1;
            sel = -1;
            cardDetail.SetInfo(-1);
            UpdateButtonState();
            isDirty = true;
            Invalidate(new Rectangle(65, 35, cardWidth * xCount+200, 630));
        }

        private void buttonNext_Click(object sender, EventArgs e)
        {
            page++;
            if (page > (totalCount - 1) / cardCount)
            {
                page--;
                return;
            }
            tar = -1;
            sel = -1;
            cardDetail.SetInfo(-1);
            UpdateButtonState();
            isDirty = true;
            Invalidate(new Rectangle(65, 35, cardWidth * xCount+200, 630));
        }

        private void DropItemViewerForm_Click(object sender, EventArgs e)
        {
            if (tar != -1)
            {
                sel = tar;

                HItemConfig itemConfig = ConfigData.GetHItemConfig(items[tar]);
                const string stars = "★★★★★★★★★★";
                itemDesStr = string.Format("{0}({2}){1}", itemConfig.Name, itemConfig.Descript, stars.Substring(10 - itemConfig.Rare));
                Invalidate(new Rectangle(65, cardHeight * yCount + 37 + 21, 500, 20));

                nlClickLabel1.ClearLabel();
                int[] cardIds = CardPieceBook.GetCardIdsByItemId(items[tar]);
                foreach (int cid in cardIds)
                {
                    var name = CardConfigManager.GetCardConfig(cid).Name;
                    nlClickLabel1.AddLabel(name, cid);
                }
                nlClickLabel1.Invalidate();
            }
        }

        private void DropItemViewerForm_MouseMove(object sender, MouseEventArgs e)
        {
            int truex = e.X - 65;
            int truey = e.Y - 35;
            if (truex > 0 && truex < xCount * cardWidth && truey > 0 && truey < yCount * cardHeight)
            {
                int temp = truex / cardWidth + truey / cardHeight * xCount + cardCount * page;
                if (temp != tar)
                {
                    tar = temp < totalCount ? temp : -1;
                    Invalidate(new Rectangle(65, 35, cardWidth * xCount, cardHeight * yCount));
                }
            }
            else
            {
                if (tar != -1)
                {
                    tar = -1;
                    Invalidate(new Rectangle(65, 35, cardWidth * xCount, cardHeight * yCount));
                }
            }
        }

        private void nlClickLabel1_SelectionChange(Object value)
        {
            cardDetail.SetInfo((int)value);
            Invalidate(new Rectangle(65, 35, cardWidth * xCount + 200, 630));
        }

        private void buttonClose_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void DropItemViewerForm_Paint(object sender, PaintEventArgs e)
        {
            BorderPainter.Draw(e.Graphics, "", Width, Height);

            Font font = new Font("黑体", 12*1.33f, FontStyle.Bold, GraphicsUnit.Pixel);
            e.Graphics.DrawString(" 全材料 ", font, Brushes.White, 320, 8);
            font.Dispose();
            Font fontblack = new Font("黑体", 12 * 1.33f, FontStyle.Bold, GraphicsUnit.Pixel);
            font = new Font("宋体", 10 * 1.33f, FontStyle.Regular, GraphicsUnit.Pixel);
            e.Graphics.FillRectangle(new SolidBrush(Color.FromArgb(245, 244, 242)), 65, cardHeight * yCount + 35, cardWidth * xCount, 93);
            e.Graphics.FillRectangle(new SolidBrush(Color.FromArgb(190, 175, 160)), 65, cardHeight * yCount + 35, cardWidth * xCount, 20);
            e.Graphics.FillRectangle(new SolidBrush(Color.FromArgb(190, 175, 160)), 65, cardHeight * yCount + 75, cardWidth * xCount, 20);
            e.Graphics.DrawString("说明", fontblack, Brushes.White, 65, cardHeight * yCount + 37);
            if (!string.IsNullOrEmpty(itemDesStr))
            {
                e.Graphics.DrawString(itemDesStr, font, Brushes.SaddleBrown, 65 + 5, cardHeight * yCount + 37 + 21);
            }
            e.Graphics.DrawString("掉落怪物", fontblack, Brushes.White, 65, cardHeight * yCount + 77);
            font.Dispose();
            fontblack.Dispose();

            cardDetail.Draw(e.Graphics);

            if (show)
            {
                int pages = totalCount / cardCount + 1;
                int cardLimit = (page < pages - 1) ? cardCount : (totalCount % cardCount);
                int former = cardCount * page + 1;
                if (isDirty)
                {
                    tempImage.Dispose();
                    tempImage = new Bitmap(cardWidth * xCount, cardHeight * yCount);
                    Graphics g = Graphics.FromImage(tempImage);
                    for (int i = former - 1; i < former + cardLimit - 1; i++)
                    {
                        g.DrawImage(HItemBook.GetHItemImage(items[i]), (i % xCount) * cardWidth, ((i / xCount) % yCount) * cardHeight, cardWidth, cardHeight);
                        var itemConfig = ConfigData.GetHItemConfig(items[i]);
                        var pen = new Pen(Color.FromName(HSTypes.I2RareColor(itemConfig.Rare)), 2);
                        g.DrawRectangle(pen, (i % xCount) * cardWidth, ((i / xCount) % yCount) * cardHeight, cardWidth-2, cardHeight-2);
                        pen.Dispose();
                    }
                    g.Dispose();
                    isDirty = false;
                }
                e.Graphics.DrawImage(tempImage, 65, 35);

                if (sel != -1 && sel < totalCount)
                {
                    SolidBrush yellowbrush = new SolidBrush(Color.FromArgb(80, Color.Lime));
                    int x = (sel % xCount) * cardWidth + 65;
                    int y = ((sel / xCount) % yCount) * cardHeight + 35;
                    e.Graphics.FillRectangle(yellowbrush, x, y, cardWidth, cardHeight);
                    yellowbrush.Dispose();

                    Pen yellowpen = new Pen(Brushes.Lime, 3);
                    e.Graphics.DrawRectangle(yellowpen, x, y, cardWidth, cardHeight);
                    yellowpen.Dispose();
                }
                if (tar != -1 && tar < totalCount)
                {
                    int x = (tar % xCount) * cardWidth + 65;
                    int y = ((tar / xCount) % yCount) * cardHeight + 35;
                    SolidBrush yellowbrush = new SolidBrush(Color.FromArgb(80, Color.Yellow));
                    e.Graphics.FillRectangle(yellowbrush, x, y, cardWidth, cardHeight);
                    yellowbrush.Dispose();

                    Pen yellowpen = new Pen(Brushes.Yellow, 3);
                    e.Graphics.DrawRectangle(yellowpen, x, y, cardWidth, cardHeight);
                    yellowpen.Dispose();
                }
            }
        }
    }
}