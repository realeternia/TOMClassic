using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using TaleofMonsters.Forms.Items.Core;
using ConfigDatas;
using ControlPlus;
using TaleofMonsters.Core;
using TaleofMonsters.Core.Config;
using TaleofMonsters.Core.Loader;
using TaleofMonsters.Datas;
using TaleofMonsters.Datas.CardPieces;
using TaleofMonsters.Datas.Items;
using TaleofMonsters.Datas.User;

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
        private int tar = -1;
        private List<int> items;
        private CardDetail cardDetail;
        private NLSelectPanel selectPanel;
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

            cardDetail = new CardDetail(this, cardWidth * xCount + 65, 35, cardHeight * yCount + 93);
            nlClickLabel1.Location = new Point(75, cardHeight * yCount + 100);
            nlClickLabel1.Size = new Size(cardWidth * xCount - 20, 23);

            selectPanel = new NLSelectPanel(65, 35, xCount * cardWidth, yCount * cardHeight, this);
            selectPanel.ItemsPerRow = xCount;
            selectPanel.ItemHeight = cardHeight;
            selectPanel.DrawCell += SelectPanel_DrawCell;
            selectPanel.SelectIndexChanged += SelectPanel_SelectIndexChanged;
        }

        public override void Init(int width, int height)
        {
            base.Init(width, height);
            page = 0;

            items = new List<int>();
            foreach (HItemConfig itemConfig in ConfigData.HItemDict.Values)
            {
                if (itemConfig.Type == (int)HItemTypes.Material)
                    items.Add(itemConfig.Id);
            }
            totalCount = items.Count;

            UpdateButtonState();
            InitItems();
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
            cardDetail.SetInfo(-1);
            UpdateButtonState();
            InitItems();
            Invalidate(new Rectangle(65, 35, cardWidth*xCount + 200, 630));
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
            cardDetail.SetInfo(-1);
            UpdateButtonState();
            InitItems();
            Invalidate(new Rectangle(65, 35, cardWidth*xCount + 200, 630));
        }

        private void InitItems()
        {
            int pages = totalCount / cardCount + 1;
            int cardLimit = (page < pages - 1) ? cardCount : (totalCount % cardCount);
            int former = cardCount * page + 1;
            var datas = new List<int>();
            for (int i = former - 1; i < former + cardLimit - 1; i++)
                datas.Add(items[i]);
            selectPanel.AddContent(datas);
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

        private void SelectPanel_SelectIndexChanged()
        {
            tar = selectPanel.SelectIndex + page * cardCount;

            if (tar != -1)
            {
                HItemConfig itemConfig = ConfigData.GetHItemConfig(items[tar]);
                const string stars = "★★★★★★★★★★";
                var itemHas = UserProfile.InfoBag.GetItemCount(itemConfig.Id);
                itemDesStr = string.Format("{0}({2}){3}{1}", itemConfig.Name, itemConfig.Descript, stars.Substring(10 - itemConfig.Rare),
                    itemHas > 0 ? string.Format("(拥有{0})", itemHas) : "");

                nlClickLabel1.ClearLabel();
                int[] cardIds = CardPieceBook.GetCardIdsByItemId(items[tar]);
                foreach (int cid in cardIds)
                {
                    var cardConfig = CardConfigManager.GetCardConfig(cid);
                    var colorName = HSTypes.I2QualityColor((int)cardConfig.Quality);
                    if (colorName == "White")
                        colorName = "DarkGray";
                    nlClickLabel1.AddLabel(cardConfig.Name, cid, Color.FromName(colorName));
                }
                nlClickLabel1.Invalidate();
                Invalidate(new Rectangle(65 + 5, cardHeight * yCount + 37 + 21, cardWidth * xCount, 30));
            }
        }

        private void SelectPanel_DrawCell(Graphics g, int info, int xOff, int yOff, bool inMouseOn, bool isTarget, bool onlyBorder)
        {
            if (!onlyBorder)
            {
                g.DrawImage(HItemBook.GetHItemImage(info), xOff, yOff, cardWidth, cardHeight);
                if (UserProfile.InfoBag.GetItemCount(info) <= 0)
                {//没有获得卡牌标黑
                    var brush = new SolidBrush(Color.FromArgb(150, Color.Black));
                    g.FillRectangle(brush, xOff, yOff, cardWidth, cardHeight);
                    brush.Dispose();
                }
                var itemConfig = ConfigData.GetHItemConfig(info);
                var pen = new Pen(Color.FromName(HSTypes.I2RareColor(itemConfig.Rare)), 2);
                g.DrawRectangle(pen, xOff, yOff, cardWidth - 2, cardHeight - 2);
                pen.Dispose();
            }

            if (inMouseOn || isTarget)
            {
                Color borderColor = isTarget ? Color.Lime : Color.Yellow;
                SolidBrush yellowbrush = new SolidBrush(Color.FromArgb(80, borderColor));
                g.FillRectangle(yellowbrush, xOff, yOff, cardWidth, cardHeight);
                yellowbrush.Dispose();

                Pen yellowpen = new Pen(borderColor, 3);
                g.DrawRectangle(yellowpen, xOff, yOff, cardWidth-3, cardHeight-3);
                yellowpen.Dispose();
            }
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
        }
    }
}