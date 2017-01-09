using System.Drawing;
using TaleofMonsters.MainItem.Quests.SceneQuests;
using TaleofMonsters.MainItem.Scenes;

namespace TaleofMonsters.MainItem.Quests
{
    internal class TalkEventItemTeleportEnable : TalkEventItem
    {
        public TalkEventItemTeleportEnable(int evtId, int level, Rectangle r, SceneQuestEvent e)
            : base(evtId, level, r, e)
        {
            Scene.Instance.EnableTeleport();
        }

        public override bool AutoClose()
        {
            return true;
        }
    }
}

