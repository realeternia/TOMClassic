using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using ControlPlus;
using NarlonLib.Math;
using TaleofMonsters.Core;
using TaleofMonsters.Core.Config;
using TaleofMonsters.Core.Loader;
using TaleofMonsters.Datas;
using TaleofMonsters.Datas.User;
using TaleofMonsters.Datas.User.Db;
using TaleofMonsters.Forms.Items;
using TaleofMonsters.Forms.Items.Core;
using TaleofMonsters.Forms.Items.Regions;
using TaleofMonsters.Forms.Items.Regions.Decorators;

namespace TaleofMonsters.Forms
{
    internal sealed partial class CardShopDungeonViewForm : BasePanel
    {
        private CellItemBox itemBox;
        private List<DbCardProduct> products;

        private VirtualRegion vRegion;

        public CardShopDungeonViewForm()
        {
            InitializeComponent();
            this.bitmapButtonClose.ImageNormal = PicLoader.Read("Button.Panel", "CloseButton1.JPG");

            vRegion = new VirtualRegion(this);
            {
                SubVirtualRegion subRegion = new ButtonRegion(1, 16, 40, 42, 23, "ShopTagOn.JPG", "");
                subRegion.AddDecorator(new RegionTextDecorator(8, 7, 9, Color.White, false, "商店"));
                vRegion.AddRegion(subRegion);
            }

            itemBox = new CellItemBox(12, 62, 85 * 6, 125 * 3);
        }

        public override void Init(int width, int height)
        {
            base.Init(width, height);
            for (int i = 0; i < 18; i++)
            {
                var item = new CardShopDungeonItem(this);
                itemBox.AddItem(item);
                item.Init(i);
            }
            ChangeShop();
        }

        public override void RefreshInfo()
        {        
            for (int i = 0; i < 18; i++)
                itemBox.Refresh(i, (i < products.Count) ? products[i] : new DbCardProduct());
            Invalidate();
        }

        private void ChangeShop()
        {
            products = new List<DbCardProduct>();
            for (int i = 0; i < 12; i++)
            {
                var rdCard = 0;
                if (MathTool.IsRandomInRange01(0.2f))
                    rdCard = CardConfigManager.GetRandomJobCard(UserProfile.InfoDungeon.JobId);
                else
                    rdCard = CardConfigManager.GetRandomJobCard(0);
                products.Add(new DbCardProduct(i + 1, rdCard, MathTool.IsRandomInRange01(0.2f) 
                    ? (int)CardProductMarkTypes.Sale:(int)CardProductMarkTypes.Null));
            }
            products.Sort(new CompareByMark());
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

        public void OnSelect(DbCardProduct card)
        {
            UserProfile.InfoCard.AddDungeonCard(card.Cid, 0);
            products.Remove(card);
            RefreshInfo();

            SoundManager.Play("System", "CoinDrop.mp3");
        }

        private void CardShopDungeonViewForm_Paint(object sender, PaintEventArgs e)
        {
            BorderPainter.Draw(e.Graphics, "", Width, Height);

            Font font = new Font("黑体", 12*1.33f, FontStyle.Bold, GraphicsUnit.Pixel);
            e.Graphics.DrawString("购买卡牌", font, Brushes.White, Width / 2 - 40, 8);
            font.Dispose();

            vRegion.Draw(e.Graphics);
            itemBox.Draw(e.Graphics);
        }

        public override void OnRemove()
        {
            base.OnRemove();

            itemBox.Dispose();
        }
    }
}