using System.Collections.Generic;
using TaleofMonsters.Core;

namespace TaleofMonsters.DataType.User.Db
{
    public class DbCardProduct
    {
        [FieldIndex(Index = 1)] public int Id;
        [FieldIndex(Index = 2)] public int Cid;
        [FieldIndex(Index = 3)] public int Mark; //CardProductMarkTypes

        public DbCardProduct()
        {

        }

        public DbCardProduct(int id, int cid, int mark)
        {
            Id = id;
            Cid = cid;
            Mark = mark;
        }

        public override string ToString()
        {
            return string.Format("id={0}", Cid);
        }
    }

    internal class CompareByMark : IComparer<DbCardProduct>
    {
        #region IComparer<DbCardProduct> 成员

        public int Compare(DbCardProduct x, DbCardProduct y)
        {
            if (x.Mark != y.Mark)
            {
                if (x.Mark == 0)
                {
                    return 1;
                }
                if (y.Mark == 0)
                {
                    return -1;
                }
                return x.Mark.CompareTo(y.Mark);
            }
            return (x.Cid.CompareTo(y.Cid));
        }

        #endregion
    }
}
