using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using NarlonLib.Core;
using NarlonLib.Math;
using NarlonLib.Tools;
using TaleofMonsters.Controler.Loader;

namespace TaleofMonsters.Forms.MiniGame
{
    internal partial class MGTwentyOne : MGBase
    {
        private enum BattleResult
        {
            MyWin, HisWin, Draw
        }

        private List<int> myCards;
        private List<int> hisCards;
        private RandomSequence rs;

        private int myMark;
        private int hisMark;

        private int myScore;
        private int hisScore;
        private bool myFull;
        private bool hisFull;

        private float showResultTime;
        private BattleResult battleResult;
             
        private TimeCounter tc = new TimeCounter(0.5f);

        public MGTwentyOne()
        {
            InitializeComponent();
            this.bitmapButtonC1.ImageNormal = PicLoader.Read("Button.Panel", "ButtonBack2.PNG");
            bitmapButtonC1.Font = new Font("宋体", 8 * 1.33f, FontStyle.Regular, GraphicsUnit.Pixel);
            bitmapButtonC1.ForeColor = Color.White;
            bitmapButtonC1.IconImage = TaleofMonsters.Core.HSIcons.GetIconsByEName("oth9");
            bitmapButtonC1.IconSize = new Size(16, 16);
            bitmapButtonC1.IconXY = new Point(4, 5);
            bitmapButtonC1.TextOffX = 8;

            this.bitmapButtonC2.ImageNormal = PicLoader.Read("Button.Panel", "ButtonBack2.PNG");
            bitmapButtonC2.Font = new Font("宋体", 8 * 1.33f, FontStyle.Regular, GraphicsUnit.Pixel);
            bitmapButtonC2.ForeColor = Color.White;
            bitmapButtonC2.IconImage = TaleofMonsters.Core.HSIcons.GetIconsByEName("oth11");
            bitmapButtonC2.IconSize = new Size(16, 16);
            bitmapButtonC2.IconXY = new Point(4, 5);
            bitmapButtonC2.TextOffX = 8;
        }

        public override void Init(int width, int height)
        {
            base.Init(width, height);
            
            RestartGame();
        }

        public override void OnFrame(int tick, float timePass)
        {
            base.OnFrame(tick, timePass);

            if (showResultTime > 0)
            {
                showResultTime -= timePass;
                if (showResultTime <= 0)
                {
                    NewRound();
                }
            }
            else
            {
                if (tc.OnTick())
                {
                    AIRound();
                }
            }
        }

        public override void RestartGame()
        {
            base.RestartGame();
            myMark = 0;
            hisMark = 0;
            NewRound();
        }

        protected override void CalculateResult()
        {
            if (myMark >= 10 || hisMark >= 10)
            {
                score = 50 + myMark*5 - hisMark*5;
                EndGame();
            }
        }

        public override void EndGame()
        {
            base.EndGame();
        }

        private void UpdateScore()
        {
            myScore = 0;
            hisScore = 0;
            for (int i = 0; i < myCards.Count; i++)
            {
                int number = (myCards[i] % 13) + 1;
                if (number > 10)
                {
                    number = 10;
                }
                myScore += number;
            }
            for (int i = 0; i < hisCards.Count; i++)
            {
                int number = (hisCards[i] % 13) + 1;
                if (number > 10)
                {
                    number = 10;
                }
                hisScore += number;
            }

            if (myScore >= 21 || hisScore >= 21 || myFull && hisFull)
            {
                if (myScore == 21 || hisScore > 21)
                {
                    myMark += 2;
                    battleResult = BattleResult.MyWin;
                }
                else if (hisScore == 21 || myScore > 21)
                {
                    hisMark += 2;
                    battleResult = BattleResult.HisWin;
                }
                else
                {
                    if (myScore > hisScore)
                    {
                        battleResult = BattleResult.MyWin;
                        myMark += 2;
                    }
                    else if (myScore < hisScore)
                    {
                        battleResult = BattleResult.HisWin;
                        hisMark += 2;
                    }
                    else
                    {
                        battleResult = BattleResult.Draw;
                        myMark ++;
                        hisMark ++;
                    }
                }

                showResultTime = 2;//显示2秒的结果
                Invalidate(new Rectangle(xoff, yoff, 324, 244));
                BeginCalculateResult();
            }
        }

        private void NewRound()
        {
            myCards = new List<int>();
            hisCards = new List<int>();
            rs = new RandomSequence(52);
           
            for (int i = 0; i < 2; i++)
            {
                myCards.Add(rs.NextNumber());
                hisCards.Add(rs.NextNumber());
            }
            myFull = false;
            hisFull = false;
            Invalidate(new Rectangle(xoff, yoff, 324, 244));
        }

        private void AIRound()
        {
            var viewScore = 0; 
            for (int i = 1; i < myCards.Count; i++)
            {
                int number = (myCards[i] % 13) + 1;
                if (number > 10)
                {
                    number = 10;
                }
                viewScore += number;
            }

            if (hisScore < viewScore && hisScore < 21)
            {
                hisCards.Add(rs.NextNumber());
                Invalidate(new Rectangle(xoff, yoff, 324, 244));
                hisFull = false;
                myFull = false;
            }
            else if (hisScore < viewScore+6 && hisScore < 16)
            {
                hisCards.Add(rs.NextNumber());
                Invalidate(new Rectangle(xoff, yoff, 324, 244));
                hisFull = false;
                myFull = false;
            }
            else
            {
                hisFull = true;
            }
            UpdateScore();
        }

        private void bitmapButtonC1_Click(object sender, EventArgs e)
        {
            if (showResultTime > 0)
            {
                return;
            }
            myCards.Add(rs.NextNumber());
            Invalidate(new Rectangle(xoff, yoff, 324, 244));
            hisFull = false;
            myFull = false;
            UpdateScore();
        }
        private void bitmapButtonC2_Click(object sender, EventArgs e)
        {
            if (showResultTime > 0)
            {
                return;
            }
            myFull = true;
            UpdateScore();
        }

        private void MGTwentyOne_Paint(object sender, PaintEventArgs e)
        {
            DrawBase(e.Graphics);

            if (!show)
                return;

            for (int i = 0; i < myCards.Count; i++)
            {
                DrawIcon(e.Graphics, 20 + xoff + i * 45, 180 + yoff, myCards[i]);
            }

            DrawIcon(e.Graphics, 20 + xoff, 80 + yoff, 99);
            for (int i = 1; i < hisCards.Count; i++)
            {
                DrawIcon(e.Graphics, 20 + xoff + i * 45, 80 + yoff, hisCards[i]);
            }

            var font = new Font("宋体", 26 * 1.33f, FontStyle.Bold, GraphicsUnit.Pixel);
            DrawShadeText(e.Graphics, string.Format(string.Format("{0}:{1}", myMark, hisMark)), font, Brushes.White, 90 + xoff, 20 + yoff);
            font.Dispose();

            if (showResultTime > 0)
            {
                DrawIcon(e.Graphics, 20 + xoff, 80 + yoff, hisCards[0]);
              
                font = new Font("宋体", 20 * 1.33f, FontStyle.Bold, GraphicsUnit.Pixel);
                DrawShadeText(e.Graphics, myScore.ToString(), font, myScore == 21 ?Brushes.Gold : myScore > 21 ? Brushes.Red : Brushes.Lime, 50 + xoff + 220, 180 + yoff);
                DrawShadeText(e.Graphics, hisScore.ToString(), font, hisScore == 21 ? Brushes.Gold : hisScore > 21 ? Brushes.Red : Brushes.Lime, 50 + xoff + 220, 80 + yoff);
                font.Dispose();

                if (battleResult == BattleResult.MyWin)
                {
                    var img2 = PicLoader.Read("System", "Win.PNG");
                    e.Graphics.DrawImage(img2, 50 + xoff + 100, 180 + yoff + 5, 40, 50);
                    img2.Dispose();
                }
                else if (battleResult == BattleResult.HisWin)
                {
                    var img2 = PicLoader.Read("System", "Win.PNG");
                    e.Graphics.DrawImage(img2, 50 + xoff + 100, 80 + yoff + 5, 40, 50);
                    img2.Dispose();
                }
                else
                {
                    var img2 = PicLoader.Read("System", "Draw.PNG");
                    e.Graphics.DrawImage(img2, 50 + xoff + 100, 130 + yoff + 5, 40, 50);
                    img2.Dispose();
                }
            }
        }

        private static void DrawIcon(Graphics g, int x, int y, int v)
        {
            if (v == 99)
            {
                var img2 = PicLoader.Read("System", "CardBack.JPG");
                g.DrawImage(img2, x, y, 40, 50);
                img2.Dispose();
                return;
            }

            g.FillRectangle(Brushes.Wheat, x, y, 40, 50);
            int type = (v/13) + 1;
            var img = PicLoader.Read("MiniGame.Poker", type + ".PNG");
            g.DrawImage(img, x,y,20,20);
            img.Dispose();
            int number = (v%13) + 1;
            string numberStr = number.ToString();
            if (number == 11)
                numberStr = "J";
            else if (number == 12)
                numberStr = "Q";
            else if (number == 13)
                numberStr = "K";
            System.Drawing.Font ft = new Font("宋体", 12, FontStyle.Bold);
            g.DrawString(numberStr,ft,type ==1 || type==3 ? Brushes.Black: Brushes.Red, x+18,y+30);

        }

    }
}
