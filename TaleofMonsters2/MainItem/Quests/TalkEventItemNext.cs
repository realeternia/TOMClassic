using System.Drawing;
using TaleofMonsters.MainItem.Quests.SceneQuests;
using TaleofMonsters.MainItem.Scenes;

namespace TaleofMonsters.MainItem.Quests
{
    internal class TalkEventItemNext : TalkEventItem
    {
        public TalkEventItemNext(int evtId, int level, Rectangle r, SceneQuestEvent e)
            : base(evtId, level, r, e)
        {
            Scene.Instance.QuestNext(evt.ParamList[0]);
        }

        public override bool AutoClose()
        {
            return true;
        }
    }
}

