using System;

namespace TaleofMonsters.DataType.Others
{
    static class GameResourceBook
    {
        /// <summary>
        /// 购买卡牌消耗Gem
        /// </summary>
        public static uint GetGemCardBuy(int qual)
        {
            return (uint)(qual * (qual + 1) * Math.Sqrt(qual)) * 2;
        }
        /// <summary>
        /// 分解卡牌获得Gem
        /// </summary>
        public static uint GetGemCardDecompose(int qual)
        {
            return (uint)(qual * (qual + 1) * Math.Sqrt(qual));
        }
        /// <summary>
        /// 消耗石材制作装备
        /// </summary>
        public static uint GetStoneMerge(int qual, int level)
        {
            return (uint)((float)level * 5 * Math.Sqrt(qual));
        }
        /// <summary>
        /// 分解装备获得石材
        /// </summary>
        public static uint GetStoneEquipDecompose(int qual, int level)
        {
            return Math.Max(1, (uint)((float)level * 2 * Math.Sqrt(qual)));
        }
    }
}
