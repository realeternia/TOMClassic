using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using NarlonLib.Control;
using NarlonLib.Core;
using TaleofMonsters.DataType;
using TaleofMonsters.DataType.Cards;
using TaleofMonsters.DataType.Decks;

namespace TaleofMonsters.Controler.Battle.Components
{
    public partial class CardFlow : UserControl
    {
        #region 委托
        delegate void TooltipChangeCallback(ImageToolTip tooltip, bool isOpen, Image img, IWin32Window w, int x, int y);
        private void TooltipChange(ImageToolTip tooltip, bool isOpen, Image img, IWin32Window w, int x, int y)
        {
            if (InvokeRequired)
            {
                TooltipChangeCallback d = TooltipChange;
                Invoke(d, new object[] {tooltip, isOpen, img, w, x, y});
            }
            else
            {
                if (isOpen)
                {
                    tooltip.Show(img, w, x, y);
                }
                else
                {
                    tooltip.Hide(w);
                }
            }
        }
        #endregion

        private class CardInfo
        {
            public int CardId;
            public int Level;
            public bool IsLeft;
            public int X;
            public int Id;
        }

        private const int ItemWidth = 20;

        private List<CardInfo> cardList = new List<CardInfo>();
        private TimeCounter counter = new TimeCounter(0.1f);
        private ImageToolTip tooltip = new ImageToolTip();
        private int mouseIndex = -1;//从1开始，鼠标指向的item

        private static int CardIndex; //分配的item id，每个自增

        private int mouseX;

        public CardFlow()
        {
            InitializeComponent();
            DoubleBuffered = true;
        }
        
        public void NewTick()
        {
            if (counter.OnTick())
            {
                foreach (var cardInfo in cardList)
                {
                    cardInfo.X += 2;
                }
                cardList.RemoveAll(card => card.X >= 900- ItemWidth);
                Invalidate();

                CheckSelect();
            }
        }

        private void CheckSelect()
        {
            int newIndex = -1;
            foreach (var cardInfo in cardList)
            {
                if (mouseX > cardInfo.X && mouseX < cardInfo.X + ItemWidth)
                {
                    newIndex = cardInfo.Id;
                    break;
                }
            }

            if (newIndex != mouseIndex)
            {
                mouseIndex = newIndex;
                TooltipChange(tooltip, false, null, this, 0, 0);
                if (mouseIndex != -1)
                {
                    var targetCard = GetCardInfo(mouseIndex);
                    var card = CardAssistant.GetCard(targetCard.CardId);
                    DeckCard dc = new DeckCard(targetCard.CardId, (byte)targetCard.Level, 0);
                    card.SetData(dc);
                    var image = card.GetPreview(CardPreviewType.Normal, new int[0]);
                    TooltipChange(tooltip, true, image, this, targetCard.X, ItemWidth);
                }
            }
        }

        private CardInfo GetCardInfo(int id)
        {
            return cardList.Find(card => card.Id == id);
        }

        public void OnPlayerUseCard(int cardId, int level, bool isLeft)
        {
            CardInfo ci = new CardInfo
            {
                CardId = cardId,
                Level = level,
                IsLeft = isLeft,
                X = 0,
                Id = CardIndex++
            };
            if (cardList.Count > 0)
                ci.X = Math.Min(0, cardList[cardList.Count - 1].X - ItemWidth);
            cardList.Add(ci);
            Invalidate();
        }

        private void CardFlow_Paint(object sender, PaintEventArgs e)
        {
            foreach (var cardInfo in cardList)
            {
                var img = CardAssistant.GetCardImage(cardInfo.CardId, ItemWidth, ItemWidth);
                e.Graphics.DrawImage(img, cardInfo.X, 2, ItemWidth, ItemWidth);
                Pen p = new Pen(cardInfo.IsLeft ? Brushes.Red : Brushes.Blue, 2);
                e.Graphics.DrawRectangle(p, cardInfo.X, 2, ItemWidth, ItemWidth);
                p.Dispose();
            }
        }

        private void CardFlow_MouseMove(object sender, MouseEventArgs e)
        {
            mouseX = e.X;
        }

        private void CardFlow_MouseLeave(object sender, EventArgs e)
        {
            mouseX = -1;
        }
    }
}
