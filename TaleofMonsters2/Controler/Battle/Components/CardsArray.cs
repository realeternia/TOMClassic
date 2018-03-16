using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using NarlonLib.Control;
using TaleofMonsters.Controler.Battle.Data.MemCard;
using TaleofMonsters.Core.Interface;
using TaleofMonsters.Datas;
using TaleofMonsters.Datas.Cards;
using TaleofMonsters.Forms.CMain;

namespace TaleofMonsters.Controler.Battle.Components
{
    internal partial class CardsArray : UserControl, ICardList
    {
        public delegate void CardArrayEventHandler(object sender, EventArgs e);
        public event CardArrayEventHandler SelectionChange;

        private readonly CardSlot[] cards = new CardSlot[10];
        private int mouseIndex = -1;//从1开始
        private int clickIndex = -1;//从1开始
        private int realCardNum = 0;
        private ImageToolTip tooltip = SystemToolTip.Instance;

        private Point lastMousePos = new Point(0);
        private bool myEnabled = true;

        public CardsArray()
        {
            InitializeComponent();
        }

        public void Init()
        {
            DoubleBuffered = true;
            for (int i = 1; i <= 10; i++)
            {
                var cardSlot1 = new CardSlot();
                cardSlot1.IsSelected = false;
                cardSlot1.Location = new System.Drawing.Point(120 * i - 120, 0);
                cardSlot1.Size = new System.Drawing.Size(120, 150);

                cardSlot1.SetSlotCard(ActiveCards.NoneCard);
                cards[i - 1] = cardSlot1;
            }
        }

        public void SetEnable(bool enable)
        {
            myEnabled = enable;
            foreach (var cardSlot in cards)
                cardSlot.Enabled = enable;
            Invalidate();
        }

        #region ICardList接口
        public void DisSelectCard()
        {
            if (clickIndex > 0)
                cards[clickIndex - 1].IsSelected = false;
            clickIndex = 0;
            SelectionChange(null, null);
        }

        public void UpdateSlot(ActiveCard[] pCards)
        {
            realCardNum = 0;
            for (int i = 0; i < 10; i++)
            {
                if (cards[i].ACard != pCards[i])
                    cards[i].SetSlotCard(pCards[i]);
                if (pCards[i].CardId > 0)
                    realCardNum++;
            }

            ResizeElements();

            Invalidate();
        }

        public int GetSelectId()
        {
            return clickIndex;
        }

        public ActiveCard GetSelectCard()
        {
            if (clickIndex > 0)
                return cards[clickIndex - 1].ACard;
            return null;
        }

        public void SetSelectId(int value)
        {
            clickIndex = value;
        }

        public int GetCapacity()
        {
            return cards.Length;
        }

        public ActiveCard[] GetAllCard()
        {
            List<ActiveCard> rcards = new List<ActiveCard>();
            foreach (var cardSlot in cards)
            {
                if (cardSlot.ACard.CardId > 0)
                    rcards.Add(cardSlot.ACard);
            }
            return rcards.ToArray();
        }
        #endregion

        private void CardsArray_Paint(object sender, PaintEventArgs e)
        {
            foreach (var cardSlot in cards)
            {
                if (cardSlot != null)
                    cardSlot.CardSlot_Paint(e); 
            }
        }

        private void CardsArray_MouseMove(object sender, MouseEventArgs e)
        {
            lastMousePos = e.Location;
            ResizeElements();
            
            int newIndex = -1;
            for (int i = 0; i < 10; i++)
            {
                var targetCard = cards[i];
                targetCard.MouseOn = false;
                if (lastMousePos.X > targetCard.Location.X && lastMousePos.X < targetCard.Location.X + targetCard.Size.Width)
                {
                    newIndex = i + 1;
                    targetCard.MouseOn = true;
                }
            }

            if (newIndex != mouseIndex)
            {
                mouseIndex = newIndex;
                tooltip.Hide(this);
                if (mouseIndex != -1 && cards[mouseIndex - 1].ACard.CardId != 0)
                {
                    var targetCard = cards[mouseIndex - 1];
                    var card = CardAssistant.GetCard(targetCard.ACard.CardId);
                    card.SetData(targetCard.ACard.Card);
                    var image = card.GetPreview(CardPreviewType.Normal, new uint[0]);
                    tooltip.Show(image, this, targetCard.Location.X, targetCard.Location.Y - image.Height - 5);
                }

                Invalidate();
            }
        }

        private void ResizeElements()
        {
            int xOff = 0;
            for (int i = 0; i < 10; i++)
            {
                var targetCard = cards[i];
                if (realCardNum > 6)
                    targetCard.Size.Width = (754 - (realCardNum + 1)*4 - 120)/(realCardNum - 1);
                else
                    targetCard.Size.Width = 120;
                xOff += 4;
                targetCard.Location.X = xOff;
                xOff += targetCard.Size.Width;

                if (mouseIndex == i + 1)
                {
                    if (realCardNum > 6)
                        xOff += 120 - targetCard.Size.Width;
                    targetCard.Size.Width = 120;
                }
            }

        }

        private void CardsArray_MouseLeave(object sender, EventArgs e)
        {
         //   mouseIndex = -1;
            tooltip.Hide(this);
        }

        private void CardsArray_MouseEnter(object sender, EventArgs e)
        {
            mouseIndex = -1;
        }

        private void CardsArray_MouseClick(object sender, MouseEventArgs e)
        {
            if (!myEnabled)
                return;

            if (clickIndex != mouseIndex)
            {
                if (clickIndex > 0)
                    cards[clickIndex - 1].IsSelected = false;
                clickIndex = mouseIndex;
                if (clickIndex > 0)
                {
                    cards[clickIndex - 1].IsSelected = true;
                    SelectionChange(this, e);
                    Invalidate();
                }
            }
        }
    }
}
