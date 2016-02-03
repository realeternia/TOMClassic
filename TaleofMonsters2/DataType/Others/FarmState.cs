using TaleofMonsters.Core;
using TaleofMonsters.Core.Interface;

namespace TaleofMonsters.DataType.Others
{
    public class FarmState
    {
        [FieldIndex(Index = 1)]
        public int Type;
        [FieldIndex(Index = 2)]
        public int Time;

        public FarmState()
        {
            
        }

        public FarmState(int type, int time)
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
