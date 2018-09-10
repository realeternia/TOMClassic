using System;
using System.Drawing;
using System.Windows.Forms;
using ConfigDatas;
using ControlPlus;
using NarlonLib.Tools;
using TaleofMonsters.Core;
using TaleofMonsters.Core.Loader;
using TaleofMonsters.Datas;
using TaleofMonsters.Datas.Effects;
using TaleofMonsters.Datas.Effects.Facts;
using TaleofMonsters.Datas.User;
using TaleofMonsters.Datas.User.Db;
using TaleofMonsters.Forms.CMain;
using TaleofMonsters.Forms.Items;
using TaleofMonsters.Forms.Items.Core;
using TaleofMonsters.Forms.Items.Regions;
using TaleofMonsters.Forms.Items.Regions.Decorators;

namespace TaleofMonsters.Forms
{
    internal sealed partial class CardShopViewForm : BasePanel
    {
        private CellItemBox itemBox;
        private NLPageSelector nlPageSelector1;
        private int page;
        private int shelf;
        private DbCardProduct[] products;
        private string timeText;

        private VirtualRegion vRegion;

        public CardShopViewForm()
        {
            InitializeComponent();
            this.bitmapButtonClose.ImageNormal = PicLoader.Read("Button.Panel", "CloseButton1.JPG");
            this.bitmapButtonRefresh.ImageNormal = PicLoader.Read("Button.Panel", "LearnButton.JPG");
            bitmapButtonRefresh.NoUseDrawNine = true;
            this.nlPageSelector1 = new NLPageSelector(this, 371, 438, 150);
            nlPageSelector1.PageChange += nlPageSelector1_PageChange;

            vRegion = new VirtualRegion(this);
            string[] txt = {"怪物", "武器", "魔法"};
            for (int i = 0; i < 3; i++)
            {
                SubVirtualRegion subRegion = new ButtonRegion(i + 1, 16 + 45 * i, 40, 42, 23, "ShopTagOn.JPG", "");
                subRegion.AddDecorator(new RegionTextDecorator(8, 7, 9, Color.White, false, txt[i]));
                vRegion.AddRegion(subRegion);
            }
            vRegion.RegionClicked += new VirtualRegion.VRegionClickEventHandler(virtualRegion_RegionClick);

            itemBox = new CellItemBox(12, 62, 85 * 6, 125 * 3);
        }

        public override void Init(int width, int height)
        {
            base.Init(width, height);
            for (int i = 0; i < 18; i++)
            {
                var item = new CardShopItem(this);
                itemBox.AddItem(item);
                item.Init(i);
            }
            virtualRegion_RegionClick(1, 0, 0, MouseButtons.Left);

            SetBgm("TOM003.mp3");
            OnFrame(0, 0);
        }

        public override void RefreshInfo()
        {        
            for (int i = 0; i < 18; i++)
                itemBox.Refresh(i, (page * 18 + i < products.Length) ? products[page * 18 + i] : new DbCardProduct());
            Invalidate();
        }

        private void ChangeShop(int type)
        {
            page = 0;
            shelf = type;
            products = UserProfile.InfoWorld.GetCardProductsByType((CardTypes)shelf);
            Array.Sort(products, new CompareByMark());
            nlPageSelector1.TotalPage = (products.Length - 1) / 18 + 1;
            RefreshInfo();
        }

        private void buttonClose_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void bitmapButtonRefresh_Click(object sender, EventArgs e)
        {
            if (MessageBoxEx2.Show("是否花10钻石立刻刷新卡片?") == DialogResult.OK)
            {
                if (UserProfile.InfoBag.PayDiamond(10))
                {
                    UserProfile.InfoRecord.SetStateById(MemPlayerStateTypes.LastCardShopTime, 0);
                    ChangeShop(shelf);

                    var effect = new StaticUIEffect(EffectBook.GetEffect("redflash"), new Point(Width / 2 - 50, Height / 2 - 50), new Size(100, 100));
                    effect.Repeat = false;
                    AddEffect(effect);
                    AddFlowCenter("卡片商店数据刷新", "Lime");
                }
                else
                {
                    MainTipManager.AddTip(HSErrors.GetDescript(ErrorConfig.Indexer.BagNotEnoughDimond), "Red");
                }
            }
        }

        private void virtualRegion_RegionClick(int id, int x, int y, MouseButtons button)
        {
            if (button == MouseButtons.Left)
            {
                for (int i = 0; i < 3; i++)
                    vRegion.SetRegionEffect(i + 1, RegionEffect.Free);

                vRegion.SetRegionEffect(id, RegionEffect.Blacken);
                shelf = id;
                ChangeShop(shelf);
                Invalidate(new Rectangle(16, 40, 45 * 3, 23));
            }
        }

        delegate void ChangeShopCallback(int shelf);
        public override void OnFrame(int tick, float timePass)
        {
            base.OnFrame(tick, timePass);
            itemBox.OnFrame();

            if ((tick % 6) == 0)
            {
                TimeSpan span = TimeTool.UnixTimeToDateTime(UserProfile.InfoRecord.GetStateById(MemPlayerStateTypes.LastCardShopTime) + GameConstants.CardShopDura) - DateTime.Now;
                if (span.TotalSeconds > 0)
                {
                    timeText = string.Format("更新剩余 {0}:{1:00}:{2:00}", span.Hours, span.Minutes, span.Seconds);
                    Invalidate(new Rectangle(18, 447, 150, 20));
                }
                else
                {
                    BeginInvoke(new ChangeShopCallback(ChangeShop), shelf);
                }
            }
        }

        public void OnSelect(DbCardProduct card)
        {
            UserProfile.InfoCard.AddCard(card.Cid);
            UserProfile.InfoWorld.RemoveCardProduct(card.Cid);
            RefreshInfo();

            SoundManager.Play("System", "CoinDrop.mp3");
        }

        private void CardShopViewForm_Paint(object sender, PaintEventArgs e)
        {
            BorderPainter.Draw(e.Graphics, "", Width, Height);

            Font font = new Font("黑体", 12*1.33f, FontStyle.Bold, GraphicsUnit.Pixel);
            e.Graphics.DrawString("卡片商店", font, Brushes.White, Width / 2 - 40, 8);
            font.Dispose();

            vRegion.Draw(e.Graphics);
            itemBox.Draw(e.Graphics);
            font = new Font("宋体", 9*1.33f, FontStyle.Regular, GraphicsUnit.Pixel);
            e.Graphics.DrawString(timeText, font, Brushes.YellowGreen, 18, 447);
            font.Dispose();
        }

        private void nlPageSelector1_PageChange(int pg)
        {
            page = pg;
            RefreshInfo();
        }

        public override void OnRemove()
        {
            base.OnRemove();

            itemBox.Dispose();
        }
    }
}