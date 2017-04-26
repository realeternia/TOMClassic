using System.Drawing;
using TaleofMonsters.Forms;
using TaleofMonsters.MainItem.Quests.SceneQuests;

namespace TaleofMonsters.MainItem.Quests
{
    internal class TalkEventItemNpc : TalkEventItem
    {
        public TalkEventItemNpc(int evtId, int level, Rectangle r, SceneQuestEvent e)
            : base(evtId, level, r, e)
        {
            if (e.ParamList[0] == "buypiece")
            {
                PanelManager.DealPanel(new BuyPieceForm());
            }
            else if (e.ParamList[0] == "changecard")
            {
                PanelManager.DealPanel(new ChangeCardForm());
            }
            else if (e.ParamList[0] == "selectjob")
            {
                PanelManager.DealPanel(new SelectJobForm());
            }
            else if (e.ParamList[0] == "merge")
            {
                PanelManager.DealPanel(new MergeWeaponForm());
            }
            else if (e.ParamList[0] == "farm")
            {
                PanelManager.DealPanel(new FarmForm());
            }
            else if (e.ParamList[0] == "bless")
            {
                PanelManager.DealPanel(new BlessForm());
            }
        }

        public override void OnFrame(int tick)
        {
            RunningState = TalkEventState.Finish;
        }
    }
}

