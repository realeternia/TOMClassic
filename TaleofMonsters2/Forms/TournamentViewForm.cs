using System;
using System.Drawing;
using System.Windows.Forms;
using ConfigDatas;
using NarlonLib.Control;
using TaleofMonsters.Core;
using TaleofMonsters.Core.Loader;
using TaleofMonsters.Datas;
using TaleofMonsters.Datas.Tournaments;
using TaleofMonsters.Datas.User;
using TaleofMonsters.Forms.CMain;
using TaleofMonsters.Forms.Items.Core;

namespace TaleofMonsters.Forms
{
    internal sealed partial class TournamentViewForm : BasePanel
    {
        private int tid;
        private ImageToolTip tooltip = SystemToolTip.Instance;

        public TournamentViewForm()
        {
            InitializeComponent();
            this.bitmapButtonClose.ImageNormal = PicLoader.Read("Button.Panel", "CloseButton1.JPG");
        }

        private void TournamentViewForm_Load(object sender, EventArgs e)
        {
            listViewMatchs.Invalidate();
        }

        public override void Init(int width, int height)
        {
            base.Init(width, height);
            ListViewItem lvm1 = new ListViewItem("总积分榜");
            lvm1.Tag = "0";
            lvm1.ForeColor = Color.Orange;
            listViewMatchs.Items.Add(lvm1);
            int[] tournamentIds = new int[ConfigData.TournamentDict.Count];
            int index = 0;
            foreach (int key in ConfigData.TournamentDict.Keys)
            {
                tournamentIds[index++] = key;
            }
            Array.Sort(tournamentIds, new CompareTournamentByADay());

            foreach (int tournamentId in tournamentIds)
            {
                TournamentConfig tournamentConfig = ConfigData.GetTournamentConfig(tournamentId);

                ListViewItem lvm = new ListViewItem(tournamentConfig.Name);
                lvm.Tag = tournamentConfig.Id.ToString();
               // Tournament tour = TournamentBook.GetTournament(tournament.id);
                //if (tour.apply_date == UserProfile.Profile.time.Date)
                //{
                //    lvm.ForeColor = Color.Red;
                //    lvm.Text = lvm.Text + @"(报名中)";
                //}
                //else if (tour.begin_date <= UserProfile.Profile.time.Date && tour.end_date >= UserProfile.Profile.time.Date)
                //{
                //    lvm.ForeColor = Color.Lime;
                //    lvm.Text = lvm.Text + @"(比赛中)";
                //}
                listViewMatchs.Items.Add(lvm);
            }
            listViewMatchs.SelectedIndices.Add(0);
        }

        private void listViewMethods_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (listViewMatchs.SelectedIndices.Count > 0)
            {
                tid = int.Parse(listViewMatchs.SelectedItems[0].Tag.ToString());
                if (tid == 0)
                {
                    viewStack1.SelectedIndex = 3;
                    tourRankList1.Init();
                    buttonEngage.Visible = false;
                    pictureBox1.Visible = false;
                }
                else
                {
                    TournamentConfig tournamentConfig = ConfigData.GetTournamentConfig(tid);
                    if (tournamentConfig.Type == (int)TournamentTypes.Cup8)
                    {
                        viewStack1.SelectedIndex = 0;
                        tourCup81.Init(tid);
                    }
                    else if (tournamentConfig.Type == (int)TournamentTypes.League4)
                    {
                        viewStack1.SelectedIndex = 1;
                        tourLeague41.Init(tid);
                    }
                    else if (tournamentConfig.Type == (int)TournamentTypes.Cup16)
                    {
                        viewStack1.SelectedIndex = 2;
                        tourCup161.Init(tid);
                    }
                    else
                    {
                        return;
                    }
                    //if (tour.apply_date != UserProfile.Profile.time.Date)
                    //    buttonEngage.Visible = false;
                    //else if (UserProfile.MemData.GetTournamentData(tid).engage)
                    //    buttonEngage.Visible = false;
                    //else if (UserProfile.Profile.level < tour.min_level || UserProfile.Profile.level > tour.max_level)
                    //    buttonEngage.Visible = false;
                    //else
                    //    buttonEngage.Visible = true;
                    pictureBox1.Image = HSIcons.GetIconsByEName(tournamentConfig.Icon);
                    pictureBox1.Visible = true;
                }
                Invalidate();
            }
        }

        private void buttonEngage_Click(object sender, EventArgs e)
        {
            TournamentConfig tournamentConfig = ConfigData.GetTournamentConfig(tid);
            if (UserProfile.InfoBasic.Level < tournamentConfig.MinLevel || UserProfile.InfoBasic.Level > tournamentConfig.MaxLevel)
                 return;
            UserProfile.InfoWorld.GetTournamentData(tid).Engage = true;
            buttonEngage.Visible = false;
            Invalidate();
        }

        private void pictureBox1_MouseEnter(object sender, EventArgs e)
        {
            Image img = TournamentBook.GetPreview(tid);
            tooltip.Show(img, sender as Control, 24, 0);
        }

        private void pictureBox1_MouseLeave(object sender, EventArgs e)
        {
            tooltip.Hide(sender as Control);
        }

        private void buttonClose_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void MergeWeaponForm_Paint(object sender, PaintEventArgs e)
        {
            BorderPainter.Draw(e.Graphics, "", Width, Height);

            Font font = new Font("黑体", 12*1.33f, FontStyle.Bold, GraphicsUnit.Pixel);
            e.Graphics.DrawString("赛事锦标", font, Brushes.White, Width / 2 - 40, 8);
            font.Dispose();

            font = new Font("宋体", 11*1.33f, FontStyle.Bold, GraphicsUnit.Pixel);
            if (tid == 0)
            {
                e.Graphics.DrawString("排名", font, Brushes.LightCyan, 215, 50);
                e.Graphics.DrawString("名字", font, Brushes.LightCyan, 325, 50);
                e.Graphics.DrawString("等级", font, Brushes.LightCyan, 400, 50);
                e.Graphics.DrawString("职业", font, Brushes.LightCyan, 450, 50);
                e.Graphics.DrawString("积分", font, Brushes.LightCyan, 560, 50);
            }
            else
            {
                //Tournament tour = TournamentBook.GetTournament(tid);
                //if (tour.begin_date <= UserProfile.Profile.time.Date && tour.end_date >= UserProfile.Profile.time.Date)
                //{
                //    e.Graphics.DrawString("比赛中", font, Brushes.Gold, 250, 50);
                //}
                //else if (tour.apply_date != UserProfile.Profile.time.Date)
                //{
                //    e.Graphics.DrawString("报名时间", font, Brushes.LightGray, 200, 50);
                //    e.Graphics.DrawString(HSTime.GetByDate(tour.apply_date).ToShortString(), font, Brushes.LightGreen, 266, 50);
                //}
                //else if (UserProfile.MemData.GetTournamentData(tid).engage)
                //{
                //    e.Graphics.DrawString("已报名", font, Brushes.Lime, 250, 50);
                //}
                //else if (UserProfile.Profile.level < tour.min_level || UserProfile.Profile.level > tour.max_level)
                //{
                //    e.Graphics.DrawString("等级限制", font, Brushes.Red, 250, 50);
                //}
            }
            font.Dispose();
            e.Graphics.DrawLine(Pens.White, 200, 70, 700,70);
        }


    }
}