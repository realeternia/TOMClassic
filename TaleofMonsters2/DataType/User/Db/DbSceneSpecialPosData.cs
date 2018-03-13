using TaleofMonsters.Core;

namespace TaleofMonsters.DataType.User.Db
{
    public class DbSceneSpecialPosData
    {
        [FieldIndex(Index = 4)] public int Id;
        [FieldIndex(Index = 1)] public byte Type; //SceneCellTypes
        [FieldIndex(Index = 3)] public bool Disabled;
        [FieldIndex(Index = 6)] public int Info;
        [FieldIndex(Index = 8)] public bool MapSetting;
        [FieldIndex(Index = 9)] public uint Flag;
    }
}