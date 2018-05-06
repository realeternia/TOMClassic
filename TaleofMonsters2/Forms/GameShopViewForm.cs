using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using ConfigDatas;
using TaleofMonsters.Core;
using TaleofMonsters.Core.Loader;
using TaleofMonsters.Datas.User;
using TaleofMonsters.Forms.Items;
using TaleofMonsters.Forms.Items.Core;
using TaleofMonsters.Forms.Items.Regions;
using TaleofMonsters.Forms.Items.Regions.Decorators;

namespace TaleofMonsters.Forms
{
    internal partial class GameShopViewForm : BasePanel
    {
        private GameShopItem[] itemControls;
        private int page;
        private List<int> productIds;
        private ControlPlus.NLPageSelector nlPageSelector1;
        private VirtualRegion vRegion;

        public GameShopViewForm()
        {
            InitializeComponent();
            this.bitmapButtonClose.ImageNormal = PicLoader.Read("Button.Panel", "CloseButton1.JPG");
            this.nlPageSelector1 = new ControlPlus.NLPageSelector(this, 365, 365, 150);
            nlPageSelector1.PageChange += nlPageSelector1_PageChange;
            productIds = new List<int>();
            vRegion = new VirtualRegion(this);
            string[] txt = { "卡包", "战斗", "道具" };
            for (int i = 0; i < 3; i++)
            {
                SubVirtualRegion subRegion = new ButtonRegion(i + 1, 16 + 45 * i, 39, 42, 23, "ShopTag.JPG", "");
                subRegion.AddDecorator(new RegionTextDecorator(8,7,9,Color.White, false, txt[i]));
                vRegion.AddRegion(subRegion);
            }

            vRegion.RegionClicked += new VirtualRegion.VRegionClickEventHandler(virtualRegion_RegionClick);
        }

        public override void Init(int width, int height)
        {
            base.Init(width, height);
            itemControls = new GameShopItem[9];
            for (int i = 0; i < 9; i++)
            {
                itemControls[i] = new GameShopItem(this, 11 + (i % 3) * 170, 61 + (i / 3) * 101, 170, 101);
                itemControls[i].Init();
            }
            virtualRegion_RegionClick(1, 0, 0, MouseButtons.Left);
        }

        public override void RefreshInfo()
        {
            for (int i = 0; i < 9; i++)
                itemControls[i].RefreshData((page * 9 + i < productIds.Count) ? productIds[page * 9 + i] : 0);
        }

        private void virtualRegion_RegionClick(int id, int x, int y, MouseButtons button)
        {
            if (button == MouseButtons.Left)
            {
                for (int i = 0; i < 4; i++)
                    vRegion.SetRegionEffect(i+1, RegionEffect.Free);

                vRegion.SetRegionEffect(id, RegionEffect.Blacken);
                productIds.Clear();
                foreach (var gameShopConfig in ConfigData.GameShopDict.Values)
                {
                    if (gameShopConfig.Shelf == id)
                        productIds.Add(gameShopConfig.Id);
                }
                nlPageSelector1.TotalPage = (productIds.Count - 1) / 9 + 1;
                page = 0;
                Invalidate(new Rectangle(16, 39, 45*4, 23));
                RefreshInfo();
            }
        }

        private void buttonClose_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void GameShopViewForm_Paint(object sender, PaintEventArgs e)
        {
            BorderPainter.Draw(e.Graphics, "", Width, Height);

            Font font = new Font("黑体", 12*1.33f, FontStyle.Bold, GraphicsUnit.Pixel);
            e.Graphics.DrawString("游戏商城", font, Brushes.White, Width / 2 - 40, 8);
            font.Dispose();

            vRegion.Draw(e.Graphics);
            foreach (var checkItem in itemControls)
                checkItem.Draw(e.Graphics);

            font = new Font("宋体", 9 * 1.33f, FontStyle.Regular, GraphicsUnit.Pixel);
            string str = string.Format("我的钻石:  {0} ", UserProfile.InfoBag.Diamond);
            e.Graphics.DrawString(str, font, Brushes.White, 20, 372);
            var strWid = TextRenderer.MeasureText(e.Graphics, str, font, new Size(0, 0), TextFormatFlags.NoPadding).Width;
            font.Dispose();
            e.Graphics.DrawImage(HSIcons.GetIconsByEName("res8"), 20 + strWid, 371, 14, 14);
        }

        private void nlPageSelector1_PageChange(int pg)
        {
            page = pg;
            RefreshInfo();
        }

        public override void OnRemove()
        {
            base.OnRemove();
            for (int i = 0; i < 9; i++)
            {
                itemControls[i].Dispose();
                itemControls[i] = null;
            }
        }
    }
}