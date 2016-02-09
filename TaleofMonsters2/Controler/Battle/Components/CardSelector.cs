﻿using System.Drawing;
using System.Windows.Forms;
using TaleofMonsters.Controler.Battle.Data.Players;
using TaleofMonsters.Controler.Loader;
using TaleofMonsters.Core;
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
        private bool[] keepCard={true,true,true}; //是否保留本卡

        private bool isButtonFirstClick = true;

        public CardSelector()
        {
            InitializeComponent();

            region = new VirtualRegion(this);
            region.AddRegion(new SwitchButtonRegion(1, 0, 0, 120, 120, 1, "ErrorButton.PNG", ""));
            region.AddRegion(new SwitchButtonRegion(2, 200, 0, 120, 120, 2, "ErrorButton.PNG", ""));
            region.AddRegion(new SwitchButtonRegion(3, 400, 0, 120, 120, 3, "ErrorButton.PNG", ""));
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
            bitmapButton1.ImageNormal = PicLoader.Read("ButtonBitmap", "ButtonBack2.PNG");

            player = p;
            cardList = player.CardsDesk;//hold住cardlist

            cards = new CardSlot[GameConstants.BattleInitialCardCount];
            InstallCards();
        }

        private void InstallCards()
        {
            var deckCards = cardList.GetAllCard();
            for (int i = 0; i < GameConstants.BattleInitialCardCount; i++)
            {
                var card = new CardSlot();
                card.SetSlotCard(deckCards[i]);
                card.Location = new Point(200 * i, 0);
                card.Size = new Size(120, 120);
                card.BgColor = Color.Black;

                cards[i] = card;
            }
        }

        private void buttonStart_Click(object sender, System.EventArgs e)
        {
            if (isButtonFirstClick)
            {
                for (int i = GameConstants.BattleInitialCardCount-1; i >= 0; i--)
                {
                    if (!keepCard[i])
                    {
                        player.CardManager.GetNextCardAt(i + 1);
                    }
                }
                InstallCards();
                region = null;
                isButtonFirstClick = false;
                bitmapButton1.ForeColor = Color.Red;
                bitmapButton1.Text = "进入游戏";
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
    }
}