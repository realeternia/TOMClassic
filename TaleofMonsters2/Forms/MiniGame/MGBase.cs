﻿using System;
using System.Drawing;
using System.Windows.Forms;
using ConfigDatas;
using ControlPlus;
using NarlonLib.Drawing;
using TaleofMonsters.Forms.Items.Core;
using TaleofMonsters.Controler.Loader;
using TaleofMonsters.DataType.User;
using TaleofMonsters.MainItem;

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

        protected int score;
        protected MinigameConfig config;

        public MGBase()
        {
            InitializeComponent();
        }

        public override void Init(int width, int height)
        {
            base.Init(width, height);
            this.bitmapButtonClose.ImageNormal = PicLoader.Read("ButtonBitmap", "CloseButton1.JPG");
            this.bitmapButtonHelp.ImageNormal = PicLoader.Read("ButtonBitmap", "LearnButton.JPG");
            show = true;
        }
        public void SetMinigameId(int id)
        {
            config = ConfigData.GetMinigameConfig(id);
            if (!string.IsNullOrEmpty(config.BgImage))
            {
                backImage = PicLoader.Read("MiniGame", config.BgImage + ".JPG");
            }
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

        public static Image GetPreview(int id)
        {
            var gameConfig = ConfigData.GetMinigameConfig(id);

            ControlPlus.TipImage tipData = new ControlPlus.TipImage();
            tipData.AddTextNewLine("评分", "White");
            tipData.AddTextNewLine("S级达成要求: ", "White");
            tipData.AddText(gameConfig.LvS.ToString(), "Gold");
            tipData.AddTextNewLine("A级达成要求: ", "White");
            tipData.AddText(gameConfig.LvA.ToString(), "Red");
            tipData.AddTextNewLine("B级达成要求: ", "White");
            tipData.AddText(gameConfig.LvB.ToString(), "Lime");

            return tipData.Image;
        }
    }
}
