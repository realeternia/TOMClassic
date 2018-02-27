using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using ConfigDatas;
using ControlPlus;
using NarlonLib.Math;
using TaleofMonsters.Controler.Loader;
using TaleofMonsters.Core;
using TaleofMonsters.DataType.Scenes;
using TaleofMonsters.DataType.User;
using TaleofMonsters.Forms.Items.Core;
using TaleofMonsters.MainItem.Scenes;

namespace TaleofMonsters.Forms
{
    internal sealed partial class WorldMapViewForm : BasePanel
    {
        private bool showImage;
        private int baseX = 900;
        private int baseY = 250;
        private int pointerY;
        private string selectName;
        private Image worldMap; 
        private HSCursor myCursor;

        private Dictionary<int, Size> iconSizeDict = new Dictionary<int, Size>();

        public WorldMapViewForm()
        {
            InitializeComponent();
            this.bitmapButtonClose.ImageNormal = PicLoader.Read("Button.Panel", "CloseButton1.JPG");
            bitmapButtonClose.NoUseDrawNine = true;
            myCursor = new HSCursor(this);
        }

        public override void Init(int width, int height)
        {
            base.Init(width, height);
            worldMap = PicLoader.Read("Map", "worldmap.JPG");
            Graphics g = Graphics.FromImage(worldMap);
            foreach (var sceneConfig in ConfigData.SceneDict.Values)
            {
                if (sceneConfig.Icon == "")
                    continue;

                var regionConfig = ConfigData.GetSceneRegionConfig(sceneConfig.RegionId);

                Image image = PicLoader.Read("Map.MapIcon", string.Format("{0}.PNG", sceneConfig.Icon));
                iconSizeDict[sceneConfig.Id] = new Size(image.Width, image.Height);

                Rectangle destRect = new Rectangle(sceneConfig.IconX-2, sceneConfig.IconY-2, image.Width+4, image.Height+4);
                Rectangle destRect2 = new Rectangle(sceneConfig.IconX, sceneConfig.IconY, image.Width, image.Height);

                g.DrawImage(image, destRect, 0, 0, image.Width, image.Height, GraphicsUnit.Pixel, HSImageAttributes.FromColor(Color.FromName(regionConfig.IconColor)));
                g.DrawImage(image, destRect2, 0, 0, image.Width, image.Height, GraphicsUnit.Pixel, HSImageAttributes.ToGray);
                image.Dispose();

                if (sceneConfig.Id == UserProfile.InfoBasic.MapId)
                {
                    baseX = sceneConfig.IconX - 750/2 + 30;
                    baseY = sceneConfig.IconY - 500/2 + 30;
                    baseX = MathTool.Clamp(baseX, 0, worldMap.Width - 750);
                    baseY = MathTool.Clamp(baseY, 0, worldMap.Height - 500);
                }
            }
            g.Dispose();

            showImage = true;
        }

        public override void OnFrame(int tick, float timePass)
        {
            if ((tick % 10) == 0)
            {
                pointerY = (pointerY == 0 ? -12 : 0);
                Invalidate();
            }
        }

        private void WorldMapViewForm_MouseMove(object sender, MouseEventArgs e)
        {
            int truex = e.X - 15;
            int truey = e.Y - 35;
            
            if (mouseHold)
            {
                if (MathTool.GetDistance(e.Location, dragStartPos)>3)
                {
                    baseX -= e.Location.X-dragStartPos.X;
                    baseY -= e.Location.Y - dragStartPos.Y;
                    baseX = MathTool.Clamp(baseX, 0, worldMap.Width - 750);
                    baseY = MathTool.Clamp(baseY, 0, worldMap.Height - 500);
                    dragStartPos = e.Location;
                    Invalidate();
                }
                dragStartPos = e.Location;
            }
            else
            {
                dragStartPos = e.Location;

                string newSel = "";
                foreach (var mapIconConfig in ConfigData.SceneDict.Values)
                {
                    if (mapIconConfig.Icon == "")
                        continue;

                    var iconSize = iconSizeDict[mapIconConfig.Id];
                    if (truex > mapIconConfig.IconX - baseX && truey > mapIconConfig.IconY - baseY && truex < mapIconConfig.IconX - baseX + iconSize.Width && truey < mapIconConfig.IconY - baseY + iconSize.Height)
                        newSel = mapIconConfig.Icon;
                }
                if (newSel != selectName)
                {
                    selectName = newSel;
                    Invalidate();
                }
            }
        }

        private bool mouseHold;
        private Point dragStartPos;
        private void WorldMapViewForm_MouseUp(object sender, MouseEventArgs e)
        {
            mouseHold = false;
            myCursor.ChangeCursor("default");
        }

        private void WorldMapViewForm_MouseDown(object sender, MouseEventArgs e)
        {
            mouseHold = true;
            myCursor.ChangeCursor("hand"); 
            dragStartPos = e.Location;
        }

        private void WorldMapViewForm_Click(object sender, EventArgs e)
        {
            if (selectName =="")
                return;

            foreach (var mapIconConfig in ConfigData.SceneDict.Values)
            {
                if (mapIconConfig.Icon == selectName && mapIconConfig.Id != UserProfile.InfoBasic.MapId)
                {
                    if (UserProfile.InfoDungeon.DungeonId > 0) //副本中不允许外传
                        return;

                    if (MessageBoxEx2.Show("是否花10钻石立刻移动到该地区?") == DialogResult.OK)
                    {
                        if (UserProfile.InfoBag.PayDiamond(10))
                        {
                            UserProfile.InfoBasic.Position = 0;//如果是0，后面流程会随机一个位置
                            Scene.Instance.ChangeMap(mapIconConfig.Id, true);
                            Close();
                        }
                    }
                    return;
                }
            }
        }

        private void buttonClose_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void WorldMapViewForm_Paint(object sender, PaintEventArgs e)
        {
            BorderPainter.Draw(e.Graphics, "", Width, Height);

            Font font = new Font("黑体", 12*1.33f, FontStyle.Bold, GraphicsUnit.Pixel);
            e.Graphics.DrawString("世界地图", font, Brushes.White, Width / 2 - 40, 8);
            font.Dispose();

            if (!showImage)
                return;

            e.Graphics.DrawImage(worldMap, new Rectangle(15, 35, 750,500), new Rectangle(baseX, baseY, 750,500), GraphicsUnit.Pixel);
            Image tit = PicLoader.Read("Map", "title.PNG");
            e.Graphics.DrawImage(tit, 30, 45, 126, 44);
            tit.Dispose();
            foreach (var mapIconConfig in ConfigData.SceneDict.Values)
            {
                if (mapIconConfig.Icon == "")
                    continue;

                if (mapIconConfig.Icon == selectName)
                {
                    var iconSize = iconSizeDict[mapIconConfig.Id];
                    int x = mapIconConfig.IconX;
                    int y = mapIconConfig.IconY;
                    int width = iconSize.Width;
                    int height = iconSize.Height;

                    if (x > baseX && y > baseY && x + width < baseX + 750 && y + height < baseY + 500)
                    {
                        Image image = PicLoader.Read("Map.MapIcon", string.Format("{0}.PNG", mapIconConfig.Icon));
                        Rectangle destRect = new Rectangle(x - baseX + 11, y - baseY + 31, width + 8, height + 8);
                        Rectangle destRect2 = new Rectangle(x - baseX + 15, y - baseY + 35, width, height);
                        e.Graphics.DrawImage(image, destRect, 0, 0, width, height, GraphicsUnit.Pixel);
                        e.Graphics.DrawImage(image, destRect2, 0, 0, width, height, GraphicsUnit.Pixel);
                        image.Dispose();

                        image = GetPreview(mapIconConfig.Id);
                        int tx = x - baseX + width;
                        if (tx > 750 - image.Width)
                            tx -= image.Width + width;
                        e.Graphics.DrawImage(image, tx + 15, y - baseY + 35, image.Width, image.Height);
                        image.Dispose();
                    }
                }
                if (mapIconConfig.Id == UserProfile.InfoBasic.MapId)
                {
                    Image image = PicLoader.Read("Map", "arrow.PNG");
                    e.Graphics.DrawImage(image, mapIconConfig.IconX - baseX + 40, mapIconConfig.IconY - baseY - 5 + pointerY, 30, 48);
                    image.Dispose();
                }
            }
        }

        private static Image GetPreview(int id)
        {
            SceneConfig sceneConfig = ConfigData.GetSceneConfig(id);

            ControlPlus.TipImage tipData = new ControlPlus.TipImage();
            tipData.AddTextNewLine(sceneConfig.Name, "Lime", 20);
            tipData.AddTextNewLine(string.Format("地图等级: {0}", sceneConfig.Level), sceneConfig.Level>UserProfile.InfoBasic.Level?"Red": "White");
            tipData.AddTextNewLine(string.Format("区域: {0}", ConfigData.GetSceneRegionConfig(sceneConfig.RegionId).Name), "White", 20);

#if DEBUG
            var sceneInfo = SceneBook.LoadSceneFile(id, 0, 0, new Random());
            var questCount = SceneQuestBook.GetQuestCount(id);
            tipData.AddTextNewLine(string.Format("饱和度：{0:0.0}/{1}", sceneInfo.SpecialCellCount + questCount, sceneInfo.CellCount), "Green");
#endif

            return tipData.Image;
        }
    }
}