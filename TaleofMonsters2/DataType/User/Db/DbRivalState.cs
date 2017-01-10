using TaleofMonsters.Core;

namespace TaleofMonsters.DataType.User.Db
{
    public class DbRivalState
    {
         [FieldIndex(Index = 1)]
        public int Pid;
         [FieldIndex(Index = 2)]
        public int Win;
         [FieldIndex(Index = 3)]
        public int Loss;
         [FieldIndex(Index = 4)]
        public bool Avail;

        public DbRivalState()
        {
            
        }

        public DbRivalState(int perid)
        {
            Pid = perid;
            Win = 0;
            Loss = 0;
            Avail = false;
        }
    }
}
