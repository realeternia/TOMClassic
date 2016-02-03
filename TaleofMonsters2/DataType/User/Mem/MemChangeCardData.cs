using TaleofMonsters.Core;
using TaleofMonsters.Core.Interface;

namespace TaleofMonsters.DataType.User.Mem
{
    public class MemChangeCardData
    {
         [FieldIndex(Index = 1)]
        public int Id1;
         [FieldIndex(Index = 2)]
        public int Type1;
         [FieldIndex(Index = 3)]
        public int Id2;
         [FieldIndex(Index = 4)]
        public int Type2;
         [FieldIndex(Index = 5)]
        public bool Used;

        public bool IsEmpty()
        {
            return Id1 == 0 && Id2 == 0;
        }
    }
}
