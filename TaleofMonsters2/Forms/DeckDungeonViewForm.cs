using System;
using System.Drawing;
using System.Windows.Forms;
using NarlonLib.Log;
using TaleofMonsters.Controler.Loader;
using TaleofMonsters.Core;
using TaleofMonsters.DataType.Decks;
using TaleofMonsters.DataType.User;
using TaleofMonsters.Forms.Items;
using TaleofMonsters.Forms.Items.Core;
using TaleofMonsters.Forms.MagicBook;

namespace TaleofMonsters.Forms
{
    internal sealed partial class DeckDungeonViewForm : BasePanel
    {
        private DeckCardRegion cardRegion;//卡盒区域
        private DeckCard targetCard;

        private bool show;

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
            cardRegion.IsDungeonMode = true;
            cardRegion.Invalidate += DeckInvalidate;
        }

        private void DeckInvalidate()
        {
            if (cardRegion != null)
                Invalidate(new Rectangle(cardRegion.X, cardRegion.Y, cardRegion.Width, cardRegion.Height));
        }

        private void DetailInvalidate()
        {
            if (cardDetail != null)
                Invalidate(new Rectangle(cardDetail.X, cardDetail.Y, cardDetail.Width, cardDetail.Height));
        }

        public override void Init(int width, int height)
        {
            base.Init(width, height);
            show = true;

            InstallDeckCard(); 

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
            dcards = new DeckCard[UserProfile.InfoCard.DungeonDeck.Count];
            for (int i = 0; i < dcards.Length; i++)
            {
                var selectCard = UserProfile.InfoCard.DungeonDeck[i];
                dcards[i] = new DeckCard(selectCard.BaseId, selectCard.Level, selectCard.Exp);
            }
            cardRegion.ChangeDeck(dcards);
            SetTargetCard(dcards[0]);
        }

        private void SetTargetCard(DeckCard card)
        {
            targetCard = card;
            cardDetail.SetInfo(targetCard);
        }

        private void DeckDungeonViewForm_MouseMove(object sender, MouseEventArgs e)
        {
            cardRegion.CheckMouseMove(e.X, e.Y);
        }

        private void DeckDungeonViewForm_MouseClick(object sender, MouseEventArgs e)
        {
            var tCard = cardRegion.GetTargetCard();
            SetTargetCard(tCard);
        }

        private void DeckDungeonViewForm_Paint(object sender, PaintEventArgs e)
        {
            BorderPainter.Draw(e.Graphics, "", Width, Height);

            Font font = new Font("黑体", 12*1.33f, FontStyle.Bold, GraphicsUnit.Pixel);
            e.Graphics.DrawString("副本卡组", font, Brushes.White, Width / 2 - 40, 8);
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