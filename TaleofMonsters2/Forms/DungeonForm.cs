using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using ConfigDatas;
using ControlPlus;
using TaleofMonsters.Core;
using TaleofMonsters.Core.Loader;
using TaleofMonsters.Datas.Scenes;
using TaleofMonsters.Datas.User;
using TaleofMonsters.Forms.CMain;
using TaleofMonsters.Forms.CMain.Scenes;
using TaleofMonsters.Forms.Items.Core;
using TaleofMonsters.Forms.Items.Regions;

namespace TaleofMonsters.Forms
{
    internal partial class DungeonForm : BasePanel
    {
        private Image backImage;
        private ImageToolTip tooltip = SystemToolTip.Instance;
        private VirtualRegion vRegion;
        private List<int> gismoList;
        private int gismoGet;
        private string title = "";
        private int jobId = 0;

        public int DungeonId { get; set; }

        public DungeonForm()
        {
            InitializeComponent();
            this.bitmapButtonClose.ImageNormal = PicLoader.Read("Button.Panel", "CloseButton1.JPG");
            this.bitmapButtonC1.ImageNormal = PicLoader.Read("Button.Panel", "ButtonBack2.PNG");
            bitmapButtonC1.Font = new Font("宋体", 8 * 1.33f, FontStyle.Regular, GraphicsUnit.Pixel);
            bitmapButtonC1.ForeColor = Color.White;
            bitmapButtonC1.IconImage = TaleofMonsters.Core.HSIcons.GetIconsByEName("hatt5");
            bitmapButtonC1.IconSize = new Size(16, 16);
            bitmapButtonC1.IconXY = new Point(4, 5);
            bitmapButtonC1.TextOffX = 8;
            DoubleBuffered = true;
        }

        public override void Init(int width, int height)
        {
            base.Init(width, height);

            var dungeonConfig = ConfigData.GetDungeonConfig(DungeonId);
            title = dungeonConfig.Name;
            colorLabel1.Text = dungeonConfig.Des;
            gismoList = DungeonBook.GetGismoListByDungeon(DungeonId);
            radioButton1.Text = ConfigData.GetJobConfig(dungeonConfig.Jobs[0]).Name;
            radioButton2.Text = ConfigData.GetJobConfig(dungeonConfig.Jobs[1]).Name;
            radioButton3.Text = ConfigData.GetJobConfig(dungeonConfig.Jobs[2]).Name;

            vRegion = new VirtualRegion(this);

            int xOff = 13;
            int yOff = 107;
            for (int i = 0; i < gismoList.Count; i++)
            {
                var targetItem = gismoList[i];
                var region = new PictureRegion(i, 52*(i%6)+ xOff+5, 52 * (i / 6) + yOff+5, 48, 48, PictureRegionCellType.Gismo, targetItem);
                vRegion.AddRegion(region);

                if (!UserProfile.Profile.InfoGismo.GetGismo(gismoList[i]))
                    region.Enabled = false;
                else
                    gismoGet++;
            }

            backImage = PicLoader.Read("Dungeon", string.Format("{0}.JPG", dungeonConfig.BgImage));
            vRegion.RegionEntered += new VirtualRegion.VRegionEnteredEventHandler(virtualRegion_RegionEntered);
            vRegion.RegionLeft += new VirtualRegion.VRegionLeftEventHandler(virtualRegion_RegionLeft);

            radioButton1.Checked = true;
        }

        public override void OnFrame(int tick, float timePass)
        {
            base.OnFrame(tick, timePass);
        }

        private void bitmapButtonC1_Click(object sender, EventArgs e)
        {
            UserProfile.InfoCard.SelectDungeonDeck(DungeonId);
            UserProfile.InfoDungeon.JobId = jobId;
            Scene.Instance.EnterDungeon(DungeonId);
            Close();
        }

        private void virtualRegion_RegionEntered(int id, int x, int y, int key)
        {
            var region = vRegion.GetRegion(id);
            if (region != null)
                region.ShowTip(tooltip, Parent, x + Location.X, y + Location.Y);
        }

        private void virtualRegion_RegionLeft()
        {
            tooltip.Hide(this);
        }

        private void DungeonForm_Paint(object sender, PaintEventArgs e)
        {
            BorderPainter.Draw(e.Graphics, "", Width, Height);

            Font font = new Font("黑体", 12 * 1.33f, FontStyle.Bold, GraphicsUnit.Pixel);
            e.Graphics.DrawString(title, font, Brushes.White, Width / 2 - 40, 8);
            font.Dispose();

            int xOff = 13;
            int yOff = 107;
            e.Graphics.DrawImage(backImage, xOff, yOff, 324, 244);

            font = new Font("黑体", 12 * 1.33f, FontStyle.Regular, GraphicsUnit.Pixel);
            e.Graphics.DrawString(string.Format("进度：{0}/{1}",gismoGet,gismoList.Count), font, Brushes.White, xOff + 10, yOff + 220);
            font.Dispose();

            vRegion.Draw(e.Graphics);

            if (jobId > 0)
            {
                JobConfig jobConfig = ConfigDatas.ConfigData.GetJobConfig(jobId);
                Image body = PicLoader.Read("Hero", string.Format("{0}.JPG", jobConfig.JobIndex));
                var destR = new Rectangle(340, 44, 100, 310);
                e.Graphics.DrawImage(body, destR, body.Width/2 - 60, 10, 120, body.Height - 10, GraphicsUnit.Pixel);
                body.Dispose();
            }
        }

        private void bitmapButtonClose_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void radioButton1_CheckedChanged(object sender, EventArgs e)
        {
            var dungeonConfig = ConfigData.GetDungeonConfig(DungeonId);
            if (radioButton1.Checked)
                jobId = dungeonConfig.Jobs[0];
            else if (radioButton2.Checked)
                jobId = dungeonConfig.Jobs[1];
            else if (radioButton3.Checked)
                jobId = dungeonConfig.Jobs[2];
            SoundManager.Play("System", "Thunder.mp3");
            Invalidate();
        }
    }
}
