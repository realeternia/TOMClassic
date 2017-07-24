using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using NarlonLib.Control;
using NarlonLib.Core;
using NarlonLib.Drawing;
using NarlonLib.Tools;
using TaleofMonsters.Core;
using TaleofMonsters.DataType;
using TaleofMonsters.DataType.Cards;
using TaleofMonsters.DataType.Cards.Spells;
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

        private interface IFlowItem
        {
            int Id { get; set; }
            int X { get; set; }
            Image ShowTip();
            void Draw(Graphics g);
        }

        #region 多态
        private class CardInfo : IFlowItem
        {
            public int CardId { get; set; }
            public int Level { get; set; }
            public bool IsLeft { get; set; }
            public int X { get; set; }
            public int Id { get; set; }

            public Image ShowTip()
            {
                var card = CardAssistant.GetCard(CardId);
                DeckCard dc = new DeckCard(CardId, (byte)Level, 0);
                card.SetData(dc);
                return card.GetPreview(CardPreviewType.Normal, new int[0]);
            }

            public virtual void Draw(Graphics g)
            {
                var img = CardAssistant.GetCardImage(CardId, ItemWidth, ItemWidth);
               g.DrawImage(img, X, 2, ItemWidth, ItemWidth);
                Pen p = new Pen(IsLeft ? Brushes.Red : Brushes.Blue, 1);
               g.DrawRectangle(p, X, 2, ItemWidth, ItemWidth);
                p.Dispose();
            }
        }

        private class TrapUseInfo : CardInfo
        {
            public override void Draw(Graphics g)
            {
                var img = HSIcons.GetIconsByEName("rot2");
                g.DrawImage(img, X, 2, ItemWidth, ItemWidth);
                Pen p = new Pen(IsLeft ? Brushes.Red : Brushes.Blue, 1);
                g.DrawRectangle(p, X, 2, ItemWidth, ItemWidth);
                p.Dispose();
            }
        }
        private class MonsterDieInfo : CardInfo
        {
            public override void Draw(Graphics g)
            {
                var img = HSIcons.GetIconsByEName("oth11");
                g.DrawImage(img, X, 2, ItemWidth, ItemWidth);
                Pen p = new Pen(IsLeft ? Brushes.Red : Brushes.Blue, 1);
                g.DrawRectangle(p, X, 2, ItemWidth, ItemWidth);
                p.Dispose();
            }
        }

        private class TrapItem : IFlowItem
        {
            public bool IsLeft { get; set; }
            public int X { get; set; }
            public int Id { get; set; }

            public Image ShowTip()
            {
                return DrawTool.GetImageByString("这是一个陷阱!!", 100);
            }

            public void Draw(Graphics g)
            {
                var img = HSIcons.GetIconsByEName("rot9");
                g.DrawImage(img, X, 2, ItemWidth, ItemWidth);
                Pen p = new Pen(IsLeft ? Brushes.Red : Brushes.Blue, 1);
                g.DrawRectangle(p, X, 2, ItemWidth, ItemWidth);
                p.Dispose();
            }
        }
        #endregion

        private const int ItemWidth = 20;

        private List<IFlowItem> cardList = new List<IFlowItem>();
        private TimeCounter counter = new TimeCounter(0.1f);
        private ImageToolTip tooltip = new ImageToolTip();
        private int mouseIndex = -1;//从1开始，鼠标指向的item

        private static int CardIndex; //分配的item id，每个自增

        private int mouseX = -1;

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
            int newIndex = -1; //默认指向不存在
            if (mouseX != -1)
            {
                foreach (var cardInfo in cardList)
                {
                    if (mouseX > cardInfo.X && mouseX < cardInfo.X + ItemWidth)
                    {
                        newIndex = cardInfo.Id;
                        break;
                    }
                }
            }

            if (newIndex != mouseIndex)
            {
                mouseIndex = newIndex;
                TooltipChange(tooltip, false, null, this, 0, 0);
                if (mouseIndex != -1)
                {
                    var targetCard = GetCardInfo(mouseIndex);
                    var image = targetCard.ShowTip();
                    TooltipChange(tooltip, true, image, this, targetCard.X, ItemWidth);
                }
            }
        }

        private IFlowItem GetCardInfo(int id)
        {
            return cardList.Find(card => card.Id == id);
        }

        #region  事件捕获
        public void OnPlayerUseCard(int cardId, int level, bool isLeft)
        {
            IFlowItem item = null;
            if (SpellBook.IsTrap(cardId))
            {
                item = new TrapItem
                {
                    IsLeft = isLeft,
                    X = 0,
                    Id = CardIndex++
                };

            }
            else
            {
                item = new CardInfo
                {
                    CardId = cardId,
                    Level = level,
                    IsLeft = isLeft,
                    X = 0,
                    Id = CardIndex++
                };
            }
            if (cardList.Count > 0)
                item.X = Math.Min(0, cardList[cardList.Count - 1].X - ItemWidth);
            cardList.Add(item);
            Invalidate();
        }

        public void OnPlayerTrapTriggered(int cardId, int level, bool isLeft)
        {
            var item = new TrapUseInfo
            {
                CardId = cardId,
                Level = level,
                IsLeft = isLeft,
                X = 0,
                Id = CardIndex++
            };
            if (cardList.Count > 0)
                item.X = Math.Min(0, cardList[cardList.Count - 1].X - ItemWidth);
            cardList.Add(item);
            Invalidate();
        }

        public void OnPlayerKillMonster(int cardId, int level, bool isLeft)
        {
            var item = new MonsterDieInfo
            {
                CardId = cardId,
                Level = level,
                IsLeft = !isLeft, //kill和死是对立的玩家
                X = 0,
                Id = CardIndex++
            };
            if (cardList.Count > 0)
                item.X = Math.Min(0, cardList[cardList.Count - 1].X - ItemWidth);
            cardList.Add(item);
            Invalidate();
        }
#endregion

        private void CardFlow_Paint(object sender, PaintEventArgs e)
        {
            foreach (var cardInfo in cardList)
            {
                cardInfo.Draw(e.Graphics);
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
