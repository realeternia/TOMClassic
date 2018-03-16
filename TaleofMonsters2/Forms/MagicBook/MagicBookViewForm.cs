using System;
using System.Drawing;
using System.Windows.Forms;
using TaleofMonsters.Core;
using TaleofMonsters.Core.Loader;
using TaleofMonsters.Forms.CMain;
using TaleofMonsters.Forms.Items.Core;
using TaleofMonsters.Forms.Items.Regions;
using TaleofMonsters.Forms.Items.Regions.Decorators;

namespace TaleofMonsters.Forms.MagicBook
{
    internal sealed partial class MagicBookViewForm : BasePanel
    {
        private VirtualRegion vRegion;

        public MagicBookViewForm()
        {
            InitializeComponent();
            this.bitmapButtonClose.ImageNormal = PicLoader.Read("Button.Panel", "CloseButton1.JPG");
            vRegion = new VirtualRegion(this);
            int xOff = 15;
            int yOff = 40;
            AddBookRegion(1, xOff + 55, yOff + 35, 51000129, "卡牌手册");
            AddBookRegion(2, xOff + 55 + 90, yOff + 35,  53000014, "技能手册");
            AddBookRegion(3, xOff + 55 + 180, yOff + 35, 53000098,"对手分析");
            AddBookRegion(4, xOff + 55, yOff + 35 + 120, 52000061,"材料大全");
            AddBookRegion(5, xOff + 55 + 90, yOff + 35 +120, 53000110, "掉落系谱");
            vRegion.RegionClicked += new VirtualRegion.VRegionClickEventHandler(virtualRegion_RegionClicked);
            vRegion.RegionEntered+=new VirtualRegion.VRegionEnteredEventHandler(virtualRegion_RegionEntered);
            vRegion.RegionLeft+=new VirtualRegion.VRegionLeftEventHandler(virtualRegion_RegionLeft);
        }

        private void AddBookRegion(int id,int x, int y, int cardId, string text)
        {
            RegionTextDecorator textControl;
            var region = new PictureAnimRegion(id, x, y, 76, 100, PictureRegionCellType.Card, cardId);
            textControl = new RegionTextDecorator(3, 80, 10, Color.Lime, true);
            textControl.SetState(text);
            region.AddDecorator(textControl);
            vRegion.AddRegion(region);
        }

        public override void Init(int width, int height)
        {
            base.Init(width, height);

            SoundManager.PlayBGM("TOM004.mp3");
            IsChangeBgm = true;
        }

        private void virtualRegion_RegionLeft()
        {
            Invalidate();
        }

        private void virtualRegion_RegionEntered(int info, int i, int y1, int key)
        {
            Invalidate();
        }

        private void virtualRegion_RegionClicked(int id, int x, int y, MouseButtons button)
        {
            switch (id)
            {
                case 1:
                    PanelManager.DealPanel(new CardViewForm());
                    break;
                case 2:
                    PanelManager.DealPanel(new MonsterSkillViewForm());
                    break;
                case 3:
                    PanelManager.DealPanel(new PeopleDeckViewForm());
                    break;
                case 4:
                    PanelManager.DealPanel(new DropItemViewerForm());
                    break;
                case 5:
                    PanelManager.DealPanel(new CardDropViewForm());
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

            vRegion.Draw(e.Graphics);
        }

    }
}