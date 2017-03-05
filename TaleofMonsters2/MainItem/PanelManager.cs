using ConfigDatas;
using TaleofMonsters.Forms;
using TaleofMonsters.Forms.MiniGame;

namespace TaleofMonsters.MainItem
{
    internal class PanelManager
    {
        public static void ShowLevelInfo(int oldLevel, int level)
        {
            if (DataType.Others.LevelInfoBook.GetLevelInfosByLevel(oldLevel, level).Length > 0)
            {
                LevelInfoForm levelInfoForm = new LevelInfoForm();
                levelInfoForm.OldLevel = oldLevel;
                levelInfoForm.Level = level;
                MainForm.Instance.DealPanel(levelInfoForm);
            }
        }

        public static void ShowGameWindow(int id)
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
            }
            if (basePanel != null)
            {
                basePanel.SetMinigameId(id);
                MainForm.Instance.DealPanel(basePanel);
            }
        }
    }
}
