using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using NarlonLib.Control;
using TaleofMonsters.Controler.Battle.Components.CardSelect;
using TaleofMonsters.Controler.Battle.Data.Players;
using TaleofMonsters.Controler.Loader;
using TaleofMonsters.DataType;
using TaleofMonsters.DataType.Cards;
using TaleofMonsters.DataType.Decks;
using TaleofMonsters.Forms.Items.Regions;

namespace TaleofMonsters.Controler.Battle.Components
{
    internal partial class CardSelector : UserControl
    {
        internal delegate void CardSelectoreEventHandler();
        public CardSelectoreEventHandler StartClicked;

        private List<CardSlot> cards;
        private VirtualRegion region;
        private ICardSelectMethod selectMethod;
        private ImageToolTip tooltip = MainItem.SystemToolTip.Instance;

        private bool canClick;

        public CardSelector()
        {
            InitializeComponent();

            region = new VirtualRegion(this);
            region.RegionClicked+=region_RegionClicked;
            region.RegionEntered += Region_RegionEntered;
            region.RegionLeft += Region_RegionLeft;

            bitmapButton1.ImageNormal = PicLoader.Read("Button.Panel", "ButtonBack2.PNG");
        }


        public void Init(Player p, ICardSelectMethod method)
        {
            canClick = true;
            selectMethod = method;
            selectMethod.Selector = this;
            selectMethod.Init(p);
            BackColor = Color.FromArgb(100, Color.Black);

            region.ClearRegion();
            var cardCount = method.GetCards().Length;
            var margin = (Width - 2 * 150 - 120) / (cardCount - 1);
            for (int i = 0; i < cardCount; i++)
            {
                region.AddRegion(new ButtonRegion(i + 1, margin * i + 150, 100, 120, 120, "ErrorButton.PNG", "ErrorButton.PNG"));
                SetRegionVisible(i + 1, false);
            }

            UpdateCards();
        }

        public void SetRegionVisible(int id, bool visible)
        {
            if (region != null)
            {
                region.SetRegionVisible(id, visible);
            }
        }

        private void region_RegionClicked(int id, int x, int y, MouseButtons button)
        {
            if (region != null && canClick)
            {
                selectMethod.RegionClicked(id);
                Invalidate();
            }
        }

        private void Region_RegionEntered(int id, int x, int y, int key)
        {
            if (cards.Count <= id - 1)
                return;

            var card = CardAssistant.GetCard(cards[id - 1].ACard.CardId);
            DeckCard dc = new DeckCard(card.CardId, (byte) cards[id - 1].ACard.Level, 0);
            card.SetData(dc);
            var img = card.GetPreview(CardPreviewType.Normal, new uint[0]);
            tooltip.Show(img, this, x, y);
        }

        private void Region_RegionLeft()
        {
            tooltip.Hide(this);
        }
        
        private void UpdateCards()
        {
            cards = new List<CardSlot>();
            var deckCards = selectMethod.GetCards();
            var margin = (Width - 2*150 - 120)/(deckCards.Length - 1); //所以必须至少2选一，不然会除零错
            for (int i = 0; i < deckCards.Length; i++)
            {
                var card = new CardSlot();
                card.SetSlotCard(deckCards[i]);
                card.Location = new Point(margin * i + 150, 100);
                card.Size = new Size(120, 120);
                card.BgColor = Color.Transparent;

                cards.Add(card);
            }
        }

        private void buttonStart_Click(object sender, System.EventArgs e)
        {
            selectMethod.OnStartButtonClick();
        }

        public void OnStartReady()
        {
            UpdateCards();
            canClick = false;
            bitmapButton1.ForeColor = Color.Red;
            bitmapButton1.Text = @"进入游戏";
            SetRegionVisible(1, false);
            SetRegionVisible(2, false);
            SetRegionVisible(3, false);
            Invalidate();
        }

        public void HideBackGroud()
        {
            bitmapButton1.Hide();
        }

        private void CardSelector_Paint(object sender, PaintEventArgs e)
        {
            if (cards == null)
            {
                return;
            }

            foreach (var cardSlot in cards)
            {
                if (cardSlot != null)
                {
                    cardSlot.CardSlot_Paint(e);
                }
            }

            if (region != null)
            {
                region.Draw(e.Graphics);
            }
        }
    }
}
