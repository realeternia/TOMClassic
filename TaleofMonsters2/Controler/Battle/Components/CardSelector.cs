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

        public CardSelector()
        {
            InitializeComponent();

            region = new VirtualRegion(this);
            region.AddRegion(new SwitchButtonRegion(1, 0, 0, 120, 120, "ErrorButton.PNG", ""));
            region.AddRegion(new SwitchButtonRegion(2, 200, 0, 120, 120, "ErrorButton.PNG", ""));
            region.AddRegion(new SwitchButtonRegion(3, 400, 0, 120, 120, "ErrorButton.PNG", ""));
            region.RegionClicked+=region_RegionClicked;

            bitmapButton1.ImageNormal = PicLoader.Read("ButtonBitmap", "ButtonBack2.PNG");
        }
        public void Init(Player p, ICardSelectMethod method)
        {
            selectMethod = method;
            selectMethod.Selector = this;
            selectMethod.Init(p);
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
            if (region != null)
            {
                selectMethod.RegionClicked(id);
                Invalidate();
            }
        }


        private void UpdateCards()
        {
            cards = new List<CardSlot>();
            var deckCards = selectMethod.GetCards();
            for (int i = 0; i < deckCards.Length; i++)
            {
                var card = new CardSlot();
                card.SetSlotCard(deckCards[i]);
                card.Location = new Point(200 * i, 0);
                card.Size = new Size(120, 120);
                card.BgColor = Color.Black;

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
            region = null;
            bitmapButton1.ForeColor = Color.Red;
            bitmapButton1.Text = @"进入游戏";
            Invalidate();
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
