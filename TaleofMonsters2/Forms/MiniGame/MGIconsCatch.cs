using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using NarlonLib.Math;
using TaleofMonsters.Controler.Loader;
using TaleofMonsters.Core;
using ControlPlus;
using TaleofMonsters.DataType.User;
using TaleofMonsters.Forms.Items.Core;
using TaleofMonsters.MainItem;

namespace TaleofMonsters.Forms.MiniGame
{
    internal partial class MGIconsCatch : BasePanel
    {
        private List<int> answers = new List<int>();
        private List<int> results = new List<int>();
        private int realanswer;
        private int tempanswer;
        private int srcValue;
        private bool show;
        private int type;

        private const int xoff = 11;
        private const int yoff = 129;

        public MGIconsCatch()
        {
            type = (int)SystemMenuIds.GameIconsCatch;
            InitializeComponent();
        }

        internal override void Init(int width, int height)
        {
            base.Init(width, height);
            #region 按钮图片
            this.bitmapButtonC1.ImageNormal = HSIcons.GetIconsByEName("atr0");
            this.bitmapButtonC2.ImageNormal = HSIcons.GetIconsByEName("atr1");
            this.bitmapButtonC3.ImageNormal = HSIcons.GetIconsByEName("atr2");
            this.bitmapButtonC4.ImageNormal = HSIcons.GetIconsByEName("atr3");
            this.bitmapButtonC5.ImageNormal = HSIcons.GetIconsByEName("atr4");
            this.bitmapButtonC6.ImageNormal = HSIcons.GetIconsByEName("atr5");
            this.bitmapButtonC7.ImageNormal = HSIcons.GetIconsByEName("atr6");
            this.bitmapButtonC8.ImageNormal = HSIcons.GetIconsByEName("atr7");
            this.bitmapButtonC9.ImageNormal = HSIcons.GetIconsByEName("atr8");
            #endregion
            this.bitmapButtonClose.ImageNormal = PicLoader.Read("ButtonBitmap", "CloseButton1.JPG");
            this.customScrollbar1.Minimum = 0;
            this.customScrollbar1.LargeChange = 17;
            this.customScrollbar1.Maximum = (answers.Count - 10) + customScrollbar1.LargeChange;
            this.customScrollbar1.SmallChange = 3;
            this.customScrollbar1.Value = 0;
            show = true;

            RestartGame();
        }

        public void RestartGame()
        {
            int[] data ={ 1, 2, 3, 4, 5, 6, 7, 8, 9 };
            for (int i = 0; i < 100; i++)
            {
                int id1 = MathTool.GetRandom(9);
                int id2 = MathTool.GetRandom(9);
                int temp = data[id1];
                data[id1] = data[id2];
                data[id2] = temp;
            }
            realanswer = 0;
            for (int i = 0; i < 4; i++)
            {
                realanswer *= 10;
                realanswer += data[i];
            }
            answers.Clear();
            results.Clear();
            Invalidate(new Rectangle(xoff, yoff, 310, 200 + 30));
        }

        public void EndGame()
        {
            string hint;
            if (realanswer == tempanswer)
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

        private int GetByteFromInt(int value, int index)
        {
            switch (index)
            {
                case 0: return value / 1000;
                case 1: return (value % 1000) / 100;
                case 2: return (value % 100) / 10;
                case 3: return value % 10;
                default: return 0;
            }
        }

        private bool HasByteFromInt(int value, int index)
        {
            int data = value;
            while (data > 0)
            {
                int v = data % 10;
                if (index == v)
                    return true;
                data /= 10;
            }
            return false;
        }

        private void bitmapButtonC1_Click(object sender, EventArgs e)
        {
            int data = int.Parse(((Control) sender).Tag.ToString());
            ((Control) sender).Enabled = false;
            if (tempanswer / 1000 == 0)
            {
                tempanswer = tempanswer + data * 1000;
            }
            else if ((tempanswer % 1000) / 100 == 0)
            {
                tempanswer = tempanswer + data * 100;
            }
            else if ((tempanswer % 100) / 10 == 0)
            {
                tempanswer = tempanswer + data * 10;
            }
            else if (tempanswer % 10 == 0)
            {
                tempanswer = tempanswer + data;
                CalculateResult();
                for (int i = 1; i <= 9; i++)
                {
                    Controls["bitmapButtonC" + i].Enabled = true;
                }
            }
            Invalidate(new Rectangle(xoff, yoff, 310, 200 + 30));
        }

        private void CalculateResult()
        {
            if (realanswer == tempanswer || answers.Count >= 14)
            {
                EndGame();
            }
            else
            {
                int result = 0;
                for (int i = 0; i < 4; i++)
                {
                    int tpByte = GetByteFromInt(tempanswer, i);
                    if (GetByteFromInt(realanswer, i) == tpByte)
                    {
                        result += 10;
                    }
                    else if (HasByteFromInt(realanswer, tpByte))
                    {
                        result++;
                    }
                }
                answers.Add(tempanswer);
                results.Add(result);
                customScrollbar1.Maximum = (answers.Count - 10) + customScrollbar1.LargeChange;
                if (answers.Count > 10)
                    srcValue = answers.Count - 10;
                else
                    srcValue = 0;
                customScrollbar1.Value = srcValue;
            }
            tempanswer = 0;
        }

        private void DrawOnLine(Graphics g, int id, int data1, int data2, int line)
        {
            for (int i = 0; i < 4; i++)
            {
                int v = GetByteFromInt(data1, i);
                g.DrawImage(HSIcons.GetIconsByEName("atr" + (v - 1)), i * 20 + 66+xoff, line * 20 + 1+yoff, 18, 18);
            }
            Font font = new Font("微软雅黑", 11*1.33f, FontStyle.Bold, GraphicsUnit.Pixel);
            g.DrawString(id < 100 ? (id + 1).ToString() : "当前", font, Brushes.White, 10 + xoff, line * 20 + 1 + yoff);
            if (id<100)
            {
                g.DrawImage(HSIcons.GetIconsByEName("npc3"), 238 + xoff, line * 20 + 1 + yoff, 18, 18);
                g.DrawImage(HSIcons.GetIconsByEName("npc2"), 278 + xoff, line * 20 + 1 + yoff, 18, 18);

                g.DrawString((data2 / 10).ToString(), font, data2 / 10 > 0 ? Brushes.Gold : Brushes.White, 220 + xoff, line * 20 + 1 + yoff);
                g.DrawString((data2 % 10).ToString(), font, (data2 % 10 > 0) ? Brushes.Gold : Brushes.White, 260 + xoff, line * 20 + 1 + yoff); 
            }
            font.Dispose();
        }

        private void customScrollbar1_Scroll(object sender, EventArgs e)
        {
            if (customScrollbar1.Value != srcValue)
            {
                srcValue = customScrollbar1.Value;
                Invalidate(new Rectangle(xoff, yoff, 310, 200));
            }
        }

        private void bitmapButtonClose_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void MGIconsCatch_Paint(object sender, PaintEventArgs e)
        {
            BorderPainter.Draw(e.Graphics, "", Width, Height);

            Font font = new Font("黑体", 12*1.33f, FontStyle.Bold, GraphicsUnit.Pixel);
            e.Graphics.DrawString("魔幻符号", font, Brushes.White, 140, 8);
            font.Dispose();

            if (!show)
                return;

            for (int i = 0; i < 5; i++)
                e.Graphics.FillRectangle(Brushes.DimGray, xoff, i * 40 + yoff, 310, 20);
            e.Graphics.DrawRectangle(Pens.AntiqueWhite, xoff, yoff, 310 - 1, 200 - 1);
            if (answers.Count <= 10)
            {
                for (int i = 0; i < answers.Count; i++)
                {
                    DrawOnLine(e.Graphics, i, answers[i], results[i], i);
                }
            }
            else
            {
                for (int i = 0; i < 10; i++)
                {
                    DrawOnLine(e.Graphics, srcValue + i, answers[srcValue + i], results[srcValue + i], i);
                }
            }
            DrawOnLine(e.Graphics, 100, tempanswer, 0, 10);
            //for (int i = 0; i < 4; i++)
            //{
            //    int v = GetByteFromInt(tempanswer, i);
            //    if (v != 0)
            //    {
            //        e.Graphics.DrawImage(HSIcons.GetIconsByEName("atr" + (v - 1)), i * 40 + 59+xoff, 206+yoff, 28, 28);
            //    }
            //}
        }
    }
}
