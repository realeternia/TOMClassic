using System.Collections.Generic;
using TaleofMonsters.DataType.User.Db;
using TaleofMonsters.MainItem.Scenes.SceneObjects;

namespace TaleofMonsters.MainItem.Scenes
{
    internal class SceneInfo
    {
        public int Id { get; private set; }
        public int XCount { get; set; }
        public int YCount { get; set; }
        public int Xoff { get; set; }
        public int Yoff { get; set; }

        public int StartPos { get; set; } //传送后指定地点
        public int RevivePos { get; set; } //复活后重生点

        public List<SceneObject> Items { get; private set; } //场景中的物件，各种npc等

        public int HiddenCellCount { get; set; } //隐藏的格子数

        public List<SceneManager.ScenePosData> MapData { get; private set; }
        public List<DbSceneSpecialPosData> SpecialData { get; private set; }

        public SceneInfo(int id)
        {
            Id = id;
            Items = new List<SceneObject>();
            MapData = new List<SceneManager.ScenePosData>();
            SpecialData = new List<DbSceneSpecialPosData>();
        }

        
    }
}