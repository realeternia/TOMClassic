using System;
using System.Collections;
using System.Drawing;
using System.Windows.Forms;
using ControlPlus;
using NarlonLib.Core;
using TaleofMonsters.Core;
using TaleofMonsters.Core.Loader;
using TaleofMonsters.Datas.Effects;
using TaleofMonsters.Datas.Effects.Facts;
using TaleofMonsters.Datas.User;
using TaleofMonsters.Datas.User.Db;
using TaleofMonsters.Forms.Items;
using TaleofMonsters.Forms.Items.Core;
using TaleofMonsters.Forms.Items.Regions;
using TaleofMonsters.Forms.Items.Regions.Decorators;
using TaleofMonsters.Rpc;

namespace TaleofMonsters.Forms
{
    internal sealed partial class DungeonCardSelectViewForm : BasePanel
    {
        private CellItemBox itemBox;
        private NLPageSelector nlPageSelector1;
        private int page;
        private DbDeckCard[] cards;

        private VirtualRegion vRegion;
        public DungeonCardItem.CardCopeMode Mode { get; set; }
        private VirtualRegionMoveMediator moveMediator;
        private int cardDealCount = 1;

        public DungeonCardSelectViewForm()
        {
            InitializeComponent();
            this.bitmapButtonClose.ImageNormal = PicLoader.Read("Button.Panel", "CloseButton1.JPG");
            this.nlPageSelector1 = new NLPageSelector(this, 371, 438, 150);
            nlPageSelector1.PageChange += nlPageSelector1_PageChange;

            vRegion = new VirtualRegion(this);
            {
                SubVirtualRegion subRegion = new ButtonRegion(1, 16, 40, 42, 23, "ShopTagOn.JPG", "");
                subRegion.AddDecorator(new RegionTextDecorator(8, 7, 9, Color.White, false, "卡牌"));
                vRegion.AddRegion(subRegion);
            }
            vRegion.AddRegion(new PictureRegion(10, (Width-120)/2, (Height-120)/2,120,120, PictureRegionCellType.Card, 0));
            vRegion.SetRegionVisible(10, false);
            moveMediator = new VirtualRegionMoveMediator(vRegion);
            itemBox = new CellItemBox(12, 62, 85 * 6, 125 * 3);
        }

        public override void Init(int width, int height)
        {
            base.Init(width, height);
            for (int i = 0; i < 18; i++)
            {
                var item = new DungeonCardItem(this);
                item.Mode = Mode;
                itemBox.AddItem(item);
                item.Init(i);
            }
            ChangeShop();

            OnFrame(0, 0);
        }

        public override void RefreshInfo()
        {        
            for (int i = 0; i < 18; i++)
                itemBox.Refresh(i, (page * 18 + i < cards.Length) ? cards[page * 18 + i] : new DbDeckCard());
            Invalidate();
        }

        protected override void BasePanelMessageWork(int token)
        {
            vRegion.SetRegionVisible(10, false); //通过线程安全方式调用
            canClose = true;
            Close();
        }

        private void ChangeShop()
        {
            page = 0;
            cards = UserProfile.InfoCard.DungeonDeck.ToArray();
           // Array.Sort(cards, new CompareByMark());
            nlPageSelector1.TotalPage = (cards.Length - 1) / 18 + 1;
            RefreshInfo();
        }

        private void buttonClose_Click(object sender, EventArgs e)
        {
            Close();
        }

        public override void OnFrame(int tick, float timePass)
        {
            base.OnFrame(tick, timePass);
            itemBox.OnFrame();
        }

        private void DungeonCardSelectViewForm_Paint(object sender, PaintEventArgs e)
        {
            BorderPainter.Draw(e.Graphics, "", Width, Height);

            Font font = new Font("黑体", 12*1.33f, FontStyle.Bold, GraphicsUnit.Pixel);
            var headName = "";
            if (Mode == DungeonCardItem.CardCopeMode.Remove)
                headName = "选择移除一张卡片";
            else if (Mode == DungeonCardItem.CardCopeMode.Upgrade)
                headName = "选择升级一张卡片";
            e.Graphics.DrawString(headName, font, Brushes.White, Width / 2 - 60, 8);
            font.Dispose();
            
            itemBox.Draw(e.Graphics);
            vRegion.Draw(e.Graphics);
        }

        private void nlPageSelector1_PageChange(int pg)
        {
            page = pg;
            RefreshInfo();
        }

        public void OnSelect(DbDeckCard card)
        {
            if(cardDealCount <= 0)
                return;
            cardDealCount--;

            if (Mode == DungeonCardItem.CardCopeMode.Remove)
            {
                foreach (var pickCard in UserProfile.InfoCard.DungeonDeck)
                {
                    if (card.BaseId == pickCard.BaseId && card.Level == pickCard.Level)
                    {
                        UserProfile.InfoCard.DungeonDeck.Remove(card);
                        break;
                    }
                }
            }
            else if (Mode == DungeonCardItem.CardCopeMode.Upgrade)
            {
                foreach (var pickCard in UserProfile.InfoCard.DungeonDeck)
                {
                    if (card.BaseId == pickCard.BaseId && card.Level == pickCard.Level)
                    {
                        card.Level = (byte)Math.Min(card.Level + 2, GameConstants.CardMaxLevel);
                        break;
                    }
                }
            }

            vRegion.SetRegionKey(10, card.BaseId);
            vRegion.SetRegionVisible(10, true);
          
            canClose = false;

            if (Mode == DungeonCardItem.CardCopeMode.Upgrade)
            {
                TalePlayer.Start(DelayUp());
            }
            else
            {
                moveMediator.FireFadeOut(10);
                TalePlayer.Start(DelayHide());
            }
            Invalidate(); //为了让卡牌显示出来
        }
        private IEnumerator DelayUp()
        {
            AddEffect(20, 20);
            yield return new NLWaitForSeconds(0.2f);
            AddEffect(50, 0);
            yield return new NLWaitForSeconds(0.3f);
            AddEffect(0, 30);
            yield return new NLWaitForSeconds(1.2f);
            BasePanelMessageSafe(1);
        }

        private IEnumerator DelayHide()
        {
            yield return new NLWaitForSeconds(1.7f);
            BasePanelMessageSafe(1);
        }

        private void AddEffect(int x, int y)
        {
            var effect = new StaticUIEffect(EffectBook.GetEffect("yellowsplash"), new Point(x + (Width - 120) / 2, y + (Height - 120) / 2), new Size(80, 80));
            effect.Repeat = false;
            AddEffect(effect);
        }

        public override void OnRemove()
        {
            base.OnRemove();

            itemBox.Dispose();
        }
    }
}