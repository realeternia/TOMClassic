using System;
using System.Drawing;
using System.Windows.Forms;
using ConfigDatas;
using ControlPlus;
using TaleofMonsters.Forms.Items.Core;
using TaleofMonsters.Controler.Loader;
using TaleofMonsters.DataType.User;

namespace TaleofMonsters.Forms.MiniGame
{
    internal partial class MGBase : BasePanel
    {
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

        protected bool show;
        private Image backImage;
        private int type;
        protected int xoff = 11;
        protected int yoff = 129;

        private string playName = "";
        protected int score;

        public MGBase()
        {
            InitializeComponent();
        }

        public override void Init(int width, int height)
        {
            base.Init(width, height);
            this.bitmapButtonClose.ImageNormal = PicLoader.Read("ButtonBitmap", "CloseButton1.JPG");
            show = true;
        }
        public void SetMinigameId(int id)
        {
            var config = ConfigData.GetMinigameConfig(id);
            if (!string.IsNullOrEmpty(config.BgImage))
            {
                backImage = PicLoader.Read("MiniGame", config.BgImage + ".JPG");
            }
            playName = config.Name;
            type = config.WindowId;
        }

        public virtual void RestartGame()
        {
            score = 0;
        }

        public virtual void EndGame()
        {
            string hint;
            if (score >= 50)
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

        protected virtual void CalculateResult()
        {//为了跨线程结算使用
        }


        private void bitmapButtonClose_Click(object sender, EventArgs e)
        {
            Close();
        }

        protected void DrawShadeText(Graphics g, string text, Font font, Brush Brush, int x, int y)
        {
            g.DrawString(text, font, Brushes.Black, x + 1, y + 1);
            g.DrawString(text, font, Brush, x, y);
        }

        protected void DrawBase(Graphics g)
        {
            BorderPainter.Draw(g, "", Width, Height);

            if (!show)
                return;

            Font font = new Font("黑体", 12*1.33f, FontStyle.Bold, GraphicsUnit.Pixel);
            g.DrawString(playName, font, Brushes.White, Width / 2 - 40, 8);
            font.Dispose();
            
            if(backImage != null)
                g.DrawImage(backImage, xoff, yoff, 324, 244);

        }
    }
}
