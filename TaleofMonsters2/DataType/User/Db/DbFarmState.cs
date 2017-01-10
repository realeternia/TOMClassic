using TaleofMonsters.Core;

namespace TaleofMonsters.DataType.User.Db
{
    public class DbFarmState
    {
        [FieldIndex(Index = 1)]
        public int Type;
        [FieldIndex(Index = 2)]
        public int Time;

        public DbFarmState()
        {
            
        }

        public DbFarmState(int type, int time)
        {
            this.Type = type;
            this.Time = time;
        }

        public override string ToString()
        {
            return string.Format("{0}-{1}", Type, Time);
        }
    }
}
