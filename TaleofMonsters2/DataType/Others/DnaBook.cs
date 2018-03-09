using System;
using System.Collections.Generic;
using System.Drawing;
using ConfigDatas;
using NarlonLib.Drawing;
using NarlonLib.Log;
using TaleofMonsters.Controler.Loader;
using TaleofMonsters.Controler.Resource;

namespace TaleofMonsters.DataType.Others
{
    public class DnaBook
    {
        private static Dictionary<string, int> dnaIdDict;
        public static int GetDnaId(string ename)
        {
            if (dnaIdDict == null)
            {
                dnaIdDict = new Dictionary<string, int>();
                foreach (var dnaConfig in ConfigData.PlayerDnaDict.Values)
                {
                    if (dnaIdDict.ContainsKey(dnaConfig.Ename))
                    {
                        NLog.Warn("GetDnaId key={0} exsited", dnaConfig.Ename);
                        continue;
                    }
                    dnaIdDict[dnaConfig.Ename] = dnaConfig.Id;
                }
            }
            return dnaIdDict[ename];
        }

        public static Image GetDnaImage(int id)
        {
            var dnaConfig = ConfigData.GetPlayerDnaConfig(id);
            string fname = string.Format("Player/Dna/{0}.PNG", dnaConfig.Url);
            if (!ImageManager.HasImage(fname))
            {
                Image image = PicLoader.Read("Player.Dna", string.Format("{0}.PNG", dnaConfig.Url));
                ImageManager.AddImage(fname, image);
            }
            return ImageManager.GetImage(fname);
        }

        public static Image GetPreview(int id)
        {
            var dnaConfig = ConfigData.GetPlayerDnaConfig(id);
            if (dnaConfig.Id <= 0)
                return DrawTool.GetImageByString("unknown", 100);

            ControlPlus.TipImage tipData = new ControlPlus.TipImage();
            tipData.AddTextNewLine(dnaConfig.Name, "White");
            tipData.AddTextNewLine(dnaConfig.Des, "Gray");

            return tipData.Image;
        }

        public static List<int> GetDnas(uint dnaInfo)
        {
            List<int> dataList = new List<int>();
            for (int i = 1; i <= 24; i++)
            {
                if ((dnaInfo & (uint)Math.Pow(2, i)) != 0)
                    dataList.Add(i);
            }
            return dataList;
        }

        public static int GetDnaEffect(string effectStr)
        {
            int val = 0;
            foreach (var chard in effectStr.Substring(3))
            {
                if (chard == '+')
                    val++;
                else
                    val--;
            }
            return val;
        }
    }
}