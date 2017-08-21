using System.Collections.Generic;
using TaleofMonsters.DataType.User.Db;
using TaleofMonsters.MainItem.Scenes.SceneObjects;

namespace TaleofMonsters.MainItem.Scenes
{
    /// <summary>
    /// 直接从配置文件读取的地图信息，禁止修改
    /// </summary>
    internal class SceneInfo
    {
        public int Id { get; private set; }
        public int XCount { get; set; }
        public int YCount { get; set; }
        public int Xoff { get; set; }
        public int Yoff { get; set; }

        public int StartPos { get; set; } //传送后指定地点
        public int RevivePos { get; set; } //复活后重生点

        public int HiddenCellCount { get; set; } //隐藏的格子数

        public List<SceneManager.ScenePosData> MapData { get; private set; }
        public List<DbSceneSpecialPosData> SpecialData { get; private set; }

        public SceneInfo(int id)
        {
            Id = id;
            MapData = new List<SceneManager.ScenePosData>();
            SpecialData = new List<DbSceneSpecialPosData>();
        }

        
    }
}