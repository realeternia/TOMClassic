using System;
using System.Drawing;
using TaleofMonsters.Controler.Loader;
using TaleofMonsters.DataType.NPCs;
using TaleofMonsters.Forms.Items;
using ConfigDatas;

namespace TaleofMonsters.Forms
{
    internal sealed partial class NpcShopForm : BasePanel
    {
        private int npcId;
        private int shopId;
        private RLIdValue[] items;
        private int page;
        private ShopItem[] itemControls;
        private ControlPlus.NLPageSelector nlPageSelector1;

        public int NpcId
        {
            set { npcId = value; }
        }

        public int ShopId
        {
            set { shopId = value; }
        }

        public NpcShopForm()
        {
            InitializeComponent();
            this.bitmapButtonClose2.ImageNormal = PicLoader.Read("ButtonBitmap", "CloseButton1.JPG");
            this.nlPageSelector1 = new ControlPlus.NLPageSelector(this, 143, 244, 150);
            nlPageSelector1.PageChange += nlPageSelector1_PageChange;
        }

        internal override void Init(int width, int height)
        {
            base.Init(width, height);
            items = new RLIdValue[ConfigData.GetNpcShopConfig(shopId).SellTable.Count];
            for (int i = 0; i < items.Length; i++)
            {
                items[i] = ConfigData.GetNpcShopConfig(shopId).SellTable[i];
            }
            itemControls = new ShopItem[6];
            for (int i = 0; i < 6; i++)
            {
                itemControls[i] = new ShopItem(this, 8 + (i % 2) * 142, 75 + (i / 2) * 55, 143, 56);
                itemControls[i].Init();
            }
            nlPageSelector1.TotalPage = (items.Length - 1) / 6 + 1;
            refreshInfo();
        }

        private void refreshInfo()
        {
            for (int i = 0; i < 6; i++)
            {
                if (page*6 + i < items.Length)
                {
                    itemControls[i].RefreshData(items[page*6 + i].Id, items[page*6 + i].Value);
                }
                else
                {
                    itemControls[i].RefreshData(0, 0);
                }
            }
        }

        private void pictureBoxCancel_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void ShopWindow_Paint(object sender, System.Windows.Forms.PaintEventArgs e)
        {
            if (npcId > 0)
            {
                Image bgImage = PicLoader.Read("System", "TalkBack.PNG");
                e.Graphics.DrawImage(bgImage, 0, 0, bgImage.Width, bgImage.Height);
                bgImage.Dispose();
                e.Graphics.DrawImage(NPCBook.GetPersonImage(npcId), 24, 0, 70, 70);

                Font font = new Font("宋体", 11*1.33f, FontStyle.Bold, GraphicsUnit.Pixel);
                e.Graphics.DrawString(ConfigData.GetNpcConfig(npcId).Name, font, Brushes.Chocolate, 131, 50);
                font.Dispose();

                foreach (ShopItem ctl in itemControls)
                {
                    ctl.Draw(e.Graphics);
                }
            }
        }

        private void nlPageSelector1_PageChange(int pg)
        {
            page = pg;
            refreshInfo();
        }
    }
}