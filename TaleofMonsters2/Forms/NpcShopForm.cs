using System;
using System.Collections.Generic;
using System.Drawing;
using TaleofMonsters.Forms.Items;
using ConfigDatas;
using NarlonLib.Math;
using TaleofMonsters.Core.Loader;
using TaleofMonsters.Datas.Items;
using TaleofMonsters.Forms.Items.Core;

namespace TaleofMonsters.Forms
{
    internal sealed partial class NpcShopForm : BasePanel
    {
        private const int MaxCellCount = 12;

        private int[] items;
        private NpcShopItem[] itemControls;

        public string ShopName { get; set; }

        public NpcShopForm()
        {
            InitializeComponent();
            this.bitmapButtonClose.ImageNormal = PicLoader.Read("Button.Panel", "CloseButton1.JPG");
        }

        public override void Init(int width, int height)
        {
            base.Init(width, height);

            var shopConfig = ConfigData.GetNpcShopConfig(GetShopId());
            var itemList = new List<int>();
            for (int i = 0; i < shopConfig.SellTable.Length; i++)
                itemList.Add(HItemBook.GetItemId(shopConfig.SellTable[i]));

            if (shopConfig.RandomChooseX > 0)
            {
                foreach (var itemName in NLRandomPicker<string>.RandomPickN(shopConfig.SellRandomTable, (uint)shopConfig.RandomChooseX))
                    itemList.Add(HItemBook.GetItemId(itemName));
            }
            items = itemList.ToArray();

            itemControls = new NpcShopItem[MaxCellCount];
            for (int i = 0; i < MaxCellCount; i++)
            {
                itemControls[i] = new NpcShopItem(this, 8 + (i % 3) * 142, 35 + (i / 3) * 55, 143, 56);
                itemControls[i].Init(shopConfig.MoneyType, shopConfig.RandomPrice, shopConfig.LimitCount);
            }
            RefreshInfo();
        }

        private int GetShopId()
        {
            foreach (var npcShopConfig in ConfigData.NpcShopDict.Values)
            {
                if (npcShopConfig.Ename == ShopName)
                    return npcShopConfig.Id;
            }
            return 0;
        }

        public override void RefreshInfo()
        {
            for (int i = 0; i < MaxCellCount; i++)
            {
                if (i < items.Length)
                    itemControls[i].RefreshData(items[i]);
                else
                    itemControls[i].RefreshData(0);
            }
        }

        private void pictureBoxCancel_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void NpcShopForm_Paint(object sender, System.Windows.Forms.PaintEventArgs e)
        {
            BorderPainter.Draw(e.Graphics, "", Width, Height);

            if (!string.IsNullOrEmpty(ShopName))
            {
                Font font2 = new Font("黑体", 12 * 1.33f, FontStyle.Bold, GraphicsUnit.Pixel);
                e.Graphics.DrawString("商店", font2, Brushes.White, Width / 2 - 40, 8);
                font2.Dispose();

                foreach (var ctl in itemControls)
                    ctl.Draw(e.Graphics);
            }
        }

        public override void OnRemove()
        {
            base.OnRemove();
            for (int i = 0; i < MaxCellCount; i++)
            {
                itemControls[i].Dispose();
                itemControls[i] = null;
            }
        }
    }
}