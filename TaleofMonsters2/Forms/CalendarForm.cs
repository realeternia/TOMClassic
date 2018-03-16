using System;
using System.Drawing;
using System.Windows.Forms;
using TaleofMonsters.Core;
using TaleofMonsters.Core.Loader;
using TaleofMonsters.Datas.Tournaments;
using TaleofMonsters.Forms.Items.Core;

namespace TaleofMonsters.Forms
{
    internal sealed partial class CalendarForm : BasePanel
    {
        private bool showImage;
        private int nowSeason;

        public CalendarForm()
        {
            InitializeComponent();
            this.bitmapButtonClose.ImageNormal = PicLoader.Read("Button.Panel", "CloseButton1.JPG");
            radioButton1.Image = HSIcons.GetIconsByEName("mth1");
            radioButton2.Image = HSIcons.GetIconsByEName("mth2");
            radioButton3.Image = HSIcons.GetIconsByEName("mth3");
            radioButton4.Image = HSIcons.GetIconsByEName("mth4");
        }

        public override void Init(int width, int height)
        {
            base.Init(width, height);
            //switch(UserProfile.Profile.time.Season)
            //{
            //    case 0: radioButton1.Checked = true; break;
            //    case 1: radioButton2.Checked = true; break;
            //    case 2: radioButton3.Checked = true; break;
            //    case 3: radioButton4.Checked = true; break;
            //}
            //nowSeason = UserProfile.Profile.time.Season;

            showImage = true;
        }

        private void buttonMonth_Click(object sender, EventArgs e)
        {
            bitmapButtonClose.Focus();
            nowSeason = int.Parse((sender as RadioButton).Tag.ToString());
            Invalidate();
        }

        private void buttonClose_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void CalendarForm_Paint(object sender, PaintEventArgs e)
        {
            BorderPainter.Draw(e.Graphics, "", Width, Height);

            Font font = new Font("黑体", 12*1.33f, FontStyle.Bold, GraphicsUnit.Pixel);
            e.Graphics.DrawString("日 历", font, Brushes.White, Width / 2 - 40, 8);
            font.Dispose();

            if (!showImage)
                return;

            //if (nowSeason == UserProfile.Profile.time.Season)
            //{
            //    int day = UserProfile.Profile.time.Day;
            //    for (int i = 0; i < 28; i++)
            //    {
            //        if (day > i)
            //        {
            //            e.Graphics.FillRectangle(Brushes.DimGray, 30 + 70 * (i%7), 45 + 70 * (i/7), 70,70);
            //        }
            //        else if (day == i)
            //        {
            //            e.Graphics.FillRectangle(Brushes.DarkRed, 30 + 70 * (i % 7), 45 + 70 * (i / 7), 70, 70);
            //        }
            //    }
            //}
            Pen pen = new Pen(Color.White, 2);
            for (int i = 0; i < 5; i++)
            {
                e.Graphics.DrawLine(pen, 30, 45 + 70 * i, 520, 45 + 70 * i);
            }
            for (int i = 0; i < 8; i++)
            {
                e.Graphics.DrawLine(pen, 30 + 70 * i, 45, 30 + 70 * i, 325);
            }
            pen.Dispose();
            font = new Font("黑体", 12*1.33f, FontStyle.Bold, GraphicsUnit.Pixel);
            Font font2 = new Font("宋体", 9*1.33f, FontStyle.Regular, GraphicsUnit.Pixel);
            for (int i = 0; i < 28; i++)
            {
                e.Graphics.DrawString((i + 1).ToString().PadLeft(2), font, Brushes.White, 70 * (i%7)+63, 70 * (i/7)+53);
                int date = nowSeason*28 + i;
                int maid = TournamentBook.GetDayApplyId(date);
                int mmid = TournamentBook.GetDayMatchId(date);

                if (maid != 0)
                {
                    e.Graphics.DrawImage(HSIcons.GetIconsByEName("oth2"), 70 * (i % 7) + 37, 70 * (i / 7) + 50,24,24);
                    e.Graphics.DrawString(ConfigDatas.ConfigData.GetTournamentConfig(maid).Name, font2, Brushes.White, 70 * (i % 7) + 33, 70 * (i / 7) + 95);
                }
                else if (mmid != 0)
                {
                    e.Graphics.DrawImage(HSIcons.GetIconsByEName("oth1"), 70 * (i % 7) + 37, 70 * (i / 7) + 50, 24, 24);
                    e.Graphics.DrawString(ConfigDatas.ConfigData.GetTournamentConfig(mmid).Name, font2, Brushes.White, 70 * (i % 7) + 33, 70 * (i / 7) + 95);
                }
            }
            font.Dispose();
            font2.Dispose();
        }
    }
}