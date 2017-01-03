using System;
using System.Drawing;
using System.Windows.Forms;
using NarlonLib.Math;
using ControlPlus;
using TaleofMonsters.DataType.User;
using TaleofMonsters.Forms.Items.Core;
using TaleofMonsters.Controler.Loader;
using TaleofMonsters.Forms.Items.Regions;
using TaleofMonsters.Forms.Items.Regions.Decorators;
using TaleofMonsters.MainItem;

namespace TaleofMonsters.Forms.MiniGame
{
    internal partial class MGUpToNumber : BasePanel
    {
        private bool show;
        private Image backImage;
        private int type;

        private VirtualRegion virtualRegion;

        private int[] itemRequired;
        private int[] itemGet;
        private int level;
        private bool isFail;

        private const int xoff=11;
        private const int yoff=129;

        public MGUpToNumber()
        {
            type = (int)SystemMenuIds.GameUpToNumber;
            InitializeComponent();

            virtualRegion = new VirtualRegion(this);
            for (int i = 0; i < 4; i++)
            {
                ButtonRegion region = new ButtonRegion(i + 1, 60 + 55 * i, 310, 50, 50,"GameUpToNumber1.PNG", "GameUpToNumber1On.PNG");
                region.AddDecorator(new RegionTextDecorator(10, 20, 10));
                virtualRegion.AddRegion(region);
            }
            virtualRegion.SetRegionDecorator(1, 0, "牛肉");
            virtualRegion.SetRegionDecorator(2, 0, "蜂蜜");
            virtualRegion.SetRegionDecorator(3, 0, "黄油");
            virtualRegion.SetRegionDecorator(4, 0, " 水");

            virtualRegion.RegionClicked += new VirtualRegion.VRegionClickEventHandler(virtualRegion_RegionClicked);
        }

        internal override void Init(int width, int height)
        {
            base.Init(width, height);
            this.bitmapButtonC1.ImageNormal = PicLoader.Read("ButtonBitmap", "ButtonBack2.PNG");
            bitmapButtonC1.Font = new Font("宋体", 8 * 1.33f, FontStyle.Regular, GraphicsUnit.Pixel);
            bitmapButtonC1.ForeColor = Color.White;
            bitmapButtonC1.IconImage = TaleofMonsters.Core.HSIcons.GetIconsByEName("res2");
            bitmapButtonC1.IconSize = new Size(16, 16);
            bitmapButtonC1.IconXY = new Point(4, 5);
            bitmapButtonC1.TextOffX = 8;
            this.bitmapButtonClose.ImageNormal = PicLoader.Read("ButtonBitmap", "CloseButton1.JPG");
            backImage = PicLoader.Read("MiniGame", "t1.JPG");
            show = true;

            itemRequired = new int[] {8, 5, 4, 15};
            RestartGame();
        }

        public void RestartGame()
        {
            itemGet = new int[4];
            level = 0;
            isFail = false;
            changeFood(1);
        }

        public void EndGame()
        {
            if (!UserProfile.Profile.InfoMinigame.Has(type))
            {
                UserProfile.Profile.InfoMinigame.Add(type);
            }

            string hint;
            if (!isFail && getPoints()>=100)
            {
                hint = "获得了游戏胜利";
                UserProfile.InfoBag.AddDiamond(10);
            }
            else
            {
                hint = "你输了";
            }

            if (MessageBoxEx2.Show(hint + ",是否花5钻石再试一次?") == DialogResult.OK)
            {
                if (UserProfile.InfoBag.PayDiamond(5))
                {
                    RestartGame();
                    return;
                }
            }

            Close();
        }

        private void bitmapButtonC1_Click(object sender, EventArgs e)
        {
            int get =  MathTool.GetRandom(4) + 2;
            itemGet[level] += get;
            string[] itemNames = {"牛肉","蜂蜜","黄油","水"};
            AddFlowCenter(string.Format("{0}x{1}", itemNames[level], get), "LimeGreen");
            if (itemGet[level]>itemRequired[level])
            {
                isFail = true;
                EndGame();
            }
            else if(getPoints()>=100)
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
                        changeFood(rlevel + 1);
                        break;
                    }
                }
            }
        }

        private int getPoints()
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
                changeFood(id);
            }
        }

        private void changeFood(int id)
        {
            level = id - 1;
            for (int i = 0; i < 4; i++)
            {
                virtualRegion.SetRegionState(i + 1, RegionState.Free);
            }
            virtualRegion.SetRegionState(id, RegionState.Rectangled);
            Invalidate(new Rectangle(xoff, yoff, 324, 244));
        }

        private void bitmapButtonClose_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void DrawShadeText(Graphics g, string text, Font font, Brush Brush, int x, int y)
        {
            g.DrawString(text, font, Brushes.Black, x + 1, y + 1);
            g.DrawString(text, font, Brush, x, y);
        }

        private void MGUpToNumber_Paint(object sender, PaintEventArgs e)
        {
            BorderPainter.Draw(e.Graphics, "", Width, Height);

            Font font = new Font("黑体", 12*1.33f, FontStyle.Bold, GraphicsUnit.Pixel);
            e.Graphics.DrawString("烹饪", font, Brushes.White, 150, 8);
            font.Dispose();

            if (!show)
                return;

            e.Graphics.DrawImage(backImage, xoff, yoff, 324, 244);

            virtualRegion.Draw(e.Graphics);

            font = new Font("宋体", 12*1.33f, FontStyle.Bold, GraphicsUnit.Pixel);
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
            DrawShadeText(e.Graphics,string.Format(string.Format("{0}/100",getPoints())), font, Brushes.Gold, 105+xoff, 140+yoff);
            font.Dispose();
        }
    }
}
