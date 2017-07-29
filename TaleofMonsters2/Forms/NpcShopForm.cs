using System;
using System.Drawing;
using TaleofMonsters.Controler.Loader;
using TaleofMonsters.Forms.Items;
using ConfigDatas;
using TaleofMonsters.DataType.Items;
using TaleofMonsters.Forms.Items.Core;

namespace TaleofMonsters.Forms
{
    internal sealed partial class NpcShopForm : BasePanel
    {
        private const int MaxCellCount = 12;

        private int[] items;
        private int page;
        private ShopItem[] itemControls;
        private ControlPlus.NLPageSelector nlPageSelector1;

        public string ShopName { get; set; }

        public NpcShopForm()
        {
            InitializeComponent();
            this.bitmapButtonClose.ImageNormal = PicLoader.Read("Button.Panel", "CloseButton1.JPG");
            this.nlPageSelector1 = new ControlPlus.NLPageSelector(this, 143, 244, 150);
            nlPageSelector1.PageChange += nlPageSelector1_PageChange;
        }

        public override void Init(int width, int height)
        {
            base.Init(width, height);

            var shopInfo = ConfigData.GetNpcShopConfig(GetShopId());
            items = new int[shopInfo.SellTable.Length];
            for (int i = 0; i < items.Length; i++)
            {
                items[i] = HItemBook.GetItemId(shopInfo.SellTable[i]);
            }
            itemControls = new ShopItem[MaxCellCount];
            for (int i = 0; i < MaxCellCount; i++)
            {
                itemControls[i] = new ShopItem(this, 8 + (i % 3) * 142, 35 + (i / 3) * 55, 143, 56);
                itemControls[i].Init(shopInfo.MoneyType);
            }
            nlPageSelector1.TotalPage = (items.Length - 1) / MaxCellCount + 1;
            RefreshInfo();
        }

        private int GetShopId()
        {
            foreach (var npcShopConfig in ConfigData.NpcShopDict.Values)
            {
                if (npcShopConfig.Ename == ShopName)
                {
                    return npcShopConfig.Id;
                }
            }
            return 0;
        }

        private void RefreshInfo()
        {
            for (int i = 0; i < MaxCellCount; i++)
            {
                if (page* MaxCellCount + i < items.Length)
                {
                    itemControls[i].RefreshData(items[page* MaxCellCount + i]);
                }
                else
                {
                    itemControls[i].RefreshData(0);
                }
            }
        }

        private void pictureBoxCancel_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void ShopWindow_Paint(object sender, System.Windows.Forms.PaintEventArgs e)
        {
            BorderPainter.Draw(e.Graphics, "", Width, Height);

            if (!string.IsNullOrEmpty(ShopName))
            {
                Font font2 = new Font("黑体", 12 * 1.33f, FontStyle.Bold, GraphicsUnit.Pixel);
                e.Graphics.DrawString("商店", font2, Brushes.White, Width / 2 - 40, 8);
                font2.Dispose();

                foreach (ShopItem ctl in itemControls)
                {
                    ctl.Draw(e.Graphics);
                }
            }
        }

        private void nlPageSelector1_PageChange(int pg)
        {
            page = pg;
            RefreshInfo();
        }
    }
}