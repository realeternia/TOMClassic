using System;
using System.Collections.Generic;
using System.Drawing;
using System.Threading;
using System.Windows.Forms;
using ControlPlus;
using NarlonLib.Math;
using TaleofMonsters.DataType.User;
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

        private bool isFail;
        private const int imageSize = 40;
        private const int margin = 2;
        private const int ColumnCount = 12;
        private const int RowCount = 7;
        private Image[] iconTypes = new Bitmap[30];
        private int[,] iconArray = new int[ColumnCount + 2, RowCount+2];
        private int cur = -1;
        private bool timeOver = false;
        private int mark = 0;

        public MGLinkGame()
        {
            InitializeComponent();
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

        internal override void Init(int width, int height)
        {
            base.Init(width, height);
        }

        public override void RestartGame()
        {
            isFail = false;
            mark = 0;
            InitIconArray();
        }

        public override void EndGame()
        {
            string hint="";
            //if (!isFail && GetPoints()>=100)
            //{
            //    hint = "获得了游戏胜利";
            //    UserProfile.InfoBag.AddDiamond(10);
            //}
            //else
            //{
            //    hint = "你输了";
            //}

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
            timeOver = false;
            Invalidate();
        }

        private void Swap(int i1, int j1, int i2, int j2)
        {
            int temp = iconArray[i1 + 1, j1 + 1];
            iconArray[i1 + 1, j1 + 1] = iconArray[i2 + 1, j2 + 1];
            iconArray[i2 + 1, j2 + 1] = temp;
        }

        private void LinkGamePanel_MouseClick(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left && !timeOver)
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
                        Thread.Sleep(200);
                    }
                    else
                    {
                        //   Sounder s = new Sounder("err");
                    }
                    cur = -1;
                }
                if (HasWin())
                {
                    isFail = false;
                    EndGame();
                    return;
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
                        for (int k = list[j].Value + totalColumn, dis = 0; k <= 99; k += totalColumn, dis++)
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
        private bool HasWin()
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
            mark += 40;
            switch (ltype)
            {
                case 21: mark += 100; break;
                case 22: mark += 150; break;
                case 23: mark += 40 * 3; break;
                case 24: mark += 40 * 5; break;
            }
            Invalidate();
            if (mark >= 20000)
            {
                isFail = false;
                EndGame();
            }
        }


        private void MGLinkGame_Paint(object sender, PaintEventArgs e)
        {
            DrawBase(e.Graphics);

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
