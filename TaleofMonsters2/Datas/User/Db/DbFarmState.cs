using TaleofMonsters.Core;

namespace TaleofMonsters.Datas.User.Db
{
    public class DbFarmState
    {
        [FieldIndex(Index = 1)] public int Type;
        [FieldIndex(Index = 2)] public int Ep; //当前的能量
        [FieldIndex(Index = 3)] public int EpNeed; //当前的能量

        public DbFarmState()
        {
        }

        public DbFarmState(int type)
        {
            Type = type;
            Ep = 0;
        }

        public override string ToString()
        {
            return string.Format("{0}-{1}", Type, Ep);
        }
    }
}
