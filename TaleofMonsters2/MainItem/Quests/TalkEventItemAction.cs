using System.Drawing;
using TaleofMonsters.DataType;
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
                case "next": foreach (var parm in config.NextQuest) //支持多个next同时触发
                        Scene.Instance.QuestNext(parm); break;
                case "changemap":
                    Scene.Instance.ChangeMap(config.SceneId, true);
                    UserProfile.InfoBasic.Position = Scene.Instance.GetStartPos(); //如果没配置了出生点，就随机一个点
                    break;
                case "detect": Scene.Instance.DetectNear(int.Parse(evt.ParamList[0])); break;
                case "detectrd": Scene.Instance.DetectRandom(int.Parse(evt.ParamList[0])); break;
                case "quest": UserProfile.InfoQuest.SetQuestState(int.Parse(evt.ParamList[0]), QuestStates.Receive); break;
                case "questp": UserProfile.InfoQuest.AddQuestProgress(int.Parse(evt.ParamList[0]), byte.Parse(evt.ParamList[1])); break;
            }
        }

        public override bool AutoClose()
        {
            return true;
        }
    }
}

