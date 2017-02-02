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
                MainForm.Instance.DealPanel(new BuyPieceForm());
            }
            else if (e.ParamList[0] == "changecard")
            {
                MainForm.Instance.DealPanel(new ChangeCardForm());
            }
            else if (e.ParamList[0] == "selectjob")
            {
                MainForm.Instance.DealPanel(new SelectJobForm());
            }
            else if (e.ParamList[0] == "merge")
            {
                MainForm.Instance.DealPanel(new MergeWeaponForm());
            }
            else if (e.ParamList[0] == "farm")
            {
                MainForm.Instance.DealPanel(new FarmForm());
            }
            else if (e.ParamList[0] == "bless")
            {
                MainForm.Instance.DealPanel(new BlessForm());
            }
        }

        public override void OnFrame(int tick)
        {
            RunningState = TalkEventState.Finish;
        }
    }
}

