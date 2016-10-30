using System;
using System.Drawing;
using System.Windows.Forms;
using TaleofMonsters.Controler.Loader;
using TaleofMonsters.Core;
using TaleofMonsters.Forms.Items.Core;
using TaleofMonsters.Forms.Items.Regions;
using TaleofMonsters.Forms.Items.Regions.Decorators;

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
            int xOff = 15;
            int yOff = 40;
            AddBookRegion(1, xOff + 55, yOff + 35, 51000129, "卡牌手册");
            AddBookRegion(2, xOff + 55 + 90, yOff + 35,  53000014, "技能手册");
            AddBookRegion(3, xOff + 55 + 180, yOff + 35, 53000098,"对手分析");
            AddBookRegion(4, xOff + 55 + 270, yOff + 35, 52000061,"材料大全");
            virtualRegion.RegionClicked += new VirtualRegion.VRegionClickEventHandler(virtualRegion_RegionClicked);
            virtualRegion.RegionEntered+=new VirtualRegion.VRegionEnteredEventHandler(virtualRegion_RegionEntered);
            virtualRegion.RegionLeft+=new VirtualRegion.VRegionLeftEventHandler(virtualRegion_RegionLeft);
        }

        private void AddBookRegion(int id,int x, int y, int cardId, string text)
        {
            RegionTextDecorator textControl;
            var region = new PictureAnimRegion(id, x, y, 76, 100, id, VirtualRegionCellType.Card, cardId);
            textControl = new RegionTextDecorator(region, 3, 80, 10, Color.Lime, true);
            textControl.SetState(text);
            region.AddDecorator(textControl);
            virtualRegion.AddRegion(region);
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

        private void MagicBookViewForm_Paint(object sender, PaintEventArgs e)
        {
            BorderPainter.Draw(e.Graphics, "", Width, Height);

            Font font = new Font("黑体", 12*1.33f, FontStyle.Bold, GraphicsUnit.Pixel);
            e.Graphics.DrawString(" 魔法书 ", font, Brushes.White, 300, 8);
            font.Dispose();

            Image back = PicLoader.Read("System", "MagicBookBack.JPG");
            e.Graphics.DrawImage(back, 5, 35, 672, 420);
            back.Dispose();

            virtualRegion.Draw(e.Graphics);
        }

    }
}