using System;
using System.Collections.Generic;
using System.Drawing;
using System.Threading;
using System.Windows.Forms;
using NarlonLib.Math;
using TaleofMonsters.Controler.Loader;

namespace TaleofMonsters.Forms.MiniGame
{
    internal partial class MGLinkGame : MGBase
    {
        private class LinkUnit
        {
            public int Value;
            public int Depth;
            public int Distance;
            public int Parent;
            public LinkUnit(int value, int depth, int distance, int parent)
            {
                this.Value = value;
                this.Depth = depth;
                this.Distance = distance;
                this.Parent = parent;
            }
        }
        private enum CheckMethod
        {
            Try, Test
        }

        private const int imageSize = 40;
        private const int margin = 2;
        private const int ColumnCount = 12;
        private const int RowCount = 7;
        private Image[] iconTypes = new Bitmap[30];
        private int[,] iconArray = new int[ColumnCount + 2, RowCount+2];
        private int cur = -1;
        private float timeLeft;
        private bool isPlaying;
        private const int RoundTime = 120;
        private const double ComboTime = 1.5;

        private DateTime lastHitTime;
        private int winLevelCount; //连胜回合数

        public MGLinkGame()
        {
            InitializeComponent();
            this.bitmapButtonC1.ImageNormal = PicLoader.Read("Button.Panel", "ButtonBack2.PNG");
            bitmapButtonC1.Font = new Font("宋体", 8 * 1.33f, FontStyle.Regular, GraphicsUnit.Pixel);
            bitmapButtonC1.ForeColor = Color.White;
            bitmapButtonC1.IconImage = TaleofMonsters.Core.HSIcons.GetIconsByEName("rot1");
            bitmapButtonC1.IconSize = new Size(16, 16);
            bitmapButtonC1.IconXY = new Point(4, 5);
            bitmapButtonC1.TextOffX = 8;
            bitmapButtonC1.Text = @"开始";
            for (int i = 0; i < 16; i++)
            {
                iconTypes[i] = PicLoader.Read("MiniGame.LinkGame", String.Format("{0}.JPG", i + 1));
            }
            for (int i = 0; i < 6; i++)
            {
                iconTypes[20 + i] = PicLoader.Read("MiniGame.LinkGame", String.Format("s{0}.JPG", i + 1));
            }
            xoff = 0;
        }

        public override void Init(int width, int height)
        {
            base.Init(width, height);
        }
        public override void OnFrame(int tick, float timePass)
        {
            base.OnFrame(tick, timePass);

            if (isPlaying && timeLeft > 0)
            {
                timeLeft -= timePass;
                Invalidate(new Rectangle(xoff + 550, yoff + 70, 100, 20));

                BeginCalculateResult();
            }
        }

        public override void RestartGame()
        {
            base.RestartGame();
            lastHitTime = DateTime.Now;
            timeLeft = RoundTime;
            score = 0;
            isPlaying = true;
            InitIconArray();

            Invalidate();
        }

        public override void EndGame()
        {
            base.EndGame();
            isPlaying = false;
        }

        private void InitIconArray()
        {
            for (int i = 0; i < ColumnCount+2; i++)
            {
                for (int j = 0; j < RowCount+2; j++)
                {
                    iconArray[i, j] = 0;
                }
            }
            for (int i = 0; i < ColumnCount; i++)
            {
                for (int j = 0; j < RowCount; j++)
                {
                    iconArray[i + 1, j + 1] = ((i + j * ColumnCount)/2)%16+1;
                }
            }
            for (int i = 0; i < 4; i++)
            {
                int sel = (MathTool.GetRandom(0, int.MaxValue) % (RowCount*ColumnCount))/2*2; //取偶
                int symid = (MathTool.GetRandom(0, int.MaxValue) % 6) + 1;
                iconArray[(sel % ColumnCount) + 1, sel / ColumnCount + 1] = 20 + symid;
                iconArray[(sel % ColumnCount) + 1 + 1, sel / ColumnCount + 1] = 20 + symid;
            }

            for (int i = 0; i < 1000; i++)
            {
                Swap(MathTool.GetRandom(0, int.MaxValue) % ColumnCount, MathTool.GetRandom(0, int.MaxValue) % RowCount,
                   MathTool.GetRandom(0, int.MaxValue) % ColumnCount, MathTool.GetRandom(0, int.MaxValue) % RowCount);
            }
        }

        protected override void CalculateResult()
        {
            if (timeLeft <= 0)
            {
                EndGame();
            }
        }

        private void Swap(int i1, int j1, int i2, int j2)
        {
            int temp = iconArray[i1 + 1, j1 + 1];
            iconArray[i1 + 1, j1 + 1] = iconArray[i2 + 1, j2 + 1];
            iconArray[i2 + 1, j2 + 1] = temp;
        }

        private void LinkGamePanel_MouseClick(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left && isPlaying)
            {
                int totalColumn = ColumnCount + 2;
                int realX = e.X - xoff;
                int realY = e.Y - yoff;
                int click;
                if (realX < imageSize * totalColumn && realY < imageSize * (RowCount+2) && iconArray[realX / imageSize, realY / imageSize] != 0)
                {
                    click = realX / imageSize + realY / imageSize * totalColumn;
                }
                else
                    return;
                if (click == cur)
                {
                    return;
                }
                if (cur == -1)
                {
                    cur = click;
                }
                else
                {
                    if (FindPath(click, cur, CheckMethod.Try))
                    {//消除
                        AddMark(iconArray[click % totalColumn, click / totalColumn]);
                        iconArray[click % totalColumn, click / totalColumn] = 0;
                        iconArray[cur % totalColumn, cur / totalColumn] = 0;
                        //  Sounder s = new Sounder("ok");

                        if (DateTime.Now.Subtract(lastHitTime).TotalSeconds < ComboTime)
                        {
                            timeLeft += (float)DateTime.Now.Subtract(lastHitTime).TotalSeconds; //这一步不算时间
                        }

                        lastHitTime = DateTime.Now;
                        Thread.Sleep(100);
                    }
                    else
                     {
                        //   Sounder s = new Sounder("err");
                    }
                    cur = -1;
                }
                if (FinishLevel())
                {
                    InitIconArray(); //下一关
                    winLevelCount ++;
                    if (winLevelCount < 5)
                    {
                        timeLeft += (float)RoundTime * 0.75f * (5-winLevelCount)/5;
                    }
                    else
                    {
                        timeLeft += (float)RoundTime * 0.75f / 10;
                    }
                }
                while (!GameAvail())
                    Resort();
                Invalidate();
            }
        }

        private int GetIconArray(int v)
        {
            int totalColumn = ColumnCount + 2;
            return iconArray[v % totalColumn, v / totalColumn];
        }

        private bool ContainV(List<LinkUnit> list, int v)
        {
            for (int i = 0; i < list.Count; i++)
                if (list[i].Value == v)
                    return true;
            return false;
        }

        #region 寻找匹配
        private bool FindPath(int click, int matchTarget, CheckMethod method)
        {
            bool f = false;
            int mindis = 999999;
            int lastNodeIndex = -1;
            if (GetIconArray(click) != GetIconArray(matchTarget))
            {
                return false;
            }
            List<LinkUnit> list = new List<LinkUnit>();
            list.Add(new LinkUnit(click, 0, 0, -1));
            int totalColumn = ColumnCount + 2;
            for (int i = 0; i < 3 && !f; i++)
            {
                for (int j = 0; j < list.Count; j++)
                {
                    if (list[j].Depth == i)
                    {
                        for (int k = list[j].Value - 1, dis = 0; k >= list[j].Value - (list[j].Value % totalColumn); k--, dis++)
                        {
                            if (TryCell(matchTarget, k, list, j, dis, ref mindis, ref lastNodeIndex, ref f)) 
                                break;
                        }
                        for (int k = list[j].Value + 1, dis = 0; k <= list[j].Value - (list[j].Value % totalColumn) + totalColumn-1; k++, dis++)
                        {
                            if (TryCell(matchTarget, k, list, j, dis, ref mindis, ref lastNodeIndex, ref f)) 
                                break;
                        }
                        for (int k = list[j].Value - totalColumn, dis = 0; k >= 0; k -= totalColumn, dis++)
                        {
                            if (TryCell(matchTarget, k, list, j, dis, ref mindis, ref lastNodeIndex, ref f)) 
                                break;
                        }
                        for (int k = list[j].Value + totalColumn, dis = 0; k < totalColumn * (RowCount+2); k += totalColumn, dis++)
                        {
                            if (TryCell(matchTarget, k, list, j, dis, ref mindis, ref lastNodeIndex, ref f)) 
                                break;
                        }
                    }
                }
            }

            if (!f)
                return false;

            if (method == CheckMethod.Try)
            {
                DrawMatchLine(click, matchTarget, lastNodeIndex, list);
            }

            return true;
        }

        private bool TryCell(int matchTarget, int picked, List<LinkUnit> list, int j, int dis, ref int mindis, ref int lastnode, ref bool f)
        {
            if (picked == matchTarget && list[j].Distance + dis < mindis)
            {
                mindis = list[j].Distance + dis;
                lastnode = j;
                f = true;
            }
            if (GetIconArray(picked) > 0)
                return true;
            else
            {
                if (!ContainV(list, picked))
                    list.Add(new LinkUnit(picked, list[j].Depth + 1, list[j].Distance + dis, j));
            }
            return false;
        }

        #endregion

        private void DrawMatchLine(int click, int matchTarget, int lastNodeIndex, List<LinkUnit> list)
        {
            Graphics g = this.CreateGraphics();

            if (lastNodeIndex <= 0)
            {
                DrawLine(g, matchTarget, click);
            }
            else
            {
                int target = matchTarget;
                int source = list[lastNodeIndex].Value;
                lastNodeIndex = list[lastNodeIndex].Parent;
                do
                {
                    DrawLine(g, source, target);
                    target = source;
                    source = list[lastNodeIndex].Value;
                    lastNodeIndex = list[lastNodeIndex].Parent;
                } while (lastNodeIndex != -1);
                DrawLine(g, source, target);
            }
        }
        private void DrawLine(Graphics g, int source, int target)
        {
            int totalColumn = ColumnCount + 2;
            Pen p = new Pen(Brushes.Blue, 5);
            g.DrawLine(p, (source % totalColumn) * imageSize + xoff + imageSize / 2, (source / totalColumn) * imageSize + yoff + imageSize / 2,
                (target % totalColumn) * imageSize + xoff + imageSize / 2, (target / totalColumn) * imageSize + yoff + imageSize / 2);
            p.Dispose();
        }

        /// <summary>
        /// 判断游戏是否结束
        /// </summary>
        private bool FinishLevel()
        {
            for (int i = 1; i < 1+ColumnCount; i++)
            {
                for (int j = 1; j < 1+RowCount; j++)
                {
                    if (iconArray[i, j] != 0)
                        return false;
                }
            }
            return true;
        }

        /// <summary>
        /// 重排连连子
        /// </summary>
        private void Resort()
        {
            List<int> li = new List<int>();
            for (int i = 1; i < 1 + ColumnCount; i++)
            {
                for (int j = 1; j < 1 + RowCount; j++)
                {
                    if (iconArray[i, j] != 0)
                        li.Add(iconArray[i, j]);
                }
            }
            Random r = new Random();
            for (int i = 0; i < ColumnCount+2; i++)
            {
                int p = r.Next() % li.Count;
                int v = li[p];
                li.RemoveAt(p);
                li.Add(v);
            }
            for (int i = 1; i < 1 + ColumnCount; i++)
            {
                for (int j = 1; j < 1 + RowCount; j++)
                {
                    if (iconArray[i, j] != 0)
                    {
                        iconArray[i, j] = li[0];
                        li.RemoveAt(0);
                    }
                }
            }
            //         Invalidate();
        }

        /// <summary>
        /// 检测是否死锁
        /// </summary>
        private bool GameAvail()
        {
            int totalColumn = ColumnCount + 2;
            for (int i1 = 1, j1 = 1; i1 < 1+ColumnCount || j1 < 1+RowCount; i1++)
            {
                if (i1 == 1 + ColumnCount)
                {
                    i1 = 0;
                    j1++;
                }
                for (int i2 = 1, j2 = 1; i2 < 1+ColumnCount || j2 < 1+RowCount; i2++)
                {
                    if (i2 == 1 + ColumnCount)
                    {
                        i2 = 0;
                        j2++;
                    }
                    if (i1 + j1 * totalColumn == i2 + j2 * totalColumn || iconArray[i1, j1] == 0 || (iconArray[i1, j1] != iconArray[i2, j2]))
                    {
                        continue;
                    }
                    if (FindPath(i1 + j1 * totalColumn, i2 + j2 * totalColumn, CheckMethod.Test))
                    {
                        return true;
                    }
                }
            }
            return false;
        }


        private void AddMark(int ltype)
        {
            score += 40;
            switch (ltype)
            {
                case 21: score += 100; break;
                case 22: score += 150; break;
                case 23: score += 40 * 3; break;
                case 24: score += 40 * 5; break;
                case 25: timeLeft += 5; break;
                case 26: timeLeft += 10; break;
            }
            Invalidate(new Rectangle(xoff + 550, yoff + 70, 100, 20)); //刷时间
            Invalidate(new Rectangle(xoff + 550, yoff + 120, 100, 20)); //刷分数
        }

        private void bitmapButtonC1_Click(object sender, EventArgs e)
        {
            if (timeLeft <= 0)
            {
                RestartGame();
                bitmapButtonC1.Visible = false;
            }
        }
        
        private void MGLinkGame_Paint(object sender, PaintEventArgs e)
        {
            DrawBase(e.Graphics);

            Font font = new Font("宋体", 11);
            e.Graphics.DrawString("时间剩余", font, Brushes.White, xoff + 550, yoff + 50);
            e.Graphics.DrawString(string.Format("{0:0}:{1:00}", (int)(timeLeft/60), (int)timeLeft % 60), font, Brushes.White, xoff + 550, yoff + 70);

            e.Graphics.DrawString("得分", font, Brushes.White, xoff +550, yoff + 100);
            e.Graphics.DrawString(score.ToString(), font, Brushes.White, xoff + 550, yoff + 120);
            font.Dispose();

            if (!show)
                return;

            for (int i = 0; i < ColumnCount; i++)
            {
                for (int j = 0; j < RowCount; j++)
                {
                    if (iconArray[i + 1, j + 1] != 0)
                        e.Graphics.DrawImage(iconTypes[iconArray[i + 1, j + 1] - 1], new Rectangle(new Point(xoff + (i + 1) * imageSize + margin, yoff + (j + 1) * imageSize + margin), new Size(imageSize, imageSize)));
                }
            }
            if (cur != -1)
            {
                Pen p = new Pen(Brushes.Red, 3);
                e.Graphics.DrawRectangle(p, new Rectangle(new Point(xoff + (cur % (ColumnCount + 2)) * imageSize + margin, yoff + (cur / (ColumnCount + 2)) * imageSize + margin), new Size(imageSize, imageSize)));
            }
        }

    }
}
