using System.Drawing;
using TaleofMonsters.Forms.CMain.Quests.SceneQuests;

namespace TaleofMonsters.Forms.CMain.Quests
{
    internal class TalkEventItemEnd : TalkEventItem
    {
        public TalkEventItemEnd(int evtId, int level, Rectangle r, SceneQuestEvent e)
            : base(evtId, level, r, e)
        {
        }

        public override bool AutoClose()
        {
            return true;
        }
    }
}

