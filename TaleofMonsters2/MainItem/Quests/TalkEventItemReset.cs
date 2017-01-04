using System.Drawing;
using TaleofMonsters.MainItem.Quests.SceneQuests;
using TaleofMonsters.MainItem.Scenes;

namespace TaleofMonsters.MainItem.Quests
{
    internal class TalkEventItemReset : TalkEventItem
    {
        public TalkEventItemReset(int evtId, Rectangle r, SceneQuestEvent e)
            : base(evtId, r, e)
        {
            Scene.Instance.ResetScene();
        }

        public override bool AutoClose()
        {
            return true;
        }
    }
}

