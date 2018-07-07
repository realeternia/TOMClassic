using System;
using System.Drawing;
using System.Windows.Forms;
using TaleofMonsters.Forms.Items.Core;
using ConfigDatas;
using TaleofMonsters.Core;
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

        public RoleForm()
        {
            InitializeComponent();
            bitmapButtonClose.ImageNormal = PicLoader.Read("Button.Panel", "CloseButton1.JPG");
            bitmapButtonJob.ImageNormal = PicLoader.Read("Button.Panel", "JobButton.JPG");
            bitmapButtonJob.NoUseDrawNine = true;
            bitmapButtonHistory.ImageNormal = PicLoader.Read("Button.Panel", "InfoButton.JPG");
            bitmapButtonHistory.NoUseDrawNine = true;
        }

        public override void Init(int width, int height)
        {
            base.Init(width, height);
            Location = new Point(Location.X - 303/2, Location.Y); //空出右边historyform
            show = true;

            if (UserProfile.InfoDungeon.DungeonId > 0)
            {
                bitmapButtonJob.Visible = false;
                bitmapButtonHistory.Visible = false;
            }
        }

        private void pictureBoxClose_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void RoleForm_Paint(object sender, PaintEventArgs e)
        {
            BorderPainter.Draw(e.Graphics, "", Width, Height);

            Font font = new Font("黑体", 12*1.33f, FontStyle.Bold, GraphicsUnit.Pixel);
            e.Graphics.DrawString(UserProfile.Profile.Name, font, Brushes.White, Width / 2 - 40, 8);
            font.Dispose();

            if (!show)
                return;

            var job = UserProfile.InfoBasic.Job;
            if (UserProfile.InfoDungeon.DungeonId > 0)
                job = UserProfile.InfoDungeon.JobId;

            JobConfig jobConfig = ConfigDatas.ConfigData.GetJobConfig(job);
            Image body = PicLoader.Read("Hero", string.Format("{0}.JPG", jobConfig.JobIndex));
            e.Graphics.DrawImage(body, 12, 40, 305, 405);
            body.Dispose();

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
                AddFlowCenter(HSErrors.GetDescript(ErrorConfig.Indexer.JobLevelLow),"Red");
                return;
            }

            if (!QuestBook.HasQuest("selectjob"))
            {
                AddFlowCenter(HSErrors.GetDescript(ErrorConfig.Indexer.JobQuestNotFin), "Red");
                return;
            }

            var jobForm = new SelectJobForm();
            jobForm.ParentPanel = this;
            PanelManager.DealPanel(jobForm);
        }

        private void bitmapButtonHistory_Click(object sender, EventArgs e)
        {
            var historyForm = new PlayerHistoryForm();
            historyForm.ParentPanel = this;
            PanelManager.DealPanel(historyForm);
        }
    }
}