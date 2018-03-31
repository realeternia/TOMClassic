using TaleofMonsters.Core;

namespace TaleofMonsters.Datas.User.Db
{
    public class DbEquipComposeData
    {
        [FieldIndex(Index = 1)] public int EquipId;//武器id
        [FieldIndex(Index = 2)] public int ItemId1;
        [FieldIndex(Index = 3)] public int ItemId2;
    }
}
