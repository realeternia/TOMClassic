using System;
using System.Drawing;
using System.Windows.Forms;
using NarlonLib.Math;
using TaleofMonsters.Core;
using TaleofMonsters.Core.Loader;
using TaleofMonsters.Forms.Items.Regions;
using TaleofMonsters.Forms.Items.Regions.Decorators;

namespace TaleofMonsters.Forms.MiniGame
{
    internal partial class MGOvercome : MGBase
    {
        private enum WinState
        {
            None, Win, Loss, Draw
        }

        private VirtualRegion vRegion;

        private int myChoice;
        private int rivalChoice;
        private WinState state;

        private int round;

        public MGOvercome()
        {
            InitializeComponent();
            this.bitmapButtonC1.ImageNormal = PicLoader.Read("Button.Panel", "ButtonBack2.PNG");
            bitmapButtonC1.Font = new Font("宋体", 8 * 1.33f, FontStyle.Regular, GraphicsUnit.Pixel);
            bitmapButtonC1.ForeColor = Color.White;
            bitmapButtonC1.IconImage = TaleofMonsters.Core.HSIcons.GetIconsByEName("hatt1");
            bitmapButtonC1.IconSize = new Size(16, 16);
            bitmapButtonC1.IconXY = new Point(4, 5);
            bitmapButtonC1.TextOffX = 8;
            vRegion = new VirtualRegion(this);
            string[] txt = {" 水", " 风", " 火", " 光", " 暗"};
            for (int i = 0; i < 5; i++)
            {
                ButtonRegion region = new ButtonRegion(i + 1, 35 + 55 * i, 310, 50, 50,"GameBackNormal1.PNG", "GameBackNormal1On.PNG");
                region.AddDecorator(new RegionTextDecorator(10, 20, 10, txt[i]));
                vRegion.AddRegion(region);
            }

            vRegion.RegionClicked += new VirtualRegion.VRegionClickEventHandler(virtualRegion_RegionClicked);
        }

        public override void Init(int width, int height)
        {
            base.Init(width, height);
            
            RestartGame();
        }

        public override void RestartGame()
        {
            base.RestartGame();
            state = WinState.None;
            myChoice = 0;
            rivalChoice = 0;
            round = 0;
            ChangeElement(1);
        }

        public override void EndGame()
        {
            base.EndGame();
        }

        private WinState[,] winTable = new WinState[,]
        {
            {WinState.Draw, WinState.Loss, WinState.Win, WinState.Win, WinState.Loss},
            {WinState.Win, WinState.Draw, WinState.Loss, WinState.Win, WinState.Loss},
            {WinState.Loss, WinState.Win, WinState.Draw, WinState.Win, WinState.Loss},
            {WinState.Loss, WinState.Loss, WinState.Loss, WinState.Draw, WinState.Win},
            {WinState.Win, WinState.Win, WinState.Win, WinState.Loss, WinState.Draw},
        };
        private void bitmapButtonC1_Click(object sender, EventArgs e)
        {
            round++;
            rivalChoice = RivalChoose();
            state = winTable[myChoice, rivalChoice];
            if (state == WinState.Win)
                score += 3;
            if (state == WinState.Draw)
                score += 1;
            Invalidate(new Rectangle(xoff, yoff, 324, 244));

            if (round >= 10)
            {
                EndGame();
            }
        }

        private int RivalChoose()
        {
            int roll = MathTool.GetRandom(100);
            if (roll > 70)
            {
                return 4;//暗
            }
            if (roll > 30)
            {
                return 3;//光
            }
            return roll/10;
        }

        void virtualRegion_RegionClicked(int id, int x, int y, MouseButtons button)
        {
            if (button == MouseButtons.Left)
            {
                ChangeElement(id);
            }
        }

        private void ChangeElement(int id)
        {
            myChoice = id - 1;
            for (int i = 0; i < 5; i++)
            {
                vRegion.SetRegionEffect(i + 1, RegionEffect.Free);
            }
            vRegion.SetRegionEffect(id, RegionEffect.Rectangled);
            state = WinState.None;
            Invalidate(new Rectangle(xoff, yoff, 324, 244));
        }

        private void MGOvercome_Paint(object sender, PaintEventArgs e)
        {
            DrawBase(e.Graphics);

            if (!show)
                return;

            vRegion.Draw(e.Graphics);

            var left = HSIcons.GetIconsByEName(GetIcon(myChoice));
            e.Graphics.DrawImage(left, 50, 160, 80, 80);

            var right = HSIcons.GetIconsByEName(GetIcon(rivalChoice));
            e.Graphics.DrawImage(right, 220, 160, 80, 80);

            var font = new Font("宋体", 26*1.33f, FontStyle.Bold, GraphicsUnit.Pixel);
            DrawShadeText(e.Graphics,string.Format(string.Format("{0}战 {1}分", round, score)), font, Brushes.White, 90+xoff, 140+yoff);
            font.Dispose();

            if (state != WinState.None)
            {
                font = new Font("宋体", 26 * 1.33f, FontStyle.Bold, GraphicsUnit.Pixel);
                DrawShadeText(e.Graphics, state.ToString(), font, Brushes.White, 130, 190);
                font.Dispose();
            }
        }

        private static string GetIcon(int index)
        {
            switch (index)
            {
                case 0: return "atr1";
                case 1: return "atr2";
                case 2: return "atr3";
                case 3: return "atr5";
                case 4: return "atr6";
            }
            return "";
        }
    }
}
