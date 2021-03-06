﻿using System;
using System.Drawing;
using TaleofMonsters.Datas;
using TaleofMonsters.Datas.Others;
using TaleofMonsters.Datas.Peoples;
using TaleofMonsters.Datas.Scenes;
using TaleofMonsters.Datas.User;
using TaleofMonsters.Forms.CMain.Blesses;
using TaleofMonsters.Forms.CMain.Quests.SceneQuests;
using TaleofMonsters.Forms.CMain.Scenes;

namespace TaleofMonsters.Forms.CMain.Quests
{
    internal class TalkEventItemAction : TalkEventItem
    {
        private int cellId;

        public TalkEventItemAction(int cellId, int evtId, int level, Rectangle r, SceneQuestEvent e)
            : base(evtId, level, r, e)
        {
            this.cellId = cellId;
        }

        public override void Init()
        {
            base.Init();
            switch (evt.Type)
            {
                case "reset": Scene.Instance.ResetScene(); break;
                case "ruin": Scene.Instance.Ruin(); break;
                case "teleport": Scene.Instance.EnableTeleport(); break;
                case "portal": Scene.Instance.RandomPortal(); break;
                case "moveback": Scene.Instance.MoveTo(UserProfile.InfoBasic.LastPosition); break;
                case "movestart": Scene.Instance.MoveTo(Scene.Instance.SceneInfo.GetStartPos()); break;
                case "hiddenway": Scene.Instance.HiddenWay(); break;
                case "next":
                    foreach (var parm in config.NextQuest) //支持多个next同时触发
                        Scene.Instance.QuestNext(parm); break;
                case "hide":
                    foreach (var parm in config.HiddenRoomQuest) //如果地图不支持，就当啥都没发生
                        Scene.Instance.OpenHidden(parm); break;
                case "changemap":
                    Scene.Instance.ChangeMap(config.SceneId, true);
                    Scene.Instance.MoveTo(Scene.Instance.SceneInfo.GetStartPos());
                    break;
                case "detect": Scene.Instance.DetectNear(int.Parse(evt.ParamList[0])); break;
                case "detectrd": Scene.Instance.DetectRandom(int.Parse(evt.ParamList[0])); break;
                case "disable": Scene.Instance.GetObjectByPos(cellId).SetEnable(false); break;
                case "quest": UserProfile.InfoQuest.SetQuestState(int.Parse(evt.ParamList[0]), QuestStates.Receive); break;
                case "questp": UserProfile.InfoQuest.AddQuestProgress(int.Parse(evt.ParamList[0]), byte.Parse(evt.ParamList[1])); break;
                case "removeditem": var itemId = DungeonBook.GetDungeonItemId(config.NeedDungeonItemId);
                    UserProfile.InfoDungeon.RemoveDungeonItem(itemId, config.NeedDungeonItemCount); break;
                case "bribe":
                    int fightLevel = Math.Max(1, level + BlessManager.FightLevelChange);
                    var cost = GameResourceBook.OutCarbuncleBribe(UserProfile.InfoBasic.Level, fightLevel);
                    UserProfile.InfoBag.SubResource(GameResourceType.Carbuncle, cost); break;
                case "unlock":
                    UserProfile.InfoRival.SetRivalAvail(PeopleBook.GetPeopleId(config.EnemyName)); break;
            }

            if (evt.Children.Count > 0)
                result = evt.Children[0];//应该是一个say
        }

        public override bool AutoClose()
        {
            return result == null; //没有后续就自动关闭
        }
    }
}

