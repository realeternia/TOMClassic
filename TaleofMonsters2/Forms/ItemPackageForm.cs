using System;
using System.Drawing;
using System.Windows.Forms;
using NarlonLib.Control;
using TaleofMonsters.Core.Loader;
using TaleofMonsters.Datas.Items;
using TaleofMonsters.Forms.CMain;
using TaleofMonsters.Forms.Items.Core;
using TaleofMonsters.Forms.Items.Regions;
using TaleofMonsters.Forms.Items.Regions.Decorators;

namespace TaleofMonsters.Forms
{
    internal sealed partial class ItemPackageForm : BasePanel
    {
        private VirtualRegion vRegion;
        private ImageToolTip tooltip = SystemToolTip.Instance;
        private int[] itemPos;
        private int[] itemIds; //卡包id

        public ItemPackageForm()
        {
            InitializeComponent();
            this.bitmapButtonClose.ImageNormal = PicLoader.Read("Button.Panel", "CloseButton1.JPG");

            vRegion = new VirtualRegion(this);
            
            vRegion.RegionEntered += OnVRegionEntered;
            vRegion.RegionLeft += OnVRegionLeft;
        }

        internal void SetItem(int[] items, int[] count)
        {
            itemIds = items;

            itemPos = new int[itemIds.Length * 2];
            for (int i = 0; i < itemIds.Length; i++)
            {
                itemPos[i * 2] = 20 + 55 * (i%3);
                itemPos[i * 2 + 1] = 46 + 55 * (i / 3);
            }

            for (int i = 0; i < itemIds.Length; i++)
            {
                var region = new PictureRegion(1 + i, itemPos[i * 2], itemPos[i * 2 + 1], 50, 50, PictureRegionCellType.Item, itemIds[i]);
                region.AddDecorator(new RegionTextDecorator(30,30,12,Color.White,true));
                vRegion.AddRegion(region);
                vRegion.SetRegionDecorator(1 + i, 0, count[i].ToString());
            }
        }

        public override void OnFrame(int tick, float timePass)
        {
            base.OnFrame(tick, timePass);
        }

        private void buttonClose_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void ItemPackageForm_Paint(object sender, PaintEventArgs e)
        {
            BorderPainter.Draw(e.Graphics, "", Width, Height);

            Image img = PicLoader.Read("System", "ItemBackDown2.JPG");
            e.Graphics.DrawImage(img, 10, 35, 185, 185);
            img.Dispose();

            vRegion.Draw(e.Graphics);

            Font font = new Font("黑体", 12*1.33f, FontStyle.Bold, GraphicsUnit.Pixel);
            e.Graphics.DrawString("礼 包", font, Brushes.White, Width/2 - 40, 8);
            font.Dispose();
        }

        private void OnVRegionEntered(int id, int x, int y, int key)
        {
            if (key > 0)
            {
                Image image = HItemBook.GetPreview(key);
                tooltip.Show(image, this, x, y);
            }
            else
            {
                tooltip.Hide(this);
            }
        }

        private void OnVRegionLeft()
        {
            tooltip.Hide(this);
        }
    }
}