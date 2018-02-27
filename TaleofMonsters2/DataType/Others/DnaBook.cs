using System.Drawing;
using ConfigDatas;
using TaleofMonsters.Controler.Loader;
using TaleofMonsters.Controler.Resource;

namespace TaleofMonsters.DataType.Others
{
    public class DnaBook
    {
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
    }
}