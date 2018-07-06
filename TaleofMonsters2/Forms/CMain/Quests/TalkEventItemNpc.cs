using System.Drawing;
using TaleofMonsters.Datas.Others;
using TaleofMonsters.Forms.CMain.Quests.SceneQuests;
using TaleofMonsters.Forms.Items;

namespace TaleofMonsters.Forms.CMain.Quests
{
    internal class TalkEventItemNpc : TalkEventItem
    {
        private bool autoClose;
        public TalkEventItemNpc(int evtId, int level, Rectangle r, SceneQuestEvent e)
            : base(evtId, level, r, e)
        {
            autoClose = true;
        }

        public override void Init()
        {
            if (evt.ParamList[0] == "buypiece")
            {
                PanelManager.DealPanel(new BuyPieceForm());
            }
            else if (evt.ParamList[0] == "changecard")
            {
                PanelManager.DealPanel(new ChangeCardForm());
            }
            else if (evt.ParamList[0] == "changeres")
            {
                PanelManager.DealPanel(new ChangeResForm());
            }
            else if (evt.ParamList[0] == "selectjob")
            {
                autoClose = false;
                PanelManager.DealPanel(new SelectJobForm());
            }
            else if (evt.ParamList[0] == "merge")
            {
                PanelManager.DealPanel(new MergeWeaponForm());
            }
            else if (evt.ParamList[0] == "bless")
            {
                PanelManager.DealPanel(new BlessForm());
            }
            else if (evt.ParamList[0] == "shop")
            {
                var shop = new NpcShopForm();
                shop.ShopName = config.ShopName;
                PanelManager.DealPanel(shop);
            }
            else if (evt.ParamList[0] == "dungeon")
            {
                var dungeonForm = new DungeonForm();
                dungeonForm.DungeonId = config.DungeonId;
                PanelManager.DealPanel(dungeonForm);
            }
            else if (evt.ParamList[0] == "dcardremove")
            {
                var dungeonForm = new DungeonCardSelectViewForm();
                dungeonForm.Mode = DungeonCardItem.CardCopeMode.Remove;
                PanelManager.DealPanel(dungeonForm);
            }
            else if (evt.ParamList[0] == "dcardup")
            {
                var dungeonForm = new DungeonCardSelectViewForm();
                dungeonForm.Mode = DungeonCardItem.CardCopeMode.Upgrade;
                PanelManager.DealPanel(dungeonForm);
            }

            inited = true;
        }

        public override bool AutoClose()
        {
            return autoClose;
        }
    }
}

