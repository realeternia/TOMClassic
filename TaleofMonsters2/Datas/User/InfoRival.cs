using System.Collections.Generic;
using TaleofMonsters.Core;
using TaleofMonsters.Datas.Peoples;
using TaleofMonsters.Datas.User.Db;

namespace TaleofMonsters.Datas.User
{
    public class InfoRival
    {
        [FieldIndex(Index = 1)] public Dictionary<int, DbRivalState> Rivals;

        public InfoRival()
        {
            Rivals = new Dictionary<int, DbRivalState>();
        }

        private DbRivalState GetRivalState(int id)
        {
            DbRivalState result = null;
            if (Rivals.TryGetValue(id, out result))
                return result;
            return new DbRivalState(id);
        }

        public void AddRivalState(int id, bool isWin)
        {
            if (PeopleBook.IsPeople(id))//打怪不记录战绩
            {
                DbRivalState result = null;
                if (!Rivals.TryGetValue(id, out result))
                {
                    result = new DbRivalState(id);
                    Rivals[id] = result;
                }

                if (isWin)
                    result.Win++;
                else
                    result.Loss++;
            }
        }

        public void SetRivalAvail(int id)
        {
            DbRivalState state;
            if (!Rivals.TryGetValue(id, out state))
                Rivals[id] = new DbRivalState(id) {Avail = true};
            else
                state.Avail = true;
        }

        public bool GetRivalAvail(int id)
        {
            return GetRivalState(id).Avail;
        }
        public int GetRivalWin(int id)
        {
            return GetRivalState(id).Win;
        }
    }
}
