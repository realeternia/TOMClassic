using TaleofMonsters.Core;

namespace TaleofMonsters.Datas.User.Db
{
    public class DbChessData
    {
        [FieldIndex(Index = 1)] public int PeopleId;
        [FieldIndex(Index = 2)] public int Pos;
        [FieldIndex(Index = 3)] public uint MeetCount; //遇见次数
    }
}