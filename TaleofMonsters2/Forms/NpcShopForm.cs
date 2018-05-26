using System;
using System.Collections.Generic;
using System.Drawing;
using TaleofMonsters.Forms.Items;
using ConfigDatas;
using ControlPlus;
using NarlonLib.Math;
using TaleofMonsters.Core.Loader;
using TaleofMonsters.Datas.Items;
using TaleofMonsters.Forms.Items.Core;

namespace TaleofMonsters.Forms
{
    internal sealed partial class NpcShopForm : BasePanel
    {
        internal class NpcShopData
        {
            public int ShopId;
            public int ItemId;
        }

        private int[] items;
        private CellItemBox itemBox;

        public string ShopName { get; set; }
        private int shopId;

        public NpcShopForm()
        {
            InitializeComponent();
            this.bitmapButtonClose.ImageNormal = PicLoader.Read("Button.Panel", "CloseButton1.JPG");

            itemBox = new CellItemBox(8, 35, 143 * 3, 56 * 4);
        }

        public override void Init(int width, int height)
        {
            base.Init(width, height);

            shopId = GetShopId();
            var shopConfig = ConfigData.GetNpcShopConfig(shopId);
            var itemList = new List<int>();
            for (int i = 0; i < shopConfig.SellTable.Length; i++)
                itemList.Add(HItemBook.GetItemId(shopConfig.SellTable[i]));

            if (shopConfig.RandomChooseX > 0)
            {
                foreach (var itemName in NLRandomPicker<string>.RandomPickN(shopConfig.SellRandomTable, (uint)shopConfig.RandomChooseX))
                    itemList.Add(HItemBook.GetItemId(itemName));
            }
            items = itemList.ToArray();

            for (int i = 0; i < 12; i++)
            {
                var item = new NpcShopItem(this);
                itemBox.AddItem(item);
                item.Init(i);
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
            for (int i = 0; i < 12; i++)
            {
                if (i < items.Length)
                    itemBox.Refresh(i, new NpcShopData { ItemId = items[i], ShopId = shopId });
                else
                    itemBox.Refresh(i, new NpcShopData());
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

                itemBox.Draw(e.Graphics);
            }
        }

        public override void OnRemove()
        {
            base.OnRemove();
            itemBox.Dispose();
        }
    }
}