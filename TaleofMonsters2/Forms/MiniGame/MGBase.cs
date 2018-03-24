using System;
using System.Drawing;
using ConfigDatas;
using ControlPlus;
using NarlonLib.Math;
using TaleofMonsters.Core.Loader;
using TaleofMonsters.Forms.CMain;
using TaleofMonsters.Forms.Items.Core;

namespace TaleofMonsters.Forms.MiniGame
{
    internal partial class MGBase : BasePanel
    {
        internal class MinigameRank
        {
            public static int S = 3;
            public static int A = 2;
            public static int B = 1;
            public static int C = 0;
        }

        delegate void CalculateMethod();
        protected void BeginCalculateResult()
        {
            if (InvokeRequired)
            {
                CalculateMethod aus = new CalculateMethod(BeginCalculateResult);
                Invoke(aus, null);
            }
            else
            {
                CalculateResult();
            }
        }

        public delegate void MinigameResultCallback(int type);
        private MinigameResultCallback resultEvent;

        protected bool show;
        private Image backImage;
        private int type;
        protected int hardness;
        protected int xoff = 11;
        protected int yoff = 129;

        protected int score;
        protected MinigameConfig config;

        public MGBase()
        {
            InitializeComponent();
        }

        public override void Init(int width, int height)
        {
            base.Init(width, height);
            this.bitmapButtonClose.ImageNormal = PicLoader.Read("Button.Panel", "CloseButton1.JPG");
            this.bitmapButtonHelp.ImageNormal = PicLoader.Read("Button.Panel", "LearnButton.JPG");
            bitmapButtonHelp.NoUseDrawNine = true;
            show = true;
        }

        public void SetMinigameId(int id, int hard)
        {
            config = ConfigData.GetMinigameConfig(id);

            if (!string.IsNullOrEmpty(config.BgImage))
                backImage = PicLoader.Read("MiniGame", config.BgImage + ".JPG");
            type = config.WindowId;
            hardness = hard;
        }

        public void SetEvent(MinigameResultCallback winCallback)
        {
            resultEvent = winCallback;
        }

        public virtual void RestartGame()
        {
            score = 0;
        }

        public virtual void EndGame()
        {
            string hint = "最终得分" + score;
            int rank = -1;
            if (score >= GetFinalMark(config.LvS))
            {
                hint += ",评级S!!!";
                rank = MinigameRank.S;
            }
            else if (score >= GetFinalMark(config.LvA))
            {
                hint += ",评级A!"; rank = MinigameRank.A;
            }
            else if (score >= GetFinalMark(config.LvB))
            {
                hint += ",评级B"; rank = MinigameRank.B;
            }
            else
            {
                hint += ",评级C"; rank = MinigameRank.C;
            }

            MessageBoxEx.Show(hint);

            //if (MessageBoxEx2.Show(hint + ",是否花5钻石再试一次?") == DialogResult.OK)
            //{
            //    if (UserProfile.InfoBag.PayDiamond(5))
            //    {
            //        RestartGame();
            //        return;
            //    }
            //}

            Close();
            if (resultEvent != null)
            {
                resultEvent(rank);
            }
        }

        protected virtual void CalculateResult()
        {//为了跨线程结算使用
        }


        private void bitmapButtonClose_Click(object sender, EventArgs e)
        {
            Close();
            if (resultEvent != null)
            {
                resultEvent(MinigameRank.C);
            }
        }

        protected void DrawShadeText(Graphics g, string text, Font font, Brush brush, int x, int y)
        {
            g.DrawString(text, font, Brushes.Black, x + 1, y + 1);
            g.DrawString(text, font, brush, x, y);
        }

        protected void DrawBase(Graphics g)
        {
            BorderPainter.Draw(g, "", Width, Height);

            if (!show)
                return;

            Font font = new Font("黑体", 12*1.33f, FontStyle.Bold, GraphicsUnit.Pixel);
            g.DrawString(config.Name, font, Brushes.White, Width / 2 - 40, 8);
            font.Dispose();
            
            if(backImage != null)
                g.DrawImage(backImage, xoff, yoff, 324, 244);

        }

        private void bitmapButtonHelp_MouseEnter(object sender, EventArgs e)
        {
            Image image = GetPreview(config.Id);
            SystemToolTip.Instance.Show(image, this, bitmapButtonHelp.Location.X + 24, bitmapButtonHelp.Location.Y);
        }

        private void bitmapButtonHelp_MouseLeave(object sender, EventArgs e)
        {
            SystemToolTip.Instance.Hide(this);
        }

        public Image GetPreview(int id)
        {
            var gameConfig = ConfigData.GetMinigameConfig(id);

            ControlPlus.TipImage tipData = new ControlPlus.TipImage();
            tipData.AddTextNewLine("评分", "White");
            tipData.AddTextNewLine("S级达成要求: ", "White");
            tipData.AddText(GetFinalMark(gameConfig.LvS).ToString(), "Gold");
            tipData.AddTextNewLine("A级达成要求: ", "White");
            tipData.AddText(GetFinalMark(gameConfig.LvA).ToString(), "Red");
            tipData.AddTextNewLine("B级达成要求: ", "White");
            tipData.AddText(GetFinalMark(gameConfig.LvB).ToString(), "Lime");

            return tipData.Image;
        }

        private int GetFinalMark(int mark)
        {
            return MathTool.Clamp(mark*(10 + hardness)/10, 1, 1000);
        }
    }
}
