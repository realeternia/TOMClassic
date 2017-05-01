using System;
using System.Drawing;
using System.Windows.Forms;
using NarlonLib.Math;
using TaleofMonsters.Controler.Loader;
using TaleofMonsters.Forms.Items.Regions;
using TaleofMonsters.Forms.Items.Regions.Decorators;

namespace TaleofMonsters.Forms.MiniGame
{
    internal partial class MGUpToNumber : MGBase
    {
        private VirtualRegion virtualRegion;

        private int[] itemRequired;
        private int[] itemGet;
        private int level;

        public MGUpToNumber()
        {
            InitializeComponent();
            this.bitmapButtonC1.ImageNormal = PicLoader.Read("ButtonBitmap", "ButtonBack2.PNG");
            bitmapButtonC1.Font = new Font("宋体", 8 * 1.33f, FontStyle.Regular, GraphicsUnit.Pixel);
            bitmapButtonC1.ForeColor = Color.White;
            bitmapButtonC1.IconImage = TaleofMonsters.Core.HSIcons.GetIconsByEName("res2");
            bitmapButtonC1.IconSize = new Size(16, 16);
            bitmapButtonC1.IconXY = new Point(4, 5);
            bitmapButtonC1.TextOffX = 8;
            virtualRegion = new VirtualRegion(this);
            for (int i = 0; i < 4; i++)
            {
                ButtonRegion region = new ButtonRegion(i + 1, 60 + 55 * i, 310, 50, 50, "GameBackNormal1.PNG", "GameBackNormal1On.PNG");
                region.AddDecorator(new RegionTextDecorator(10, 20, 10));
                virtualRegion.AddRegion(region);
            }
            virtualRegion.SetRegionDecorator(1, 0, "牛肉");
            virtualRegion.SetRegionDecorator(2, 0, "蜂蜜");
            virtualRegion.SetRegionDecorator(3, 0, "黄油");
            virtualRegion.SetRegionDecorator(4, 0, " 水");

            virtualRegion.RegionClicked += new VirtualRegion.VRegionClickEventHandler(virtualRegion_RegionClicked);
        }

        public override void Init(int width, int height)
        {
            base.Init(width, height);
            
            itemRequired = new int[] {8, 5, 4, 15};
            RestartGame();
        }

        public override void RestartGame()
        {
            base.RestartGame();
            itemGet = new int[4];
            level = 0;
            ChangeFood(1);
        }

        public override void EndGame()
        {
            base.EndGame();
        }

        private void bitmapButtonC1_Click(object sender, EventArgs e)
        {
            int get =  MathTool.GetRandom(4) + 2;
            itemGet[level] += get;
            string[] itemNames = {"牛肉","蜂蜜","黄油","水"};
            AddFlowCenter(string.Format("{0}x{1}", itemNames[level], get), "LimeGreen");
            score = GetPoints();
            if (itemGet[level] > itemRequired[level] || score >= 100)
            {
                EndGame();
            }
            else if (itemGet[level] == itemRequired[level])
            {
                for (int i = 1; i < 4; i++)
                {
                    int rlevel = (i + level)%4;
                    if (itemGet[rlevel] != itemRequired[rlevel])
                    {
                        ChangeFood(rlevel + 1);
                        break;
                    }
                }
            }
        }

        private int GetPoints()
        {
            int[] marks = {5, 4, 5, 2};
            int mk = 0;
            for (int i = 0; i < 4; i++)
            {
                mk += marks[i]*itemGet[i];
            }
            return mk;
        }

        void virtualRegion_RegionClicked(int id, int x, int y, MouseButtons button)
        {
            if (button == MouseButtons.Left)
            {
                ChangeFood(id);
            }
        }

        private void ChangeFood(int id)
        {
            level = id - 1;
            for (int i = 0; i < 4; i++)
            {
                virtualRegion.SetRegionState(i + 1, RegionState.Free);
            }
            virtualRegion.SetRegionState(id, RegionState.Rectangled);
            Invalidate(new Rectangle(xoff, yoff, 324, 244));
        }

        private void MGUpToNumber_Paint(object sender, PaintEventArgs e)
        {
            DrawBase(e.Graphics);

            if (!show)
                return;

            virtualRegion.Draw(e.Graphics);

            var font = new Font("宋体", 12*1.33f, FontStyle.Bold, GraphicsUnit.Pixel);
            DrawShadeText(e.Graphics, string.Format("牛肉x{0}", itemRequired[0]), font, level == 0 ? Brushes.LightGreen : Brushes.White, xoff+5, 20+yoff);
            DrawShadeText(e.Graphics, string.Format("蜂蜜x{0}", itemRequired[1]), font, level == 1 ? Brushes.LightGreen : Brushes.White, xoff + 5, 50 + yoff);
            DrawShadeText(e.Graphics, string.Format("黄油x{0}", itemRequired[2]), font, level == 2 ? Brushes.LightGreen : Brushes.White, xoff + 5, 80 + yoff);
            DrawShadeText(e.Graphics, string.Format(" 水 x{0}", itemRequired[3]), font, level == 3 ? Brushes.LightGreen : Brushes.White, xoff + 5, 110 + yoff);
            font.Dispose();

            for (int i = 0; i < 4; i++)
            {
                Image img = PicLoader.Read("MiniGame.Soup", string.Format("item{0}.PNG", i + 1));
                for (int j = 0; j < itemGet[i]; j++)
                {
                    e.Graphics.DrawImage(img, 80+xoff + j * 12, 17+yoff + 30 * i, 24, 24);
                }
                img.Dispose();
            }

            font = new Font("宋体", 26*1.33f, FontStyle.Bold, GraphicsUnit.Pixel);
            DrawShadeText(e.Graphics,string.Format(string.Format("{0}/100",GetPoints())), font, Brushes.Gold, 105+xoff, 140+yoff);
            font.Dispose();
        }
    }
}
