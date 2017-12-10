using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using NarlonLib.Log;
using TaleofMonsters.Config;
using TaleofMonsters.Controler.Loader;
using TaleofMonsters.Core;
using TaleofMonsters.DataType.Decks;
using TaleofMonsters.DataType.User;
using TaleofMonsters.DataType.User.Db;
using TaleofMonsters.Forms.Items;
using TaleofMonsters.Forms.Items.Core;
using TaleofMonsters.Forms.MagicBook;

namespace TaleofMonsters.Forms
{
    internal sealed partial class DeckDungeonViewForm : BasePanel
    {
        private class CompareDeckCardByType : IComparer<DeckCard>
        {
            #region IComparer<CardDescript> 成员

            public int Compare(DeckCard cx, DeckCard cy)
            {
                if (cx.BaseId == cy.BaseId && cy.BaseId == 0)
                {
                    return 0;
                }
                if (cy.BaseId == 0)
                {
                    return -1;
                }
                if (cx.BaseId == 0)
                {
                    return 1;
                }
                int typex = CardConfigManager.GetCardConfig(cx.BaseId).Attr;
                int typey = CardConfigManager.GetCardConfig(cy.BaseId).Attr;
                if (typex != typey)
                {
                    return typex.CompareTo(typey);
                }

                return cx.BaseId.CompareTo(cy.BaseId);
            }

            #endregion
        }

        private DeckCardRegion cardRegion;//卡盒区域
        private DeckCard targetCard;

        private bool show;
        private int floor;

        private CardDetail cardDetail;

        public DeckDungeonViewForm()
        {
            InitializeComponent();

            this.bitmapButtonClose.ImageNormal = PicLoader.Read("Button.Panel", "CloseButton1.JPG");
            bitmapButtonClose.NoUseDrawNine = true;
          
            cardDetail = new CardDetail(this, 480, 35, 565);
            cardDetail.Enabled = true;
            cardDetail.Invalidate += DetailInvalidate;
            cardRegion = new DeckCardRegion(5, 35, 476, 510);
            cardRegion.Invalidate += DeckInvalidate;
        }

        private void DeckInvalidate()
        {
            if (cardRegion != null)
            {
                Invalidate(new Rectangle(cardRegion.X, cardRegion.Y, cardRegion.Width, cardRegion.Height));
            }
        }

        private void DetailInvalidate()
        {
            if (cardDetail != null)
            {
                Invalidate(new Rectangle(cardDetail.X, cardDetail.Y, cardDetail.Width, cardDetail.Height));
            }
        }

        public override void Init(int width, int height)
        {
            base.Init(width, height);
            show = true;

            ChangeDeck(1);

            SoundManager.PlayBGM("TOM003.mp3");
            IsChangeBgm = true;
        }

        public override void OnFrame(int tick, float timePass)
        {
            base.OnFrame(tick, timePass);

            cardDetail.OnFrame();
        }

        private void buttonClose_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void InstallDeckCard()
        {
            DeckCard[] dcards = null;
            DbDeckData dk = UserProfile.InfoCard.SelectedDeck;
            dcards = new DeckCard[GameConstants.DeckCardCount];
            for (int i = 0; i < GameConstants.DeckCardCount; i++)
            {
                int cid = dk.GetCardAt(i);
                dcards[i] = new DeckCard(UserProfile.InfoCard.GetDeckCardById(cid));
            }
        }

        private void ChangeDeck(int type)
        {
            InstallDeckCard();

            floor = type;
            DeckCard[] dcards = null;
            if (floor == 1)
            {
                dcards = new DeckCard[UserProfile.InfoCard.Cards.Count];
                int i = 0;
                foreach (var card in UserProfile.InfoCard.Cards.Values)
                {
                    dcards[i++] = new DeckCard(card);
                }
                Array.Sort(dcards, new CompareDeckCardByType());
            }
            else if (floor == 2)
            {
                dcards = new DeckCard[UserProfile.InfoCard.Newcards.Count];
                for (int i = 0; i < dcards.Length; i++)
                {
                    dcards[i] = new DeckCard(UserProfile.InfoCard.GetDeckCardById(UserProfile.InfoCard.Newcards[i]));
                }
            }

            cardRegion.ChangeDeck(dcards);
            SetTargetCard(dcards[0]);

            Invalidate();
        }

        private void SetTargetCard(DeckCard card)
        {
            targetCard = card;
            cardDetail.SetInfo(targetCard);
        }

        private void DeckViewForm_MouseMove(object sender, MouseEventArgs e)
        {
            cardRegion.CheckMouseMove(e.X, e.Y);
        }

        private void DeckViewForm_MouseClick(object sender, MouseEventArgs e)
        {
            var tCard = cardRegion.GetTargetCard();
            SetTargetCard(tCard);
        }

        private void DeckViewForm_Paint(object sender, PaintEventArgs e)
        {
            BorderPainter.Draw(e.Graphics, "", Width, Height);

            Font font = new Font("黑体", 12*1.33f, FontStyle.Bold, GraphicsUnit.Pixel);
            e.Graphics.DrawString("我的卡组", font, Brushes.White, Width / 2 - 40, 8);
            font.Dispose();

            try
            {
                if (show)
                {
                    cardDetail.Draw(e.Graphics);
                    cardRegion.Draw(e.Graphics);
                }
            }
            catch (Exception ex)
            {
                NLog.Error("DeckDungeonViewForm_Paint" + ex);
            }
        }

    }
}