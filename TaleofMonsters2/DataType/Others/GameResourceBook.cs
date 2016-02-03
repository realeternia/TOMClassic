using System;

namespace TaleofMonsters.DataType.Others
{
    static class GameResourceBook
    {
        public static int GetGemCardBuy(int qual)
        {
            return (int)(qual * (qual + 1) * Math.Sqrt(qual)) * 2;
        }

        public static int GetGemCardDecompose(int qual)
        {
            return (int)(qual * (qual + 1) * Math.Sqrt(qual));
        }

        public static int GetMercuryMerge(int qual, int level)
        {
            return (int)((float)level * 1.5 * Math.Sqrt(qual));
        }

        public static int GetMercuryEquipDecompose(int qual, int level)
        {
            return Math.Max(1, (int)((float)level * 1 * Math.Sqrt(qual)));
        }
    }
}
