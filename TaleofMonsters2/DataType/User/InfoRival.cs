using System.Collections.Generic;
using TaleofMonsters.Core;
using TaleofMonsters.DataType.Achieves;
using TaleofMonsters.DataType.Peoples;
using TaleofMonsters.DataType.User.Db;

namespace TaleofMonsters.DataType.User
{
    public class InfoRival
    {
        [FieldIndex(Index = 1)] public Dictionary<int, DbRivalState> Rivals;

        public InfoRival()
        {
            Rivals = new Dictionary<int, DbRivalState>();
        }

        public DbRivalState GetRivalState(int id)
        {
            if (!Rivals.ContainsKey(id))
            {
                return new DbRivalState(id);
            }
            return Rivals[id];
        }

        public void AddRivalState(int id, bool isWin)
        {
            if (PeopleBook.IsPeople(id))//打怪不记录战绩
            {
                if (!Rivals.ContainsKey(id))
                {
                    Rivals[id] = new DbRivalState(id);
                }
                if (isWin)
                {
                    Rivals[id].Win++;
                }
                else
                {
                    Rivals[id].Loss++;
                }
            }

            AchieveBook.CheckByCheckType("fight");
        }

        public void SetRivalAvail(int id)
        {
            if (!Rivals.ContainsKey(id))
            {
                Rivals[id] = new DbRivalState(id);
            }

            Rivals[id].Avail = true;
            AchieveBook.CheckByCheckType("people");
        }

        public int GetRivalAvailCount()
        {
            int count = 0;
            foreach (var state in Rivals.Values)
            {
                if (state.Avail)
                    count++;
            }
            return count;
        }
    }
}
