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
            List<PlayerManaTypes> manaQueue = null;
            float roundRate = 0;
            if (player != null)
            {
                nextAimMana = player.EnergyGenerator.NextAimMana;
                manaQueue = player.EnergyGenerator.QueuedMana;
                roundRate = player.GetRoundRate();
            }

            Brush b1 = new LinearGradientBrush(new Rectangle(0, 18, 500, 10), Color.FromArgb(255, 120, 120), Color.FromArgb(255, 0, 0), LinearGradientMode.Vertical);
            Brush b2 = new LinearGradientBrush(new Rectangle(0, 30, 500, 10), Color.FromArgb(120, 120, 255), Color.FromArgb(0, 0, 255), LinearGradientMode.Vertical);
            Pen pen = new Pen(Brushes.Black, 2);
            if (IsLeft)
            {
                if (manaQueue != null)
                {
                    for (int i = 0; i < manaQueue.Count; i++)
                        e.Graphics.FillRectangle(PaintTool.GetBrushByManaType((int)manaQueue[i]),78-7,12+i*7,17,6);
                }
                if (nextAimMana != PlayerManaTypes.None)
                {
                    e.Graphics.DrawImage(HSIcons.GetIconsByEName("mix" + (int)nextAimMana), 78, 10, 30, 30);
                }
                var destRegion = new Rectangle(78, 10, 30, (int)(30*(1-roundRate)));
                e.Graphics.DrawImage(HSIcons.GetIconsByEName("mix0"), destRegion, 0, 0, 32, 32 * (1-roundRate), GraphicsUnit.Pixel);
                if (player != null)
                {
                    for (int i = 0; i < player.EnergyGenerator.LimitMp; i++)
                    {
                        if (i < player.Mp)
                        {
                            e.Graphics.FillRectangle(PaintTool.GetBrushByManaType((int)PlayerManaTypes.Mp), 112 + 24*i, 10, 22, 8);
                        }
                        e.Graphics.DrawRectangle(Pens.Gray, 112 + 24 * i, 10, 22, 8);
                    }
                    for (int i = 0; i < player.EnergyGenerator.LimitPp; i++)
                    {
                        if (i < player.Pp)
                        {
                            e.Graphics.FillRectangle(PaintTool.GetBrushByManaType((int)PlayerManaTypes.Pp), 112 + 24*i, 20, 22, 8);
                        }
                        e.Graphics.DrawRectangle(Pens.Gray, 112 + 24 * i, 20, 22, 8);
                    }
                    for (int i = 0; i < player.EnergyGenerator.LimitLp; i++)
                    {
                        if (i < player.Lp)
                        {
                            e.Graphics.FillRectangle(PaintTool.GetBrushByManaType((int)PlayerManaTypes.Lp), 112 + 24*i, 30, 22, 8);
                        }
                        e.Graphics.DrawRectangle(Pens.Gray, 112 + 24 * i, 30, 22, 8);
                    } 
                }
            }
            else
            {
                if (manaQueue != null)
                {
                    for (int i = 0; i < manaQueue.Count; i++)
                        e.Graphics.FillRectangle(PaintTool.GetBrushByManaType((int)manaQueue[i]), Width - 108 + 20, 12 + i * 7, 17, 6);
                }
                if (nextAimMana != PlayerManaTypes.None)
                {
                    e.Graphics.DrawImage(HSIcons.GetIconsByEName("mix" + (int) nextAimMana), Width - 108, 10, 30, 30);
                }
                var destRegion = new Rectangle(Width - 108, 10, 30, (int)(30 * (1 - roundRate)));
                e.Graphics.DrawImage(HSIcons.GetIconsByEName("mix0"), destRegion, 0, 0, 32, 32 * (1 - roundRate), GraphicsUnit.Pixel);

                if (player != null)
                {
                    for (int i = 0; i < player.EnergyGenerator.LimitMp; i++)
                    {
                        if (i < player.Mp)
                        {
                            e.Graphics.FillRectangle(PaintTool.GetBrushByManaType((int)PlayerManaTypes.Mp), Width - 134 - 24 * i, 10, 22, 8);    
                        }
                        e.Graphics.DrawRectangle(Pens.Gray, Width - 134 - 24 * i, 10, 22, 8);
                    }
                    for (int i = 0; i < player.EnergyGenerator.LimitPp; i++)
                    {
                        if (i < player.Pp)
                        {
                            e.Graphics.FillRectangle(PaintTool.GetBrushByManaType((int)PlayerManaTypes.Pp), Width - 134 - 24*i, 20, 22, 8);
                        }
                        e.Graphics.DrawRectangle(Pens.Gray, Width - 134 - 24 * i, 20, 22, 8);
                    }
                    for (int i = 0; i < player.EnergyGenerator.LimitLp; i++)
                    {
                        if (i < player.Lp)
                        {
                            e.Graphics.FillRectangle(PaintTool.GetBrushByManaType((int)PlayerManaTypes.Lp), Width - 134 - 24*i, 30, 22, 8);
                        }
                        e.Graphics.DrawRectangle(Pens.Gray, Width - 134 - 24 * i, 30, 22, 8);
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
