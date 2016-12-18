using System;
using System.Drawing;
using System.Windows.Forms;
using TaleofMonsters.Controler.Loader;
using TaleofMonsters.Core;
using TaleofMonsters.DataType.Mazes;
using System.IO;
using TaleofMonsters.Forms.Items.Core;
using TaleofMonsters.MainItem;
using TaleofMonsters.MainItem.Scenes;

namespace TaleofMonsters.Forms
{
    internal sealed partial class MazeWindow : BasePanel
    {
        private int npcId;
        private int mazeId;
        private Maze maze;
        private int[,] mazeState;
        private int x, y;
        private int ox, oy;
        private int wx, wy;
        private int last;
        private int heroAct;
        private bool getEnd = true;
        private Image mazeBack;
        private Image hero;
        private int mWidth;
        private int mHeight;
        private HSCursor myCursor;

        public int NpcId
        {
            get { return npcId; }
            set { npcId = value; }
        }

        public int MazeId
        {
            get { return mazeId; }
            set
            {
                mazeId = value;
                maze = new Maze(mazeId);
                initMazeData();
                freshButtonState();
            }
        }

        private void initMazeData()
        {
            #region 数据装载
            StreamReader sr = new StreamReader(DataLoader.Read("Maze", string.Format("{0}.maz", maze.MazeConfig.Path)));
            mWidth = int.Parse(sr.ReadLine());
            mHeight = int.Parse(sr.ReadLine());
            mazeState = new int[mWidth, mHeight];
            for (int i = 0; i < mHeight; i++)
            {
                string[] datas = sr.ReadLine().Split(' ');
                for (int j = 0; j < mWidth; j++)
                {
                    mazeState[j, i] = int.Parse(datas[j]);
                    if (mazeState[j, i] == 2)
                    {
                        x = j;
                        y = i;
                    }
                    else if (mazeState[j, i] == 3)
                    {
                        getEnd = false;
                    }
                }
            }
            sr.Close();
            #endregion

            wx = x - 5;
            wy = y - 5;
            if (wx < 0)
                wx = 0;
            if (wy < 0)
                wy = 0;
            if (mWidth >= 10 && wx + 10 > mWidth)
                wx = mWidth - 10;
            if (mHeight >= 10 && wy + 10 > mHeight)
                wy = mHeight - 10;

            mazeBack = PicLoader.Read("Maze", string.Format("{0}.JPG", maze.MazeConfig.Path));
            hero = PicLoader.Read("Map", "hero.PNG");
        }

        public MazeWindow()
        {
            InitializeComponent();
            this.bitmapButtonClose.ImageNormal = PicLoader.Read("ButtonBitmap", "CloseButton1.JPG");
            myCursor = new HSCursor(this);
        }

        internal override void OnFrame(int tick)
        {
            base.OnFrame(tick);
            if ((tick % 6) == 0)
            {
                heroAct = ((++heroAct)%4);
                Invalidate();
            }
        }

        private void freshButtonState()
        {
            if (mazeState[x, y] == 3)
            {
                if (!getEnd)
                {
                    getEnd = true;
                }
            }
            if (mazeState[x, y] != 4)
            {
                maze.CheckEvent(this, x, y, onSuccess, onFail);
            }
        }

        private void onSuccess()
        {
            mazeState[x, y] = 4;
            Invalidate();
        }

        private void onFail()
        {
            x = ox;
            y = oy;
            Invalidate();
        }

        private void doAction(int aid)
        {
            ox = x;
            oy = y;
            int oldwx = wx;
            int oldwy = wy;
            switch (aid)
            {
                case 0: y++; if (wy == y - 7) wy++; break;
                case 1: x--; if (wx == x-2) wx--; break;
                case 2: x++; if (wx == x - 7) wx++; break;
                case 3: y--; if (wy == y-2) wy--; break;
            }
            if (wx < 0) wx = 0;
            if (wy < 0) wy = 0;
            if (wx + 10 > mWidth) wx--;
            if (wy + 10 > mHeight) wy--;
            if (mazeState[x, y] == 1 || maze.IsBlock(x, y))
            {
                x = ox;
                y = oy;
                wx = oldwx;
                wy = oldwy;
            }
            last = aid;
            freshButtonState();
        }

        private void MazeWindow_MouseClick(object sender, MouseEventArgs e)
        {
            int personx = (x - wx) * 38 + 19;
            int persony = (y - wy) * 38 + 19;
            int mx = e.X - 8;
            int my = e.Y - 35;

            if (Math.Abs(mx - personx) > Math.Abs(my - persony))
            {
                doAction(mx < personx ? 1 : 2);
            }
            else
            {
                doAction(my < persony ? 3 : 0);
            }
        }

        private void pictureBoxCancel_Click(object sender, EventArgs e)
        {
            Scene.Instance.RefreshNpcState();
            Close();
        }

        private void MazeWindow_MouseMove(object sender, MouseEventArgs e)
        {
            if (e.X > 8 && e.Y > 35 && e.X < 388 && e.Y < 415)
            {
                int cellx = (e.X - 8) / 38;
                int celly = (e.Y - 35) / 38;

                if (mazeState[cellx + wx, celly + wy] == 1)
                {
                    myCursor.ChangeCursor("default");
                }
                else if (mazeState[cellx + wx, celly + wy] == 4)//使用过
                {
                    myCursor.ChangeCursor("move");
                }
                else
                {
                    string type = maze.GetItemType(cellx + wx, celly + wy);
                    if (type == "item" || type == "gold" || type == "resource")
                    {
                        myCursor.ChangeCursor("equip");
                    }
                    else if (type == "mon")
                    {
                        myCursor.ChangeCursor("fight");
                    }
                    else
                    {
                        myCursor.ChangeCursor("move");
                    }
                }
            }
            else
            {
                myCursor.ChangeCursor("default");
            }
        }

        private void MazeWindow_Paint(object sender, PaintEventArgs e)
        {
            BorderPainter.Draw(e.Graphics, "", Width, Height);

            Font font = new Font("黑体", 12*1.33f, FontStyle.Bold, GraphicsUnit.Pixel);
            e.Graphics.DrawString(ConfigDatas.ConfigData.GetNpcConfig(npcId).Name, font, Brushes.White, Width / 2 - 40, 8);
            font.Dispose();

            int xoff = 8;
            int yoff = 35;
            e.Graphics.DrawImage(mazeBack, new Rectangle(xoff, yoff, 380, 380), wx * 38, wy * 38, 380, 380, GraphicsUnit.Pixel);

            e.Graphics.DrawImage(hero, new Rectangle((x - wx) * 38+xoff + 6, (y - wy) * 38+yoff, 23, 35), heroAct * 23, last * 35, 23, 35, GraphicsUnit.Pixel);
            for (int i = wx; i < Math.Min(wx + 10, mazeState.GetLength(0) - 1); i++)
            {
                for (int j = wy; j < Math.Min(wy + 10, mazeState.GetLength(1) - 1); j++)
                {
                    Rectangle dest = new Rectangle((i - wx) * 38+xoff, (j - wy) * 38+yoff, 38, 38);
                    if (mazeState[i, j] != 4)
                    {
                        maze.DrawIcon(e.Graphics, dest, i, j, heroAct);
                    }
                }
            }
#if DEBUG
            e.Graphics.DrawString(string.Format("{0},{1}",x, y), new Font("宋体", 11), Brushes.Black, xoff, yoff);
#endif
            e.Graphics.DrawRectangle(Pens.White, xoff, yoff, 379, 379);
        }
    }
}