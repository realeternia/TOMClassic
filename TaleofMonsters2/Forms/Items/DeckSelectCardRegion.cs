using System;
using System.Collections.Generic;
using System.Drawing;
using TaleofMonsters.Config;
using TaleofMonsters.Core;
using TaleofMonsters.DataType.Cards;
using TaleofMonsters.DataType.Decks;

namespace TaleofMonsters.Forms.Items
{
    internal class DeckSelectCardRegion
    {
        internal class CompareDeckCardByStar : IComparer<DeckCard>
        {
            #region IComparer<CardDescript> 成员

            public int Compare(DeckCard cx, DeckCard cy)
            {
                if (cx.BaseId <= 0 && cy.BaseId <= 0)
                {
                    return 0;
                }
                if (cx.BaseId <= 0)
                {
                    return 1;
                }
                if (cy.BaseId <= 0)
                {
                    return -1;
                }

                var cardX = CardConfigManager.GetCardConfig(cx.BaseId);
                var cardY = CardConfigManager.GetCardConfig(cy.BaseId);
                if (cardX.Star != cardY.Star)
                {
                    return cardX.Star.CompareTo(cardY.Star);
                }

                if (cardX.Quality != cardY.Quality)
                {
                    return cardX.Quality.CompareTo(cardY.Quality);
                }

                return (cx.BaseId.CompareTo(cy.BaseId));
            }

            #endregion
        }

        public int X { get; private set; }
        public int Y { get; private set; }
        public int Width { get; private set; }
        public int Height { get; private set; }
        public DeckCardRegion.InvalidateRegion Invalidate;

        private int tar = -1;
        private DeckCard[] dcards;
        private const int cellHeight = 16;

        public DeckSelectCardRegion(int x, int y, int width, int height)
        {
            X = x;
            Y = y;
            Width = width;
            Height = height;
        }

        public void ChangeDeck(DeckCard[] decks)
        {
            dcards = decks;
            Array.Sort(dcards, new CompareDeckCardByStar());
        }

        public DeckCard GetTargetCard()
        {
            if (tar >= 0 && tar < dcards.Length)
            {
                return dcards[tar];
            }
            return null;
        }

        public void CheckMouseMove(int mousex, int mousey)
        {
            int truex = mousex - X;
            int truey = mousey - Y;
            if (truex > 0 && truex < Width && truey > 0 && truey < Height)
            {
                int temp = truey / cellHeight;
                if (temp != tar)
                {
                    if (temp < dcards.Length)
                    {
                        tar = temp;
                    }
                    else
                    {
                        tar = -1;
                    }
                    if (Invalidate != null)
                    {
                        Invalidate();
                    }
                }
            }
            else
            {
                if (tar != -1)
                {
                    tar = -1;
                    if (Invalidate != null)
                    {
                        Invalidate();
                    }
                }
            }
        }

        public void Draw(Graphics eg)
        {
            eg.DrawRectangle(Pens.White,X,Y,Width,Height);

            Font fontsong = new Font("宋体", 9*1.33f, FontStyle.Regular, GraphicsUnit.Pixel);
            Font fontBold = new Font("宋体", 10*1.33f, FontStyle.Bold, GraphicsUnit.Pixel);
            for (int i = 0; i < dcards.Length; i++)
            {
                int yoff = i * cellHeight;
                if (dcards[i].BaseId <= 0)
                {//如果没有卡，就画一圈红背景
                    eg.DrawRectangle(Pens.DimGray, X, Y + yoff, Width, cellHeight);
                    continue;
                }
                if (tar == i)
                {
                    eg.FillRectangle(Brushes.DarkBlue, X, Y + yoff, Width, cellHeight);
                }
                var cardConfigData = CardConfigManager.GetCardConfig(dcards[i].BaseId);
                var cardImg = CardAssistant.GetCardImage(dcards[i].BaseId, 30, 30);
                eg.DrawImage(cardImg, new Rectangle(X, Y + yoff, 30, cellHeight), 0, 6, 30, cellHeight, GraphicsUnit.Pixel);
                eg.DrawString(cardConfigData.Star.ToString(), fontBold, Brushes.Yellow, X-1, Y + yoff);
                Color color = Color.FromName(HSTypes.I2QualityColor(cardConfigData.Quality));
                Brush colorBrush = new SolidBrush(color);
                var cardName = cardConfigData.Name;
                if (cardName.Length>4)
                {
                    cardName = cardName.Substring(0, 4);
                }
                eg.DrawString(string.Format("{0}({1})", cardName, dcards[i].Level), fontsong, colorBrush, X + 30, Y + yoff);
                colorBrush.Dispose();
            }
            fontsong.Dispose();
            fontBold.Dispose();
        }
    }
}