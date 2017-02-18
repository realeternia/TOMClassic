using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;

namespace SceneMaker
{
    internal class Scene
    {
        public static Scene Instance { get; set; }

        private Image mainBottom;
        private Image mainTop;
        private Image mainTopRes;
        private Image mainTopTitle;
        private Image miniBack;
        private Image backPicture;
        private List<SceneObject> sceneItems; //场景中的物件，各种npc等
        private Control parent;

        private int width, height;// 场景的宽度和高度

        public Scene(Control p, int w, int h)
        {
            parent = p;
            width = w;
            height = h;

            Init();
        }

        public void Init()
        {
            mainTop = Image.FromFile("./System/MainTop.JPG");
            mainTopRes = Image.FromFile("./System/MainTopRes.PNG");
            mainTopTitle = Image.FromFile("./System/MainTopTitle.PNG");
            mainBottom = Image.FromFile("./System/MainBottom.JPG");
            miniBack = Image.FromFile("./System/MiniBack.PNG");
        }

        public void ChangeMap(string name, bool isWarp)
        {
            sceneItems = SceneManager.RefreshSceneObjects(name, width, height - 35, isWarp ? SceneManager.SceneFreshReason.Warp : SceneManager.SceneFreshReason.Load);
            parent.Invalidate();
        }

        public void ChangeBg(string name)
        {
            if (backPicture != null)
                backPicture.Dispose();
            backPicture = Image.FromFile(name);
            parent.Invalidate();
        }


        public void Paint(Graphics g, int timeMinutes)
        {
            if (backPicture != null)
            {
                g.DrawImage(backPicture, 0, 50, width, height - 35);
            }
            g.DrawImage(mainTop, 0, 0, width, 50);
            g.DrawImage(mainTopRes, (width - 688) / 2, 4, 688, 37);//688是图片尺寸
            g.DrawImage(mainTopTitle, width - 145, 3, 115, 32);
            g.DrawImage(mainBottom, 0, height - 35, width, 35);

            Font font = new Font("微软雅黑", 12 * 1.33f, FontStyle.Bold, GraphicsUnit.Pixel);
            Font font2 = new Font("宋体", 9 * 1.33f, FontStyle.Bold, GraphicsUnit.Pixel);

            if (timeMinutes >= 960 && timeMinutes < 1080)
            {
                Brush yellow = new SolidBrush(Color.FromArgb(50, 255, 200, 0));
                g.FillRectangle(yellow, 0, 50, width, height);
                yellow.Dispose();
            }
            else if (timeMinutes >= 1080)
            {
                Brush blue = new SolidBrush(Color.FromArgb(80, 0, 0, 150));
                g.FillRectangle(blue, 0, 50, width, height);
                blue.Dispose();
            }

            int xOff = (width - 688) / 2 + 103;

            xOff = (width - 688) / 2 + 30;
            g.FillRectangle(Brushes.DimGray, xOff, 41, 630, 8);

            font.Dispose();
            font2.Dispose();

          //  g.DrawImage(miniMap, width - 160, 43, 150, 150);
            g.DrawImage(miniBack, width - 190, 38, 185, 160);

            DrawCellAndToken(g);
        }

        private void DrawCellAndToken(Graphics g)
        {
            if(sceneItems == null)
                return;
            foreach (SceneObject obj in sceneItems)
            {
                obj.Draw(g, 0);
            }
        }
    }
}