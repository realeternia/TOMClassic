﻿using System;
using System.Diagnostics;
using System.Drawing;
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
using TaleofMonsters.Tools;

namespace TaleofMonsters
{
    internal partial class MainForm : Form
    {
        delegate void ShowDisconnectCallback(string text);
        public void ShowDisconnectSafe(string text)
        {
            if (InvokeRequired)
            {
                ShowDisconnectCallback d = ShowDisconnectSafe;
                Invoke(d, new object[] { text });
            }
            else
            {
                PanelManager.DealPanel(BlackWallForm.Instance);
                BringToFront();
                MessageBoxEx.Show(text);
                PanelManager.DealPanel(BlackWallForm.Instance);
                ChangePage(0);
            }
        }

        delegate void LoginResultCallback();
        public void LoginResult()
        {
            if (InvokeRequired)
            {
                LoginResultCallback d = LoginResult;
                Invoke(d, new object[] { });
            }
            else
            {
                var isNewUser = false;
                if (string.IsNullOrEmpty(UserProfile.Profile.Name))
                {
                    CreatePlayerForm cpf = new CreatePlayerForm();
                    cpf.BringToFront();
                    cpf.ShowDialog();
                    isNewUser = true;
                    if (cpf.Result == DialogResult.Cancel)
                        return;
                }
                MainTipManager.Refresh();
                Scene.Instance.ChangeMap(UserProfile.InfoBasic.MapId, isNewUser);
                UserProfile.Profile.OnLogin();

                page = 1;
                viewStack1.SelectedIndex = page;
            }
        }

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
            myCursor = new HSCursor(this);
            flowController = new MainFlowController(tabPageGame);

            Instance = this;
            WorldInfoManager.Load();
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
                SoundManager.PlayBGMScene("SCN000.mp3");
                page = pg;
                viewStack1.SelectedIndex = page;
            }
            else if (pg == 1)
            {
                UserProfile.ProfileName = textBoxName.Text;
                UserProfile.Connect();
            }

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
            var resultD = NameChecker.CheckNameEng(textBoxName.Text, GameConstants.ProfileNameLengthMin, GameConstants.ProfileNameLengthMax);
            if (resultD != NameChecker.NameCheckResult.Ok)
            {
                if (resultD == NameChecker.NameCheckResult.NameEmpty)
                    MessageBoxEx.Show("账号名不能为空");
                else if (resultD == NameChecker.NameCheckResult.NameLengthError)
                    MessageBoxEx.Show("账号名需要在3-12个字之内");
                else if (resultD == NameChecker.NameCheckResult.PunctuationOnly)
                    MessageBoxEx.Show("不能仅包含标点符号");
                else if (resultD == NameChecker.NameCheckResult.EngOnly)
                    MessageBoxEx.Show("仅能使用英文和数字");
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
                UserProfile.Save();
            }
        }

        private void MainForm_MouseMove(object sender, MouseEventArgs e)
        {
            if (lastMouseMoveTime + 50 > TimeTool.GetNowMiliSecond())
                return;
            lastMouseMoveTime = TimeTool.GetNowMiliSecond();

            if (SystemMenuManager.UpdateToolbar(e.X, e.Y))
                tabPageGame.Invalidate();

            Scene.Instance.CheckMouseMove(e.X, e.Y);
        }

        private void MainForm_MouseClick(object sender, MouseEventArgs e)
        {
            SystemMenuManager.CheckItemClick((SystemMenuIds)SystemMenuManager.MenuTar);
            if (PanelManager.PanelCount <= 0)
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
        public void RefreshView(Rectangle rect)
        {
            tabPageGame.Invalidate(rect);
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
                UserProfile.Oneloop();

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
            var loginBg = PicLoader.Read("System", "LoginBg.PNG");
            e.Graphics.DrawImage(loginBg,Width/2-413/2,310,413,242);
            loginBg.Dispose();

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