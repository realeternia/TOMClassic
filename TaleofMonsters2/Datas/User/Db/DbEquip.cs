using TaleofMonsters.Core;

namespace TaleofMonsters.Datas.User.Db
{
    public class DbEquip
    {
        [FieldIndex(Index = 1)] public int BaseId;
        [FieldIndex(Index = 2)] public int Level;
        [FieldIndex(Index = 3)] public int Exp;

        public DbEquip()
        {
        }

        public void Reset()
        {
            BaseId = 0;
            Level = 0;
            Exp = 0;
        }

        public override string ToString()
        {
            return string.Format("{0}-{1}", BaseId, Level);
        }
    }
}
