using TaleofMonsters.Forms;
using TaleofMonsters.Forms.MiniGame;

namespace TaleofMonsters.MainItem
{
    internal class PanelManager
    {
        public static void ShowLevelUp()
        {
            LevelUpForm levelUpForm = new LevelUpForm();
            MainForm.Instance.DealPanel(levelUpForm);
        }

        public static void ShowLevelInfo(int level)
        {
            if(DataType.Others.LevelInfoBook.GetLevelInfosByLevel(level).Length>0)
            {
                LevelInfoForm levelInfoForm = new LevelInfoForm();
                levelInfoForm.Level = level;
                MainForm.Instance.DealPanel(levelInfoForm);
            }
        }

        public static void ShowGameWindow(SystemMenuIds wid)
        {
            switch (wid)
            {
                case SystemMenuIds.GameUpToNumber:
                    MainForm.Instance.DealPanel(new MGUpToNumber());break;
                case SystemMenuIds.GameBattleRobot:
                    MainForm.Instance.DealPanel(new MGBattleRobot());break;
                case SystemMenuIds.GameIconsCatch:
                    MainForm.Instance.DealPanel(new MGIconsCatch());break;
                case SystemMenuIds.GameThreeBody:
                    MainForm.Instance.DealPanel(new MGThreeBody()); break;
                case SystemMenuIds.GameSeven:
                    MainForm.Instance.DealPanel(new MGSeven()); break;
            }
        }
    }
}
