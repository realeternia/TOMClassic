using System.Drawing;
using TaleofMonsters.MainItem.Quests.SceneQuests;

namespace TaleofMonsters.MainItem.Quests
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

