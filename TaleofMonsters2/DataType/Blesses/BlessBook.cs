using System;
using System.Collections.Generic;
using System.Drawing;
using ConfigDatas;
using ControlPlus;
using NarlonLib.Math;
using TaleofMonsters.Controler.Loader;
using TaleofMonsters.Controler.Resource;
using TaleofMonsters.DataType.User;

namespace TaleofMonsters.DataType.Blesses
{
    internal static class BlessBook
    {
        private static Dictionary<int, List<int>> activeBlessDict = new Dictionary<int, List<int>>();
        private static Dictionary<int, List<int>> negativeBlessDict = new Dictionary<int, List<int>>();
        static BlessBook()
        {
            for (int i = 0; i < 4; i++)
            {
                BlessBook.activeBlessDict[i] = new List<int>();
                BlessBook.negativeBlessDict[i] = new List<int>();
            }
            foreach (var blessConfig in ConfigData.BlessDict.Values)
            {
                if (blessConfig.Type == 1)
                    BlessBook.activeBlessDict[blessConfig.Level].Add(blessConfig.Id);
                else
                    BlessBook.negativeBlessDict[blessConfig.Level].Add(blessConfig.Id);
            }
        }

        public static Image GetBlessImage(int id)
        {
            string fname = String.Format("Bless/{0}.PNG", ConfigData.BlessDict[id].Icon);
            if (!ImageManager.HasImage(fname))
            {
                Image image = PicLoader.Read("Bless", String.Format("{0}.PNG", ConfigData.BlessDict[id].Icon));
                ImageManager.AddImage(fname, image);
            }
            return ImageManager.GetImage(fname);
        }

        public static Image GetPreview(int key)
        {
            var config = ConfigData.GetBlessConfig(key);
            var lastTime = 0;
            if (UserProfile.InfoWorld.Blesses.ContainsKey(key))
                lastTime = UserProfile.InfoWorld.Blesses[key];
            TipImage tipData = new TipImage();
            tipData.AddTextNewLine(config.Name, config.Type == 1 ? "Green" : "Red", 20);
            tipData.AddLine(2);
            tipData.AddTextLines(config.Descript, "White", 15, true);
            tipData.AddTextNewLine(String.Format("剩余步数{0}", lastTime), "Wheat");
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
