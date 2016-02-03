using System.Collections;
using System.Drawing;
using System.Windows.Forms;
using NarlonLib.Core;
using TaleofMonsters.Controler.Battle.Data.Players;
using TaleofMonsters.Core.Interface;
using TaleofMonsters.Forms.Items.Regions;

namespace TaleofMonsters.Controler.Battle.Components
{
    internal partial class CardSelector : UserControl
    {
        internal delegate void CardSelectoreEventHandler();
        internal event CardSelectoreEventHandler StartClicked;

        private CardSlot[] cards;
        private ICardList cardList;
        private Player player;

        private VirtualRegion region;
        private bool[] keepCard={true,true,true,true}; //是否保留本卡

        private bool isButtonFirstClick = true;

        public CardSelector()
        {
            InitializeComponent();

            region = new VirtualRegion(this);
            region.AddRegion(new SwitchButtonRegion(1, 0, 0, 120, 120, 1, "ErrorButton.PNG", ""));
            region.AddRegion(new SwitchButtonRegion(2, 155, 0, 120, 120, 2, "ErrorButton.PNG", ""));
            region.AddRegion(new SwitchButtonRegion(3, 310, 0, 120, 120, 3, "ErrorButton.PNG", ""));
            region.AddRegion(new SwitchButtonRegion(4, 465, 0, 120, 120, 4, "ErrorButton.PNG", ""));
            region.RegionClicked+=region_RegionClicked;
        }

        private void region_RegionClicked(int info, MouseButtons button)
        {
            if (region != null)
            {
                keepCard[info - 1] = !keepCard[info - 1];
                region.SetRegionParm(info, keepCard[info - 1]);
                Invalidate();
            }
        }

        public void Init(Player p)
        {
            player = p;
            cardList = player.CardsDesk;//hold住cardlist

            cards = new CardSlot[4];
            InstallCards();
        }

        private void InstallCards()
        {
            var deckCards = cardList.GetAllCard();
            for (int i = 0; i < 4; i++)
            {
                var card = new CardSlot();
                card.SetSlotCard(deckCards[i]);
                card.Location = new Point(155 * i, 0);
                card.Size = new Size(120, 120);
                card.BgColor = Color.Black;

                cards[i] = card;
            }
        }

        private void buttonStart_Click(object sender, System.EventArgs e)
        {
            if (isButtonFirstClick)
            {
                for (int i = 3; i >= 0; i--)
                {
                    if (!keepCard[i])
                    {
                        player.CardManager.GetNextCardAt(i + 1);
                    }
                }
                InstallCards();
                region = null;
                isButtonFirstClick = false;
                buttonStart.ForeColor = Color.Red;
                buttonStart.Text = "进入游戏";
                Invalidate();
            }
            else
            {
                if (StartClicked != null)
                {
                    StartClicked();
                }
            }
        }

        IEnumerator DelayClick()
        {
            yield return new NLWaitForSeconds(3);

            if (StartClicked != null)
            {
                StartClicked();
            }
        }

        private void CardSelector_Paint(object sender, PaintEventArgs e)
        {
            if (cards == null)
            {
                return;
            }

            foreach (CardSlot cardSlot in cards)
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

        private void CardSelector_Click(object sender, System.EventArgs e)
        {

        }
    }
}
