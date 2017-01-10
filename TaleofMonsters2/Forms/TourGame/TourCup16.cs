using System;
using System.Drawing;
using System.Windows.Forms;
using TaleofMonsters.DataType.User;
using TaleofMonsters.DataType.User.Db;

namespace TaleofMonsters.Forms.TourGame
{
    internal partial class TourCup16 : UserControl
    {
        private bool show;
        private DbTournamentData tourData;
        private Button[] buttons;
        private int tid;

        public TourCup16()
        {
            InitializeComponent();            
        }

        public void Init(int id)
        {
            show = true;

            tid = id;
            tourData = UserProfile.InfoWorld.GetTournamentData(id);

            if (buttons != null)
            {
                foreach (Button button in buttons)
                {
                    Controls.Remove(button);
                }
            }

            buttons = new Button[15];
            for (int i = 0; i < 8; i++)
            {
                Button btn = MatchManager.GetButton(i, 131, 72 * i + 26);
                btn.Click += new EventHandler(button1_Click);
                Controls.Add(btn);
                buttons[i] = btn;
            }
            for (int i = 0; i < 4; i++)
            {
                Button btn = MatchManager.GetButton(i+8, 249, 144 * i + 62);
                btn.Click += new EventHandler(button1_Click);
                Controls.Add(btn);
                buttons[i+8] = btn;
            }
            for (int i = 0; i < 2; i++)
            {
                Button btn = MatchManager.GetButton(i + 12, 367, 288 * i + 135);
                btn.Click += new EventHandler(button1_Click);
                Controls.Add(btn);
                buttons[i+12] = btn;
            }
            Button btn2 = MatchManager.GetButton(14, 485, 280);
            btn2.Click += new EventHandler(button1_Click);
            Controls.Add(btn2);
            buttons[14] = btn2;

            RefreshButtons();

            Invalidate();
        }

        private void RefreshButtons()
        {
       //     Tournament tour = TournamentBook.GetTournament(tid);
    //        int date = UserProfile.Profile.time.Date;

            //for (int i = 0; i < 15; i++)
            //{
            //    buttons[i].Visible = tour.matchs[i].date == date && tourData.results[i].winner == 0;
            //}
        }

        private void button1_Click(object sender, EventArgs e)
        {
            int mid = int.Parse(((Button)sender).Tag.ToString());

            tourData.CheckMatch(mid, false);

            Invalidate();
            RefreshButtons();
        }

        private bool IsPlayerLost(int pid)
        {
            foreach (MatchResult matchResult in tourData.Results)
            {
                if (matchResult.Loser==pid)
                {
                    return true;
                }
            }
            return false;
        }

        private void TourCup16_Paint(object sender, PaintEventArgs e)
        {
            if (show)
            {
                Font font = new Font("宋体", 11*1.33f, FontStyle.Regular, GraphicsUnit.Pixel);

                for (int i = 0; i < 16; i++)
                {
                    Image head = MatchManager.GetHeadImage(tourData.Pids[i]);
                    if (head != null)
                    {
                        e.Graphics.DrawImage(head, 10, 36 * i + 10, 24, 24);
                    }
                    e.Graphics.DrawString(MatchManager.GetPlayerName(tourData.Pids[i]), font, IsPlayerLost(tourData.Pids[i]) ? Brushes.DimGray : Brushes.White, 36, 36 * i + 15);
                }
                for (int i = 0; i < 8; i++)
                {
                    MatchManager.DrawCrossing(e.Graphics, 90, 72 * i + 21, 126, 72 * i + 57);
                    if (tourData.Results[i].Winner != 0)
                    {
                        Image head = MatchManager.GetHeadImage(tourData.Results[i].Winner);
                        if (head != null)
                        {
                            e.Graphics.DrawImage(head, 128, 72 * i + 25, 24, 24);
                        }
                        e.Graphics.DrawString(MatchManager.GetPlayerName(tourData.Results[i].Winner), font, IsPlayerLost(tourData.Results[i].Winner) ? Brushes.DimGray : Brushes.White, 154, 72 * i + 30);
                    }
                }
                for (int i = 0; i < 4; i++)
                {
                    MatchManager.DrawCrossing(e.Graphics, 208, 144 * i + 38, 244, 144 * i + 110);
                    if (tourData.Results[i + 8].Winner != 0)
                    {
                        Image head = MatchManager.GetHeadImage(tourData.Results[i + 8].Winner);
                        if (head != null)
                        {
                            e.Graphics.DrawImage(head, 246, 144 * i + 61, 24, 24);
                        }
                        e.Graphics.DrawString(MatchManager.GetPlayerName(tourData.Results[i + 8].Winner), font, IsPlayerLost(tourData.Results[i + 8].Winner) ? Brushes.DimGray : Brushes.White, 272, 144 * i + 66);
                    }
                }
                for (int i = 0; i < 2; i++)
                {
                    MatchManager.DrawCrossing(e.Graphics, 326, 288 * i + 74, 362, 288 * i + 219);
                    if (tourData.Results[i + 12].Winner != 0)
                    {
                        Image head = MatchManager.GetHeadImage(tourData.Results[i + 12].Winner);
                        if (head != null)
                        {
                            e.Graphics.DrawImage(head, 364, 288 * i + 134, 24, 24);
                        }
                        e.Graphics.DrawString(MatchManager.GetPlayerName(tourData.Results[i + 12].Winner), font, IsPlayerLost(tourData.Results[i + 12].Winner) ? Brushes.DimGray : Brushes.White, 390, 288 * i + 140);
                    }
                }
                MatchManager.DrawCrossing(e.Graphics, 444, 149, 480, 435);
                if (tourData.Results[14].Winner != 0)
                {
                    Image head = MatchManager.GetHeadImage(tourData.Results[14].Winner);
                    if (head != null)
                    {
                        e.Graphics.DrawImage(head, 482, 278, 24, 24);
                    }
                    e.Graphics.DrawString(MatchManager.GetPlayerName(tourData.Results[14].Winner), font, Brushes.White, 508, 284);
                }
            }
        }
    }
}
