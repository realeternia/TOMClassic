using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using System.Drawing.Drawing2D;
using System.Security.Cryptography.X509Certificates;
using ConfigDatas;
using NarlonLib.Control;
using NarlonLib.Core;
using TaleofMonsters.Config;
using TaleofMonsters.Controler.Battle.Data.Players;
using TaleofMonsters.Controler.Loader;
using TaleofMonsters.Core;
using TaleofMonsters.DataType.Cards;

namespace TaleofMonsters.Controler.Battle.Components
{
    internal sealed partial class LifeClock : UserControl
    {
        internal delegate void LifeClockFigueEventHandler();
        internal event LifeClockFigueEventHandler HeadClicked;

        private ImageToolTip tooltip = MainItem.SystemToolTip.Instance;

        private Player player;
        private Image back;
        private Image head;
        private string nameStr;
        public bool IsLeft { private get; set; }

        private bool mouseIn;

        public LifeClock()
        {
            InitializeComponent();
            DoubleBuffered = true;
        }

        public void Init()
        {
            back = PicLoader.Read("System", "LifeBack.JPG");
        }

        public void SetPlayer(Player p, string name, int headid)
        {
            nameStr = name;
			if(head != null)
				head.Dispose();
            head = PicLoader.Read("Player", string.Format("{0}.PNG", headid));
            player = p;
            player.ManaChanged += new Player.PlayerPointEventHandler(player_ManaChanged);
            player.CardLeftChanged += new Player.PlayerPointEventHandler(player_CardLeftChanged);
            player.TrapStateChanged += new Player.PlayerPointEventHandler(player_TrapChanged);
            Invalidate();
        }

        public void SetPlayer(Player p, int id)
        {
            if (id <= 0)
				return;

			PeopleConfig peopleConfig = ConfigData.GetPeopleConfig(id);
            nameStr = peopleConfig.Name;
			if(head != null)
				head.Dispose();
			head = PicLoader.Read("People", string.Format("{0}.PNG", peopleConfig.Figue));
            player = p;
            player.ManaChanged += new Player.PlayerPointEventHandler(player_ManaChanged);
            player.CardLeftChanged += new Player.PlayerPointEventHandler(player_CardLeftChanged);
            player.TrapStateChanged += new Player.PlayerPointEventHandler(player_TrapChanged);
            Invalidate();
        }

        private void player_ManaChanged()
        {
            Invalidate(new Rectangle(IsLeft ? 68 : 4, 10, Width - 72, 30));
        }
        private void player_CardLeftChanged()
        {
            Invalidate(new Rectangle(IsLeft ? Width - 52 : 53, 44, 20, 20));
        }
        private void player_TrapChanged()
        {
            Invalidate(new Rectangle(IsLeft ? Width - 120 : 80, 44, 50, 20));
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
            float lenth = TextRenderer.MeasureText(e.Graphics, nameStr, font, new Size(0, 0), TextFormatFlags.NoPadding).Width;
            e.Graphics.DrawString(nameStr, font, Brushes.White, IsLeft ? 72 : Width - 72 - lenth, 44);
            e.Graphics.DrawImage(HSIcons.GetIconsByEName("tsk7"), IsLeft ? Width - 75 : 30,44,18,18);//画剩余卡牌数
            if (player != null)
            { 
                e.Graphics.DrawString(player.Cards.LeftCount.ToString(), font, Brushes.White, IsLeft ? Width-52 : 53, 44);
                if (player.TrapHolder.Count > 0)
                {
                    var icon = HSIcons.GetIconsByEName("tsk6");
                    for (int i = 0; i < player.TrapHolder.Count; i++)
                        e.Graphics.DrawImage(icon, (IsLeft ? Width - 120 : 80)+i*8, 44, 18, 18);
                }
            }
            font.Dispose();
        }

        private void LifeClock_MouseMove(object sender, MouseEventArgs e)
        {
            if (IsLeft)
            {
                if (e.X > 2 && e.X < 60 && e.Y > 2 && e.Y < 66)
                {
                    if (!mouseIn)
                    {
                        mouseIn = true;
                        ShowTip();
                    }
                }
                else
                {
                    if (mouseIn)
                    {
                        mouseIn = false;
                        tooltip.Hide(this);
                    }
                }
            }
            else
            {
                if (e.X > 320 && e.X < 378 && e.Y > 2 && e.Y < 66)
                {
                    if (!mouseIn)
                    {
                        mouseIn = true;
                        ShowTip();
                    }
                }
                else
                {
                    if (mouseIn)
                    {
                        mouseIn = false;
                        tooltip.Hide(this);
                    }
                }
            }
        }

        private void LifeClock_MouseLeave(object sender, System.EventArgs e)
        {
            if (mouseIn)
            {
                mouseIn = false;
                tooltip.Hide(this);
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

        public void ShowTip()
        {
            int x = 0, y = 0;
            var img = GetPlayerImage();
            if (!IsLeft) //右边那人
                x = Width - img.Width;

            tooltip.Show(img, this, x, Location.Y+ 40);
        }

        private Image GetPlayerImage()
        {
            ControlPlus.TipImage tipData = new ControlPlus.TipImage();
            tipData.AddTextNewLine(string.Format("Lv{0}", player.Level), "LightBlue", 20);
            tipData.AddTextNewLine("能量回复比率", "White");
            tipData.AddTextNewLine(string.Format("LP {0}", player.EnergyGenerator.RateLp.ToString().PadLeft(3, ' ')), "Gold");
            tipData.AddBar(100, player.EnergyGenerator.RateLp, Color.Yellow, Color.Gold);
            tipData.AddTextNewLine(string.Format("PP {0}", player.EnergyGenerator.RatePp.ToString().PadLeft(3, ' ')), "Red");
            tipData.AddBar(100, player.EnergyGenerator.RatePp, Color.Pink, Color.Red);
            tipData.AddTextNewLine(string.Format("MP {0}", player.EnergyGenerator.RateMp.ToString().PadLeft(3, ' ')), "Blue");
            tipData.AddBar(100, player.EnergyGenerator.RateMp, Color.Cyan, Color.Blue);

            player.TrapHolder.GenerateImage(tipData, player is HumanPlayer);

            var rival = player.Rival as Player;
            if (rival.HasHolyWord("witcheye"))
            {
                tipData.AddLine();
                tipData.AddTextNewLine("手牌", "White");
                for (int i = 0; i < 10; i++)
                {
                    var card = player.CardManager.GetDeckCardAt(i);
                    if (card.CardId > 0)
                    {
                        var cardConfig = CardConfigManager.GetCardConfig(card.CardId);
                        tipData.AddTextNewLine("-", "White");
                        tipData.AddImage(CardAssistant.GetCardImage(card.CardId, 20, 20));
                        tipData.AddText(string.Format("{0}({1}★)Lv{2}", cardConfig.Name, cardConfig.Star, card.Level), HSTypes.I2QualityColor(cardConfig.Quality));
                    }
                }
            }
            return tipData.Image;
        }

    }
}
