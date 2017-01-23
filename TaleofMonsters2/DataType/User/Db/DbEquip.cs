using TaleofMonsters.Core;

namespace TaleofMonsters.DataType.User.Db
{
    public class DbEquip
    {
        [FieldIndex(Index = 1)] public int BaseId;
        [FieldIndex(Index = 2)] public int Dura;
        [FieldIndex(Index = 3)] public int ExpireTime;

        public DbEquip()
        {
        }

        public DbEquip(int baseId, int dura, int expire)
        {
            BaseId = baseId;
            Dura = dura;
            ExpireTime = expire;
        }

        public void Reset()
        {
            BaseId = 0;
            Dura = 0;
            ExpireTime = 0;
        }
    }
}
