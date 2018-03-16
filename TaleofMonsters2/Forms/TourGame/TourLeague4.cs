using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using ConfigDatas;
using TaleofMonsters.Datas.Tournaments;
using TaleofMonsters.Datas.User;
using TaleofMonsters.Datas.User.Db;

namespace TaleofMonsters.Forms.TourGame
{
    internal partial class TourLeague4 : UserControl
    {
        private bool show;
        private DbTournamentData tourData;
        private int tid;
        private Button[] buttons;
        private int headSize = 36;
        private int gridSize = 50;

        public TourLeague4()
        {
            InitializeComponent();
        }

        public void Init(int id)
        {
            show = true;

            tid = id;
            tourData = UserProfile.InfoWorld.GetTournamentData(id);

            TournamentConfig tournamentConfig = ConfigData.GetTournamentConfig(tid);
            if (tournamentConfig.PlayerCount > 4)
            {
                headSize = 24;
                gridSize = 36;
            }
            else
            {
                headSize = 36;
                gridSize = 50;
            }
            if (buttons!=null)
            {
                foreach (Button button in buttons)
                {
                    Controls.Remove(button);
                }
            }
            buttons = new Button[tournamentConfig.MatchCount];
            for (int i = 0; i < tournamentConfig.MatchCount; i++)
            {
                Button btn = MatchManager.GetButton(i, 116, 18 + gridSize * i);
                btn.Click += new EventHandler(button1_Click);
                Controls.Add(btn);
                buttons[i] = btn;
            }

            RefreshButtons();

            Height = Math.Max(410, tournamentConfig.MatchCount * gridSize);
            Invalidate();
        }

        private void RefreshButtons()
        {
          //  Tournament tour = TournamentBook.GetTournament(tid);
          //  int date = UserProfile.Profile.time.Date;

            //for (int i = 0; i < tour.match_count; i++)
            //{
            //    buttons[i].Visible = tour.matchs[i].date == date && tourData.results[i].winner == 0;
            //}
        }

        private void button1_Click(object sender, EventArgs e)
        {
            int mid = int.Parse(((Button) sender).Tag.ToString());

            tourData.CheckMatch(mid, false);

            Invalidate();
            RefreshButtons();
        }

        private void TourLeague4_Paint(object sender, PaintEventArgs e)
        {
            if (show)
            {
                Font font = new Font("宋体", 11*1.33f, FontStyle.Regular, GraphicsUnit.Pixel);
                TournamentConfig tournamentConfig = ConfigData.GetTournamentConfig(tid);
                foreach (int mid in TournamentBook.GetTournamentMatchIds(tid))
                {
                    TournamentMatchConfig tournamentMatchConfig = ConfigData.GetTournamentMatchConfig(mid);
                    Image head = MatchManager.GetHeadImage(tourData.Pids[tournamentMatchConfig.LeftValue]);
                    if (head != null)
                    {                       
                        e.Graphics.DrawImage(head, 10 + (36 - headSize) / 2, gridSize * tournamentMatchConfig.Offset + 10 + (36 - headSize) / 2, headSize, headSize);
                    }
                    Brush nbrush = new SolidBrush(MatchManager.GetNameColor(tourData.Results[tournamentMatchConfig.Offset].Winner, tourData.Pids[tournamentMatchConfig.LeftValue]));
                    e.Graphics.DrawString(MatchManager.GetPlayerName(tourData.Pids[tournamentMatchConfig.LeftValue]), font, nbrush, 50, gridSize * tournamentMatchConfig.Offset + 25);
                    nbrush.Dispose();

                    head = MatchManager.GetHeadImage(tourData.Pids[tournamentMatchConfig.RightValue]);
                    if (head != null)
                    {
                        e.Graphics.DrawImage(head, 210 + (36 - headSize) / 2, gridSize * tournamentMatchConfig.Offset + 10 + (36 - headSize) / 2, headSize, headSize);
                    }
                    nbrush = new SolidBrush(MatchManager.GetNameColor(tourData.Results[tournamentMatchConfig.Offset].Winner, tourData.Pids[tournamentMatchConfig.RightValue]));
                    e.Graphics.DrawString(MatchManager.GetPlayerName(tourData.Pids[tournamentMatchConfig.RightValue]), font, nbrush, 250, gridSize * tournamentMatchConfig.Offset + 25);
                    nbrush.Dispose();

                    if (tourData.Results[tournamentMatchConfig.Offset].Winner != 0)
                    {
                        e.Graphics.DrawString(tourData.Results[tournamentMatchConfig.Offset].Winner == tourData.Pids[tournamentMatchConfig.LeftValue] ? "胜" : "负", font, Brushes.White, 143, gridSize * tournamentMatchConfig.Offset + 25);
                    }
                }

                List<int[]> ranks = tourData.GetRanks();
                Brush brush = new SolidBrush(Color.FromArgb(15, 15, 15));
                e.Graphics.FillRectangle(brush, 340, 53, 165, 200);
                brush.Dispose();
                e.Graphics.DrawRectangle(Pens.Gray, 340, 53, 165, tournamentConfig.PlayerCount * 50);
                e.Graphics.DrawLine(Pens.Gray, 400, 53, 400, 53 + tournamentConfig.PlayerCount * 50);
                for (int i = 0; i < tournamentConfig.PlayerCount - 1; i++)
                {
                    e.Graphics.DrawLine(Pens.Gray, 340, 103 + 50 * i, 505, 103 + 50 * i);
                }
                for (int i = 0; i < tournamentConfig.PlayerCount; i++)
                {
                    Image head = MatchManager.GetHeadImage(ranks[i][0]);
                    if (head != null)
                    {
                        e.Graphics.DrawImage(head, 350, 50 * i + 60, 36, 36);
                        e.Graphics.DrawString(string.Format("{0}战 {1}胜 {2}分", ranks[i][1], ranks[i][2], ranks[i][3]), font, Brushes.White, 410, 50 * i + 75);
                    }
                }

                font.Dispose();
            }
        }

    }
}
