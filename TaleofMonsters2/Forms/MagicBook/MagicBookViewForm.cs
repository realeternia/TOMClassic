using System;
using System.Drawing;
using System.Windows.Forms;
using TaleofMonsters.Controler.Loader;
using TaleofMonsters.Core;
using TaleofMonsters.Forms.Items.Core;
using TaleofMonsters.Forms.Items.Regions;

namespace TaleofMonsters.Forms.MagicBook
{
    internal sealed partial class MagicBookViewForm : BasePanel
    {
        private VirtualRegion virtualRegion;
        private int last = -1;

        public MagicBookViewForm()
        {
            InitializeComponent();
            this.bitmapButtonClose.ImageNormal = PicLoader.Read("ButtonBitmap", "CloseButton1.JPG");
            virtualRegion = new VirtualRegion(this);
            virtualRegion.AddRegion(new SubVirtualRegion(1, 59, 84, 250, 19, 1));
            virtualRegion.AddRegion(new SubVirtualRegion(2, 59, 114, 250, 19, 2));
            virtualRegion.AddRegion(new SubVirtualRegion(3, 59, 144, 250, 19, 3));
            virtualRegion.AddRegion(new SubVirtualRegion(4, 59, 174, 250, 19, 4));
            virtualRegion.AddRegion(new SubVirtualRegion(5, 59, 204, 250, 19, 5));
            virtualRegion.RegionClicked += new VirtualRegion.VRegionClickEventHandler(virtualRegion_RegionClicked);
            virtualRegion.RegionEntered+=new VirtualRegion.VRegionEnteredEventHandler(virtualRegion_RegionEntered);
            virtualRegion.RegionLeft+=new VirtualRegion.VRegionLeftEventHandler(virtualRegion_RegionLeft);
        }

        internal override void Init(int width, int height)
        {
            base.Init(width, height);

            SoundManager.PlayBGM("TOM004.MP3");
            IsChangeBgm = true;
        }

        private void virtualRegion_RegionLeft()
        {
            last = -1;
            Invalidate();
        }

        private void virtualRegion_RegionEntered(int info, int i, int y1, int key)
        {
            last = info;
            Invalidate();
        }

        private void virtualRegion_RegionClicked(int info, MouseButtons button)
        {
            switch (info)
            {
                case 1:
                    MainForm.Instance.DealPanel(new CardViewForm());
                    break;
                case 2:
                    MainForm.Instance.DealPanel(new MonsterSkillViewForm());
                    break;
                case 3:
                    MainForm.Instance.DealPanel(new PeopleDeckViewForm());
                    break;
                case 4:
                    MainForm.Instance.DealPanel(new DropItemViewerForm());
                    break;
            }
        }

        private void buttonClose_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void CardShopViewForm_Paint(object sender, PaintEventArgs e)
        {
            BorderPainter.Draw(e.Graphics, "", Width, Height);

            Font font = new Font("黑体", 12*1.33f, FontStyle.Bold, GraphicsUnit.Pixel);
            e.Graphics.DrawString(" 魔法书 ", font, Brushes.White, 300, 8);
            font.Dispose();

            Image back = PicLoader.Read("System", "MagicBookBack.JPG");
            e.Graphics.DrawImage(back, 15, 35, 672, 420);
            back.Dispose();

            font = new Font("华文行楷", 15*1.33f, FontStyle.Regular, GraphicsUnit.Pixel);
            e.Graphics.DrawString("查看所有全卡片效果", font, last == 1 ? Brushes.Yellow : Brushes.Black, 59, 84);
            e.Graphics.DrawString("查看所有卡片技能效果", font, last == 2 ? Brushes.Yellow : Brushes.Black, 59, 114);
            e.Graphics.DrawString("查看所有对手卡组", font, last == 3? Brushes.Yellow : Brushes.Black, 59, 144);
            e.Graphics.DrawString("查看所有材料掉落", font, last == 4? Brushes.Yellow : Brushes.Black, 59, 174);
            font.Dispose();
        }

    }
}