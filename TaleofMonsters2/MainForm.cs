using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Windows.Forms;
using ConfigDatas;
using ControlPlus; 
using NarlonLib.Core;
using TaleofMonsters.Controler.Loader;
using TaleofMonsters.Controler.World;
using TaleofMonsters.Core;
using TaleofMonsters.DataType.User;
using TaleofMonsters.Forms;
using TaleofMonsters.MainItem;
using System.Threading;
using NarlonLib.Log;

namespace TaleofMonsters
{
    internal partial class MainForm : Form
    {
        private HSCursor myCursor;
        private int page;
        private List<BasePanel> panelList = new List<BasePanel>();
        private Thread workThread;
        private int timeTick;
        private int timeMinutes;
        private long lastMouseMoveTime;
        private string mouseStr;
        public static MainForm Instance { get; private set; }

        public MainForm()
        {
            InitializeComponent();
            bitmapButtonLogin.ImageNormal = PicLoader.Read("ButtonBitmap", "ButtonBack1.PNG");
            bitmapButtonExit.ImageNormal = PicLoader.Read("ButtonBitmap", "ButtonBack1.PNG");

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
            Instance = this;
        }
        
        private void MainForm_Load(object sender, EventArgs e)
        {
            string version = FileVersionInfo.GetVersionInfo(Application.ExecutablePath).FileVersion;
            Text = string.Format("幻兽传说卡片版 v{0}", version);
            tabPageLogin.BackgroundImage = PicLoader.Read("System", "LogBack.JPG");

            ConfigData.LoadData();
            Scene.Instance = new Scene(tabPageGame, tabPageGame.Width, tabPageGame.Height);
            SystemMenuManager.Load(tabPageGame.Width, tabPageGame.Height);
            MainTipManager.Init(tabPageGame.Height);
            DbSerializer.Init();
            WorldInfoManager.Load();

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
                textBoxName.Text = "narlon1";
                textBoxPasswd.Text = "";
                SoundManager.PlayBGM("TOM000.MP3");
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
                SoundManager.PlayBGM("TOM001.MP3");
                Scene.Instance.ChangeMap(UserProfile.InfoBasic.MapId);
                UserProfile.Profile.OnLogin();
                timeMinutes = (int) DateTime.Now.TimeOfDay.TotalMinutes;
            }
            page = pg;
            viewStack1.SelectedIndex = page;
        }

        private void MainForm_Paint(object sender, PaintEventArgs e)
        {
            try
            {
                Scene.Instance.Paint(e.Graphics, timeMinutes);
#if DEBUG
                Font font = new Font("微软雅黑", 12*1.33f, FontStyle.Bold, GraphicsUnit.Pixel);
                e.Graphics.DrawString(mouseStr, font, Brushes.White, 10, tabPageGame.Height-60);
                font.Dispose();
#endif
                SystemMenuManager.DrawAll(e.Graphics);
                MainTipManager.DrawAll(e.Graphics);
            }
            catch (Exception err)
            {
                NLog.Error(err);
            }
        }

        private void buttonLogin_Click(object sender, EventArgs e)
        {
            if (textBoxName.Text == "")
            {
                MessageBoxEx.Show("游戏账号不能为空！");
                return;
            }

            ChangePage(1);
        }

        private void MainForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (viewStack1.SelectedIndex == 1)
            {
                WorldInfoManager.Save();
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

#if DEBUG
            mouseStr = string.Format("{0},{1}", e.X - 35, e.Y - 85);
            tabPageGame.Invalidate(new Rectangle(10, tabPageGame.Height-40, 100, 30));
#endif
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
                SystemMenuManager.CheckHotKey(e.KeyCode);
            }
        }

        private void buttonExit_Click(object sender, EventArgs e)
        {
            Close();
        }

        public void AddTip(string newtip, string color)
        {
            MainTipManager.AddTip(newtip, color);
            tabPageGame.Invalidate();
        }

        public void RefreshPanel()
        {
            tabPageGame.Invalidate();
        }

        public void DealPanel(BasePanel panel)
        {
            foreach (Control control in tabPageGame.Controls)
            {
                if (control.GetType() == panel.GetType())
                {
                    RemovePanel(control as BasePanel);
                    return;
                }
            }

            if (panel.NeedBlackForm)
            {
                BlackWallForm.Instance.Init(tabPageGame.Width, tabPageGame.Height);
                tabPageGame.Controls.Add(BlackWallForm.Instance);
                BlackWallForm.Instance.BringToFront();
                SystemMenuManager.isHotkeyEnabled = false;
            }
            else
            {
                panelList.Add(panel);
            }
            panel.Init(tabPageGame.Width, tabPageGame.Height);            
            tabPageGame.Controls.Add(panel);
            panel.BringToFront();
            SystemMenuManager.formCount++;
        }

        public void RemovePanel(BasePanel panel)
        {
            if (panel.IsChangeBgm)
            {
                SoundManager.PlayLastBGM();
            }
            if (panel.NeedBlackForm)
            {
                tabPageGame.Controls.Remove(BlackWallForm.Instance);
                SystemMenuManager.isHotkeyEnabled = true;
            }
            tabPageGame.Controls.Remove(panel);
            panelList.Remove(panel);
            SystemMenuManager.formCount--;
        }

        public BasePanel FindForm(Type type)
        {
            foreach (Control control in tabPageGame.Controls)
            {
                if (control.GetType() == type)
                {
                    return control as BasePanel;
                }
            }
            return null;
        }

        public bool CloseLastPanel()
        {
            if (panelList.Count <= 0)
            {
                return false;
            }

            RemovePanel(panelList[panelList.Count-1]);

            return true;
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
                        {
                            timeTick -= 1000;
                        }
                        foreach (Control control in tabPageGame.Controls)
                        {
                            if (control is BasePanel)
                            {
                                (control as BasePanel).OnFrame(timeTick);
                            }
                        }

                        if (SystemMenuManager.isHotkeyEnabled && (timeTick % 5) == 0)
                        {
                            SystemMenuManager.UpdateAll(tabPageGame);

                            if (MainTipManager.OnFrame())
                            {
                                tabPageGame.Invalidate();
                            }

                            if (UserProfile.Profile != null)
                            {
                                int time = (int)DateTime.Now.TimeOfDay.TotalMinutes;
                                if (timeMinutes == 0 || time != timeMinutes)
                                {
                                    if (timeMinutes == 0 || (time % 60) == 0)
                                    {
                                        timeMinutes = time;
                                        if (time == 0)
                                        {
                                            UserProfile.Profile.OnNewDay();
                                        }
                                        tabPageGame.Invalidate();
                                    }
                                }
                                else
                                {
                                    tabPageGame.Invalidate(new Rectangle(10, 8, 130, 18));
                                }
                            }
                            Scene.Instance.TimeGo(timeMinutes);
                        }
                    }
                    catch (Exception e)
                    {
                        NarlonLib.Log.NLog.Fatal(e);
                        throw;
                    }
                }
                Thread.Sleep(50);
            }
        }
    }
}