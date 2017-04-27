using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using NarlonLib.Math;
using TaleofMonsters.Controler.Loader;
using TaleofMonsters.MainItem;

namespace TaleofMonsters.Forms.MiniGame
{
    internal partial class MGRussiaBlock : MGBase
    {
        private enum BlockType
        {
            None, Square, Line, T, Z, CZ, L, CL
        }

        private const int RowCount = 20;
        private const int ColumnCount = 10;
        private const int CellSize = 19;

        private int[,] cellMap;

        private bool isPlaying;
        private Point targetPos;
        private BlockType targetType;
        private int targetDirect; //0-3

        private BlockType nextType;

        private int pastTime;

        private bool keyDownPressed;
        private bool keyLeftPressed;
        private bool keyRightPressed;

        private int lastPressCheckTime;

        public MGRussiaBlock()
        {
            InitializeComponent();
            this.bitmapButtonC1.ImageNormal = PicLoader.Read("ButtonBitmap", "ButtonBack2.PNG");
            bitmapButtonC1.Font = new Font("宋体", 8 * 1.33f, FontStyle.Regular, GraphicsUnit.Pixel);
            bitmapButtonC1.ForeColor = Color.White;
            bitmapButtonC1.IconImage = TaleofMonsters.Core.HSIcons.GetIconsByEName("rot1");
            bitmapButtonC1.IconSize = new Size(16, 16);
            bitmapButtonC1.IconXY = new Point(4, 5);
            bitmapButtonC1.TextOffX = 8;
            bitmapButtonC1.Text = @"开始";
            xoff += 15;
        }

        public override void Init(int width, int height)
        {
            base.Init(width, height);
            SystemMenuManager.IsHotkeyEnabled = false;//防止乱弹界面
        }

        public override void OnFrame(int tick, float timePass)
        {
            base.OnFrame(tick, timePass);

            if (isPlaying)
            {
                TimelyMoveBlock(tick);
                Invalidate(new Rectangle(xoff, yoff, 190 + 4, 380 + 4));
            }
        }
        public override void OnRemove()
        {
            SystemMenuManager.IsHotkeyEnabled = true;
        }

        public override void RestartGame()
        {
            base.RestartGame();

            isPlaying = true;
            cellMap = new int[ColumnCount, RowCount];
            nextType = 0;
            NewBlock();
            keyDownPressed = false;
            keyLeftPressed = false;
            keyRightPressed = false;
            lastPressCheckTime = 0;
            Invalidate();
        }

        public override void EndGame()
        {
            base.EndGame();
        }
        private void TimelyMoveBlock(int tick)
        {
            if (keyDownPressed || keyLeftPressed || keyRightPressed)
            {
                if (lastPressCheckTime == 0 || tick - lastPressCheckTime >= 3)
                {
                    lastPressCheckTime = tick;
                    if (keyDownPressed)
                    {
                        pastTime += 20;
                    }
                    else if (keyLeftPressed || keyRightPressed)
                    {
                        if (keyLeftPressed)
                        {
                            targetPos.X--;
                            if (!CanChange())
                                targetPos.X++;
                        }
                        else if (keyRightPressed)
                        {
                            targetPos.X++;
                            if (!CanChange())
                                targetPos.X--;
                        }
                    }
                }
            }

            pastTime++; //自然下落
            while (pastTime >= 10)
            {
                pastTime -= 10;

                targetPos.Y++;
                var fail = !CanChange();
                if (fail) //到底了
                {
                    targetPos.Y--;
                    AddMark(10);
                    CheckCrash();
                    NewBlock();
                }
            }
        }

        private void CheckCrash()
        {
            var possess = GetCellPossess(targetPos, targetType);
            List<int> removeLineList = new List<int>();
            foreach (var point in possess)
            {
                cellMap[point.X, point.Y] = Color.Gray.ToArgb();
                int i = 0;
                for (i = 0; i < ColumnCount; i++)
                {
                    if (cellMap[i, point.Y] == 0)
                    {
                        break;
                    }
                }
                if (i == ColumnCount) //消除
                {
                    if (!removeLineList.Contains(point.Y))
                    {
                        removeLineList.Add(point.Y);
                        AddMark(100);
                    }
                }
            }

            removeLineList.Sort();//从小到大？
            if (removeLineList.Count > 0)
            {
                foreach (var lineNumber in removeLineList)
                {
                    for (int j = lineNumber; j >= 1; j--)
                    {
                        for (int i = 0; i < ColumnCount; i++)
                        {
                            cellMap[i, j] = cellMap[i, j - 1];
                        }
                    }
                }
            }
        }

        private void NewBlock()
        {
            if (nextType != BlockType.None)
                targetType = nextType;
            else
                targetType = (BlockType)MathTool.GetRandom(7)+ 1;
            nextType = (BlockType)MathTool.GetRandom(7) + 1;
            Invalidate(new Rectangle(xoff + ColumnCount * CellSize + 20, yoff, 80, 80));

            targetPos = new Point(MathTool.GetRandom(3, ColumnCount-3) , 1);
            targetDirect = MathTool.GetRandom(4);

            BeginCalculateResult();
        }
        
        protected override void CalculateResult()
        {
            if (!CanChange())
            {
                EndGame();
            }
        }

        private bool CanChange()
        {
            var possess = GetCellPossess(targetPos, targetType);
            foreach (var point in possess)
            {
                if (point.X < 0 || point.X >= ColumnCount)
                {
                    return false;
                }

                if (point.Y >= RowCount || cellMap[point.X, point.Y] != 0)
                {
                    return false;
                }
            }
            return true;
        }

        private List<Point> GetCellPossess(Point ind, BlockType type)
        {
            List<Point> datas = new List<Point>();
            switch (type)
            {
                case BlockType.Square:
                    datas.Add(new Point(0,0)); datas.Add(new Point(1, 1)); datas.Add(new Point(1, 0)); datas.Add(new Point(0, 1)); break;
                case BlockType.Line:
                    if (targetDirect == 0 || targetDirect == 2)
                    {
                        datas.Add(new Point(0, 0));
                        datas.Add(new Point(-1, 0));
                        datas.Add(new Point(1, 0));
                        datas.Add(new Point(2, 0));
                    }
                    else
                    {
                        datas.Add(new Point(0, 0));
                        datas.Add(new Point(0, -1));
                        datas.Add(new Point(0, 1));
                        datas.Add(new Point(0, 2));
                    }
                    break;
                case BlockType.T:
                    if (targetDirect == 0)
                    {
                        datas.Add(new Point(0, 0));
                        datas.Add(new Point(0, -1));
                        datas.Add(new Point(1, 0));
                        datas.Add(new Point(-1, 0));
                    }
                    else if (targetDirect == 1)
                    {
                        datas.Add(new Point(0, 0));
                        datas.Add(new Point(0, -1));
                        datas.Add(new Point(1, 0));
                        datas.Add(new Point(0, 1));
                    }
                    else if (targetDirect == 2)
                    {
                        datas.Add(new Point(0, 0));
                        datas.Add(new Point(0, 1));
                        datas.Add(new Point(1, 0));
                        datas.Add(new Point(-1, 0));
                    }
                    else if (targetDirect == 3)
                    {
                        datas.Add(new Point(0, 0));
                        datas.Add(new Point(0, -1));
                        datas.Add(new Point(-1, 0));
                        datas.Add(new Point(0, 1));
                    }
                    break;
                case BlockType.Z:
                    if (targetDirect == 0 || targetDirect == 2)
                    {
                        datas.Add(new Point(0, 0));
                        datas.Add(new Point(0, -1));
                        datas.Add(new Point(-1, -1));
                        datas.Add(new Point(1, 0));
                    }
                    else
                    {
                        datas.Add(new Point(0, 0));
                        datas.Add(new Point(0, 1));
                        datas.Add(new Point(1, 0));
                        datas.Add(new Point(1, -1));
                    }
                    break;
                case BlockType.CZ:
                    if (targetDirect == 0 || targetDirect == 2)
                    {
                        datas.Add(new Point(0, 0));
                        datas.Add(new Point(0, -1));
                        datas.Add(new Point(1, -1));
                        datas.Add(new Point(-1, 0));
                    }
                    else
                    {
                        datas.Add(new Point(0, 0));
                        datas.Add(new Point(0, -1));
                        datas.Add(new Point(1, 0));
                        datas.Add(new Point(1, 1));
                    }
                    break;
                case BlockType.L:
                    if (targetDirect == 0)
                    {
                        datas.Add(new Point(0, 0));
                        datas.Add(new Point(1, 0));
                        datas.Add(new Point(-1, 0));
                        datas.Add(new Point(-1, 1));
                    }
                    else if (targetDirect == 1)
                    {
                        datas.Add(new Point(0, 0));
                        datas.Add(new Point(0, -1));
                        datas.Add(new Point(0, 1));
                        datas.Add(new Point(-1, -1));
                    }
                    else if (targetDirect == 2)
                    {
                        datas.Add(new Point(0, 0));
                        datas.Add(new Point(1, 0));
                        datas.Add(new Point(-1, 0));
                        datas.Add(new Point(1, -1));
                    }
                    else if (targetDirect == 3)
                    {
                        datas.Add(new Point(0, 0));
                        datas.Add(new Point(0, -1));
                        datas.Add(new Point(0, 1));
                        datas.Add(new Point(1, 1));
                    }
                    break;
                case BlockType.CL:
                    if (targetDirect == 0)
                    {
                        datas.Add(new Point(0, 0));
                        datas.Add(new Point(1, 0));
                        datas.Add(new Point(-1, 0));
                        datas.Add(new Point(-1, -1));
                    }
                    else if (targetDirect == 1)
                    {
                        datas.Add(new Point(0, 0));
                        datas.Add(new Point(0, -1));
                        datas.Add(new Point(0, 1));
                        datas.Add(new Point(1, -1));
                    }
                    else if (targetDirect == 2)
                    {
                        datas.Add(new Point(0, 0));
                        datas.Add(new Point(1, 0));
                        datas.Add(new Point(-1, 0));
                        datas.Add(new Point(1, 1));
                    }
                    else if (targetDirect == 3)
                    {
                        datas.Add(new Point(0, 0));
                        datas.Add(new Point(0, -1));
                        datas.Add(new Point(0, 1));
                        datas.Add(new Point(-1, 1));
                    }
                    break;
            }

            for (int i = 0; i < datas.Count; i++)
            {
                datas[i] = new Point(datas[i].X+ ind.X, datas[i].Y+ ind.Y);
            }
            return datas;
        }

        private Color GetCellColor(BlockType t)
        {
            switch (t)
            {
                case BlockType.Line: return Color.PaleVioletRed;
                case BlockType.Square: return Color.DeepSkyBlue;
                case BlockType.T: return Color.LimeGreen;
                case BlockType.Z: return Color.DarkGreen;
                case BlockType.CZ: return Color.Brown;
                case BlockType.L: return Color.Yellow;
                case BlockType.CL: return Color.Violet;
            }
            return Color.White;
        }

        public override void OnHsKeyUp(KeyEventArgs e)
        {
            if (e.KeyCode == Keys.S)
            {
                keyDownPressed = false;
                lastPressCheckTime = 0;
            }
            if (e.KeyCode == Keys.A)
            {
                keyLeftPressed = false;
                lastPressCheckTime = 0;
            }
            if (e.KeyCode == Keys.D)
            {
                keyRightPressed = false;
                lastPressCheckTime = 0;
            }
            if (e.KeyCode == Keys.K)
            {
                targetDirect = (targetDirect + 1)%4;
                if (!CanChange())
                    targetDirect = (targetDirect - 1) % 4;
                Invalidate(new Rectangle(xoff, yoff, 190, 380));
            } 
            if (e.KeyCode == Keys.J)
            {
                targetDirect = (targetDirect - 1) % 4;
                if (!CanChange())
                    targetDirect = (targetDirect + 1) % 4;
                Invalidate(new Rectangle(xoff, yoff, 190, 380));
            }
        }
        public override void OnHsKeyDown(KeyEventArgs e)
        {
            if (e.KeyCode == Keys.S)
            {
                keyDownPressed = true;
            }
            if (e.KeyCode == Keys.A)
            {
                keyLeftPressed = true;
            }
            if (e.KeyCode == Keys.D)
            {
                keyRightPressed = true;
            }
        }

        private void AddMark(int add)
        {
            score += add;
            Invalidate(new Rectangle(xoff + ColumnCount * CellSize + 20, yoff + 100, 100, 40));
            if (score >= 20000)
            {
                EndGame();
            }
        }
        private void bitmapButtonC1_Click(object sender, System.EventArgs e)
        {
            if (!isPlaying)
            {
                RestartGame();
                bitmapButtonC1.Visible = false;
            }
        }

        private void MGRussiaBlock_Paint(object sender, PaintEventArgs e)
        {
            DrawBase(e.Graphics);

            e.Graphics.DrawRectangle(Pens.Gray, xoff, yoff, ColumnCount * CellSize, RowCount * CellSize);
            e.Graphics.DrawRectangle(Pens.Gray, xoff + ColumnCount * CellSize +20, yoff, 80,80);

            Font font = new Font("宋体", 11);
            e.Graphics.DrawString("得分", font, Brushes.White, xoff + ColumnCount * CellSize + 20, yoff + 100);
            e.Graphics.DrawString(score.ToString(), font, Brushes.White, xoff + ColumnCount * CellSize + 20, yoff + 120);
            font.Dispose();

            if (!show || !isPlaying)
                return;
            
            //for (int i = 0; i <= RowCount; i++)
            //{
            //    e.Graphics.DrawLine(Pens.Gray, xoff, yoff + i * CellSize, xoff + ColumnCount * CellSize, yoff + i * CellSize);
            //}
            //for (int i = 0; i <= ColumnCount; i++)
            //{
            //    e.Graphics.DrawLine(Pens.Gray, xoff + i * CellSize, yoff, xoff + i * CellSize, yoff + RowCount * CellSize);
            //}

            for (int i = 0; i < ColumnCount; i++)
            {
                for (int j = 0; j < RowCount; j++)
                {
                    var val = cellMap[i, j];
                    if (val != 0)
                    {
                        DrawCell(e, new Point(i,j), Color.FromArgb(val));
                    }
                }
            }

            var possess = GetCellPossess(targetPos, targetType);
            foreach (var point in possess)
            {
                DrawCell(e, point, GetCellColor(targetType));
            }

            if (nextType != BlockType.None)
            {
                possess = GetCellPossess(new Point(ColumnCount + 2, 1), nextType);
                foreach (var point in possess)
                {
                    DrawCell(e, point, GetCellColor(nextType));
                }
            }
        }

        private void DrawCell(PaintEventArgs e, Point point, Color color)
        {
            Brush b = new SolidBrush(color);
            e.Graphics.FillRectangle(b, xoff + point.X*CellSize + 1, yoff + point.Y*CellSize + 1, CellSize - 2, CellSize - 2);
            b.Dispose();
        }
    }
}
