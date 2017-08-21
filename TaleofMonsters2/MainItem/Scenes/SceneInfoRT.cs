using System.Collections.Generic;
using NarlonLib.Math;
using TaleofMonsters.MainItem.Scenes.SceneObjects;

namespace TaleofMonsters.MainItem.Scenes
{
    /// <summary>
    /// sceneinfo runtime , 运行时的数据
    /// </summary>
    internal class SceneInfoRT
    {
        public SceneInfo Script { get; set; }

        public List<SceneObject> Items { get; private set; } //场景中的物件，各种npc等

        public SceneInfoRT()
        {
            Items = new List<SceneObject>();
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
            return Items[MathTool.GetRandom(Items.Count)].Id; //随机给一个
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
            return Items[MathTool.GetRandom(Items.Count)].Id; //随机给一个
        }
    }
}