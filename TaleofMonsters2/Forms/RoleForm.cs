using System;
using System.Drawing;
using System.Windows.Forms;
using TaleofMonsters.Forms.Items.Core;
using TaleofMonsters.Forms.Items.Regions;
using ConfigDatas;
using TaleofMonsters.Core.Loader;
using TaleofMonsters.Datas.Others;
using TaleofMonsters.Datas.Quests;
using TaleofMonsters.Datas.User;
using TaleofMonsters.Forms.CMain;

namespace TaleofMonsters.Forms
{
    internal sealed partial class RoleForm : BasePanel
    {
        private bool show;
        private VirtualRegion vRegion;

        public RoleForm()
        {
            InitializeComponent();
            bitmapButtonClose.ImageNormal = PicLoader.Read("Button.Panel", "CloseButton1.JPG");
            bitmapButtonJob.ImageNormal = PicLoader.Read("Button.Panel", "LearnButton.JPG");

            vRegion = new VirtualRegion(this);
            vRegion.RegionClicked += new VirtualRegion.VRegionClickEventHandler(virtualRegion_RegionClicked);
            vRegion.RegionEntered += new VirtualRegion.VRegionEnteredEventHandler(virtualRegion_RegionEntered);
            vRegion.RegionLeft += new VirtualRegion.VRegionLeftEventHandler(virtualRegion_RegionLeft);
        }

        public override void Init(int width, int height)
        {
            base.Init(width, height);

            show = true;
           // bitmapButtonJob.Visible = QuestBook.HasQuest("selectjob") && UserProfile.Profile.InfoBasic.Level >= 5;
        }

        private void pictureBoxClose_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void virtualRegion_RegionEntered(int id, int x, int y, int key)
        {

        }

        private void virtualRegion_RegionLeft()
        {
        }

        private void virtualRegion_RegionClicked(int id, int x, int y, MouseButtons button)
        {
        }

        private void RoleForm_Paint(object sender, PaintEventArgs e)
        {
            BorderPainter.Draw(e.Graphics, "", Width, Height);

            Font font = new Font("黑体", 12*1.33f, FontStyle.Bold, GraphicsUnit.Pixel);
            e.Graphics.DrawString("主角", font, Brushes.White, Width / 2 - 40, 8);
            font.Dispose();

            if (!show)
                return;

            JobConfig jobConfig = ConfigDatas.ConfigData.GetJobConfig(UserProfile.InfoBasic.Job);
            Image body = PicLoader.Read("Hero", string.Format("{0}.JPG", jobConfig.JobIndex));
            e.Graphics.DrawImage(body, 15, 45, 300, 300);
            body.Dispose();

            if (vRegion != null)
                vRegion.Draw(e.Graphics);

            e.Graphics.FillRectangle(Brushes.LightSlateGray, 92, 113, 42, 42);
            Image head = PicLoader.Read("Player", string.Format("{0}.PNG", UserProfile.InfoBasic.Head));
            e.Graphics.DrawImage(head, 93, 114, 40, 40);
            head.Dispose();

            font = new Font("宋体", 11*1.33f, FontStyle.Regular, GraphicsUnit.Pixel);
            Font font2 = new Font("宋体", 10*1.33f, FontStyle.Regular, GraphicsUnit.Pixel);
            Font font3 = new Font("宋体", 9*1.33f, FontStyle.Regular, GraphicsUnit.Pixel);
            string namestr = string.Format("{0}(Lv{1}{2})", UserProfile.Profile.Name, UserProfile.InfoBasic.Level, ConfigData.GetJobConfig(UserProfile.InfoBasic.Job).Name);
            e.Graphics.DrawString(namestr, font, Brushes.White, 182, 61);

            Brush brush = new SolidBrush(Color.FromArgb(180, Color.Black));
            e.Graphics.FillRectangle(brush, 15, 300, 300, 50);
            brush.Dispose();

            string expstr = string.Format("{0}/{1}", UserProfile.InfoBasic.Exp, ExpTree.GetNextRequired(UserProfile.InfoBasic.Level));
            e.Graphics.DrawString(expstr, font2, Brushes.White, 38, 161);
            e.Graphics.FillRectangle(Brushes.DimGray, 31, 178, 80, 2);
            e.Graphics.FillRectangle(Brushes.DodgerBlue, 31, 178, Math.Min(UserProfile.InfoBasic.Exp * 79 / ExpTree.GetNextRequired(UserProfile.InfoBasic.Level) + 1, 80), 2);

            //e.Graphics.DrawString("攻击", font3, Brushes.White, 157, 92);
            //e.Graphics.DrawString("生命", font3, Brushes.White, 157+52, 92);
            //e.Graphics.DrawString("攻速", font3, Brushes.White, 157 + 52*2, 92);
            //e.Graphics.DrawString("射程", font3, Brushes.White, 157 + 52*3, 92);

            //e.Graphics.DrawString("领导", font3, Brushes.White, 157, 155);
            //e.Graphics.DrawString("力量", font3, Brushes.White, 157 + 52, 155);
            //e.Graphics.DrawString("魔力", font3, Brushes.White, 157 + 52 * 2, 155);

            font.Dispose();
            font2.Dispose();
            font3.Dispose();
        }

        private void bitmapButtonJob_Click(object sender, EventArgs e)
        {
            PanelManager.DealPanel(new SelectJobForm());
        }
    }
}