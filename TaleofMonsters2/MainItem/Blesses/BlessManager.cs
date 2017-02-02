using System;
using System.Collections.Generic;
using System.Drawing;
using ConfigDatas;
using NarlonLib.Math;
using TaleofMonsters.DataType.User;

namespace TaleofMonsters.MainItem.Blesses
{
    internal static class BlessManager
    {
        internal delegate void BlessUpdateMethod();

        private static BlessConfig cache;

        public static BlessUpdateMethod Update = null;

        private static Dictionary<int, List<int>> activeBlessDict = new Dictionary<int, List<int>>();
        private static Dictionary<int, List<int>> negativeBlessDict = new Dictionary<int, List<int>>();

        static BlessManager()
        {
            for (int i = 0; i < 4; i++)
            {
                activeBlessDict[i] = new List<int>();
                negativeBlessDict[i] = new List<int>();
            }
            foreach (var blessConfig in ConfigData.BlessDict.Values)
            {
                if (blessConfig.Type == 1)
                    activeBlessDict[blessConfig.Level].Add(blessConfig.Id);
                else
                    negativeBlessDict[blessConfig.Level].Add(blessConfig.Id);
            }
        }

        public static void OnChangeMap()
        {
            RebuildCache();
        }

        public static void AddBless(int id, int time)
        {
            if (UserProfile.InfoWorld.Blesses.Count >= 10) //最大10个bless
                return;
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

        public static int GetRandomBlessLevel(bool isActive, int level)
        {
            List<int> toCheck;
            if (isActive)
                toCheck = activeBlessDict[level];
            else
                toCheck = negativeBlessDict[level];
            return toCheck[MathTool.GetRandom(toCheck.Count)];
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
                cache.FightLevelChange += config.FightLevelChange;
                cache.FightWinAddExp += config.FightWinAddExp;
                cache.FightWinAddHealth += config.FightWinAddHealth;
                cache.FightFailSubHealth += config.FightFailSubHealth;
                cache.FightFailSubMental += config.FightFailSubMental;
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
