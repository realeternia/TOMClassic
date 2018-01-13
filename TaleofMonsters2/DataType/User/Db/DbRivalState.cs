using System.Collections.Generic;
using ConfigDatas;
using TaleofMonsters.Core;

namespace TaleofMonsters.DataType.User.Db
{
    public class DbRivalState
    {
        [FieldIndex(Index = 1)] public int Pid;
        [FieldIndex(Index = 2)] public int Win;
        [FieldIndex(Index = 3)] public int Loss;
        [FieldIndex(Index = 4)] public bool Avail;

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


        internal class CompareByQuality : IComparer<DbRivalState>
        {
            #region IComparer<DbRivalState> 成员

            public int Compare(DbRivalState x, DbRivalState y)
            {
                if (x.Pid == 0)
                    return 1;
                if (y.Pid == 0)
                    return -1;
                var xPeople = ConfigData.GetPeopleConfig(x.Pid);
                var yPeople = ConfigData.GetPeopleConfig(y.Pid);
                if (xPeople.Quality != yPeople.Quality)
                    return xPeople.Quality.CompareTo(yPeople.Quality);
                return xPeople.Level.CompareTo(yPeople.Level);
            }

            #endregion
        }
    }
}
