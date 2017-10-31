using ConfigDatas;
using TaleofMonsters.Core;

namespace TaleofMonsters.DataType.User
{
    public class InfoDungeon
    {
        [FieldIndex(Index = 1)] public int DungeonId; //副本id

        [FieldIndex(Index = 2)] public int Str; //力量
        [FieldIndex(Index = 3)] public int Agi; //敏捷
        [FieldIndex(Index = 4)] public int Intl; //智慧
        [FieldIndex(Index = 5)] public int Perc; //感知
        [FieldIndex(Index = 6)] public int Endu; //耐力

        public InfoDungeon()
        {

        }

        public void Enter(int dungeonId) //进入副本需要初始化
        {
            DungeonId = dungeonId;
            var dungeonConfig = ConfigData.GetDungeonConfig(dungeonId);
            Str = dungeonConfig.Str;
            Agi = dungeonConfig.Agi;
            Intl = dungeonConfig.Intl;
            Perc = dungeonConfig.Perc;
            Endu = dungeonConfig.Endu;
        }

        public void Leave()
        {
            DungeonId = 0;
        }
    }
}
