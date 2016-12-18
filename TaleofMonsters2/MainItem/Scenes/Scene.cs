using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;
using ConfigDatas;
using TaleofMonsters.Controler.Loader;
using TaleofMonsters.DataType.Others;
using TaleofMonsters.DataType.User;
using TaleofMonsters.MainItem.Scenes.SceneObjects;

namespace TaleofMonsters.MainItem.Scenes
{
    internal class Scene
    {
        public static Scene Instance { get; set; }

        private Image mainBottom;
        private Image miniMap;
        private Image mainTop;
        private Image mainTopRes;
        private Image mainTopTitle;
        private Image miniBack;
        private Image backPicture;
        private int npcTar = -1;
        private List<SceneObject> sceneItems; //场景中的物件，各种npc等
        private string sceneName;
        private Control parent;

        private int width, height;// 场景的宽度和高度

        public Scene(Control p, int w, int h)
        {
            parent = p;
            width = w;
            height = h;
        }

        public void Init()
        {
            mainTop = PicLoader.Read("System", "MainTop.JPG");
            mainTopRes = PicLoader.Read("System", "MainTopRes.PNG");
            mainTopTitle = PicLoader.Read("System", "MainTopTitle.PNG");
            mainBottom = PicLoader.Read("System", "MainBottom.JPG");
            miniBack = PicLoader.Read("System", "MiniBack.PNG");
        }

        public void ChangeMap(int mapid)
        {
            if (backPicture != null)
                backPicture.Dispose();
            SceneConfig sceneConfig = ConfigData.GetSceneConfig(mapid);
            backPicture = PicLoader.Read("Scene", string.Format("{0}.JPG", sceneConfig.Url));
            sceneName = sceneConfig.Name;

            GenerateMiniMap(mapid, sceneConfig.WindowX, sceneConfig.WindowY);

            UserProfile.InfoBasic.MapId = mapid;

            SystemMenuManager.ResetIconState(); //reset main icon state

            sceneItems = SceneManager.GetSceneObjects(UserProfile.InfoBasic.MapId, width, height - 35);

            parent.Invalidate();
        }

        private void GenerateMiniMap(int mapid, int wx, int wy)
        {
            Image allMap = PicLoader.Read("Map", "worldmap.JPG"); //生成世界地图
            Graphics g = Graphics.FromImage(allMap);
            Font font = new Font("微软雅黑", 18*1.33f, FontStyle.Bold, GraphicsUnit.Pixel);
            foreach (SceneMapIconConfig mapIconConfig in ConfigData.SceneMapIconDict.Values)
            {
                if (mapIconConfig.IconX < wx || mapIconConfig.IconX > wx + 300 || mapIconConfig.IconY < wy || mapIconConfig.IconY > wy + 300)
                    continue;
                string tname = ConfigData.GetSceneConfig(mapIconConfig.Id).Name;
                g.DrawString(tname, font, Brushes.Black, mapIconConfig.IconX + 2, mapIconConfig.IconY + 21);
                g.DrawString(tname, font, mapIconConfig.Id == mapid ? Brushes.Lime : Brushes.White, mapIconConfig.IconX, mapIconConfig.IconY + 20);
            }
            font.Dispose();
            g.Dispose();

            if (miniMap != null)
            {
                miniMap.Dispose();
            }
            miniMap = new Bitmap(150, 150); //绘制小地图
            g = Graphics.FromImage(miniMap);
            Rectangle destRect = new Rectangle(0, 0, 150, 150);
            g.DrawImage(allMap, destRect, wx, wy, 300, 300, GraphicsUnit.Pixel);
            g.Dispose();
            allMap.Dispose();
        }

        public void TimeGo(int timeMinutes)
        {

        }
        
        public void CheckMouseMove(int x, int y)
        {
            int nTemp = -1;
            foreach (var sceneObject in sceneItems)
            {
                if (sceneObject.IsMouseIn(x, y))
                    nTemp = sceneObject.Id;
            }
            if (npcTar != nTemp)
            {
                npcTar = nTemp;
                parent.Invalidate();
            }
        }

        public void CheckMouseClick()
        {
            if (npcTar == -1) return;
            foreach (var sceneObject in sceneItems)
            {
                if (sceneObject.Id == npcTar)
                    if(sceneObject.OnClick())
                        parent.Invalidate();
            }
        }

        public void Paint(Graphics g, int timeMinutes)
        {
            g.DrawImage(backPicture, 0, 50, width, height-35);
            g.DrawImage(mainTop, 0, 0, width, 50);
            g.DrawImage(mainTopRes, (width-688)/2, 4, 688, 37);//688是图片尺寸
            g.DrawImage(mainTopTitle, width-145, 3, 115, 32);
            g.DrawImage(mainBottom, 0, height-35, width, 35);


            if (UserProfile.Profile == null || UserProfile.InfoBasic.MapId == 0)
            {
                return;
            }

            Font font = new Font("微软雅黑", 12*1.33f, FontStyle.Bold, GraphicsUnit.Pixel);
            Font font2 = new Font("宋体", 9*1.33f, FontStyle.Bold, GraphicsUnit.Pixel);

            Image head = PicLoader.Read("Player", string.Format("{0}.PNG", UserProfile.InfoBasic.Face));
            g.DrawImage(head, 5, 5, 40, 40);
            head.Dispose();
            g.DrawString(UserProfile.InfoBasic.Level.ToString(), font2, Brushes.Black, 3, 3);
            g.DrawString(UserProfile.InfoBasic.Level.ToString(), font2, Brushes.White, 2, 2);
            g.DrawString(UserProfile.ProfileName, font, Brushes.White, 50, 10);

            LinearGradientBrush b1 = new LinearGradientBrush(new Rectangle(50, 30, 100, 5), Color.LawnGreen, Color.LightSalmon, LinearGradientMode.Horizontal);
            g.FillRectangle(b1, 50, 30, Math.Min(UserProfile.InfoBasic.Ap, 100), 5);
            g.DrawRectangle(Pens.Navy, 50, 30, 100, 5);
            b1.Dispose();
            
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

            int len = (int)g.MeasureString(sceneName, font).Width;
            g.DrawString(sceneName, font, Brushes.White, new PointF(width - 85 - len / 2, 8));

            int xOff = (width - 688)/2 + 103;
            g.DrawString(UserProfile.InfoBag.Resource.Lumber.ToString(), font, Brushes.White, new PointF(xOff, 13));
            g.DrawString(UserProfile.InfoBag.Resource.Stone.ToString(), font, Brushes.White, new PointF(xOff+82, 13));
            g.DrawString(UserProfile.InfoBag.Resource.Mercury.ToString(), font, Brushes.White, new PointF(xOff+82*2, 13));
            g.DrawString(UserProfile.InfoBag.Resource.Carbuncle.ToString(), font, Brushes.White, new PointF(xOff + 82 * 3, 13));
            g.DrawString(UserProfile.InfoBag.Resource.Sulfur.ToString(), font, Brushes.White, new PointF(xOff + 82 * 4, 13));
            g.DrawString(UserProfile.InfoBag.Resource.Gem.ToString(), font, Brushes.White, new PointF(xOff + 82 * 5, 13));
            g.DrawString(UserProfile.InfoBag.Resource.Gold.ToString(), font, Brushes.White, new PointF(xOff + 82 * 6, 13));

            xOff = (width - 688) / 2 + 30;
            g.FillRectangle(Brushes.DimGray, xOff, 41, 630, 8);
            b1 = new LinearGradientBrush(new Rectangle(xOff, 41, 630, 8), Color.White, Color.Gray, LinearGradientMode.Vertical);
            g.FillRectangle(b1, xOff, 41, Math.Min(UserProfile.InfoBasic.Exp*630/ExpTree.GetNextRequired(UserProfile.InfoBasic.Level), 630), 8);
            g.DrawRectangle(new Pen(Brushes.Black, 2), xOff, 41, 630, 8);
            b1.Dispose();

            font.Dispose();
            font2.Dispose();

            g.DrawImage(miniMap, width-160, 43, 150, 150);
            g.DrawImage(miniBack, width - 190, 38, 185, 160);

            //画NPC
            foreach (SceneObject obj in sceneItems)
            {
                obj.Draw(g, npcTar);
            }
        }

        public void RefreshNpcState()
        {

        }
    }
}
