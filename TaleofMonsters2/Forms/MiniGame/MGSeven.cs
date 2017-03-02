using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using NarlonLib.Math;
using ControlPlus;
using TaleofMonsters.DataType.User;
using TaleofMonsters.Forms.Items.Core;
using TaleofMonsters.Controler.Loader;
using TaleofMonsters.MainItem;

namespace TaleofMonsters.Forms.MiniGame
{
    internal partial class MGSeven : BasePanel, IMinigameForm
    {
        delegate void CalculateMethod();
        private void BeginCalculateResult()
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

        private bool show;
        private Image backImage;
        private int type;

        private const float MinSpeed = 0.3f;

        private List<IconCell> c1ItemList = new List<IconCell>();
        private List<IconCell> c2ItemList = new List<IconCell>();
        private List<IconCell> c3ItemList = new List<IconCell>();
        private float c1Speed = 0f;
        private float c2Speed = 0f;
        private float c3Speed = 0f;

        private bool c1Stop;
        private bool c2Stop;
        private bool c3Stop;

        private bool isFail;

        private const int xoff = 11;
        private const int yoff = 129;

        public MGSeven()
        {
            type = (int)SystemMenuIds.GameSeven;
            InitializeComponent();
        }

        internal override void Init(int width, int height)
        {
            base.Init(width, height);
            this.bitmapButtonC1.ImageNormal = PicLoader.Read("ButtonBitmap", "ButtonBack2.PNG");
            bitmapButtonC1.Font = new Font("宋体", 8 * 1.33f, FontStyle.Regular, GraphicsUnit.Pixel);
            bitmapButtonC1.ForeColor = Color.White;
            bitmapButtonC1.IconImage = TaleofMonsters.Core.HSIcons.GetIconsByEName("rot1");
            bitmapButtonC1.IconSize = new Size(16, 16);
            bitmapButtonC1.IconXY = new Point(4, 5);
            bitmapButtonC1.TextOffX = 8;
            bitmapButtonC1.Text = @"开始";
            this.bitmapButtonC2.ImageNormal = PicLoader.Read("ButtonBitmap", "ButtonBack2.PNG");
            bitmapButtonC2.Font = new Font("宋体", 8 * 1.33f, FontStyle.Regular, GraphicsUnit.Pixel);
            bitmapButtonC2.ForeColor = Color.White;
            bitmapButtonC2.IconImage = TaleofMonsters.Core.HSIcons.GetIconsByEName("rot2");
            bitmapButtonC2.IconSize = new Size(16, 16);
            bitmapButtonC2.IconXY = new Point(4, 5);
            bitmapButtonC2.TextOffX = 8;
            bitmapButtonC2.Text = @"停止";
            this.bitmapButtonC3.ImageNormal = PicLoader.Read("ButtonBitmap", "ButtonBack2.PNG");
            bitmapButtonC3.Font = new Font("宋体", 8 * 1.33f, FontStyle.Regular, GraphicsUnit.Pixel);
            bitmapButtonC3.ForeColor = Color.White;
            bitmapButtonC3.IconImage = TaleofMonsters.Core.HSIcons.GetIconsByEName("rot2");
            bitmapButtonC3.IconSize = new Size(16, 16);
            bitmapButtonC3.IconXY = new Point(4, 5);
            bitmapButtonC3.TextOffX = 8;
            bitmapButtonC3.Text = @"停止";
            this.bitmapButtonC4.ImageNormal = PicLoader.Read("ButtonBitmap", "ButtonBack2.PNG");
            bitmapButtonC4.Font = new Font("宋体", 8 * 1.33f, FontStyle.Regular, GraphicsUnit.Pixel);
            bitmapButtonC4.ForeColor = Color.White;
            bitmapButtonC4.IconImage = TaleofMonsters.Core.HSIcons.GetIconsByEName("rot2");
            bitmapButtonC4.IconSize = new Size(16, 16);
            bitmapButtonC4.IconXY = new Point(4, 5);
            bitmapButtonC4.TextOffX = 8;
            bitmapButtonC4.Text = @"停止";

            this.bitmapButtonClose.ImageNormal = PicLoader.Read("ButtonBitmap", "CloseButton1.JPG");
            backImage = PicLoader.Read("MiniGame", "t6.JPG");
            show = true;

            RestartGame();
        }

        internal override void OnFrame(int tick)
        {
            base.OnFrame(tick);
            if ((tick%5) == 0)
            {
                if (c1Stop)
                {
                    c1Speed *= 0.8f;
                    if (c1Speed <= MinSpeed)
                    {
                        c1Speed = 0;
                    }
                }
                if (c2Stop)
                {
                    c2Speed *= 0.8f;
                    if (c2Speed <= MinSpeed)
                    {
                        c2Speed = 0;
                    }
                }
                if (c3Stop)
                {
                    c3Speed *= 0.8f;
                    if (c3Speed <= MinSpeed)
                    {
                        c3Speed = 0;
                    }
                }
            }
            foreach (IconCell iconCell in c1ItemList)
            {
                iconCell.Y += 3.2f*c1Speed;
                if (iconCell.Y>= 400)
                {
                    iconCell.Y -= 77*7;
                }
            }
            foreach (IconCell iconCell in c2ItemList)
            {
                iconCell.Y += 3.2f * c2Speed;
                if (iconCell.Y >= 400)
                {
                    iconCell.Y -= 77 * 7;
                }
            }
            foreach (IconCell iconCell in c3ItemList)
            {
                iconCell.Y += 3.2f * c3Speed;
                if (iconCell.Y >= 400)
                {
                    iconCell.Y -= 77 * 7;
                }
            }
            
            BeginCalculateResult();

            Invalidate(new Rectangle(xoff, yoff, 324, 244));
        }

        public void RestartGame()
        {
            #region 初始化各个元素

            c1ItemList.Clear();
            c2ItemList.Clear();
            c3ItemList.Clear();
            for (int i = 0; i < 7; i++)
            {
                IconCell ic = new IconCell();
                ic.Index = i + 1;
                ic.Y = i*77;
                ic.X = 52;
                c1ItemList.Add(ic);
            }
            for (int i = 0; i < 7; i++)
            {
                IconCell ic = new IconCell();
                ic.Index = i + 1;
                ic.Y = i * 77;
                ic.X = 141;
                c2ItemList.Add(ic);
            }
            for (int i = 0; i < 7; i++)
            {
                IconCell ic = new IconCell();
                ic.Index = i + 1;
                ic.Y = i * 77;
                ic.X = 230;
                c3ItemList.Add(ic);
            }
            #endregion

            bitmapButtonC1.Visible = true;
            bitmapButtonC2.Visible = false;
            bitmapButtonC3.Visible = false;
            bitmapButtonC4.Visible = false;
            c1Stop = false;
            c2Stop = false;
            c3Stop = false;
            isFail = false;
        }

        private void CalculateResult()
        {
            if (c1Stop && c2Stop && c3Stop &&c1Speed == 0f && c2Speed == 0f && c3Speed == 0)
            {
                int index1 = FindMin(c1ItemList).Index;
                int index2 = FindMin(c2ItemList).Index;
                int index3 = FindMin(c3ItemList).Index;

                if (index1 == index2 && index1 == index3)
                {
                    isFail = false;
                }
                else if (index1 == 7 || index2==7 || index3 == 7)//出现7直接获胜
                {
                    isFail = false;
                }
                else
                {
                    isFail = true;
                }

                EndGame();
            }
        }

        private IconCell FindMin(List<IconCell> list)
        {
            IconCell nearItem = null;
            float dis = float.MaxValue;
            foreach (IconCell iconCell in list)
            {
                float nowDis = Math.Abs(iconCell.Y - 231);
                if (nowDis < dis)
                {
                    nearItem = iconCell;
                    dis = nowDis;
                }
            }
            return nearItem;
        }

        public void EndGame()
        {
            string hint;
            if (!isFail)
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
            c1Speed = MathTool.GetRandom(15, 20);
            c2Speed = MathTool.GetRandom(15, 20);
            c3Speed = MathTool.GetRandom(15, 20);

            bitmapButtonC1.Visible = false;
            bitmapButtonC2.Visible = true;
            bitmapButtonC3.Visible = true;
            bitmapButtonC4.Visible = true;
        }

        private void bitmapButtonC2_Click(object sender, EventArgs e)
        {
            c2Stop = true;
        }

        private void bitmapButtonC3_Click(object sender, EventArgs e)
        {
            c1Stop = true;
        }

        private void bitmapButtonC4_Click(object sender, EventArgs e)
        {
            c3Stop = true;
        }

        private void bitmapButtonClose_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void MGUpToNumber_Paint(object sender, PaintEventArgs e)
        {
            BorderPainter.Draw(e.Graphics, "", Width, Height);

            Font font = new Font("黑体", 12*1.33f, FontStyle.Bold, GraphicsUnit.Pixel);
            e.Graphics.DrawString("SEVEN", font, Brushes.White, 150, 8);
            font.Dispose();

            if (!show)
                return;

            e.Graphics.DrawImage(backImage, xoff, yoff, 324, 244);

            foreach (IconCell iconCell in c1ItemList)
            {
                iconCell.Draw(e.Graphics, yoff+47,yoff+216);
            }
            foreach (IconCell iconCell in c2ItemList)
            {
                iconCell.Draw(e.Graphics, yoff + 47, yoff + 216);
            }
            foreach (IconCell iconCell in c3ItemList)
            {
                iconCell.Draw(e.Graphics, yoff + 47, yoff + 216);
            }
        }

    }

    internal class IconCell
    {
        public int Index;
        public float Y;
        public float X;

        public void Draw(Graphics g, int yMin, int yMax)
        {
            if (Y+64 < yMin || Y>yMax)
            {
                return;
            }

            Image img = PicLoader.Read("MiniGame.Seven", string.Format("g{0}.PNG", Index));
            if (Y >= yMin && Y+64 < yMax) //画整个
            {
                g.DrawImage(img, X, Y, 64, 64);    
            }
            else if (Y< yMin) //下半个
            {
                g.DrawImage(img, new RectangleF(X, yMin, 64, 64 + Y - yMin), new RectangleF(0, yMin - Y, 64, 64 + Y - yMin), GraphicsUnit.Pixel);
            }
            else if (Y + 64 > yMax) //上半个
            {
                float yAdd = yMax - Y;
                g.DrawImage(img, new RectangleF(X, yMax - yAdd, 64, yAdd), new RectangleF(0, 0, 64, yAdd), GraphicsUnit.Pixel);
            }
            
            img.Dispose();
        }
    }
}
