using System;
using System.Drawing;
using ConfigDatas;
using ControlPlus;
using NarlonLib.Drawing;
using TaleofMonsters.Controler.Loader;
using TaleofMonsters.Controler.Resource;

namespace TaleofMonsters.DataType.Others
{
    public class DnaBook
    {
        public static Image GetDnaImage(int id)
        {
            var dnaConfig = ConfigData.GetPlayerDnaConfig(id);
            string fname = String.Format("Player/Dna/{0}.PNG", dnaConfig.Url);
            if (!ImageManager.HasImage(fname))
            {
                Image image = PicLoader.Read("Player.Dna", String.Format("{0}.PNG", dnaConfig.Url));
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
    }
}