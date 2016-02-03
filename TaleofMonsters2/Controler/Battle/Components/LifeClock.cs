using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using System.Drawing.Drawing2D;
using ConfigDatas;
using TaleofMonsters.Controler.Battle.Data.Players;
using TaleofMonsters.Controler.Loader;
using TaleofMonsters.Core;

namespace TaleofMonsters.Controler.Battle.Components
{
    internal sealed partial class LifeClock : UserControl
    {
        internal delegate void LifeClockFigueEventHandler();
        internal event LifeClockFigueEventHandler HeadClicked;

        private Player player;
        private Image back;
        private Image head;
        private string nameStr;
        public bool IsLeft { private get; set; }

        #region 属性
        internal Player Player
        {
            get { return player; }
            set
            {
                player = value;
                if (player != null)
                {
                    player.ManaChanged += new Player.PlayerPointEventHandler(player_ManaChanged);
                }
                Invalidate();
            }
        }
        
        #endregion

        public LifeClock()
        {
            InitializeComponent();
            DoubleBuffered = true;
        }

        public void Init()
        {
            back = PicLoader.Read("System", "LifeBack.JPG");
        }

        public void SetPlayer(string name, int headid)
        {
            nameStr = name;
			if(head != null)
				head.Dispose();
            head = PicLoader.Read("Player", string.Format("{0}.PNG", headid));
            Invalidate();
        }

        public void SetPlayer(int id)
        {
            if (id <= 0)
				return;

			PeopleConfig peopleConfig = ConfigData.GetPeopleConfig(id);
            nameStr = peopleConfig.Name;
			if(head != null)
				head.Dispose();
			head = PicLoader.Read("People", string.Format("{0}.PNG", peopleConfig.Figue));
			Invalidate();
        }

        private void player_ManaChanged()
        {
            Invalidate(new Rectangle(IsLeft ? 68 : 4, 10, Width - 72, 30));
        }

        private void LifeClock_Paint(object sender, PaintEventArgs e)
        {
            if (back == null)//在editor模式下就不绘制了
            {
                return;
            }
            e.Graphics.DrawImage(back, 0, 0, Width, Height);
            if (head != null)
            {
                if (IsLeft)
                    e.Graphics.DrawImage(head, 4, 3, 63, 64);
                else
                    e.Graphics.DrawImage(head, Width - 67, 3, 63, 64);
            }
            PlayerManaTypes nextAimMana = PlayerManaTypes.None;
            float roundRate = 0;
            if (player != null)
            {
                nextAimMana = player.EnergyGenerator.NextAimMana;
                roundRate = player.GetRoundRate();
            }

            Brush b1 = new LinearGradientBrush(new Rectangle(0, 18, 500, 10), Color.FromArgb(255, 120, 120), Color.FromArgb(255, 0, 0), LinearGradientMode.Vertical);
            Brush b2 = new LinearGradientBrush(new Rectangle(0, 30, 500, 10), Color.FromArgb(120, 120, 255), Color.FromArgb(0, 0, 255), LinearGradientMode.Vertical);
            Pen pen = new Pen(Brushes.Black, 2);
            if (IsLeft)
            {
                if (nextAimMana != PlayerManaTypes.None)
                {
                    e.Graphics.DrawImage(HSIcons.GetIconsByEName("mix" + (int)nextAimMana), 73, 10, 30, 30);
                }
                var destRegion = new Rectangle(73, 10, 30, (int)(30*(1-roundRate)));
                e.Graphics.DrawImage(HSIcons.GetIconsByEName("mix0"), destRegion, 0, 0, 32, 32 * (1-roundRate), GraphicsUnit.Pixel);
                if (player != null)
                {
                    for (int i = 0; i < player.Mp; i++)
                    {
                        e.Graphics.DrawImage(HSIcons.GetIconsByEName("mix1"), 112 + 24 * i, 10, 20, 10);
                    }
                    for (int i = 0; i < player.Pp; i++)
                    {
                        e.Graphics.DrawImage(HSIcons.GetIconsByEName("mix2"), 112 + 24 * i, 20, 20, 10);
                    }
                    for (int i = 0; i < player.Lp; i++)
                    {
                        e.Graphics.DrawImage(HSIcons.GetIconsByEName("mix3"), 112 + 24 * i, 30, 20, 10);
                    } 
                }
            }
            else
            {
                if (nextAimMana != PlayerManaTypes.None)
                {
                    e.Graphics.DrawImage(HSIcons.GetIconsByEName("mix" + (int) nextAimMana), Width - 105, 10, 30, 30);
                }
                var destRegion = new Rectangle(Width - 105, 10, 30, (int)(30 * (1 - roundRate)));
                e.Graphics.DrawImage(HSIcons.GetIconsByEName("mix0"), destRegion, 0, 0, 32, 32 * (1 - roundRate), GraphicsUnit.Pixel);

                if (player != null)
                {
                    for (int i = 0; i < player.Mp; i++)
                    {
                        e.Graphics.DrawImage(HSIcons.GetIconsByEName("mix1"), Width - 134 - 24 * i, 10, 20, 10);
                    }
                    for (int i = 0; i < player.Pp; i++)
                    {
                        e.Graphics.DrawImage(HSIcons.GetIconsByEName("mix2"), Width - 134 - 24 * i, 20, 20, 10);
                    }
                    for (int i = 0; i < player.Lp; i++)
                    {
                        e.Graphics.DrawImage(HSIcons.GetIconsByEName("mix3"), Width - 134 - 24 * i, 30, 20, 10);
                    }
                }
            }
            pen.Dispose();
            b1.Dispose();
            b2.Dispose();
            Font font = new Font("幼圆", 12*1.33f, FontStyle.Bold, GraphicsUnit.Pixel);
            int lenth = (int) e.Graphics.MeasureString(nameStr, font).Width;
            e.Graphics.DrawString(nameStr, font, Brushes.White, IsLeft ? 72 : Width - 72 - lenth, 44);
            font.Dispose();
            if (player != null)
            {
                //e.Graphics.FillRectangle(Brushes.DarkSlateGray, IsLeft ? 308 - 6 * 40 : 34 + 6 * 40, 3, 36, 12);
                //e.Graphics.DrawImage(Core.HSIcons.GetIconsByEName("sym1"), IsLeft ? 310 - 6 * 40 : 36 + 6 * 40, 2, 12, 12);

                //e.Graphics.FillRectangle(Brushes.DarkSlateGray, IsLeft ? 308 - 5 * 40 : 34 + 5 * 40, 3, 36, 12);
                //e.Graphics.DrawImage(Core.HSIcons.GetIconsByEName("sym2"), IsLeft ? 310 - 5 * 40 : 36 + 5 * 40, 2, 12, 12);

                //e.Graphics.FillRectangle(Brushes.DarkSlateGray, IsLeft ? 308 - 4 * 40 : 34 + 4 * 40, 3, 36, 12);
                //e.Graphics.DrawImage(Core.HSIcons.GetIconsByEName("sym3"), IsLeft ? 310 - 4 * 40 : 36 + 4 * 40, 2, 12, 12);

                font = new Font("Arial", 8*1.33f, FontStyle.Regular, GraphicsUnit.Pixel);
              //  e.Graphics.DrawString(string.Format("{0:00}", player.GetExtraInfo(PlayerExtraInfo.Time)), font, Brushes.White, IsLeft ? 308 - 6 * 40 + 17 : 34 + 6 * 40 + 17, 2);
             //   e.Graphics.DrawString(string.Format("{0:00}", player.GetExtraInfo(PlayerExtraInfo.Soul)), font, Brushes.White, IsLeft ? 308 - 5 * 40 + 17 : 34 + 5 * 40 + 17, 2);
              //  e.Graphics.DrawString(string.Format("{0:00}", player.GetExtraInfo(PlayerExtraInfo.Nature)), font, Brushes.White, IsLeft ? 308 - 4 * 40 + 17 : 34 + 4 * 40 + 17, 2);
                font.Dispose();

                //int id = 0;显示对方卡牌
                //for (int i = 1; i <= 6; i++)
                //{
                //    ActiveCard card = player.GetDeckCardAt(i);                        
                //    if (card.Id>0)
                //    {
                //        Image cimg = CardAssistant.GetCardImage(card.CardId, 16, 16);
                //        e.Graphics.DrawImage(cimg, new Rectangle(IsLeft ? 355 - 18*id : 9 + 18*id, 46, 16, 16), new Rectangle(0, 0, cimg.Width, cimg.Height), GraphicsUnit.Pixel);
                //        id++;
                //    }
                //}
            }
        }

        private void LifeClock_MouseClick(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                if (IsLeft)
                {
                    if (e.X > 4 && e.X < 67 && e.Y > 3 && e.Y < 67)
                    {
                        if (HeadClicked != null)
                            HeadClicked();
                    }
                }
            }
        }
    }
}
