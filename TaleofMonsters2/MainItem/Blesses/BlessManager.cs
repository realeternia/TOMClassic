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

        public static BlessUpdateMethod Update = null;

        public static void AddBless(int id, int time)
        {
            UserProfile.InfoWorld.Blesses[id] = time;
            if (Update != null)
            {
                Update();
            }
        }

        public static void RemoveBless(int id)
        {
            UserProfile.InfoWorld.Blesses.Remove(id);
            if (Update != null)
            {
                Update();
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

        public static uint GetSceneMove(int based)
        {
            foreach (var key in UserProfile.InfoWorld.Blesses.Keys)
            {
                var config = ConfigData.GetBlessConfig(key);
                based += config.MoveFoodChange;
            }
            return (uint)Math.Max(0,based);
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
