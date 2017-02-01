using System.Drawing;
using ConfigDatas;
using TaleofMonsters.Controler.Loader;
using TaleofMonsters.Controler.Resource;

namespace TaleofMonsters.DataType.Blesses
{
    internal static class BlessBook
    {
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
    }
}
