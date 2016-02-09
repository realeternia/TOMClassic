﻿using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using TaleofMonsters.Config;
using TaleofMonsters.Controler.Loader;
using TaleofMonsters.DataType.Cards;
using TaleofMonsters.Core;
using TaleofMonsters.Forms.Items.Core;
using ConfigDatas;

namespace TaleofMonsters.Forms.MagicBook
{
    internal sealed partial class CardViewForm : BasePanel
    {
        private const int cardWidth = 67;
        private const int cardHeight = 90;
        private int xCount = 8;
        private int yCount = 5;
        private int cardCount;
        private int page;
        private bool show;
        private int tar = -1;
        private List<int> cards;
        private Bitmap tempImage;
        private bool isDirty = true;
        private CardDetail cardDetail;

        private int filterCardCode =-1;
        private int filterLevel=0;
        private int filterQual=-1;
        private string filterRemark="无";

        public CardViewForm()
        {
            InitializeComponent();
            this.bitmapButtonClose.ImageNormal = PicLoader.Read("ButtonBitmap", "CloseButton1.JPG");
            bitmapButtonClose.NoUseDrawNine = true;
            this.bitmapButtonNext.ImageNormal = PicLoader.Read("ButtonBitmap", "NextButton.JPG");
            bitmapButtonNext.NoUseDrawNine = true;
            this.bitmapButtonPre.ImageNormal = PicLoader.Read("ButtonBitmap", "PreButton.JPG");
            bitmapButtonPre.NoUseDrawNine = true;
            tempImage = new Bitmap(cardWidth * xCount, cardHeight*yCount);
        }

        internal override void Init(int width, int height)
        {
            #region 智能修正面板元素数量
            //823,575 x=8,y=5
            int borderX = 823 - cardWidth * xCount;
            int borderY = 575 - cardHeight * yCount;

            xCount = (width - borderX) * 8 / 10 / cardWidth; //0.8作为一个边缘因子
            yCount= (height - borderY) * 9 / 10 / cardHeight;
            cardCount = xCount*yCount;

            Width = xCount * cardWidth + borderX;
            Height = yCount * cardHeight + borderY;
            #endregion

            base.Init(width, height);
            show = true;
            cards = new List<int>();
            comboBoxType.SelectedIndex = 0;
            comboBoxLevel.SelectedIndex = 0;
            comboBoxQual.SelectedIndex = 0;
            comboBoxRemark.SelectedIndex = 0;
            cardDetail = new CardDetail(this, cardWidth * xCount + 65, 35, cardHeight * yCount + 70);
            ChangeCards();
        }

        internal override void OnFrame(int tick)
        {
            cardDetail.OnFrame();
        }

        private void buttonOk_Click(object sender, EventArgs e)
        {
            filterCardCode = HSTypes.Attr2I(comboBoxType.SelectedItem.ToString());
            filterLevel = comboBoxLevel.SelectedIndex;
            filterQual = comboBoxQual.SelectedIndex - 1;
            filterRemark = comboBoxRemark.SelectedItem.ToString();
            ChangeCards();
        }

        private void ChangeCards()
        {
            page = 0;
            cards.Clear();
            tar = -1;
            cardDetail.SetInfo(-1);

            List<CardConfigData> configData = new List<CardConfigData>();
            #region 数据装载
            foreach (var monsterConfig in ConfigData.MonsterDict.Values)
            {
                if (monsterConfig.IsSpecial > 0)
                    continue;
                if (filterCardCode != -1 && monsterConfig.Attr != filterCardCode)
                    continue;
                if (filterLevel != 0 && monsterConfig.Star != filterLevel)
                    continue;
                var cardData = CardConfigManager.GetCardConfig(monsterConfig.Id);
                int cardQual = cardData.Quality;
                if (filterQual != -1 && cardQual != filterQual)
                    continue;
                if (filterRemark != "无" && (string.IsNullOrEmpty(monsterConfig.Remark) || !monsterConfig.Remark.Contains(filterRemark)))
                    continue;
                configData.Add(cardData);
            }
            foreach (var weaponConfig in ConfigData.WeaponDict.Values)
            {
                if (weaponConfig.IsSpecial > 0)
                    continue;
                if (filterCardCode != -1 && weaponConfig.Attr != filterCardCode)
                    continue;
                if (filterLevel != 0 && weaponConfig.Star != filterLevel)
                    continue;
                var cardData = CardConfigManager.GetCardConfig(weaponConfig.Id);
                int cardQual = cardData.Quality;
                if (filterQual != -1 && cardQual != filterQual)
                    continue;
                if (filterRemark != "无" && (string.IsNullOrEmpty(weaponConfig.Remark) || !weaponConfig.Remark.Contains(filterRemark)))
                    continue;
                configData.Add(cardData);
            }
            foreach (var spellConfig in ConfigData.SpellDict.Values)
            {
                if (spellConfig.IsSpecial > 0)
                    continue;
                if (filterCardCode != -1 && spellConfig.Attr != filterCardCode)
                    continue;
                if (filterLevel != 0 && spellConfig.Star != filterLevel)
                    continue;
                var cardData = CardConfigManager.GetCardConfig(spellConfig.Id);
                int cardQual = cardData.Quality;
                if (filterQual != -1 && cardQual != filterQual)
                    continue;
                if (filterRemark != "无" && (string.IsNullOrEmpty(spellConfig.Remark) || !spellConfig.Remark.Contains(filterRemark)))
                    continue;
                configData.Add(cardData);
            }

            #endregion

            configData.Sort(new CompareByCard());
            cards = configData.ConvertAll(card => card.Id);

            UpdateButtonState();

            if (cards.Count > 0)
                cardDetail.SetInfo(cards[0]);
            isDirty = true;
            Invalidate(new Rectangle(65, 35, cardWidth * xCount + 200, 630));
        }

        private void UpdateButtonState()
        {
            bitmapButtonPre.Enabled = page > 0;
            bitmapButtonNext.Enabled = (page + 1) * cardCount < cards.Count;
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
            isDirty = true;
            Invalidate(new Rectangle(65, 110, cardWidth*xCount, cardHeight*yCount));
        }

        private void buttonNext_Click(object sender, EventArgs e)
        {
            page++;
            if (page > (cards.Count - 1) / cardCount)
            {
                page--;
                return;
            }
            tar = -1;
            cardDetail.SetInfo(-1);
            UpdateButtonState();
            isDirty = true;
            Invalidate(new Rectangle(65, 110, cardWidth * xCount, cardHeight * yCount));
        }

        private void buttonClose_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void CardViewForm_MouseMove(object sender, MouseEventArgs e)
        {
            cardDetail.CheckMouseMove(e.X, e.Y);

            int truex = e.X - 65;
            int truey = e.Y - 110;
            if (truex > 0 && truex < xCount * cardWidth && truey > 0 && truey < yCount * cardHeight)
            {
                int temp = truex / cardWidth + truey / cardHeight * xCount + cardCount * page;
                if (temp != tar)
                {
                    if (temp < cards.Count)
                    {
                        tar = temp;
                    }
                    else
                    {
                        tar = -1;
                    }
                    Invalidate(new Rectangle(65, 110, cardWidth * xCount, cardHeight * yCount));
                }
            }
            else
            {
                if (tar != -1)
                {
                    tar = -1;
                    Invalidate(new Rectangle(65, 110, cardWidth * xCount, cardHeight * yCount));
                }
            }
        }

        private void CardViewForm_Click(object sender, EventArgs e)
        {
            if (tar != -1)
            {
                cardDetail.SetInfo(cards[tar]);
                Invalidate(new Rectangle(65 + cardWidth * xCount, 35, 200, 525));
            }
        }

        private void DrawOnCardView(Graphics g, int cid, int x, int y, bool isSelected)
        {
            CardAssistant.DrawBase(g, cid, x, y, cardWidth, cardHeight);
            if (isSelected)
            {
                var brushes = new SolidBrush(Color.FromArgb(130, Color.Yellow));
                g.FillRectangle(brushes, x, y, cardWidth, cardHeight);
                brushes.Dispose();
            }

            var cardConfigData = CardConfigManager.GetCardConfig(cid);
            Font font = new Font("宋体", 5*1.33f, FontStyle.Regular, GraphicsUnit.Pixel);
            g.DrawString(("★★★★★★★★★★").Substring(10 - cardConfigData.Star), font, Brushes.Yellow, x+3, y+3);
            font.Dispose();

            var quality = cardConfigData.Quality + 1;
            g.DrawImage(HSIcons.GetIconsByEName("gem" + quality), x + cardWidth / 2 - 8, y + cardHeight - 20, 16, 16);

//#if DEBUG
//            int off = 0;
//            if (DataType.Peoples.PeopleBook.IsPeopleDropCard(cid))
//            {
//                g.DrawImage(HSIcons.GetIconsByEName("cad1"), x + 5 + (off++ * 12), y + 30, 24, 24);
//            }
//            if (DataType.Tasks.TaskBook.HasCard(cid))
//            {
//                g.DrawImage(HSIcons.GetIconsByEName("cad3"), x + 5 + (off++ * 12), y + 30, 24, 24);
//            }
//            if (DeckBook.HasCard("rookie", cid) || HItemBook.IsGiftHasCard(cid))
//            {
//                g.DrawImage(HSIcons.GetIconsByEName("cad4"), x + 5 + (off * 12), y + 30, 24, 24);
//            }
//#endif
        }

        private void CardViewForm_Paint(object sender, PaintEventArgs e)
        {
            BorderPainter.Draw(e.Graphics, "", Width, Height);

            Font font = new Font("黑体", 12*1.33f, FontStyle.Bold, GraphicsUnit.Pixel);
            e.Graphics.DrawString(" 全卡片 ", font, Brushes.White, Width / 2 - 40, 8);
            font.Dispose();

            cardDetail.Draw(e.Graphics);
            e.Graphics.FillRectangle(Brushes.DarkGray, 67, 37, cardWidth * xCount - 4, 71);
            Image img = PicLoader.Read("System", "SearchBack.JPG");
            e.Graphics.DrawImage(img, 70, 40, cardWidth * xCount - 10, 65);
            img.Dispose();

            font = new Font("宋体", 9 * 1.33f, FontStyle.Regular, GraphicsUnit.Pixel);
            e.Graphics.DrawString("分类", font,Brushes.White ,86,51);
            e.Graphics.DrawString("星级", font, Brushes.White, 231, 51);
            e.Graphics.DrawString("品质", font, Brushes.White, 386, 51);
            e.Graphics.DrawString("过滤", font, Brushes.White, 86, 83);
            font.Dispose();

            if (show)
            {
                int pages = cards.Count / cardCount + 1;
                int cardLimit = (page < pages - 1) ? cardCount : (cards.Count % cardCount);
                int former = cardCount * page + 1;
                if (isDirty)
                {
                    tempImage.Dispose();
                    tempImage = new Bitmap(cardWidth * xCount, cardHeight * yCount);
                    Graphics g = Graphics.FromImage(tempImage);
                    for (int i = former - 1; i < former + cardLimit - 1; i++)
                    {
                        int ri = i % (xCount * yCount);
                        int x = (ri % xCount) * cardWidth;
                        int y = (ri / xCount) * cardHeight;
                        DrawOnCardView(g, cards[i], x, y, false);
                    }
                    g.Dispose();
                    isDirty = false;
                }
                e.Graphics.DrawImage(tempImage, 65, 110);
                if (tar != -1 && tar < cards.Count)
                {
                    int ri = tar % (xCount * yCount);
                    int x = (ri % xCount) * cardWidth + 65;
                    int y = (ri/xCount)*cardHeight + 110;
                    DrawOnCardView(e.Graphics, cards[tar], x, y, true);
                }
            }
        }

        private class CompareByCard : IComparer<CardConfigData>
        {
            #region IComparer<int> 成员

            public int Compare(CardConfigData x, CardConfigData y)
            {
                if (x.Attr != y.Attr)
                {
                    return x.Attr.CompareTo(y.Attr);
                }

                if (x.Star != y.Star)
                {
                    return x.Star.CompareTo(y.Star);
                }

                if (x.Quality != y.Quality)
                {
                    return x.Quality.CompareTo(y.Quality);
                }

                return x.Id.CompareTo(y.Id);
            }

            #endregion
        }
    }

}