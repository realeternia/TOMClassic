using System.Collections.Generic;
using ConfigDatas;
using TaleofMonsters.DataType;
using TaleofMonsters.DataType.User;

namespace TaleofMonsters.MainItem.Blesses
{
    internal static class BlessManager
    {
        internal delegate void BlessUpdateMethod();

        private static BlessConfig cache;

        public static BlessUpdateMethod Update = null;
        
        public static void OnChangeMap()
        {
            RebuildCache();
        }

        public static void AddBless(int id, int time = 0)
        {
            if (UserProfile.InfoWorld.Blesses.Count >= 10) //最大10个bless
                return;
            var blessConfig = ConfigData.GetBlessConfig(id);
            if (time == 0)
                time = blessConfig.Round;

            if (UserProfile.InfoWorld.Blesses.ContainsKey(id))
                UserProfile.InfoWorld.Blesses[id] += time;
            else
                UserProfile.InfoWorld.Blesses[id] = time;

            if (blessConfig.Type == (int)BlessTypes.Negative)
                UserProfile.InfoRecord.AddRecordById((int)MemPlayerRecordTypes.AddCurse, 1);
            else
                UserProfile.InfoRecord.AddRecordById((int)MemPlayerRecordTypes.AddBless, 1);

            if (Update != null)
                Update();
            RebuildCache();
        }

        public static void RemoveBless(int id)
        {
            UserProfile.InfoWorld.Blesses.Remove(id);
            if (Update != null)
                Update();
            RebuildCache();
        }

        public static List<int> GetNegtiveBless()
        {
            List<int> datas = new List<int>();
            foreach (var bless in UserProfile.InfoWorld.Blesses)
            {
                var blessConfig = ConfigData.GetBlessConfig(bless.Key);
                if (blessConfig.Type == (int)BlessTypes.Negative)
                    datas.Add(blessConfig.Id);
            }
            return datas;
        }

        private static void RebuildCache()
        {
            cache = new BlessConfig();
            foreach (var key in UserProfile.InfoWorld.Blesses.Keys)
            {
                var config = ConfigData.GetBlessConfig(key);
                cache.MoveFoodChange += config.MoveFoodChange;
                cache.MoveDistance += config.MoveDistance;
                cache.MoveCostHp |= config.MoveCostHp;
                cache.MoveSameCostFood |= config.MoveSameCostFood;
                cache.PunishFoodMulti += config.PunishFoodMulti;
                cache.PunishGoldMulti += config.PunishGoldMulti;
                cache.PunishHealthMulti += config.PunishHealthMulti;
                cache.PunishMentalMulti += config.PunishMentalMulti;
                cache.RewardResMulti += config.RewardResMulti;
                cache.RewardExpMulti += config.RewardExpMulti;
                cache.RewardFoodMulti += config.RewardFoodMulti;
                cache.RewardGoldMulti += config.RewardGoldMulti;
                cache.RewardHealthMulti += config.RewardHealthMulti;
                cache.RewardMentalMulti += config.RewardMentalMulti;
                cache.RollWinAddGold += config.RollWinAddGold;
                cache.RollFailSubHealth += config.RollFailSubHealth;
                cache.FightLevelChange += config.FightLevelChange;
                cache.FightWinAddExp += config.FightWinAddExp;
                cache.FightWinAddHealth += config.FightWinAddHealth;
                cache.FightFailSubHealth += config.FightFailSubHealth;
                cache.FightFailSubMental += config.FightFailSubMental;
                cache.RollAlwaysFailBig |= config.RollAlwaysFailBig;
                cache.RollAlwaysWinBig |= config.RollAlwaysWinBig;
                cache.TradeAddRate += config.TradeAddRate;
                cache.TradeNeedRate += config.TradeNeedRate;
                cache.TestAdjustPointTwo += config.TestAdjustPointTwo;
                cache.TestBallChange += config.TestBallChange;
                cache.TestBallSynchonize |= config.TestBallSynchonize;
            }
        }

        public static void OnMove()
        {
            Dictionary<int, int> replace = new Dictionary<int, int>();
            int count = UserProfile.InfoWorld.Blesses.Count;
            foreach (var bless in UserProfile.InfoWorld.Blesses)
            {
                if (bless.Value > 1)
                    replace[bless.Key] = bless.Value-1;
            }
            UserProfile.InfoWorld.Blesses = replace;
            if (count != UserProfile.InfoWorld.Blesses.Count)
            {
                if (Update != null)
                    Update();
                RebuildCache();
            }
        }

        public static int MoveFoodChange
        {
            get { return cache.MoveFoodChange; }
        }
        public static int MoveDistance
        {
            get { return cache.MoveDistance; }
        }
        public static bool MoveCostHp
        {
            get { return cache.MoveCostHp; }
        }
        public static bool MoveSameCostFood
        {
            get { return cache.MoveSameCostFood; }
        }
        public static int PunishFoodMulti
        {
            get { return cache.PunishFoodMulti; }
        }
        public static int PunishGoldMulti
        {
            get { return cache.PunishGoldMulti; }
        }
        public static int PunishHealthMulti
        {
            get { return cache.PunishHealthMulti; }
        }
        public static int PunishMentalMulti
        {
            get { return cache.PunishMentalMulti; }
        }
        public static int RewardExpMulti
        {
            get { return cache.RewardExpMulti; }
        }
        public static int RewardFoodMulti
        {
            get { return cache.RewardFoodMulti; }
        }
        public static int RewardGoldMulti
        {
            get { return cache.RewardGoldMulti; }
        }
        public static int RewardHealthMulti
        {
            get { return cache.RewardHealthMulti; }
        }
        public static int RewardMentalMulti
        {
            get { return cache.RewardMentalMulti; }
        }
        public static int RewardResMulti
        {
            get { return cache.RewardResMulti; }
        }
        public static int RollWinAddGold
        {
            get { return cache.RollWinAddGold; }
        }
        public static int RollFailSubHealth
        {
            get { return cache.RollFailSubHealth; }
        }
        public static int FightLevelChange
        {
            get { return cache.FightLevelChange; }
        }

        public static int FightWinAddExp
        {
            get { return cache.FightWinAddExp; }
        }
        public static int FightWinAddHealth
        {
            get { return cache.FightWinAddHealth; }
        }
        public static int FightFailSubHealth
        {
            get { return cache.FightFailSubHealth; }
        }
        public static int FightFailSubMental
        {
            get { return cache.FightFailSubMental; }
        }
        public static bool RollAlwaysWinBig
        {
            get { return cache.RollAlwaysWinBig; }
        }
        public static bool RollAlwaysFailBig
        {
            get { return cache.RollAlwaysFailBig; }
        }
        public static double TradeNeedRate
        {
            get { return cache.TradeNeedRate; }
        }
        public static double TradeAddRate
        {
            get { return cache.TradeAddRate; }
        }
        public static int TestAdjustPointTwo
        {
            get { return cache.TestAdjustPointTwo; }
        }
        public static int TestBallChange
        {
            get { return cache.TestBallChange; }
        }
        public static bool TestBallSynchonize
        {
            get { return cache.TestBallSynchonize; }
        }
    }
}
