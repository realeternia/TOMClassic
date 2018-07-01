using System;
using System.Drawing;
using System.Windows.Forms;
using ControlPlus;
using TaleofMonsters.Core.Loader;
using TaleofMonsters.Datas.Scenes;
using TaleofMonsters.Datas.User;
using TaleofMonsters.Forms.CMain;
using TaleofMonsters.Forms.Items.Core;
using TaleofMonsters.Forms.Items.Regions;

namespace TaleofMonsters.Forms
{
    internal partial class BlackStoneForm : BasePanel
    {
        private Image backImage;
        private ImageToolTip tooltip = SystemToolTip.Instance;
        private VirtualRegion vRegion;

        public BlackStoneForm()
        {
            InitializeComponent();
            this.bitmapButtonClose.ImageNormal = PicLoader.Read("Button.Panel", "CloseButton1.JPG");
            this.bitmapButtonC1.ImageNormal = PicLoader.Read("Button.Panel", "ButtonBack2.PNG");
            bitmapButtonC1.Font = new Font("宋体", 8 * 1.33f, FontStyle.Regular, GraphicsUnit.Pixel);
            bitmapButtonC1.ForeColor = Color.White;
            bitmapButtonC1.IconImage = TaleofMonsters.Core.HSIcons.GetIconsByEName("tsk7");
            bitmapButtonC1.IconSize = new Size(16, 16);
            bitmapButtonC1.IconXY = new Point(4, 5);
            bitmapButtonC1.TextOffX = 8;
            DoubleBuffered = true;
        }

        public override void Init(int width, int height)
        {
            base.Init(width, height);

            var stoneId = DungeonBook.GetDungeonItemId("diheiyao");
            vRegion = new VirtualRegion(this);
            vRegion.AddRegion(new PictureRegion(10, 40, 120, 40, 40, PictureRegionCellType.DungeonItem, stoneId));

            backImage = PicLoader.Read("System", "StoneBack.JPG");
            vRegion.RegionEntered += new VirtualRegion.VRegionEnteredEventHandler(virtualRegion_RegionEntered);
            vRegion.RegionLeft += new VirtualRegion.VRegionLeftEventHandler(virtualRegion_RegionLeft);
        }

        public override void OnFrame(int tick, float timePass)
        {
            base.OnFrame(tick, timePass);
        }

        private void bitmapButtonC1_Click(object sender, EventArgs e)
        {

            AddFlowCenter("合成成功", "Lime");
        }

        private void virtualRegion_RegionEntered(int id, int x, int y, int key)
        {
            var region = vRegion.GetRegion(id);
            if (region != null)
                region.ShowTip(tooltip, this, x, y);
        }

        private void virtualRegion_RegionLeft()
        {
            tooltip.Hide(this);
        }

        private void BlackStoneForm_Paint(object sender, PaintEventArgs e)
        {
            BorderPainter.Draw(e.Graphics, "", Width, Height);

            Font font = new Font("黑体", 12 * 1.33f, FontStyle.Bold, GraphicsUnit.Pixel);
            e.Graphics.DrawString("黑曜石兑换", font, Brushes.White, Width / 2 - 40, 8);
            font.Dispose();

            int xOff = 13;
            int yOff = 107;
            e.Graphics.DrawImage(backImage, xOff, yOff, 324, 244);

            vRegion.Draw(e.Graphics);

            var stoneId = DungeonBook.GetDungeonItemId("diheiyao");
            var itemCount = UserProfile.InfoDungeon.GetDungeonItemCount(stoneId);
            Font font2 = new Font("宋体", 13 * 1.33f, FontStyle.Bold, GraphicsUnit.Pixel);
            e.Graphics.DrawString(string.Format("x{0}", itemCount), font2, Brushes.Black, xOff + 75 + 1, yOff + 32 + 1);
            e.Graphics.DrawString(string.Format("x{0}", itemCount), font2, Brushes.White, xOff + 75, yOff + 32);
            font2.Dispose();
        }

        private void bitmapButtonClose_Click(object sender, EventArgs e)
        {
            Close();
        }
    }
}
