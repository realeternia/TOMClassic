using System.Drawing;
using TaleofMonsters.Core;
using TaleofMonsters.DataType.Peoples;
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
          
        }

        public override void OnFrame(int tick)
        {
            RunningState = TalkEventState.Finish;
        }
    }
}

