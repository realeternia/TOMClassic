using System.Collections.Generic;
using TaleofMonsters.DataType;

namespace TaleofMonsters.Forms.CMain.Scenes
{
    /// <summary>
    /// 直接从配置文件读取的地图信息，禁止修改
    /// </summary>
    internal class SceneInfo
    {
        internal struct SceneScriptPosData
        {
            public int Id;
            public int X;
            public int Y;
            public int Width;
            public int Height;

            public int HiddenIndex; //1开始都是隐藏的
        }
        internal struct SceneScriptSpecialData
        {
            public int Id;
            public SceneCellTypes Type;
            public int Info;
        }

        public int Id { get; private set; }
        public int XCount { get; set; }
        public int YCount { get; set; }
        public int Xoff { get; set; }
        public int Yoff { get; set; }

        public int StartPos { get; set; } //传送后指定地点
        public int RevivePos { get; set; } //复活后重生点

        public int HiddenCellCount { get; set; } //隐藏的格子数

        public List<SceneScriptPosData> MapData { get; private set; } //随机后的结果也会到这里
        public List<SceneScriptSpecialData> SpecialData { get; private set; } //写死的npc等等

        public int CellCount { get { return MapData.Count; } }
        public int SpecialCellCount { get { return SpecialData.Count; } }

        public SceneInfo(int id)
        {
            Id = id;
            MapData = new List<SceneScriptPosData>();
            SpecialData = new List<SceneScriptSpecialData>();
        }

        public Dictionary<int, SceneScriptPosData> GetCellDict()
        {
            var dict = new Dictionary<int, SceneScriptPosData>();
            foreach (var sceneScriptPosData in MapData)
            {
                dict[sceneScriptPosData.Id] = sceneScriptPosData;
            }
            return dict;
        }
    }
}