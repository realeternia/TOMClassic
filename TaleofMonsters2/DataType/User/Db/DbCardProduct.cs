using System.Collections.Generic;
using NarlonLib.Math;
using TaleofMonsters.Config;
using TaleofMonsters.Core;
using TaleofMonsters.DataType.Others;

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

        public GameResource Price
        {
            get
            {
                var cardData = CardConfigManager.GetCardConfig(Cid);

                GameResource res = new GameResource();
                res.Gold = cardData.Star*30;
                var markType = (CardProductMarkTypes) Mark;
                if (markType == CardProductMarkTypes.Sale)
                {
                    res.Gold = MathTool.GetRound(res.Gold, 20);
                }
                else if (markType == CardProductMarkTypes.Gold)
                {
                    res.Gold = res.Gold*12/10;
                }
                else if (markType == CardProductMarkTypes.Hot)
                {
                    res.Gold = res.Gold*8/10;
                }
                else if (markType == CardProductMarkTypes.Only)
                {
                    res.Gold = 300;
                }
                int qual = cardData.Quality + 1;
                res.Add(GameResourceType.Gem, (int) GameResourceBook.OutGemCardBuy(qual));
                return res;
            }
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
