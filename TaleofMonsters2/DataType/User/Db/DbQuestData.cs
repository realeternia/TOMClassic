using TaleofMonsters.Core;

namespace TaleofMonsters.DataType.User.Db
{
    public class DbQuestData
    {
        [FieldIndex(Index = 1)] public int QuestId;
        [FieldIndex(Index = 2)] public byte State;

        public DbQuestData()
        {
        }
    }
}
