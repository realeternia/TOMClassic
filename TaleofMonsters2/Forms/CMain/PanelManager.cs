using System;
using System.Collections.Generic;
using System.Windows.Forms;
using ConfigDatas;
using TaleofMonsters.Core;
using TaleofMonsters.Forms.MiniGame;

namespace TaleofMonsters.Forms.CMain
{
    internal class PanelManager
    {
        private static List<BasePanel> panelList = new List<BasePanel>(); //受到esc影响关闭的面板，都在这里
        public static int PanelCount;

        private static int formWidth;
        private static int formHeight;

        public static void Init(int wid, int height)
        {
            formWidth = wid;
            formHeight = height;
        }

        public static void DealPanel(BasePanel panel)
        {
            var type = FindPanel(panel.GetType());
            if (type != null)
            {
                RemovePanel(type);
                return;
            }

            SoundManager.Play("System", "OpenForm.mp3");
            AddPanel(panel);
        }

        private static void AddPanel(BasePanel panel)
        {
            if (panel.NeedBlackForm)
            {
                BlackWallForm.Instance.Init(formWidth, formHeight);
                MainForm.Instance.AddPanelAct(BlackWallForm.Instance);
            }
            else
            {
                panelList.Add(panel);
            }
            panel.Init(formWidth, formHeight);
            MainForm.Instance.AddPanelAct(panel);
            PanelCount++;
        }

        public static void RemovePanel(BasePanel panel)
        {
            var type = FindPanel(panel.GetType());
            if(type == null)
                return;

            if (panel.IsChangeBgm)
            {
                SoundManager.PlayLastBGM();
            }
            if (panel.NeedBlackForm)
            {
                BlackWallForm.Instance.OnRemove();
                MainForm.Instance.RemovePanelAct(BlackWallForm.Instance);
            }
            panel.OnRemove();
            MainForm.Instance.RemovePanelAct(panel);
            panelList.Remove(panel);
            PanelCount--;
        }

        public static BasePanel FindPanel(Type type)
        {
            return MainForm.Instance.FindPanelAct(type);
        }

        public static bool CloseLastPanel()
        {
            if (panelList.Count <= 0)
                return false;

            RemovePanel(panelList[panelList.Count - 1]);

            return true;
        }

        public static void CheckHotKey(KeyEventArgs e, bool isUp)
        {
            if (isUp)
            {
                foreach (var basePanel in panelList)
                    basePanel.OnHsKeyUp(e);
            }
            else
            {
                foreach (var basePanel in panelList)
                    basePanel.OnHsKeyDown(e);
            }
        }

        public static void ShowLevelInfo(int oldLevel, int level)
        {
            LevelInfoForm levelInfoForm = new LevelInfoForm
            {
                OldLevel = oldLevel,
                Level = level
            };
            DealPanel(levelInfoForm);
        }

        public static void ShowGameWindow(int id, int hardness, MGBase.MinigameResultCallback winCallback)
        {
            var config = ConfigData.GetMinigameConfig(id);
            MGBase basePanel = null;
            switch ((SystemMenuIds)config.WindowId)
            {
                case SystemMenuIds.GameUpToNumber:
                    basePanel = new MGUpToNumber();break;
                case SystemMenuIds.GameBattleRobot:
                    basePanel = new MGBattleRobot();break;
                case SystemMenuIds.GameIconsCatch:
                    basePanel = new MGIconsCatch();break;
                case SystemMenuIds.GameThreeBody:
                    basePanel =new MGThreeBody(); break;
                case SystemMenuIds.GameSeven:
                    basePanel = new MGSeven(); break;
                case SystemMenuIds.GameRussiaBlock:
                    basePanel = new MGRussiaBlock(); break;
                case SystemMenuIds.GameLink:
                    basePanel = new MGLinkGame(); break;
                case SystemMenuIds.GameOvercome:
                    basePanel = new MGOvercome(); break;
                case SystemMenuIds.GameRacing:
                    basePanel = new MGRacing(); break;
                case SystemMenuIds.GameVoting:
                    basePanel = new MGVoting(); break;
                case SystemMenuIds.GameTwentyOne:
                    basePanel = new MGTwentyOne(); break;
            }
            if (basePanel != null)
            {
                basePanel.SetMinigameId(id, hardness);
                basePanel.SetEvent(winCallback);
                DealPanel(basePanel);
            }
        }
    }
}
