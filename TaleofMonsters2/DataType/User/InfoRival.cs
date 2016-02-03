using System.Collections.Generic;
using System.IO;
using TaleofMonsters.Core;
using TaleofMonsters.DataType.Achieves;
using TaleofMonsters.DataType.Peoples;

namespace TaleofMonsters.DataType.User
{
    public class InfoRival
    {
        [FieldIndex(Index = 1)]
        public Dictionary<int, RivalState> Rivals ;

        public InfoRival()
        {
            Rivals = new Dictionary<int, RivalState>();
        }

        public RivalState GetRivalState(int id)
        {
            if (!Rivals.ContainsKey(id))
            {
                return new RivalState(id);
            }
            return Rivals[id];
        }

        public void AddRivalState(int id, bool isWin)
        {
            if (PeopleBook.IsPeople(id))//打怪不记录战绩
            {
                if (!Rivals.ContainsKey(id))
                {
                    Rivals[id] = new RivalState(id);
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
                Rivals[id] = new RivalState(id);
            }

            Rivals[id].Avail = true;
            AchieveBook.CheckByCheckType("people");
        }

        public int GetRivalAvailCount()
        {
            int count = 0;
            foreach (RivalState state in Rivals.Values)
            {
                if (state.Avail)
                    count++;
            }
            return count;
        }
    }
}
