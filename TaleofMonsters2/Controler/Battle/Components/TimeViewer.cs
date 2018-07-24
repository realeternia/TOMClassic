using System;
using System.Drawing;
using System.Windows.Forms;
using System.Drawing.Drawing2D;
using ControlPlus;
using TaleofMonsters.Controler.Battle.Tool;
using TaleofMonsters.Core;
using TaleofMonsters.Datas;
using TaleofMonsters.Datas.Cards;
using TaleofMonsters.Datas.Decks;

namespace TaleofMonsters.Controler.Battle.Components
{
    internal partial class TimeViewer : UserControl
    {
        private float time;//当前的虚拟时间
        private float round;//当前的回合数，超过固定值就可以抽牌
        private bool isShow;
        private ImageToolTip tooltip = new ImageToolTip();
        private bool mouseIn;

        public TimeViewer()
        {
            InitializeComponent();
        }

        internal void Init()
        {
            isShow = true;
        }

        internal void OnFrame()
        {
            var roundMark = BattleManager.Instance.RoundMark;
            var roundTotal = (float)roundMark*50/GameConstants.RoundTime;//回合数
            var roundTime = roundTotal*2; //一个回合两小时
            var oldTime = time;
            time = (roundTime + 8)%24; //开始是8点

            BattleManager.Instance.IsNight = (time < 6 || time > 18);
            if (oldTime < 6 && time>=6)
                SoundManager.Play("Time", "DaybreakRooster.mp3");
            else if (oldTime < 18 && time >= 18)
                SoundManager.Play("Time", "DuskWolf.mp3");

            round = BattleManager.Instance.Round;
            Invalidate();
        }

        private void TimeViewer_Paint(object sender, PaintEventArgs e)
        {
            LinearGradientBrush b1 = new LinearGradientBrush(new Rectangle(0, 0, Width, Height), Color.LightGreen, Color.Green, LinearGradientMode.Vertical);
            e.Graphics.FillRectangle(b1, 0, 0, round * Width, 30);
            b1.Dispose();

            Font font = new Font("Arial", 20*1.33f, FontStyle.Regular, GraphicsUnit.Pixel);
            e.Graphics.DrawString(string.Format("{0:00}:{1:00}", (int)time, (int)((time - (int)time) * 4) * 15), font, Brushes.White, 22, 0);
            font.Dispose();

            if (!isShow)
                return;

            BattleManager.Instance.RelicHolder.Draw(e.Graphics);
        }

        private void TimeViewer_MouseMove(object sender, MouseEventArgs e)
        {
            var relic = BattleManager.Instance.RelicHolder.GetRelic(e.X, e.Y);
            if (relic != null)
            {
                if (!mouseIn)
                {
                    var card = CardAssistant.GetCard(relic.Id);
                    DeckCard dc = new DeckCard(relic.Id, (byte) relic.Level, 0);
                    card.SetData(dc);
                    var img = card.GetPreview(CardPreviewType.Normal, new uint[0]);
                    tooltip.Show(img, this, e.X, e.Y + 20);
                    mouseIn = true;
                }
            }
            else if(mouseIn)
            {
                tooltip.Hide(this);
                mouseIn = false;
            }
        }

        private void TimeViewer_MouseLeave(object sender, EventArgs e)
        {
            if (mouseIn)
            {
                tooltip.Hide(this);
                mouseIn = false;
            }
        }
    }
}
