using TaleofMonsters.Core;

namespace TaleofMonsters.DataType.User.Db
{
    public class DbSceneSpecialPosData
    {
        [FieldIndex(Index = 4)] public int Id;
        [FieldIndex(Index = 1)] public string Type;
        [FieldIndex(Index = 3)] public bool Disabled;
        [FieldIndex(Index = 6)] public int Info;
        [FieldIndex(Index = 7)] public int Info2;
        [FieldIndex(Index = 8)] public bool MapSetting;
    }
}