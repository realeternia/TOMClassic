using System.Collections.Generic;
using ConfigDatas;
using NarlonLib.Math;
using TaleofMonsters.Datas;
using TaleofMonsters.Datas.Scenes;
using TaleofMonsters.Datas.User;
using TaleofMonsters.Datas.User.Db;
using TaleofMonsters.Forms.CMain.Scenes.SceneObjects;

namespace TaleofMonsters.Forms.CMain.Scenes
{
    /// <summary>
    /// sceneinfo runtime , 运行时的数据
    /// </summary>
    internal class SceneInfoRT
    {
        public SceneInfo Script { get; set; }

        private int hiddenIndex = 1; //已经触发的隐藏级别
        public List<SceneObject> Items { get; private set; } //场景中的物件，各种npc等

        public SceneInfoRT()
        {
            Items = new List<SceneObject>();
        }

        public void AddCellInitial(SceneInfo.SceneScriptPosData cellConfigData, DbSceneSpecialPosData specialPosData, int id, SceneFreshReason reason)
        {
            SceneObject so;
            if (cellConfigData.Id > 0)
            {
                switch (specialPosData.Type)
                {
                    case (byte)SceneCellTypes.Quest:
                        so = new SceneQuest(specialPosData.Id, cellConfigData.X, cellConfigData.Y, cellConfigData.Width, cellConfigData.Height, specialPosData.Info);
                        so.Disabled = specialPosData.Disabled; break;
                    case (byte)SceneCellTypes.Warp:
                        so = new SceneWarp(specialPosData.Id, cellConfigData.X, cellConfigData.Y, cellConfigData.Width, cellConfigData.Height, specialPosData.Info);
                        so.Disabled = specialPosData.Disabled;
                        if (ConfigData.GetSceneConfig(id).Type == (int)SceneTypes.Common && reason == SceneFreshReason.Warp)
                        {
                            specialPosData.Disabled = true;
                            so.Disabled = true;//如果是切场景，切到战斗场景，所有传送门自动关闭
                        }
                        break;
                    default:
                        so = new SceneTile(specialPosData.Id, cellConfigData.X, cellConfigData.Y, cellConfigData.Width, cellConfigData.Height); break;
                }
                so.Flag = specialPosData.Flag;
                so.MapSetting = specialPosData.MapSetting;
            }
            else
            {
                so = new SceneTile(specialPosData.Id, cellConfigData.X, cellConfigData.Y, cellConfigData.Width, cellConfigData.Height);
                so.Disabled = true;
                //throw new Exception("RefreshSceneObjects error");
            }
            Items.Add(so);
        }

        public void ReplaceCellQuest(int cellId, string eventName)
        {
            for (int i = 0; i < Items.Count; i++)
            {
                if (Items[i].Id != cellId)
                    continue;

                var oldCell = Items[i];
                int qId = SceneQuestBook.GetSceneQuestByName(eventName);
                Items[i] = new SceneQuest(oldCell.Id, oldCell.X, oldCell.Y, oldCell.Width, oldCell.Height, qId);
                Items[i].MapSetting = true;

                UserProfile.InfoWorld.UpdatePosInfo(cellId, qId);
                UserProfile.InfoWorld.UpdatePosMapSetting(cellId, true);
            }
        }

        public int GetStartPos()
        {
            foreach (var sceneObject in Items)
            {
                if (sceneObject.Id == Script.StartPos)
                {
                    return sceneObject.Id;
                }
            }
            return GetRandom(0, false); //随机给一个
        }

        public int GetRevivePos()
        {
            foreach (var sceneObject in Items)
            {
                if (sceneObject.Id == Script.RevivePos)
                {
                    return sceneObject.Id;
                }
            }
            return GetRandom(0, false); //随机给一个
        }

        public int GetRandom(int fromId, bool checkEvent)
        {
            while (true)
            {
                int index = MathTool.GetRandom(Items.Count);
                var targetCell = Items[index];
                if (targetCell.Id == fromId)
                    continue;
                if (checkEvent && !targetCell.CanBeReplaced())
                    continue;

                return targetCell.Id;
            }
        }

        public int FindCell(int fromId, string qname)
        {
            foreach (var sceneObject in Items)
            {
                if (sceneObject is SceneQuest)
                {
                    var config = ConfigData.GetSceneQuestConfig((sceneObject as SceneQuest).EventId);
                    if (config.Ename == qname && sceneObject.Id != fromId)
                    {
                        return sceneObject.Id;
                    }
                }
            }
            return 0;
        }

        public void OpenHidden(string eventName)
        {//按序触发隐藏
            var cellList = Script.MapData.FindAll(cell => cell.HiddenIndex == hiddenIndex);
            if (cellList.Count <= 0)
                return;

            foreach (var sceneScriptData in cellList)
            {
                SceneObject questData;
                DbSceneSpecialPosData specialPos;
                if (string.IsNullOrEmpty(eventName))
                {
                    questData = new SceneTile(sceneScriptData.Id, sceneScriptData.X, sceneScriptData.Y, sceneScriptData.Width, sceneScriptData.Height);
                    specialPos = new DbSceneSpecialPosData
                    {
                        Id = sceneScriptData.Id,
                        Type = (byte)SceneCellTypes.Tile,
                        MapSetting = true,
                        Flag = (uint)SceneObject.ScenePosFlagType.Hidden
                    };
                }
                else
                {
                    int qId = SceneQuestBook.GetSceneQuestByName(eventName);
                    questData = new SceneQuest(sceneScriptData.Id, sceneScriptData.X, sceneScriptData.Y, sceneScriptData.Width, sceneScriptData.Height, qId);
                    specialPos = new DbSceneSpecialPosData
                    {
                        Id = sceneScriptData.Id,
                        Info = qId,
                        Type = (byte)SceneCellTypes.Quest,
                        MapSetting = true,
                        Flag = (uint)SceneObject.ScenePosFlagType.Hidden
                    };
                }
                questData.MapSetting = true;
                questData.AddFlag(SceneObject.ScenePosFlagType.Hidden);
                Items.Add(questData);
                UserProfile.InfoWorld.AddPos(specialPos);
            }

            hiddenIndex++;
        }
    }
}