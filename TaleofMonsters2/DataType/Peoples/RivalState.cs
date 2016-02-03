using TaleofMonsters.Core;

namespace TaleofMonsters.DataType.Peoples
{
    public class RivalState
    {
         [FieldIndex(Index = 1)]
        public int Pid;
         [FieldIndex(Index = 2)]
        public int Win;
         [FieldIndex(Index = 3)]
        public int Loss;
         [FieldIndex(Index = 4)]
        public bool Avail;

        public RivalState()
        {
            
        }

        public RivalState(int perid)
        {
            Pid = perid;
            Win = 0;
            Loss = 0;
            Avail = false;
        }
    }
}
