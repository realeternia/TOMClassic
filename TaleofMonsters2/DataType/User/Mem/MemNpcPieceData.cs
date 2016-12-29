using TaleofMonsters.Core;

namespace TaleofMonsters.DataType.User.Mem
{
    public class MemNpcPieceData
    {
        [FieldIndex(Index = 1)] public int Id;
        [FieldIndex(Index = 2)] public int Count;
        [FieldIndex(Index = 3)] public bool Used;

        public bool IsEmpty()
        {
            return Id == 0;
        }
    }
}
