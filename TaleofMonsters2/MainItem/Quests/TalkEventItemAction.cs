using System.Drawing;
using TaleofMonsters.DataType.User;
using TaleofMonsters.MainItem.Quests.SceneQuests;
using TaleofMonsters.MainItem.Scenes;

namespace TaleofMonsters.MainItem.Quests
{
    internal class TalkEventItemAction : TalkEventItem
    {
        public TalkEventItemAction(int evtId, int level, Rectangle r, SceneQuestEvent e)
            : base(evtId, level, r, e)
        {
            switch (e.Type)
            {
                case "reset": Scene.Instance.ResetScene(); break;
                case "teleport": Scene.Instance.EnableTeleport(); break;
                case "portal": Scene.Instance.RandomPortal(); break;
                case "move": Scene.Instance.MoveTo(config.Position); break;
                case "hiddenway": Scene.Instance.HiddenWay(); break;
                case "next": Scene.Instance.QuestNext(evt.ParamList[0]); break;
                case "changemap":
                    Scene.Instance.ChangeMap(config.SceneId, true);
                    UserProfile.InfoBasic.Position = Scene.Instance.GetStartPos(); //如果没配置了出生点，就随机一个点
                    break;
            }
        }

        public override bool AutoClose()
        {
            return true;
        }
    }
}

