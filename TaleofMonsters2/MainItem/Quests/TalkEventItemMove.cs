using System.Drawing;
using TaleofMonsters.MainItem.Quests.SceneQuests;
using TaleofMonsters.MainItem.Scenes;

namespace TaleofMonsters.MainItem.Quests
{
    internal class TalkEventItemMove : TalkEventItem
    {
        public TalkEventItemMove(int evtId, int level, Rectangle r, SceneQuestEvent e)
            : base(evtId, level, r, e)
        {
            Scene.Instance.MoveTo(config.Position);
        }

        public override bool AutoClose()
        {
            return true;
        }
    }
}

