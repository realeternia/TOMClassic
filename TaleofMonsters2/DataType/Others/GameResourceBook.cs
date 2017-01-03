using System;

namespace TaleofMonsters.DataType.Others
{
    internal static class GameResourceBook
    {
        /// <summary>
        /// 战斗获得金币
        /// </summary>
        public static uint InGoldFight(int level, bool isPeople)
        {
            return isPeople ? (uint)level : (uint)level * 3 + 12;
        }
        /// <summary>
        /// 场景剧情获得金币
        /// </summary>
        public static uint InGoldSceneQuest(int level, int rate)
        {
            return (uint)((level*5 + 10)*rate/100);
        }
        /// <summary>
        /// 场景剧情获得食物
        /// </summary>
        public static uint InFoodSceneQuest(int rate)
        {
            return (uint)(5 * rate / 100);
        }
        /// <summary>
        /// 场景剧情获得经验
        /// </summary>
        public static uint InExpSceneQuest(int level, int rate)
        {
            return (uint)(5 * rate / 100);//todo
        }
        /// <summary>
        /// 战斗获得经验值
        /// </summary>
        public static uint InExpFight(int level, int rLevel)
        {
            return (uint)(ExpTree.GetNextRequired(rLevel) / 2 / (15 + Math.Abs(level - rLevel) * 3) + 1);
        }
        /// <summary>
        /// 购买卡牌消耗Gem
        /// </summary>
        public static uint OutGemCardBuy(int qual)
        {
            return (uint)(qual * (qual + 1) * Math.Sqrt(qual)) * 2;
        }
        /// <summary>
        /// 分解卡牌获得Gem
        /// </summary>
        public static uint OutGemCardDecompose(int qual)
        {
            return (uint)(qual * (qual + 1) * Math.Sqrt(qual));
        }
        /// <summary>
        /// 消耗石材制作装备
        /// </summary>
        public static uint OutStoneMerge(int qual, int level)
        {
            return (uint)((float)level * 5 * Math.Sqrt(qual));
        }
        /// <summary>
        /// 分解装备获得石材
        /// </summary>
        public static uint InStoneEquipDecompose(int qual, int level)
        {
            return Math.Max(1, (uint)((float)level * 2 * Math.Sqrt(qual)));
        }
    }
}
