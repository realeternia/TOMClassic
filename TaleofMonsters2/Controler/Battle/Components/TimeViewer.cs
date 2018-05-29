using System.Drawing;
using System.Windows.Forms;
using System.Drawing.Drawing2D;
using TaleofMonsters.Controler.Battle.Tool;
using TaleofMonsters.Core;
using TaleofMonsters.Datas.Cards;

namespace TaleofMonsters.Controler.Battle.Components
{
    internal partial class TimeViewer : UserControl
    {
        private float time;//当前的虚拟时间
        private float round;//当前的回合数，超过固定值就可以抽牌
        private int daytime;
        private bool isShow;

        internal TimeViewer()
        {
            InitializeComponent();
        }

        internal void Init()
        {
            time = 32;
            daytime = 1;
            isShow = true;
        }

        internal void OnFrame(float roundT)
        {
            float oldTime = time;
            //time += pastTime*10;//todo
            //if (time > 96) time = 0;
            BattleManager.Instance.IsNight = (time < 24 || time >= 72);
            daytime = BattleManager.Instance.IsNight ? 2 : 1;
            if (oldTime < 24 && time>=24)
            {
                SoundManager.Play("Time", "DaybreakRooster.mp3");
            }
            else if (oldTime < 72 && time >= 72)
            {
                SoundManager.Play("Time", "DuskWolf.mp3");
            }

            round = roundT;
            Invalidate();
        }

        private void TimeViewer_Paint(object sender, PaintEventArgs e)
        {
            LinearGradientBrush b1 = new LinearGradientBrush(new Rectangle(0, 0, Width, Height), Color.LightGreen, Color.Green, LinearGradientMode.Vertical);
            e.Graphics.FillRectangle(b1, 0, 0, round * Width, 30);
            b1.Dispose();

            Font font = new Font("Arial", 20*1.33f, FontStyle.Regular, GraphicsUnit.Pixel);
            e.Graphics.DrawString(string.Format("{0:00}:{1:00}", time / 4, (time % 4) * 15), font, Brushes.White, 22, 0);
            font.Dispose();

            if (!isShow)
                return;

            for (int i = 0; i < BattleManager.Instance.TrapHolder.TrapList.Count; i++)
            {
                var trapInfo = BattleManager.Instance.TrapHolder.TrapList[i];
                var rect = new Rectangle(6 + 35 * i, 35, 30, 30);
                if (trapInfo.Owner.IsLeft)
                {
                    Pen colorPen = new Pen(Color.Red, 3);
                    e.Graphics.DrawImage(CardAssistant.GetCardImage(trapInfo.SpellId, 30, 30), rect);
                    e.Graphics.DrawRectangle(colorPen, rect);
                    colorPen.Dispose();
                }
                else
                {
                    Pen colorPen = new Pen(Color.Blue, 3);
                    e.Graphics.DrawImage(HSIcons.GetIconsByEName("rot9"), rect);
                    e.Graphics.DrawRectangle(colorPen, rect);
                    colorPen.Dispose();
                }
            }
        }
    }
}
