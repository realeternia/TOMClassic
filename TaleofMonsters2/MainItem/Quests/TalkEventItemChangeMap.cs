using System.Drawing;
using TaleofMonsters.DataType.User;
using TaleofMonsters.MainItem.Quests.SceneQuests;
using TaleofMonsters.MainItem.Scenes;

namespace TaleofMonsters.MainItem.Quests
{
    internal class TalkEventItemChangeMap : TalkEventItem
    {
        public TalkEventItemChangeMap(int evtId, int level, Rectangle r, SceneQuestEvent e)
            : base(evtId, level, r, e)
        {
            Scene.Instance.ChangeMap(config.SceneId, true);
            UserProfile.InfoBasic.Position = Scene.Instance.GetStartPos(); //如果没配置了出生点，就随机一个点
        }

        public override bool AutoClose()
        {
            return true;
        }
    }
}

