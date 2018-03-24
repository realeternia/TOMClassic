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
            bitmapButtonJob.ImageNormal = PicLoader.Read("Button.Panel", "JobButton.JPG");
            bitmapButtonJob.NoUseDrawNine = true;
            bitmapButtonHistory.ImageNormal = PicLoader.Read("Button.Panel", "InfoButton.JPG");
            bitmapButtonHistory.NoUseDrawNine = true;

            vRegion = new VirtualRegion(this);
            vRegion.RegionClicked += new VirtualRegion.VRegionClickEventHandler(virtualRegion_RegionClicked);
            vRegion.RegionEntered += new VirtualRegion.VRegionEnteredEventHandler(virtualRegion_RegionEntered);
            vRegion.RegionLeft += new VirtualRegion.VRegionLeftEventHandler(virtualRegion_RegionLeft);
        }

        public override void Init(int width, int height)
        {
            base.Init(width, height);

            show = true;
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
            e.Graphics.DrawString(UserProfile.Profile.Name, font, Brushes.White, Width / 2 - 40, 8);
            font.Dispose();

            if (!show)
                return;

            JobConfig jobConfig = ConfigDatas.ConfigData.GetJobConfig(UserProfile.InfoBasic.Job);
            Image body = PicLoader.Read("Hero", string.Format("{0}.JPG", jobConfig.JobIndex));
            e.Graphics.DrawImage(body, 12, 40, 305, 405);
            body.Dispose();

            if (vRegion != null)
                vRegion.Draw(e.Graphics);

            e.Graphics.FillRectangle(Brushes.LightSlateGray, 25-1, 55-1, 52, 52);
            Image head = PicLoader.Read("Player", string.Format("{0}.PNG", UserProfile.InfoBasic.Head));
            e.Graphics.DrawImage(head, 25, 55, 50, 50);
            head.Dispose();

            font = new Font("宋体", 11*1.33f, FontStyle.Regular, GraphicsUnit.Pixel);
            Font font2 = new Font("宋体", 10*1.33f, FontStyle.Regular, GraphicsUnit.Pixel);

            Brush brush = new SolidBrush(Color.FromArgb(220, Color.Black));
            e.Graphics.FillRectangle(brush, 12, 300, 305, 105);
            brush.Dispose();

            string namestr = string.Format("Lv {0}", UserProfile.InfoBasic.Level);
            e.Graphics.DrawString(namestr, font, Brushes.White, 20, 305);

            string expstr = string.Format("{0}/{1}", UserProfile.InfoBasic.Exp, ExpTree.GetNextRequired(UserProfile.InfoBasic.Level));
            e.Graphics.DrawString(expstr, font2, Brushes.White, 130, 300);
            e.Graphics.FillRectangle(Brushes.DimGray, 80, 314, 180, 4);
            e.Graphics.FillRectangle(Brushes.DodgerBlue, 80, 314, Math.Min(UserProfile.InfoBasic.Exp * 179 / ExpTree.GetNextRequired(UserProfile.InfoBasic.Level) + 1, 180), 4);

            e.Graphics.DrawString("职业", font2, Brushes.White, 20, 325);
            e.Graphics.DrawString(jobConfig.Name, font2, Brushes.White, 80, 325);
            e.Graphics.DrawString("领导", font2, Brushes.White, 20, 345);
            e.Graphics.DrawString(jobConfig.EnergyRate[0].ToString(), font2, Brushes.Yellow, 80, 345);
            e.Graphics.DrawString("力量", font2, Brushes.White, 20+160, 345);
            e.Graphics.DrawString(jobConfig.EnergyRate[1].ToString(), font2, Brushes.Red, 80+ 160, 345);
            e.Graphics.DrawString("魔力", font2, Brushes.White, 20, 365);
            e.Graphics.DrawString(jobConfig.EnergyRate[2].ToString(), font2, Brushes.CornflowerBlue, 80, 365);
            e.Graphics.DrawString("技能", font2, Brushes.White, 20, 385);
            if(jobConfig.SkillId > 0)
                e.Graphics.DrawString(ConfigData.GetSkillConfig(jobConfig.SkillId).Name, font2, Brushes.GreenYellow, 80, 385);

            font.Dispose();
            font2.Dispose();
        }

        private void bitmapButtonJob_Click(object sender, EventArgs e)
        {
            if (UserProfile.Profile.InfoBasic.Level < 5)
            {
                AddFlowCenter("等级5开启转职系统","Red");
                return;
            }

            if (!QuestBook.HasQuest("selectjob"))
            {
                AddFlowCenter("完成任务【开始】开启转职系统", "Red");
                return;
            }

            PanelManager.DealPanel(new SelectJobForm());
        }

        private void bitmapButtonHistory_Click(object sender, EventArgs e)
        {
            PanelManager.DealPanel(new UserForm());
        }
    }
}