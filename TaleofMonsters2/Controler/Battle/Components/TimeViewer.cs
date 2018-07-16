using System;
using System.Drawing;
using System.Windows.Forms;
using System.Drawing.Drawing2D;
using TaleofMonsters.Controler.Battle.Tool;
using TaleofMonsters.Core;
using TaleofMonsters.Core.Loader;
using TaleofMonsters.Datas.Cards;

namespace TaleofMonsters.Controler.Battle.Components
{
    internal partial class TimeViewer : UserControl
    {
        private float time;//当前的虚拟时间
        private float round;//当前的回合数，超过固定值就可以抽牌
        private bool isShow;

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
            
            for (int i = 0; i < BattleManager.Instance.RelicHolder.RelicList.Count; i++)
            {
                var trapInfo = BattleManager.Instance.RelicHolder.RelicList[i];
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

            var bgImg = PicLoader.Read("System", "w0.JPG");
            for (int i = BattleManager.Instance.RelicHolder.RelicList.Count; i < GameConstants.MaxTrapCount; i++)
            {
                var rect = new Rectangle(6 + 35 * i, 35, 30, 30);
                e.Graphics.DrawImage(bgImg, rect);
            }
            bgImg.Dispose();
        }
    }
}
