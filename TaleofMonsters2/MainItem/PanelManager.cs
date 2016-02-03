using TaleofMonsters.DataType.Peoples;
using TaleofMonsters.DataType.User;
using TaleofMonsters.Forms;
using TaleofMonsters.Forms.MiniGame;

namespace TaleofMonsters.MainItem
{
    internal class PanelManager
    {
        private const int peopleBazi = 10001;

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
                if (level == 3)
                {
                    RivalState rival = new RivalState(peopleBazi);
                    rival.Avail = true;
                    UserProfile.InfoRival.Rivals[peopleBazi] = rival;
                }
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
