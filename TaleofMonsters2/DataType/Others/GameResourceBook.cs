using System;
using NarlonLib.Math;

namespace TaleofMonsters.DataType.Others
{
    internal static class GameResourceBook
    {
        private const int GoldFactor = 4;

        /// <summary>
        /// 出售道具所得
        /// </summary>
        public static uint InGoldSellItem(int rare, int rate)
        {
            int[] rareArray = {1, 4, 8, 14, 22, 36, 60, 100};
            
            return (uint)(rareArray[rare] * GoldFactor * rate / 100);
        } 
        /// <summary>
        /// 购买道具付出
        /// </summary>
        public static uint OutGoldSellItem(int rare, int rate)
        {
            int[] rareArray = { 1, 4, 8, 14, 22, 36, 60, 100 };

            return (uint)(rareArray[rare] * GoldFactor * rate / 100) * 2;
        }
        /// <summary>
        /// 战斗获得金币
        /// </summary>
        public static uint InGoldFight(int level, bool isPeople)
        {
            uint get = Math.Max(1, (uint) (ExpTree.GetResourceFactor(level)*GoldFactor/2));
            return isPeople ? get : get/2;
        }
        /// <summary>
        /// 场景剧情获得金币
        /// </summary>
        public static uint InGoldSceneQuest(int level, int rate, bool noRandom = false)
        {
            if (rate <= 0)
                return 0;
            double[] factor = new[] {0.2, 0.5, 0.5, 1, 1, 1.5, 2.2};
            rate = (int) (rate*(noRandom ? 1 : factor[MathTool.GetRandom(factor.Length)]));
            return Math.Max(1, (uint)(ExpTree.GetResourceFactor(level)*GoldFactor*rate/100));
        }
        /// <summary>
        /// 场景剧情消耗金币
        /// </summary>
        public static uint OutGoldSceneQuest(int level, int rate, bool noRandom = false)
        {
            if (rate <= 0)
                return 0;
            double[] factor = new[] { 0.5, 1, 1, 1.5};
            rate = (int) (rate*(noRandom ? 1 : factor[MathTool.GetRandom(factor.Length)]))*3/4;
            return Math.Max(1, (uint)(ExpTree.GetResourceFactor(level) * GoldFactor * rate / 100));
        }
        /// <summary>
        /// 消耗金钱制作装备图纸,level 1-5
        /// </summary>
        public static uint OutGoldMerge(int qual)
        {
            return (uint)((float)2 * 2 * Math.Sqrt(qual))*10;//2-5-10-16
        }
        /// <summary>
        /// 场景剧情获得食物
        /// </summary>
        public static uint InFoodSceneQuest(int rate, bool noRandom = false)
        {
            double[] factor = new[] { 0.5, 1, 1, 1.5 };
            rate = (int)(rate * (noRandom ? 1 : factor[MathTool.GetRandom(factor.Length)]));
            return (uint)(10 * rate / 100);
        }
        /// <summary>
        /// 场景剧情消耗食物
        /// </summary>
        public static uint OutFoodSceneQuest(int rate, bool noRandom = false)
        {
            double[] factor = new[] { 0.5, 1, 1, 1.5 };
            rate = (int)(rate * (noRandom ? 1 : factor[MathTool.GetRandom(factor.Length)]));
            return (uint)(8 * rate / 100);
        }
        /// <summary>
        /// 场景剧情获得健康
        /// </summary>
        public static uint InHealthSceneQuest(int rate, bool noRandom = false)
        {
            double[] factor = new[] { 0.5, 0.5, 1, 1, 2 };
            rate = (int)(rate * (noRandom ? 1 : factor[MathTool.GetRandom(factor.Length)]));
            return (uint)(10 * rate / 100);
        }
        /// <summary>
        /// 场景剧情消耗健康
        /// </summary>
        public static uint OutHealthSceneQuest(int rate, bool noRandom = false)
        {
            double[] factor = new[] { 0.5, 0.5, 1, 1, 2 };
            rate = (int)(rate * (noRandom ? 1 : factor[MathTool.GetRandom(factor.Length)]));
            return (uint)(10 * rate / 100);
        }
        /// <summary>
        /// 场景剧情获得精神
        /// </summary>
        public static uint InMentalSceneQuest(int rate, bool noRandom = false)
        {
            double[] factor = new[] { 0.2, 0.2, 0.5, 0.5, 1, 1, 2, 3 };
            rate = (int)(rate * (noRandom ? 1 : factor[MathTool.GetRandom(factor.Length)]));
            return (uint)(15 * rate / 100);
        }
        /// <summary>
        /// 场景剧情消耗精神
        /// </summary>
        public static uint OutMentalSceneQuest(int rate, bool noRandom = false)
        {
            double[] factor = new[] { 0.2, 0.2, 0.5, 0.5, 1, 1, 2, 3 };
            rate = (int)(rate * (noRandom ? 1 : factor[MathTool.GetRandom(factor.Length)]));
            return (uint)(15 * rate / 100);
        }
        /// <summary>
        /// 场景剧情获得经验
        /// </summary>
        public static uint InExpSceneQuest(int level, int rate)
        {
            if (rate > 0)
            {
                return Math.Max(1, (uint)(ExpTree.GetNextRequired(level) / 12 * rate / 100));
            }
            return 0;
        }
        /// <summary>
        /// 战斗获得经验值
        /// </summary>
        public static uint InExpFight(int level, int rLevel)
        {
            return Math.Max(1, (uint)(ExpTree.GetNextRequired(rLevel) / 2 / (15 + Math.Abs(level - rLevel) * 3) + 1));
        }
        /// <summary>
        /// 消耗石材制作装备,level 1-5
        /// </summary>
        public static uint OutStoneMerge(int qual)
        {
            return (uint)((float)2 * 2 * Math.Sqrt(qual));//2-5-10-16
        }
        /// <summary>
        /// 分解装备获得石材
        /// </summary>
        public static uint InStoneEquipDecompose(int qual)
        {
            return Math.Max(1, (uint)(2 * Math.Sqrt(qual) * 0.7));//2-5-10-16 0.7
        }
        /// <summary>
        /// 消耗木材建设农场
        /// </summary>
        public static uint OutWoodBuildFarm(uint cost)
        {
            return cost;
        }

        /// <summary>
        /// 购买武器卡牌消耗Gem
        /// </summary>
        public static uint OutGemCardBuy(int qual)
        {
            return (uint)(qual * (qual + 1) * Math.Sqrt(qual)) * 2; //2-8-20-40
        }
        /// <summary>
        /// 购买怪物卡牌消耗Carbuncle
        /// </summary>
        public static uint OutCarbuncleCardBuy(int qual)
        {
            return (uint)(qual * (qual + 1) * Math.Sqrt(qual)) * 2; //2-8-20-40
        }
        /// <summary>
        /// 战斗中贿赂怪物消耗Carbuncle
        /// </summary>
        public static uint OutCarbuncleBribe(int myLevel, int level)
        {
            int levelDiffer = level - myLevel;
            if (levelDiffer < -5)
                return 3;
            if (levelDiffer <= 0)
                return 5;
            return (uint)(levelDiffer * (levelDiffer + 1)) + 5;
        }
        /// <summary>
        /// 消耗水银购买祝福
        /// </summary>
        public static uint OutMercuryBlessBuy(int level)
        {
            return (uint)(3*level);
        }
        /// <summary>
        /// 购买法术卡牌消耗Mercury
        /// </summary>
        public static uint OutMercuryCardBuy(int qual)
        {
            return (uint)(qual * (qual + 1) * Math.Sqrt(qual)) * 2; //2-8-20-40
        }
    }
}
