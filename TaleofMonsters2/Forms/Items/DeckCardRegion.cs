using System;
using System.Collections.Generic;
using System.Drawing;
using ConfigDatas;
using TaleofMonsters.Core;
using TaleofMonsters.Core.Config;
using TaleofMonsters.Core.Loader;
using TaleofMonsters.Datas.Cards;
using TaleofMonsters.Datas.Decks;
using TaleofMonsters.Datas.User;

namespace TaleofMonsters.Forms.Items
{
    internal class DeckCardRegion : IDisposable
    {
        internal delegate void InvalidateRegion();

        private const int cardWidth = 68;
        private const int cardHeight = 84;
        private int xCount, yCount;
        public int CardCount { get; private set; }
        public int CardTotalCount { get; private set; }

        public int X { get; private set; }
        public int Y { get; private set; }
        public int Width { get; private set; }
        public int Height { get; private set; }
        public InvalidateRegion Invalidate;

        private Bitmap tempImage;
        private bool isDirty = true;
        private Dictionary<int, string> cardAttr = new Dictionary<int, string>();
        public int Page { get; private set; }
        private int tar = -1;
        private DeckCard[] dcards;

        public bool IsDungeonMode { get; set; } //副本卡组显示模式

        public DeckCardRegion(int x, int y, int width, int height)
        {
            xCount = width / cardWidth;
            yCount = height / cardHeight;
            CardCount = xCount * yCount;

            X = x;
            Y = y;
            Width = width;
            Height = height;

            tempImage = new Bitmap(width, height);
        }

        private void RefreshDict()
        {
            cardAttr.Clear();
            if (!IsDungeonMode)
            {
                for (int i = 0; i < GameConstants.DeckCardCount; i++)
                    AddCardAttr(UserProfile.InfoCard.SelectedDeck.GetCardAt(i), "D");
                foreach (int cid in UserProfile.InfoCard.Newcards)
                    AddCardAttr(cid, "N");
            }
        }

        private void AddCardAttr(int cardid, string mark)
        {
            if (cardAttr.ContainsKey(cardid))
                cardAttr[cardid] += mark;
            else
                cardAttr.Add(cardid, mark);
        }

        private string GetCardAttr(int cardid)
        {
            if (!cardAttr.ContainsKey(cardid))
                return "";

            return cardAttr[cardid];
        }

        public void ChangeDeck(DeckCard[] decks)
        {
            Page = 0;
            tar = -1;
            isDirty = true;
            dcards = decks;
            Array.Sort(dcards, new DeckSelectCardRegion.CompareDeckCardByStar());
            CardTotalCount = decks.Length;
        }

        public DeckCard GetTargetCard()
        {
            if (tar >= 0 && tar < dcards.Length)
                return dcards[tar];
            return null;
        }

        public DeckCard GetCard(int cardId)
        {
            foreach (var deckCard in dcards)
            {
                if (deckCard.BaseId == cardId)
                    return deckCard;
            }
            return null;
        }

        public void ClearCard()
        {
            dcards[tar] = new DeckCard(0, 0, 0);
        }

        public void MenuRefresh()
        {
            isDirty = true;
        }

        public bool PrePage()
        {
            Page--;
            if (Page < 0)
            {
                Page++;
                return false;
            }
            tar = Page*CardCount;
            isDirty = true;
            Invalidate();
            return true;
        }

        public bool NextPage()
        {
            Page++;
            if (Page > (dcards.Length - 1) / CardCount)
            {
                Page--;
                return false;
            }
            tar = Page * CardCount;
            isDirty = true;
            Invalidate();
            return true;
        }

        private void DrawOnDeckView(Graphics g, DeckCard card, int x, int y, bool isSelected, string attr)
        {
            CardAssistant.DrawBase(g, card.BaseId, x, y, cardWidth, cardHeight);
            if (isSelected)
            {
                var brushes = new SolidBrush(Color.FromArgb(130, Color.Yellow));
                g.FillRectangle(brushes, x, y, cardWidth, cardHeight);
                brushes.Dispose();
            }

            if (card.BaseId <= 0)
                return;

            var cardConfigData = CardConfigManager.GetCardConfig(card.BaseId);
            Font font = new Font("宋体", 5*1.33f, FontStyle.Regular, GraphicsUnit.Pixel);
            g.DrawString(("★★★★★★★★★★").Substring(10 - cardConfigData.Star), font, Brushes.Yellow, x + 3, y + 3);
            font.Dispose();

            var quality = cardConfigData.Quality + 1;
            g.DrawImage(HSIcons.GetIconsByEName("gem" + (int)quality), x + cardWidth / 2 - 8, y + cardHeight - 20, 16, 16);

            var jobId = cardConfigData.JobId;
            if (jobId > 0)
            {
                var jobConfig = ConfigData.GetJobConfig(jobId);
                Brush brush = new SolidBrush(Color.FromName(jobConfig.Color));
                g.FillRectangle(brush, x + cardWidth - 24, y + 4, 20, 20);
                g.DrawImage(HSIcons.GetIconsByEName("job" + jobConfig.JobIndex), x + cardWidth - 24, y + 4, 20, 20);
                brush.Dispose();
            }

            g.FillEllipse(Brushes.Black, x + 3, y + 4 + cardHeight - 26, 16, 16);
            Font fontsong = new Font("宋体", 9 * 1.33f, FontStyle.Bold, GraphicsUnit.Pixel);
            string text = string.Format("{0:00}", card.Level);
            g.DrawString(text, fontsong, Brushes.Yellow, x + 4, y + 4 + cardHeight - 23);

            if (!IsDungeonMode)
            {
                if (attr.Contains("D"))
                {
                    Image mark = PicLoader.Read("System", "MarkSelect.PNG");
                    g.DrawImage(mark, x + cardWidth - 21, y + cardHeight - 25, 21, 25);
                    if (attr.Contains("DD")) //2张出战
                        g.DrawImage(mark, x + cardWidth - 21, y + cardHeight - 25 - 15, 21, 25);
                    mark.Dispose();
                }
                if (attr.Contains("N"))
                {
                    Image mark = PicLoader.Read("System", "MarkNew.PNG");
                    g.DrawImage(mark, x, y, 30, 30);
                    mark.Dispose();
                }
                if (card.Exp >= ConfigData.GetLevelExpConfig(card.Level).CardExp)//可以升级
                {
                    g.DrawString(text, fontsong, Brushes.Lime, x + 4, y + 4 + cardHeight - 23);
                    Image mark = PicLoader.Read("System", "ArrowU.PNG");
                    g.DrawImage(mark, x + 2, y + 4 + cardHeight - 43, 18, 16);
                    mark.Dispose();
                }
            }
            else if (IsDungeonMode && card.Exp == 99) //特殊处理
            {
                Image mark = PicLoader.Read("System", "MarkDun.PNG");
                g.DrawImage(mark, x + cardWidth - 33, y+1, 30, 30);
                mark.Dispose();
            }
            fontsong.Dispose();
        }

        public void CheckMouseMove(int mousex, int mousey)
        {
            int truex = mousex - X;
            int truey = mousey - Y;
            if (truex > 0 && truex < xCount * cardWidth && truey > 0 && truey < yCount * cardHeight)
            {
                int temp = truex / cardWidth + truey / cardHeight * xCount + CardCount * Page;
                if (temp != tar)
                {
                    if (temp < dcards.Length)
                        tar = temp;
                    else
                        tar = -1;
                    if (Invalidate != null)
                        Invalidate();
                }
            }
            else
            {
                if (tar != -1)
                {
                    tar = -1;
                    if (Invalidate != null)
                        Invalidate();
                }
            }
        }

        public void Draw(Graphics eg)
        {
            int pages = dcards.Length / CardCount + 1;
            int cardLimit = (Page != pages - 1) ? CardCount : (dcards.Length % CardCount);
            int former = CardCount * Page + 1;

            if (isDirty)
            {
                RefreshDict();
                tempImage.Dispose();
                tempImage = new Bitmap(Width, Height);
                Graphics g = Graphics.FromImage(tempImage);
                for (int i = former - 1; i < former + cardLimit - 1; i++)
                {
                    int ri = i % (xCount * yCount);
                    int x = (ri % xCount) * cardWidth;
                    int y = (ri / xCount) * cardHeight;
                    DrawOnDeckView(g, dcards[i], x, y, false, GetCardAttr(dcards[i].BaseId));
                }
                g.Dispose();

                isDirty = false;
            }
            eg.DrawImage(tempImage, X, Y);
            if (tar != -1)
            {
                int ri = tar % (xCount * yCount);
                int x = (ri % xCount) * cardWidth + X;
                int y = (ri / xCount) * cardHeight + Y;
                DrawOnDeckView(eg, dcards[tar], x, y, true, GetCardAttr(dcards[tar].BaseId));
            }
        }
        public void Dispose()
        {
        }
    }
}