using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using NarlonLib.Math;
using TaleofMonsters.Controler.Loader;
using TaleofMonsters.Core;
using TaleofMonsters.Forms.Items.Regions;

namespace TaleofMonsters.Forms.MiniGame
{
    internal partial class MGVoting : MGBase
    {
        private class VoteQuestion
        {
            public string Question;
            public double EffHumanMin;
            public double EffHumanMax;
            public double EffOrcMin;
            public double EffOrcMax;
            public double EffElfMin;
            public double EffElfMax;
            public double EffDwalfMin;
            public double EffDwalfMax;

            public VoteQuestion(string q, double e1min, double e1max, double e2min, double e2max, double e3min, double e3max, double e4min, double e4max)
            {
                Question = q;
                EffHumanMin = e1min;
                EffHumanMax = e1max;
                EffOrcMin = e2min;
                EffOrcMax = e2max;
                EffElfMin = e3min;
                EffElfMax = e3max;
                EffDwalfMin = e4min;
                EffDwalfMax = e4max;
            }
        }

        private double supportRateHuman;
        private double supportRateOrc;
        private double supportRateElf;
        private double supportRateDwalf;

        private double supportRateHumanOld;
        private double supportRateOrcOld;
        private double supportRateElfOld;
        private double supportRateDwalfOld;

        private double supportRateTotal;

        private uint countHuman; //人口基数
        private uint countOrc;
        private uint countElf;
        private uint countDwalf;

        private VirtualRegion virtualRegion;

        private VoteQuestion[] questions = new VoteQuestion[]
        {
            new VoteQuestion("控制垃圾投放，保护环境卫生", 0.0, 0.2, -0.1, 0.0, -0.1, 0.05, -0.05, 0.15),
            new VoteQuestion("大力推进武器销售和制造产业的发展", 0.0, 0.1, 0.15, 0.3, -0.2, -0.15, 0.05, 0.2),
            new VoteQuestion("推行宵禁，加强社区安全建设", -0.1, 0.1, -0.2, 0.0, -0.05, 0.05, 0.1, 0.25),
            new VoteQuestion("采购大批绿色植物，绿化环境", -0.05, 0.1, -0.15, 0.1, 0.15, 0.25, -0.1, 0.0),
            new VoteQuestion("提高退休福利，增加养老拨款", 0.1, 0.25, -0.1, 0.05, 0.0, 0.15, 0.05, 0.15),
            new VoteQuestion("增加税收，加强城防建设", -0.1, 0.0, 0.1, 0.2, -0.05, 0.1, 0.0, 0.1),
            new VoteQuestion("增加教育支出，提升国民素质", 0.0, 0.3, -0.1, 0.05, 0.05, 0.1, 0.0, 0.1),
            new VoteQuestion("严正打击假冒伪劣商品", -0.05, 0.1, 0.05, 0.25, -0.1, 0.1, 0.05, 0.15),
            new VoteQuestion("大力倡导宗教建设", -0.05, 0.2, -0.05, 0.2, -0.1, 0.1, 0.0, 0.05),
            new VoteQuestion("提倡绿色经济，推动旅游发展", 0.05, 0.3, 0.05, 0.15, 0.0, 0.05, -0.05, 0.05),
            new VoteQuestion("严查贪污腐败，杜绝经济犯罪", -0.1, 0.05, 0.05, 0.25, -0.05, 0.05, -0.05, 0.2),
            new VoteQuestion("推进农村现代化建设", -0.05, 0.15, -0.05, 0.05, 0.05, 0.15, 0.1, 0.25),
        };

        private int[] targetQuestions;
        private int selectQuestionId;

        private int round;

        public MGVoting()
        {
            InitializeComponent();
            this.bitmapButtonC1.ImageNormal = PicLoader.Read("Button.Panel", "ButtonBack2.PNG");
            bitmapButtonC1.Font = new Font("宋体", 8 * 1.33f, FontStyle.Regular, GraphicsUnit.Pixel);
            bitmapButtonC1.ForeColor = Color.White;
            bitmapButtonC1.IconImage = TaleofMonsters.Core.HSIcons.GetIconsByEName("rot3");
            bitmapButtonC1.IconSize = new Size(16, 16);
            bitmapButtonC1.IconXY = new Point(4, 5);
            bitmapButtonC1.TextOffX = 8;
            virtualRegion = new VirtualRegion(this);
            for (int i = 0; i < 3; i++)
            {
                ButtonRegion region = new ButtonRegion(i + 1, 30, 270+30*i, 20, 20, "GameBackNormal1.PNG", "GameBackNormal1On.PNG");
                virtualRegion.AddRegion(region);
            }
            
            virtualRegion.RegionClicked += new VirtualRegion.VRegionClickEventHandler(virtualRegion_RegionClicked);
        }

        public override void Init(int width, int height)
        {
            base.Init(width, height);
            
            RestartGame();
        }

        public override void RestartGame()
        {
            base.RestartGame();
            round = 0;
            supportRateTotal = 0;
            supportRateHuman = 0;
            supportRateHumanOld = 0;
            supportRateElf = 0;
            supportRateElfOld = 0;
            supportRateOrc = 0;
            supportRateOrcOld = 0;
            supportRateDwalf = 0;
            supportRateDwalfOld = 0;
            countHuman = (uint)MathTool.GetRandom(60, 150);
            countOrc = (uint)MathTool.GetRandom(40, 70);
            countElf = (uint)MathTool.GetRandom(50, 130);
            countDwalf = (uint)MathTool.GetRandom(50, 120);
            NextQuestion();            
        }

        public override void EndGame()
        {
            base.EndGame();
        }

        private void NextQuestion()
        {
            if (targetQuestions != null)
            {
                CheckAnswer();
            }

            List<int> qidList = new List<int>();
            for (int id = 0; id < questions.Length; id++)
            {
                qidList.Add(id);
            }
            targetQuestions = NLRandomPicker<int>.RandomPickN(qidList, 3).ToArray();
            virtualRegion_RegionClicked(MathTool.GetRandom(3)+1, 1,1, MouseButtons.Left);//模拟一个随机点击事件
            Invalidate(new Rectangle(xoff, yoff, 324, 244));
        }

        private void CheckAnswer()
        {
            VoteQuestion selectQuestion = questions[selectQuestionId];
            supportRateHumanOld = supportRateHuman;
            supportRateOrcOld = supportRateOrc;
            supportRateElfOld = supportRateElf;
            supportRateDwalfOld = supportRateDwalf;
            supportRateHuman = GetNewSupportRate (supportRateHuman, MathTool.GetRandom(selectQuestion.EffHumanMin, selectQuestion.EffHumanMax));
            supportRateOrc = GetNewSupportRate(supportRateOrc, MathTool.GetRandom(selectQuestion.EffOrcMin, selectQuestion.EffOrcMax));
            supportRateElf = GetNewSupportRate(supportRateElf, MathTool.GetRandom(selectQuestion.EffElfMin, selectQuestion.EffElfMax));
            supportRateDwalf = GetNewSupportRate(supportRateDwalf, MathTool.GetRandom(selectQuestion.EffDwalfMin, selectQuestion.EffDwalfMax));

            supportRateTotal = countHuman * supportRateHuman + supportRateOrc * countOrc + supportRateElf * countElf + supportRateDwalf * countDwalf;
            supportRateTotal /= (countHuman + countOrc + countElf + countDwalf);

            round++;
            if (round >= 10)
            {
                score = (int) (supportRateTotal*100);
                EndGame();
            }
        }

        private double GetNewSupportRate(double nowRate, double change)
        {
            var result = nowRate;
            if (result > 0.3)
            {
                result += change;
            }
            else
            {
                result += change*MathTool.GetRandom(1, 2);
            }
            result = Math.Max(result, 0);
            return result;
        }

        private void bitmapButtonC1_Click(object sender, EventArgs e)
        {
            NextQuestion();
            Invalidate(new Rectangle(xoff, yoff, 324, 244));
        }

        void virtualRegion_RegionClicked(int id, int x, int y, MouseButtons button)
        {
            if (button == MouseButtons.Left)
            {
                for (int i = 0; i < 3; i++)
                {
                    virtualRegion.SetRegionState(i + 1, RegionState.Free);
                }
                selectQuestionId = targetQuestions[id-1];
                virtualRegion.SetRegionState(id, RegionState.Rectangled);
                Invalidate(new Rectangle(xoff, yoff, 324, 244));
            }
        }


        private void MGVoting_Paint(object sender, PaintEventArgs e)
        {
            DrawBase(e.Graphics);

            if (!show)
                return;

            virtualRegion.Draw(e.Graphics);

            e.Graphics.DrawImage(HSIcons.GetIconsByEName("rac8"), xoff + 80, yoff + 10, 30, 30);
            e.Graphics.DrawImage(HSIcons.GetIconsByEName("rac9"), xoff + 140, yoff + 10, 30, 30);
            e.Graphics.DrawImage(HSIcons.GetIconsByEName("rac3"), xoff + 200, yoff + 10, 30, 30);
            e.Graphics.DrawImage(HSIcons.GetIconsByEName("rac15"), xoff + 260, yoff + 10, 30, 30);

            var font = new Font("宋体", 11 * 1.33f, FontStyle.Bold, GraphicsUnit.Pixel);
            DrawShadeText(e.Graphics, "人口", font, Brushes.White, xoff + 10, yoff + 50);
            DrawShadeText(e.Graphics, "前轮", font, Brushes.White, xoff + 10, yoff + 70);
            DrawShadeText(e.Graphics, "支持率", font, Brushes.White, xoff + 5, yoff + 90);

            DrawShadeText(e.Graphics, countHuman.ToString(), font, Brushes.White, xoff + 80, yoff + 50);
            DrawShadeText(e.Graphics, countOrc.ToString(), font, Brushes.White, xoff + 140, yoff + 50);
            DrawShadeText(e.Graphics, countElf.ToString(), font, Brushes.White, xoff + 200, yoff + 50);
            DrawShadeText(e.Graphics, countDwalf.ToString(), font, Brushes.White, xoff + 260, yoff + 50);

            DrawShadeText(e.Graphics, string.Format("{0:0}%", supportRateHumanOld * 100), font, Brushes.White, xoff + 80, yoff + 70);
            DrawShadeText(e.Graphics, string.Format("{0:0}%", supportRateOrcOld * 100), font, Brushes.White, xoff + 140, yoff + 70);
            DrawShadeText(e.Graphics, string.Format("{0:0}%", supportRateElfOld * 100), font, Brushes.White, xoff + 200, yoff + 70);
            DrawShadeText(e.Graphics, string.Format("{0:0}%", supportRateDwalfOld * 100), font, Brushes.White, xoff + 260, yoff + 70);

            DrawShadeText(e.Graphics, string.Format("{0:0}%", supportRateHuman*100), font, Brushes.White, xoff + 80, yoff + 90);
            DrawShadeText(e.Graphics, string.Format("{0:0}%", supportRateOrc * 100), font, Brushes.White, xoff + 140, yoff + 90);
            DrawShadeText(e.Graphics, string.Format("{0:0}%", supportRateElf * 100), font, Brushes.White, xoff + 200, yoff + 90);
            DrawShadeText(e.Graphics, string.Format("{0:0}%", supportRateDwalf * 100), font, Brushes.White, xoff + 260, yoff + 90);

            for (int i = 0; i < targetQuestions.Length; i++)
            {
                var qData = questions[targetQuestions[i]];
                DrawShadeText(e.Graphics, qData.Question, font, Brushes.White, 55, 273 + 30*i);
            }
            
            font.Dispose();

            font = new Font("宋体", 18 * 1.33f, FontStyle.Bold, GraphicsUnit.Pixel);
            DrawShadeText(e.Graphics, string.Format("第{0}天  {1:0}%", round+1, supportRateTotal * 100), font, Brushes.Gold, xoff + 90, yoff + 110);
            font.Dispose();
        }
    }
}
