using System;
using System.Drawing;
using System.Windows.Forms;
using ConfigDatas;
using ControlPlus;
using TaleofMonsters.Controler.Loader;
using TaleofMonsters.Core;
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

        public WorldMapViewForm()
        {
            InitializeComponent();
            this.bitmapButtonClose.ImageNormal = PicLoader.Read("ButtonBitmap", "CloseButton1.JPG");
            bitmapButtonClose.NoUseDrawNine = true;
            this.bitmapButtonLeft.ImageNormal = PicLoader.Read("ButtonBitmap", "PreButton.JPG");
            bitmapButtonLeft.NoUseDrawNine = true;
            this.bitmapButtonRight.ImageNormal = PicLoader.Read("ButtonBitmap", "NextButton.JPG");
            bitmapButtonRight.NoUseDrawNine = true;
            this.bitmapButtonUp.ImageNormal = PicLoader.Read("ButtonBitmap", "UpButton.JPG");
            bitmapButtonUp.NoUseDrawNine = true;
            this.bitmapButtonDown.ImageNormal = PicLoader.Read("ButtonBitmap", "DownButton.JPG");
            bitmapButtonDown.NoUseDrawNine = true;
        }

        internal override void Init(int width, int height)
        {
            base.Init(width, height);
            worldMap = PicLoader.Read("Map", "worldmap.JPG");
            Graphics g = Graphics.FromImage(worldMap);
            foreach (SceneMapIconConfig mapIconConfig in ConfigData.SceneMapIconDict.Values)
            {
                if (mapIconConfig.Level > UserProfile.InfoBasic.Level)
                    continue;

                if (mapIconConfig.Icon=="")
                    continue;

                Image image = PicLoader.Read("MapIcon", string.Format("{0}.PNG", mapIconConfig.Icon));
                Rectangle destRect = new Rectangle(mapIconConfig.IconX, mapIconConfig.IconY, mapIconConfig.IconWidth, mapIconConfig.IconHeight);
                g.DrawImage(image, destRect, 0, 0, mapIconConfig.IconWidth, mapIconConfig.IconHeight, GraphicsUnit.Pixel, HSImageAttributes.ToGray);
                image.Dispose();
            }
            g.Dispose();

            showImage = true;
        }

        internal override void OnFrame(int tick)
        {
            if ((tick % 10) == 0)
            {
                pointerY = (pointerY == 0 ? -12 : 0);
                Invalidate();
            }
        }

        private void UpdateButtonState()
        {
            bitmapButtonLeft.Enabled = baseX > 0;
            bitmapButtonRight.Enabled = baseX < worldMap.Width - 750;
            bitmapButtonUp.Enabled = baseY > 0;
            bitmapButtonDown.Enabled = baseY < worldMap.Height - 500;
        }

        private void bitmapButtonLeft_Click(object sender, EventArgs e)
        {
            baseX -= 50;
            if (baseX < 0) baseX = 0;
            UpdateButtonState();
            Invalidate();
        }

        private void bitmapButtonUp_Click(object sender, EventArgs e)
        {
            baseY -= 50;
            if (baseY < 0) baseY = 0;
            UpdateButtonState();
            Invalidate();
        }

        private void bitmapButtonRight_Click(object sender, EventArgs e)
        {
            baseX += 50;
            if (baseX > worldMap.Width - 750) baseX = worldMap.Width - 750;
            UpdateButtonState();
            Invalidate();
        }

        private void bitmapButtonDown_Click(object sender, EventArgs e)
        {
            baseY += 50;
            if (baseY > worldMap.Height - 500) baseY = worldMap.Height - 500;
            UpdateButtonState();
            Invalidate();
        }

        private void WorldMapViewForm_MouseMove(object sender, MouseEventArgs e)
        {
            int truex = e.X - 15;
            int truey = e.Y - 35;

            string newSel = "";
            foreach (SceneMapIconConfig mapIconConfig in ConfigData.SceneMapIconDict.Values)
            {
                if (mapIconConfig.Level > UserProfile.InfoBasic.Level)
                {
                    continue;
                }

                if (truex > mapIconConfig.IconX - baseX && truey > mapIconConfig.IconY - baseY && truex < mapIconConfig.IconX - baseX + mapIconConfig.IconWidth && truey < mapIconConfig.IconY - baseY + mapIconConfig.IconHeight)
                {
                    newSel = mapIconConfig.Icon;
                }
            }
            if (newSel != selectName)
            {
                selectName = newSel;
                Invalidate();
            }
        }

        private void WorldMapViewForm_Click(object sender, EventArgs e)
        {
            foreach (SceneMapIconConfig mapIconConfig in ConfigData.SceneMapIconDict.Values)
            {
                if (mapIconConfig.Icon == selectName && mapIconConfig.Id != UserProfile.InfoBasic.MapId)
                {
                    if (UserProfile.InfoBasic.Level < mapIconConfig.Level)
                    {
                        return;
                    }

                    if (MessageBoxEx2.Show("是否花10钻石立刻移动到该地区?") == DialogResult.OK)
                    {
                        if (UserProfile.InfoBag.PayDiamond(10))
                        {
                            Scene.Instance.ChangeMap(mapIconConfig.Id);
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
            foreach (SceneMapIconConfig mapIconConfig in ConfigData.SceneMapIconDict.Values)
            {
                if (mapIconConfig.Icon == selectName)
                {
                    int x = mapIconConfig.IconX;
                    int y = mapIconConfig.IconY;
                    int width = mapIconConfig.IconWidth;
                    int height = mapIconConfig.IconHeight;

                    if (x > baseX && y > baseY && x + width < baseX + 750 && y + height < baseY + 500)
                    {
                        Image image = PicLoader.Read("MapIcon", string.Format("{0}.PNG", mapIconConfig.Icon));
                        Rectangle destRect = new Rectangle(x - baseX + 11, y - baseY + 31, width + 8, height + 8);
                        Rectangle destRect2 = new Rectangle(x - baseX + 15, y - baseY + 35, width, height);
                        e.Graphics.DrawImage(image, destRect, 0, 0, width, height, GraphicsUnit.Pixel);
                        e.Graphics.DrawImage(image, destRect2, 0, 0, width, height, GraphicsUnit.Pixel);
                        image.Dispose();

                        image = SceneManager.GetPreview(mapIconConfig.Id);
                        int tx = x - baseX + width;
                        if (tx > 750 - image.Width)
                        {
                            tx -= image.Width + width;
                        }
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
    }
}