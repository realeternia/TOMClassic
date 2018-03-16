using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using NarlonLib.Math;
using TaleofMonsters.Core.Loader;
using TaleofMonsters.Forms.Items.Regions.Decorators;
using TaleofMonsters.Forms.Items.Regions;

namespace TaleofMonsters.Forms.MiniGame
{
    internal partial class MGRacing : MGBase
    {
        private class RaceSegData
        {
            public enum HardnessType
            {
                Easy,
                Med,
                Danger
            }

            public int Length;
            public HardnessType Hardness;
        }
        private const int SpeedSlow = 1;
        private const int SpeedMed = 2;
        private const int SpeedHigh = 3;
        private const int SpeedSuper = 4;

        private VirtualRegion vRegion;

        private const int RaceTotal = 15; //赛段总数
        private int raceId; //第几赛段
        private List<RaceSegData> raceSeg;

        private int distanceDiffer; //距离差

        private int speed; //当前的速度模式

        public MGRacing()
        {
            InitializeComponent();
            this.bitmapButtonC1.ImageNormal = PicLoader.Read("Button.Panel", "ButtonBack2.PNG");
            bitmapButtonC1.Font = new Font("宋体", 8 * 1.33f, FontStyle.Regular, GraphicsUnit.Pixel);
            bitmapButtonC1.ForeColor = Color.White;
            bitmapButtonC1.IconImage = TaleofMonsters.Core.HSIcons.GetIconsByEName("rot3");
            bitmapButtonC1.IconSize = new Size(16, 16);
            bitmapButtonC1.IconXY = new Point(4, 5);
            bitmapButtonC1.TextOffX = 8;
            vRegion = new VirtualRegion(this);
            for (int i = 0; i < 3; i++)
            {
                ButtonRegion region = new ButtonRegion(i + 1, 40 + 70 * i, 310, 50, 50, "GameBackNormal1.PNG", "GameBackNormal1On.PNG");
                region.AddDecorator(new RegionTextDecorator(10, 20, 10));
                vRegion.AddRegion(region);
            }
            var region2 = new ButtonRegion(4, 40 + 55 * 4, 310, 50, 50, "GameBackNormal2.PNG", "GameBackNormal1On.PNG");
            region2.AddDecorator(new RegionTextDecorator(10, 20, 10));
            vRegion.AddRegion(region2);
            vRegion.SetRegionDecorator(1, 0, "低速");
            vRegion.SetRegionDecorator(2, 0, "中速");
            vRegion.SetRegionDecorator(3, 0, "高速");
            vRegion.SetRegionDecorator(4, 0, "氮气");
            
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
            raceSeg = new List<RaceSegData>();
            for (int i = 0; i < RaceTotal; i++)
            {
                RaceSegData race = new RaceSegData();
                int roll = MathTool.GetRandom(100);
                if (roll > 80)
                    race.Length = MathTool.GetRandom(1300, 1500); //长
                else if (roll > 30)
                    race.Length = MathTool.GetRandom(850, 1150); //中
                else
                    race.Length = MathTool.GetRandom(500, 700); //短
                roll = MathTool.GetRandom(100);
                if (roll > 80)
                    race.Hardness = RaceSegData.HardnessType.Danger;
                else if (roll > 30)
                    race.Hardness = RaceSegData.HardnessType.Med;
                else
                    race.Hardness = RaceSegData.HardnessType.Easy;
                raceSeg.Add(race);
            }
            raceId = 0;
            distanceDiffer = 0;
            ChangeWork(SpeedMed);            
        }

        public override void EndGame()
        {
            base.EndGame();
        }

        private void ChangeWork(int id)
        {
            speed = id;
            for (int i = 0; i < 4; i++)
            {
                vRegion.SetRegionState(i + 1, RegionState.Free);
            }
            vRegion.SetRegionState(id, RegionState.Rectangled);
            Invalidate(new Rectangle(xoff, yoff, 324, 244));
        }

        private void bitmapButtonC1_Click(object sender, EventArgs e)
        {
            var raceData = raceSeg[raceId];
            var playerSpd = GetSpeed(speed, (int)raceData.Hardness);

            var aiSpdTp= AIChooseSpd(distanceDiffer, raceData.Hardness);
            var aiSpd = GetSpeed(aiSpdTp, (int)raceData.Hardness);

            if (playerSpd > aiSpd)
                distanceDiffer += raceData.Length*(playerSpd - aiSpd)/playerSpd;
            else if (playerSpd < aiSpd)
                distanceDiffer -= raceData.Length * (aiSpd - playerSpd) / aiSpd;

            if (raceId >= RaceTotal - 1)
                EndGame();
            else
                raceId++;
            score = Math.Max(0, distanceDiffer);
            Invalidate(new Rectangle(xoff, yoff, 324, 244));
        }

        private static int[,] breakTable = new int[,]
        {
            {0, 1, 3},
            {3, 7, 15},
            {10, 20, 30},
            {25, 35, 45},
        };

        private static int GetSpeed(int type, int danger)
        {
            if (MathTool.GetRandom(100) < breakTable[type - 1, danger])
                return 0;

            int realSpd = 0;
            if (type == SpeedSlow)
                realSpd = MathTool.GetRandom(50, 70);
            else if (type == SpeedMed)
                realSpd = MathTool.GetRandom(90, 110);
            else if (type == SpeedHigh)
                realSpd = MathTool.GetRandom(130, 150);
            else if (type == SpeedSuper)
                realSpd = MathTool.GetRandom(200, 250);
            return realSpd;
        }


        private static int AIChooseSpd(int diff, RaceSegData.HardnessType hard)
        {
            var spd = SpeedMed;
            if (hard == RaceSegData.HardnessType.Easy)
                spd = SpeedHigh;
            if (diff > 500 && hard != RaceSegData.HardnessType.Danger) //落后太多
                spd++;
            if (diff < -500 && hard != RaceSegData.HardnessType.Easy) //领先太多
                spd--;
            return spd;
        }

        void virtualRegion_RegionClicked(int id, int x, int y, MouseButtons button)
        {
            if (button == MouseButtons.Left)
            {
                ChangeWork(id);
            }
        }


        private void MGUpToNumber_Paint(object sender, PaintEventArgs e)
        {
            DrawBase(e.Graphics);

            if (!show)
                return;

            vRegion.Draw(e.Graphics);

            string hardDes = "";
            string distanceDes = "";
            var info = raceSeg[raceId];
            switch (info.Hardness)
            {
                case RaceSegData.HardnessType.Easy: hardDes = "简单"; break;
                case RaceSegData.HardnessType.Med: hardDes = "中等"; break;
                case RaceSegData.HardnessType.Danger: hardDes = "艰险"; break;
            }
            if (info.Length > 1200)
                distanceDes = "长";
            else if (info.Length > 800)
                distanceDes = "中等";
            else
                distanceDes = "短";


            Image img = PicLoader.Read("MiniGame.Racing", string.Format("race{0}.PNG", (int)info.Hardness+1));
            e.Graphics.DrawImage(img, 50 + xoff, 50 + yoff, 100, 100);
            img.Dispose();
            img = PicLoader.Read("Border", "questb1.PNG");
            e.Graphics.DrawImage(img, 50 + xoff, 50 + yoff, 100, 100);
            img.Dispose();
            var font = new Font("宋体", 12*1.33f, FontStyle.Bold, GraphicsUnit.Pixel);
            DrawShadeText(e.Graphics, string.Format("第{0}赛段", raceId + 1), font, Brushes.Lime, xoff + 165, 35 + yoff);
            DrawShadeText(e.Graphics, string.Format("长度 {0}", distanceDes), font, Brushes.White, xoff+165, 65+yoff);
            DrawShadeText(e.Graphics, string.Format("难度 {0}", hardDes), font, Brushes.White, xoff + 165, 95 + yoff);
            if (distanceDiffer >= 0)
                DrawShadeText(e.Graphics, string.Format("领先 {0}米", distanceDiffer), font, Brushes.Lime, xoff + 165, 125 + yoff);
           else
                DrawShadeText(e.Graphics, string.Format("落后 {0}米", -distanceDiffer), font, Brushes.Red, xoff + 165, 125 + yoff);

            font.Dispose();
        }
    }
}
