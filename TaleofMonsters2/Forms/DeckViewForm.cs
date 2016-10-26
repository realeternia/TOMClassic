using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using ControlPlus;
using NarlonLib.Control;
using NarlonLib.Log;
using TaleofMonsters.Config;
using TaleofMonsters.Controler.Loader;
using TaleofMonsters.Core;
using TaleofMonsters.DataType.Decks;
using TaleofMonsters.DataType.User;
using TaleofMonsters.DataType.User.Mem;
using TaleofMonsters.Forms.Items;
using TaleofMonsters.Forms.Items.Core;
using TaleofMonsters.Forms.Items.Regions;
using TaleofMonsters.Forms.Items.Regions.Decorators;
using TaleofMonsters.Forms.MagicBook;
using TaleofMonsters.MainItem;

namespace TaleofMonsters.Forms
{
    internal sealed partial class DeckViewForm : BasePanel
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

        private VirtualRegion virtualRegion;
        private DeckCardRegion cardRegion;//卡盒区域
        private DeckSelectCardRegion selectRegion;//卡组区域
        private DeckCard targetCard;

        private const int yoff = 35;
        private bool show;
        private int floor;

        private bool isShowStatistic; //默认显示carddetail

        private CardDetail cardDetail;
        private CardDeckStatistic deckStatistic; 

        private PopMenuDeck popMenuDeck;
        private PoperContainer popContainer;

        public DeckViewForm()
        {
            InitializeComponent();
            virtualRegion = new VirtualRegion(this);
            SubVirtualRegion region;
            for (int i = 0; i < 2; i++)
            {
                region = new ButtonRegion(i + 1, 12+120+85*i, 40, 74, 24, i + 1, "CommonButton1.JPG", "CommonButton1On.JPG");
                region.AddDecorator(new RegionTextDecorator(region, 8, 7, 10, Color.Black));
                virtualRegion.AddRegion(region);
            }
            virtualRegion.SetRegionDecorator(1, 0, "全部卡片");
            virtualRegion.SetRegionDecorator(2, 0, " 新卡片");

            virtualRegion.RegionClicked += OnVRegionClicked;
            virtualRegion.RegionEntered += OnVRegionEntered;
            virtualRegion.RegionLeft += OnVRegionLeft;

            this.bitmapButtonClose.ImageNormal = PicLoader.Read("ButtonBitmap", "CloseButton1.JPG");
            bitmapButtonClose.NoUseDrawNine = true;
            this.bitmapButtonNext.ImageNormal = PicLoader.Read("ButtonBitmap", "NextButton.JPG");
            bitmapButtonNext.NoUseDrawNine = true;
            this.bitmapButtonSwitch.ImageNormal = PicLoader.Read("ButtonBitmap", "ButtonBack2.PNG");
            bitmapButtonSwitch.Font = new Font("宋体", 8 * 1.33f, FontStyle.Regular, GraphicsUnit.Pixel);
            bitmapButtonSwitch.ForeColor = Color.White;
            bitmapButtonSwitch.IconImage = PicLoader.Read("Icon", "rot4.PNG");
            bitmapButtonSwitch.IconSize = new Size(16, 16);
            bitmapButtonSwitch.IconXY = new Point(4, 4);
            bitmapButtonSwitch.TextOffX = 8;
            bitmapButtonSwitch.Text = "切换";
           // bitmapButtonNext.NoUseDrawNine = true;
            this.bitmapButtonPre.ImageNormal = PicLoader.Read("ButtonBitmap", "PreButton.JPG");
            bitmapButtonPre.NoUseDrawNine = true;
            this.bitmapButtonNextD.ImageNormal = PicLoader.Read("ButtonBitmap", "NextButton.JPG");
            bitmapButtonNextD.NoUseDrawNine = true;
            this.bitmapButtonPreD.ImageNormal = PicLoader.Read("ButtonBitmap", "PreButton.JPG");
            bitmapButtonPreD.NoUseDrawNine = true;
            this.bitmapButtonDel.ImageNormal = PicLoader.Read("ButtonBitmap", "DelButton.JPG");
            bitmapButtonDel.NoUseDrawNine = true;
          
            cardDetail = new CardDetail(this, 605, 35, 505);
            cardDetail.Invalidate += DetailInvalidate;
            deckStatistic = new CardDeckStatistic(this, 605, 35, 505);
            cardRegion = new DeckCardRegion(5+120, 35 + yoff, 480, 450);
            cardRegion.Invalidate += DeckInvalidate;
            selectRegion = new DeckSelectCardRegion(5, 35, 120, 480);
            selectRegion.Invalidate += SelectDeckInvalidate;

            popMenuDeck = new PopMenuDeck();
            popContainer = new PoperContainer(popMenuDeck);
            popMenuDeck.PoperContainer = popContainer;
            popMenuDeck.Form = this;
        }

        private void DeckInvalidate()
        {
            if (cardRegion != null)
            {
                Invalidate(new Rectangle(cardRegion.X, cardRegion.Y, cardRegion.Width, cardRegion.Height));
            }
        }

        private void SelectDeckInvalidate()
        {
            if (selectRegion != null)
            {
                Invalidate(new Rectangle(selectRegion.X, selectRegion.Y, selectRegion.Width, selectRegion.Height));
            }
        }

        private void DetailInvalidate()
        {
            if (cardDetail != null)
            {
                Invalidate(new Rectangle(cardDetail.X, cardDetail.Y, cardDetail.Width, cardDetail.Height));
            }
        }

        internal override void Init(int width, int height)
        {
            base.Init(width, height);
            show = true;

            ChangeDeck(1);
            UpdateButtonState();
            UpdateDeckButtonState();            

            SoundManager.PlayBGM("TOM003.MP3");
            IsChangeBgm = true;
        }

        internal override void OnFrame(int tick)
        {
            base.OnFrame(tick);

            cardDetail.OnFrame();
        }

        private void buttonClose_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void InstallDeckCard()
        {
            DeckCard[] dcards = null;
            MemDeckData dk = UserProfile.InfoCard.SelectedDeck;
            dcards = new DeckCard[GameConstants.DeckCardCount];
            for (int i = 0; i < GameConstants.DeckCardCount; i++)
            {
                int cid = dk.GetCardAt(i);
                dcards[i] = UserProfile.InfoCard.GetDeckCardById(cid);
            }

            selectRegion.ChangeDeck(dcards); 
        }

        private void ChangeDeck(int type)
        {
            InstallDeckCard();

            floor = type;
            DeckCard[] dcards = null;
            if (floor == 1)
            {
                dcards = new DeckCard[UserProfile.InfoCard.Cards.Count];
                UserProfile.InfoCard.Cards.Values.CopyTo(dcards, 0);
                Array.Sort(dcards, new CompareDeckCardByType());
            }
            else if (floor == 2)
            {
                dcards = new DeckCard[UserProfile.InfoCard.Newcards.Count];
                for (int i = 0; i < dcards.Length; i++)
                {
                    dcards[i] = UserProfile.InfoCard.GetDeckCardById(UserProfile.InfoCard.Newcards[i]);
                }
            }

            cardRegion.ChangeDeck(dcards);
            SetTargetCard(dcards[0]);

            UpdateButtonState();
            UpdateDeckButtonState();
            for (int i = 0; i < 3; i++)
            {
                virtualRegion.SetRegionState(i + 1, RegionState.Free);
            }
            virtualRegion.SetRegionState(type, RegionState.Blacken);
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

            selectRegion.CheckMouseMove(e.X, e.Y);
        }

        private void DeckViewForm_MouseClick(object sender, MouseEventArgs e)
        {
            bool cardFromRegion = true;
            var tCard = cardRegion.GetTargetCard();
            if (tCard == null)
            {
                tCard = selectRegion.GetTargetCard();
                if (tCard == null)
                {
                    return;
                }
                cardFromRegion = false;
            }

            SetTargetCard(tCard);
            if (e.Button == MouseButtons.Right)
            {
                popMenuDeck.Clear();

                if (cardFromRegion)
                {
                    popMenuDeck.AddItem("activate", "添加到卡组");
                    popMenuDeck.AddItem("delete", "分解碎片", "Red");
                    popMenuDeck.AddItem("levelup", "升级");
                }
                else
                {
                    popMenuDeck.AddItem("remove", "从卡组移除");
                }

                popMenuDeck.AddItem("cancel", "取消");
                popMenuDeck.TargetCard = targetCard;
                popMenuDeck.AutoResize();
                popContainer.Show(this, e.Location.X, e.Location.Y);                
            }
        }

        public void MenuRefresh(bool needUpdate)
        {
            cardRegion.MenuRefresh();
            if (needUpdate)
            {
                var card = cardRegion.GetTargetCard();
                SetTargetCard(card);
            }
            Invalidate();
        }

        public void ActivateCard()
        {
            InstallDeckCard();
            cardRegion.Invalidate();
            selectRegion.Invalidate();
        }

        private void UpdateButtonState()
        {
            bitmapButtonPre.Enabled = cardRegion.Page > 0;
            bitmapButtonNext.Enabled = (cardRegion.Page + 1) * cardRegion.CardCount < cardRegion.CardTotalCount;
            bitmapButtonPre.Visible = bitmapButtonPre.Enabled || bitmapButtonNext.Enabled;
            bitmapButtonNext.Visible = bitmapButtonPre.Enabled || bitmapButtonNext.Enabled;
        }

        private void buttonPre_Click(object sender, EventArgs e)
        {
            if (cardRegion.PrePage())
            {
                var card = cardRegion.GetTargetCard();
                SetTargetCard(card);
                UpdateButtonState();
            }
        }

        private void buttonNext_Click(object sender, EventArgs e)
        {
            if (cardRegion.NextPage())
            {
                var card = cardRegion.GetTargetCard();
                SetTargetCard(card);
                UpdateButtonState();
            }
        }
        private void bitmapButtonSwitch_Click(object sender, EventArgs e)
        {
            isShowStatistic = !isShowStatistic;
            Invalidate(new Rectangle(cardDetail.X, cardDetail.Y, cardDetail.Width, cardDetail.Height));
        }

        private void UpdateDeckButtonState()
        {
            bitmapButtonPreD.Enabled = UserProfile.InfoCard.DeckId > 0;
            bitmapButtonNextD.Enabled = UserProfile.InfoCard.DeckId + 1 < GameConstants.PlayDeckCount;
        }

        private void buttonNextD_Click(object sender, EventArgs e)
        {
            int tmp = UserProfile.InfoCard.DeckId + 1;
            if (tmp >= GameConstants.PlayDeckCount)
            {
                return;
            }
            UserProfile.InfoCard.DeckId = tmp;
            InstallDeckCard();
            cardRegion.MenuRefresh();
            Invalidate();
            UpdateDeckButtonState();
        }

        private void buttonPreD_Click(object sender, EventArgs e)
        {
            int tmp = UserProfile.InfoCard.DeckId - 1;
            if (tmp < 0)
            {
                return;
            }
            UserProfile.InfoCard.DeckId = tmp;
            InstallDeckCard();
            cardRegion.MenuRefresh();
            Invalidate();
            UpdateDeckButtonState();
        }

        private void buttonDelD_Click(object sender, EventArgs e)
        {
            if (MessageBoxEx2.Show("确定要清除卡组内所有卡片？") == DialogResult.OK)
            {
                for (int i = 0; i < GameConstants.DeckCardCount; i++)
                {
                    UserProfile.InfoCard.SelectedDeck.SetCardAt(i, -1);
                }
            }
            InstallDeckCard();
            cardRegion.MenuRefresh();
            Invalidate();
        }

        private void OnVRegionClicked(int info, MouseButtons button)
        {
            if (button == MouseButtons.Left)
            {
                if (info>0 && info < 10)
                {
                    ChangeDeck(info);
                }
                //else if (info == 11)//说明是升级
                //{
                //    int itemId = 32101;//todo 规则需要修改
                //    HItemConfig itemConfig = ConfigData.GetHItemConfig(itemId);
                //    if (UserProfile.Profile.InfoBag.GetItemCount(itemId) > 0)//给卡牌增加经验
                //    {
                //        UserProfile.Profile.InfoBag.DeleteItem(itemId, 1);
                //     //   UserProfile.InfoCard.AddCardExp(targetCard.Id, itemConfig.UseEffect);
                //    }
                //    cardRegion.MenuRefresh();
                //    Invalidate();
                //}
            }
        }

        private void OnVRegionEntered(int info, int x, int y, int key)
        {

        }

        private void OnVRegionLeft()
        {

        }

        private void DeckViewForm_Paint(object sender, PaintEventArgs e)
        {
            BorderPainter.Draw(e.Graphics, "", Width, Height);

            Font font = new Font("黑体", 12*1.33f, FontStyle.Bold, GraphicsUnit.Pixel);
            e.Graphics.DrawString("我的卡片", font, Brushes.White, Width / 2 - 40, 8);
            font.Dispose();

            try
            {
                if (show)
                {
                    if (!isShowStatistic)
                        cardDetail.Draw(e.Graphics);
                    else
                        deckStatistic.Draw(e.Graphics);
                    cardRegion.Draw(e.Graphics);
                    selectRegion.Draw(e.Graphics);

                    virtualRegion.Draw(e.Graphics);

                    font = new Font("黑体", 12*1.33f, FontStyle.Bold, GraphicsUnit.Pixel);
                    e.Graphics.DrawString(UserProfile.InfoCard.SelectedDeck.Name, font, Brushes.White, 5, 520);
                    font.Dispose();
                }
            }
            catch (Exception ex)
            {
                NLog.Error("DeckViewForm_Paint" + ex);
            }
        }

    }
}