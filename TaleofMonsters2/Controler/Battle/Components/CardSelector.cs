using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using TaleofMonsters.Controler.Battle.Components.CardSelect;
using TaleofMonsters.Controler.Battle.Data.Players;
using TaleofMonsters.Controler.Loader;
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

        private bool canClick;

        public CardSelector()
        {
            InitializeComponent();

            region = new VirtualRegion(this);
            region.RegionClicked+=region_RegionClicked;

            bitmapButton1.ImageNormal = PicLoader.Read("ButtonBitmap", "ButtonBack2.PNG");
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
                region.AddRegion(new SwitchButtonRegion(i +1, margin * i + 150, 100, 120, 120, "ErrorButton.PNG", ""));
            }

            UpdateCards();
        }

        public void SetRegionParm(int id, bool parm)
        {
            if (region != null)
            {
                region.SetRegionParm(id, parm);
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
            region.SetRegionParm(1, true);
            region.SetRegionParm(2, true);
            region.SetRegionParm(3, true);
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
