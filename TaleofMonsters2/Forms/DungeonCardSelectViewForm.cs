using System;
using System.Drawing;
using System.Windows.Forms;
using ControlPlus;
using TaleofMonsters.Core.Loader;
using TaleofMonsters.Datas.User;
using TaleofMonsters.Datas.User.Db;
using TaleofMonsters.Forms.Items;
using TaleofMonsters.Forms.Items.Core;
using TaleofMonsters.Forms.Items.Regions;
using TaleofMonsters.Forms.Items.Regions.Decorators;

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

        private void CardShopViewForm_Paint(object sender, PaintEventArgs e)
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

            vRegion.Draw(e.Graphics);
            itemBox.Draw(e.Graphics);
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