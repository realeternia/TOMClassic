using System.Drawing;
using TaleofMonsters.MainItem.Quests.SceneQuests;

namespace TaleofMonsters.MainItem.Quests
{
    internal class TalkEventItemEnd : TalkEventItem
    {
        public TalkEventItemEnd(int evtId, Rectangle r, SceneQuestEvent e)
            : base(evtId, r, e)
        {
        }

        public override bool AutoClose()
        {
            return true;
        }
    }
}

