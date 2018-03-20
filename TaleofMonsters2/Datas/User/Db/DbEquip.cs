using TaleofMonsters.Core;

namespace TaleofMonsters.Datas.User.Db
{
    public class DbEquip
    {
        [FieldIndex(Index = 1)] public int BaseId;
        [FieldIndex(Index = 2)] public int Level;

        public DbEquip()
        {
        }

        public void Reset()
        {
            BaseId = 0;
            Level = 0;
        }
    }
}
