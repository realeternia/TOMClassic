﻿using System.Collections.Generic;
using System.Drawing;
using ConfigDatas;
using ControlPlus;
using NarlonLib.Math;
using TaleofMonsters.Core.Loader;
using TaleofMonsters.Datas.User;
using TaleofMonsters.Tools;

namespace TaleofMonsters.Datas.Blesses
{
    internal static class BlessBook
    {
        private static Dictionary<string, int> blessNameDict = null;
        public static int GetBlessByName(string name)
        {
            if (blessNameDict == null)
            {
                blessNameDict = new Dictionary<string, int>();
                foreach (var blessConfig in ConfigData.BlessDict.Values)
                    blessNameDict.Add(blessConfig.Ename, blessConfig.Id);
            }
            int blessId;
            if (!blessNameDict.TryGetValue(name, out blessId))
            {
                throw new KeyNotFoundException("bless name not found " + name);
            }
            return blessId;
        }

        private static Dictionary<int, List<int>> activeBlessDict = new Dictionary<int, List<int>>();
        private static Dictionary<int, List<int>> negativeBlessDict = new Dictionary<int, List<int>>();
        static BlessBook()
        {
            for (int i = 0; i < 4; i++)
            {
                activeBlessDict[i] = new List<int>();
                negativeBlessDict[i] = new List<int>();
            }
            foreach (var blessConfig in ConfigData.BlessDict.Values)
            {
                if (!blessConfig.IsRandom)
                    continue;
                if (blessConfig.Type == (int)BlessTypes.Active)
                    activeBlessDict[blessConfig.Level].Add(blessConfig.Id);
                else if (blessConfig.Type == (int)BlessTypes.Negative)
                    negativeBlessDict[blessConfig.Level].Add(blessConfig.Id);
            }
        }

        public static Image GetBlessImage(int id)
        {
            string fname = string.Format("Bless/{0}.PNG", ConfigData.BlessDict[id].Icon);
            if (!ImageManager.HasImage(fname))
            {
                Image image = PicLoader.Read("Bless", string.Format("{0}.PNG", ConfigData.BlessDict[id].Icon));
                ImageManager.AddImage(fname, image);
            }
            return ImageManager.GetImage(fname);
        }

        public static Image GetPreview(int key)
        {
            var config = ConfigData.GetBlessConfig(key);
            var lastTime = UserProfile.InfoWorld.GetBlessTime(key);
            TipImage tipData = new TipImage(PaintTool.GetTalkColor);
            var color = "Gold";
            if (config.Type == (int) BlessTypes.Active)
                color = "Green";
            else if (config.Type == (int)BlessTypes.Negative)
                color = "Red";
            tipData.AddTextNewLine(config.Name, color, 20);
            tipData.AddLine(2);
            tipData.AddTextLines(config.Descript, "White", 15, true);
            tipData.AddTextNewLine(string.Format("剩余步数{0}", lastTime), "Lime");
            return tipData.Image;
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
    }
}
