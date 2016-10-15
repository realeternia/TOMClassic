using System;
using System.Collections.Generic;
using System.Windows.Forms;
using NarlonLib.Control;
using TaleofMonsters.Controler.Battle.Data.MemCard;
using TaleofMonsters.Core.Interface;
using TaleofMonsters.DataType;
using TaleofMonsters.DataType.Cards;

namespace TaleofMonsters.Controler.Battle.Components
{
    internal partial class CardsArray : UserControl, ICardList
    {
        public delegate void CardArrayEventHandler(Object sender, EventArgs e);
        public event CardArrayEventHandler SelectionChange;

        private CardSlot[] cards = new CardSlot[10];
        private int mouseIndex = -1;//从1开始
        private int clickIndex = -1;//从1开始
        private int realCardNum = 0;
        private ImageToolTip tooltip = MainItem.SystemToolTip.Instance;

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
            Enabled = enable;
            foreach (CardSlot cardSlot in cards)
            {
                cardSlot.Enabled = enable;
            }
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
                if (cards[i].ACard.Id != pCards[i].Id)
                    cards[i].SetSlotCard(pCards[i]);
                if (pCards[i].CardId > 0)
                {
                    realCardNum++;
                }
            }

            if (realCardNum <= 6)
            {
                for (int i = 0; i < pCards.Length; i++)
                {
                    cards[i].Size.Width = 120;
                    cards[i].Location.X = 120*i+4*i+4;
                }
            }
            else
            {
                for (int i = 0; i < pCards.Length; i++)
                {
                    cards[i].Size.Width = (754 - (realCardNum + 1) * 4 - 120) / (realCardNum-1);
                    cards[i].Location.X = cards[i].Size.Width * i+4*i+4;
                }
            }
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
                {
                    rcards.Add(cardSlot.ACard);
                }
            }
            return rcards.ToArray();
        }
        #endregion

        private void CardsArray_Paint(object sender, PaintEventArgs e)
        {
            foreach (CardSlot cardSlot in cards)
            {
                if (cardSlot != null)
                {
                    cardSlot.CardSlot_Paint(e); 
                }
            }
        }

        private void CardsArray_MouseMove(object sender, MouseEventArgs e)
        {
            if (!Enabled)
            {
                return;
            }

            int newIndex = -1;
            int xOff = 0;

            for (int i = 0; i < 10; i++)
            {
                if (realCardNum > 6)
                {
                    cards[i].Size.Width = (754 - (realCardNum + 1) * 4 - 120) / (realCardNum - 1);
                    xOff += 4;
                }
                else
                {
                    cards[i].Size.Width = 120;
                    xOff += 4;
                }
                cards[i].Location.X = xOff;
                xOff += cards[i].Size.Width;

                if (e.X > cards[i].Location.X && e.X < cards[i].Location.X + cards[i].Size.Width)
                {
                    if (realCardNum > 6)
                    {
                        xOff += 120 - cards[i].Size.Width;
                    }
                    cards[i].Size.Width = 120;
                }
            }

            for (int i = 0; i < 10; i++)
            {
                cards[i].MouseOn = false;
                if (e.X > cards[i].Location.X && e.X < cards[i].Location.X + cards[i].Size.Width)
                {
                    newIndex = i + 1;
                    cards[i].MouseOn = true;
                }
            }

            if (newIndex != mouseIndex)
            {
                mouseIndex = newIndex;
                tooltip.Hide(this);
                if (mouseIndex != -1 && cards[mouseIndex - 1].ACard.Id != 0)
                {
                    var targetCard = cards[mouseIndex - 1];
                    var card = CardAssistant.GetCard(targetCard.ACard.CardId);
                    card.SetData(targetCard.ACard);
                    var image = card.GetPreview(CardPreviewType.Normal, new int[0]);
                    tooltip.Show(image, this, targetCard.Location.X, targetCard.Location.Y-image.Height-5);
                }
                Invalidate();
            }
        }

        private void CardsArray_MouseLeave(object sender, EventArgs e)
        {
            tooltip.Hide(this);
        }

        private void CardsArray_MouseEnter(object sender, EventArgs e)
        {
            mouseIndex = -1;
        }

        private void CardsArray_MouseClick(object sender, MouseEventArgs e)
        {
            if (!Enabled)
            {
                return;
            }

            if (clickIndex != mouseIndex)
            {
                if (clickIndex > 0)
                {
                    cards[clickIndex-1].IsSelected = false;
                }
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
