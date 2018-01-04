using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Text.RegularExpressions;
using System.Threading;
using System.Windows.Forms;
using ConfigDatas;
using ControlPlus; 
using NarlonLib.Log;
using NarlonLib.Tools;
using TaleofMonsters.Config;
using TaleofMonsters.Controler.GM;
using TaleofMonsters.Controler.Loader;
using TaleofMonsters.Controler.World;
using TaleofMonsters.Core;
using TaleofMonsters.DataType.User;
using TaleofMonsters.Forms;
using TaleofMonsters.MainItem;
using TaleofMonsters.MainItem.Scenes;

namespace TaleofMonsters
{
    internal partial class MainForm : Form
    {
        public static MainForm Instance { get; private set; }

        private HSCursor myCursor;
        private int page;

        private Thread workThread;
        private int timeTick;
        private long lastMouseMoveTime;
        private MainFlowController flowController;

        public MainForm()
        {
            InitializeComponent();
            bitmapButtonLogin.ImageNormal = PicLoader.Read("Button.Panel", "ButtonBack1.PNG");
            bitmapButtonExit.ImageNormal = PicLoader.Read("Button.Panel", "ButtonBack1.PNG");

            if (Config.Config.ResolutionBigger)
            {
                Width = 1440;
                Height = 900;
            }
            else
            {
                Width = 1152;
                Height = 720;
            }
            myCursor = new HSCursor(this);
            flowController = new MainFlowController(tabPageGame);
            Instance = this;
        }
        
        private void MainForm_Load(object sender, EventArgs e)
        {
            string version = FileVersionInfo.GetVersionInfo(Application.ExecutablePath).FileVersion;
            Text = string.Format("幻兽传说卡片版 v{0}", version);
          
            try
            {
                ConfigData.LoadData();
                Scene.Instance = new Scene(tabPageGame, tabPageGame.Width, tabPageGame.Height);
                SystemMenuManager.Load(tabPageGame.Width, tabPageGame.Height);
                MainTipManager.Init(tabPageGame.Height);
                PanelManager.Init(tabPageGame.Width, tabPageGame.Height);
                CardConfigManager.Init();
                DbSerializer.Init();
                WorldInfoManager.Load();
            }
            catch (Exception ex)
            {
                NLog.Warn(ex);
                Close();
            }

            tabPageLogin.BackgroundImage = PicLoader.Read("System", "LogBack.JPG");
            textBoxName.Text = WorldInfoManager.LastAccountName;
            ChangePage(0);
            myCursor.ChangeCursor("default");
            Scene.Instance.Init();

            workThread = new Thread(TimeGo);
            workThread.IsBackground = true;
            workThread.Start();
        }

        public void ChangePage(int pg)
        {
            if (pg == 0)
            {
                textBoxPasswd.Text = "";
                SoundManager.PlayBGM("TOM000.mp3");
            }
            else if (pg == 1)
            {
                UserProfile.ProfileName = textBoxName.Text;
                if (!UserProfile.LoadFromDB(textBoxName.Text))
                {
                    UserProfile.Profile = new Profile();
                    CreatePlayerForm cpf = new CreatePlayerForm();
                    cpf.ShowDialog();
                    if (cpf.Result == DialogResult.Cancel)
                        return;
                }
                MainTipManager.Refresh();
                SoundManager.PlayBGM("TOM001.mp3");
                Scene.Instance.ChangeMap(UserProfile.InfoBasic.MapId, false);
                UserProfile.Profile.OnLogin();
            }
            page = pg;
            viewStack1.SelectedIndex = page;
        }

        public void AddFlow(string msg, string icon, Color color, Point pos)
        {           
            if (!string.IsNullOrEmpty(msg))
            {
                flowController.Add(msg, icon, color, pos);
            }
        }

        private void buttonLogin_Click(object sender, EventArgs e)
        {
            if (textBoxName.Text == "")
            {
                MessageBoxEx.Show("游戏账号不能为空！");
                return;
            }

            Regex regex = new Regex("[a-zA-Z0-9]+");
            var match = regex.Match(textBoxName.Text);
            if (!match.Success || match.Captures[0].Value != textBoxName.Text)
            {
                MessageBoxEx.Show("账户名只能包含字母和数字");
                return;
            }

            WorldInfoManager.LastAccountName = textBoxName.Text;
            ChangePage(1);
        }

        private void MainForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (viewStack1.SelectedIndex == 1)
            {
                UserProfile.Profile.OnLogout();
                UserProfile.SaveToDB();
            }
        }

        private void MainForm_MouseMove(object sender, MouseEventArgs e)
        {
            if (lastMouseMoveTime + 50 > TimeTool.GetNowMiliSecond())
            {
                return;
            }
            lastMouseMoveTime = TimeTool.GetNowMiliSecond();

            if (SystemMenuManager.UpdateToolbar(e.X, e.Y))
            {
                tabPageGame.Invalidate();
            }

            Scene.Instance.CheckMouseMove(e.X, e.Y);
        }

        private void MainForm_MouseClick(object sender, MouseEventArgs e)
        {
            SystemMenuManager.CheckItemClick((SystemMenuIds)SystemMenuManager.MenuTar);
            Scene.Instance.CheckMouseClick();
        }

        private void MainForm_KeyUp(object sender, KeyEventArgs e)
        {
            if (viewStack1.SelectedIndex == 1)
            {
                PanelManager.CheckHotKey(e, true);
                SystemMenuManager.CheckHotKey(e);
            }
        }
        private void MainForm_KeyDown(object sender, KeyEventArgs e)
        {
            if (viewStack1.SelectedIndex == 1)
                PanelManager.CheckHotKey(e, false);
        }

        private void buttonExit_Click(object sender, EventArgs e)
        {
            Close();
        }

        public void RefreshView()
        {//有的时候需要整天刷新一次
            tabPageGame.Invalidate();
        }

        public BasePanel FindPanelAct(Type type)
        {
            foreach (var control in tabPageGame.Controls)
            {
                if (control.GetType() == type)
                    return control as BasePanel;
            }

            return null;
        }

        public void AddPanelAct(BasePanel panel)
        {
            tabPageGame.Controls.Add(panel);
            panel.BringToFront();
        }

        public void RemovePanelAct(BasePanel panel)
        {
            tabPageGame.Controls.Remove(panel);
        }

        private void TimeGo()
        {
            while (true)
            {
                if (page == 1)
                {
                    try
                    {
                        timeTick++;
                        if (timeTick > 1000)
                            timeTick -= 1000;
                        foreach (var control in tabPageGame.Controls)
                        {
                            if (control is BasePanel)
                                (control as BasePanel).OnFrame(timeTick, 0.05f);
                        }

                        if (SystemMenuManager.IsHotkeyEnabled && (timeTick % 5) == 0)
                        {
                            SystemMenuManager.UpdateAll(tabPageGame);

                            if (MainTipManager.OnFrame())
                                tabPageGame.Invalidate();
                        }

                        if (SystemMenuManager.GMMode)
                            GMCodeZone.OnFrame();

                        Scene.Instance.TimeGo(0.05f);
                        if (flowController != null)
                            flowController.CheckTick();
                    }
                    catch (Exception e)
                    {
                        NLog.Error(e);
                        throw;
                    }
                }
                Thread.Sleep(50);
            }
        }

        private void tabPageLogin_Paint(object sender, PaintEventArgs e)
        {
            e.Graphics.DrawImage(HSIcons.GetIconsByEName("rac5"), 10, tabPageLogin.Height - 160, 20, 20);
            e.Graphics.DrawImage(HSIcons.GetIconsByEName("hatt1"), 10, tabPageLogin.Height - 135, 20, 20);
            e.Graphics.DrawImage(HSIcons.GetIconsByEName("spl2"), 10, tabPageLogin.Height - 110, 20, 20);

            Font font = new Font("微软雅黑", 12 * 1.33f, FontStyle.Regular, GraphicsUnit.Pixel);
            e.Graphics.DrawString(string.Format("{0} / {1}", CardConfigManager.MonsterAvail, CardConfigManager.MonsterTotal), font, Brushes.White, 35, tabPageLogin.Height - 160);
            e.Graphics.DrawString(string.Format("{0} / {1}", CardConfigManager.WeaponAvail, CardConfigManager.WeaponTotal), font, Brushes.White, 35, tabPageLogin.Height - 135);
            e.Graphics.DrawString(string.Format("{0} / {1}", CardConfigManager.SpellAvail, CardConfigManager.SpellTotal), font, Brushes.White, 35, tabPageLogin.Height - 110);
            font.Dispose();
        }

        private void tabPageGame_Paint(object sender, PaintEventArgs e)
        {
            try
            {
                Scene.Instance.Paint(e.Graphics);

                SystemMenuManager.DrawAll(e.Graphics);
                MainTipManager.DrawAll(e.Graphics);
                flowController.DrawAll(e.Graphics);

                if (SystemMenuManager.GMMode) //希望在最上层，所以必须最后绘制
                    GMCodeZone.Paint(e.Graphics, tabPageGame.Width, tabPageGame.Height);
            }
            catch (Exception err)
            {
                NLog.Error(err);
            }
        }

    }
}