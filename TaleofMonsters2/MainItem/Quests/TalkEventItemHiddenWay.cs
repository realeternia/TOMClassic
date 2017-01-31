using System.Drawing;
using TaleofMonsters.MainItem.Quests.SceneQuests;
using TaleofMonsters.MainItem.Scenes;

namespace TaleofMonsters.MainItem.Quests
{
    internal class TalkEventItemHiddenWay : TalkEventItem
    {
        public TalkEventItemHiddenWay(int evtId, int level, Rectangle r, SceneQuestEvent e)
            : base(evtId, level, r, e)
        {
            Scene.Instance.HiddenWay();
        }

        public override bool AutoClose()
        {
            return true;
        }
    }
}

