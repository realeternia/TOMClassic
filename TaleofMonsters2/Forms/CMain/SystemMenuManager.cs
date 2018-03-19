using System.Collections.Generic;
using System.Windows.Forms;
using ConfigDatas;
using ControlPlus;
using TaleofMonsters.Controler.GM;
using TaleofMonsters.Datas.Others;
using TaleofMonsters.Datas.User;
using TaleofMonsters.Forms.CMain.Scenes;
using TaleofMonsters.Forms.Items.Core;
using TaleofMonsters.Forms.MagicBook;

namespace TaleofMonsters.Forms.CMain 
{
    internal static class SystemMenuManager
    {
        private static List<ToolBarItemData> menuItemList;
        private static List<ToolBarItemData> activeItemList;
        private static List<RiverFlow> flowList;

        public static bool IsHotkeyEnabled { get; set; }

        public static int MenuTar { get; private set; }

        public static bool GMMode { get; private set; }

        static SystemMenuManager()
        {
            IsHotkeyEnabled = true;
            MenuTar = -1;
        }

        public static void Load(int width, int height)
        {
            flowList = new List<RiverFlow>();
            flowList.Add(new RiverFlow(width-138, height-55, 50, 50, 4, IconDirections.RightToLeft));
       //     flowList.Add(new RiverFlow(10, 50, 50, 50, 5, IconDirections.LeftToRight)); bless system conflict
            flowList.Add(new RiverFlow(width-54, 200, 50, 50, 5, IconDirections.UpToDown));

            menuItemList = new List<ToolBarItemData>();
            foreach (var mainIconConfig in ConfigData.MainIconDict.Values)
                menuItemList.Add(new ToolBarItemData(mainIconConfig.Id, width, height));
        }

        private static void Reload()
        {
            foreach (var riverFlow in flowList)
            {
                riverFlow.Reset();
            }

            activeItemList = new List<ToolBarItemData>();
            foreach (var toolBarItemData in menuItemList)
            {
                int itemFlow = toolBarItemData.MainIconConfig.Flow;
                if (!toolBarItemData.Enable || itemFlow == -1)
                    continue;

                if (UserProfile.InfoBasic.Level < toolBarItemData.MainIconConfig.Level)
                    continue;

                if (!toolBarItemData.MainIconConfig.ShowInDungeon && UserProfile.InfoDungeon.DungeonId > 0 || 
                    !toolBarItemData.MainIconConfig.ShowInScene && UserProfile.InfoDungeon.DungeonId <= 0)
                    continue;

                if (itemFlow > 0)
                {
                    System.Drawing.Point newPoint = flowList[itemFlow - 1].GetNextPosition();
                    toolBarItemData.SetSize(newPoint.X, newPoint.Y, flowList[itemFlow - 1].Width, flowList[itemFlow - 1].Height);
                }
               
                activeItemList.Add(toolBarItemData);
            }
        }

        public static bool UpdateToolbar(int mouseX, int mouseY)
        {
            foreach (var item in activeItemList)
            {
                if (item.InRegion(mouseX, mouseY))
                {
                    if (item.Id != MenuTar)
                    {
                        MenuTar = item.Id;
                        return true;
                    }
                    return false;
                }
            }
            if (MenuTar != -1)
            {
                MenuTar = -1;
                return true;
            }
            return false;
        }

        public static void ResetIconState()
        {
            foreach (var toolBarItemData in menuItemList)
            {
                if (toolBarItemData.Id >= 1000)
                    toolBarItemData.Enable = false;
            }

            Reload();
        }

        public static void UpdateAll(Control parent)
         {
             foreach (var item in activeItemList)
                 item.Update(parent);
         }

        public static void DrawAll(System.Drawing.Graphics g)
        {
            foreach (var item in activeItemList)
                item.Draw(g, MenuTar);
        }

        public static void CheckItemClick(SystemMenuIds id)
        {
            foreach (var toolBarItemData in activeItemList)
            {
                if ((SystemMenuIds)toolBarItemData.Id == id && toolBarItemData.InCD)
                    return;
            }

            switch (id)
            {
                case SystemMenuIds.SystemMenu:
                    PanelManager.DealPanel(new SystemMenu());
                    break;
                case SystemMenuIds.UserForm:
                    PanelManager.DealPanel(new UserForm());
                    break;
                case SystemMenuIds.LeaveDungeon:
                    if (MessageBoxEx2.Show("确定离开副本?") == DialogResult.OK)
                        Scene.Instance.LeaveDungeon();
                    break;
                case SystemMenuIds.ItemForm:
                    PanelManager.DealPanel(new ItemForm());
                    break;
                case SystemMenuIds.PeopleViewForm:
                    PanelManager.DealPanel(new PeopleViewForm());
                    break;
                case SystemMenuIds.DeckViewForm:
                    PanelManager.DealPanel(new DeckViewForm());
                    break;
                case SystemMenuIds.DeckViewDungeonForm:
                    PanelManager.DealPanel(new DeckDungeonViewForm());
                    break;
                case SystemMenuIds.GameShopViewForm:
                    PanelManager.DealPanel(new GameShopViewForm());
                    break;
                case SystemMenuIds.TaskForm:
                    PanelManager.DealPanel(new QuestForm());
                    break;
                case SystemMenuIds.EquipmentForm:
                    PanelManager.DealPanel(new CastleForm());
                    break;
                case SystemMenuIds.EquipComposeForm:
                    PanelManager.DealPanel(new EquipComposeForm());
                    break;
                case SystemMenuIds.MagicBookViewForm:
                    PanelManager.DealPanel(new MagicBookViewForm());
                    break;
                case SystemMenuIds.WorldMapViewForm:
                    PanelManager.DealPanel(new WorldMapViewForm());
                    break;
                case SystemMenuIds.RankForm:
                    PanelManager.DealPanel(new RankForm());
                    break;
                case SystemMenuIds.CardShopViewForm:
                    PanelManager.DealPanel(new CardShopViewForm());
                    break;
                case SystemMenuIds.TournamentViewForm:
                    PanelManager.DealPanel(new TournamentViewForm());
                    break;
                case SystemMenuIds.TreasureWheelForm:
                    TreasureWheelBook.Show(1);
                    break;
                case SystemMenuIds.DailyCardForm:
                    PanelManager.DealPanel(new DailyCardForm());
                    break;
                case SystemMenuIds.QuestionForm:
                    PanelManager.DealPanel(new QuestionForm());
                    break;
                case SystemMenuIds.MinigameForm:
                    PanelManager.DealPanel(new MinigameForm());
                    break;
                case SystemMenuIds.StoryForm:
                    PanelManager.DealPanel(new StoryForm());
                    break;
            }
        }

        public static void CheckHotKey(KeyEventArgs e)
        {
            if (!IsHotkeyEnabled)
                return;

            if (GMMode && e.KeyCode != Keys.Oemtilde)
            {
                GMCodeZone.OnKeyDown(e);
                return;
            }

            switch (e.KeyCode)
            {
                case Keys.Escape:
                    if (!PanelManager.CloseLastPanel())//如果已经没有面板了，才出
                        CheckItemClick(SystemMenuIds.SystemMenu);
                    break;
                case Keys.Oemtilde:
                    GMMode = !GMMode;
                    MainForm.Instance.RefreshView();
                    break;
            }
        }
    }

    internal enum SystemMenuIds
    {
        SystemMenu = 1,
        GameShopViewForm = 2,
        UserForm = 3,
        TaskForm = 6,
        PeopleViewForm = 7,
        DeckViewForm = 8,
        DeckViewDungeonForm = 9,
        EquipComposeForm = 10,
        ItemForm = 11,
        EquipmentForm = 12,
        LeaveDungeon = 31,
        MagicBookViewForm = 32,
        WorldMapViewForm = 33,
        RankForm = 34,
        CardShopViewForm = 36,
        TournamentViewForm = 37,
        TreasureWheelForm = 39,
        DailyCardForm = 40,
        QuestionForm = 41,
        MinigameForm = 42,
        StoryForm = 43,
        GameUpToNumber = 1100,
        GameIconsCatch = 1101,
        GameBattleRobot = 1102,
        GameThreeBody = 1103,
        GameSeven = 1104,
        GameRussiaBlock = 1105,
        GameLink = 1106,
        GameOvercome = 1107,
        GameRacing = 1108,
        GameVoting = 1109,
        GameTwentyOne = 1110,
    }
}
