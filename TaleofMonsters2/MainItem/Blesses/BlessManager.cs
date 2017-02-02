using System;
using System.Collections.Generic;
using System.Drawing;
using ConfigDatas;
using TaleofMonsters.DataType.User;

namespace TaleofMonsters.MainItem.Blesses
{
    internal static class BlessManager
    {
        internal delegate void BlessUpdateMethod();

        private static BlessConfig cache;

        public static BlessUpdateMethod Update = null;

        public static void Init()
        {
            RebuildCache();
        }

        public static void AddBless(int id, int time)
        {
            UserProfile.InfoWorld.Blesses[id] = time;
            if (Update != null)
            {
                Update();
            }
            RebuildCache();
        }

        public static void RemoveBless(int id)
        {
            UserProfile.InfoWorld.Blesses.Remove(id);
            if (Update != null)
            {
                Update();
            }
            RebuildCache();
        }

        private static void RebuildCache()
        {
            cache = new BlessConfig();
            foreach (var key in UserProfile.InfoWorld.Blesses.Keys)
            {
                var config = ConfigData.GetBlessConfig(key);
                cache.MoveFoodChange += config.MoveFoodChange;
                cache.PunishFoodMulti += config.PunishFoodMulti;
                cache.PunishGoldMulti += config.PunishGoldMulti;
                cache.PunishHealthMulti += config.PunishHealthMulti;
                cache.PunishMentalMulti += config.PunishMentalMulti;
                cache.RewardExpMulti += config.RewardExpMulti;
                cache.RewardFoodMulti += config.RewardFoodMulti;
                cache.RewardGoldMulti += config.RewardGoldMulti;
                cache.RewardHealthMulti += config.RewardHealthMulti;
                cache.RewardMentalMulti += config.RewardMentalMulti;
                cache.RollWinAddGold += config.RollWinAddGold;
                cache.RollFailSubHealth += config.RollFailSubHealth;
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
                {
                    Update();
                }
            }
        }

        public static int SceneMove
        {
            get { return cache.MoveFoodChange; }
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
        public static int RollWinAddGold
        {
            get { return cache.RollWinAddGold; }
        }
        public static int RollFailSubHealth
        {
            get { return cache.RollFailSubHealth; }
        }

        public static Image GetBlessImage(int key)
        {
            var config = ConfigData.GetBlessConfig(key);
            var lastTime = 0;
            if (UserProfile.InfoWorld.Blesses.ContainsKey(key))
                lastTime = UserProfile.InfoWorld.Blesses[key];
            ControlPlus.TipImage tipData = new ControlPlus.TipImage();
            tipData.AddTextNewLine(config.Name, config.Type == 1? "Green":"Red", 20);
            tipData.AddLine(2);
            tipData.AddTextNewLine(config.Descript, "White");
            tipData.AddTextNewLine(string.Format("剩余回合{0}", lastTime), "White");
            return tipData.Image;
        }
    }
}
