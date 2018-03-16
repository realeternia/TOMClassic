using TaleofMonsters.Core;

namespace TaleofMonsters.Datas.User.Db
{
    public class DbQuestData
    {
        [FieldIndex(Index = 1)] public int QuestId;
        [FieldIndex(Index = 2)] public byte State;
        [FieldIndex(Index = 3)] public byte Progress;

        public DbQuestData()
        {
        }
    }
}
