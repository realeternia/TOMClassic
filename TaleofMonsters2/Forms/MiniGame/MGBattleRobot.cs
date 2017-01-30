using System;
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
    internal partial class MGBattleRobot : BasePanel
    {
        private bool show;
        private Image backImage;
        private int hp;
        private int[,] attrs;
        private int ehp;
        private int[,] eattrs;
        private int type;
        private string resultStr;
        private int stag;
        private int lastact;
        private int lasteact;

        private const int xoff = 11;
        private const int yoff = 129;

        public MGBattleRobot()
        {
            type = (int)SystemMenuIds.GameBattleRobot;
            InitializeComponent();
        }

        internal override void Init(int width, int height)
        {
            base.Init(width, height);
            this.bitmapButtonClose.ImageNormal = PicLoader.Read("ButtonBitmap", "CloseButton1.JPG");
            backImage = PicLoader.Read("MiniGame", "t2.JPG");
            show = true;

            RestartGame();
        }

        public void RestartGame()
        {
            InitAttrs();
            Invalidate(new Rectangle(xoff, yoff, 324, 244));
        }

        public void EndGame()
        {
            string hint;
            if (hp > 0)
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

        private void InitAttrs()
        {
            hp = 100;
            ehp = 100;
            attrs = new int[,] { { 10, 0 }, { 15, 0 }, { 24, 0 }, { 60, 0 }, { 40, 0 }, { 30, 0 } };
            eattrs = new int[,] { { 12, 0 }, { 18, 0 }, { 30, 0 }, { 70, 0 }, { 50, 0 }, { 35, 0 } };
            for (int i = 0; i < 3; i++)
            {
                int rindex = MathTool.GetRandom(6);
                attrs[rindex, 0] = Math.Min(attrs[rindex, 0]*13/10, 100);
                rindex = MathTool.GetRandom(6);
                eattrs[rindex, 0] = Math.Min(eattrs[rindex, 0]*13/10, 100);
            }
            stag = 1;
            lastact = -1;
            lasteact = -1;
            resultStr = "战斗开始";
        }

        private void bitmapButtonC1_Click(object sender, EventArgs e)
        {
            CheckAction(0);
        }

        private void bitmapButtonC2_Click(object sender, EventArgs e)
        {
            CheckAction(1);
        }

        private void bitmapButtonC3_Click(object sender, EventArgs e)
        {
            CheckAction(2);
        }

        private void CheckAction(int act)
        {
            int[] hits = new int[]{100, 75, 50};
            if ((stag % 2) == 0)
            {
                int eact = MathTool.GetRandom(3) + 3;
                if (MathTool.GetRandom(100) >= hits[act])
                {
                    resultStr = "你没有击中目标";
                }
                else
                {
                    if (eact - act == 3)
                    {
                        int dam = attrs[act, 0]*(100 - eattrs[eact, 0])/100;
                        ehp -= dam;
                        resultStr = string.Format("对手使用了{0}，受到{1}点伤害", eact == 3 ? "格斗盾" : (eact == 4 ? "拦截弹" : "干扰器"), dam);
                        eattrs[eact, 1] = 1;
                    }
                    else
                    {
                        int dam = attrs[act, 0];
                        ehp -= dam;
                        resultStr = string.Format("对手错误使用了{0}，受到{1}点伤害", eact == 3 ? "格斗盾" : (eact == 4 ? "拦截弹" : "干扰器"), dam);
                    }
                }
                lastact = act;
                lasteact = eact;
                bitmapButtonC1.Text = "格斗盾";
                bitmapButtonC2.Text = "拦截弹";
                bitmapButtonC3.Text = "干扰器";
                AddFlowCenter("我方转为防守方", "Blue");
            }
            else
            {
                int eact = MathTool.GetRandom(3);
                if (MathTool.GetRandom(100) >= hits[eact])
                {
                    resultStr = "对方没有击中你";
                }
                else
                {
                    if (act == eact)
                    {
                        int dam = eattrs[eact, 0] * (100 - attrs[act+3, 0]) / 100;
                        hp -= dam;
                        resultStr = string.Format("对手使用了{0}，防御成功，受到{1}点伤害", eact == 0 ? "激光刀" : (eact == 1 ? "导弹" : "空袭"), dam);
                    }
                    else
                    {
                        int dam = eattrs[eact, 0];
                        hp -= dam;
                        resultStr = string.Format("对手使用了{0}，受到{1}点伤害", eact == 0 ? "激光刀" : (eact == 1 ? "导弹" : "空袭"), dam);
                    }
                    eattrs[eact, 1] = 1;
                }
                lastact = act + 3;
                lasteact = eact;
                bitmapButtonC1.Text = "激光刀";
                bitmapButtonC2.Text = "导弹";
                bitmapButtonC3.Text = "空袭";
                AddFlowCenter("我方转为进攻方", "Red");
            }
            Invalidate(new Rectangle(xoff, yoff, 324, 244));
            stag++;

            if (hp <= 0 || ehp <= 0)
            {
                EndGame();
            }
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

        private void MGBattleRobot_Paint(object sender, PaintEventArgs e)
        {
            BorderPainter.Draw(e.Graphics, "", Width, Height);

            Font font = new Font("黑体", 12*1.33f, FontStyle.Bold, GraphicsUnit.Pixel);
            e.Graphics.DrawString("机器人大战", font, Brushes.White, 140, 8);
            font.Dispose();

            if (!show)
                return;

            e.Graphics.DrawImage(backImage, xoff, yoff, 324, 244);

            font = new Font("宋体", 11*1.33f, FontStyle.Bold, GraphicsUnit.Pixel);
            DrawShadeText(e.Graphics, "我方机器人", font, Brushes.Wheat, 20+xoff, 11+yoff);
            DrawShadeText(e.Graphics, string.Format("生  命 {0}", hp > 0 ? hp : 0), font, Brushes.Lime, 20+xoff, 40+yoff);
            SolidBrush sb = new SolidBrush(Color.FromArgb(160, Color.Blue));
            if ((stag % 2) == 0)
            {
                e.Graphics.FillRectangle(sb, 17+xoff, 55+yoff, 100, 60);
            }
            else
            {
                e.Graphics.FillRectangle(sb, 17+xoff, 117+yoff, 100, 60);
            }
            sb.Dispose();
            DrawShadeText(e.Graphics, string.Format("激光刀 {0}", attrs[0, 0]), font, lastact == 0 ? Brushes.Gold : Brushes.White, 20+xoff, 60+yoff);
            DrawShadeText(e.Graphics, string.Format("导  弹 {0}", attrs[1, 0]), font, lastact == 1 ? Brushes.Gold : Brushes.White, 20 + xoff, 80 + yoff);
            DrawShadeText(e.Graphics, string.Format("空  袭 {0}", attrs[2, 0]), font, lastact == 2 ? Brushes.Gold : Brushes.White, 20 + xoff, 100 + yoff);
            DrawShadeText(e.Graphics, string.Format("格斗盾 {0}%", attrs[3, 0]), font, lastact == 3 ? Brushes.Gold : Brushes.White, 20 + xoff, 120 + yoff);
            DrawShadeText(e.Graphics, string.Format("拦截弹 {0}%", attrs[4, 0]), font, lastact == 4 ? Brushes.Gold : Brushes.White, 20 + xoff, 140 + yoff);
            DrawShadeText(e.Graphics, string.Format("干扰器 {0}%", attrs[5, 0]), font, lastact == 5 ? Brushes.Gold : Brushes.White, 20 + xoff, 160 + yoff);

            DrawShadeText(e.Graphics, "对方机器人", font, Brushes.Gold, 180+xoff, 11+yoff);
            DrawShadeText(e.Graphics, string.Format("生  命 {0}", ehp > 0 ? ehp : 0), font, Brushes.Lime, 180+xoff, 40+yoff);
            DrawShadeText(e.Graphics, string.Format("激光刀 {0}", eattrs[0, 1] == 0 ? "??" : eattrs[0, 0].ToString()), font, lasteact == 0 ? Brushes.Gold : Brushes.White, 180+xoff, 60+yoff);
            DrawShadeText(e.Graphics, string.Format("导  弹 {0}", eattrs[1, 1] == 0 ? "??" : eattrs[1, 0].ToString()), font, lasteact == 1 ? Brushes.Gold : Brushes.White, 180 + xoff, 80 + yoff);
            DrawShadeText(e.Graphics, string.Format("空  袭 {0}", eattrs[2, 1] == 0 ? "??" : eattrs[2, 0].ToString()), font, lasteact == 2 ? Brushes.Gold : Brushes.White, 180 + xoff, 100 + yoff);
            DrawShadeText(e.Graphics, string.Format("格斗盾 {0}%", eattrs[3, 1] == 0 ? "??" : eattrs[3, 0].ToString()), font, lasteact == 3 ? Brushes.Gold : Brushes.White, 180 + xoff, 120 + yoff);
            DrawShadeText(e.Graphics, string.Format("拦截弹 {0}%", eattrs[4, 1] == 0 ? "??" : eattrs[4, 0].ToString()), font, lasteact == 4 ? Brushes.Gold : Brushes.White, 180 + xoff, 140 + yoff);
            DrawShadeText(e.Graphics, string.Format("干扰器 {0}%", eattrs[5, 1] == 0 ? "??" : eattrs[5, 0].ToString()), font, lasteact == 5 ? Brushes.Gold : Brushes.White, 180 + xoff, 160 + yoff);

            DrawShadeText(e.Graphics, resultStr, font, Brushes.Red, 20+xoff, 210+yoff);

            font.Dispose();
        }
    }
}
