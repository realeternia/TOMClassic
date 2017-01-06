﻿using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;
using ConfigDatas;
using NarlonLib.Control;
using NarlonLib.Drawing;
using TaleofMonsters.Controler.Loader;
using TaleofMonsters.Core;
using TaleofMonsters.DataType;
using TaleofMonsters.DataType.Others;
using TaleofMonsters.DataType.User;
using TaleofMonsters.Forms.Items.Regions;
using TaleofMonsters.MainItem.Scenes.SceneObjects;

namespace TaleofMonsters.MainItem.Scenes
{
    internal class Scene
    {
        private class MovingData
        {
            public float Time { get; set; }
            public Point Source { get; set; }
            public Point Dest { get; set; }
            public int DestId { get; set; }
        }
        public static Scene Instance { get; set; }

        private Image mainBottom;
        private Image miniMap;
        private Image mainTop;
        private Image mainTopRes;
        private Image mainTopTitle;
        private Image miniBack;
        private Image backPicture;
        private int cellTar = -1;
        private List<SceneObject> sceneItems; //场景中的物件，各种npc等
        private string sceneName;
        private Control parent;

        private int width, height;// 场景的宽度和高度

        private VirtualRegion vRegion;
        private ImageToolTip tooltip = MainItem.SystemToolTip.Instance;
        private MovingData movingData = new MovingData();

        private const float ChessMoveAnimTime =0.5f;//旗子跳跃的动画世界

        public Scene(Control p, int w, int h)
        {
            parent = p;
            width = w;
            height = h;

            int xOff = (width - 688) / 2 + 103;
            vRegion = new VirtualRegion(parent);

            vRegion.AddRegion(new SubVirtualRegion(1, xOff - 60 + 82 * 6, 13, 80, 20));
            vRegion.AddRegion(new SubVirtualRegion(2, xOff-60, 13, 80, 20));
            vRegion.AddRegion(new SubVirtualRegion(3, xOff - 60+82, 13, 80, 20));
            vRegion.AddRegion(new SubVirtualRegion(4, xOff - 60 + 82*2, 13, 80, 20));
            vRegion.AddRegion(new SubVirtualRegion(5, xOff - 60 + 82*3, 13, 80, 20));
            vRegion.AddRegion(new SubVirtualRegion(6, xOff - 60 + 82*4, 13, 80, 20));
            vRegion.AddRegion(new SubVirtualRegion(7, xOff - 60 + 82*5, 13, 80, 20));
            
            vRegion.AddRegion(new SubVirtualRegion(10, 0, 0, 150, 50));
            vRegion.RegionEntered += virtualRegion_RegionEntered;
            vRegion.RegionLeft += virtualRegion_RegionLeft;
        }

        public void Init()
        {
            mainTop = PicLoader.Read("System", "MainTop.JPG");
            mainTopRes = PicLoader.Read("System", "MainTopRes.PNG");
            mainTopTitle = PicLoader.Read("System", "MainTopTitle.PNG");
            mainBottom = PicLoader.Read("System", "MainBottom.JPG");
            miniBack = PicLoader.Read("System", "MiniBack.PNG");
        }

        public void ChangeMap(int mapid, bool isWarp)
        {
            if (backPicture != null)
                backPicture.Dispose();
            SceneConfig sceneConfig = ConfigData.GetSceneConfig(mapid);
            backPicture = PicLoader.Read("Scene", string.Format("{0}.JPG", sceneConfig.Url));
            sceneName = sceneConfig.Name;

            GenerateMiniMap(mapid, sceneConfig.WindowX, sceneConfig.WindowY);

            UserProfile.InfoBasic.MapId = mapid;

            SystemMenuManager.ResetIconState(); //reset main icon state

            sceneItems = SceneManager.GetSceneObjects(UserProfile.InfoBasic.MapId, width, height - 35, isWarp);

            parent.Invalidate();
        }

        private void GenerateMiniMap(int mapid, int wx, int wy)
        {
            Image allMap = PicLoader.Read("Map", "worldmap.JPG"); //生成世界地图
            Graphics g = Graphics.FromImage(allMap);
            Font font = new Font("微软雅黑", 18*1.33f, FontStyle.Bold, GraphicsUnit.Pixel);
            foreach (var mapIconConfig in ConfigData.SceneMapIconDict.Values)
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

        public void TimeGo(float timePast)
        {
            if (movingData.Time > 0)
            {
                movingData.Time -= timePast;
                var x = movingData.Source.X/2 + movingData.Dest.X/2;
                var y = movingData.Source.Y/2 + movingData.Dest.Y/2;
                parent.Invalidate(new Rectangle(x - 150, y - 150, 300, 300));

                if (movingData.Time <= 0)
                {
                    movingData.Time = 0;

                    foreach (var sceneObject in sceneItems)
                    {
                        if (sceneObject.Id == movingData.DestId)
                            sceneObject.MoveEnd();
                    }
                }
            }
        }
        
        public void CheckMouseMove(int x, int y)
        {
            if (movingData.Time > 0) return;
            int nTemp = -1;
            foreach (var sceneObject in sceneItems)
            {
                if (sceneObject.IsMouseIn(x, y))
                    nTemp = sceneObject.Id;
            }
            if (cellTar != nTemp)
            {
                cellTar = nTemp;
                parent.Invalidate();
            }
        }

        public void CheckMouseClick()
        {
            if (cellTar == -1) return;
            if (movingData.Time > 0) return;
            SceneObject src = null;
            SceneObject dest = null;
            foreach (var sceneObject in sceneItems)
            {
                if (sceneObject.Id == cellTar)
                {
                    dest = sceneObject;
                }
                if (sceneObject.Id == UserProfile.InfoBasic.Position)
                {
                    src = sceneObject;
                }
            }

            if (dest!=null && dest.OnClick())
            {
                int drawWidth = 57 * src.Width / GameConstants.SceneTileStandardWidth;
                int drawHeight = 139 * src.Height / GameConstants.SceneTileStandardHeight;
                movingData.Source = new Point(src.X - drawWidth / 2 + src.Width / 8, src.Y - drawHeight + src.Height / 3);
                movingData.DestId = dest.Id;
                movingData.Dest = new Point(dest.X - drawWidth / 2 + dest.Width / 8, dest.Y - drawHeight + dest.Height / 3);
                movingData.Time = ChessMoveAnimTime;
                parent.Invalidate();
            }
        }


        private void virtualRegion_RegionEntered(int id, int x, int y, int key)
        {
            if (id == 10)
            {
                Image image = GetPlayerImage();
                tooltip.Show(image, parent, 0,50);
            }
            else if (id < 10)
            {
                var resName = HSTypes.I2Resource(id - 1);
                string resStr = string.Format("{0}:{1}", resName, UserProfile.Profile.InfoBag.Resource.Get((GameResourceType)(id-1)));
                Image image = DrawTool.GetImageByString(resStr, 100);
                tooltip.Show(image, parent, x, y);
            }
        }

        private void virtualRegion_RegionLeft()
        {
            tooltip.Hide(parent);
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
            g.DrawImage(head, 0, 0, 50, 50);
            head.Dispose();
            g.DrawString(UserProfile.InfoBasic.Level.ToString(), font2, Brushes.Black, 3, 3);
            g.DrawString(UserProfile.InfoBasic.Level.ToString(), font2, Brushes.White, 2, 2);
           // g.DrawString(UserProfile.ProfileName, font, Brushes.White, 50, 10);

            LinearGradientBrush b1 = new LinearGradientBrush(new Rectangle(55, 5, 100, 5), Color.LightCoral, Color.Red, LinearGradientMode.Horizontal);
            g.FillRectangle(b1, 55, 5, Math.Min(UserProfile.InfoBasic.HealthPoint, 100), 5);
            g.DrawRectangle(Pens.DarkGray, 55, 5, 100, 5);
            b1.Dispose();

            b1 = new LinearGradientBrush(new Rectangle(55, 12, 100, 5), Color.LightBlue, Color.Blue, LinearGradientMode.Horizontal);
            g.FillRectangle(b1, 55, 12, Math.Min(UserProfile.InfoBasic.MentalPoint, 100), 5);
            g.DrawRectangle(Pens.DarkGray, 55, 12, 100, 5);
            b1.Dispose();

            b1 = new LinearGradientBrush(new Rectangle(55, 19, 100, 5), Color.LightGreen, Color.Green, LinearGradientMode.Horizontal);
            g.FillRectangle(b1, 55, 19, Math.Min(UserProfile.InfoBasic.FoodPoint, 100), 5);
            g.DrawRectangle(Pens.DarkGray, 55, 19, 100, 5);
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

            DrawCellAndToken(g);
        }

        private void DrawCellAndToken(Graphics g)
        {
            SceneObject possessCell = null;
            foreach (SceneObject obj in sceneItems)
            {
                obj.Draw(g, cellTar);
                if (obj.Id == UserProfile.Profile.InfoBasic.Position)
                {
                    possessCell = obj;
                }
            }

            if (possessCell != null)
            {
                Image token = PicLoader.Read("Map", "Token.PNG");
                int drawWidth = token.Width*possessCell.Width/GameConstants.SceneTileStandardWidth;
                int drawHeight = token.Height*possessCell.Height/GameConstants.SceneTileStandardHeight;
                if (movingData.Time <= 0)
                {
                    g.DrawImage(token, possessCell.X - drawWidth/2 + possessCell.Width/8,
                        possessCell.Y - drawHeight + possessCell.Height/3, drawWidth, drawHeight);
                }
                else
                {
                    int realX = (int)(movingData.Source.X*(movingData.Time)/ChessMoveAnimTime +
                             movingData.Dest.X*(ChessMoveAnimTime - movingData.Time)/ChessMoveAnimTime);
                    int yOff = (int) (Math.Pow(realX - (movingData.Source.X + movingData.Dest.X)/2, 2)*(4*80)/
                                      Math.Pow(movingData.Source.X - movingData.Dest.X, 2) - 80);
                    var pos = new Point(realX,yOff +(int)(movingData.Source.Y*(movingData.Time)/ChessMoveAnimTime +
                                 movingData.Dest.Y*(ChessMoveAnimTime - movingData.Time)/ChessMoveAnimTime));
                    g.DrawImage(token, pos.X, pos.Y, drawWidth, drawHeight);
                }
                token.Dispose();
            }
        }

        private Image GetPlayerImage()
        {
            ControlPlus.TipImage tipData = new ControlPlus.TipImage();
            tipData.AddTextNewLine(string.Format("{0}(Lv{1})", UserProfile.ProfileName, UserProfile.InfoBasic.Level), "LightBlue", 20);
            tipData.AddLine(2);
            tipData.AddTextNewLine(string.Format("生命:{0}",UserProfile.InfoBasic.HealthPoint), "Red");
            tipData.AddTextNewLine(string.Format("精神:{0}", UserProfile.InfoBasic.MentalPoint), "LightBlue");
            tipData.AddTextNewLine(string.Format("食物:{0}", UserProfile.InfoBasic.FoodPoint), "LightGreen");
            return tipData.Image;
        }

        public void ResetScene()
        {
            sceneItems = SceneManager.GetSceneObjects(UserProfile.InfoBasic.MapId, width, height - 35, true);
            parent.Invalidate();
        }
    }
}